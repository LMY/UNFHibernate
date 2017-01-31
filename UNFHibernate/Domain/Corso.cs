using Iesi.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNFHibernate.Domain
{
    public class Corso
    {
        public virtual Guid ID { get; set; }
        public virtual Stagione stagione { get; set; }
        public virtual ListinoCorsi listino { get; set; }
        public virtual string Codice { get; set; }
        public virtual string Descrizione { get; set; }
        public virtual string Tipologia { get; set; }
        public virtual DateTime DataInizio { get; set; }
        public virtual DateTime DataFine { get; set; }
        public virtual int MaxIscritti { get; set; }
        public virtual string Attivo { get; set; }
        public virtual string CodiceStampa { get; set; }
        public virtual string TipoCartellino { get; set; }
        public virtual string Orario { get; set; }
        public virtual bool Bimbi { get; set; }

        public virtual ICollection<Iscrizione> Iscrizioni
        {
            get { return iscrizioni ?? (iscrizioni = new HashedSet<Iscrizione>()); }
            set { iscrizioni = value; }
        }
        private ICollection<Iscrizione> iscrizioni;

        public virtual ICollection<Istruttore> Istruttori
        {
            get { return istruttori ?? (istruttori = new List<Istruttore>()); }
            set { istruttori = value; }
        }
        private ICollection<Istruttore> istruttori;

        public static readonly char[] giorniCodes = { '1', '2', '3', '4', '5', '6', '7' };

        public virtual string getOrarioString()
        {
            var lista = EntryOrario.getOrarioList(Orario);

            string ret = string.Empty;

            foreach (EntryOrario lc in lista)
                for (int i = 0; i < lc.getNGiorniSettimana(); i++)
                    ret += (string.IsNullOrEmpty(ret) ? "" : ", ") + lc.Dalle.ToString("H:mm") + "-" + lc.Alle.ToString("H:mm");

            return ret;
        }

        public virtual bool[] getGiorniSettimana()
        {
            if (string.IsNullOrEmpty(Orario))
                return new bool[7];

            bool[] ret = new bool[7];

            var lista = EntryOrario.getOrarioList(Orario);
            foreach (EntryOrario lc in lista)
            {
                if (lc.Giornate.Contains("1"))
                    ret[0] = true;
                if (lc.Giornate.Contains("2"))
                    ret[1] = true;
                if (lc.Giornate.Contains("3"))
                    ret[2] = true;
                if (lc.Giornate.Contains("4"))
                    ret[3] = true;
                if (lc.Giornate.Contains("5"))
                    ret[4] = true;
                if (lc.Giornate.Contains("6"))
                    ret[5] = true;
                if (lc.Giornate.Contains("7"))
                    ret[6] = true;
            }

            return ret;
        }

        public virtual List<DateTime> getGiornate()
        {
            List<DateTime> giornistagione = stagione.getGiornate();
            bool[] giorno = getGiorniSettimana();

            List<DateTime> lista = new List<DateTime>();
            foreach (DateTime i in giornistagione)
                // solo se > data_inizio
                if (i.Date.CompareTo(DataInizio.Date) >= 0 && i.Date.CompareTo(DataFine.Date) <= 0)
                if (i.DayOfWeek == DayOfWeek.Monday && giorno[0] ||
                    i.DayOfWeek == DayOfWeek.Tuesday && giorno[1] ||
                    i.DayOfWeek == DayOfWeek.Wednesday && giorno[2] ||
                    i.DayOfWeek == DayOfWeek.Thursday && giorno[3] ||
                    i.DayOfWeek == DayOfWeek.Friday && giorno[4] ||
                    i.DayOfWeek == DayOfWeek.Saturday && giorno[5] ||
                    i.DayOfWeek == DayOfWeek.Sunday && giorno[6])
                    lista.Add(i);

            return lista;
        }


        //public virtual List<DateTime> getGiornate(Chiusura[] chiusure = null)
        //{
        //    List<DateTime> ret = new List<DateTime>();

        //    bool[] giorno = getGiorniSettimana();

        //    if (DataInizio != null && DataFine != null)
        //        for (DateTime i = DataInizio; i <= DataFine; i = i.AddDays(1))
        //        {
        //            bool skip = false;
        //            if (chiusure != null) // se sono specificate delle chiusure
        //                foreach (Chiusura c in chiusure)
        //                    if (c.DataInizio.CompareTo(i.Date) >= 0 && c.DataInizio.CompareTo(i.Date) <= 0)
        //                    {
        //                        skip = true;
        //                        break;
        //                    }
        //            if (skip)
        //                continue;

        //            if (i.DayOfWeek == DayOfWeek.Monday && giorno[0] ||
        //                i.DayOfWeek == DayOfWeek.Tuesday && giorno[1] ||
        //                i.DayOfWeek == DayOfWeek.Wednesday && giorno[2] ||
        //                i.DayOfWeek == DayOfWeek.Thursday && giorno[3] ||
        //                i.DayOfWeek == DayOfWeek.Friday && giorno[4] ||
        //                i.DayOfWeek == DayOfWeek.Saturday && giorno[5] ||
        //                i.DayOfWeek == DayOfWeek.Sunday && giorno[6])
        //                ret.Add(i);
        //        }

        //    return ret;
        //}
    }
}
