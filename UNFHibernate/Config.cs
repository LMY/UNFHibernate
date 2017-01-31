using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNFHibernate
{
    class Config
    {
        public static Config Instance { get; private set; }

        public static void Init()
        {
            Instance = new Config();
        }


        public String ConnectionString { get; set; }

        public String[] Cartellini { get; private set; }
        public String[] TipologiaCorsi { get; private set; }
        public String[] ModalitaPagamento { get; private set; }

        public int RefreshAnagrafica { get; private set; }
        public int RefreshAltre { get; private set; }

        public bool ShowRefreshTime { get; private set; }
        public bool ShowColumnDovuto { get; private set; }

        public bool QuitWordAfterPrintCartellino { get; private set; }
        public bool QuitWordAfterPrintPagamento { get; private set; }
        public bool QuitWordAfterPrintLibroSoci { get; private set; }
        public bool QuitWordAfterPrintIngressiGiorno { get; private set; }

        public bool ShowErrors { get; private set; }
        public bool promptBackupOnExit { get; private set; }
        public bool writeLogOnExit { get; private set; }
        
        public double IVA { get; private set; }
        
        public Color ColorNonPagato { get; private set; }

        public String LogFilename { get; private set; }
        public Log.LogLevel LogLevel { get; private set; }


        private Config()
        {
            //ConfigurationManager.AppSettings["ConnectionString"].ToString();
            ConnectionString = ConfigurationManager.AppSettings["ConnectionString"].ToString();

            try { Cartellini = LeggiTxt(".\\Resources\\cartellini.txt"); }
            catch { /*MessageBox.Show("Impossibile leggere il file dei cartellini");*/ Cartellini = new String[0]; }


            try { TipologiaCorsi = LeggiTxt(".\\Resources\\tipicorso.txt"); }
            catch { /*MessageBox.Show("Impossibile leggere il file dei cartellini");*/ TipologiaCorsi = new String[0]; }

            try { ModalitaPagamento = LeggiTxt(".\\Resources\\modalitapagamenti.txt"); }
            catch { ModalitaPagamento = new String[0]; }

            try { RefreshAnagrafica = Int32.Parse(ConfigurationManager.AppSettings["RefreshAnagrafica"].ToString()); }
            catch { RefreshAnagrafica = 2000; }

            try { IVA = Double.Parse(ConfigurationManager.AppSettings["IVA"].ToString()); }
            catch { IVA = 21.0f; }

            try { RefreshAltre = Int32.Parse(ConfigurationManager.AppSettings["RefreshAltre"].ToString()); }
            catch { RefreshAltre = 2000; }

            try { ShowColumnDovuto = ConfigurationManager.AppSettings["ShowColumnDovuto"].ToString().Equals("true"); }
            catch { ShowColumnDovuto = false; }

            try { ShowRefreshTime = ConfigurationManager.AppSettings["ShowRefreshTime"].ToString().Equals("true"); }
            catch { ShowRefreshTime = false; }

            try { QuitWordAfterPrintCartellino = ConfigurationManager.AppSettings["QuitWordAfterPrintCartellino"].ToString().Equals("true"); }
            catch { QuitWordAfterPrintCartellino = false; }

            try { QuitWordAfterPrintPagamento = ConfigurationManager.AppSettings["QuitWordAfterPrintPagamento"].ToString().Equals("true"); }
            catch { QuitWordAfterPrintPagamento = false; }

            try { QuitWordAfterPrintLibroSoci = ConfigurationManager.AppSettings["QuitWordAfterPrintLibroSoci"].ToString().Equals("true"); }
            catch { QuitWordAfterPrintLibroSoci = false; }

            try { QuitWordAfterPrintIngressiGiorno = ConfigurationManager.AppSettings["QuitWordAfterPrintIngressiGiorno"].ToString().Equals("true"); }
            catch { QuitWordAfterPrintIngressiGiorno = false; }

            try { ShowErrors = ConfigurationManager.AppSettings["ShowErrors"].ToString().Equals("true"); }
            catch { ShowErrors = false; }

            try { promptBackupOnExit = ConfigurationManager.AppSettings["promptBackupOnExit"].ToString().Equals("true"); }
            catch { promptBackupOnExit = false; }

            try { writeLogOnExit = ConfigurationManager.AppSettings["writeLogOnExit"].ToString().Equals("true"); }
            catch { writeLogOnExit = false; }

            try { ColorNonPagato = Color.FromName(ConfigurationManager.AppSettings["ColorNonPagato"].ToString()); }
            catch { ColorNonPagato = Color.Red; }


            try { LogFilename = ConfigurationManager.AppSettings["LogFilename"].ToString(); }
            catch { LogFilename = "unf_log.txt"; }

            try { LogLevel = Log.Name2Loglevel(ConfigurationManager.AppSettings["LogLevel"].ToString()); }
            catch { LogLevel = Log.LogLevel.Warning; }

        }

        public String[] LeggiTxt(string filename)
        {
            string line;

            List<string> crt = new List<string>();

            // Read the file and display it line by line.
            System.IO.StreamReader file = new System.IO.StreamReader(filename);
            while ((line = file.ReadLine()) != null)
                crt.Add(line);

             return crt.ToArray<String>();
        }



    }
}
