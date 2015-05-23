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
using System.Windows.Navigation;
using System.IO;

namespace AGQRecorder0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Program> Programs;

        string data_filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"AGQRecorder\data.txt");
        Program nextProgram;
        bool displayNextRecordIn;
        ListDisplayer displayer;

        public string rtmp = @"rtmpdump.exe";
        public string server = @"rtmp://fms-base1.mitene.ad.jp/agqr/aandg22";

        public MainWindow()
        {
            InitializeComponent();

            Programs = new List<Program>();
            displayNextRecordIn = false;

            if (!Directory.Exists(Path.GetDirectoryName(data_filename))) {
                Directory.CreateDirectory(Path.GetDirectoryName(data_filename));
            }
            if (!File.Exists(data_filename)) {
                File.Create(data_filename).Dispose();
            }
            else {
                using (StreamReader reader = new StreamReader(data_filename, Encoding.UTF8)) {
                    string line;
                    rtmp = reader.ReadLine();
                    server = reader.ReadLine();
                    while ((line = reader.ReadLine()) != null) {
                        Programs.Add(new Program(line));
                    }
                }
            }

            displayer = new ListDisplayer(ListStackPanel);
            displayer.DisplayPrograms(Programs);

            AddEventHandlers();
            MakeDraggable();
            initTray();

        }

        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1) {
                StringBuilder sb = new StringBuilder();
                foreach (Program p in Programs) {
                    sb.Append(p.ToString() + " " + p.GetNextRecordDateTime().ToString() + Environment.NewLine);
                }
                MessageBox.Show(sb.ToString());
            }
            if (e.Key == Key.F2) {
                Programs[0].RunRecorder();
            }
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            using (StreamWriter writer = new StreamWriter(data_filename)) {
                writer.WriteLine(rtmp);
                writer.WriteLine(server);
                foreach (Program p in Programs) {
                    writer.WriteLine(p.SaveData());
                }
            }
            tray.Dispose();
        }


        void FreshControls()
        {
            Label[] Labels = new Label[] { Sun, Mon, Tue, Wed, Thu, Fri, Sat, Kak };
            TextBox[] TextBoxes = new TextBox[] { StartHour, StartMinute, EndHour, EndMinute };

            foreach (Label l in Labels) {
                l.Foreground = Brushes.Gray;
            }
            foreach (TextBox tb in TextBoxes) {
                tb.Text = "";
            }
            TitleTextBox.Text = "";

        }

        void DisplayNextRecord(Label l)
        {
            if (nextProgram == null) { return; }
            DateTime nextRecord = nextProgram.GetNextRecordDateTime();
            string title = nextProgram.Title;
            if (title.Length > 20) {
                title = title.Substring(0, 20) + "...";
            }
            if (displayNextRecordIn) {
                TimeSpan ts = (nextRecord - DateTime.Now).Add(new TimeSpan(0, 0, 1));
                l.Content = String.Format("{0} in {1} : {2} : {3}",
                    title, ts.Days*24 + ts.Hours, ts.Minutes, ts.Seconds);
            }
            else {
                l.Content = title + " at " + nextRecord.ToString().Replace('/', '.');
            }
        }

        int NewProgramId()
        {
            int ret;
            for (ret=0; ret<Programs.Count; ret++) {
                bool flag = false;
                foreach (Program p in Programs) {
                    if (flag = (p.Id == ret)) {
                        break;
                    }
                }
                if (!flag) {
                    break;
                }
            }
            return ret;
        }


        public string GatherData()
        {

            Label[] Labels = new Label[] { Sun, Mon, Tue, Wed, Thu, Fri, Sat };
            TextBox[] TextBoxes = new TextBox[] { StartHour, StartMinute, EndHour, EndMinute };
            StringBuilder sb = new StringBuilder();

            sb.Append(String.Format("{0},", NewProgramId()));
            sb.Append(TitleTextBox.Text + ",");
            foreach (TextBox tb in TextBoxes) {
                try {
                    int num = Convert.ToInt32(tb.Text);
                    sb.Append(String.Format("{0:D2},", num));
                }
                catch {
                    throw new Exception(tb.Name + " is not a valid number");
                }
            }
            if (Convert.ToInt32(StartHour.Text)*60 + Convert.ToInt32(StartMinute.Text)
                    >= Convert.ToInt32(EndHour.Text)*60 + Convert.ToInt32(EndMinute.Text)) {
                        throw new Exception("Ending time is the same or later than starting time");
            }

            bool valid = false;
            foreach (Label l in Labels) {
                sb.Append((l.Foreground == Brushes.Black) ? "1" : "0");
                valid |= (l.Foreground == Brushes.Black);
            }
            if (!valid) {
                throw new Exception("No day of week selected");
            }
            sb.Append((Kak.Foreground == Brushes.Black) ? ",1," : ",0,");
            sb.Append(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            return sb.ToString();
        }

        void FillData(Program p)
        {
            Label[] Labels = new Label[] { Sun, Mon, Tue, Wed, Thu, Fri, Sat };

            TitleTextBox.Text = p.Title;
            StartHour.Text    = p.StartHour.ToString();
            StartMinute.Text  = p.StartMinute.ToString();
            EndHour.Text      = p.EndHour.ToString();
            EndMinute.Text    = p.EndMinute.ToString();

            for (int i=0; i<7; i++) {
                Labels[i].Foreground = (p.Youbi[i] ? Brushes.Black : Brushes.Gray);
            }
            Kak.Foreground = (p.Kakushu ? Brushes.Black : Brushes.Gray);
        }

        void ShowListLayout()
        {
            ListButtonImage.Visibility = Visibility.Hidden;
            AddButtonImage.Visibility  = Visibility.Visible;
            LayoutRootGrid.RowDefinitions[1].Height = new GridLength(0);
            LayoutRootGrid.RowDefinitions[2].Height = new GridLength(15, GridUnitType.Star);
            LayoutRootGrid.RowDefinitions[3].Height = new GridLength(0);
            AddButtonGrid.Visibility        = Visibility.Hidden;
            EditDeleteButtonGrid.Visibility = Visibility.Visible;

            displayer.DisplayPrograms(Programs);
        }

        void ShowEditLayout()
        {
            ListButtonImage.Visibility = Visibility.Visible;
            AddButtonImage.Visibility  = Visibility.Hidden;
            LayoutRootGrid.RowDefinitions[1].Height = new GridLength(15, GridUnitType.Star);
            LayoutRootGrid.RowDefinitions[2].Height = new GridLength(0);
            LayoutRootGrid.RowDefinitions[3].Height = new GridLength(0);
            AddButtonGrid.Visibility        = Visibility.Visible;
            EditDeleteButtonGrid.Visibility = Visibility.Hidden;
        }

        void ShowSettingLayout()
        {
            ListButtonImage.Visibility = Visibility.Visible;
            AddButtonImage.Visibility  = Visibility.Hidden;
            LayoutRootGrid.RowDefinitions[1].Height = new GridLength(0);
            LayoutRootGrid.RowDefinitions[2].Height = new GridLength(0);
            LayoutRootGrid.RowDefinitions[3].Height = new GridLength(15, GridUnitType.Star);
            AddButtonGrid.Visibility        = Visibility.Hidden;
            EditDeleteButtonGrid.Visibility = Visibility.Visible;

            DisplaySettings();
        }
    }
}
