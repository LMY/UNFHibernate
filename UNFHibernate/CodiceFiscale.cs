using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UNFHibernate
{
    class CodiceFiscale
    {
        public static readonly string fileCodiciComuni = ".\\Resources\\CFComuni.txt";


        public static void init()
        {
            CodiciComune = initCodiciComuni(fileCodiciComuni);
        }

        public static void shutdown()
        {
            if (CodiciComune != null)
                CodiciComune.Clear();
        }

        public static String calculate(String nome, String cognome, DateTime nascita, String comnas, bool maschio)
        {
            nome = nome.ToUpper();
            cognome = cognome.ToUpper();
            comnas = comnas.ToUpper();

            String res = getThreeLetters(cognome, false);
            res += getThreeLetters(nome, true);
            res += nascita.Year.ToString().Substring(2, 2);
            res += getMeseNascita(nascita.Month);
            res += (nascita.Day + (maschio ? 0 : 40)).ToString("00");
            res += getCodiceComune(comnas);
            res += getChecksum(res);

            return res;
        }

        public static String getCodiceComune(String comune)
        {
            try { return CodiciComune[comune]; }
            catch { return "X000"; }
        }

        protected static Dictionary<String, String> CodiciComune = null;
        public static Dictionary<String, String> initCodiciComuni(String filename)
        {
            try
            {
                Dictionary<String, String> ret = new Dictionary<String, String>();

                using (StreamReader sr = new StreamReader(filename))
                {
                    string line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        String[] tok = line.Split(';');
                        try { ret.Add(tok[0], tok[1]); }
                        catch { }
                    }
                }

                return ret;
            }
            catch (Exception e)
            {
                Log.Instance.WriteLine(Log.LogLevel.Warning, "Errore caricando il file CodiceFiscale::comune(" + filename + "): " + e.Message);
                return new Dictionary<String, String>();
            }
        }

        public static readonly char[] vowels = { 'A', 'E', 'I', 'O', 'U' };
        public static string getThreeLetters(String input, bool nome)
        {
            String res = "";

            for (int i = 0; i < input.Length; i++)
                if (Char.IsNumber(input[i]) || Char.IsLetter(input[i]))
                    if (!vowels.Contains(input[i]))
                        res += input[i];

            if (nome && res.Length > 3)
                return "" + res[0] + res[2] + res[3];
            else if (res.Length == 3)
                return res.Substring(0, 3);

            for (int i = 0; i < input.Length; i++)
                if (Char.IsNumber(input[i]) || Char.IsLetter(input[i]))
                    if (vowels.Contains(input[i]))
                        res += input[i];

            //if (nome && res.Length > 3)
            //    return "" + res[0] + res[2] + res[3];
            /*else*/ if (res.Length >= 3)
                return res.Substring(0, 3);

            while (res.Length < 3)
                res = res + "X";

            //if (nome && res.Length > 3)
            //    return "" + res[0] + res[2] + res[3];
            //else
                return res.Substring(0, 3);
        }

        public static readonly char[] mesiCodici = { 'A', 'B', 'C', 'D', 'E', 'H', 'L', 'M', 'P', 'R', 'S', 'T' };
        public static char getMeseNascita(int mese)
        {
            return mese >= 1 && mese <= 12 ? mesiCodici[mese - 1] : 'X';
        }

        public static readonly int[] checksumCodici = { 1, 0, 5, 7, 9, 13, 15, 17, 19, 21, 2, 4, 18, 20, 11, 3, 6, 8, 12, 14, 16, 10, 22, 25, 24, 23 };
        public static char getChecksum(string input)
        {
            int cin = 0;

            for (int i=0; i<input.Length; i++)
            {
                int n = input[i] - (Char.IsNumber(input[i]) ? '0' : 'A');
                cin += (i % 2 == 0 ? checksumCodici[n] : n);
            }

            return Convert.ToChar(65 + (cin - (cin/26)*26));
        }


        /*
        public static String getCodiceComuneOnTheFly(String comune)
        {
            try
            {
                using (StreamReader sr = new StreamReader(fileCodiciComuni))
                {
                    string line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        String[] tok = line.Split(';');

                        if (tok[0].Equals(comune, StringComparison.InvariantCultureIgnoreCase))
                            return tok[1];
                    }
                }

                return string.Empty;
            }
            catch
            {
                return String.Empty;
            }
        }
        */
    }
}
