using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AGQRecorder0
{

    class Program
    {
        public string Title { get; set; }

        public int Id;

        public int StartHour;
        public int StartMinute;
        public int EndHour;
        public int EndMinute;
        public bool[] Youbi;
        public bool Kakushu;
        public DateTime DateAdded;

        private Recorder recradio;

        private int[] NextYoubi;
        private DateTime FirstRecordDateTime;
        private DateTime NextRecordDateTime;


        private int getDuration()
        {
            return ((EndHour*60 + EndMinute) - (StartHour*60 + StartMinute)) * 60 + 60;
        }


        /*
         * 0,津田のラジオ「っだー!!」,12,00,12,30,0001000,0,2015/04/01
         */
        public Program(String data)
        {
            string[] line = data.Split(',');

            Youbi = new bool[7];

            Id           = Convert.ToInt32(line[0]);
            Title        = line[1];
            StartHour    = Convert.ToInt32(line[2]);
            StartMinute  = Convert.ToInt32(line[3]);
            EndHour      = Convert.ToInt32(line[4]);
            EndMinute    = Convert.ToInt32(line[5]);
            for (int i=0; i<7; i++) {
                Youbi[i] = (line[6][i] == '1');
            }
            Kakushu      = line[7] == "1";
            DateAdded    = Convert.ToDateTime(line[8]);

            NextYoubi = new int[7];
            for (int i=0; i<7; i++) {
                if (Youbi[i]) {
                    int j=i+1;
                    while (!Youbi[j%7]) {
                        j++;
                    }
                    NextYoubi[i] = j-i;
                    if (Kakushu && j>6) {
                        NextYoubi[i] += 7;
                    }
                }
            }

            FirstRecordDateTime = new DateTime(DateAdded.Year, DateAdded.Month, DateAdded.Day, StartHour%24, StartMinute, 0);
            if (StartHour > 23) {
                FirstRecordDateTime = FirstRecordDateTime.AddDays(1.0);
            }
            if ((DateAdded.Hour*60 + DateAdded.Minute)*60 + DateAdded.Second > (StartHour*60 + StartMinute)*60 - 30) {
                FirstRecordDateTime = FirstRecordDateTime.AddDays(1.0);
            }
            while (!Youbi[(Convert.ToInt32(FirstRecordDateTime.DayOfWeek) + ((StartHour > 23) ? 6 : 0)) % 7]) {
                FirstRecordDateTime = FirstRecordDateTime.AddDays(1.0);
            }
            FirstRecordDateTime = FirstRecordDateTime.AddSeconds(-30.0);
            NextRecordDateTime = GetNextRecordDateTime();


            string timestamp = String.Format("{0:D2}{1:D2}-{2:D2}{3:D2}", StartHour, StartMinute, EndHour, EndMinute);
            string duration  = String.Format("{0}", getDuration());

            recradio = new Recorder(timestamp, duration);
        }

        public string SaveData()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Format("{0},{1},",Id, Title));
            sb.Append(String.Format("{0:D2},{1:D2},{2:D2},{3:D2},", StartHour, StartMinute, EndHour, EndMinute));
            for (int i=0; i<7; i++) {
                sb.Append(Youbi[i] ? "1" : "0");
            }
            sb.Append(String.Format(",{0},",Kakushu ? "1" : "0"));
            sb.Append(DateAdded.ToString("yyyy/MM/dd HH:mm:ss"));

            return sb.ToString();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Format("{0} ", Title));
            foreach (bool y in Youbi) {
                sb.Append(y ? "1" : "0");
            }
            sb.Append(" " + GetNextRecordDateTime().ToString());
            return sb.ToString();
        }

        public DateTime GetNextRecordDateTime()
        {
            DateTime NextRecordDateTime = FirstRecordDateTime;

            while (NextRecordDateTime < DateTime.Now) {
                NextRecordDateTime = NextRecordDateTime.AddDays(NextYoubi[Convert.ToInt32(NextRecordDateTime.DayOfWeek)]);
            }

            return NextRecordDateTime;
        }

        public void RunRecorder()
        {
            string filename = 
                System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                                       String.Format("[{0:D4}.{1:D2}.{2:D2}] {3:D2}{4:D2}-{5:D2}{6:D2}.flv",
                                                        DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                                                    StartHour, StartMinute, EndHour, EndMinute));
            string duration = String.Format("{0}", getDuration());
            Console.WriteLine("Record Started");
            recradio.Start(filename, duration);
        }

        public bool IsRecording()
        {
            if (recradio.IsRunning()) {
                return true;
            }
            return false;
        }

        public void setRecorderProgram(string file)
        {
            recradio.rtmp = file;
        }

        public void setRecordingServer()
        {
            // if needed
        }

    }
}
