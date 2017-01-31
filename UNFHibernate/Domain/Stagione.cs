using Iesi.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNFHibernate.Domain
{
    public class Stagione
    {
        public virtual Guid ID { get; set; }
        public virtual string Descrizione { get; set; }
        public virtual DateTime DataInizio { get; set; }
        public virtual DateTime DataFine { get; set; }
        public virtual bool Corrente { get; set; }

        public virtual DateTime FineQuadrimestre { get; set; }

        public virtual ICollection<Corso> Corsi
        {
            get { return corsi ?? (corsi = new HashedSet<Corso>()); }
            set { corsi = value; }
        }
        private ICollection<Corso> corsi;

        public virtual ICollection<Chiusura> Chiusure
        {
            get { return chiusure ?? (chiusure = new HashedSet<Chiusura>()); }
            set { chiusure = value; }
        }
        private ICollection<Chiusura> chiusure;


        public virtual List<DateTime> getGiornate()
        {
            List<DateTime> lista = new List<DateTime>();

            for (DateTime i = DataInizio; i <= DataFine; i = i.AddDays(1))
                lista.Add(i);

            foreach (Chiusura c in Chiusure)
            {
                List<DateTime> giornichiuso = c.getGiornate();
                foreach (DateTime g in giornichiuso)
                    lista.Remove(g);
            }

            return lista;
        }
    }
}
