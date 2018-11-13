using DeuluwaCore.Model;
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

        private void CourseCreateButton(object sender, RoutedEventArgs e)
        {
            NewCourseSetting();
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

        private void SaveCourseInformation(object sender, RoutedEventArgs e)
        {
            
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


        }

        private void ViewCourseInfo(CourseInformation course)
        {
            courseNameLabel.Text = course.coursename;
            teacherLabel.Text = course.teacher + "(" + course.teacherid + ")";
            classNameLabel.Text = course.roomname + "(" + course.roomindex + ")";
            SetDate(course.coursedate);
            SetTime(course.courseStartTimeOrigin, course.courseEndTimeOrigin);
            SetDayCheck(course.classdayOrigin);
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

        }

        private void ClassNameMouseClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }
}
