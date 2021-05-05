using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FileManager_AvaloniaUI_HY33SZ.Models;
using FileManager_AvaloniaUI_HY33SZ.ViewModels;
using System;

namespace FileManager_AvaloniaUI_HY33SZ.Views
{
    public class CustomViewWindow : Window
    {
        public static CustomViewModel VM;

        public CustomViewWindow()
        {
            InitializeComponent();
            this.AttachDevTools();
            InitVM();
        }


        private void InitVM()
        {
            if (VM == null)
            {
                VM = new CustomViewModel();
            }

            this.DataContext = VM;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }


        private void CustomViewWindow_Opened(object? sender, EventArgs e)
        {
            (this.DataContext as CustomViewModel).SaveCurrentProperties.Execute(null);
        }

        private void Apply_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var asd = (this.DataContext as CustomViewModel).Properties;
            this.Close(true);
        }

        private void Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var vm = this.DataContext as CustomViewModel;
            vm.Properties = null;
            vm.Properties = vm.OldProperties;
            
            this.Close(false);
        }
    }
}
