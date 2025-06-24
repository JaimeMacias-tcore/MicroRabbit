using MicroRabbit.Infra.IoC;
using MicroRabbit.Transfer.Data.Context;
using MicroRabbit.Transfer.Domain.EventHandlers;
using MicroRabbit.Transfer.Domain.Events;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<TransferDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TransferDbConnection")));


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Transfer MicroService",
        Version = "v1"
    });
});

builder.Services.AddMediatR(opts =>
{
    opts.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

DependencyContainer.RegisterServices(builder.Services);
DependencyContainer.RegisterTransferServices(builder.Services);

var app = builder.Build();

//Configure EventBus
var eventBus = app.Services.GetRequiredService<MicroRabbit.Domain.Core.Bus.IEventBus>();
await eventBus.Subscribe<TransferCreatedEvent, TransferEventHandler>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Transfer MicroService V1");
});

app.Run();
