using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager_AvaloniaUI_HY33SZ.Models
{
    class DirectoryLogic : IDirectoryLogic
    {


        public ObservableCollection<DirectoryEntry> CollectDirectoryEntries(string currentDirectory)
        {
            ObservableCollection<DirectoryEntry> entries = new ObservableCollection<DirectoryEntry>();

            try
            {
                Directory.GetDirectories(currentDirectory).ToList().ForEach(
                     t => entries.Add(new DirectoryEntry(t, true)));

                Directory.GetFiles(currentDirectory).ToList().ForEach(
                     t => entries.Add(new DirectoryEntry(t, false)));
            }
            catch (UnauthorizedAccessException) { }


            return entries;
        }

        public IEnumerable<string> CollectDrives()
        {
           return DriveInfo.GetDrives().Select(t=>t.Name);
        }

        public ObservableCollection<DirectoryEntry> CollectOpenedTreeDir(ObservableCollection<DirectoryEntry> Entries,
           string CurrentDirectory, DirectoryEntry SelectedEntry)
        {
            var openedEntries = CollectDirectoryEntries(CurrentDirectory);
            openedEntries.ToList().ForEach(t => t.Name = new string(' ', t.Level * 5) + t.Name);

            SelectedEntry.Opened = true;

            int i = Entries.IndexOf(SelectedEntry) + 1;
            int j = 0;
            while (i < Entries.Count && j < openedEntries.Count)
            {
                Entries.Insert(i, openedEntries[j]);
                i++;
                j++;
            }

            return Entries;
        }

        public ObservableCollection<DirectoryEntry> CollectClosedTreeDir(ObservableCollection<DirectoryEntry> Entries, 
            string CurrentDirectory, DirectoryEntry SelectedEntry)
        {
            SelectedEntry.Opened = false;

            int removeIndex = Entries.IndexOf(SelectedEntry) + 1;
            while (removeIndex < Entries.Count && Entries[removeIndex].Level > SelectedEntry.Level)
            {
                Entries.RemoveAt(removeIndex);
            }

            return Entries;
        }

        public void OpenEntry(DirectoryEntry entry)
        {
            new Process
            {
                StartInfo = new ProcessStartInfo(entry.Path)
                {
                    UseShellExecute = true
                }
            }.Start();
        }

        public string PreviousDirectoryPath(string currentDirectory)
        {
            var output = System.IO.Directory.GetParent(currentDirectory);
            if (output != null)
                return output.FullName;
            else
                return currentDirectory;
        }

       
    }
}
