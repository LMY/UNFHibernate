using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNFHibernate.Domain
{
    public class Istruttore
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

        public virtual string Note { get; set; }

        public virtual bool? Male { get; set; }

        public virtual IList<Corso> Corsi
        {
            get { return corsi ?? (corsi = new List<Corso>()); }
            set { corsi = value; }
        }
        private IList<Corso> corsi;
    }
}
