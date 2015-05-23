using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace AGQRecorder0
{
    public partial class MainWindow : System.Windows.Window
    {
        private void MakeDraggable()
        {
            BorderGrid.MouseDown += BorderGrid_MouseDown;
        }

        void BorderGrid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left) {
                this.DragMove();
            }
        }

    }
}
