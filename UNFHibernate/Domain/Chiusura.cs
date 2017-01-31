using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNFHibernate.Domain
{
    public class Chiusura
    {
        public virtual Guid ID { get; set; }
        public virtual string Descrizione { get; set; }
        public virtual DateTime DataInizio { get; set; }
        public virtual DateTime DataFine { get; set; }

        public virtual Stagione stagione { get; set; }


        public virtual List<DateTime> getGiornate()
        {
            List<DateTime> lista = new List<DateTime>();

            for (DateTime i = DataInizio; i <= DataFine; i = i.AddDays(1))
                lista.Add(i);

            return lista;
        }
    }
}
