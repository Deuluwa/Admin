using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DeuluwaPIM.View
{
    /// <summary>
    /// SearchWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SearchWindow
    {
        public enum Category
        {
            USERNAME,
            CLASSNAME,
            COURSENAME,
            NONE
        }

        public class Result
        {
            public string index { get; set; }
            public string name { get; set; }

            public Result(Dictionary<string, string> dictionary, Category category)
            {
                if (category == Category.CLASSNAME)
                {
                    if (dictionary.ContainsKey("index")) index = dictionary["index"];
                    if (dictionary.ContainsKey("name")) name = dictionary["name"];
                }
                else if (category == Category.COURSENAME)
                {
                    if (dictionary.ContainsKey("index")) index = dictionary["index"];
                    if (dictionary.ContainsKey("coursename")) name = dictionary["coursename"];
                }
                else if (category == Category.USERNAME)
                {
                    if (dictionary.ContainsKey("id")) index = dictionary["id"];
                    if (dictionary.ContainsKey("name")) name = dictionary["name"];
                }
            }
        }

        public delegate void ExitAction(List<string> result);
        public event ExitAction exitActionEvent;

        List<Result> resultList;
        Category category = Category.NONE;

        public SearchWindow(Category category)
        {
            InitializeComponent();
            this.category = category;

            resultDataGrid.RowStyle = Constants.MakeRowStyle(MouseDoubleClickEvent, new MouseButtonEventHandler(DataGridCell_MouseDoubleClick));

            searchKeywordTextBox.Focus();
            Search("");
        }

        private void DataGridCell_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            Result result = row.Item as Result;

            List<string> resultReturn = new List<string>();
            resultReturn.Add(result.index);
            resultReturn.Add(result.name);

            exitActionEvent(resultReturn);
            Close();
        }

        private async void Search(string keyword)
        {
            resultList = new List<Result>();

            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            string categoryKeyword = "";

            if (category == Category.USERNAME)
            {
                categoryKeyword = "userlist";
                if (keyword != "") categoryKeyword += "/?username=" + keyword;
            }
            else if(category == Category.CLASSNAME)
            {
                categoryKeyword = "roomlist";
                if (keyword != "") categoryKeyword += "/?roomname=" + keyword;
            }
            var resultString = await DeuluwaCore.Constants.HttpRequest(DeuluwaCore.Constants.shared.GetData("url") + categoryKeyword);

            if (resultString == "NO RESULT") return;

            list = DeuluwaCore.Controller.JsonConverter.GetDictionaryList
                (resultString);

            foreach (var result in list)
            {
                resultList.Add(new Result(result, category));
            }

            resultDataGrid.ItemsSource = resultList;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Search(searchKeywordTextBox.Text);
        }

        private void searchKeywordTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Search(searchKeywordTextBox.Text);
            }
        }

        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Close();
        }
    }
}