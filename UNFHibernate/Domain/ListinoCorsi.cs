using Iesi.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UNFHibernate.Domain
{
    public class ListinoCorsi
    {
        public virtual Guid ID { get; set; }
        public virtual string ingressi { get; set; }
        public virtual string descrizione { get; set; }

        public virtual ICollection<Corso> Corsi
        {
            get { return corsi ?? (corsi = new HashedSet<Corso>()); }
            set { corsi = value; }
        }
        private ICollection<Corso> corsi;
    }
}
