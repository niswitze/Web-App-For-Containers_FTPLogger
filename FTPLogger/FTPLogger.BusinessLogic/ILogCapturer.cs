using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FTPLogger.BusinessLogic
{
    public interface ILogCapturer<FileType>
    {

        FileType ReturnAllLogFiles();

        Task StoreAllLogFilesAsync(FileType allFiles);

        string RemoveAllLogFiles(FileType allFiles);
    }
}
