using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UNFHibernate.Domain;

namespace UNFHibernate
{
    class DB
    {
        public static DB instance { get; private set; }
        public static void init()
        {
            instance = new DB();
        }

        public List<Persona> persone { get; private set; }
        public List<Corso> corsi { get; private set; }
        public List<Iscrizione> iscrizioni { get; private set; }
        public List<Chiusura> chiusure { get; private set; }
        public List<Stagione> stagioni { get; private set; }
        public List<ListinoCorsi> listini { get; private set; }

        public Stagione stagione_corrente { get; private set; }

        public List<Istruttore> istruttori { get; private set; }


        private DB()
        {
            Refresh();
        }

        public void Refresh()
        {
            Clear();
            ImportFromDb();
        }

        public void Clear()
        {
            stagione_corrente = null;

            listini = new List<ListinoCorsi>();
            persone = new List<Persona>();
            corsi = new List<Corso>();
            listini = new List<ListinoCorsi>();
            iscrizioni = new List<Iscrizione>();
            chiusure = new List<Chiusura>();
            stagioni = new List<Stagione>();
            istruttori = new List<Istruttore>();
        }

        public void ImportFromDb()
        {
            using (NHibernate.ISession session = HibernateHelper.Open())
            using (NHibernate.ITransaction transaction = session.BeginTransaction())
                try
                {
                    stagione_corrente = null;
                    IList<Stagione> dbstagioni = session.QueryOver<Stagione>().OrderBy(x => x.DataFine).Desc.List();
                    foreach (Stagione p in dbstagioni)
                    {
                        fetchStagione(p);
                        stagioni.Add(p);

                        if (stagione_corrente == null || p.DataInizio > stagione_corrente.DataInizio)   // calcola stagione corrente
                            stagione_corrente = p;
                    }

                    IList<ListinoCorsi> dblistino = session.QueryOver<ListinoCorsi>().List();
                    foreach (ListinoCorsi p in dblistino)
                    {
                        fetchListino(p);
                        listini.Add(p);
                    }

                    IList<Chiusura> dbchiusure = session.QueryOver<Chiusura>().OrderBy(x => x.DataFine).Desc.List();
                    foreach (Chiusura p in dbchiusure)
                        chiusure.Add(p);

                    IList<Persona> dbpersone = session.QueryOver<Persona>().OrderBy(x => x.Cognome).Asc.ThenBy(x => x.Nome).Asc.ThenBy(x => x.DataNascita).Desc.List(); 
                    foreach (Persona p in dbpersone)
                    {
                        fetchPersona(p);
                        persone.Add(p);
                    }

                    IList<Corso> dbcorsi = session.QueryOver<Corso>().OrderBy(x => x.Attivo).Desc.ThenBy(x => x.Codice).Asc.ThenBy(x => x.DataFine).Desc.List();
                        //.ThenBy(x => x.DataFine).Asc.ThenBy(x => x.DataInizio).Asc.List();
                    foreach (Corso p in dbcorsi)
                    {
                        fetchCorso(p);
                        corsi.Add(p);
                    }

                    IList<Istruttore> dbistruttori = session.QueryOver<Istruttore>().OrderBy(x => x.Cognome).Asc.ThenBy(x => x.Nome).Asc.List();
                    foreach (Istruttore p in dbistruttori)
                    {
                        fetchIstruttore(p);
                        istruttori.Add(p);
                    }

                    IList<Iscrizione> dbiscrizioni = session.QueryOver<Iscrizione>().List();
                    foreach (Iscrizione p in dbiscrizioni)
                        iscrizioni.Add(p);


                    transaction.Commit();
                }
                catch (Exception exc)
                {
                    String errorString = "ImportFromDb::" + exc.Message;
                    Log.Instance.WriteLine(Log.LogLevel.Error, errorString);
                }
        }


        // lazy getters
        private void fetchStagione(Stagione s)
        {
            List<Corso> corsiCollegati = s.Corsi.ToList();
            foreach (Corso iss in corsiCollegati)
            {
                object iid = iss.ID;
                object cid = iss.stagione.ID;

                //String name = iss.corso.Descrizione;
            }

            List<Chiusura> chiusCollegate = s.Chiusure.ToList();
            foreach (var c in chiusCollegate)
            {
                object iid = c.ID;
            }
        }

        private void fetchPersona(Persona p)
        {
            List<Iscrizione> iscrizionicollegate = p.Iscrizioni.ToList();
            foreach (Iscrizione iss in iscrizionicollegate)
            {
                object iid = iss.ID;
                object cid = iss.corso.ID;
                //String name = iss.corso.Descrizione;
            }
        }

        private void fetchListino(ListinoCorsi p)
        {
            List<Corso> corsicollegati = p.Corsi.ToList();
            foreach (Corso iss in corsicollegati)
            {
                object iid = iss.ID;
            }
        }

        private void fetchCorso(Corso p)
        {
            List<Istruttore> istruttori = p.Istruttori.ToList();
            List<Iscrizione> iscrizionicollegate = p.Iscrizioni.ToList();
            foreach (Iscrizione iss in iscrizionicollegate)
            {
                object iid = iss.ID;
                object pid = iss.persona.ID;
            }

            if (p.listino != null)
            {
                object lid = p.listino.ID;
            }
        }

        private void fetchIstruttore(Istruttore p)
        {
            List<Corso> corsi = p.Corsi.ToList();

            foreach (Corso c in corsi)
            {
                object cid = c.ID;
                //String name = c.Descrizione;
            }
        }


        // chiusura
        public Chiusura getChiusura(Guid id)
        {
            foreach (Chiusura c in chiusure)
                if (c.ID == id)
                    return c;

            return null;
        }

        public bool save(Chiusura chiusura)
        {
            int idx = chiusure.FindIndex(x => x.ID == chiusura.ID);

            if (idx >= 0)
                chiusure[idx] = chiusura;
            else
                chiusure.Add(chiusura);

            if (chiusura.stagione != null && !chiusura.stagione.Chiusure.Contains(chiusura))
            {
                chiusura.stagione.Chiusure.Add(chiusura);
                //retv &= DB.saveStagione(chiusura.stagione); // lmy:: mmmmm?!
            }

            return DB.saveChiusura(chiusura);
        }

        public bool removeChiusura(Chiusura chiusura)
        {
            if (chiusura.stagione != null && chiusura.stagione.Chiusure.Contains(chiusura))
            {
                chiusura.stagione.Chiusure.Remove(chiusura);
                DB.saveStagione(chiusura.stagione);
            }

            chiusure.Remove(chiusura);
            return DB.deleteChiusura(chiusura);
        }



        public ListinoCorsi getListino(Guid id)
        {
            foreach (ListinoCorsi c in listini)
                if (c.ID == id)
                    return c;

            return null;
        }

        public bool save(ListinoCorsi listino)
        {
            int idx = listini.FindIndex(x => x.ID == listino.ID);

            if (idx >= 0)
                listini[idx] = listino;
            else
                listini.Add(listino);

            return DB.saveListino(listino);
        }

        public bool removeListino(ListinoCorsi listino)
        {
            listini.Remove(listino);
            return DB.deleteListino(listino);
        }


        // stagione
        public Stagione getStagione(Guid id)
        {
            foreach (Stagione c in stagioni)
                if (c.ID == id)
                    return c;

            return null;
        }

        public bool save(Stagione stagione)
        {
            int idx = stagioni.FindIndex(x => x.ID == stagione.ID);

            if (idx >= 0)
                stagioni[idx] = stagione;
            else
                stagioni.Add(stagione);

            // controlla se è la nuova stagione corrente
            if (stagione_corrente == null || stagione.DataInizio > stagione_corrente.DataInizio)
                stagione_corrente = stagione;

            return DB.saveStagione(stagione);
        }

        public bool isStagioneCorrente(Stagione stagione)
        {
            if (stagione_corrente == null) return false;
            return stagione.ID == stagione_corrente.ID;
        }

        public void removeStagione(Stagione stagione)
        {
            stagioni.Remove(stagione);
            DB.deleteStagione(stagione);

            if (isStagioneCorrente(stagione))
            {
                // refresh stagioni
                stagione_corrente = null;
                stagioni.Clear();

                using (NHibernate.ISession session = HibernateHelper.Open())
                using (NHibernate.ITransaction transaction = session.BeginTransaction())
                    try
                    {
                        IList<Stagione> dbstagioni = session.QueryOver<Stagione>().OrderBy(x => x.DataFine).Desc.List();
                        foreach (Stagione p in dbstagioni)
                        {
                            stagioni.Add(p);

                            if (stagione_corrente == null || p.DataInizio > stagione_corrente.DataInizio)   // calcola stagione corrente
                                stagione_corrente = p;
                        }
                    }
                    catch (Exception exc)
                    {
                        String errorString = "removeStagione(" + (stagione!= null?(stagione.Descrizione??string.Empty):"null")+"): "+exc.Message;
                        Log.Instance.WriteLine(Log.LogLevel.Error, errorString);
                    }
            }
        }


        // istruttore
        public Istruttore getIstruttore(Guid id)
        {
            foreach (Istruttore c in istruttori)
                if (c.ID == id)
                    return c;

            return null;
        }

        public bool save(Istruttore istruttore)
        {
            int idx = istruttori.FindIndex(x => x.ID == istruttore.ID);

            if (idx >= 0)
                istruttori[idx] = istruttore;
            else
                istruttori.Add(istruttore);

            return DB.saveIstruttore(istruttore);
        }

        public bool removeIstruttore(Istruttore i)
        {
            // elimina i collegamenti coi corsi che tiene
            Corso[] corsi_che_tiene = i.Corsi.ToArray();
            foreach (Corso c in corsi_che_tiene)
            {
                c.Istruttori.Remove(i);
                i.Corsi.Remove(c);
            }

            istruttori.Remove(i);

            using (NHibernate.ISession session = HibernateHelper.Open())
            using (NHibernate.ITransaction transaction = session.BeginTransaction())
                try
                {
                    foreach (Corso c in corsi_che_tiene)
                        session.SaveOrUpdate(c);
                    session.SaveOrUpdate(i);
                    session.Delete(i);

                    transaction.Commit();
                    return true;
                }
                catch (Exception exc)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Error, "instance::removeIstruttore(" + (i != null ? i.ID.ToString() : "null") + "):" + exc.Message);
                    return false;
                }
        }

        // corso
        public Corso getCorso(Guid id)
        {
            foreach (Corso c in corsi)
                if (c.ID == id)
                    return c;

            return null;
        }

        public bool save(Corso corso)
        {
            int idx = corsi.FindIndex(x => x.ID == corso.ID);

            if (idx >= 0)
                corsi[idx] = corso;
            else
                corsi.Add(corso);

            if (corso.stagione != null && !corso.stagione.Corsi.Contains(corso))
            {
                corso.stagione.Corsi.Add(corso);
                //retv &= DB.saveStagione(chiusura.stagione); // lmy:: mmmmm?!
            }

            return DB.saveCorso(corso);
        }

        public bool removeCorso(Corso c)
        {
            if (c.listino != null)
                try
                {
                    var listino = c.listino;
                    listino.Corsi.Remove(c);
                    DB.saveListino(listino);
                }
                catch (Exception e) { Log.Instance.WriteLine("RemoveCorso::tentativo rimozione listino ha portato a un errore: " + e.ToString());  }

            if (c.stagione != null)
                try
                {
                    var stagione = c.stagione;
                    stagione.Corsi.Remove(c);
                    DB.saveStagione(stagione);
                }
                catch (Exception e) { Log.Instance.WriteLine("RemoveCorso::tentativo rimozione stagione ha portato a un errore: " + e.ToString()); }

            corsi.Remove(c);
            return DB.deleteCorso(c);
        }

        // iscrizione
        public Iscrizione getIscrizione(Guid id)
        {
            foreach (Iscrizione c in iscrizioni)
                if (c.ID == id)
                    return c;

            return null;
        }

        public bool save(Iscrizione iscrizione)
        {
            int idx = iscrizioni.FindIndex(x => x.ID == iscrizione.ID);

            if (idx >= 0)
                iscrizioni[idx] = iscrizione;
            else
                iscrizioni.Add(iscrizione);

            return DB.saveIscrizione(iscrizione);
        }

        public bool save(List<Iscrizione> iscrizioni)
        {
            foreach (Iscrizione iscrizione in iscrizioni)
            {
                int idx = iscrizioni.FindIndex(x => x.ID == iscrizione.ID);

                if (idx >= 0)
                {
                    iscrizioni[idx].data_socio = iscrizione.data_socio;
                    iscrizioni[idx].tesseran = iscrizione.tesseran;
                }
                else
                    iscrizioni.Add(iscrizione);
            }

            return DB.saveIscrizioni(iscrizioni);
        }


        // persone
        public Persona getPersona(Guid id)
        {
            foreach (Persona c in persone)
                if (c.ID == id)
                    return c;

            return null;
        }

        public bool save(Persona persona)
        {
            int idx = persone.FindIndex(x => x.ID == persona.ID);

            if (idx >= 0)
                persone[idx] = persona;
            else
                persone.Add(persona);

            bool ret = DB.savePersona(persona);

            return ret;
        }

        public bool removePersona(Persona p)
        {
            persone.Remove(p);
            return DB.deletePersona(p);
        }



        #region Persona
        public static bool savePersona(Persona[] p)
        {
            using (NHibernate.ISession session = HibernateHelper.Open())
            using (NHibernate.ITransaction transaction = session.BeginTransaction())
                try
                {
                    foreach (Persona pers in p)
                        session.SaveOrUpdate(pers);

                    transaction.Commit();
                    return true;
                }
                catch (Exception exc)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Error, "savePersone:" + exc.Message);
                    return false;
                }
        }

        public static bool savePersona(Persona p)
        {
            using (NHibernate.ISession session = HibernateHelper.Open())
            using (NHibernate.ITransaction transaction = session.BeginTransaction())
                try
                {
                    session.SaveOrUpdate(p);
                    transaction.Commit();
                    return true;
                }
                catch (Exception exc)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Error, "savePersona(" +
                            (p != null ?
                                p.Nome ?? string.Empty + " " + p.Cognome ?? string.Empty
                                : "null")+   "):" + exc.Message);

                    return false;
                }
        }

        public static bool deletePersona(Persona p)
        {
            using (NHibernate.ISession session = HibernateHelper.Open())
            using (NHibernate.ITransaction transaction = session.BeginTransaction())
                try
                {
                    session.Delete(p);
                    transaction.Commit();
                    return true;
                }
                catch (Exception exc)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Error, "deletePersona(" +
                        (p != null ?
                            p.Nome ?? string.Empty + " " + p.Cognome ?? string.Empty
                            : "null") + "):" + exc.Message);
                    return false;
                }
        }
#endregion



        public int getFirstCartellinoLibero()
        {
            int maxcart = 0;

            foreach (Iscrizione i in iscrizioni)
                if (i.tesseran > maxcart)
                    maxcart = i.tesseran;

            return maxcart+1;
        }


        public ListinoCorsi getListinoByName(String name)
        {
            foreach (ListinoCorsi c in listini)
                if (c.descrizione != null && c.descrizione.Equals(name))
                    return c;

            return null;
        }

        public Stagione getStagioneByName(String name)
        {
            foreach (Stagione s in stagioni)
                if (s.Descrizione != null && s.Descrizione.Equals(name))
                    return s;

            return null;
        }



        #region Corsi
        public static bool saveCorso(Corso p)
        {
            using (NHibernate.ISession session = HibernateHelper.Open())
            using (NHibernate.ITransaction transaction = session.BeginTransaction())
                try
                {
                    session.SaveOrUpdate(p);
                    transaction.Commit();
                    return true;
                }
                catch (Exception exc)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Error, "saveCorso(" + (p != null ? p.ID.ToString() : "null") + "):" + exc.Message);
                    return false;
                }
        }

        public static bool insertCorso(Corso p, Guid id)
        {
            using (NHibernate.ISession session = HibernateHelper.Open())
            using (NHibernate.ITransaction transaction = session.BeginTransaction())
                try
                {
                    session.Save(p, id);
                    transaction.Commit();
                    return true;
                }
                catch (Exception exc)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Error, "insertCorso(" + (p != null ? p.ID.ToString() : "null") + "   " + id.ToString() + "):" + exc.Message);
                    return false;
                }
        }

        public static bool deleteCorso(Corso p)
        {
            using (NHibernate.ISession session = HibernateHelper.Open())
            using (NHibernate.ITransaction transaction = session.BeginTransaction())
                try
                {
                    session.Delete(p);
                    transaction.Commit();
                    return true;
                }
                catch (Exception exc)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Error, "deleteCorso(" + (p != null ? p.ID.ToString() : "null") + "):" + exc.Message);
                    return false;
                }
        }
        #endregion

        #region Istruttori

        public static bool saveIstruttore(Istruttore p)
        {
            using (NHibernate.ISession session = HibernateHelper.Open())
            using (NHibernate.ITransaction transaction = session.BeginTransaction())
                try
                {
                    session.SaveOrUpdate(p);
                    transaction.Commit();
                    return true;
                }
                catch (Exception exc)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Error, "saveIstruttore(" + (p != null ? p.ID.ToString() : "null") + "):" + exc.Message);
                    return false;
                }
        }

        public static bool insertIstruttore(Istruttore p, Guid id)
        {
            using (NHibernate.ISession session = HibernateHelper.Open())
            using (NHibernate.ITransaction transaction = session.BeginTransaction())
                try
                {
                    session.Save(p, id);
                    transaction.Commit();
                    return true;
                }
                catch (Exception exc)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Error, "insertIstruttore(" + (p != null ? p.ID.ToString() : "null") + "   " + id.ToString() + "):" + exc.Message);
                    return false;
                }
        }

        public static bool deleteIstruttore(Istruttore p)
        {
            using (NHibernate.ISession session = HibernateHelper.Open())
            using (NHibernate.ITransaction transaction = session.BeginTransaction())
                try
                {
                    session.Delete(p);
                    transaction.Commit();
                    return true;
                }
                catch (Exception exc)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Error, "deleteIstruttore(" + (p != null ? p.ID.ToString() : "null") + "):" + exc.Message);
                    return false;
                }
        }
        #endregion

        #region Stagioni
        public static Stagione getStagioneCorrente()
        {
            Stagione ret = null;
            using (NHibernate.ISession session = HibernateHelper.Open())
            using (NHibernate.ITransaction transaction = session.BeginTransaction())
                try
                {
                    IList<Stagione> stag = session.QueryOver<Stagione>().OrderBy(x => x.DataInizio).Desc.List();

                    ret = stag.Count == 0 ? null : stag.ElementAt(0);

                    transaction.Commit();
                }
                catch (Exception exc)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Error, "getStagioneCorrente::" + exc.Message);
                    return null;
                }

            return ret;
        }

        public static bool insertStagione(Stagione p, Guid id)
        {
            using (NHibernate.ISession session = HibernateHelper.Open())
            using (NHibernate.ITransaction transaction = session.BeginTransaction())
                try
                {
                    session.Save(p, id);
                    transaction.Commit();
                    return true;
                }
                catch (Exception exc)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Error, "insertStagione(" + (p != null ? p.ID.ToString() : "null") + "   " + id.ToString() + "):" + exc.Message);
                    return false;
                }
        }

        public static bool saveStagione(Stagione p)
        {
            using (NHibernate.ISession session = HibernateHelper.Open())
            using (NHibernate.ITransaction transaction = session.BeginTransaction())
                try
                {
                    session.SaveOrUpdate(p);
                    transaction.Commit();
                    return true;
                }
                catch (Exception exc)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Error, "saveStagione(" + (p != null ? p.ID.ToString() : "null") + "):" + exc.Message);
                    return false;
                }
        }

        public static bool deleteStagione(Stagione p)
        {
            using (NHibernate.ISession session = HibernateHelper.Open())
            using (NHibernate.ITransaction transaction = session.BeginTransaction())
                try
                {
                    session.Delete(p);
                    transaction.Commit();
                    return true;
                }
                catch (Exception exc)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Error, "deleteStagione(" + (p != null ? p.ID.ToString() : "null") + "):" + exc.Message);
                    return false;
                }
        }

        #endregion

        #region Chiusure
        public static bool insertChiusura(Chiusura p, Guid id)
        {
            using (NHibernate.ISession session = HibernateHelper.Open())
            using (NHibernate.ITransaction transaction = session.BeginTransaction())
                try
                {
                    session.Save(p, id);
                    transaction.Commit();
                    return true;
                }
                catch (Exception exc)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Error, "insertChiusura(" + (p != null ? p.ID.ToString() : "null") + "   "+ id.ToString() +  "):" + exc.Message);
                    return false;
                }
        }

        public static bool saveChiusura(Chiusura p)
        {
            using (NHibernate.ISession session = HibernateHelper.Open())
            using (NHibernate.ITransaction transaction = session.BeginTransaction())
                try
                {
                    session.SaveOrUpdate(p);
                    transaction.Commit();
                    return true;
                }
                catch (Exception exc)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Error, "saveChiusura(" + (p != null ? p.ID.ToString() : "null") + "):" + exc.Message);
                    return false;
                }
        }

        public static bool deleteChiusura(Chiusura p)
        {
            using (NHibernate.ISession session = HibernateHelper.Open())
            using (NHibernate.ITransaction transaction = session.BeginTransaction())
                try
                {
                    session.Delete(p);
                    transaction.Commit();
                    return true;
                }
                catch (Exception exc)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Error, "deleteChiusura(" + (p != null ? p.ID.ToString() : "null") + "):" + exc.Message);
                    return false;
                }
        }

        #endregion

        #region Iscrizione
        public static bool saveIscrizione(Iscrizione p)
        {
            using (NHibernate.ISession session = HibernateHelper.Open())
            using (NHibernate.ITransaction transaction = session.BeginTransaction())
                try
                {
                    session.SaveOrUpdate(p);
                    transaction.Commit();
                    return true;
                }
                catch (Exception exc)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Error, "saveIscrizione(" + (p != null ? p.ID.ToString() : "null") + "):" + exc.Message);
                    return false;
                }
        }

        public static bool saveIscrizioni(List<Iscrizione> p)
        {
            using (NHibernate.ISession session = HibernateHelper.Open())
            using (NHibernate.ITransaction transaction = session.BeginTransaction())
                try
                {
                    foreach (Iscrizione i in p)
                        session.SaveOrUpdate(i);

                    transaction.Commit();
                    return true;
                }
                catch (Exception exc)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Error, "saveIscrizioni::" + exc.Message);
                    return false;
                }
        }

        public static bool deleteIscrizione(Iscrizione p)
        {
            using (NHibernate.ISession session = HibernateHelper.Open())
            using (NHibernate.ITransaction transaction = session.BeginTransaction())
                try
                {
                    session.Delete(p);
                    transaction.Commit();
                    return true;
                }
                catch (Exception exc)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Error, "deleteIscrizione(" + (p != null ? p.ID.ToString() : "null") + "):" + exc.Message);
                    return false;
                }
        }
        #endregion

        #region Listino

        public static bool insertListino(ListinoCorsi p, Guid id)
        {
            using (NHibernate.ISession session = HibernateHelper.Open())
            using (NHibernate.ITransaction transaction = session.BeginTransaction())
                try
                {
                    session.Save(p, id);
                    transaction.Commit();
                    return true;
                }
                catch (Exception exc)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Error, "insertListino(" + (p != null ? p.ID.ToString() : "null") + "   " + id.ToString() + "):" + exc.Message);
                    return false;
                }
        }

        public static bool saveListino(ListinoCorsi p)
        {
            using (NHibernate.ISession session = HibernateHelper.Open())
            using (NHibernate.ITransaction transaction = session.BeginTransaction())
                try
                {
                    session.SaveOrUpdate(p);
                    transaction.Commit();
                    return true;
                }
                catch (Exception exc)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Error, "saveListino(" + (p != null ? p.ID.ToString() : "null") + "):" + exc.Message);
                    return false;
                }
        }

        public static bool deleteListino(ListinoCorsi p)
        {
            using (NHibernate.ISession session = HibernateHelper.Open())
            using (NHibernate.ITransaction transaction = session.BeginTransaction())
                try
                {
                    session.Delete(p);
                    transaction.Commit();
                    return true;
                }
                catch (Exception exc)
                {
                    Log.Instance.WriteLine(Log.LogLevel.Error, "deleteStagione(" + (p != null ? p.ID.ToString() : "null") + "):" + exc.Message);
                    return false;
                }
        }

        #endregion



        public Stagione getStagioneById(Guid id)
        {
            foreach (Stagione s in stagioni)
                if (s.ID == id)
                    return s;

            return stagione_corrente;
        }

        public ListinoCorsi getListinoById(Guid id)
        {
            foreach (ListinoCorsi s in listini)
                if (s.ID == id)
                    return s;

            return null;
        }

        public int removePersoneDuplicate()
        {
            Dictionary<String, List<Persona>> coppie = new Dictionary<string, List<Persona>>();
            foreach (Persona p in persone)
            {
                String name = p.Nome + p.Cognome + (p.DataNascita!=null? Utils.dateToPrintableString(p.DataNascita.Value.Date) : string.Empty);
                
                List<Persona> lista = null;
                if (!coppie.TryGetValue(name, out lista))
                    lista = new List<Persona>();

                lista.Add(p);
                coppie[name] = lista;
            }

            int removed = 0;

            foreach (var x in coppie.Keys)
            {
                List<Persona> simili = coppie[x];

                // find max
                int max = simili[0].countEmptyFields();
                int maxi = 0;

                for (int i=1; i<simili.Count; i++)
                {
                    int thisempty = simili[i].countEmptyFields();
                    if (thisempty < max)
                    {
                        max = thisempty;
                        maxi = i;
                    }
                }

                // delete all others
                for (int i = 0; i < simili.Count; i++)
                    if (i != maxi)
                    {
                        this.removePersona(simili[i]);
                        removed++;
                    }
            }

            return removed;
        }




    }
}
