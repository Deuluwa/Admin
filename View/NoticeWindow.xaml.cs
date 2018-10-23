using DeuluwaCore.Model;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DeuluwaPIM.View
{
    /// <summary>
    /// NoticeWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NoticeWindow
    {
        bool writeMode;
        public NoticeWindow(bool writeMode, string index = "")
        {
            InitializeComponent();
            this.writeMode = writeMode;
            WriteMode(this.writeMode, index);
            LoadNoticeData();
            indexColumn.Visibility = Visibility.Hidden;
            datagrid.RowHeight = 25;
        }

        private void WriteMode(bool writeMode, string index = "")
        {
            this.writeMode = writeMode;
            textBox.Document.Blocks.Clear();

            if (this.writeMode)
            {
                textBox.IsReadOnly = false;
                writerLabel.Content = MainWindow.userName;
                writeButton.Content = "작성 완료";
            }
            else
            {
                textBox.IsReadOnly = true;
                writeButton.Content = "신규 작성";
                foreach (var message in Model.NoticeList.list)
                {
                    if (index == message.index)
                    {
                        textBox.Document.Blocks.Add(new Paragraph(new Run(message.message)));
                        writerLabel.Content = message.user;
                        dateLabel.Content = message.time;
                        break;
                    }
                }
            }
        }

        private async void LoadNoticeData()
        {
            Model.NoticeList.list = await LoadData();
            datagrid.ItemsSource = Model.NoticeList.list;

            WriteMode(false, Model.NoticeList.list[0].index);
        }

        private async Task<List<NoticeMessage>> LoadData()
        {
            var list =  DeuluwaCore.Controller.JsonConverter.GetDictionaryList(
                await DeuluwaCore.Constants.HttpRequest(
                    "http://silco.co.kr:18000/notice"
                    ));

            List<NoticeMessage> messageList = new List<NoticeMessage>();
            foreach(var dict in list)
            {
                messageList.Add(new NoticeMessage(dict));
            }

            return messageList;
        }

        private async void datagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NoticeMessage message = datagrid.SelectedItem as NoticeMessage;
            if (writeMode)
            {
                var messageBoxResult = await this.ShowMessageAsync("작성중이던 글", "작성중이던 글을 취소하겠습니까?", MessageDialogStyle.AffirmativeAndNegative, Constants.metroDialogSettings);
                if (messageBoxResult == MessageDialogResult.Negative)
                {
                    return;
                }
            }
            WriteMode(false, message.index);
        }

        private async void writeButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if ((string)button.Content == "신규 작성")
            {
                WriteMode(true);
            }
            else
            {

                var messageBoxResult = await this.ShowMessageAsync("공지 작성", "글을 작성하시겠습니까?", MessageDialogStyle.AffirmativeAndNegative, Constants.metroDialogSettings);
                if (messageBoxResult == MessageDialogResult.Affirmative)
                {
                    string postData = string.Format("id={0}", MainWindow.userId);
                    postData += string.Format("&message={0}", new TextRange(textBox.Document.ContentStart, textBox.Document.ContentEnd).Text);
                    string result = await DeuluwaCore.Constants.HttpRequestPost("http://silco.co.kr:18000/writenoticemessage/", postData);
                    try
                    {
                        LoadNoticeData();
                    }
                    catch
                    {
                        await this.ShowMessageAsync("실패", "글 작성 실패");
                    }

                }
            }
        }
    }
}
