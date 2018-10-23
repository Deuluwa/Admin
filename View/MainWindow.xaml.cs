using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using DeuluwaCore.Model;

namespace DeuluwaPIM.View
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow
    {
        public static bool loggined = false;
        public static string userId = "";
        public static string userName = "";

        public MainWindow()
        {
            InitializeComponent();
            LoginWindow lw = new LoginWindow();
            lw.ShowDialog();
            if (!loggined) { Close(); }

            LoadNoticeData();

            datagrid.RowStyle = Constants.MakeRowStyle(MouseDoubleClickEvent, new MouseButtonEventHandler(DataGridCell_MouseDoubleClick));
            datagrid.RowHeight = 25;
        }

        private async void LoadNoticeData()
        {

            Model.NoticeList.list = await LoadData();

            datagrid.ItemsSource = Model.NoticeList.list;
        }

        private async Task<List<NoticeMessage>> LoadData()
        {
            var result = DeuluwaCore.Constants.HttpRequest("http://silco.co.kr:18000/notice");
            var array = DeuluwaCore.Controller.JsonConverter.GetDictionaryList(await result);
            List<NoticeMessage> messageList = new List<NoticeMessage>();
            foreach(var message in array)
            {
                messageList.Add(new NoticeMessage(message));
            }

            return messageList;
        }

        private void AccountManagement(object sender, RoutedEventArgs e)
        {
            var window = new UserControlWindow()
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.ShowDialog();
        }

        private void CheckManagement(object sender, RoutedEventArgs e)
        {
            //출석 관리
        }

        private void CourseManagement(object sender, RoutedEventArgs e)
        {
            //수업 관리
        }

        private void ClassManagement(object sender, RoutedEventArgs e)
        {
            //교실 관리
        }

        private void Setting(object sender, RoutedEventArgs e)
        {
            //환경 설정
        }

        private async Task<bool> IsLogout()
        {
            if (!(bool)Properties.Settings.Default["autologin"]) return true;
            
            var result = await this.ShowMessageAsync("로그아웃", "로그아웃하시겠습니까?\r\n로그아웃을 하면 저장된 정보가 사라집니다",MessageDialogStyle.AffirmativeAndNegative, Constants.metroDialogSettings);
            if(result == MessageDialogResult.Affirmative)
            {
                return true;
            }

            return false;
        }

        private async void Logout(object sender, RoutedEventArgs e)
        {

            if (await IsLogout())
            {
                //로그 아웃
                Hide();

                loggined = false;
                Properties.Settings.Default["autologin"] = false;
                Properties.Settings.Default["id"] = null;
                Properties.Settings.Default["password"] = null;
                Properties.Settings.Default.Save();

                LoginWindow lw = new LoginWindow();
                lw.ShowDialog();

                if (!loggined) { Close(); }

                Show();
            }
        }

        private void WriteButton_Click(object sender, RoutedEventArgs e)
        {
            NoticeWindow window = new NoticeWindow(true);
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.ShowDialog();
        }

        private void DataGridCell_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            NoticeMessage item = row.Item as NoticeMessage;

            NoticeWindow window = new NoticeWindow(false, item.index);
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.ShowDialog();

        }

        
    }
}
