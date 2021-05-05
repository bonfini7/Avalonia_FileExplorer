using FileManager_AvaloniaUI_HY33SZ.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileManager_AvaloniaUI_HY33SZ.ViewModels
{
    class ViewModel : ViewModelBase
    {
        IDirectoryLogic logic;

        public bool TreeViewMode = false;

        private ObservableCollection<DirectoryEntry> entries;
        public ObservableCollection<DirectoryEntry> Entries { get => entries; set => Set(ref entries,value); }

        private DirectoryEntry selectedEntry;
        public DirectoryEntry SelectedEntry { get => selectedEntry; set => Set(ref selectedEntry, value); }

        private string currentDirectory;
        public string CurrentDirectory { get => currentDirectory; set => Set(ref currentDirectory, value); }

        public List<string> GetDrives { get; private set; }

        //Command(s)

        public ICommand SelectEntryCommand { get; private set; }
        public ICommand GoBackCommand { get; private set; }
        public ICommand ViewChanged { get; private set; }

        //Constructor(s)

        public ViewModel(IDirectoryLogic logic)
        {
            this.logic = logic;              
            this.GetDrives = this.logic.CollectDrives().ToList();
            this.CurrentDirectory = this.GetDrives[0];
            this.Entries = this.logic.CollectDirectoryEntries(this.CurrentDirectory);

            SelectEntryCommand = new RelayCommand(() =>
            {
                if (selectedEntry == null) return;

                if (!SelectedEntry.IsDir)
                {
                    this.logic.OpenEntry(SelectedEntry);
                    return;
                }

                if(SelectedEntry.Opened)
                {
                    GoBackCommand.Execute(null);
                    return;
                }

                this.CurrentDirectory = SelectedEntry.Path;
                Entries.First(t => t.Path == this.CurrentDirectory).Opened = true;

                if (!TreeViewMode)
                {                   
                    this.Entries = this.logic.CollectDirectoryEntries(this.CurrentDirectory);
                }
                else
                {
                    this.Entries = this.logic.CollectOpenedTreeDir(Entries, CurrentDirectory, SelectedEntry);
                }
                    
            }
            , true);

            GoBackCommand = new RelayCommand(() =>
            {
                var thisWasClosed = this.currentDirectory;
                this.CurrentDirectory = this.logic.PreviousDirectoryPath(this.CurrentDirectory);

                if (!TreeViewMode)
                {
                    this.Entries = this.logic.CollectDirectoryEntries(this.CurrentDirectory);
                    Entries.First(t => t.Path == thisWasClosed).Opened = false;
                }
                else
                {
                    this.Entries = this.logic.CollectClosedTreeDir(Entries, CurrentDirectory, SelectedEntry);
                }
                
            });

            ViewChanged = new RelayCommand(() =>
            {
                this.Entries = this.logic.CollectDirectoryEntries(this.CurrentDirectory);               
            });
        }



        public ViewModel() :this(new DirectoryLogic())
        { }  
    }
}
