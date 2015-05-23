using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace AGQRecorder0
{
    class Recorder
    {
        public string server;
        public string rtmp;

        Process recradio;

        public Recorder(string _timestamp, string _duration)
        {

            server = (System.Windows.Application.Current.MainWindow as MainWindow).server;
            rtmp = (System.Windows.Application.Current.MainWindow as MainWindow).rtmp;

            recradio = new Process();

            //recradio.StartInfo.RedirectStandardOutput = true;
            recradio.StartInfo.UseShellExecute = false;
            recradio.StartInfo.CreateNoWindow = false;
            recradio.StartInfo.FileName = rtmp;

        }

        public void Start(string filename, string duration)
        {
            recradio.StartInfo.Arguments = String.Format("-r {0} --live -B {1} -o \"{2}\" --timeout 10", server, duration, filename);
            recradio.Start();
        }

        public bool IsRunning()
        {
            try {
                return !recradio.HasExited;
            }
            catch {
                return false;
            }
        }
    }
}
