using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace OrdersApi
{
    public class Settings
    {
        public const string FormatErrorMessageMissingEnvironmentVariables = "Cannot start application. Missing environment variables: {0}";
        public const string FormatErrorMessageInvalidEnvironmentVariables = "Cannot start application. Invalid environment variables: {0}";

        public Settings(IConfiguration configuration)
        {
            var declaredProperties =
                GetType().GetTypeInfo().DeclaredProperties.ToList();

            var missingSettings =
                (from propertyInfo in declaredProperties
                    where string.IsNullOrWhiteSpace(configuration[propertyInfo.Name])
                    select propertyInfo.Name).ToList();

            if (missingSettings.Any())
                throw new Exception(
                    string.Format(FormatErrorMessageMissingEnvironmentVariables, string.Join(", ", missingSettings)));

            var invalidSettings = new List<string>();

            foreach (var propertyInfo in declaredProperties)
            {
                var configValue = configuration[propertyInfo.Name];

                if (propertyInfo.PropertyType == typeof(bool))
                {
                    if (bool.TryParse(configValue, out var value))
                        propertyInfo.SetValue(this, value);
                    else
                        invalidSettings.Add(propertyInfo.Name);
                }

                else if (propertyInfo.PropertyType == typeof(DateTime))
                {
                    if(DateTime.TryParse(configValue, out var value))
                        propertyInfo.SetValue(this, value);
                    else
                        invalidSettings.Add(propertyInfo.Name);
                }

                else if (propertyInfo.PropertyType == typeof(Guid))
                {
                    if(Guid.TryParse(configValue, out var value))
                        propertyInfo.SetValue(this, value);
                    else
                        invalidSettings.Add(propertyInfo.Name);
                }

                else if (propertyInfo.PropertyType == typeof(int))
                {
                    if(int.TryParse(configValue, out var value))
                        propertyInfo.SetValue(this, value);
                    else
                        invalidSettings.Add(propertyInfo.Name);
                }

                else
                    propertyInfo.SetValue(this, configValue);
            }
            
            if (invalidSettings.Any())
                throw new Exception(
                    string.Format(FormatErrorMessageInvalidEnvironmentVariables, string.Join(", ", invalidSettings)));
        }

        public string EventStoreHost { get; set; }

        public int EventStorePort { get; set; }

        public string EventStoreUsername { get; set; }

        public string EventStorePassword { get; set; }

        public string EventStoreStream { get; set; }
        
        public string MongoConnectionString { get; set; }
    }
}