using Admin.Application.CommandHandlers;
using Admin.Infrastructure.Repositories;

//using Admin.Application.Repositories;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<uhesDapperContext>();
builder.Services.AddScoped<Repositories>();

// This works across all MediatR versions:
builder.Services.AddMediatR(typeof(QueryCommandHandler).Assembly);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// *** Add this line below to enable CORS middleware ***
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
