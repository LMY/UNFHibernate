using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UNFHibernate.Domain;

namespace UNFHibernate
{
    public class HibernateHelper
    {
        public static ISessionFactory Factory { get; private set; }
        public static Configuration Config { get; private set; }

        public static void Init()
        {
            try
            {
                Config = new Configuration();
                Config.Configure();
                Factory = Config.BuildSessionFactory();

                UpdateSchema();
            }
            catch (Exception e)
            {
                Log.Instance.WriteLine(Log.LogLevel.Error, "CRITICO: Errore in HibernateHelper. Non è possibile connettersi al db, o c'è qualche problema col db: "+e.Message);
                //Environment.FailFast("CRITICO: Errore in HibernateHelper. Non è possibile connettersi al db, o c'è qualche problema col db...");
            }
        }

        public static ISession Open()
        {
            return Factory.OpenSession();
        }

        private static void UpdateSchema()
        {
            new SchemaUpdate(HibernateHelper.Config).Execute(true, true);
            //new SchemaExport(HibernateHelper.Config).Execute(true, true, true);
        }
    }
}
