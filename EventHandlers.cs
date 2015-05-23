using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace AGQRecorder0
{
    public partial class MainWindow : System.Windows.Window
    {
        private void AddEventHandlers()
        {
            Label[] Labels = new Label[] { Sun, Mon, Tue, Wed, Thu, Fri, Sat, Kak };
            ButtonAdd.Click    += ButtonAdd_Click;
            ButtonEdit.Click   += ButtonEdit_Click;
            ButtonDelete.Click += ButtonDelete_Click;
            this.KeyDown += MainWindow_KeyDown;

            foreach (Label label in Labels) {
                label.MouseDown += label_MouseDown;
                label.Foreground = Brushes.Gray;
            }

            NextRecordLabel.MouseDown += NextRecordLabel_MouseDown;
            ExitButtonImage.Click     += ExitButtonImage_Click;
            MinimizeButtonImage.Click += MinimizeButtonImage_Click;
            ListButtonImage.Click     += ListButtonImage_Click;
            AddButtonImage.Click      += AddButtonImage_Click;
            SettingButtonImage.Click  += SettingButtonImage_Click;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(1000000);
            timer.Start();
            timer_Tick(null, null);
        }

        void SettingButtonImage_Click(object sender, RoutedEventArgs e)
        {
            ShowSettingLayout();
        }


        void ListButtonImage_Click(object sender, RoutedEventArgs e)
        {
            ShowListLayout();
        }

        void AddButtonImage_Click(object sender, RoutedEventArgs e)
        {
            ShowEditLayout();
            FreshControls();
        }

        void MinimizeButtonImage_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        void ExitButtonImage_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        Program selectedProgram;
        void DeleteSelectedProgram()
        {
            if (selectedProgram != null) {
                Programs.Remove(selectedProgram);
                selectedProgram = null;
            }
        }
        void AddProgram()
        {
            string programData;
            try {
                programData = GatherData();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                return;
            }
            Programs.Add(new Program(programData));
            Console.WriteLine("Program added : " + programData);
        }


        void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Editing Program : " + selectedProgram.ToString());
            DeleteSelectedProgram();

            AddProgram();
            ShowListLayout();
        }

        void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Delete?", "Deleting", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                Console.WriteLine("Deleting Program : " + selectedProgram.ToString());
                DeleteSelectedProgram();
                ShowListLayout();
            }
        }

        void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            AddProgram();
            ShowListLayout();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            DateTime nextRecord = new DateTime(9999, 12, 31);
            foreach (Program p in Programs) {
                DateTime nextTime = p.GetNextRecordDateTime();
                if (nextRecord > nextTime) {
                    nextProgram = p;
                    nextRecord = nextTime;
                }
            }

            if (Math.Abs((nextRecord - DateTime.Now).TotalSeconds) < 0.1) {
                if (!nextProgram.IsRecording()) {
                    nextProgram.RunRecorder();
                }
            }

            CurrentTimeLabel.Content = DateTime.Now.ToString().Replace('/', '.');
            DisplayNextRecord(NextRecordLabel);
        }

        void label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Label l = sender as Label;
            l.Foreground = (l.Foreground == Brushes.Gray) ? Brushes.Black : Brushes.Gray;
        }

        void NextRecordLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            displayNextRecordIn = !displayNextRecordIn;
            DisplayNextRecord(sender as Label);
            e.Handled = true;
        }

        public void ListGrid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Grid g = sender as Grid;

            ListButtonImage.Visibility = Visibility.Visible;
            AddButtonImage.Visibility  = Visibility.Hidden;
            LayoutRootGrid.RowDefinitions[1].Height = new GridLength(15, GridUnitType.Star);
            LayoutRootGrid.RowDefinitions[2].Height = new GridLength(0);
            AddButtonGrid.Visibility        = Visibility.Hidden;
            EditDeleteButtonGrid.Visibility = Visibility.Visible;

            foreach (Program p in Programs) {
                if (p.Id == Convert.ToInt32(g.Name.Substring(6))) {
                    FillData(p);
                    selectedProgram = p;
                    break;
                }
            }
        }

        public void Setting_MouseDown(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Name == "rtmp_setting") {
                Microsoft.Win32.OpenFileDialog d = new Microsoft.Win32.OpenFileDialog();

                d.DefaultExt = ".exe";
                Nullable<bool> result = d.ShowDialog();

                if (result == true) {
                    string filename = d.FileName;
                    rtmp = filename;
                }
                DisplaySettings();
            }
        }
    }
}
