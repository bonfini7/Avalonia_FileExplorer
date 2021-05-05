using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager_AvaloniaUI_HY33SZ.Models
{
    public class DirectoryEntry
    {
        private FileInfo info;
        private FileInfo Info
        {
            get
            {
                if(info == null)
                {
                    info = new FileInfo(Path);
                }
                return info;
            }
        }
        public string Path { get; set; }

        private string name;
        public string Name
        {
            get
            {
                if (name != null) return name;

                if (!string.IsNullOrEmpty(Path)) // If we can't find the path the return value is an empty string
                {
                    name = System.IO.Path.GetFileName(Path);
                    if (name == String.Empty) // If we have the path correctly but we can't get the nem of the folder/file, we returns the type.
                    {
                        return Extension;
                    }
                        
                }                 
                return name;                 
            }
            set
            {
                name = value;
            }
        }

        public string Extension
        {
            get
            {
                return !IsDir ? System.IO.Path.GetExtension(Path) : "<DIR>";
            }
        }
        public bool IsDir { get; set; }
        public string LastWriteDate { 
            get 
            {
                return Info.LastWriteTime.ToString("yyyy.MM.dd HH:mm");
            } 
        }

        public string Size
        {
            get
            {
                return IsDir ? "" : (Info.Length / 1024d).ToString("#0");
            }
        }

        public string CreationTime
        {
            get
            {
                return Info.CreationTime.ToString("yyyy.MM.dd HH:mm");
            }
        }

        public int ReadOnly
        {
            get
            {
                return Info.IsReadOnly ? 1 : 0;
            }
        }
        public int Level
        {
            get
            {
                int i = -1;
                DirectoryInfo current = new DirectoryInfo(Path);
                while (current != null)
                {
                    i++;
                    current = System.IO.Directory.GetParent(current.FullName);
                }

                return i;
                
            }
        }

        public bool Opened { get; set; }

        public DirectoryEntry(string? path, bool isDir)
        {
            Path = path;
            IsDir = isDir;
            Opened = false;
        }

        public override string? ToString()
        {
            return IsDir ? Name : System.IO.Path.GetFileName(Path);
        }
    }
}
