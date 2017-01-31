using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UNFHibernate
{
    public class Utils
    {
        public static void SetPickerValid(DateTimePicker picker, bool enable)
        {
            if (enable)
                picker.Format = DateTimePickerFormat.Short;
            else
            {
                picker.CustomFormat = " ";
                picker.Format = DateTimePickerFormat.Custom;
            }
        }

        public static void SetPickerValidIfEnabled(DateTime? dt, ref DateTimePicker picker, bool enable = false)
        {
            if (dt != null)
                try { picker.Value = dt.Value; }
                catch { }
            else
            {
                if (enable)
                    picker.Format = DateTimePickerFormat.Short;
                else
                {
                    picker.CustomFormat = " ";
                    picker.Format = DateTimePickerFormat.Custom;
                }
            }
        }

        public static String moneylongToString(long x)
        {
            if (x == 0) return String.Empty;
            float r = (float)x / 100;
            return r.ToString("N2");
        }

        public static long moneystringToLong(String s)
        {
            try {
                float f = float.Parse(s);
                return (long)Math.Round(100 * f);
            }
            catch { return 0; }
        }

        public static readonly String[] giorniSettimana = { "Lun", "Mar", "Mer", "Gio", "Ven", "Sab", "Dom" };

        public static String giornateToString(bool[] giorni)
        {
            String ret = string.Empty;
            int count = 0;

            for (int i = 0; i < giorni.Length; i++)
                if (giorni[i])
                {
                    ret += (count==0 ? "" : ", ") + giorniSettimana[i];
                    count++;
                }

            if (count == 0)
                return string.Empty;
            else if (count == 7)
                return "Tutta la settimana";

            return ret;
        }


        public static readonly String AttivoTrue = "S";
        public static readonly String AttivoFalse = "N";

        public static bool isAttivo(String s)
        {
            return s != null ? s.Equals(AttivoTrue) : false;
        }

        public static String toAttivo(bool b)
        {
            return b ? AttivoTrue : AttivoFalse;
        }

        public static void anagraficheToCsv(string filename, DataGridView p2)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                String intestazione = string.Empty;
                int adds=0;

                foreach (DataGridViewColumn c in p2.Columns)
                    intestazione += (adds++ > 0 ? ";" : string.Empty) + c.Name;

                sw.WriteLine(intestazione);

                foreach (DataGridViewRow r in p2.Rows)
                {
                    String line = string.Empty;
                    adds = 0;

                    foreach (DataGridViewCell cell in r.Cells)
                    {
                        String content = cell.Value == null ? string.Empty : cell.Value.ToString();
                        line += (adds++ > 0 ? ";" : string.Empty) + content;
                    }

                    sw.WriteLine(line);
                }
            }
        }

        public static String dateToPrintableString(DateTime? d)
        {
            if (d == null)
                return string.Empty;
            return dateToPrintableString(d.Value);
        }

        public static String dateToPrintableString(DateTime d)
        {
            return d.ToString("d");
        }
    }


    public class EntryOrario
    {
        public String Giornate { get; set; }
        public DateTime Dalle { get; set; }
        public DateTime Alle { get; set; }
        public String Corsie { get; set; }

        public EntryOrario(String line)
        {
            Dalle = Alle = new DateTime(2000, 1, 1, 1, 1, 1);
            Corsie = string.Empty;
            Giornate = string.Empty;

            String[] a = line.Split('@');
            String[] giornate = a[0].Split(',');

            foreach (String g in giornate)
                if (g.StartsWith("l") || g.Equals("1"))
                {
                    if (!String.IsNullOrEmpty(Giornate)) Giornate += ",";
                    Giornate += "1";
                }
                else if (g.StartsWith("ma") || g.Equals("2"))
                {
                    if (!String.IsNullOrEmpty(Giornate)) Giornate += ",";
                    Giornate += "2";
                }
                else if (g.StartsWith("me") || g.Equals("3"))
                {
                    if (!String.IsNullOrEmpty(Giornate)) Giornate += ",";
                    Giornate += "3";
                }
                else if (g.StartsWith("g") || g.Equals("4"))
                {
                    if (!String.IsNullOrEmpty(Giornate)) Giornate += ",";
                    Giornate += "4";
                }
                else if (g.StartsWith("v") || g.Equals("5"))
                {
                    if (!String.IsNullOrEmpty(Giornate)) Giornate += ",";
                    Giornate += "5";
                }
                else if (g.StartsWith("s") || g.Equals("6"))
                {
                    if (!String.IsNullOrEmpty(Giornate)) Giornate += ",";
                    Giornate += "6";
                }
                else if (g.StartsWith("d") || g.Equals("7"))
                {
                    if (!String.IsNullOrEmpty(Giornate)) Giornate += ",";
                    Giornate += "7";
                }

            // a[1] è hmm-hmm(corsie)
            if (a.Length > 1)
            {
                int posmeno = a[1].IndexOf('-');
                int posparap = a[1].IndexOf('/');

                String orada = posmeno >= 0 ? a[1].Substring(0, posmeno) : string.Empty;
                String oraa = posmeno >= 0 && posparap >= 0 ? a[1].Substring(posmeno + 1, posparap - posmeno - 1) : string.Empty;

                if (!String.IsNullOrEmpty(orada))
                {
                    Dalle = DateTime.ParseExact(orada, "H:mm", CultureInfo.InstalledUICulture);

                    if (!String.IsNullOrEmpty(oraa))
                    {
                        Alle = DateTime.ParseExact(oraa, "H:mm", CultureInfo.InstalledUICulture);
                        Corsie = a[1].Substring(posparap + 1);
                    }
                }
            }
        }

        public EntryOrario()
        {
            Dalle = Alle = new DateTime(2000, 1, 1, 1, 1, 1);
            Corsie = string.Empty;
            Giornate = string.Empty;
        }

        public override String ToString()
        {
            return Giornate + "@" + Dalle.ToString("H:mm") + "-" + Alle.ToString("H:mm") + "/" + Corsie;
        }

        internal bool hasDay(int i)
        {
            return Giornate.Contains(i.ToString());
        }

        public static List<EntryOrario> getOrarioList(String orari)
        {
            List<EntryOrario> ret = new List<EntryOrario>();

            if (!String.IsNullOrEmpty(orari))
            {
                String[] entries = orari.Split(';');
                foreach (String e in entries)
                    ret.Add(new EntryOrario(e));
            }

            return ret;
        }

        public static String getOrarioString(List<EntryOrario> lista)
        {
            String ret = string.Empty;

            foreach (EntryOrario eo in lista)
            {
                if (!string.IsNullOrEmpty(ret))
                    ret += ";";
                ret += eo.ToString();
            }

            return ret;
        }

        public int getNGiorniSettimana()
        {
            int count = 0;

            for (int i = 1; i <= 7; i++)
                if (Giornate.Contains(""+i))
                    count++;

            return count;
        }
    }
}
