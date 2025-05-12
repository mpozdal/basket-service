using System.Text;
using System.Text.Json;
using BasketService.Application.Interfaces;
using BasketService.Domain.Framework;
using EventStore.Client;

namespace BasketService.Infrastructure.Repositories;

public class AggregateRepository : IAggregateRepository
{
    private readonly EventStoreClient _eventStore;

    public AggregateRepository(EventStoreClient eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task SaveAsync<T>(T aggregate) where T : Aggregate
    {
        var events = aggregate.GetChanges()
            .Select(@event =>
                new EventData(
                    Uuid.NewUuid(),
                    @event.GetType().Name,
                    JsonSerializer.SerializeToUtf8Bytes(@event),
                    Encoding.UTF8.GetBytes(@event.GetType().AssemblyQualifiedName!)

                )
            )
            .ToArray();

        if (!events.Any())
            return;

        var streamName = GetStreamName(aggregate, aggregate.Id);

        await _eventStore.AppendToStreamAsync(
            streamName,
            StreamState.Any,
            events
        );
    }

    public async Task<T> LoadAsync<T>(Guid aggregateId) where T : Aggregate, new()
    {
        if (aggregateId == Guid.Empty)
            throw new ArgumentNullException(nameof(aggregateId));

        var aggregate = new T();
        var streamName = GetStreamName(aggregate, aggregateId);
        var events = new List<object>();

        var result = _eventStore.ReadStreamAsync(
            Direction.Forwards,
            streamName,
            StreamPosition.Start
        );

        await foreach (var resolvedEvent in result)
        {
            var metadata = Encoding.UTF8.GetString(resolvedEvent.Event.Metadata.ToArray()).Trim('"');
            
            var eventType = Type.GetType(metadata);

            if (eventType is null)
                throw new InvalidOperationException($"Cannot resolve type: {metadata}");


            var data = JsonSerializer.Deserialize(
                resolvedEvent.Event.Data.Span,
                eventType
            );

            if (data is not null)
                events.Add(data);
        }

        if (events.Count > 0)
        {
            aggregate.Load(
                (long)events.Count - 1,
                events.ToArray()
            );
        }

        return aggregate;
    }

    private string GetStreamName<T>(T type, Guid aggregateId) =>
        $"{type.GetType().Name}-{aggregateId}";
}
