// ViewModels/MainViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CodeSnapshotter.Models;
using CodeSnapshotter.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace CodeSnapshotter.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IFilePickerService _filePickerService;
        private readonly SnapshotService _snapshotService;
        private StorageFolder _selectedFolder;

        [ObservableProperty]
        private string selectedFolderPath = "Kein Verzeichnis ausgewählt";

        [ObservableProperty]
        private string includeExtensions = ".ts,.html,.scss,.css,.js,.json,.md,.xml,.gitignore,.env";

        [ObservableProperty]
        private string excludeDirectories = "node_modules, platforms, www, dist,.git,.angular,.vscode,.idea, bin, obj";

        [ObservableProperty]
        private string excludeFiles = "package-lock.json, yarn.lock";

        [ObservableProperty]
        private int outputFormatIndex = 0;

        [ObservableProperty]
        private string statusText = "Bereit.";

        [ObservableProperty]
        private bool isBusy = false;

        public MainViewModel(IFilePickerService filePickerService, SnapshotService snapshotService)
        {
            _filePickerService = filePickerService;
            _snapshotService = snapshotService;
        }

        [RelayCommand]
        private async Task SelectFolderAsync()
        {
            StorageFolder folder = await _filePickerService.PickFolderAsync();
            if (folder != null)
            {
                _selectedFolder = folder;
                SelectedFolderPath = folder.Path;
                StatusText = "Bereit zur Generierung des Snapshots.";
                GenerateSnapshotCommand.NotifyCanExecuteChanged();
            }
        }

        [RelayCommand]
        private async Task GenerateSnapshotAsync()
        {
            IsBusy = true;
            StatusText = "Verarbeite Dateien...";

            try
            {
                SnapshotConfig config = new SnapshotConfig
                {
                    IncludeExtensions = includeExtensions.Split(',').Select(s => s.Trim()).ToList(),
                    ExcludeDirectories = excludeDirectories.Split(',').Select(s => s.Trim()).ToList(),
                    ExcludeFiles = excludeFiles.Split(',').Select(s => s.Trim()).ToList(),
                    IsMarkdown = outputFormatIndex == 0,
                    StatusUpdateCallback = (message) => StatusText = message
                };

                string snapshotContent = await _snapshotService.CreateSnapshotAsync(_selectedFolder, config);

                StorageFile saveFile = await _filePickerService.PickSaveFileAsync(config.IsMarkdown);
                if (saveFile != null)
                {
                    StatusText = "Speichere Snapshot...";
                    await FileIO.WriteTextAsync(saveFile, snapshotContent);
                    StatusText = $"Snapshot erfolgreich gespeichert unter: {saveFile.Path}";
                }
                else
                {
                    StatusText = "Speichervorgang abgebrochen.";
                }
            }
            catch (Exception ex)
            {
                StatusText = $"Fehler: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        // This method is linked to the GenerateSnapshotCommand and controls when the button is enabled
        private bool CanGenerateSnapshot() => _selectedFolder != null && !IsBusy;
    }
}