// App.xaml.cs
using CodeSnapshotter.Services;
using CodeSnapshotter.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;

namespace CodeSnapshotter
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; }
        public new static App Current => (App)Application.Current;

        public App()
        {
            Services = ConfigureServices();
            this.InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();
        }

        private Window m_window;

        private static IServiceProvider ConfigureServices()
        {
            ServiceCollection services = new ServiceCollection();

            // Services
            services.AddSingleton<IFilePickerService, FilePickerService>();
            services.AddSingleton<SnapshotService>();

            // ViewModels
            services.AddTransient<MainViewModel>();

            return services.BuildServiceProvider();
        }
    }
}