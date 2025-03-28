using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stripe;
using System;

var builder = WebApplication.CreateBuilder(args);



// ✅ Stripe API Anahtarını `appsettings.json` İçinden Al
var stripeSecretKey = builder.Configuration["Stripe:SecretKey"];
if (string.IsNullOrEmpty(stripeSecretKey))
{
    
    throw new Exception("⚠ Stripe API anahtarı bulunamadı! `appsettings.json` dosyanı kontrol et.");
}
StripeConfiguration.ApiKey = stripeSecretKey;

// ✅ Servisleri Bağımlılıklar İçin Ekleyelim
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<StripeService>();

// ✅ CORS Ayarları (Frontend bağlanacaksa)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// ✅ Geliştirme Ortamında Swagger Kullan
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();

app.Run();