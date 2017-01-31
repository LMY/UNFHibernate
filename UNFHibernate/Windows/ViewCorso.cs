using Iesi.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UNFHibernate.Domain;
using UNFHibernate.Windows;

namespace UNFHibernate
{
    public partial class ViewCorso : Form
    {
        private ListinoCorsi oldListino = null;
        private Stagione oldStagione = null;

        public ViewCorso()
        {
            InitializeComponent();
        }

        public static void Show(MainForm m, UNFHibernate.Domain.Corso c)
        {
            ViewCorso va = new ViewCorso();
            va.setCorso(c);
            va.mainform = m;
            va.Show();
            va.Event_ChangeWarningColors(null, null);
        }

        public MainForm mainform { get; set; }
        public UNFHibernate.Domain.Corso corso { get; private set; }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Save();
            if (mainform != null)
                mainform.RefreshCorsi();
        }

        private void buttonSaveExit_Click(object sender, EventArgs e)
        {
            if (Save())
            {
                if (mainform != null)
                    mainform.RefreshCorsi();

                Close();
            }
        }

        public bool Save()
        {
            if (String.IsNullOrEmpty(comboListino.Text))
            {
                MessageBox.Show("Non puoi salvare il corso senza scegliere un listino!", "ERRORE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
            {
                UpdateData();
                return DB.instance.save(corso);
            }
        }

        public void setCorso(UNFHibernate.Domain.Corso c)
        {
            comboTipologie.Items.Clear();
            String[] tipologie = Config.Instance.TipologiaCorsi;
            foreach (String s in tipologie)
                comboTipologie.Items.Add(s);

            comboCartellini.Items.Clear();
            String[] cartellini = Config.Instance.Cartellini;
            foreach (String s in cartellini)
                comboCartellini.Items.Add(s);

            comboStagioni.Items.Clear();
            List<Stagione> stagioni = DB.instance.stagioni;
            foreach (Stagione s in stagioni)
                comboStagioni.Items.Add(s.Descrizione);

            comboListino.Items.Clear();
            List<ListinoCorsi> ilistini = DB.instance.listini;
            foreach (ListinoCorsi s in ilistini)
                comboListino.Items.Add(s.descrizione);
            
            corso = c;

            if (corso != null)
            {
                textDescrizione.Text = corso.Descrizione ?? string.Empty;
                textCodice.Text = corso.Codice ?? string.Empty;
                textCodiceStampa.Text = corso.CodiceStampa ?? string.Empty;
                RefreshOrari();
                textMaxIscritti.Text = corso.MaxIscritti.ToString();

                if (corso.Tipologia != null)
                {
                    if (!comboTipologie.Items.Contains(corso.Tipologia))
                        comboTipologie.Items.Add(corso.Tipologia);
                    comboTipologie.Text = corso.Tipologia;
                }

                if (corso.TipoCartellino != null)
                {
                    if (!comboCartellini.Items.Contains(corso.TipoCartellino))
                        comboCartellini.Items.Add(corso.TipoCartellino);
                    comboCartellini.Text = corso.TipoCartellino;
                }

                if (corso.stagione != null)
                {
                    if (!comboStagioni.Items.Contains(corso.stagione.Descrizione))
                        comboStagioni.Items.Add(corso.stagione.Descrizione);
                    comboStagioni.Text = corso.stagione.Descrizione;
                }

                if (corso.listino != null)
                {
                    if (!comboListino.Items.Contains(corso.listino.descrizione))
                        comboListino.Items.Add(corso.listino.descrizione);
                    comboListino.Text = corso.listino.descrizione;
                }

                checkBimbi.Checked = corso.Bimbi;
                checkAttivo.Checked = Utils.isAttivo(corso.Attivo);

                Utils.SetPickerValidIfEnabled(corso.DataInizio, ref dateInizio);
                Utils.SetPickerValidIfEnabled(corso.DataFine, ref dateFine);
            }
            else
            {
                Log.Instance.WriteLine("Creazione ViewCorso con corso == null??");
                Utils.SetPickerValid(dateInizio, false);
                Utils.SetPickerValid(dateFine, false);
            }

            RefreshGrids();
        }

        public void UpdateData()
        {

            if (corso == null)
            {
                corso = new UNFHibernate.Domain.Corso();
                corso.stagione = DB.instance.stagione_corrente;
            }
            else
            {
                foreach (Stagione s in DB.instance.stagioni)
                    if (s.Descrizione.Equals(comboStagioni.Text))
                    {
                        // se è stata cambiata la stagione, si deve rimuovere questo corso dalla lista di corsi di quella stagione
                        if (s.ID != corso.stagione.ID)
                        { 
                            corso.stagione.Corsi.Remove(corso);
                            corso.stagione = s;
                        }
                        break;
                    }
            }

            corso.Descrizione = textDescrizione.Text;
            corso.Codice = textCodice.Text;
            corso.CodiceStampa = textCodiceStampa.Text;
            
            try
            {
                corso.Orario = EntryOrario.getOrarioString(lista_orari);
            }
            catch { corso.Orario = string.Empty; }

            corso.Tipologia = comboTipologie.Text;
            corso.TipoCartellino = comboCartellini.Text;

            corso.Bimbi = checkBimbi.Checked;
            corso.Attivo = Utils.toAttivo(checkAttivo.Checked);

            try { corso.MaxIscritti = int.Parse(textMaxIscritti.Text); }
            catch { corso.MaxIscritti = 0; }

            //corso.listino 
            ListinoCorsi nuovolistino = DB.instance.getListinoByName(comboListino.Text);
            if (nuovolistino.ID != corso.listino.ID)
            {
                corso.listino.Corsi.Remove(corso);
                corso.listino = nuovolistino;
            }

            if (dateInizio.Format == DateTimePickerFormat.Short)
                corso.DataInizio = dateInizio.Value;

            if (dateFine.Format == DateTimePickerFormat.Short)
                corso.DataFine = dateFine.Value;
        }


        public void CalculateIngressi()
        {
            if (dateFine.Value == null || dateInizio.Value == null)
            {
                textIngressiTotali.Text = string.Empty;
                textIngressiRestanti.Text = string.Empty;
            }
            else
            {
                textIngressiTotali.Text = countGiornate(dateFine.Value, dateInizio.Value).ToString();
                textIngressiRestanti.Text = countGiornate(dateFine.Value, DateTime.Today).ToString();
            }
        }

        private int countGiornate(DateTime to, DateTime from)
        {
            if (from == null || to == null || to < from)
                return 0;
            
            int ret = 0;

            DateTime maxdate = to.Date;
            IList<Chiusura> lechiusure = DB.instance.chiusure;

            bool[] giorno = getGiornate();

            for (DateTime i = from.Date; i <= maxdate; i = i.AddDays(1))
                if (i.DayOfWeek == DayOfWeek.Monday && giorno[0] ||
                    i.DayOfWeek == DayOfWeek.Tuesday && giorno[1] ||
                    i.DayOfWeek == DayOfWeek.Wednesday && giorno[2] ||
                    i.DayOfWeek == DayOfWeek.Thursday && giorno[3] ||
                    i.DayOfWeek == DayOfWeek.Friday && giorno[4] ||
                    i.DayOfWeek == DayOfWeek.Saturday && giorno[5] ||
                    i.DayOfWeek == DayOfWeek.Sunday && giorno[6])
                {
                    if (lechiusure != null)
                    {
                        bool found = false;

                        foreach (Chiusura c in lechiusure)
                            if (c.DataFine != null && c.DataInizio != null)
                            if (c.DataInizio <= i && c.DataFine >= i)
                            {
                                found = true;
                                break;
                            }

                        if (found)      // niente
                            continue;
                    }

                    ret++;
                }

            return ret;
        }


        private void dateInizio_ValueChanged(object sender, EventArgs e)
        {
            CalculateIngressi();
        }

        private static readonly String[] validIstruttoriColumn = new String[] { "Nome", "Cognome", "DataNascita", "NumeroCellulare" };
        private static readonly String[] validOrarioColumn = new String[] { "Giornate", "Dalle", "Alle", "Corsie" };

        public void RefreshGrids()
        {
            RefreshIstruttori();
            RefreshIscritti();
        }

        public void RefreshIstruttori()
        {
            if (corso == null)
                return;
            BindingSource m_bs = new BindingSource();
            m_bs.DataSource = corso.Istruttori;

            dataGridViewIstruttori.DataSource = m_bs;
            dataGridViewIstruttori.AutoGenerateColumns = true;

            if (validIstruttoriColumn.Length > 0)
                for (int i = 0; i < dataGridViewIstruttori.Columns.Count; i++)
                    dataGridViewIstruttori.Columns[i].Visible = validIstruttoriColumn.Contains(dataGridViewIstruttori.Columns[i].Name);
        }

        private List<EntryOrario> lista_orari;

        public void RefreshOrari()
        {
            if (corso == null)
                return;

            BindingSource m_bs = new BindingSource();
            lista_orari = EntryOrario.getOrarioList(corso.Orario);
            m_bs.DataSource = lista_orari;

            gridOrari.DataSource = m_bs;
            gridOrari.AutoGenerateColumns = true;

            if (validOrarioColumn.Length > 0)
                for (int i = 0; i < gridOrari.Columns.Count; i++)
                {
                    gridOrari.Columns[i].Visible = validOrarioColumn.Contains(gridOrari.Columns[i].Name);

                    if (gridOrari.Columns[i].Name.Contains("lle"))  // Dalle e Alle :)
                        gridOrari.Columns[i].DefaultCellStyle.Format = "H:mm";
                }
        }


        public void setNewIstruttore(object selectedIstruttore)
        {
            if (corso == null)
                return;

            Istruttore istr = DB.instance.getIstruttore((Guid) selectedIstruttore);

            if (!istr.Corsi.Contains(corso))
                istr.Corsi.Add(corso);

            if (!corso.Istruttori.Contains(istr))
                corso.Istruttori.Add(istr);

            DB.instance.save(corso);
            DB.instance.save(istr);

            RefreshIstruttori();
        }

        private void buttonAddIstruttore_Click(object sender, EventArgs e)
        {
            if (corso == null || corso.ID == null || corso.ID == Guid.Empty)
            {
                MessageBox.Show("Impossibile aggiungere l'istruttore perchè il corso non è ancora stato salvato.\nPremere \"Salva\" in basso a sinistra", "ERRORE");
                return;
            }

            SelezionaIstruttore.Show(mainform, this);
        }

        private void buttonDelIstruttore_Click(object sender, EventArgs e)
        {
            object id = null;

            try { id = dataGridViewIstruttori.Rows[dataGridViewIstruttori.SelectedCells[0].RowIndex].Cells[0].Value; }
            catch { }

            if (id == null)
                return;

            Istruttore istr = DB.instance.getIstruttore((Guid) id);

            istr.Corsi.Remove(corso);
            corso.Istruttori.Remove(istr);

            DB.instance.save(corso);
            DB.instance.save(istr);

            RefreshIstruttori();
        }

        
        
        public void RefreshIscritti()
        {
            if (corso == null)
                return;

            List<Iscrizione> nuovalistaiscrizioni = new List<Iscrizione>();

            foreach (Iscrizione i in corso.Iscrizioni)
                nuovalistaiscrizioni.Add(i);

            corso.Iscrizioni = nuovalistaiscrizioni;
            List<EntryIscrizione> lista = EntryIscrizione.createList(nuovalistaiscrizioni);
            dataGridViewIscritti.DataSource = lista;
            dataGridViewIscritti.AutoGenerateColumns = true;

            int cnt = 0;
            foreach (var x in lista)
                if (x.DaContare)
                    cnt++;
            textBox7.Text = ""+cnt;
            for (int i = 0; i < lista.Count; i++)
                if (!lista.ElementAt(i).DaContare)
                    dataGridViewIscritti.Rows[i].DefaultCellStyle.ForeColor = Color.DarkRed;
            //dataGridViewIscritti.Rows[i].DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(255, System.Drawing.Color.FromArgb(120, 120, 120));

            for (int i = 0; i < dataGridViewIscritti.Columns.Count; i++)
            {
                dataGridViewIscritti.Columns[i].Visible = !dataGridViewIscritti.Columns[i].Name.Equals("ID") && !dataGridViewIscritti.Columns[i].Name.Equals("DaContare");

                if (dataGridViewIscritti.Columns[i].Name.Contains("Data"))
                    dataGridViewIscritti.Columns[i].DefaultCellStyle.Format = "d";
            }

            dataGridViewIscritti.Refresh();
        }


        public class EntryIscrizione
        {
            public object ID { get; set; }
            public String Nome { get; set; }
            public String Cognome { get; set; }
            public DateTime? DataNascita { get; set; }
            public DateTime? DataFine { get; set; }

            public bool DaContare { get; set; }

            public EntryIscrizione(Iscrizione i)
            {
                ID = i.ID;
                Nome = i.persona.Nome;
                Cognome = i.persona.Cognome;
                DataNascita = i.persona.DataNascita;
                DataFine = i.getLastDate();

                try { DaContare = DataFine.Value.CompareTo(DateTime.Today) >= 0; }
                catch { DaContare = false; }
            }

            public static List<EntryIscrizione> createList(List<Iscrizione> iscrizioni)
            {
                List<EntryIscrizione> ret = new List<EntryIscrizione>();

                foreach (Iscrizione i in iscrizioni)
                {
                    EntryIscrizione ee = new EntryIscrizione(i);
                    //if (ee.DaContare)
                        ret.Add(ee);
                }

                return ret;
            }
        }

        private void dataGridViewIscritti_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Iscrizione isc = DB.instance.getIscrizione((Guid) dataGridViewIscritti.Rows[e.RowIndex].Cells[0].Value);

            if (isc == null)
                return;

            if (isc.persona != null)
                mainform.OpenAnagrafica(isc.persona);
        }

        private void Event_ChangeWarningColors(object sender, EventArgs e)
        {
            textDescrizione.BackColor = String.IsNullOrEmpty(textDescrizione.Text) ? Color.Red : SystemColors.Control;

            comboListino.BackColor = String.IsNullOrEmpty(comboListino.Text) ? Color.Red : SystemColors.Control;
            
            // maxIscritti
            try
            {
                int iscritti = Int32.Parse(textMaxIscritti.Text);
                textMaxIscritti.BackColor = iscritti > 0 ? SystemColors.Control : Color.Red;
            }
            catch
            {
                textMaxIscritti.BackColor = Color.Red;
            }
        }

        public bool[] getGiornate()
        {
            bool[] retv = new bool[7];

            var src = gridOrari.DataSource as BindingSource;

            if (src != null)
            {
                List<EntryOrario> orari = src.List as List<EntryOrario>;
                if (orari != null)
                    for (int i = 0; i < retv.Length; i++)
                        foreach (EntryOrario eo in orari)
                            if (eo.hasDay(i + 1))
                                retv[i] = true;
            }

            return retv;
        }
    }
}
