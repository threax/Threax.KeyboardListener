using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        bool sleepMode = false;

        public DetectorForm()
        {
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

            ToggleProgram = "toggle.bat";
            SleepProgram = "sleep.bat";
            WakeProgram = "wake.bat";
            SystemProgram = "system.bat";
            TvProgram = "tv.bat";
            SoundProgram = "sound.bat";
        }

        public String ToggleProgram { get; set; }

        public String SleepProgram { get; set; }

        public String WakeProgram { get; set; }

        public String SystemProgram { get; set; }

        public String TvProgram { get; set; }

        public String SoundProgram { get; set; }

        public bool SleepMode
        {
            get
            {
                return sleepMode;
            }
            set
            {
                sleepMode = value;
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_POWERBROADCAST:
                    switch (m.WParam.ToInt64())
                    {
                        case PBT_APMSUSPEND:
                            break;
                        case PBT_APMRESUMESUSPEND:
                            runProgram(WakeProgram, false);
                            break;
                    }
                    break;
                case WM_HOTKEY:
                    switch (m.WParam.ToInt64())
                    {
                        case SleepEvent:
                            if (sleepMode)
                            {
                                runProgram(SleepProgram, true);
                                Application.SetSuspendState(PowerState.Suspend, true, true);
                            }
                            else
                            {
                                runProgram(ToggleProgram, false);
                            }
                            break;
                        case TvEvent:
                            runProgram(TvProgram, false);
                            break;
                        case SoundEvent:
                            runProgram(SoundProgram, false);
                            break;
                        case SystemEvent:
                            runProgram(SystemProgram, false);
                            break;
                    }
                    break;
            }
            base.WndProc(ref m);
        }

        private void runProgram(String program, bool waitForExit)
        {
            Console.WriteLine(program);
            //if (!String.IsNullOrEmpty(program))
            //{
            //    if (File.Exists(program))
            //    {
            //        try
            //        {
            //            var startInfo = new ProcessStartInfo();
            //            startInfo.FileName = program;
            //            if (!showWindowsToolStripMenuItem.Checked)
            //            {
            //                startInfo.CreateNoWindow = true;
            //                startInfo.UseShellExecute = false;
            //            }
            //            var process = Process.Start(startInfo);
            //            if (waitForExit)
            //            {
            //                process.WaitForExit();
            //            }
            //        }
            //        catch (Exception) { }
            //    }
            //    else if(showWindowsToolStripMenuItem.Checked)
            //    {
            //        MessageBox.Show(String.Format("Cannot find program '{0}'", program), "Progam not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, int vk);
    }
}
