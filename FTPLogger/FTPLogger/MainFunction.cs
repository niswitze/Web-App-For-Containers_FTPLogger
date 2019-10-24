using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using FTPLogger.BusinessLogic;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FTPLogger
{
    public class MainFunction
    {
        private readonly ILogCapturer<List<string>> _logCapturer;

        public MainFunction(ILogCapturer<List<string>> logCapturer)
        {
            _logCapturer = logCapturer;

        }

        [FunctionName("MainFunction")]
        public async Task Run([TimerTrigger("0 * * * * *")]TimerInfo myTimer, ILogger log)
        {

            log.LogInformation($"Log capturer function started at: {DateTime.Now}");

            try
            {
                var logFileRequest = _logCapturer.ReturnAllLogFiles();

                await _logCapturer.StoreAllLogFilesAsync(logFileRequest);

            }
            catch (Exception e)
            {
                log.LogInformation($"AN EXCEPTION OCCURED: {e.Message?.ToString()}");
            }
            finally
            {
                log.LogInformation($"Log capturer function finished at: {DateTime.Now}");
            }        

           
        }
    }
}
