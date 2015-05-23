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
    public partial class MainWindow : System.Windows.Window
    {

        public void DisplaySettings()
        {

            while (SettingStackPanel.Children.Count > 0) {
                SettingStackPanel.Children.RemoveAt(0);
            }
            
            Grid rtmpGrid = new Grid();
            Label l_rtmp = new Label();
            TextBox tb_rtmp = new TextBox();

            rtmpGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            rtmpGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) });

            l_rtmp.Content = "rtmpdump";
            l_rtmp.FontSize = 14;

            tb_rtmp.Name = "rtmp_setting";
            tb_rtmp.BorderThickness = new Thickness(0, 0, 0, 1);
            tb_rtmp.BorderBrush = Brushes.Black;
            tb_rtmp.Padding = new Thickness(0, 6, 0, 0);
            tb_rtmp.Margin = new Thickness(5, 0, 5, 0);
            tb_rtmp.Text = rtmp;
            tb_rtmp.FontSize = 14;
            tb_rtmp.GotFocus += (System.Windows.Application.Current.MainWindow as MainWindow).Setting_MouseDown;
            
            Grid.SetColumn(l_rtmp, 0);
            Grid.SetColumn(tb_rtmp, 1);

            rtmpGrid.Children.Add(l_rtmp);
            rtmpGrid.Children.Add(tb_rtmp);


            Grid serverGrid = new Grid();
            Label l_server_location = new Label();
            TextBox tb_server = new TextBox();

            serverGrid.Name = "rtmp_location_setting";

            serverGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            serverGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) });


            l_server_location.Content = "Server";
            l_server_location.FontSize = 14;

            tb_server.BorderThickness = new Thickness(0, 0, 0, 1);
            tb_server.BorderBrush = Brushes.Black;
            tb_server.Padding = new Thickness(0, 6, 0, 0);
            tb_server.Margin = new Thickness(5, 0, 5, 0);
            tb_server.Text = server;
            tb_server.FontSize = 14;

            Grid.SetColumn(l_server_location, 0);
            Grid.SetColumn(tb_server, 1);

            serverGrid.Children.Add(l_server_location);
            serverGrid.Children.Add(tb_server);

            Separator s = new Separator { Background = (Brush)(new BrushConverter()).ConvertFromString("#FF9BBB5A") };

            SettingStackPanel.Children.Add(rtmpGrid);
            SettingStackPanel.Children.Add(s);
            SettingStackPanel.Children.Add(serverGrid);
        }

    }
}
