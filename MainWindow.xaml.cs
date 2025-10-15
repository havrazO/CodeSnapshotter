using CodeSnapshotter.Services;
using CodeSnapshotter.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;

namespace CodeSnapshotter
{
    public sealed partial class MainWindow : Window
    {
        public MainViewModel ViewModel { get; }

        public MainWindow()
        {
            this.InitializeComponent();
            this.Title = "Code Snapshotter";

            ViewModel = App.Current.Services.GetService<MainViewModel>();

            // Initialize the file picker service with the window handle
            IFilePickerService? filePickerService = App.Current.Services.GetService<IFilePickerService>();
            filePickerService.Initialize(this);
        }
    }
}