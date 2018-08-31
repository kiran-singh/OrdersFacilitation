using System;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using OrdersApi.Filters;
using OrdersApi.Middleware;
using OrdersApi.Models;
using OrdersApi.Services;
using OrdersApi.Validators;
using Serilog;
using Serilog.Events;
using Swashbuckle.AspNetCore.Swagger;
using ILogger = Serilog.ILogger;

namespace OrdersApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Settings = new Settings(configuration);
        }

        public Settings Settings { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(dispose: true));

            services.AddMvc(
                    options =>
                    {
                        options.Filters.Add<LogActionFilterAttribute>();
                        options.Filters.Add<ValidateModelAttribute>();
                    }
                )
                ;

            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new Info {Title = "Orders API", Version = "v1"}); });
            
            services.TryAddTransient<ICustomerService, CustomerService>();

            services.TryAddSingleton<ICustomSerializer, CustomSerializer>();

            services.TryAddSingleton<ICustomDataConverter, CustomDataConverter>();

            services.TryAddTransient<IEventStoreConnection>(serviceProvider =>
            {
                var connectionSettingsBuilder = ConnectionSettings.Create()
                    .SetDefaultUserCredentials(new UserCredentials(Settings.EventStoreUsername,
                        Settings.EventStorePassword));

                connectionSettingsBuilder.KeepReconnecting().LimitReconnectionsTo(1000);

                var connectionSettings = connectionSettingsBuilder.Build();

                var connectionString = string.Format("tcp://{0}:{1}@{2}:{3}", 
                                        connectionSettings.DefaultUserCredentials.Username,
                                        connectionSettings.DefaultUserCredentials.Password, 
                                        Settings.EventStoreHost,
                                        Settings.EventStorePort);

                var eventStoreConnection = EventStoreConnection.Create(connectionSettings, new Uri(connectionString));

                eventStoreConnection.ConnectAsync().Wait();

                return eventStoreConnection;
            });

            services.TryAddTransient<IEventService>(serviceProvider =>
                new EventService(serviceProvider.GetService<ILogger>(),
                    serviceProvider.GetService<IEventStoreConnection>(),
                    serviceProvider.GetService<ICustomDataConverter>(), 
                    Settings.EventStoreStream,
                    serviceProvider.GetService<ICustomSerializer>()));

            services.TryAddScoped<IMongoCollection<Customer>>(serviceProvider =>
                new MongoClient(Settings.MongoConnectionString).GetDatabase("local")
                    .GetCollection<Customer>($"{nameof(Customer)}s"));

            services.TryAddSingleton<ILogger>(new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger());
            
            services.TryAddSingleton<ILogWrapper, LogWrapper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseMvc();

            app.UseSwagger()
                .UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Orders API V1"); });
        }
    }
}