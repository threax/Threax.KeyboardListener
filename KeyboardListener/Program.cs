using KeyboardListener;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Threax.Extensions.Configuration.SchemaBinder;

namespace SleepDetector
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                var appDir = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);

                var configBuilder = new ConfigurationBuilder();
                configBuilder.AddJsonFile(Path.Combine(appDir, "appsettings.json"));
                configBuilder.AddJsonFile(Path.Combine(appDir, "appsettings.Production.json"), optional: true);
                configBuilder.AddJsonFile(Path.Combine(appDir, "appsettings.secrets.json"), optional: true);
                var configuration = new SchemaConfigurationBinder(configBuilder.Build());

                var eventConfig = new EventConfig();
                configuration.Bind("Events", eventConfig);

                //setup our DI
                var serviceProvider = new ServiceCollection()
                    .AddSingleton<DetectorForm>()
                    .AddSingleton<EventConfig>(eventConfig)
                    .AddHomeClient(o => configuration.Bind("HomeClient", o))
                    .BuildServiceProvider();

                DetectorForm form = serviceProvider.GetRequiredService<DetectorForm>();
                Application.Run(form);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
