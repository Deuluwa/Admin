using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace DeuluwaPIM.View
{
    /// <summary>
    /// AttendanceManagement.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AttendanceManagement
    {
        public class DateClass
        {
            public string date { get; set; }

            public DateClass(Dictionary<string, string> value)
            {
                this.date = value["date"];
            }
        }

        public class CheckClass
        {
            public string id { get; set; }
            public string name { get; set; }
            public string check { get; set; }
            public DateTime time { get; set; }

            public CheckClass(Dictionary<string, string> value, string currentDate)
            {
                id = value["userid"];
                name = value["name"];
                check = value["checked"] == "0" ? "출석" : value["checked"] == "1" ? "지각" : "결석";

                var timeStr = value["checktime"].Trim();
                time = DateTime.ParseExact(currentDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                TimeSpan ts = new TimeSpan(Convert.ToInt32(timeStr.Substring(0, 2)), Convert.ToInt32(timeStr.Substring(2, 2)), 0);
                time = time.Date + ts;
            }
        }

        string courseId = "";
        List<DateClass> dateList;
        List<CheckClass> checkList;
        public AttendanceManagement(string courseId)
        {
            InitializeComponent();

            this.courseId = courseId;
            LoadDateData();
        }

        private async void LoadDateData()
        {
            dateList = new List<DateClass>();

            var requestResult = await DeuluwaCore.Constants.HttpRequest(DeuluwaCore.Constants.shared.GetData("url") + "checkdaylist/?courseid=" + courseId);

            if (requestResult.Contains("failed")) return;

            var resultList = DeuluwaCore.Controller.JsonConverter.GetDictionaryList(requestResult);

            

            foreach(var result in resultList)
            {
                dateList.Add(new DateClass(result));
            }

            dateDataGrid.ItemsSource = dateList;
        }

        private async void LoadCheckData(string date)
        {
            checkList = new List<CheckClass>();
            var requestResult = await DeuluwaCore.Constants.HttpRequest(
                DeuluwaCore.Constants.shared.GetData("url") + "checklist/?courseid=" + courseId + "&date=" + date);

            if (requestResult.Contains("failed")) return;

            var resultList = DeuluwaCore.Controller.JsonConverter.GetDictionaryList(requestResult);

            foreach(var result in resultList)
            {
                checkList.Add(new CheckClass(result, date));
            }

            userDataGrid.ItemsSource = checkList;
        }

        private void DateChanged(object sender, SelectionChangedEventArgs e)
        {
            string date = (dateDataGrid.SelectedItem as DateClass).date;
            LoadCheckData(date);
        }

        //일괄 출석
        private async void AllCheck_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var date = (dateDataGrid.SelectedItem as DateClass).date;
            var requestResult = await DeuluwaCore.Constants.HttpRequest(DeuluwaCore.Constants.shared.GetData("url") + "updateattendance/" +
                "?courseid=" + courseId + "&date=" + date + "&mode=" + "allcheck");

            if (requestResult == "success") LoadCheckData(date);
        }

        //출석
        private async void Check_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var date = (dateDataGrid.SelectedItem as DateClass).date;
            var userId = (userDataGrid.SelectedItem as CheckClass).id;

            var requestResult = await DeuluwaCore.Constants.HttpRequest(DeuluwaCore.Constants.shared.GetData("url") + "updateattendance/" +
                "?courseid=" + courseId + "&date=" + date + "&mode=" + "check" + "&userid=" + userId);

            if (requestResult == "success") LoadCheckData(date);
        }

        //결석
        private async void Absent_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var date = (dateDataGrid.SelectedItem as DateClass).date;
            var userId = (userDataGrid.SelectedItem as CheckClass).id;

            var requestResult = await DeuluwaCore.Constants.HttpRequest(DeuluwaCore.Constants.shared.GetData("url") + "updateattendance/" +
                "?courseid=" + courseId + "&date=" + date + "&mode=" + "absent" + "&userid=" + userId);

            if (requestResult == "success") LoadCheckData(date);
        }

        //지각
        private async void Tardy_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var date = (dateDataGrid.SelectedItem as DateClass).date;
            var userId = (userDataGrid.SelectedItem as CheckClass).id;

            var requestResult = await DeuluwaCore.Constants.HttpRequest(DeuluwaCore.Constants.shared.GetData("url") + "updateattendance/" +
                "?courseid=" + courseId + "&date=" + date + "&mode=" + "tardy" + "&userid=" + userId);

            if (requestResult == "success") LoadCheckData(date);
        }

        //적용
        private async void Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var date = (dateDataGrid.SelectedItem as DateClass).date;

            foreach(var check in checkList)
            {
                await DeuluwaCore.Constants.HttpRequest(DeuluwaCore.Constants.shared.GetData("url") + "updateattendance/" +
                "?courseid=" + courseId + "&date=" + date + "&mode=" + "update" + "&userid=" + check.id + 
                "&updatetime=" + string.Format("{0:00}{1:00}", check.time.Hour, check.time.Minute));
            }

            LoadCheckData(date);
        }

        private void CheckTimePicker_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            var time = sender as Xceed.Wpf.Toolkit.DateTimeUpDown;
            try
            {
                (userDataGrid.CurrentCell.Item as CheckClass).time = (DateTime)time.Value;
            }
            catch { }
        }
    }
}