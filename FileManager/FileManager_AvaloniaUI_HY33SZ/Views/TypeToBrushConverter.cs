
using Avalonia.Data;
using Avalonia.Data.Converters;
using FileManager_AvaloniaUI_HY33SZ.Models;
using System;
using System.Collections.Generic;
using Avalonia.Media;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager_AvaloniaUI_HY33SZ.Views
{
    public class TypeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            /*var entry = (DirectoryEntry)value;
            if (entry.IsDir) return Brushes.LightYellow;
            if (entry.Type.ToLower() == "exe") return Brushes.Red;*/

            bool IsDir = (bool)value;
            if(IsDir) return Brushes.LightYellow;
            return Brushes.White;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Binding();
        }
    }
}
