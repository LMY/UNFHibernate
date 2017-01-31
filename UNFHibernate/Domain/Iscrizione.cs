using Iesi.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNFHibernate.Domain
{
    public class Iscrizione
    {
        public virtual Guid ID { get; set; }

        public virtual Persona persona { get; set; }
        public virtual Corso corso { get; set; }

        public virtual DateTime data_iscrizione { get; set; }

        public virtual DateTime? data_inizio { get; set; }

        public virtual DateTime? data_socio { get; set; }
        public virtual int tesseran { get; set; }

        public virtual int ingressi { get; set; }

        public virtual DateTime? data_certificato { get; set; }
        public virtual DateTime? data_rinuncia { get; set; }

        public virtual bool tassa_iscrizione { get; set; }

        public virtual long importo { get; set; }

        public virtual bool Saldato { get; set; }


        public virtual DateTime? primopagamento_data { get; set; }
        public virtual String primopagamento_modalita { get; set; }
        public virtual long primopagamento_importo { get; set; }

        public virtual DateTime? secondopagamento_data { get; set; }
        public virtual String secondopagamento_modalita { get; set; }
        public virtual long secondopagamento_importo { get; set; }

        public virtual DateTime? terzopagamento_data { get; set; }
        public virtual String terzopagamento_modalita { get; set; }
        public virtual long terzopagamento_importo { get; set; }


        public virtual DateTime getLastDate(int ingressiDaUsare = -1, DateTime? calculataDataInizio = null)
        {
            if (calculataDataInizio == null)
            {
                if (data_inizio != null)
                    calculataDataInizio = data_inizio.Value.Date;
                else
                    return DateTime.Today;
            }
            if (ingressiDaUsare < 0)
                ingressiDaUsare = ingressi;
            
            List<DateTime> giornatecorso = corso.getGiornate();

            if (giornatecorso.Count == 0)
                return DateTime.Today;

            if (data_inizio == null)
                return giornatecorso.ElementAt(giornatecorso.Count - 1);

            int starting = -1;

            for (int i=0; i<giornatecorso.Count; i++)
                if (giornatecorso.ElementAt(i).CompareTo(calculataDataInizio.Value.Date) >= 0)
                {
                    starting = i;
                    break;
                }

            int idx = starting + ingressiDaUsare - 1;

            if (starting < 0 || idx < 0 || idx >= giornatecorso.Count)
                return giornatecorso.ElementAt(giornatecorso.Count - 1);

            return giornatecorso.ElementAt(idx);
        }
    }
}
