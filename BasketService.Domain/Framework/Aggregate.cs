namespace BasketService.Domain.Framework;

public abstract class Aggregate
{
    readonly IList<object> _changes = new List<object>();
    public Guid Id { get; protected set; }
    public static long Version { get; protected set; } = -1;
    protected abstract void When(object @event);

    public void Apply(object @event)
    {
        When(@event);
        _changes.Add(@event);
    }

    public void Load(long version, IEnumerable<object> history)
    {
        Version = version;
        foreach (var item in history)
        {
            When(item);
        }
    }
    
    public object[] GetChanges() => _changes.ToArray();
    public void ClearChanges() => _changes.Clear();

}