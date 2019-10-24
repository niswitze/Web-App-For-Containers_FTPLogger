using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;

namespace FTPLogger.BusinessLogic
{
    public class FTPCapturer<FileType> : ILogCapturer<FileType>
                                        where FileType : List<string>
    {

        private readonly string userName = Environment.GetEnvironmentVariable("ftpUserName");
        private readonly string passWord = Environment.GetEnvironmentVariable("passWord");
        private readonly string ftpSite = Environment.GetEnvironmentVariable("ftpSite");
        private readonly string containerName = Environment.GetEnvironmentVariable("containerName");

        private readonly ILogger<FTPCapturer<FileType>> _logger;
        private readonly CloudStorageAccount _cloudStorageAccount;

        public FTPCapturer(ILogger<FTPCapturer<FileType>> logger, CloudStorageAccount cloudStorageAccount)
        {
            _logger = logger;
            _cloudStorageAccount = cloudStorageAccount;
        }

        public string RemoveAllLogFiles(FileType allFiles)
        {
            throw new NotImplementedException();
        }

        public FileType ReturnAllLogFiles()
        {
            var fileNames = new List<string>();

            FtpWebRequest request = ConfigureFtpWebRequest(null);
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
               fileNames = reader.ReadToEnd().
                                              Split(new[] { Environment.NewLine }, StringSplitOptions.None).
                                              Where(x => x.Contains(".log")).
                                              ToList();
            }

            return fileNames as FileType;

        }

        public async Task StoreAllLogFilesAsync(FileType allFiles)
        {
           
            var cloudBlobClient = _cloudStorageAccount.CreateCloudBlobClient();
            
            await cloudBlobClient.GetContainerReference(containerName).CreateIfNotExistsAsync();

            var cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);
            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference("DummyFileName");

            foreach (var file in allFiles)
            {
                try
                {
                    var content = ReadAllFileContent(file);
                    cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(file);
                    await cloudBlockBlob.UploadFromStreamAsync(content);

                    content.Close();
                }
                catch (Exception e)
                {
                    _logger.LogInformation($"AN EXCEPTION OCCURED: {e.Message?.ToString()} File {file} could not be processed.");
                }   
            }
        }

        private FtpWebRequest ConfigureFtpWebRequest(string fileName)
        {
            if (fileName != null)
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"{ftpSite}/{fileName}");

                request.Credentials = new NetworkCredential(userName, passWord);

                request.KeepAlive = false;
                request.UseBinary = true;
                request.UsePassive = true;
                request.EnableSsl = true;

                request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);

                return request;
            }
            else
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpSite);

                request.Credentials = new NetworkCredential(userName, passWord);

                request.KeepAlive = false;
                request.UseBinary = true;
                request.UsePassive = true;
                request.EnableSsl = true;

                request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);

                return request;
            }
        }

        private Stream ReadAllFileContent(string fileName)
        {
            FtpWebRequest request = ConfigureFtpWebRequest(fileName);

            request.Method = WebRequestMethods.Ftp.DownloadFile;

            var responseStream = request.GetResponse().GetResponseStream();

            return responseStream;
        }

    }
}
