using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace DataAccessLibrary.Services
{
    public class BlobService
    {
        private static BlobContainerClient _containerClient { get; set; }
        private static BlobClient _blobClient { get; set; }

        private static StorageFolder _localAppData = ApplicationData.Current.LocalFolder;

        public static async Task<bool> InitStorageAsync()
        {
            try
            {
                BlobServiceClient blobServiceClient = new BlobServiceClient(Config.BlobConnectionString);

                try
                {
                    _containerClient = await blobServiceClient.CreateBlobContainerAsync(Config.BlobContainerName);
                    return true;
                }
                catch
                {
                    try
                    {
                        _containerClient = blobServiceClient.GetBlobContainerClient(Config.BlobContainerName);
                        return true;
                    }
                    catch 
                    {
                        return false;
                    }
                }
            } 
            catch 
            {
                return false;
            }
        }

        public static async Task StoreFileAsync(StorageFile file, string ticketId)
        {
            var localFile = await SaveFileLocally(file, ticketId);
            await UploadFileToAzure(localFile);
        }

        // Kopierar filen till lokal mapp för "lättare" tillgång
        // Döper filen till ticketId för enkel koppling
        private static async Task<StorageFile> SaveFileLocally(StorageFile file, string ticketId)
            => await file.CopyAsync(_localAppData, 
                                    ticketId + file.FileType,
                                    NameCollisionOption.ReplaceExisting);

        // Laddar upp till Azure för "global" tillgång
        private static async Task UploadFileToAzure(StorageFile file)
        {
            _blobClient = _containerClient.GetBlobClient(file.Name);

            using FileStream fileStream = File.OpenRead(file.Path);
            await _blobClient.UploadAsync(fileStream, true);
            fileStream.Close();
        }

        /// <summary>
        /// Deletes the file from Azure Blob Storage (using DeleteIfExistsAsync) and locally if found.
        /// </summary>
        public static async Task DeleteFileIfExistAsync(string fileName)
        {
            try
            {
                _blobClient = _containerClient.GetBlobClient(fileName);
                await _blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
            }
            catch
            {
                Debug.WriteLine($"{fileName} could not be found in Azure Blob Storage ({_containerClient.Uri})");
            }

            try
            {
                var _file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                await _file.DeleteAsync();
            }
            catch
            {
                Debug.WriteLine($"{fileName} could not be found in {ApplicationData.Current.LocalFolder.Path}");
            }
        }
        /// <summary>
        /// Checks local application folder for attachment file, and downloads it from Azure Blob Storage if not found.
        /// </summary>
        public static async Task DownloadFileIfNotExistAsync(string fileName)
        {
            try
            {
                var _file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
            }
            catch
            {
                try
                {
                    Debug.WriteLine($"{fileName} could not be not found in {ApplicationData.Current.LocalFolder} "
                                   + "- downloading from Azure Blob Storage.");

                    _blobClient = _containerClient.GetBlobClient(fileName);
                    BlobDownloadInfo download = await _blobClient.DownloadAsync();

                    using FileStream fileStream = File.OpenWrite($"{_localAppData.Path}\\{fileName}");
                    await download.Content.CopyToAsync(fileStream);
                    fileStream.Close();
                }
                catch
                {
                    Debug.WriteLine($"Could not download {fileName} from {_containerClient.Uri}");
                }

            }
        }

    }
}
