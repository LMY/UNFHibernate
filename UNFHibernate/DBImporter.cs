using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UNFHibernate.Domain;

namespace UNFHibernate
{
    class DBImporter
    {
        #region General
        public static readonly char[] DefaultSeparator = new char[] { '\t' };
        public static readonly string DefaultSeparatorString = "\t";
        public static readonly string TrueStringValue = "True";

        public static List<String[]> readCsv(String filename, char[] separator = null)
        {
            if (separator == null)
                separator = DefaultSeparator;

            string line;

            List<string[]> crt = new List<string[]>();

            // Read the file and display it line by line.
            try 
            {
                System.IO.StreamReader file = new System.IO.StreamReader(filename);

                while ((line = file.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line) || (!separator.Equals(";") && line.StartsWith(";")))
                        continue;

                    string[] tok = line.Split(separator);
                    crt.Add(tok);
                }
            }
            catch (Exception e)
            {
                Log.Instance.WriteLine("Impossibile leggere i dati dal file: " + filename+"   errore="+e.Message);
                return new List<string[]>();
            }

            return crt;
        }

        public static DateTime convertData(String s)
        {
            if (String.IsNullOrEmpty(s) || s.Equals("[null]"))
                return DateTime.Now;    // questo non succede

            // 01.11.2012, 00:00:00.000
            int dd = Int32.Parse(s.Substring(0, 2));
            int mm = Int32.Parse(s.Substring(3, 2));
            int yy = Int32.Parse(s.Substring(6, 4));

            return new DateTime(yy, mm, dd);
        }

        public static DateTime? convertDataNullable(String s)
        {
            if (String.IsNullOrEmpty(s) || s.Equals("[null]"))
                return null;

            // 01.11.2012, 00:00:00.000
            int dd = Int32.Parse(s.Substring(0, 2));
            int mm = Int32.Parse(s.Substring(3, 2));
            int yy = Int32.Parse(s.Substring(6, 4));

            return new DateTime(yy, mm, dd);
        }

        public static String convertString(String cell)
        {
            return cell.Equals("[null]") ? null : cell;
        }

        public static Dictionary<int, string> readDictionaryIntString(List<String[]> lines)
        {
            Dictionary<int, string> ret = new Dictionary<int, string>();

            for (int i = 1; i < lines.Count; i++)
                try
                {
                    int value = Int32.Parse(lines.ElementAt(i)[0]);
                    ret.Add(value, lines.ElementAt(i)[1]);
                }
                catch { return new Dictionary<int, string>(); }

            return ret;
        }

        public static List<int[]> readDictionaryIntInt(List<String[]> lines)
        {
            List<int[]> ret = new List<int[]>();

            for (int i = 1; i < lines.Count; i++)
                try
                {
                    int key = Int32.Parse(lines.ElementAt(i)[0]);
                    int value = Int32.Parse(lines.ElementAt(i)[1]);
                    ret.Add(new int[] { key, value });
                }
                catch { return new List<int[]>(); }

            return ret;
        }

        internal static int getIntValueFromDictionary(List<int[]> tuples, int id)
        {
            foreach (int[] a in tuples)
                if (a[0] == id)
                    return a[1];

            return -1;
        }

        #endregion

        #region Import From InterBASE
        private static Dictionary<int, Persona> personaDictionary = null;
        private static Dictionary<int, Corso> corsoDictionary = null;
        public static void freeDictionaries()
        {
            personaDictionary.Clear();
            corsoDictionary.Clear();
        }


        public static Persona[] convertPersone(List<String[]> lines)
        {
            personaDictionary = new Dictionary<int, Persona>();
            List<Persona> persone = new List<Persona>();

            for (int i = 1; i < lines.Count; i++)
                try
                {
                    Application.DoEvents();
                    //ID	COGNOME	NOME	DATANASC	LUOGONASC	INDIRIZZO
                    //COMUNE CAP	PROV	CODFISC	NUMTEL
                    //TESSERA DATACERT	DATAISCRIZ	STORICO	TEMP
                    //COGNGEN NOMEGEN	DATANGEN	LUOGOGEN	TASSAISCR
                    //NOTE NUMCELL	EMAIL
                    String[] cell = lines.ElementAt(i);

                    Persona p = new Persona();

                    int id = 0;
                    try { id = Int32.Parse(cell[0]); }
                    catch { }

                    p.Cognome = convertString(cell[1]);
                    p.Nome = convertString(cell[2]);
                    try { p.DataNascita = convertDataNullable(cell[3]); }
                    catch { }
                    p.LuogoNascita = convertString(cell[4]);
                    p.Indirizzo = convertString(cell[5]);
                    p.Comune = convertString(cell[6]);
                    p.CAP = convertString(cell[7]);
                    p.Provincia = convertString(cell[8]);
                    p.CodiceFiscale = convertString(cell[9]);
                    p.NumeroTelefono = convertString(cell[10]);
                    try { p.Tessera = Int32.Parse(cell[11]); } catch{}
                    try { p.DataCertificato = convertDataNullable(cell[12]); }
                    catch { }
                    try { p.DataIscrizione = convertDataNullable(cell[13]); }
                    catch { }

                    String cognomegenitore = convertString(cell[16]);
                    String nomegenitore = convertString(cell[17]);
                    string nascgen = convertString(cell[18]);
                    string luogogen = convertString(cell[19]);

                    p.TassaIscrizione = cell[20]=="S";
                    p.NumeroCellulare = convertString(cell[22]);
                    p.Email = convertString(cell[22]);

                    p.GenitoreNome = nomegenitore;
                    p.GenitoreCognome = cognomegenitore;
                    try { p.GenitoreDataNascita = convertData(nascgen); }
                    catch { }
                    p.GenitoreLuogoNascita = luogogen;

                    persone.Add(p);
                    personaDictionary.Add(id, p);
                }
                catch { }

            return persone.ToArray();
        }

        public static bool savePersone(Persona[] persone)
        {
            return DB.savePersona(persone);
        }

        public static void importPersone(String filename, char[] separator)
        {
            savePersone(convertPersone(readCsv(filename, separator)));
        }

        public readonly static string FormatoOrario = "{0:d/M/yyyy HH:mm:ss}";


        public static Stagione getStagioneForDate(DateTime dt)
        {
            if (dt != null)
                foreach (Stagione s in DB.instance.stagioni)
                    if (s.DataInizio != null && s.DataFine != null)
                        if (s.DataInizio.CompareTo(dt) >= 0 && s.DataFine.CompareTo(dt) <= 0)
                            return s;

            return DB.instance.stagione_corrente;
        }

        // chiusure
        public static Chiusura[] convertChiusure(List<String[]> lines)
        {
            List<Chiusura> chiusure = new List<Chiusura>();

            for (int i = 1; i < lines.Count; i++)
                try
                {
                    String[] cell = lines.ElementAt(i);

                    DateTime data_inizio = convertData(cell[1]);
                    DateTime data_fine = convertData(cell[2]);
                    Stagione stag = getStagioneForDate(data_inizio);

                    chiusure.Add(new Chiusura() { DataInizio = data_inizio, DataFine = data_fine, Descrizione = string.Empty, stagione=stag });
                }
                catch { }

            return chiusure.ToArray();
        }

        public static bool saveChiusure(Chiusura[] chiusure)
        {
            bool ret = true;

            foreach (Chiusura c in chiusure)
                ret &= DB.saveChiusura(c);

            return ret;
        }

        public static void importChiusure(String filename, char[] separator)
        {
            saveChiusure(convertChiusure(readCsv(filename, separator)));
        }


        // stagioni
        public static Stagione[] convertStagioni(List<String[]> lines)
        {
            List<Stagione> stagioni = new List<Stagione>();

            for (int i = 1; i < lines.Count; i++)
                try
                {
                    String[] cell = lines.ElementAt(i);
                    //ID	DESCRIZ	DATAINI	DATAFINE	CORRENTE	FINEQUADRIMESTRE

                    DateTime laDataFine = convertData(cell[3]);

                    DateTime finequad;
                    try { finequad = convertData(cell[6]); }
                    catch { finequad = laDataFine; };


                    stagioni.Add(new Stagione() { DataInizio = convertData(cell[2]), DataFine = laDataFine, Descrizione = cell[1], FineQuadrimestre = finequad });
                }
                catch { }

            return stagioni.ToArray();
        }

        public static bool saveStagioni(Stagione[] stagioni)
        {
            bool ret = true;

            foreach (Stagione s in stagioni)
                ret &= DB.saveStagione(s);

            return ret;
        }

        public static void importStagioni(String filename, char[] separator)
        {
            saveStagioni(convertStagioni(readCsv(filename, separator)));
        }





        // corsi
        public static Corso[] convertCorsi(List<String[]> lines, Dictionary<int, string> orari, List<int[]> corsiorari)
        {
            corsoDictionary = new Dictionary<int, Corso>();
            List<Corso> corsi = new List<Corso>();

            for (int i = 1; i < lines.Count; i++)
                try
                {
                    Application.DoEvents();
                    String[] cell = lines.ElementAt(i);

                    // ID	CODICE	DESCRIZ	DATAINI	DATAFINE	NUMMAX	ATTIVO	CODSTAMPA	TIPOCART	BIMBO	PERIODICO	PERIODO

                    Corso c = new Corso();
                    int id = -1;
                    try { id = Int32.Parse(cell[0]); }
                    catch { }
                    c.Codice = convertString(cell[1]);
                    c.Descrizione = convertString(cell[2]);
                    try { c.DataInizio = convertData(cell[3]); }
                    catch { }
                    try { c.DataFine = convertData(cell[4]); }
                    catch { }
                    try { c.MaxIscritti = Int32.Parse(convertString(cell[5])); }
                    catch { }

                    try
                    {
                        c.Orario = orari[getIntValueFromDictionary(corsiorari, id)];
                    }
                    catch { }


                    c.Attivo = convertString(cell[6]);
                    c.CodiceStampa = convertString(cell[7]);
                    c.TipoCartellino = convertString(cell[8]);
                    c.Bimbi = cell[9].Equals("S");
                    //c.Periodo = convertString(cell[10]);
                    c.stagione = getStagioneForDate(c.DataInizio);

                    corsi.Add(c);
                    corsoDictionary.Add(id, c);
                }
                catch { }

            return corsi.ToArray();
        }

        public static bool saveCorsi(Corso[] corsi)
        {
            bool ret = true;

            foreach (Corso c in corsi)
                ret &= DB.saveCorso(c);

            return ret;
        }

        public static void importCorsi(String filename, String filename_orari, String filename_corsiorari, char[] separator)
        {
            var corsi = readCsv(filename, separator);
            var orari = readDictionaryIntString(readCsv(filename_orari, separator));
            var asscorsi = readDictionaryIntInt(readCsv(filename_corsiorari, separator));

            saveCorsi(convertCorsi(corsi, orari, asscorsi));
        }



        // iscrizioni
        public static Iscrizione[] convertIscrizioni(List<String[]> lines)
        {
            List<Iscrizione> iscrizioni = new List<Iscrizione>();

            for (int i = 1; i < lines.Count; i++)
                try
                {
                    Application.DoEvents();
                    String[] cell = lines.ElementAt(i);

                    // ID	ANAGRAFICO	STAGIONE	CORSO	SCADENZA	DOVUTO
                    //SALDATO	DATARATA1	IMPRATA1	DATARATA2	IMPRATA2
                    //RINUNCIA	BANCOMAT	DATAINI	DATAMOD	IMPPAGATO
                    //GIORNATE	ORARIO	PERIODO	INGRESSI	PROVA

                    int id = 0;
                    try { id = Int32.Parse(cell[0]); }
                    catch { }

                    int idpers = 0;
                    try { idpers = Int32.Parse(cell[1]); }
                    catch { }

                    int idstagione = 0;
                    try { idstagione = Int32.Parse(cell[2]); }
                    catch { }

                    int idcorso = 0;
                    try { idcorso = Int32.Parse(cell[3]); }
                    catch { }

                    Iscrizione iscrizione = new Iscrizione();

                    iscrizione.persona = personaDictionary[idpers];
                    iscrizione.corso = corsoDictionary[idcorso];

                    DateTime? scadenza = convertDataNullable(cell[4]);

                    double dovuto = 0;
                    try { dovuto = Double.Parse(cell[5]); }
                    catch { }

                    iscrizione.Saldato = cell[6].Equals("S");


                    iscrizione.primopagamento_data = convertDataNullable(cell[7]);
                    iscrizione.primopagamento_importo = 0;
                    try { iscrizione.primopagamento_importo = Utils.moneystringToLong(cell[8]); }
                    catch { }

                    iscrizione.secondopagamento_data = convertDataNullable(cell[9]);
                    iscrizione.secondopagamento_importo = 0;
                    try { iscrizione.secondopagamento_importo = Utils.moneystringToLong(cell[10]); }
                    catch { }

                    //DateTime? rinuncia = convertDataNullable(cell[11]);
                    
                    iscrizione.primopagamento_modalita = cell[12].Equals("S") ? "bancomat" : "contanti";

                    if (iscrizione.secondopagamento_data != null)
                        iscrizione.secondopagamento_modalita = iscrizione.primopagamento_modalita;

                    iscrizione.data_inizio = convertDataNullable(cell[13]);
                    try { iscrizione.data_iscrizione = (iscrizione.primopagamento_data ?? iscrizione.data_inizio).Value; }
                    catch
                    {
                        iscrizione.data_inizio = DateTime.Now.Date;
                        Log.Instance.WriteLine(Log.LogLevel.Warning, "convertIscrizioni: l'iscrizione " + id + " non ha una data associata");
                    }

                    //DateTime? datamod = convertDataNullable(cell[14]);

                    double imppagato = 0;
                    try { imppagato = Double.Parse(cell[15]); }
                    catch { }

                    int giornate = 0;
                    try { giornate = Int32.Parse(cell[16]); }
                    catch { }

                    int idorario = 0;
                    try { idorario = Int32.Parse(cell[17]); }
                    catch { }

                    string periodo = cell[18];

                    iscrizione.ingressi = 0;
                    try { iscrizione.ingressi = Int32.Parse(cell[19]); }
                    catch { }

                    bool Prova = cell[20].Equals("S");
                    /*
                    c.data_inizio;
                    c.data_iscrizione;
                    c.data_socio;
                    c.corso;
                    c.ingressi;
                    c.persona;
                    c.Saldato;
                    c.tesseran;
                    c.primopagamento_data;
                    c.primopagamento_importo;
                    c.primopagamento_modalita;
                    */

                    /*
                    c.Codice = convertString(cell[1]);
                    c.Descrizione = convertString(cell[2]);
                    try { c.DataInizio = convertData(cell[3]); }
                    catch { }
                    try { c.DataFine = convertData(cell[4]); }
                    catch { }
                    try { c.MaxIscritti = Int32.Parse(convertString(cell[5])); }
                    catch { }

                    c.Attivo = convertString(cell[6]);
                    c.CodiceStampa = convertString(cell[7]);
                    c.TipoCartellino = convertString(cell[8]);
                    c.Bimbi = cell[9].Equals("S");
                    c.Periodo = convertString(cell[10]);
                    */
                    iscrizioni.Add(iscrizione);
                }
                catch (Exception e) { Log.Instance.WriteLine(Log.LogLevel.Error, "Error reading isrcizioni: " + e.Message); }

            return iscrizioni.ToArray();
        }

        public static bool saveIscrizioni(Iscrizione[] iscrizioni)
        {
            bool ret = true;

            foreach (Iscrizione c in iscrizioni)
                ret &= DB.saveIscrizione(c);

            return ret;
        }

        public static void importIscrizioni(String filename, char[] separator)
        {
            saveIscrizioni(convertIscrizioni(readCsv(filename, separator)));
        }


        public static void deleteAllAndClear(MainForm mainform)
        {
            if (promptAndDelete())
            {
                DB.instance.Clear();
                mainform.invalidateGridViews();
            }
        }

        public static void importAll(String dirname, MainForm mainform)
        {
            deleteAllAndClear(mainform);

            //promptAndDelete();
            //DB.instance.Clear();
            //mainform.invalidateGridViews();

            if (!dirname.EndsWith("\\"))
                dirname += "\\";

            DBImporter.importStagioni(dirname + "stagione.csv", null);
            DBImporter.importChiusure(dirname + "chiusure.csv", null);
            DBImporter.importPersone(dirname + "anagrafico.csv", null);
            DBImporter.importCorsi(dirname + "corso.csv", dirname + "orario.csv", dirname + "assorari.csv", null);  // spostare una riga in giu?
            DBImporter.importIscrizioni(dirname + "storico.csv", null);
            //DBImporter.importIstruttori(dirname + "istruttori.csv", null);

            Log.Instance.WriteLine(Log.LogLevel.Info, "Import Completato");
            MessageBox.Show("Import Completato!", "Fatto", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        #endregion

        #region BACKUP
        public static void backupAll(String dirname)
        {
            Log.Instance.WriteLine(Log.LogLevel.Info, "Back up all to: "+dirname);
            DB.instance.Refresh();

            if (!dirname.EndsWith("\\"))
                dirname += "\\";

            backupListini(DB.instance.listini, dirname + "listini.csv");
            backupPersone(DB.instance.persone, dirname + "persone.csv");
            backupCorsi(DB.instance.corsi, dirname + "corsi.csv");
            backupIscrizioni(DB.instance.iscrizioni, dirname + "iscrizioni.csv");
            backupIstruttori(DB.instance.istruttori, dirname + "istruttori.csv");
            backupChiusure(DB.instance.chiusure, dirname + "chiusure.csv");
            backupStagioni(DB.instance.stagioni, dirname + "stagioni.csv");
            backupCorsiIstruttori(DB.instance.istruttori, dirname + "corsiistruttori.csv");
        }

        public static bool backupPersone(List<Persona> persone, String filename)
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename))
                {
                    file.WriteLine("ID" + DefaultSeparatorString + "NOME" + DefaultSeparatorString + "COGNOME" + DefaultSeparatorString + "DATANASCITA" + DefaultSeparatorString +
                        "LUOGONASCITA" + DefaultSeparatorString + "INDIRIZZO" + DefaultSeparatorString + "COMUNE" + DefaultSeparatorString +
                        "CAP" + DefaultSeparatorString + "PROVINCIA" + DefaultSeparatorString + "CODICEFISCALE" + DefaultSeparatorString +
                        "NUMEROTELEFONO" + DefaultSeparatorString + "NUMEROCELLULARE" + DefaultSeparatorString + "EMAIL" + DefaultSeparatorString +
                        "TESSERA" + DefaultSeparatorString + "DATACERTIFICATO" + DefaultSeparatorString + "DATAISCRIZIONE" + DefaultSeparatorString +
                        "TASSAISCRIZIONE" + DefaultSeparatorString + "NOTE" + DefaultSeparatorString + "MALE"+ DefaultSeparatorString + 
                        "GENITORENOME" + DefaultSeparatorString + "GENITORECOGNOME" + DefaultSeparatorString + "GENITOREDATANASCITA" + DefaultSeparatorString + "GENITORELUOGONASCITA");

                    foreach (Persona i in persone)
                        file.WriteLine(i.ID + DefaultSeparatorString +
                            i.Nome + DefaultSeparatorString +
                            i.Cognome + DefaultSeparatorString +
                            Utils.dateToPrintableString(i.DataNascita) + DefaultSeparatorString +
                            i.LuogoNascita + DefaultSeparatorString +
                            i.Indirizzo + DefaultSeparatorString +
                            i.Comune + DefaultSeparatorString +
                            i.CAP + DefaultSeparatorString +
                            i.Provincia + DefaultSeparatorString +
                            i.CodiceFiscale + DefaultSeparatorString +
                            i.NumeroTelefono + DefaultSeparatorString +
                            i.NumeroCellulare + DefaultSeparatorString +
                            i.Email + DefaultSeparatorString +

                            i.Tessera + DefaultSeparatorString +
                            Utils.dateToPrintableString(i.DataCertificato) + DefaultSeparatorString +
                            Utils.dateToPrintableString(i.DataIscrizione) + DefaultSeparatorString +
                            i.TassaIscrizione + DefaultSeparatorString +

                            i.Note + DefaultSeparatorString +
                            i.Male + DefaultSeparatorString +
                            i.GenitoreNome + DefaultSeparatorString +
                            i.GenitoreCognome + DefaultSeparatorString +
                            Utils.dateToPrintableString(i.GenitoreDataNascita) + DefaultSeparatorString +
                            i.GenitoreLuogoNascita);
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Instance.WriteLine(Log.LogLevel.Exception, "backupPersone::" + e.Message);
                return false;
            }
        }

        public static bool backupListini(List<ListinoCorsi> listini, String filename)
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename))
                {

                    file.WriteLine("ID" + DefaultSeparatorString + "Descrizione" + DefaultSeparatorString + "Ingressi");

                    foreach (ListinoCorsi l in listini)
                        file.WriteLine(l.ID + DefaultSeparatorString +
                            l.descrizione + DefaultSeparatorString +
                            l.ingressi + DefaultSeparatorString);
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Instance.WriteLine(Log.LogLevel.Exception, "backupListini::" + e.Message);
                return false;
            }
        }


        public static bool backupCorsi(List<Corso> corsi, String filename)
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename))
                {
                    file.WriteLine("ID" + DefaultSeparatorString + "Stagione" + DefaultSeparatorString + "Listino" + DefaultSeparatorString + "Codice" + DefaultSeparatorString + "Descrizione" + DefaultSeparatorString + "Tipologia" + DefaultSeparatorString +
                        "DataInizio" + DefaultSeparatorString + "DataFine" + DefaultSeparatorString +
                        "MaxIscritti" + DefaultSeparatorString + "Attivo" + DefaultSeparatorString + "CodiceStampa" + DefaultSeparatorString +
                        "TipoCartellino" + DefaultSeparatorString + "Orario" + DefaultSeparatorString + "Periodico" + DefaultSeparatorString +
                        "Bimbi" + DefaultSeparatorString);

                    foreach (Corso c in corsi)
                        file.WriteLine(c.ID + DefaultSeparatorString +
                            (c.stagione != null ? c.stagione.ID.ToString() : string.Empty) + DefaultSeparatorString +
                            (c.listino != null ? c.listino.ID.ToString() : string.Empty) + DefaultSeparatorString +
                            c.Codice + DefaultSeparatorString +
                            c.Descrizione + DefaultSeparatorString +
                            c.Tipologia + DefaultSeparatorString +
                            Utils.dateToPrintableString(c.DataInizio) + DefaultSeparatorString +
                            Utils.dateToPrintableString(c.DataFine) + DefaultSeparatorString +
                            c.MaxIscritti + DefaultSeparatorString +
                            c.Attivo + DefaultSeparatorString +
                            c.CodiceStampa + DefaultSeparatorString +
                            c.TipoCartellino + DefaultSeparatorString +
                            c.Orario + DefaultSeparatorString +
                            c.Bimbi + DefaultSeparatorString);
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Instance.WriteLine(Log.LogLevel.Exception, "backupCorsi::" + e.Message);
                return false;
            }
        }

        public static bool backupIscrizioni(List<Iscrizione> iscrizione, String filename)
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename))
                {
                    file.WriteLine("ID" + DefaultSeparatorString + "IDpersona" + DefaultSeparatorString + "IDcorso" + DefaultSeparatorString + "DataInizio" + DefaultSeparatorString +
                        "DataIscrizione" + DefaultSeparatorString + "DataSocio" + DefaultSeparatorString + "Ingressi" + DefaultSeparatorString +
                        "Pag1Data" + DefaultSeparatorString + "Pag1Importo" + DefaultSeparatorString + "Pag1Modalita" + DefaultSeparatorString +
                        "Pag2Data" + DefaultSeparatorString + "Pag2Importo" + DefaultSeparatorString + "Pag2Modalita" + DefaultSeparatorString +
                        "Pag3Data" + DefaultSeparatorString + "Pag3Importo" + DefaultSeparatorString + "Pag3Modalita" + DefaultSeparatorString +
                        "Saldato" + DefaultSeparatorString + "TesseraN");

                    foreach (Iscrizione c in iscrizione)
                        file.WriteLine(c.ID + DefaultSeparatorString +
                            c.persona.ID + DefaultSeparatorString +
                            c.corso.ID + DefaultSeparatorString +
                            Utils.dateToPrintableString(c.data_inizio) + DefaultSeparatorString +
                            Utils.dateToPrintableString(c.data_iscrizione) + DefaultSeparatorString +
                            Utils.dateToPrintableString(c.data_socio) + DefaultSeparatorString +
                            c.ingressi + DefaultSeparatorString +
                            Utils.dateToPrintableString(c.primopagamento_data) + DefaultSeparatorString +
                            c.primopagamento_importo + DefaultSeparatorString +
                            c.primopagamento_modalita + DefaultSeparatorString +
                            Utils.dateToPrintableString(c.secondopagamento_data) + DefaultSeparatorString +
                            c.secondopagamento_importo + DefaultSeparatorString +
                            c.secondopagamento_modalita + DefaultSeparatorString +
                            Utils.dateToPrintableString(c.terzopagamento_data) + DefaultSeparatorString +
                            c.terzopagamento_importo + DefaultSeparatorString +
                            c.terzopagamento_modalita + DefaultSeparatorString +
                            c.Saldato + DefaultSeparatorString +
                            c.tesseran);
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Instance.WriteLine(Log.LogLevel.Exception, "backupIscrizioni::" + e.Message);
                return false;
            }
        }


        public static bool backupIstruttori(List<Istruttore> istruttori, String filename)
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename))
                {
                    file.WriteLine("ID" + DefaultSeparatorString + "NOME" + DefaultSeparatorString + "COGNOME" + DefaultSeparatorString + "DATANASCITA" + DefaultSeparatorString +
                        "LUOGONASCITA" + DefaultSeparatorString + "INDIRIZZO" + DefaultSeparatorString + "COMUNE" + DefaultSeparatorString +
                        "CAP" + DefaultSeparatorString + "PROVINCIA" + DefaultSeparatorString + "CODICEFISCALE" + DefaultSeparatorString +
                        "NUMEROTELEFONO" + DefaultSeparatorString + "NUMEROCELLULARE" + DefaultSeparatorString + "EMAIL" + DefaultSeparatorString +
                        "NOTE" + DefaultSeparatorString + "MALE");

                    foreach (Istruttore i in istruttori)
                        file.WriteLine(i.ID + DefaultSeparatorString +
                            i.Nome + DefaultSeparatorString +
                            i.Cognome + DefaultSeparatorString +
                            Utils.dateToPrintableString(i.DataNascita) + DefaultSeparatorString +
                            i.LuogoNascita + DefaultSeparatorString +
                            i.Indirizzo + DefaultSeparatorString +
                            i.Comune + DefaultSeparatorString +
                            i.CAP + DefaultSeparatorString +
                            i.Provincia + DefaultSeparatorString +
                            i.CodiceFiscale + DefaultSeparatorString +
                            i.NumeroTelefono + DefaultSeparatorString +
                            i.NumeroCellulare + DefaultSeparatorString +
                            i.Email + DefaultSeparatorString +
                            i.Note + DefaultSeparatorString +
                            i.Male + DefaultSeparatorString);

                }

                return true;
            }
            catch (Exception e)
            {
                Log.Instance.WriteLine(Log.LogLevel.Exception, "backupIstruttori::" + e.Message);
                return false;
            }
        }


        public static bool backupChiusure(List<Chiusura> chiusure, String filename)
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename))
                {
                    file.WriteLine("ID" + DefaultSeparatorString + "STAGIONE" + DefaultSeparatorString + "DESCRIZIONE" + DefaultSeparatorString + "DATAINIZIO" + DefaultSeparatorString + "DATAFINE");

                    foreach (Chiusura c in chiusure)
                        file.WriteLine(c.ID + DefaultSeparatorString +
                            c.stagione.ID + DefaultSeparatorString +
                            c.Descrizione + DefaultSeparatorString +
                            Utils.dateToPrintableString(c.DataInizio) + DefaultSeparatorString +
                            Utils.dateToPrintableString(c.DataFine) + DefaultSeparatorString);
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Instance.WriteLine(Log.LogLevel.Exception, "backupChiusure::" + e.Message);
                return false;
            }

        }


        public static bool backupStagioni(List<Stagione> stagioni, String filename)
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename))
                {
                    file.WriteLine("ID" + DefaultSeparatorString + "DESCRIZIONE" + DefaultSeparatorString + "DATAINIZIO" + DefaultSeparatorString + "DATAFINE"
                         + DefaultSeparatorString + "DAFAFINEQUADRIMESTRE" + DefaultSeparatorString + "CORRENTE");

                    foreach (Stagione s in stagioni)
                        file.WriteLine(s.ID + DefaultSeparatorString +
                            s.Descrizione + DefaultSeparatorString +
                            Utils.dateToPrintableString(s.DataInizio) + DefaultSeparatorString +
                            Utils.dateToPrintableString(s.DataFine) + DefaultSeparatorString +
                            Utils.dateToPrintableString(s.FineQuadrimestre) + DefaultSeparatorString +
                            s.Corrente + DefaultSeparatorString);
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Instance.WriteLine(Log.LogLevel.Exception, "backupStagioni::" + e.Message);
                return false;
            }
        
        }

        public static bool backupCorsiIstruttori(List<Istruttore> istruttori, String filename)
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename))
                {
                    file.WriteLine("IDistruttore" + DefaultSeparatorString + "IDcorso");

                    foreach (Istruttore s in istruttori)
                        foreach (Corso c in s.Corsi)
                            file.WriteLine(s.ID + DefaultSeparatorString + c.ID);
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Instance.WriteLine(Log.LogLevel.Exception, "backupCorsiIstruttori::" + e.Message);
                return false;
            }

        }
        #endregion

        #region DeleteExistingData
        public static void deleteAll()
        {
            deleteData(DB.instance.iscrizioni);
            deleteData(DB.instance.istruttori);
            deleteData(DB.instance.corsi);
            deleteData(DB.instance.listini);
            deleteData(DB.instance.chiusure);
            deleteData(DB.instance.stagioni);
            deleteData(DB.instance.persone);
        }

        public static void deleteData(List<Chiusura> list)
        {
            Chiusura[] list1 = list.ToArray();
            foreach (Chiusura c in list1)
                DB.instance.removeChiusura(c);
        }

        public static void deleteData(List<Stagione> list)
        {
            Stagione[] list1 = list.ToArray();
            foreach (Stagione c in list1)
                DB.instance.removeStagione(c);
        }

        public static void deleteData(List<Corso> list)
        {
            Corso[] list1 = list.ToArray();
            foreach (Corso c in list1)
                DB.instance.removeCorso(c);
        }

        public static void deleteData(List<ListinoCorsi> list)
        {
            ListinoCorsi[] list1 = list.ToArray();
            foreach (ListinoCorsi c in list1)
                DB.instance.removeListino(c);
        }

        public static void deleteData(List<Persona> list)
        {
            Persona[] list1 = list.ToArray();
            foreach (Persona c in list1)
                DB.instance.removePersona(c);
        }

        public static void deleteData(List<Istruttore> list)
        {
            Istruttore[] list1 = list.ToArray();
            foreach (Istruttore c in list1)
                DB.instance.removeIstruttore(c);
        }

        public static void deleteData(List<Iscrizione> list)
        {
            Iscrizione[] list1 = list.ToArray();
            foreach (Iscrizione iscrizione in list1)
            {
                Persona persona = iscrizione.persona;
                Corso corso = iscrizione.corso;

                persona.Iscrizioni.Remove(iscrizione);
                corso.Iscrizioni.Remove(iscrizione);

                DB.deleteIscrizione(iscrizione);
            }
        }


        public static bool promptAndDelete()
        {
            // chiedi due volte se sei sicuro
            DialogResult dialogResult = MessageBox.Show("SEI ASSOLUTAMENTE SICURO?\nVuoi cancellare completamente i dati?", "Conferma Eliminazione Totale", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                DialogResult dialogResult1 = MessageBox.Show("TE LO CHIEDO DI NUOVO:\nVuoi cancellare TUTTI i dati del database?", "Conferma Eliminazione Totale", MessageBoxButtons.YesNo);
                if (dialogResult1 == DialogResult.Yes)
                {
                    // chiedi se si vuole effettuare un backup
                    DialogResult dialogResult3 = MessageBox.Show("Vuoi eseguire un backup prima?", "Backup?", MessageBoxButtons.YesNo);
                    if (dialogResult3 == DialogResult.Yes)
                    {
                        FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
                        if (dialog.ShowDialog() == DialogResult.OK)
                            backupAll(dialog.SelectedPath);
                    }

                    // elimina tutto
                    Log.Instance.WriteLine(Log.LogLevel.Info, "Cancellazione intero database!");
                    deleteAll();
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Restore
        public static void restoreAll(String dirname, MainForm mainform)
        {
            Log.Instance.WriteLine(Log.LogLevel.Info, "Restore all from: "+dirname);

            if (promptAndDelete())
            {
                DB.instance.Clear();
                mainform.invalidateGridViews();
            }

            if (!dirname.EndsWith("\\"))
                dirname += "\\";

            restoreListini(readCsv(dirname + "listini.csv", DefaultSeparator));
            restoreChiusure(readCsv(dirname + "chiusure.csv", DefaultSeparator));
            restoreStagioni(readCsv(dirname + "stagioni.csv", DefaultSeparator));
            restoreIstruttori(readCsv(dirname + "istruttori.csv", DefaultSeparator));
            restorePersone(readCsv(dirname + "persone.csv", DefaultSeparator));
            restoreCorsi(readCsv(dirname + "corsi.csv", DefaultSeparator));
            DB.instance.ImportFromDb();

            restoreIscrizioni(readCsv(dirname + "iscrizioni.csv", DefaultSeparator));
            restoreCorsiIstruttori(readCsv(dirname + "corsiistruttori.csv", DefaultSeparator));

            DB.instance.Refresh();

            Log.Instance.WriteLine(Log.LogLevel.Info, "Restore Completato");
            MessageBox.Show("Restore Completato!", "Fatto", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        public static void restorePersone(string filename) { restorePersone(readCsv(filename, DefaultSeparator)); DB.instance.Refresh(); }
        public static void restoreIstruttori(string filename) { restoreIstruttori(readCsv(filename, DefaultSeparator)); DB.instance.Refresh(); }
        public static void restoreStagioni(string filename) { restoreStagioni(readCsv(filename, DefaultSeparator)); DB.instance.Refresh(); }
        public static void restoreChiusure(string filename) { restoreChiusure(readCsv(filename, DefaultSeparator)); DB.instance.Refresh(); }
        public static void restoreCorsi(string filename) { restoreCorsi(readCsv(filename, DefaultSeparator)); DB.instance.Refresh(); }
        public static void restoreIscrizioni(string filename) { restoreIscrizioni(readCsv(filename, DefaultSeparator)); DB.instance.Refresh(); }
        public static void restoreCorsiIstruttori(string filename) { restoreCorsiIstruttori(readCsv(filename, DefaultSeparator)); DB.instance.Refresh(); }
        public static void restoreListini(string filename) { restoreListini(readCsv(filename, DefaultSeparator)); DB.instance.Refresh(); }



        private static void restorePersone(List<String[]> lines)
        {

            using (NHibernate.ISession session = HibernateHelper.Open())
            using (NHibernate.ITransaction transaction = session.BeginTransaction())
                try
                {
                    for (int i = 1; i < lines.Count; i++)
                        try
                        {
                            // ID	NOME	COGNOME	DATANASCITA	LUOGONASCITA	INDIRIZZO	COMUNE	CAP	PROVINCIA	CODICEFISCALE	NUMEROTELEFONO	NUMEROCELLULARE	EMAIL	
                            //TESSERA	DATACERTIFICATO	DATAISCRIZIONE	TASSAISCRIZIONE	NOTE	MALE
                            Application.DoEvents();
                            String[] cell = lines.ElementAt(i);

                            Persona s = new Persona()
                            {
                                Nome = convertString(cell[1]),
                                Cognome = convertString(cell[2]),
                                DataNascita = convertDataNullable(cell[3]),
                                LuogoNascita = convertString(cell[4]),
                                Indirizzo = convertString(cell[5]),
                                Comune = convertString(cell[6]),
                                CAP = convertString(cell[7]),
                                Provincia = convertString(cell[8]),
                                CodiceFiscale = convertString(cell[9]),
                                NumeroTelefono = convertString(cell[10]),
                                NumeroCellulare = convertString(cell[11]),
                                Email = convertString(cell[12]),
                                //Tessera = convertString(cell[13]),
                                DataCertificato = convertDataNullable(cell[14]),
                                DataIscrizione = convertDataNullable(cell[15]),
                                TassaIscrizione = convertString(cell[16]).Equals(TrueStringValue),

                                Note = convertString(cell[17]),
                                Male = convertString(cell[18]).Equals(TrueStringValue),

                                GenitoreNome = convertString(cell[19]),
                                GenitoreCognome = convertString(cell[20]),
                                GenitoreDataNascita = convertDataNullable(cell[21]),
                                GenitoreLuogoNascita =  convertString(cell[22])
                            };
                            try { s.ID = Guid.Parse(cell[0]); }
                            catch { }

                            try { s.Tessera = Int32.Parse(cell[13]); }
                            catch { s.Tessera = null; }

                            session.Save(s, s.ID);
                        }
                        catch (Exception e)
                        {
                            Log.Instance.WriteLine(Log.LogLevel.Warning, "restorePersone @" + i + " error:" + e.Message);
                        }

                    transaction.Commit();
                }
                catch (Exception exc)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Error, "restorePersone::" + exc.Message);
                }
        }

        private static void restoreIstruttori(List<String[]> lines)
        {
            for (int i = 1; i < lines.Count; i++)
                try
                {
                    // ID	NOME	COGNOME	DATANASCITA	LUOGONASCITA	INDIRIZZO	COMUNE	CAP	PROVINCIA	CODICEFISCALE	NUMEROTELEFONO	NUMEROCELLULARE	EMAIL	NOTE	MALE
                    String[] cell = lines.ElementAt(i);

                    Istruttore s = new Istruttore()
                    {
                        Nome = convertString(cell[1]),
                        Cognome = convertString(cell[2]),
                        DataNascita = convertDataNullable(cell[3]),
                        LuogoNascita = convertString(cell[4]),
                        Indirizzo = convertString(cell[5]),
                        Comune = convertString(cell[6]),
                        CAP = convertString(cell[7]),
                        Provincia = convertString(cell[8]),
                        CodiceFiscale = convertString(cell[9]),
                        NumeroTelefono = convertString(cell[10]),
                        NumeroCellulare = convertString(cell[11]),
                        Email = convertString(cell[12]),
                        Note = convertString(cell[13]),
                        Male = convertString(cell[14]).Equals(TrueStringValue)
                    };
                    try { s.ID = Guid.Parse(cell[0]); }
                    catch { }

                    DB.insertIstruttore(s, s.ID);
                }
                catch (Exception e)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Warning, "restoreIstruttori @" + i + " error:" + e.Message);
                }
        }

        private static void restoreCorsiIstruttori(List<String[]> lines)
        {
            for (int i = 1; i < lines.Count; i++)
                try
                {
                    // IDistruttore IDcorso
                    String[] cell = lines.ElementAt(i);
                    Istruttore istruttore = null;
                    Corso corso = null;

                    try { istruttore = DB.instance.getIstruttore(Guid.Parse(cell[0])); }
                    catch { }

                    try { corso = DB.instance.getCorso(Guid.Parse(cell[1])); }
                    catch { }

                    if (istruttore == null || corso == null)
                    {
                        Log.Instance.WriteLine(Log.LogLevel.Warning, "restoreCorsiIstruttori NULL @" + i);
                        continue;
                    }

                    if (!istruttore.Corsi.Contains(corso))
                        istruttore.Corsi.Add(corso);

                    if (!corso.Istruttori.Contains(istruttore))
                        corso.Istruttori.Add(istruttore);

                    DB.instance.save(corso);
                    DB.instance.save(istruttore);
                }
                catch (Exception e)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Warning, "restoreCorsiIstruttori @" + i + " error:" + e.Message);
                }
        }


        private static void restoreListini(List<String[]> lines)
        {
            for (int i = 1; i < lines.Count; i++)
                try
                {
                    String[] cell = lines.ElementAt(i);
                    ListinoCorsi lc = new ListinoCorsi { descrizione = cell[1], ingressi = cell[2] };

                    try { lc.ID = Guid.Parse(cell[0]); }
                    catch { }

                    DB.insertListino(lc, lc.ID);
                }
                catch (Exception e)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Warning, "restoreListini @" + i + " error:" + e.Message);
                }
        }



        private static void restoreStagioni(List<String[]> lines)
        {
            for (int i = 1; i < lines.Count; i++)
                try
                {
                    // ID	DESCRIZIONE	DATAINIZIO	DATAFINE	DAFAFINEQUADRIMESTRE	CORRENTE
                    String[] cell = lines.ElementAt(i);

                    Stagione s = new Stagione() { Descrizione = convertString(cell[1]), DataInizio = convertData(cell[2]), DataFine = convertData(cell[3]),
                                                  FineQuadrimestre = convertData(cell[4]),
                                                  Corrente = convertString(cell[5]).Equals(TrueStringValue) };
                    try { s.ID = Guid.Parse(cell[0]); }
                    catch { }

                    DB.insertStagione(s, s.ID);
                }
                catch (Exception e)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Warning, "restoreStagioni @" + i + " error:" + e.Message);
                }
        }

        private static void restoreChiusure(List<String[]> lines)
        {
            for (int i = 1; i < lines.Count; i++)
                try
                {
                    //ID	STAGIONE	DESCRIZIONE	DATAINIZIO	DATAFINE
                    String[] cell = lines.ElementAt(i);

                    Chiusura c = new Chiusura() { Descrizione = convertString(cell[2]), DataInizio = convertData(cell[3]), DataFine = convertData(cell[4]) };

                    try { c.stagione = DB.instance.getStagioneById(Guid.Parse(cell[1])); }
                    catch { }

                    try { c.ID = Guid.Parse(cell[0]); }
                    catch { }

                    DB.insertChiusura(c, c.ID);
                }
                catch (Exception e)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Warning, "restoreChiusure @" + i + " error:" + e.Message);
                }
        }


        private static void restoreCorsi(List<String[]> lines)
        {
            for (int i = 1; i < lines.Count; i++)
                try
                {
                    //ID	Stagione	Listino	Codice	Descrizione	Tipologia		DataInizio	DataFine	MaxIscritti	Attivo	CodiceStampa	TipoCartellino	Orario	Bimbi
                    String[] cell = lines.ElementAt(i);

                    Corso c = new Corso() {
                        Codice =  convertString(cell[3]),
                        Descrizione = convertString(cell[4]),
                        Tipologia =  convertString(cell[5]),

                        DataInizio = convertData(cell[6]),
                        DataFine = convertData(cell[7]),

                        Attivo = convertString(cell[9]),
                        CodiceStampa = convertString(cell[10]),
                        TipoCartellino = convertString(cell[11]),
                        Orario = convertString(cell[12]),
                        Bimbi = convertString(cell[13]).Equals(TrueStringValue),
                    };
                    try { c.ID = Guid.Parse(cell[0]); }
                    catch { }

                    try { c.stagione = DB.instance.getStagioneById(Guid.Parse(cell[1])); }
                    catch { }

                    try { c.listino = DB.instance.getListinoById(Guid.Parse(cell[2])); }
                    catch { }

                    try { c.MaxIscritti = Int32.Parse(cell[8]); }
                    catch { }

                    DB.insertCorso(c, c.ID);
                }
                catch (Exception e)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Warning, "restoreChiusure @" + i + " error:" + e.Message);
                }
        }


        private static void restoreIscrizioni(List<String[]> lines)
        {
            using (NHibernate.ISession session = HibernateHelper.Open())
            using (NHibernate.ITransaction transaction = session.BeginTransaction())
                try
                {
                    for (int i = 1; i < lines.Count; i++)
                        try
                        {
                            // ID	IDpersona	IDcorso	DataInizio	DataIscrizione	DataSocio	Ingressi	Pag1Data	Pag1Importo	Pag1Modalita
                            // Pag2Data	Pag2Importo	Pag2Modalita	Pag3Data	Pag3Importo	Pag3Modalita	Saldato	TesseraN
                            Application.DoEvents();
                            String[] cell = lines.ElementAt(i);

                            Persona p = DB.instance.getPersona(Guid.Parse(cell[1]));
                            Corso c = DB.instance.getCorso(Guid.Parse(cell[2]));

                            if (p == null || c == null)
                            {
                                Log.Instance.WriteLine(Log.LogLevel.Warning, "restoreIscrizioni NULLs:" + (p == null ? "Persona=" + cell[1] : string.Empty) + " " + (c == null ? "Corso=" + cell[2] : string.Empty));
                                continue;
                            }


                            Iscrizione s = new Iscrizione()
                            {
                                corso = c,
                                persona = p,
                                data_inizio = convertDataNullable(cell[3]),
                                data_iscrizione = convertData(cell[4]),
                                data_socio = convertDataNullable(cell[5]),
                                //ingressi = convertString(cell[6]),
                                primopagamento_data = convertDataNullable(cell[7]),
                                //primopagamento_importo = convertString(cell[8]),
                                primopagamento_modalita= convertString(cell[9]),
                                secondopagamento_data = convertDataNullable(cell[10]),
                                //secondopagamento_importo = convertString(cell[11]),
                                secondopagamento_modalita = convertString(cell[12]),
                                terzopagamento_data = convertDataNullable(cell[13]),
                                //terzopagamento_importo = convertString(cell[14]),
                                terzopagamento_modalita = convertString(cell[15]),
                                Saldato = convertString(cell[16]).Equals(TrueStringValue),
                                //tesseran = convertString(cell[17])
                            };
                            try { s.ID = Guid.Parse(cell[0]); }
                            catch { }

                            try { s.ingressi = Int32.Parse(cell[6]); }
                            catch { }
                            try { s.primopagamento_importo = long.Parse(cell[8]); }
                            catch { }
                            try { s.secondopagamento_importo = long.Parse(cell[11]); }
                            catch { }
                            try { s.terzopagamento_importo = long.Parse(cell[14]); }
                            catch { }
                            try { s.tesseran = Int32.Parse(cell[17]); }
                            catch { }


                            p.Iscrizioni.Add(s);
                            c.Iscrizioni.Add(s);

                            //session.SaveOrUpdate(p);
                            //session.SaveOrUpdate(c);
                            session.Save(s, s.ID);
                            //session.Save(s);
                        }
                        catch (Exception e)
                        {
                            Log.Instance.WriteLine(Log.LogLevel.Warning, "restoreIscrizioni @" + i + " error:" + e.Message);
                        }

                    transaction.Commit();
                }
                catch (Exception exc)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Error, "restoreIscrizioni::" + exc.Message);
                }
        }
        #endregion
    }
}

