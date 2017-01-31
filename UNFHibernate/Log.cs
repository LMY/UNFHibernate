using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace UNFHibernate
{
    public class Log
    {
        public static readonly String DEFAULT_FILENAME = "Log.txt";
        public static readonly LogLevel DEFAULT_LOGLEVEL = LogLevel.Info;

        public enum LogLevel { Never=0, Nolog=1, Info=2, Dev=3, Warning=4, Exception=5, Error=6, All=7, Always=8 };
        public static readonly String[] LogLevelNames = { "Never", "NoLog", "Info", "Dev", "Warning", "Exception", "Error", "All", "Always" };


        #region SINGLETON
        public static Log Instance { get; private set; }

        public static bool init()                               { return init(DEFAULT_LOGLEVEL); }
        public static bool init(LogLevel lvl)                   { return init(lvl, DEFAULT_FILENAME); }
        public static bool init(String filename)                { return init(DEFAULT_LOGLEVEL, DEFAULT_FILENAME); }
        public static bool init(LogLevel lvl, String filename)
        {
            Instance = new Log(filename);
            return true;
        }
        #endregion



        public String Filename { get; private set; }
        private StreamWriter sw { get; set; }

        public bool UseTimeStamp { get; set; }
        public LogLevel Loglevel { get; set; }


        private Log(String filename)
            : this(filename, DEFAULT_LOGLEVEL)
        { }

        private Log(String filename, LogLevel min_loglevel)
            : this(filename, min_loglevel, true)
        { }

        private Log(String filename, LogLevel min_loglevel, bool timestamp)
            : this(filename, min_loglevel, timestamp, true)
        { }

        private Log(String filename, LogLevel min_loglevel, bool timestamp, bool append)
        {
            Filename = filename;
            Loglevel = min_loglevel;
            UseTimeStamp = timestamp;
            sw = new StreamWriter(filename, append);
        }


        public void Write()                 { Write(DEFAULT_LOGLEVEL); }
        public void WriteLine()             { WriteLine(DEFAULT_LOGLEVEL); }
        public void Write(String s)         { Write(DEFAULT_LOGLEVEL, s); }
        public void WriteLine(String s)     { WriteLine(DEFAULT_LOGLEVEL, s); }
        public void Write(LogLevel lvl)     { Write(lvl, String.Empty); }
        public void WriteLine(LogLevel lvl) { WriteLine(lvl, String.Empty); }

        public void WriteLine(LogLevel lvl, String line)
        {
            Write(line+Environment.NewLine);
        }


        public void Write(LogLevel lvl, String text)
        {
            if (lvl < Loglevel)
                return;
#if DEBUG
            Console.Write(text);
#endif
            try
            {
                sw.Write((UseTimeStamp ? getTimestamp() + " " : "") + Loglevel2Name(lvl)+"\t"+ text);
                sw.Flush();
            }
            catch { }

            if (Config.Instance.ShowErrors && lvl > Log.LogLevel.Info)
                MessageBox.Show(text, Loglevel2Name(lvl), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        public static String getTimestamp()
        {            
            return DateTime.Now.ToString();
        }

        public static String Loglevel2Name(LogLevel lvl)
        {
            return LogLevelNames[(int)lvl];
            /*
            switch (lvl)
            {
                case LogLevel.Never: return "Never";
                case LogLevel.Nolog: return "NoLog";
                case LogLevel.Dev: return "Dev";
                case LogLevel.Warning: return "Warning";
                case LogLevel.Exception: return "Exception";
                case LogLevel.Error: return "Error";
                case LogLevel.All: return "All";
                case LogLevel.Always: return "Always";
                default:
                case LogLevel.Info: return "Info";
            }*/
        }

        public static LogLevel Name2Loglevel(String name)
        {
            //y per quanto un dizionario sia "più corretto", è più lento
            for (int i = 0; i < LogLevelNames.Length; i++)
                if (name.Equals(LogLevelNames[i], StringComparison.InvariantCultureIgnoreCase))
                    return (LogLevel)i;

            return DEFAULT_LOGLEVEL;

            /*
            if (name.Equals("Never", StringComparison.InvariantCultureIgnoreCase))
                return LogLevel.Never;
            else if (name.Equals("NoLog", StringComparison.InvariantCultureIgnoreCase))
                return LogLevel.Nolog;
            else if (name.Equals("Dev", StringComparison.InvariantCultureIgnoreCase))
                return LogLevel.Dev;
            else if (name.Equals("Warning", StringComparison.InvariantCultureIgnoreCase))
                return LogLevel.Warning;
            else if (name.Equals("Exception", StringComparison.InvariantCultureIgnoreCase))
                return LogLevel.Exception;
            else if (name.Equals("Error", StringComparison.InvariantCultureIgnoreCase))
                return LogLevel.Error;
            else if (name.Equals("All", StringComparison.InvariantCultureIgnoreCase))
                return LogLevel.All;
            else if (name.Equals("Always", StringComparison.InvariantCultureIgnoreCase))
                return LogLevel.Always;
            else //if (name.Equals("Info", StringComparison.InvariantCultureIgnoreCase))
                return LogLevel.Info;*/
        }
    }
}
