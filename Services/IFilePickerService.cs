using System.Threading.Tasks;
using Windows.Storage;

namespace CodeSnapshotter.Services
{
    public interface IFilePickerService
    {
        void Initialize(object window);
        Task<StorageFolder> PickFolderAsync();
        Task<StorageFile> PickSaveFileAsync(bool isMarkdown);
    }
}