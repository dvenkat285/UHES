using Admin.Infrastructure.Shared.Interfaces.Gateway;
using Admin.Infrastructure.Shared.Services.Gateway;
using Admin.MVCWebUI.MappingProfiles;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient<IHESApiGatewayService, HESApiGatewayService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5000/");

    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());



// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=SlabsAndTariffs}/{action=SlabsAndTariffsList}/{id?}");
app.MapControllerRoute(
    name: "mvc",
    pattern: "{controller}/{action}/{id?}");


app.Run();
