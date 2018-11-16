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
            public string time { get; set; }

            public CheckClass(Dictionary<string, string> value)
            {
                id = value["userid"];
                name = value["name"];
                check = value["checked"] == "0" ? "출석" : value["checked"] == "1" ? "지각" : "결석";

                time = value["checktime"].Trim();
                time = Convert.ToInt32(time.Substring(0, 2)) >= 12 ?
                "PM " + string.Format("{0:00}:{1}", Convert.ToInt32(time.Substring(0, 2)) - 12, time.Substring(2, 2)) :
                "AM " + time;
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
                checkList.Add(new CheckClass(result));
            }

            userDataGrid.ItemsSource = checkList;
        }

        private void DateChanged(object sender, SelectionChangedEventArgs e)
        {
            string date = (dateDataGrid.SelectedItem as DateClass).date;
            LoadCheckData(date);
        }
    }
}