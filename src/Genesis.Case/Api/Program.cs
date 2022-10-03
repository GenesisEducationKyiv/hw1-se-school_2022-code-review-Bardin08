using System.Reflection;
using Core;
using Data;
using Extensions.Logger;
using Integrations.Crypto;
using Integrations.Notifications;
using Integrations.RabbitMq;
using Microsoft.OpenApi.Models;

namespace Api
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

            builder.Services.AddAutoMapper(typeof(Program).Assembly);

            builder.Services.AddCoreLogic(builder.Configuration);
            builder.Services.AddDataLayer(builder.Configuration);
            
            builder.Services.AddRabbitMq(builder.Configuration);
            builder.Services.AddCustomLogger();
            
            builder.Services.AddNotificationsIntegration(builder.Configuration);
            builder.Services.AddCryptoIntegration(builder.Configuration);

            builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Genesis Engineering School Case Task API",
                    Description = "An ASP.NET Core Web API for receiving BTC to UAH exchange rate, and notify functionality."
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
            });

            var app = builder.Build();

// Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapControllers();

            app.Run();
        }
    }
}
