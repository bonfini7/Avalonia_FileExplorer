using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FileManager_AvaloniaUI_HY33SZ.Models;
using FileManager_AvaloniaUI_HY33SZ.ViewModels;
using FileManager_AvaloniaUI_HY33SZ.Views;

namespace FileManager_AvaloniaUI_HY33SZ
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                   // DataContext = new ViewModel(new DirectoryLogic()),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
