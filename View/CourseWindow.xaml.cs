using System;
using System.Windows;

namespace DeuluwaPIM.View
{
    /// <summary>
    /// CourseWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CourseWindow
    {
        System.Windows.Controls.Primitives.ToggleButton[] dayCheckControls;
        public CourseWindow()
        {
            InitializeComponent();
            AddDayCheckControl();
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
            startDate.SelectedDate = DateTime.Now;
            endDate.SelectedDate = DateTime.Now.AddMonths(1);
            courseNameLabel.Text = "";
            teacherLabel.Text = "";
            classNameLabel.Text = "";
            startTime.Value = DateTime.Now;
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
    }
}
