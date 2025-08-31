using Cadastro;
using Cadastro.Commands;
using Microsoft.AspNetCore.Mvc;
using SimpleMediator.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IClienteService, ClienteService>();

// REGISTRO DO MEDIATOR

// Mais performatico
//builder.Services.AddSimpleMediator("Cadastro", "Crm", "Financeiro", "SharedKernel");

// Mais custoso - Más registra tudo automaticamente (Preferivel)
builder.Services.AddSimpleMediator();
//builder.Services.AddSimpleMediator(AppDomain.CurrentDomain.GetAssemblies());

// Mais trabalhoso
//services.AddSingleton<IMediator, Mediator>();
//services.AddTransient<IRequestHandler<CadastrarClienteCommand, string>, ClienteHandler>();
//services.AddTransient<IRequestHandler<ExcluirClienteCommand, bool>, ClienteHandler>();
//services.AddTransient<INotificationHandler<ClienteCadastradoEvent>, GestaoContaHandler>();
//services.AddTransient<INotificationHandler<ClienteCadastradoEvent>, NotificadorHandler>();
//services.AddTransient<INotificationHandler<ClienteExcluidoEvent>, GestaoContaHandler>();
//services.AddTransient<INotificationHandler<ClienteExcluidoEvent>, NotificadorHandler>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

app.UseHttpsRedirection();

string[] summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    WeatherForecast[] forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapPost("/cadastrar", async ([FromBody] CadastrarClienteCommand command, IClienteService service) =>
{
    string resultado = await service.CadastrarCliente(command);
    return Results.Ok(resultado);
}).WithName("CadastrarCliente")
  .WithTags("Clientes");

app.MapPost("/excluir", async ([FromBody] ExcluirClienteCommand command, IClienteService service) =>
{
    bool resultado = await service.ExcluirCliente(command);

    if (!resultado)
        return Results.BadRequest($"Problema ao excluir o cliente {command.Id}");

    return Results.Ok(command.Id);
}).WithName("ExcluirCliente")
  .WithTags("Clientes");

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}