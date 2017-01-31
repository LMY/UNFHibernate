using Iesi.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNFHibernate.Domain
{
    public class Persona
    {
        public virtual Guid ID { get; set; }
        public virtual string Nome { get; set; }
        public virtual string Cognome { get; set; }
        public virtual DateTime? DataNascita { get; set; }
        public virtual string LuogoNascita { get; set; }
        public virtual string Indirizzo { get; set; }
        public virtual string Comune { get; set; }
        public virtual string CAP { get; set; }
        public virtual string Provincia { get; set; }
        public virtual string CodiceFiscale { get; set; }

        public virtual string NumeroTelefono { get; set; }
        public virtual string NumeroCellulare { get; set; }
        public virtual string Email { get; set; }

        public virtual int? Tessera { get; set; }
        public virtual DateTime? DataCertificato { get; set; }
        public virtual DateTime? DataIscrizione { get; set; }
        public virtual bool? TassaIscrizione { get; set; }

        public virtual string Note { get; set; }

        public virtual bool? Male { get; set; }

        public virtual string GenitoreNome { get; set; }
        public virtual string GenitoreCognome { get; set; }
        public virtual DateTime? GenitoreDataNascita { get; set; }
        public virtual string GenitoreLuogoNascita { get; set; }


        public virtual int countEmptyFields()
        {
            int cnt = 0;

            if (!string.IsNullOrEmpty(Nome))
                cnt++;
            if (!string.IsNullOrEmpty(Cognome))
                cnt++;
            if (DataNascita != null)
                cnt++;
            if (!string.IsNullOrEmpty(LuogoNascita))
                cnt++;
            if (!string.IsNullOrEmpty(Indirizzo))
                cnt++;
            if (!string.IsNullOrEmpty(Comune))
                cnt++;
            if (!string.IsNullOrEmpty(CAP))
                cnt++;
            if (!string.IsNullOrEmpty(Provincia))
                cnt++;
            if (!string.IsNullOrEmpty(CodiceFiscale))
                cnt++;
            if (!string.IsNullOrEmpty(NumeroTelefono))
                cnt++;
            if (!string.IsNullOrEmpty(NumeroCellulare))
                cnt++;
            if (!string.IsNullOrEmpty(Email))
                cnt++;

            if (Tessera != null)
                cnt++;
            if (DataCertificato != null)
                cnt++;
            if (DataIscrizione != null)
                cnt++;
            if (TassaIscrizione != null)
                cnt++;
            if (Male != null)
                cnt++;

            if (!string.IsNullOrEmpty(GenitoreNome))
                cnt++;
            if (!string.IsNullOrEmpty(GenitoreCognome))
                cnt++;
            if (!string.IsNullOrEmpty(GenitoreLuogoNascita))
                cnt++;
            if (GenitoreDataNascita != null)
                cnt++;


            return cnt;
        }


        public virtual ICollection<Iscrizione> Iscrizioni
        {
            get { return iscrizioni ?? (iscrizioni = new HashedSet<Iscrizione>()); }
            set { iscrizioni = value; }
        }
        private ICollection<Iscrizione> iscrizioni;



        public virtual long getDovuto()
        {
            long dovuto = 0;

            foreach (Iscrizione i in iscrizioni)
            {
                long icorso = i.importo;
                icorso -= i.primopagamento_importo;
                icorso -= i.secondopagamento_importo;
                icorso -= i.terzopagamento_importo;

                if (icorso > 0)
                    dovuto += icorso;
            }

            return dovuto;
        }
    }
}
