using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using UNFHibernate.Domain;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace UNFHibernate.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Can_generate_schema()
        {
            HibernateHelper.Init();
        }

        [Test]
        public void Can_add_persona()
        {
            HibernateHelper.Init();

            Persona p1 = new Persona { Nome = "Qualcuno", DataNascita = new DateTime(1981, 12, 12), Male = true };
            using (ISession session = HibernateHelper.Open())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(p1);
                transaction.Commit();
            }
        }

        [Test]
        public void Can_add_corso()
        {
            HibernateHelper.Init();

            Corso c1 = new Corso { Codice = "Cod1", Descrizione = "Corso Figo" };
            using (ISession session = HibernateHelper.Open())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(c1);
                transaction.Commit();
            }
        }

        [Test]
        public void Can_add_stagione()
        {
            HibernateHelper.Init();

            Stagione c1 = new Stagione { Corrente = false, Descrizione = "ci sarà sicuro il sole" };
            using (ISession session = HibernateHelper.Open())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(c1);
                transaction.Commit();
            }
        }

        [Test]
        public void Can_add_chiusura()
        {
            HibernateHelper.Init();

            using (ISession session = HibernateHelper.Open())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(new Chiusura { DataFine = DateTime.Today, DataInizio = DateTime.Today, Descrizione = "perchè son robe" });
                transaction.Commit();
            }
        }
    }
}
