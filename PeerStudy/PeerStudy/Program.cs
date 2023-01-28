using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PeerStudy;
using PeerStudy.Infrastructure.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.RegisterDbContext(builder.Configuration.GetConnectionString("DBConnectionString"));

builder.Services.RegisterServices();
builder.Services.RegisterBlazorComponentLibraries();

var app = builder.Build();


var serviceProvider = builder.Services.BuildServiceProvider();
var service = serviceProvider.GetService<IDatabaseSeedingService>();

service?.InsertTeachers();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
