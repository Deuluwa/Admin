using DeuluwaCore.Model;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Windows;

namespace DeuluwaPIM.View
{
    /// <summary>
    /// CourseWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CourseWindow
    {
        System.Windows.Controls.Primitives.ToggleButton[] dayCheckControls;
        List<CourseInformation> courseInformationList;
        List<User> courseStudentList;
        string courseId = "";

        public CourseWindow()
        {
            InitializeComponent();

            teacherLabel.IsReadOnly = true;
            classNameLabel.IsReadOnly = true;

            AddDayCheckControl();

            GetCourseInformationList(true);
        }

        private void AddDayCheckControl()
        {
            dayCheckControls = new System.Windows.Controls.Primitives.ToggleButton[7];
            dayCheckControls[0] = monCheck;
            dayCheckControls[1] = tueCheck;
            dayCheckControls[2] = wedCheck;
            dayCheckControls[3] = thuCheck;
            dayCheckControls[4] = friCheck;
            dayCheckControls[5] = satCheck;
            dayCheckControls[6] = sunCheck;
        }

        private void NewCourseSetting()
        {
            startDatePicker.SelectedDate = DateTime.Now;
            endDatePicker.SelectedDate = DateTime.Now.AddMonths(1);
            courseNameLabel.Text = "";
            teacherLabel.Text = "";
            classNameLabel.Text = "";
            startTimePicker.Value = DateTime.Now;
            runningTimeLabel.Text = "60";
            SetDayCheck("FFFFFFF");
        }

        private Dictionary<string, string> GetCourseInformation(string mode)
        {
            Dictionary<string, string> addData = new Dictionary<string, string>();

            addData.Add("mode", mode);

            string userid = teacherLabel.Text.Substring(
                teacherLabel.Text.LastIndexOf('(') + 1,
                teacherLabel.Text.LastIndexOf(')') - teacherLabel.Text.LastIndexOf('(') - 1
                );

            addData.Add("teacher", userid);

            string roomIndex = classNameLabel.Text.Substring(
                classNameLabel.Text.LastIndexOf('(') + 1,
                classNameLabel.Text.LastIndexOf(')') - classNameLabel.Text.LastIndexOf('(') - 1
                );

            addData.Add("room", roomIndex);

            addData.Add("startdate", string.Format("{0:0000}-{1:00}-{2:00}",
                startDatePicker.SelectedDate.Value.Date.Year,
                startDatePicker.SelectedDate.Value.Date.Month,
                startDatePicker.SelectedDate.Value.Date.Day));

            addData.Add("enddate", string.Format("{0:0000}-{1:00}-{2:00}",
                endDatePicker.SelectedDate.Value.Date.Year,
                endDatePicker.SelectedDate.Value.Date.Month,
                endDatePicker.SelectedDate.Value.Date.Day));

            addData.Add("starttime", string.Format("{0:00}{1:00}",
                startTimePicker.Value.Value.Hour,
                startTimePicker.Value.Value.Minute));

            addData.Add("runningtime", runningTimeLabel.Text);

            addData.Add("classday", GetDayCheck());

            addData.Add("coursename", courseNameLabel.Text);

            return addData;
        }

        private async void CourseCreateButton(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> courseInformation = GetCourseInformation("add");

            string data = "";

            foreach(KeyValuePair<string, string> ci in courseInformation)
            {
                data += "&" + ci.Key + "=" + ci.Value;
            }

            data = data.Remove(0, 1);
        
            var insertResult = await DeuluwaCore.Constants.HttpRequestPost(DeuluwaCore.Constants.shared.GetData("url") + "managecourse/", data);

            if (!insertResult.Contains("success")) return;

            string insertCourseId = insertResult.Substring(10, insertResult.Length - 10);

            courseId = insertCourseId;

            GetCourseInformationList(false);
        }

        private string GetDayCheck()
        {
            string result = "";

            for(int i=0; i < 7; i++)
            {
                result += dayCheckControls[i].IsChecked == true ? "T" : "F";
            }

            return result;
        }

        private void SetDayCheck(string result)
        {
            try
            {
                for (int i = 0; i < 7; i++)
                {
                    dayCheckControls[i].IsChecked = result[i] == 'T' ? true : false;
                }
            }
            catch
            {

            }
        }

        private async void SaveCourseInformation(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> courseInformation = GetCourseInformation("modify");
            courseInformation.Add("courseid", courseId);

            string data = "";

            foreach (KeyValuePair<string, string> ci in courseInformation)
            {
                data += "&" + ci.Key + "=" + ci.Value;
            }

            data = data.Remove(0, 1);

            var insertResult = await DeuluwaCore.Constants.HttpRequestPost(DeuluwaCore.Constants.shared.GetData("url") + "managecourse/", data);

            if (!insertResult.Contains("success")) return;

            GetCourseInformationList(false);
        }

        private async void GetCourseInformationList(bool first)
        {
            courseInformationList = new List<CourseInformation>();

            var list = DeuluwaCore.Controller.JsonConverter.GetDictionaryList
                    (await DeuluwaCore.Constants.HttpRequest(DeuluwaCore.Constants.shared.GetData("url") + "courseinformation/"));

            foreach (var courseinfo in list)
            {
                courseInformationList.Add(new CourseInformation(courseinfo));
            }

            courseListGrid.ItemsSource = courseInformationList;

            if (first)
            {
                courseListGrid.SelectedIndex = 0;
            }

            else {
                int i = 0;
                foreach(var courseinfo in courseInformationList)
                {
                    if (courseinfo.index == courseId)
                    {
                        courseListGrid.SelectedIndex = i;
                        return;
                    }
                    i++;
                }
            }
        }

        private void ViewCourseInfo(CourseInformation course)
        {
            courseNameLabel.Text = course.coursename;
            teacherLabel.Text = course.teacher + "(" + course.teacherid + ")";
            classNameLabel.Text = course.roomname + "(" + course.roomindex + ")";
            SetDate(course.coursedate);
            SetTime(course.courseStartTimeOrigin, course.courseEndTimeOrigin);
            SetDayCheck(course.classdayOrigin);
            GetUserList(course.index);
            courseId = course.index;
        }

        private void SetDate(string dateRangeString)
        {
            if (dateRangeString.Length != 23) return;

            string startDateString = dateRangeString.Substring(0, 10);
            string endDateString = dateRangeString.Substring(13, 10);

            DateTime startDateTime = DateTime.ParseExact(startDateString, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime endDateTime = DateTime.ParseExact(endDateString, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            startDatePicker.SelectedDate = startDateTime;
            endDatePicker.SelectedDate = endDateTime;
        }

        private void SetTime(string startTimeString, string endTimeString)
        {
            DateTime startTime = DateTime.ParseExact(startTimeString, "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
            DateTime endTime = DateTime.ParseExact(endTimeString, "HH:mm", System.Globalization.CultureInfo.InvariantCulture);

            if (endTime < startTime) endTime = endTime.AddDays(1);

            TimeSpan timeSpan = endTime - startTime;
            int runningTime = (int)timeSpan.TotalMinutes;

            startTimePicker.Value = startTime;
            runningTimeLabel.Text = runningTime.ToString();
        }

        private void CourseListSelcetionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                CourseInformation info = courseListGrid.SelectedItem as CourseInformation;
                ViewCourseInfo(info);
            }
            catch { }
        }

        private void TeacherTextBoxClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var searchWindow = new SearchWindow(SearchWindow.Category.USERNAME)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            searchWindow.exitActionEvent += TeacherSet;

            searchWindow.ShowDialog();
        }

        private void TeacherSet(List<string> result)
        {
            teacherLabel.Text = result[1] + "(" + result[0] + ")";
        }

        private void ClassNameMouseClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var searchWindow = new SearchWindow(SearchWindow.Category.CLASSNAME)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            searchWindow.exitActionEvent += RoomSet;

            searchWindow.ShowDialog();
        }

        private void RoomSet(List<string> result)
        {
            classNameLabel.Text = result[1] + "(" + result[0] + ")";
        }

        private async void GetUserList(string courseid)
        {
            var httpResult = await DeuluwaCore.Constants.HttpRequest(DeuluwaCore.Constants.shared.GetData("url") + "coursestudentlist/?courseid=" + courseid);

            courseStudentList = new List<User>();

            userListDataGrid.ItemsSource = courseStudentList;

            if (httpResult == "NO RESULT") return;

            var jsonList = DeuluwaCore.Controller.JsonConverter.GetDictionaryList(httpResult);

            foreach(var json in jsonList)
            {
                courseStudentList.Add(new User(json));
            }

            userListDataGrid.Items.Refresh();
        }

        private void AddStudentClick(object sender, RoutedEventArgs e)
        {
            var searchWindow = new SearchWindow(SearchWindow.Category.USERNAME)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            searchWindow.exitActionEvent += AddStudent;

            searchWindow.ShowDialog();
        }

        private async void AddStudent(List<string> result)
        {
            if (courseId == "") return;

            string postData = string.Format("courseid={0}&userid={1}&mode=add", courseId, result[0]);

            string postResult = await DeuluwaCore.Constants.HttpRequestPost(DeuluwaCore.Constants.shared.GetData("url") + "managestudenttocourse/", postData);

            if(postResult != "success")
            {
                await this.ShowMessageAsync("등록 실패", postResult+"\r\n 나중에 다시 시도해 주세요", MessageDialogStyle.Affirmative, Constants.metroDialogSettings);
                return;
            }

            GetUserList(courseId);
        }

        private async void DeleteStudentMenuClick(object sender, RoutedEventArgs e)
        {
            if (courseId == "") return;

            try
            {
                var user = userListDataGrid.SelectedItem as User;

                string postData = string.Format("courseid={0}&userid={1}&mode=delete", courseId, user.id);

                string postResult = await DeuluwaCore.Constants.HttpRequestPost(DeuluwaCore.Constants.shared.GetData("url") + "managestudenttocourse/", postData);

                if (postResult != "success")
                {
                    await this.ShowMessageAsync("삭제 실패", postResult + "\r\n 나중에 다시 시도해 주세요", MessageDialogStyle.Affirmative, Constants.metroDialogSettings);
                    return;
                }

                GetUserList(courseId);
            }
            catch
            {
                return;
            }
        }

        private void AddStudentMenuClick(object sender, RoutedEventArgs e)
        {
            var searchWindow = new SearchWindow(SearchWindow.Category.USERNAME)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            searchWindow.exitActionEvent += AddStudent;

            searchWindow.ShowDialog();
        }

        private void ClearCorurseInformation(object sender, RoutedEventArgs e)
        {
            NewCourseSetting();
        }

        private async void DeleteCourseInformation(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> courseInformation = new Dictionary<string, string>();

            courseInformation.Add("mode", "delete");
            courseInformation.Add("courseid", courseId);

            string data = "";

            foreach (KeyValuePair<string, string> ci in courseInformation)
            {
                data += "&" + ci.Key + "=" + ci.Value;
            }

            data = data.Remove(0, 1);

            var insertResult = await DeuluwaCore.Constants.HttpRequestPost(DeuluwaCore.Constants.shared.GetData("url") + "managecourse/", data);

            if (!insertResult.Contains("success")) return;

            courseId = "";

            GetCourseInformationList(true);
        }
    }
}