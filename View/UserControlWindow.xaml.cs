using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls.Dialogs;
using DeuluwaCore.Model;
using System.Threading.Tasks;

namespace DeuluwaPIM.View
{
    /// <summary>
    /// UserControlWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UserControlWindow
    {
        List<User> userList;
        List<CourseInformation> courseList;
        List<Attendance> attendanceList;

        bool addMode = false;

        public UserControlWindow()
        {
            InitializeComponent();

            ModifyMode(false);


            LoadUsers(true);
        }

        private async void LoadUsers(bool first)
        {
            userList = new List<User>();

            var list = DeuluwaCore.Controller.JsonConverter.GetDictionaryList
                (await DeuluwaCore.Constants.HttpRequest("http://silco.co.kr:18000/userlist"));

            foreach (var dict in list)
            {
                userList.Add(new User(dict));
            }

            userDatagrid.ItemsSource = userList;
            if (first) userDatagrid.SelectedIndex = 0;
        }

        private void LoadUsers(string content)
        {
            userDatagrid.ItemsSource = null;
            userList = new List<User>();

            var list = DeuluwaCore.Controller.JsonConverter.GetDictionaryList
                (content);

            foreach (var dict in list)
            {
                userList.Add(new User(dict));
            }

            userDatagrid.ItemsSource = userList;
        }

        private async void LoadCourses(string id)
        {
            try
            {
                courseList = new List<CourseInformation>();
                var dictList = DeuluwaCore.Controller.JsonConverter.GetDictionaryList
                    (await DeuluwaCore.Constants.HttpRequest("http://silco.co.kr:18000/usercourselist/?id=" + id));

                foreach (var dict in dictList)
                {
                    courseList.Add(new CourseInformation(dict));
                }

                courseDatagrid.ItemsSource = courseList;
            }
            catch { }
        }

        private async void LoadAttendances(string id, string courseid)
        {
            try
            {
                attendanceList = new List<Attendance>();
                List<Dictionary<string, string>> list = DeuluwaCore.Controller.JsonConverter.GetDictionaryList
                    (await DeuluwaCore.Constants.HttpRequest("http://silco.co.kr:18000/userattendancelist/?courseid=" + courseid + "&id=" + id));

                foreach (var attendance in list)
                {
                    attendanceList.Add(new Attendance(attendance));
                }

                attendanceDatagrid.ItemsSource = attendanceList;

            }
            catch
            {

            }
        }

        private void ViewUser(User user)
        {
            //개인정보 로드
            idBox.Text = user.id;
            nameBox.Text = user.name;
            phonenumberBox.Text = user.phonenumber;
            addressBox.Text = user.address;
            isAdminSwitch.IsChecked = user.admin == "True" ? true : false;

            //수강과목 로드
            LoadCourses(user.id);
        }

        private async void ModifyButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button.Content as string == "정보 수정")
            {
                button.Content = "수정 완료";
                ModifyMode(true);
            }
            else
            {
                var dialogResult = await this.ShowMessageAsync("수정", "이대로 수정을 하시겠습니까?", MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, Constants.metroDialogSettings);
                if (dialogResult == MessageDialogResult.Affirmative)
                {
                    //수정 확인
                    UpdateData();
                    button.Content = "정보 수정";
                    ModifyMode(false);
                }
                else if (dialogResult == MessageDialogResult.FirstAuxiliary)
                {
                    //취소
                    button.Content = "정보 수정";
                    ModifyMode(false);
                }

            }
        }

        private async void UpdateData()
        {
            List<Dictionary<string, string>> list;
            try
            {
                string postdata = string.Format("id={0}&name={1}&phonenumber={2}&address={3}", idBox.Text, nameBox.Text, phonenumberBox.Text, addressBox.Text);

                list = DeuluwaCore.Controller.JsonConverter.GetDictionaryList
                    (await DeuluwaCore.Constants.HttpRequestPost(DeuluwaCore.Constants.shared.GetData("url") + "userupdate", postdata));
                int number = 0;

                foreach (var dict in list)
                {
                    User user = new User(dict);
                    if (userList[number].id == idBox.Text)
                    {
                        userList[number] = user;
                        break;
                    }
                    number++;
                }

                userDatagrid.ItemsSource = null;
                userDatagrid.ItemsSource = userList;

                userDatagrid.SelectedIndex = number;

                ViewUser(userDatagrid.SelectedItem as User);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e);
                await this.ShowMessageAsync("수정", "수정에 실패했어욥 ㅠㅠ", MessageDialogStyle.Affirmative, Constants.metroDialogSettings);
            }

        }

        private void ModifyMode(bool on)
        {
            if (on)
            {
                nameBox.IsReadOnly = false;
                phonenumberBox.IsReadOnly = false;
                addressBox.IsReadOnly = false;
                isAdminSwitch.IsEnabled = true;
            }
            else
            {
                nameBox.IsReadOnly = true;
                phonenumberBox.IsReadOnly = true;
                addressBox.IsReadOnly = true;
                isAdminSwitch.IsEnabled = false;
            }
        }

        private void userDatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                User item = userDatagrid.SelectedItem as User;

                ViewUser(item);
            }
            catch
            {
                System.Console.WriteLine("리스트 끊김 쉬벌");
            }
        }

        private void courseDatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CourseInformation item = courseDatagrid.SelectedItem as CourseInformation;

            LoadAttendances(idBox.Text, item.index);
        }

        private async void InsertUserClick(object sender, RoutedEventArgs e)
        {
            if ((string)insertButton.Content == "신규 등록")
            {
                addMode = true;
                idBox.IsReadOnly = false;
                ModifyMode(true);
                insertButton.Content = "등록 완료";
                idBox.Text = "";
                nameBox.Text = "";
                phonenumberBox.Text = "";
                addressBox.Text = "";
            }
            else
            {
                var dialogResult = await this.ShowMessageAsync("등록", "등록 하시겠습니까?", MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, Constants.metroDialogSettings);

                if (dialogResult == MessageDialogResult.Affirmative)
                {
                    if (idBox.Text == "" || nameBox.Text == "" || phonenumberBox.Text == "" || addressBox.Text == "")
                    {
                        await this.ShowMessageAsync("제발좀", "항목이 비었습니다. 제발 좀 사람같이 조작 해 주십시오 짜증나니까", MessageDialogStyle.Affirmative, Constants.metroDialogSettings);
                        return;
                    }
                    var result = await InsertUser();
                    if (result == InsertResult.SUCCESS)
                    {
                        idBox.IsReadOnly = true;
                        ModifyMode(false);
                        await this.ShowMessageAsync("성공", "ID 등록 성공 했구요, 비밀번호는 폰번호에요", MessageDialogStyle.Affirmative, Constants.metroDialogSettings);
                        insertButton.Content = "신규 등록";

                        string id = idBox.Text;
                        for (int i = 0; i < userList.Count; i++) if (userList[i].id == id)
                            {
                                userDatagrid.SelectedIndex = i;
                                break;
                            }
                        addMode = false;
                    }
                    else if (result == InsertResult.IDOVERLAP)
                    {
                        await this.ShowMessageAsync("중복", "아이디 중복이에여 쫌 제대로 확인좀 하고 하시라구요 짜증나게 하지 말고;;", MessageDialogStyle.Affirmative, Constants.metroDialogSettings);
                    }
                    else if (result == InsertResult.SERVERERROR)
                    {
                        await this.ShowMessageAsync("오류", "연결에 문제가 있는데 나중에 해봐요", MessageDialogStyle.Affirmative, Constants.metroDialogSettings);
                    }
                }
                else if (dialogResult == MessageDialogResult.FirstAuxiliary)
                {
                    //등록 취소
                    idBox.IsReadOnly = true;
                    ModifyMode(false);
                    insertButton.Content = "신규 등록";
                    addMode = false;
                    LoadUsers(true);
                }
            }
        }

        enum InsertResult
        {
            SUCCESS,
            IDOVERLAP,
            SERVERERROR,
            NONE
        }

        private async Task<InsertResult> InsertUser()
        {
            string postData = string.Format("id={0}&name={1}&phonenumber={2}&address={3}&admin={4}",
                idBox.Text, nameBox.Text, phonenumberBox.Text, addressBox.Text,
                (bool)isAdminSwitch.IsChecked ? "True" : "False");

            var result = await DeuluwaCore.Constants.HttpRequestPost(DeuluwaCore.Constants.shared.GetData("url") + "useradd", postData);

            if (result.Contains("중복")) return InsertResult.IDOVERLAP;
            try
            {
                LoadUsers(result);

                return InsertResult.SUCCESS;
            }
            catch
            {
                return InsertResult.SERVERERROR;
            }
        }

        private void userDatagrid_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (addMode) e.Handled = true;
        }

        private async void PasswordReset(object sender, RoutedEventArgs e)
        {
            var dialogResult = await this.ShowMessageAsync("비밀번호", "비밀번호를 전화번호로 초기화 하시겠습니까?", MessageDialogStyle.AffirmativeAndNegative, Constants.metroDialogSettings);
            //비밀번호 초기화 코드 작성

            if (dialogResult == MessageDialogResult.Affirmative)
            {
                var result = await DeuluwaCore.Constants.HttpRequest(DeuluwaCore.Constants.shared.GetData("url") + "resetpassword/?id=" + idBox.Text);

                if (result == "success") await this.ShowMessageAsync("비밀번호", "초기화가 완료되었습니다", MessageDialogStyle.Affirmative, Constants.metroDialogSettings);
                else
                {
                    await this.ShowMessageAsync("비밀번호", "서버에 문제가 있어 보입니다", MessageDialogStyle.Affirmative, Constants.metroDialogSettings);
                }
            }
        }
    }
}