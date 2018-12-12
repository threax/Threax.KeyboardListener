using KeyboardListener;
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Threax.Home.Client;

namespace SleepDetector
{
    enum KeyEvent
    {
        MainEvent = 0,
        TvEvent = 1,
        SoundEvent = 2,
        LightEvent = 3,
    }

    public partial class DetectorForm : Form
    {
        //vk codes http://msdn.microsoft.com/en-us/library/windows/desktop/dd375731%28v=vs.85%29.aspx
        const int WM_HOTKEY = 0x0312;

        private EntryPointInjector entryPoint;
        private EventConfig eventConfig;

        public DetectorForm(EntryPointInjector entryPoint, EventConfig eventConfig)
        {
            this.entryPoint = entryPoint;
            this.eventConfig = eventConfig;

            InitializeComponent();

            MakeHiddenWindow();

            RegisterHotKey(this.Handle, (int)KeyEvent.MainEvent, 0x0004 | 0x4000, (int)Keys.Escape);
            RegisterHotKey(this.Handle, (int)KeyEvent.TvEvent, 0x0001 | 0x0002 | 0x4000, (int)Keys.T);
            RegisterHotKey(this.Handle, (int)KeyEvent.SoundEvent, 0x0001 | 0x0002 | 0x4000, (int)Keys.S);
            RegisterHotKey(this.Handle, (int)KeyEvent.LightEvent, 0x0001 | 0x0002 | 0x4000, (int)Keys.L);
        }

        private async Task HotKeyEvent(KeyEvent evt)
        {
            //This can be made better, dont have to hard code like this
            switch (evt)
            {
                case KeyEvent.MainEvent:
                    await RunEvent(this.eventConfig.Main);
                    break;
                case KeyEvent.TvEvent:
                    await RunEvent(this.eventConfig.Tv);
                    break;
                case KeyEvent.SoundEvent:
                    await RunEvent(this.eventConfig.Sound);
                    break;
                case KeyEvent.LightEvent:
                    await RunEvent(this.eventConfig.Light);
                    break;
            }            
        }

        private async Task RunEvent(KeyboardListener.HotkeyEvent evt)
        {
            var entry = await entryPoint.Load();
            var lookup = evt.Actions.Select(i => i.SwitchId).ToList();
            var switches = await entry.ListSwitches(new SwitchQuery()
            {
                SwitchIds = lookup,
                Limit = int.MaxValue
            });

            foreach(var action in evt.Actions)
            {
                var sw = switches.Items.First(i => i.Data.SwitchId == action.SwitchId);
                await sw.Set(new SetSwitchInput()
                {
                    Brightness = action.Brightness,
                    HexColor = action.HexColor,
                    Value = action.Value
                });
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_HOTKEY:
                    var wparam = (KeyEvent)m.WParam.ToInt32();
                    Task.Run(async () =>
                    {
                        try
                        {
                            await HotKeyEvent(wparam);
                        }
                        catch(Exception ex)
                        {
                            Action action = () =>
                            {
                                MessageBox.Show($"A {ex.GetType().Name} occured.\nMessage:\n{ex.Message}");
                            };
                            this.Invoke(action);
                        }
                    });
                    break;
            }
            base.WndProc(ref m);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MakeHiddenWindow()
        {
            //Thanks to Bill at http://stackoverflow.com/questions/683896/any-way-to-create-a-hidden-main-window-in-c
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(-2000, -2000);
            this.Size = new Size(1, 1);
        }

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, int vk);
    }
}
