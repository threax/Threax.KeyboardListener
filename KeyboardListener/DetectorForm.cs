using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Threax.Home.Client;

namespace SleepDetector
{
    public partial class DetectorForm : Form
    {
        //vk codes http://msdn.microsoft.com/en-us/library/windows/desktop/dd375731%28v=vs.85%29.aspx

        const int WM_HOTKEY = 0x0312;

        const int WM_POWERBROADCAST = 536;
        const int PBT_APMSUSPEND = 4;
        const int PBT_APMRESUMESUSPEND = 7;

        const int SleepEvent = 0;
        const int TvEvent = 1;
        const int SoundEvent = 2;
        const int SystemEvent = 3;

        private EntryPointInjector entryPoint;

        public DetectorForm(EntryPointInjector entryPoint)
        {
            this.entryPoint = entryPoint;

            InitializeComponent();

            //Thanks to Bill at http://stackoverflow.com/questions/683896/any-way-to-create-a-hidden-main-window-in-c
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(-2000, -2000);
            this.Size = new Size(1, 1);

            RegisterHotKey(this.Handle, SleepEvent, 0x0004 | 0x4000, (int)Keys.Escape);
            RegisterHotKey(this.Handle, TvEvent, 0x0001 | 0x0002 | 0x4000, (int)Keys.T);
            RegisterHotKey(this.Handle, SoundEvent, 0x0001 | 0x0002 | 0x4000, (int)Keys.S);
            RegisterHotKey(this.Handle, SystemEvent, 0x0001 | 0x0002 | 0x4000, (int)Keys.P);
        }

        private Task HotKeyEvent(long evt)
        {
            Console.WriteLine(evt);
            return Task.CompletedTask;
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                //This is wake
                //case WM_POWERBROADCAST:
                //    switch (m.WParam.ToInt64())
                //    {
                //        case PBT_APMSUSPEND:
                //            break;
                //        case PBT_APMRESUMESUSPEND:
                //            runProgram(WakeProgram, false);
                //            break;
                //    }
                //    break;
                case WM_HOTKEY:
                    var wparam = m.WParam.ToInt64();
                    Task.Run(() => HotKeyEvent(wparam));
                    break;
            }
            base.WndProc(ref m);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, int vk);
    }
}
