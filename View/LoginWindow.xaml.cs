using MahApps.Metro.Controls.Dialogs;
using System;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace DeuluwaPIM.View
{
    /// <summary>
    /// LoginWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoginWindow
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void Login(bool isAutologin)
        {
            try
            {
                string url = string.Format("http://silco.co.kr:18000/user/?id={0}&password={1}", idTextbox.Text, passwordTextbox.Password.ToString());
                string result = await DeuluwaCore.Constants.HttpRequest(url);


                if (result.IndexOf("admin") > 0)
                {
                    //로그인 성공 처리
                    MainWindow.loggined = true;
                    MainWindow.userId = idTextbox.Text;
                    MainWindow.userName = 
                        DeuluwaCore.Controller.JsonConverter.GetDictionary
                        (await DeuluwaCore.Constants.HttpRequest(string.Format("http://silco.co.kr:18000/userinfo/?id={0}", idTextbox.Text)))
                        ["name"];
                    
                    if (!isAutologin) SaveData();
                    Close();
                }
                else if (result == "failed")
                {
                    await this.ShowMessageAsync("접속 오류", "ID와 PW를 확인 해 주세요. \r\n관리자만 접속이 가능합니다!");
                }
                else
                {
                    await this.ShowMessageAsync("접속 오류", "접속이 안되네여..");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await this.ShowMessageAsync("접속 오류", "접속이 안되네여..");
                return;
            }
        }

        private void SaveData()
        {
            if (autoLoginSwitch.IsChecked == true)
            {
                Properties.Settings.Default["autologin"] = true;
                Properties.Settings.Default["id"] = idTextbox.Text;

                byte[] bytes = Encoding.Unicode.GetBytes(passwordTextbox.Password);
                Properties.Settings.Default["password"] = Convert.ToBase64String(bytes);
            }
            else
            {
                Properties.Settings.Default["autologin"] = false;
                Properties.Settings.Default["id"] = null;
                Properties.Settings.Default["password"] = null;
            }
            Properties.Settings.Default.Save();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Login(false);
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((bool)Properties.Settings.Default["autologin"] == true)
                {
                    idTextbox.Text = Properties.Settings.Default["id"].ToString();

                    byte[] bytes = Convert.FromBase64String(Properties.Settings.Default["password"].ToString());
                    passwordTextbox.Password = Encoding.Unicode.GetString(bytes);
                    Login(true);
                }
            }
            catch { }
        }

        private void IDBoxKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                passwordTextbox.Focus();
            }
        }

        private void PasswordBoxKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Login(false);
            }
        }
    }
}