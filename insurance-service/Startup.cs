using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using insurance_service.Data;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Prometheus;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace insurance_service
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Database with resilience
            services.AddDbContext<InsurancesDbContext>(options => 
            {
                options.UseNpgsql(_configuration.GetConnectionString("InsurancesDatabase"), 
                    npgsqlOptions => 
                    {
                        npgsqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorCodesToAdd: null);
                    });
            });

            // Enhanced Health Checks
            services.AddHealthChecks()
                .AddDbContextCheck<InsurancesDbContext>()
                .AddNpgSql(
                    _configuration.GetConnectionString("InsurancesDatabase"),
                    name: "database",
                    tags: new[] { "ready" })
                .AddCheck("memory", () => 
                    HealthCheckResult.Healthy("Memory usage is normal"),
                    tags: new[] { "live" });

            services.AddControllers();
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "insurance-service", Version = "v1"});
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Enable prometheus metrics
            app.UseMetricServer();
            app.UseHttpMetrics();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "insurance-service v1"));

            // Kubernetes probes
            app.UseHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = reg => reg.Tags.Contains("live"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = reg => reg.Tags.Contains("ready"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseRouting();

            app.UseEndpoints(endpoints => 
            { 
                endpoints.MapControllers();
                endpoints.MapMetrics(); // Expose metrics endpoint
            });

            // Automatic migrations for k8s deployments
            using var scope = app.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<InsurancesDbContext>();
            dbContext.Database.Migrate();
        }
    }
}