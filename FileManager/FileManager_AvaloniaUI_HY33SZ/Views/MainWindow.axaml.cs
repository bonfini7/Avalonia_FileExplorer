using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Converters;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using Avalonia.Styling;
using FileManager_AvaloniaUI_HY33SZ.Models;
using FileManager_AvaloniaUI_HY33SZ.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace FileManager_AvaloniaUI_HY33SZ.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Initialize the datacontextes and the control events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Opened(object? sender, System.EventArgs e)
        {
            this.FindControl<DockPanel>("left_fileList").DataContext = new ViewModel();
            this.FindControl<DockPanel>("right_fileList").DataContext = new ViewModel();

            this.FindControl<ComboBox>("cb_left").SelectionChanged += ComboBox_SelectionChanged;
            this.FindControl<ComboBox>("cb_right").SelectionChanged += ComboBox_SelectionChanged;

            lastSelectedButton = this.FindControl<Button>("brief");
        }


        /// <summary>
        /// It modifies the directory entry based on the user's choice.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Listbox_DoubleTapped(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            DockPanel currentPanel = GetCurrentDockPanel(sender);          
            if (currentPanel != null)
            {
                (currentPanel.DataContext as ViewModel).SelectEntryCommand.Execute(null);
            }

            if (!(currentPanel.DataContext as ViewModel).TreeViewMode)
                ValidateGoBackButtonVisibility(currentPanel);
        }

        /// <summary>
        /// Selection changed means the drive changed. It modifies the directory entry based on the user's choice.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (sender == null) return;

            DockPanel currentPanel = GetCurrentDockPanel(sender);
            if (currentPanel != null)
            {
                var selectedDrive = (sender as ComboBox).SelectedItem;

                if (selectedDrive != null)
                {
                    var vm = (currentPanel.DataContext as ViewModel);
                    vm.SelectedEntry = new DirectoryEntry(selectedDrive.ToString(), true);
                    vm.SelectEntryCommand.Execute(null);
                }

                ValidateGoBackButtonVisibility(currentPanel);
            }             
        }

        /// <summary>
        /// Returns to the current directory's parent folder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoBackButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender == null) return;
            
            DockPanel currentPanel = GetCurrentDockPanel(sender);
            if (currentPanel != null)
            {
                (currentPanel.DataContext as ViewModel).GoBackCommand.Execute(null);
                ValidateGoBackButtonVisibility(currentPanel);
            }                        
        }

        private void BriefViewClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            SetButtonSelected(sender as Button);
            (this.FindControl<DockPanel>("left_fileList").DataContext as ViewModel).TreeViewMode = false;
            (this.FindControl<DockPanel>("right_fileList").DataContext as ViewModel).TreeViewMode = false;

            var DataTemplate = (DataTemplate)Resources["BriefTemplate"];
            var ControlTemplate = (ControlTemplate)Resources["FullControlTemplate"];
            var left = this.FindControl<ListBox>("left_listbox");
            var right = this.FindControl<ListBox>("right_listbox");

            left.ItemTemplate = DataTemplate;
            left.Template = ControlTemplate;
            right.ItemTemplate = DataTemplate;
            right.Template = ControlTemplate;

            UpdateLists();
        }

        private void FullViewClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            SetButtonSelected(sender as Button);
            (this.FindControl<DockPanel>("left_fileList").DataContext as ViewModel).TreeViewMode = false;
            (this.FindControl<DockPanel>("right_fileList").DataContext as ViewModel).TreeViewMode = false;

            var DataTemplate = (DataTemplate)Resources["FullTemplate"];
            var ControlTemplate = (ControlTemplate)Resources["FullControlTemplate"];
            var left = this.FindControl<ListBox>("left_listbox");
            var right = this.FindControl<ListBox>("right_listbox");

            left.ItemTemplate = DataTemplate;
            left.Template = ControlTemplate;
            right.ItemTemplate = DataTemplate;
            right.Template = ControlTemplate;

            UpdateLists();
        }

        private void TreeViewClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            SetButtonSelected(sender as Button);
            HideGoBackButtons();

            (this.FindControl<DockPanel>("left_fileList").DataContext as ViewModel).TreeViewMode = true;
            (this.FindControl<DockPanel>("right_fileList").DataContext as ViewModel).TreeViewMode = true;

            var DataTemplate = (DataTemplate)Resources["BriefTemplate"];
            var ControlTemplate = (ControlTemplate)Resources["FullControlTemplate"];
            var left = this.FindControl<ListBox>("left_listbox");
            var right = this.FindControl<ListBox>("right_listbox");

            left.ItemTemplate = DataTemplate;
            left.Template = ControlTemplate;
            right.ItemTemplate = DataTemplate;
            right.Template = ControlTemplate;

            UpdateLists();
        }

        private async void CustomViewClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var left = this.FindControl<ListBox>("left_listbox");
            var right = this.FindControl<ListBox>("right_listbox");

            CustomViewWindow CustomWindow = new CustomViewWindow();
            var res = await CustomWindow.ShowDialog<bool>(this);
            if (res == true)
            {
                (this.FindControl<DockPanel>("left_fileList").DataContext as ViewModel).TreeViewMode = false;
                (this.FindControl<DockPanel>("right_fileList").DataContext as ViewModel).TreeViewMode = false;

                left.ItemTemplate = GetCustomDataTemplate(CustomViewWindow.VM.Properties);
                right.ItemTemplate = GetCustomDataTemplate(CustomViewWindow.VM.Properties);

                //TODO: Here is the ControlTemplate that doesn't work:
                left.Template = GetCustomControlTemplate(CustomViewWindow.VM.Properties); 
                SetButtonSelected(sender as Button);

                UpdateLists();
            }          
        }

        private IDataTemplate GetCustomDataTemplate(ViewProperties props)
        {
            Func<DirectoryEntry, INameScope, IControl> build = (x,y) =>
            {
                var mainDockpanel = new DockPanel()
                {                  
                    LastChildFill = true,
                };
              
                var secondaryGrid = new Grid()
                {
                    Background = x.IsDir ? Brushes.LightYellow : Brushes.White,
                };
                DockPanel.SetDock(secondaryGrid, Dock.Top);

                int columnCount = 0;
                foreach (var item in props.GetType().GetProperties())
                {
                    if (item.Name == "Name" || item.Name == "Extension" || (bool)item.GetValue(props) == true)
                    {
                        secondaryGrid.ColumnDefinitions.Add(new ColumnDefinition() { SharedSizeGroup = $"col{columnCount}" });
                        var currentChild = new TextBlock()
                        {
                            [!TextBlock.TextProperty] = new Binding(item.Name),
                        };
                        Grid.SetColumn(currentChild, columnCount++);

                        secondaryGrid.Children.Add(currentChild);
                    }
                }              
                mainDockpanel.Children.Add(secondaryGrid);

                return mainDockpanel;       
            
            };
            return new FuncDataTemplate<DirectoryEntry>(build);
        }

        

        private IControlTemplate GetCustomControlTemplate(ViewProperties props)
        {          
            Func<ITemplatedControl, INameScope, IControl> build = (x, y) =>
            {
                var mainDockpanel = new DockPanel()
                {
                    LastChildFill = true,
                };

                var secondaryGrid = new Grid()
                {
                   Height = 30,
                };
                DockPanel.SetDock(secondaryGrid, Dock.Top);

                int columnCount = 0;
                foreach (var item in props.GetType().GetProperties())
                {
                    if ((bool)item.GetValue(props) == true)
                    {
                        secondaryGrid.ColumnDefinitions.Add(new ColumnDefinition() { SharedSizeGroup = $"col{columnCount}" });
                        var currentChild = new Label()
                        {
                            Content = item.Name,
                        };
                        Grid.SetColumn(currentChild, columnCount++);
                        secondaryGrid.Children.Add(currentChild);
                    }
                }
                mainDockpanel.Children.Add(secondaryGrid);

                mainDockpanel.Children.Add(
                    new ScrollViewer()
                    {          
                        Content = new ItemsPresenter()
                        {
                          
                        }                        
                    });

                return mainDockpanel;
            };

            return new FuncControlTemplate(build);               
        }

        private void UpdateLists()
        {
            var left_vm = this.FindControl<DockPanel>("left_fileList").DataContext as ViewModel;
            var right_vm = this.FindControl<DockPanel>("right_fileList").DataContext as ViewModel;

            left_vm.CurrentDirectory = this.FindControl<ComboBox>("cb_left").SelectedItem.ToString();
            right_vm.CurrentDirectory = this.FindControl<ComboBox>("cb_right").SelectedItem.ToString();

            left_vm.ViewChanged.Execute(null);
            right_vm.ViewChanged.Execute(null);
        }

        Button lastSelectedButton;
        private void SetButtonSelected(Button button)
        {
            if (lastSelectedButton != null)
                lastSelectedButton.Background = Brushes.LightGray;

            button.Background = Brushes.DodgerBlue;
            lastSelectedButton = button;
        }

        /// <summary>
        /// The go back button is only visible if the current directory is NOT a root folder.
        /// This method checks if the button's visibility is needed or not.
        /// </summary>
        /// <param name="currentPanel">The parent panel's reference.</param>
        private void ValidateGoBackButtonVisibility(DockPanel currentPanel)
        {          
            // the namings are la_left_fileList And la_right_fileList
            // only the middle word is different so we can find it based on the currentPanel.
            string leftOrRight = currentPanel.Name.Replace("la_", "").Replace("_fileList", "");

            Label PathLabel = currentPanel.FindControl<Label>($"la_{leftOrRight}_path");
            Button GoBackButton = currentPanel.FindControl<Button>($"bt_{leftOrRight}_goback");

            if (PathLabel == null || PathLabel.Content == null) return;

            bool IsRootDir = System.IO.Directory.GetParent( PathLabel.Content.ToString() ) == null;

            if (IsRootDir && GoBackButton.IsVisible != false) // If the current directory is a root folder, we cannot go back
                GoBackButton.IsVisible = false;
            else if (!IsRootDir && GoBackButton.IsVisible != true)
                GoBackButton.IsVisible = true;

        }

        private void HideGoBackButtons()
        {
            var left_bt = this.FindControl<Button>("bt_left_goback");
            var right_bt = this.FindControl<Button>("bt_right_goback");

            left_bt.IsVisible = false;
            right_bt.IsVisible = false;
        }

        /// <summary>
        /// Returns the top parent with the type of DockPanel
        /// </summary>
        /// <param name="control">Child control that we want to identify it's parent</param>
        /// <returns></returns>
        private DockPanel GetCurrentDockPanel(object control)
        {
            DockPanel result = new DockPanel();
            if (control == null) return result;

            IControl current = (control as IControl).Parent;              
            while (current.GetType() == typeof(DockPanel))
            {
                result = (DockPanel)current;
                current = current.Parent;
            }           
            return result;
        }
    }
}
