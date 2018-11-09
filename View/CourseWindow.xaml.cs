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
using MahApps.Metro.Controls.Dialogs;
using DeuluwaCore.Model;

namespace DeuluwaPIM.View
{
    /// <summary>
    /// CourseWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CourseWindow
    {
        public CourseWindow()
        {
            InitializeComponent();
            
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
        }

        private void CourseCreateButton(object sender, RoutedEventArgs e)
        {
            NewCourseSetting();
        }
    }
}
