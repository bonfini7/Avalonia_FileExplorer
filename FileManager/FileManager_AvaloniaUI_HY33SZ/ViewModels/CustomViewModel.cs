using FileManager_AvaloniaUI_HY33SZ.Models;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileManager_AvaloniaUI_HY33SZ.ViewModels
{
    public class CustomViewModel
    {
        PropertiesLogic logic;
        public ViewProperties Properties { get; set; }
        public ViewProperties OldProperties { get; set; }
        public ICommand SaveCurrentProperties;

        public CustomViewModel(PropertiesLogic logic)
        {
            this.logic = logic;

            Properties = new ViewProperties
            {
                Name = true,
                Extension = true,
                Size = true,
                LastWriteDate = true,
            };

            SaveCurrentProperties = new RelayCommand(() =>
            {
                OldProperties = this.logic.CopyCurrentProperties(Properties);
            });
        }

        public CustomViewModel() : this(new PropertiesLogic())
        {   }


    }
}
