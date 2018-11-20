using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DeuluwaPIM.View
{
    /// <summary>
    /// ClassManagementWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ClassManagementWindow
    {
        public class Room
        {
            public string index { get; set; }
            public string name { get; set; }
            public string latitude { get; set; }
            public string longitude { get; set; }
            public string nfc { get; set; }

            public Room(Dictionary<string ,string> value)
            {
                if (value.ContainsKey("index")) index = value["index"];
                if (value.ContainsKey("name")) name = value["name"];
                if (value.ContainsKey("latitude")) latitude = value["latitude"];
                if (value.ContainsKey("longitude")) longitude = value["longitude"];
                if (value.ContainsKey("nfc")) nfc = value["nfc"];
            }
        }

        enum CheckErrorMessage
        {
            NOFLOAT,
            BLANKINCLUDE,
            SUCCESS,
            NONE
        }

        List<Room> rooms;
        bool writingMode = false;

        public ClassManagementWindow()
        {
            InitializeComponent();
            LoadRoomList(true);
        }

        private async void LoadRoomList(bool first)
        {
            var result = DeuluwaCore.Controller.JsonConverter.GetDictionaryList(
                await DeuluwaCore.Constants.HttpRequest(DeuluwaCore.Constants.shared.GetData("url") + "roomlist2"));

            if (result.Count <= 0) return;

            rooms = new List<Room>();

            foreach(Dictionary<string, string> room in result)
            {
                rooms.Add(new Room(room));
            }

            classList.ItemsSource = rooms;

            if (first)
            {
                classList.SelectedIndex = 0;
            }
        }

        private async void AddClass_Click(object sender, RoutedEventArgs e)
        {
            if(newClass.Content.ToString() == "신규")
            {
                newClass.Content = "생성";
                className.Text = "";
                classLongitude.Text = "";
                classLatitude.Text = "";
                classNFC.Text = "";
                writingMode = true;
            }
            else
            {
                var result = AddValidateCheck();
                if(result == CheckErrorMessage.BLANKINCLUDE)
                {
                    await this.ShowMessageAsync("제발좀", "항목이 비었습니다. 제발 좀 사람같이 조작 해 주십시오 짜증나니까", MessageDialogStyle.Affirmative, Constants.metroDialogSettings);
                }
                else if(result == CheckErrorMessage.NOFLOAT)
                {
                    await this.ShowMessageAsync("제발좀", "올바른 경도와 위도를 설정해 주세요,\r\n혹시 바보이신가요?\r\n가까에 있는 사람한테 부탁을 해 보세요", MessageDialogStyle.Affirmative, Constants.metroDialogSettings);
                }
                else
                {
                    string postData =
                        "&nfc=" + classNFC.Text +
                        "&latitude=" + classLatitude.Text +
                        "&longitude=" + classLongitude.Text +
                        "&name=" + className.Text;

                    var requestResult = await DeuluwaCore.Constants.HttpRequestPost(DeuluwaCore.Constants.shared.GetData("url") + "roomcreate/", postData);

                    if (requestResult.Trim() == "success")
                    {
                        LoadRoomList(false);
                        classList.SelectedIndex = rooms.Count - 1;
                    }

                    newClass.Content = "신규";
                    writingMode = false;
                }
            }
        }

        private CheckErrorMessage AddValidateCheck()
        {
            if (className.Text == "" || classLongitude.Text == "" || classLatitude.Text == "" || classNFC.Text == "") return CheckErrorMessage.BLANKINCLUDE;

            float value = 0;
            bool parseResult1 = float.TryParse(classLongitude.Text, out value);
            bool parseResult2 = float.TryParse(classLatitude.Text, out value);

            if (!parseResult1 || !parseResult2) return CheckErrorMessage.NOFLOAT;

            return CheckErrorMessage.SUCCESS;
        }

        private async void UpdateClass_Click(object sender, RoutedEventArgs e)
        {
            if (!await WriteModeDisable()) return;

            string roomIndex = (classList.SelectedItem as Room).index;

            string postData =
                "roomid=" + roomIndex +
                "&nfc=" + classNFC.Text +
                "&latitude=" + classLatitude.Text +
                "&longitude=" + classLongitude.Text +
                "&name=" + className.Text;

            var result = await DeuluwaCore.Constants.HttpRequestPost(DeuluwaCore.Constants.shared.GetData("url") + "roomupdate/", postData);

            if (result.Trim() == "success")
            {
                LoadRoomList(false);
                int index = 0;

                foreach(Room room in rooms)
                {
                    if (roomIndex == room.index) break;
                    index++;
                }
                if (index < rooms.Count) classList.SelectedIndex = index;
            }
        }

        private async void DeleteClass_Click(object sender, RoutedEventArgs e)
        {
            if (!await WriteModeDisable()) return;

            string roomIndex = (classList.SelectedItem as Room).index;

            var result = await DeuluwaCore.Constants.HttpRequest(DeuluwaCore.Constants.shared.GetData("url") + "roomdelete/?roomid=" + roomIndex);

            Console.WriteLine(result);

            if (result.Trim() == "success")
            {
                LoadRoomList(true);
            }

            else if(result.Contains("Ongoing Course"))
            {
                await this.ShowMessageAsync("혹시 내게 불만있어요?", "이미 있다 Course\r\n저를 짜증나게 합니까? Why? \r\n이것은 경고이다. Never", MessageDialogStyle.Affirmative, Constants.metroDialogSettings);
            }

        }

        private async void classList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!await WriteModeDisable()) return;

            var item = classList.SelectedItem as Room;
            try
            {
                className.Text = item.name;
                classLongitude.Text = item.longitude;
                classLatitude.Text = item.latitude;
                classNFC.Text = item.nfc;
            }
            catch { }
        }

        private async Task<bool> WriteModeDisable()
        {
            if (!writingMode) return true;
            var result = await this.ShowMessageAsync("신규 작성중", "지금 새로 교실을 추가하고있습니다만.. 취소가 가능합니다.\r\n\r\n취소 할래요?", MessageDialogStyle.AffirmativeAndNegative, Constants.metroDialogSettings);

            if (result == MessageDialogResult.Affirmative)
            {
                writingMode = false;
                newClass.Content = "신규";
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
