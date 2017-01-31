using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UNFHibernate
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Config.Init();

            Log.init(Config.Instance.LogLevel, Config.Instance.LogFilename);
            Log.Instance.WriteLine(Log.LogLevel.Always, "Program Started");

            CodiceFiscale.init();

            HibernateHelper.Init();
            DB.init();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
