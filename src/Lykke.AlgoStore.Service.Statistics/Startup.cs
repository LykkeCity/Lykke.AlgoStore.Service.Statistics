using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using JetBrains.Annotations;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.Security.InstanceAuth;
using Lykke.AlgoStore.Service.Statistics.Filters;
using Lykke.AlgoStore.Service.Statistics.Settings;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Sdk;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.AlgoStore.Service.Statistics
{
    [UsedImplicitly]
    public class Startup
    {
        private readonly LykkeSwaggerOptions _swaggerOptions = new LykkeSwaggerOptions
        {
            ApiTitle = "Algo Store Statistics API",
            ApiVersion = "v1"
        };

        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {                                   
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.SwaggerOptions = _swaggerOptions;

                options.Logs = logs =>
                {
                    logs.AzureTableName = "AlgoStoreStatisticsLog";
                    logs.AzureTableConnectionStringResolver = settings => settings.AlgoStoreStatisticsService.Db.LogsConnectionString;

                    // TODO: You could add extended logging configuration here:
                    /* 
                    logs.Extended = extendedLogs =>
                    {
                        // For example, you could add additional slack channel like this:
                        extendedLogs.AddAdditionalSlackChannel("Statistics", channelOptions =>
                        {
                            channelOptions.MinLogLevel = LogLevel.Information;
                        });
                    };
                    */
                };

                // TODO: Extend the service configuration
                
                options.Extend = (sc, settings) =>
                {
                    sc.AddInstanceAuthentication(settings.CurrentValue.AlgoStoreStatisticsService.StatisticsServiceAuth);
                };

                //Extended Swagger configuration
                options.Swagger = swagger =>
                {
                    swagger.OperationFilter<ApiKeyHeaderOperationFilter>();
                };
                
            });
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app)
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<AutoMapperModelProfile>();
                //cfg.AddProfile<Services.AutoMapperProfile>();
                //cfg.AddProfile<AzureRepositories.AutoMapperProfile>();
            });

            Mapper.AssertConfigurationIsValid();

            app.UseAuthentication();

            app.UseLykkeConfiguration(options =>
            {
                options.SwaggerOptions = _swaggerOptions;

                // TODO: Configure additional middleware for eg authentication or maintenancemode checks
                /*
                options.WithMiddleware = x =>
                {
                    x.UseMaintenanceMode<AppSettings>(settings => new MaintenanceMode
                    {
                        Enabled = settings.MaintenanceMode?.Enabled ?? false,
                        Reason = settings.MaintenanceMode?.Reason
                    });
                    x.UseAuthentication();
                };
                */

                options.DefaultErrorHandler = ex =>
                {
                    string errorMessage;

                    switch (ex)
                    {
                        case InvalidOperationException ioe:
                            errorMessage = $"Invalid operation: {ioe.Message}";
                            break;
                        case ValidationException ve:
                            errorMessage = $"Validation error: {ve.Message}";
                            break;
                        default:
                            errorMessage = "Technical problem";
                            break;
                    }

                    return ErrorResponse.Create(errorMessage);
                };
            });

        }
    }
}
