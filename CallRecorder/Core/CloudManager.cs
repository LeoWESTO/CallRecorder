using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YandexDisk.Client.Http;
using YandexDisk.Client;
using YandexDisk.Client.Clients;
using System.IO;
using File = System.IO.File;
using YandexDisk.Client.Protocol;

namespace CallRecorder.Core
{
    public static class CloudManager
    {
        public static void UploadFile(FileInfo file)
        {
            try
            {
                Task.Run(() =>
                {
                    IDiskApi diskApi = new DiskHttpApi(RefreshToken());
                    Utils.Log($"Отправка файла {file.FullName} на сервер...");

                    diskApi.Files.UploadFileAsync(
                        path: $"/Leads/{file.Name}",
                        overwrite: true,
                        localFile: file.FullName,
                        cancellationToken: CancellationToken.None);
                });
                
            }
            catch(Exception ex) { Utils.Log(ex.Message); }
        }
        public static async Task RefreshCloudAsync()
        {
            IDiskApi diskApi = new DiskHttpApi(RefreshToken());
            Resource leadsResourceDescription = await diskApi.MetaInfo.GetInfoAsync(new ResourceRequest
            {
                Path = "/Leads",
            }, CancellationToken.None);

            IEnumerable<Resource> allFilesInFolder = leadsResourceDescription.Embedded.Items.Where(item => item.Type == ResourceType.File);

            var cloudFiles = allFilesInFolder.Select(file => file.Name);
            foreach (var localFile in new DirectoryInfo("Records").GetFiles())
            {
                if (!cloudFiles.Contains(localFile.Name))
                {
                    UploadFile(localFile);
                }
            }
        }
        private static string RefreshToken()
        {
            string token = string.Empty;
            try
            {
                token = File.ReadAllText("token.txt");
            }
            catch (Exception ex) { Utils.Log(ex.Message); }

            return token;
        }
    }
}
