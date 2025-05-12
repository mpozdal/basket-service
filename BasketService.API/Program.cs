using System.Reflection;
using BasketService.Application.Commands;
using BasketService.Application.Interfaces;
using BasketService.Infrastructure;
using BasketService.Infrastructure.ProductService;
using EventStore.Client;
using EventStore.ClientAPI;
using MediatR;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateBasketCommandHandler).Assembly));

builder.Services.AddHttpClient<IProductServiceClient, ProductServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("Services:Product")!);
});
builder.Services.AddSingleton(sp =>
{
    var settings = EventStoreClientSettings.Create("esdb://localhost:2113?tls=false");
    return new EventStoreClient(settings);
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.MapControllers();

app.Run();