using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNFHibernate.Domain
{
    public class PersonaGenitore
    {
        public virtual Guid ID { get; set; }
        public virtual string Nome { get; set; }
        public virtual string Cognome { get; set; }
        public virtual DateTime? DataNascita { get; set; }
        public virtual string LuogoNascita { get; set; }

        public virtual Persona Figlio { get; set; }
    }
}
