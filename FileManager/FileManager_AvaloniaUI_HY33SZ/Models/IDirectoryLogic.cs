using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager_AvaloniaUI_HY33SZ.Models
{
    interface IDirectoryLogic
    {
        IEnumerable<string> CollectDrives();

        ObservableCollection<DirectoryEntry> CollectDirectoryEntries(string currentDirectory);

        ObservableCollection<DirectoryEntry> CollectOpenedTreeDir(ObservableCollection<DirectoryEntry> Entries,
           string CurrentDirectory, DirectoryEntry SelectedEntry);

        ObservableCollection<DirectoryEntry> CollectClosedTreeDir(ObservableCollection<DirectoryEntry> Entries,
           string CurrentDirectory, DirectoryEntry SelectedEntry);

        string PreviousDirectoryPath(string currentDirectory);

        void OpenEntry(DirectoryEntry entry);
    }
}
