using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UNFHibernate.Components;
using UNFHibernate.Domain;
using UNFHibernate.Windows;

namespace UNFHibernate
{
    public partial class ViewAnagrafica : Form
    {
        public ViewAnagrafica()
        {
            InitializeComponent();
            mainform = null;
        }

        private MainForm mainform;
        public Persona persona { get; set; }


        public void setPersona(MainForm m, UNFHibernate.Domain.Persona p)
        {
            mainform = m;
            persona = p;

            if (persona != null)
            {
                textCognome.Text = persona.Cognome ?? string.Empty;
                textNome.Text = persona.Nome ?? string.Empty;

                Utils.SetPickerValidIfEnabled(persona.DataNascita, ref dateNascita);

                textLuogoNascita.Text = persona.LuogoNascita ?? string.Empty;

                if (persona.Male != null && persona.Male.Value)
                    checkBoxMaschio.Checked = true;

                textIndirizzo.Text = persona.Indirizzo ?? string.Empty;
                textComune.Text = persona.Comune ?? string.Empty;
                textCAP.Text = persona.CAP ?? string.Empty;
                textProvincia.Text = persona.Provincia ?? string.Empty;
                textCodiceFiscale.Text = persona.CodiceFiscale ?? string.Empty;
                textEmail.Text = persona.Email ?? string.Empty;
                textNumeroTelefono.Text = persona.NumeroTelefono ?? string.Empty;
                textNumeroCellulare.Text = persona.NumeroCellulare ?? string.Empty;
                textNote.Text = persona.Note ?? string.Empty;

                textGenitoreNome.Text = persona.GenitoreNome ?? string.Empty;
                textGenitoreCognome.Text = persona.GenitoreCognome ?? string.Empty;
                textGenitoreLuogoNascita.Text = persona.GenitoreLuogoNascita ?? string.Empty;
                Utils.SetPickerValidIfEnabled(persona.GenitoreDataNascita, ref dateNascitaGenitore);
            }
            else
            {
                Utils.SetPickerValid(dateNascita, false);
                Utils.SetPickerValid(dateNascitaGenitore, false);
            }

            RefreshIscrizioni();
        }


        public void UpdatePersona()
        {
            if (persona == null)
                persona = new Persona();

            persona.Cognome = textCognome.Text;
            persona.Nome = textNome.Text;
            persona.LuogoNascita = textLuogoNascita.Text;
            persona.Indirizzo = textIndirizzo.Text;
            persona.Comune = textComune.Text;
            persona.CAP = textCAP.Text;
            persona.Provincia = textProvincia.Text;
            persona.CodiceFiscale = textCodiceFiscale.Text;
            persona.Email = textEmail.Text;
            persona.NumeroTelefono = textNumeroTelefono.Text;
            persona.NumeroCellulare = textNumeroCellulare.Text;
            persona.Male = checkBoxMaschio.Checked;
            persona.Note = textNote.Text;
            if (dateNascita.Format == DateTimePickerFormat.Short)
                persona.DataNascita = dateNascita.Value;

            persona.GenitoreNome = textGenitoreNome.Text;
            persona.GenitoreCognome = textGenitoreCognome.Text;
            persona.GenitoreLuogoNascita = textGenitoreLuogoNascita.Text;
            if (dateNascitaGenitore.Format == DateTimePickerFormat.Short)
                persona.GenitoreDataNascita = dateNascitaGenitore.Value;
        }

        public void Save()
        {
            UpdatePersona();
            DB.instance.save(persona);

            foreach (TabPage page in panelIscrizioni.TabPages)
                foreach (Control control in page.Controls)
                    if (control is PanelIscrizione)
                    {
                        PanelIscrizione p = control as PanelIscrizione;
                        try { p.Save(); }
                        catch { }
                    }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Save();
            if (mainform != null)
                mainform.RefreshAnagrafiche();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Save();
            if (mainform != null)
                mainform.RefreshAnagrafiche();

            Close();
        }

        public static void Show(MainForm m, UNFHibernate.Domain.Persona p)
        {
            ViewAnagrafica va = new ViewAnagrafica();
            va.setPersona(m, p);
            va.mainform = m;
            va.Show();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            Utils.SetPickerValid(dateNascita, true);
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            Utils.SetPickerValid(dateNascitaGenitore, true);
        }

        public void setNewCorso(object id)
        {
            if (persona == null || persona.ID == null || persona.ID == Guid.Empty)
            {
                MessageBox.Show("Impossibile aggiungere il corso perchè la persona non è stata salvata.\nPremere \"Salva\" in basso a sinistra", "ERRORE");
                return;
            }

            Corso corso = DB.instance.getCorso((Guid)id);
            bool saldato = false;
            Iscrizione iscrz = new Iscrizione() { corso = corso, persona = persona, data_iscrizione = DateTime.Today, Saldato=saldato };

            bool found = false;

            foreach (Iscrizione vecchieisc in persona.Iscrizioni)
                if (vecchieisc.corso.ID == iscrz.corso.ID)
                {
                    found = true;
                    break;
                }

            if (found == true)
            {
                MessageBox.Show("Questa persona è già iscritta a quel corso!");
            }
            else
            {
                if (!persona.Iscrizioni.Contains(iscrz))
                    persona.Iscrizioni.Add(iscrz);

                if (!corso.Iscrizioni.Contains(iscrz))
                    corso.Iscrizioni.Add(iscrz);

                DB.instance.save(iscrz);
            }

            RefreshIscrizioni();
            mainform.RefreshAnagrafiche();
        }

        public void RefreshIscrizioni()
        {
            if (persona == null || !searchCambiaQualcosa())
                return;

            //panelIscrizioni.SuspendLayout();
            List<Iscrizione> ordered = persona.Iscrizioni /*.OrderByDescending(x => x.Saldato) */.OrderByDescending(x => x.corso.DataInizio).ToList();

            // per ogni panel
            while (panelIscrizioni.Controls.Count > 0)
            {
                var controltoremove = panelIscrizioni.Controls[0];
                panelIscrizioni.Controls.Remove(controltoremove);   // eliminalo
                controltoremove.Dispose();
            }

            foreach (Iscrizione i in ordered)
                if (!violaCriteriRicercaIscrizioni(i))
                {
                    PanelIscrizione pi = new PanelIscrizione();
                    pi.setIscrizione(i, mainform, this);
                    pi.Dock = DockStyle.Fill;

                    TabPage tp = new TabPage(i.corso.Descrizione);
                    tp.Controls.Add(pi);

                    panelIscrizioni.TabPages.Add(tp);
                }
            //panelIscrizioni.ResumeLayout();
        }

        private bool violaCriteriRicercaIscrizioni(Iscrizione i)
        {
            if (i.Saldato)
            {
                if (checkBoxCorsiAttivi.Checked && !Utils.isAttivo(i.corso.Attivo))
                    return true;

                if (checkBoxCorsiNonSaldati.Checked)
                    return true;
            }

            return false;
        }

        private bool searchCambiaQualcosa()
        {
            List<Iscrizione> processed = new List<Iscrizione>();

            foreach (Control c in panelIscrizioni.Controls)
            {
                // se non si riesce a convertire
                PanelIscrizione pi = c as PanelIscrizione;
                if (pi == null)
                    return true;

                // se ora l'iscrizione non deve più essere visualizzata
                if (violaCriteriRicercaIscrizioni(pi.iscrizione))
                    return true;

                // se l'iscrizione non esiste più
                if (!persona.Iscrizioni.Contains(pi.iscrizione))
                    return true;

                processed.Add(pi.iscrizione);
            }

            foreach (Iscrizione i in persona.Iscrizioni)
                if (!processed.Contains(i) && !violaCriteriRicercaIscrizioni(i))     // c'è nei controlli, e ora non viola i criteri
                    return true;

            return false;
        }

        private void button_calcCF(object sender, EventArgs e)
        {
            String nome = textNome.Text;
            String cognome = textCognome.Text;
            String comnas = textLuogoNascita.Text;
            bool m = checkBoxMaschio.Checked;
            DateTime data_nascita = dateNascita.Value;

            textCodiceFiscale.Text = CodiceFiscale.calculate(nome, cognome, data_nascita, comnas, m);
        }

        private void buttonAddIstruttore_Click(object sender, EventArgs e)
        {
            if (persona == null || persona.ID == null || persona.ID == Guid.Empty)
            {
                MessageBox.Show("Impossibile aggiungere l'iscrizione perchè la persona non è ancora stata salvata.\nPremere \"Salva\" in basso a sinistra", "ERRORE");
                return;
            }

            SelezionaCorso.Show(mainform, this);
        }

        private void Event_SearchCorsiChanged(object sender, EventArgs e)
        {
            RefreshIscrizioni();
        }
    }
}
