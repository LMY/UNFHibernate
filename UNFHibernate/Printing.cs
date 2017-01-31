using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using UNFHibernate.Domain;

namespace UNFHibernate
{
    class Printing
    {
        public static readonly String PathCartellino = "\\Templates\\Cartellino.dot";
        public static readonly String PathPagamento = "\\Templates\\Pagamento.dot";
        public static readonly String PathLibroSoci = "\\Templates\\LibroSoci.dot";
        public static readonly String PathIncassiGiorno = "\\Templates\\IncassiGiorno.dot";

        public static void printCartellino(Iscrizione iscrizione)
        {
            String colore = iscrizione.corso.TipoCartellino;

            if (String.IsNullOrEmpty(colore))
                MessageBox.Show("Questo corso non ha un cartellino associato!\nVerrà stampato il cartellino di default");
            
            string oTemplatePath;

            if (String.IsNullOrEmpty(colore))
                oTemplatePath = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + PathCartellino;
            else
            {
                string fullPath = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + PathCartellino;

                int punto = fullPath.LastIndexOf('.');
                if (punto < 0)
                    oTemplatePath = fullPath;
                else
                {
                    oTemplatePath = fullPath.Substring(0, punto);
                    oTemplatePath += "-" + colore;
                    oTemplatePath += fullPath.Substring(punto);
                }
            }

            if (!File.Exists(oTemplatePath) && !Directory.Exists(oTemplatePath))
            {
                MessageBox.Show("Impossibile trovare il modello di word associato al cartellino \"" + (colore??"null") + "\"");
                return;
            }

            printIscrizione((Object) oTemplatePath, iscrizione, -1, Config.Instance.QuitWordAfterPrintCartellino);
        }

        public static void printPagamento(Iscrizione iscrizione, int npagamento)
        {
            Object oTemplatePath = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + PathPagamento;
            printIscrizione(oTemplatePath, iscrizione, npagamento, Config.Instance.QuitWordAfterPrintPagamento);
        }

        public static void printLibroSoci(List<Iscrizione> iscrizioni, DateTime data_iscrizione, bool shallclose)
        {
            Object oTemplatePath = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + PathLibroSoci;
            printLibroSoci(oTemplatePath, iscrizioni, data_iscrizione, shallclose);
        }

        public static void printIncassiGiorno(List<UNFHibernate.Windows.ViewIncassi.EntryIscrizione> pagamenti, DateTime DataIscrizione)
        {
            Object oTemplatePath = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + PathIncassiGiorno;
            printIncassiGiorno(oTemplatePath, pagamenti, DataIscrizione, Config.Instance.QuitWordAfterPrintIngressiGiorno);
        }


        public static void printIscrizione(Object oTemplatePath, Iscrizione iscrizione, int npagamento, bool shallclose)
        {
            Microsoft.Office.Interop.Word.Application wordApp = null;

            try
            {
                Object oMissing = System.Reflection.Missing.Value;

                wordApp = new Microsoft.Office.Interop.Word.Application();
                Document wordDoc = new Document();
                wordDoc = wordApp.Documents.Add(ref oTemplatePath, ref oMissing, ref oMissing, ref oMissing);

                foreach (Field myMergeField in wordDoc.Fields)
                {
                    Range rngFieldCode = myMergeField.Code;
                    String fieldText = rngFieldCode.Text;
                    if (fieldText.StartsWith(" MERGEFIELD"))
                    {
                        Int32 endMerge = fieldText.IndexOf("\\");
                        Int32 fieldNameLength = fieldText.Length - endMerge;
                        String fieldName = fieldText.Substring(11, endMerge - 11);

                        fieldName = fieldName.Trim();

                        if (fieldName == "Nome")
                        {
                            myMergeField.Select();
                            wordApp.Selection.TypeText(stringToWord(iscrizione.persona.Nome));
                        }
                        else if (fieldName == "Cognome")
                        {
                            myMergeField.Select();
                            wordApp.Selection.TypeText(stringToWord(iscrizione.persona.Cognome));
                        }
                        else if (fieldName == "Nascita")
                        {
                            myMergeField.Select();

                            if (iscrizione.persona.DataNascita != null)
                            { 
                                DateTime dt = (DateTime)iscrizione.persona.DataNascita;
                                wordApp.Selection.TypeText(dt.ToString("d"));
                            }
                            else
                            {
                                wordApp.Selection.TypeText("  /  /");
                            }
                        }
                        else if (fieldName == "CF")
                        {
                            myMergeField.Select();
                            wordApp.Selection.TypeText(stringToWord(iscrizione.persona.CodiceFiscale));
                        }
                        else if (fieldName == "ComuneNascita")
                        {
                            myMergeField.Select();
                            wordApp.Selection.TypeText(stringToWord(iscrizione.persona.LuogoNascita));
                        }
                        else if (fieldName == "Corso")
                        {
                            myMergeField.Select();
                            wordApp.Selection.TypeText(stringToWord(iscrizione.corso.Descrizione));
                        }
                        else if (fieldName == "Oggi")
                        {
                            myMergeField.Select();
                            wordApp.Selection.TypeText(DateTime.Today.ToString("d"));
                        }
                        else if (fieldName == "Giornate")
                        {
                            myMergeField.Select();
                            wordApp.Selection.TypeText(stringToWord(Utils.giornateToString(iscrizione.corso.getGiorniSettimana())));
                        }
                        else if (fieldName == "Orario")
                        {
                            myMergeField.Select();
                            wordApp.Selection.TypeText(stringToWord(iscrizione.corso.getOrarioString()));
                        }
                        else if (fieldName == "Dal")
                        {
                            myMergeField.Select();
                            if (iscrizione.data_inizio != null)
                                wordApp.Selection.TypeText(iscrizione.data_inizio.Value.Date.ToString("d"));
                        }
                        else if (fieldName == "Al")
                        {
                            myMergeField.Select();
                            //    wordApp.Selection.TypeText(iscrizione.corso.DataFine.ToString("d"));
                            wordApp.Selection.TypeText(Utils.dateToPrintableString(iscrizione.getLastDate()));
                        }
                        else if (fieldName == "Pagato")
                        {
                            myMergeField.Select();
                            wordApp.Selection.TypeText(iscrizione.Saldato ? "SI" : "NO");
                        }
                        else if (fieldName == "Indirizzo")
                        {
                            myMergeField.Select();
                            wordApp.Selection.TypeText(stringToWord(iscrizione.persona.Indirizzo));
                        }
                        else if (fieldName == "NumeroTelefono")
                        {
                            myMergeField.Select();
                            wordApp.Selection.TypeText(stringToWord(iscrizione.persona.NumeroTelefono));
                        }
                        else if (fieldName == "NumeroCellulare")
                        {
                            myMergeField.Select();
                            wordApp.Selection.TypeText(stringToWord(iscrizione.persona.NumeroCellulare));
                        }
                        else if (fieldName == "Email")
                        {
                            myMergeField.Select();
                            wordApp.Selection.TypeText(stringToWord(iscrizione.persona.Email));
                        }
                        else if (fieldName == "Stagione")
                        {
                            myMergeField.Select();
                            wordApp.Selection.TypeText(stringToWord(DB.instance.stagione_corrente.Descrizione));
                        }

                        else if (fieldName == "Importo")
                        {
                            if (npagamento < 0) continue;

                            myMergeField.Select();

                            String importo = Utils.moneylongToString(npagamento == 0 ? iscrizione.primopagamento_importo :
                                npagamento == 1 ? iscrizione.secondopagamento_importo :
                                iscrizione.terzopagamento_importo);

                            wordApp.Selection.TypeText(stringToWord(importo));
                        }
                        else if (fieldName == "ModalitaPagamento")
                        {
                            if (npagamento < 0) continue;

                            myMergeField.Select();
                            String modalita = npagamento == 0 ? iscrizione.primopagamento_modalita :
                                npagamento == 1 ? iscrizione.secondopagamento_modalita :
                                iscrizione.terzopagamento_modalita;

                            wordApp.Selection.TypeText(stringToWord(modalita));
                        }
                        else if (fieldName == "DataPagamento")
                        {
                            if (npagamento < 0) continue;

                            try
                            {
                                myMergeField.Select();
                                DateTime? datapagamento = npagamento == 0 ? iscrizione.primopagamento_data :
                                    npagamento == 1 ? iscrizione.secondopagamento_data :
                                    iscrizione.terzopagamento_data;

                                if (datapagamento != null)
                                    wordApp.Selection.TypeText(datapagamento.Value.ToString("d"));
                            }
                            catch { }
                        }

                        else if (fieldName.StartsWith("Importo"))
                        {
                            try { 
                                int n = Int32.Parse(fieldName.Replace("Importo", String.Empty));

                                myMergeField.Select();
                                String importo = Utils.moneylongToString(n == 0 ? iscrizione.primopagamento_importo :
                                            n == 1 ? iscrizione.secondopagamento_importo :
                                            iscrizione.terzopagamento_importo);

                                wordApp.Selection.TypeText(stringToWord(importo));
                            }
                            catch { continue; }
                        }
                        else if (fieldName.StartsWith("ModalitaPagamento"))
                        {
                            try
                            {
                                int n = Int32.Parse(fieldName.Replace("ModalitaPagamento", String.Empty));

                                myMergeField.Select();
                                String modalita = n == 0 ? iscrizione.primopagamento_modalita :
                                                n == 1 ? iscrizione.secondopagamento_modalita :
                                                iscrizione.terzopagamento_modalita;

                                wordApp.Selection.TypeText(stringToWord(modalita));
                            }
                            catch { continue; }
                        }
                        else if (fieldName.StartsWith("DataPagamento"))
                        {
                            try
                            {
                                int n = Int32.Parse(fieldName.Replace("DataPagamento", String.Empty));

                                myMergeField.Select();
                                DateTime? datapagamento = n == 0 ? iscrizione.primopagamento_data :
                                                n == 1 ? iscrizione.secondopagamento_data :
                                                iscrizione.terzopagamento_data;

                                if (datapagamento != null)
                                    wordApp.Selection.TypeText(datapagamento.Value.ToString("d"));
                            }
                            catch { continue; }
                        }
                    }
                }

                wordApp.PrintOut();
                if (shallclose)
                {
                    Microsoft.Office.Interop.Word._Document document = wordApp.ActiveDocument;
                    document.Close(Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges);
                    ((Microsoft.Office.Interop.Word._Application)wordApp).Quit();
                }
                else
                    wordApp.Visible = true;
            }
            catch (Exception e)
            {
                String errorString = "Errore Stampando un Pagamento:\n" + e.Message;

                if (Config.Instance.ShowErrors)
                    MessageBox.Show(errorString, "Errore ", MessageBoxButtons.OK, MessageBoxIcon.Error);

                Log.Instance.WriteLine(Log.LogLevel.Exception, errorString);

                try { ((Microsoft.Office.Interop.Word._Application)wordApp).Quit(); }
                catch (Exception e1)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Exception, "Errore chiudendo Office: " + e1.ToString());
                }

            }
        }


        public static void printLibroSoci(Object oTemplatePath, List<Iscrizione> iscrizioni, DateTime data_iscrizione, bool shallclose)
        {
            Microsoft.Office.Interop.Word.Application wordApp = null;

        //    try
         //   {
                Object oMissing = System.Reflection.Missing.Value;

                wordApp = new Microsoft.Office.Interop.Word.Application();
                Document wordDoc = new Document();
                wordDoc = wordApp.Documents.Add(ref oTemplatePath, ref oMissing, ref oMissing, ref oMissing);

                foreach (Field myMergeField in wordDoc.Fields)
                {
                    Range rngFieldCode = myMergeField.Code;
                    String fieldText = rngFieldCode.Text;
                    if (fieldText.StartsWith(" MERGEFIELD"))
                    {
                        Int32 endMerge = fieldText.IndexOf("\\");
                        Int32 fieldNameLength = fieldText.Length - endMerge;
                        String fieldName = fieldText.Substring(11, endMerge - 11);

                        fieldName = fieldName.Trim();

                        if (fieldName == "Stagione" && DB.instance.stagione_corrente != null)
                        {
                            myMergeField.Select();
                            wordApp.Selection.TypeText(stringToWord(DB.instance.stagione_corrente.Descrizione));
                        }
                        else if (fieldName == "Oggi")
                        {
                            myMergeField.Select();
                            wordApp.Selection.TypeText(DateTime.Today.ToString("d"));
                        }
                        else if (fieldName == "DataIscrizione")
                        {
                            myMergeField.Select();
                            wordApp.Selection.TypeText(data_iscrizione.ToString("d"));
                        }
                    }
                }


                int nrighe = iscrizioni.Count + 1;
                int ncolonne = 7;

                object start = wordDoc.Content.End-1;
                object end = wordDoc.Content.End-1;
                Range tableLocation = wordDoc.Range(ref start, ref end);
                Table t = wordDoc.Tables.Add(tableLocation, nrighe, ncolonne);

                int rownum = 1;
                for (int c = 1; c < ncolonne+1; c++)
                {
                    Cell cell = t.Cell(rownum, c);
                    cell.Range.Bold = 1;
                    cell.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    cell.Range.Text = c == 1 ? "Nome" :
                        c == 2 ? "Cognome" :
                        c == 3 ? "Data Nascita" :
                        c == 4 ? "Luogo Nascita" :
                        c == 5 ? "Codice Fiscale" :
                        c == 6 ? "Tessera#" :
                        "Data Socio";
                }
                rownum++;

                foreach (Iscrizione i in iscrizioni)
                {
                    for (int c = 1; c < ncolonne+1; c++)
                    {
                        Cell cell = t.Cell(rownum, c);

                        cell.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                        cell.Range.Text = c == 1 ? i.persona.Nome :
                            c == 2 ? i.persona.Cognome :
                            c == 3 ? (i.persona.DataNascita == null ? string.Empty : ((DateTime)i.persona.DataNascita).ToString("d")) :
                            c == 4 ? i.persona.LuogoNascita :
                            c == 5 ? i.persona.CodiceFiscale :
                            c == 6 ? ""+i.tesseran :
                            (i.data_socio == null ? string.Empty : ((DateTime)i.data_socio).ToString("d"));
                    }
                    rownum++;
                }


                wordApp.PrintOut();
                if (shallclose)
                {
                    Microsoft.Office.Interop.Word._Document document = wordApp.ActiveDocument;
                    document.Close(Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges);
                    ((Microsoft.Office.Interop.Word._Application)wordApp).Quit();
                }
                else
                    wordApp.Visible = true;
          /*  }
            catch (Exception e)
            {
                String errorString = "Errore Stampando il Libro Soci:\n" + e.Message;

                if (Config.Instance.ShowErrors)
                    MessageBox.Show(errorString, "Errore ", MessageBoxButtons.OK, MessageBoxIcon.Error);

                Log.Instance.WriteLine(Log.LogLevel.Exception, errorString);

                try { ((Microsoft.Office.Interop.Word._Application)wordApp).Quit(); }
                catch (Exception e1)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Exception, "Errore chiudendo Office: "+e1.ToString());
                }
            }*/

        }



        public static void printIncassiGiorno(Object oTemplatePath, List<UNFHibernate.Windows.ViewIncassi.EntryIscrizione> pagamenti, DateTime DataIscrizione, bool shallclose)
        {
            Microsoft.Office.Interop.Word.Application wordApp = null;

            try
            {
                Object oMissing = System.Reflection.Missing.Value;

                wordApp = new Microsoft.Office.Interop.Word.Application();
                Document wordDoc = new Document();
                wordDoc = wordApp.Documents.Add(ref oTemplatePath, ref oMissing, ref oMissing, ref oMissing);

                foreach (Field myMergeField in wordDoc.Fields)
                {
                    Range rngFieldCode = myMergeField.Code;
                    String fieldText = rngFieldCode.Text;
                    if (fieldText.StartsWith(" MERGEFIELD"))
                    {
                        Int32 endMerge = fieldText.IndexOf("\\");
                        Int32 fieldNameLength = fieldText.Length - endMerge;
                        String fieldName = fieldText.Substring(11, endMerge - 11);

                        fieldName = fieldName.Trim();

                        if (fieldName == "Stagione" && DB.instance.stagione_corrente != null)
                        {
                            myMergeField.Select();
                            wordApp.Selection.TypeText(stringToWord(DB.instance.stagione_corrente.Descrizione));
                        }
                        else if (fieldName == "Oggi")
                        {
                            myMergeField.Select();
                            wordApp.Selection.TypeText(DateTime.Today.ToString("d"));
                        }
                        else if (fieldName == "Data")
                        {
                            myMergeField.Select();
                            wordApp.Selection.TypeText(DataIscrizione.ToString("d"));
                        }
                    }
                }

                int nrighe = pagamenti.Count + 1;
                int ncolonne = 5;

                object start = wordDoc.Content.End-1;
                object end = wordDoc.Content.End-1;
                Range tableLocation = wordDoc.Range(ref start, ref end);
                Table t = wordDoc.Tables.Add(tableLocation, nrighe, ncolonne);

                int rownum = 1;
                for (int c = 1; c < ncolonne + 1; c++)
                {
                    Cell cell = t.Cell(rownum, c);
                    cell.Range.Bold = 1;
                    cell.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    cell.Range.Text = c == 1 ? "Nome" :
                        c == 2 ? "Cognome" :
                        c == 3 ? "Corso" :
                        c == 4 ? "Modalita" :
                        "Importo";
                }
                rownum++;

                foreach (UNFHibernate.Windows.ViewIncassi.EntryIscrizione i in pagamenti)
                {
                    for (int c = 1; c < ncolonne + 1; c++)
                    {
                        Cell cell = t.Cell(rownum, c);

                        cell.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                        cell.Range.Text = c == 1 ? i.Nome :
                            c == 2 ? i.Cognome :
                            c == 3 ? i.Corso :
                            c == 4 ? i.Modalita :
                            i.Importo;
                    }
                    rownum++;
                }


                wordApp.PrintOut();
                if (shallclose)
                {
                    Microsoft.Office.Interop.Word._Document document = wordApp.ActiveDocument;
                    document.Close(Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges);
                    ((Microsoft.Office.Interop.Word._Application)wordApp).Quit();
                }
                else
                    wordApp.Visible = true;
            }
            catch (Exception e)
            {
                String errorString = "Errore Stampando gli incassi del giorno:\n" + e.Message;

                if (Config.Instance.ShowErrors)
                    MessageBox.Show(errorString, "Errore ", MessageBoxButtons.OK, MessageBoxIcon.Error);

                Log.Instance.WriteLine(Log.LogLevel.Exception, errorString);

                try { ((Microsoft.Office.Interop.Word._Application)wordApp).Quit(); }
                catch (Exception e1)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Exception, "Errore chiudendo Office: "+e1.ToString());
                }
            }

        }



        public static String stringToWord(String s)
        {
            return String.IsNullOrEmpty(s) ? " " : s;
        }
    }
}
