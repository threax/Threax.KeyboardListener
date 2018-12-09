using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SleepDetector
{
    static class Program
    {
        static string ConfigFileName = "SleepConfig.ini";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            DetectorForm form = new DetectorForm();

            //ConfigFile config = new ConfigFile(ConfigFileName);
            //config.loadConfigFile();
            //var section = config.createOrRetrieveConfigSection("Program");
            form.SleepMode = false;  //section.getValue("SleepMode", false);

            try
            {
                //if (!File.Exists(ConfigFileName))
                //{
                //    config.writeConfigFile();
                //}
            }
            catch (Exception)
            {
                //don't care
            }

            Application.Run(form);
        }
    }
}
