using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Tewr.Blazor.FileReader;
using Atles.Client.Components.Shared;

namespace Atles.Client.Shared
{
    public abstract class FileManagerComponent : SharedComponentBase
    {
        [Parameter] public string FileInputId { get; set; }
        [Parameter] public string FileInputAccept { get; set; } = ".zip";

        [Inject] public IFileReaderService FileReaderService { get; set; }

        protected ElementReference FileInput;

        protected string Status = string.Empty;

        private IEnumerable<IFileReference> _selectedFiles;

        protected async Task HandleSelectedAsync()
        {
            _selectedFiles = await FileReaderService.CreateReference(FileInput).EnumerateFilesAsync();
        }

        protected async Task UploadFilesAsync()
        {
            Status = null;

            foreach (var file in _selectedFiles)
            {
                if (file == null) continue;

                var fileInfo = await file.ReadFileInfoAsync();

                using (var ms = await file.CreateMemoryStreamAsync(4 * 1024))
                {
                    var content = new MultipartFormDataContent();
                    content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
                    content.Add(new StreamContent(ms, Convert.ToInt32(ms.Length)), "file", fileInfo.Name);

                    await ApiService.PostAsync("api/admin/themes/upload-theme-file", content);

                    Status = $"Finished uploading {fileInfo.Size} bytes from {fileInfo.Name}";
                }
            }
        }
    }
}