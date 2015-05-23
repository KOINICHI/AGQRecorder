using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Text;
using System.Threading.Tasks;

namespace AGQRecorder0
{
    class ListDisplayer
    {
        StackPanel stackpanel;

        public ListDisplayer(StackPanel _stackpanel)
        {
            stackpanel = _stackpanel;
        }

        public void DisplayPrograms(List<Program> Programs)
        {
            while (stackpanel.Children.Count > 0) {
                stackpanel.Children.RemoveAt(0);
            }
            Programs.Sort(delegate(Program a, Program b) {
                return a.GetNextRecordDateTime().CompareTo(b.GetNextRecordDateTime());
            });

            StringBuilder sb = new StringBuilder();
            foreach (Program p in Programs) {
                Grid g = new Grid();
                Label l_title = new Label();
                Label l_date  = new Label();

                g.Name = String.Format("radio_{0}", p.Id);

                g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) });
                g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                g.MouseDown += (System.Windows.Application.Current.MainWindow as MainWindow).ListGrid_MouseDown;

                string title = p.Title;
                if (title.Length > 35) {
                    title = title.Substring(0, 50) + "...";
                }
                l_title.Content = title;
                l_title.FontSize = 14;
                l_title.HorizontalContentAlignment = HorizontalAlignment.Left;
                l_title.Margin = new Thickness(5, 0, 0, 0);

                l_date.Content = p.GetNextRecordDateTime().AddSeconds(30.0).ToString("MM/dd HH:mm");
                l_date.FontSize = 14;
                l_date.HorizontalContentAlignment = HorizontalAlignment.Center;

                Grid.SetColumn(l_title, 0);
                Grid.SetColumn(l_date, 1);


                g.Children.Add(l_title);
                g.Children.Add(l_date);

                Separator s = new Separator { Background = (Brush)(new BrushConverter()).ConvertFromString("#FF9BBB5A") };

                stackpanel.Children.Add(g);
                stackpanel.Children.Add(s);
            }
            if (stackpanel.Children.Count > 1) {
                stackpanel.Children.RemoveAt(stackpanel.Children.Count-1);
            }
            //System.Windows.MessageBox.Show(sb.ToString());
        }

    }
}
