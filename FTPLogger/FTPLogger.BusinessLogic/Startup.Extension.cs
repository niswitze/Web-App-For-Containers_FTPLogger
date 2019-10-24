using Microsoft.Azure.Storage;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace FTPLogger.BusinessLogic.StartupExtension
{
    public static class StorageExtension 
    {
        /// <summary>
        /// Startup extension to register all services   
        /// </summary>
        public static IServiceCollection RegisterAllServices(
            this IServiceCollection services)
        {

            services.AddSingleton<ILogCapturer<List<string>>, FTPCapturer<List<string>>>();

            CloudStorageAccount.TryParse(Environment.GetEnvironmentVariable("AzureWebJobsStorage")?.ToString(), out CloudStorageAccount cloudStorageAccount);

            if (cloudStorageAccount == null)
            {
                throw new NullReferenceException("Cloud Storage Account object could not be created. Please check to" +
                                                 "ensure the app setting for AzureWebJobsStorage has been set");
            }

            services.AddSingleton(cloudStorageAccount);
            return services;
        }
    }
}
