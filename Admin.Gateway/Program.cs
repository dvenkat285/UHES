using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add Ocelot configuration from ocelot.json
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Register Ocelot services
builder.Services.AddOcelot();

var app = builder.Build();

app.UseHttpsRedirection();

// Use Ocelot middleware
Console.WriteLine("Ocelot is starting...");
await app.UseOcelot();
Console.WriteLine("Ocelot is running...");

app.Run();
