using MahApps.Metro.Controls.Dialogs;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace DeuluwaPIM
{
    class Constants
    {

        public static MetroDialogSettings metroDialogSettings = new MetroDialogSettings
        {
            AffirmativeButtonText = "네 ㅎㅎ",
            NegativeButtonText = "ㄴㄴ;;",
            FirstAuxiliaryButtonText = "걍 취소"
        };

        public static Style MakeRowStyle(RoutedEvent routedEvent, System.Windows.Input.MouseButtonEventHandler handler)
        {
            Style rowStyle = new Style(typeof(DataGridRow));
            rowStyle.Setters.Add(new EventSetter(routedEvent, handler));
            return rowStyle;
        }
    }
}
