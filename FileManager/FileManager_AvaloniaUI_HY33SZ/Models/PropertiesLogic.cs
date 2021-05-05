using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager_AvaloniaUI_HY33SZ.Models
{
    public class PropertiesLogic
    {
        public ViewProperties CopyCurrentProperties(ViewProperties current)
        {
            return new ViewProperties()
            {
                Name = current.Name,
                CreationTime = current.CreationTime,
                Extension = current.Extension,
                LastWriteDate = current.LastWriteDate,
                ReadOnly = current.ReadOnly,
                Size = current.Size,
            };
        }
    }
}
