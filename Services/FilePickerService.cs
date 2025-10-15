using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace CodeSnapshotter.Services
{
    public class FilePickerService : IFilePickerService
    {
        private nint _windowHandle;

        public void Initialize(object window)
        {
            _windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);
        }

        public async Task<StorageFolder> PickFolderAsync()
        {
            FolderPicker folderPicker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.ComputerFolder
            };
            folderPicker.FileTypeFilter.Add("*");
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, _windowHandle);
            return await folderPicker.PickSingleFolderAsync();
        }

        public async Task<StorageFile> PickSaveFileAsync(bool isMarkdown)
        {
            FileSavePicker savePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };

            if (isMarkdown)
            {
                savePicker.FileTypeChoices.Add("Markdown File", new List<string>() { ".md" });
                savePicker.SuggestedFileName = "CodeSnapshot.md";
            }
            else
            {
                savePicker.FileTypeChoices.Add("Text File", new List<string>() { ".txt" });
                savePicker.SuggestedFileName = "CodeSnapshot.txt";
            }

            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, _windowHandle);
            return await savePicker.PickSaveFileAsync();
        }
    }
}