using NHibernate.Criterion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UNFHibernate.Domain;

namespace UNFHibernate
{
    public partial class MainForm : Form
    {
        #region window stuff
        public MainForm()
        {
            InitializeComponent();
            populateGridViews();

            comboCartellini.Items.Clear();
            String[] modalitapagamenti = Config.Instance.Cartellini;
            foreach (String s in modalitapagamenti)
                comboCartellini.Items.Add(s);

            Stagione sc = DB.getStagioneCorrente();
        }

        private void populateGridViews()
        {
            RefreshStagioni();
            RefreshChiusure();
            RefreshListini();
            RefreshIstruttori();
            RefreshAnagrafiche();
            RefreshCorsi();
        }


        public void invalidateGridViews()
        {
            GridAnagrafica.DataSource = null;
            gridChiusure.DataSource = null;
            gridStagioni.DataSource = null;
            gridIstruttori.DataSource = null;
            GridCorsi.DataSource = null;
        }
        #endregion

        #region Anagrafica
        public void RefreshAnagrafiche()
        {
            BindingSource m_bs = new BindingSource();
            master_anagrafica = EntryAnagrafica.createList(DB.instance.persone);
            m_bs.DataSource = master_anagrafica;
            GridAnagrafica.DataSource = m_bs;

            for (int i = 0; i < GridAnagrafica.Columns.Count; i++)
            {
                GridAnagrafica.Columns[i].Visible = !(GridAnagrafica.Columns[i].Name.Equals("ID") || GridAnagrafica.Columns[i].Name.Equals("Note") || GridAnagrafica.Columns[i].Name.Equals("TuttiCorsi"));

                if (GridAnagrafica.Columns[i].Name.Contains("Data"))
                    GridAnagrafica.Columns[i].DefaultCellStyle.Format = "d";
            }

            ApplyAnagraficaSearchCriteria();
        }
        public List<EntryAnagrafica> master_anagrafica { get; private set; }

        public class EntryAnagrafica
        {
            public object ID { get; set; }
            public String Cognome { get; set; }
            public String Nome { get; set; }
            public DateTime? DataNascita { get; set; }
            public DateTime? DataCertificato { get; set; }
            public String Corso { get; set; }
            public String Dovuto { get; set; }
            public String Note { get; set; }    // 6 campo, ricordalo, ti servirà
            public String TuttiCorsi { get; set; }

            public EntryAnagrafica(Persona i)
            {
                ID = i.ID;
                Nome = i.Nome;
                Cognome = i.Cognome;
                DataCertificato = i.DataCertificato;
                DataNascita = i.DataNascita;
                Dovuto = Utils.moneylongToString(i.getDovuto());
                Note = i.Note;
                TuttiCorsi = "";
                
                string corso = "";
                string tutticorsi = "";

                if (i.Iscrizioni.Count > 0)
                {
                    DateTime? last = null;
                    
                    foreach (Iscrizione isc in i.Iscrizioni)
                        if (isc.data_iscrizione != null)
                            if (last == null || isc.data_iscrizione > last)
                            {
                                last = isc.data_iscrizione;
                                corso = isc.corso.Descrizione;
                                if (tutticorsi != "")
                                    tutticorsi += ',';
                                tutticorsi += corso;
                            }
                }

                Corso = corso;
                TuttiCorsi = tutticorsi.ToUpper();
            }

            public bool isSelected(DateTime? mi, DateTime? max, String nname, String ccognome, String corso, bool solo_nonsaldati)
            {
                if (!String.IsNullOrWhiteSpace(corso) && !TuttiCorsi.Contains(corso))
                    return false;

                if (solo_nonsaldati && String.IsNullOrEmpty(Dovuto))
                    return false;

                if (mi != null && max != null && DataNascita != null)
                {
                    if (!(DataNascita >= mi && DataNascita <= max))
                        return false;
                }

                
                if (String.IsNullOrWhiteSpace(ccognome)) // ricerca di solo nome
                {
                    if (!String.IsNullOrWhiteSpace(nname) && !Nome.ToUpper().Contains(nname) && !Cognome.ToUpper().Contains(nname))
                        return false;
                }
                else // ricerca nome e cognome
                {
                    if (!String.IsNullOrWhiteSpace(nname) && !Nome.ToUpper().Contains(nname) && !Cognome.ToUpper().Contains(nname))
                        return false;

                    if (!String.IsNullOrWhiteSpace(ccognome) && !Nome.ToUpper().Contains(ccognome) && !Cognome.ToUpper().Contains(ccognome))
                        return false;
                }



                return true;
            }

            public static List<EntryAnagrafica> createList(List<Persona> iscrizioni)
            {
                List<EntryAnagrafica> ret = new List<EntryAnagrafica>();

                foreach (Persona i in iscrizioni)
                    ret.Add(new EntryAnagrafica(i));

                return ret;
            }

            public static List<EntryAnagrafica> cutList(List<EntryAnagrafica> input, DateTime? mi, DateTime? max, String nname, String coggg, String corso, bool solo_nonsaldati)
            {
                List<EntryAnagrafica> output = new List<EntryAnagrafica>();

                foreach (EntryAnagrafica i in input)
                    if (i.isSelected(mi, max, nname, coggg, corso, solo_nonsaldati))
                        output.Add(i);

                return output;
            }
        }

        // reset search
        private void ButtonResetSearch_Click(object sender, EventArgs e)
        {
            checkBox2.Checked = false;
            TextboxSearchName.Text = String.Empty;
            TextboxSearchCorso.Text = String.Empty;
            TextBoxSearchMonth.Text = String.Empty;
            TextBoxSearchYear.Text = String.Empty;
        }

        // on tooltip
        private void GridAnagrafica_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //return;
            if (e.ColumnIndex == GridAnagrafica.Columns["Nome"].Index || e.ColumnIndex == GridAnagrafica.Columns["Cognome"].Index)   // 1 = "Nome"
            {
                var cell = GridAnagrafica.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (cell != null)
                try
                {
                    object val = GridAnagrafica.Rows[e.RowIndex].Cells["Note"].Value;    // 6 = "Note"
                    if (val != null)
                        cell.ToolTipText = val.ToString();
                }
                catch (Exception exc) { Log.Instance.WriteLine(Log.LogLevel.Warning, "GridAnagrafica_CellFormatting::" + exc.Message); }

                DataGridViewRow row = GridAnagrafica.Rows[e.RowIndex];
                String cellDovuto = row.Cells["Dovuto"].Value.ToString();
                row.DefaultCellStyle.ForeColor = String.IsNullOrEmpty(cellDovuto) ? Color.Black : Color.Red;
            }
        }

        // double click
        private void GridAnagrafica_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                object id = GridAnagrafica.Rows[e.RowIndex].Cells[0].Value;

                if (id != null)
                    OpenAnagrafica(DB.instance.getPersona((Guid)id));
            }
            catch (Exception exc) { Log.Instance.WriteLine(Log.LogLevel.Warning, "GridAnagrafica_CellDoubleClick::" + exc.Message); }
        }


        void anagraficaTimer_Tick(object sender, EventArgs e)
        {
            anagraficaTimer.Stop();
            anagraficaTimer = null;
            ApplyAnagraficaSearchCriteria();
        }

        private void ApplyAnagraficaSearchCriteria()
        {
            if (Config.Instance.ShowColumnDovuto)
                GridAnagrafica.Columns["Dovuto"].Visible = checkBox2.Checked;

            if (String.IsNullOrEmpty(TextboxSearchName.Text) && String.IsNullOrEmpty(TextBoxSearchMonth.Text) && String.IsNullOrEmpty(TextBoxSearchYear.Text) && String.IsNullOrEmpty(TextboxSearchCorso.Text) && !checkBox2.Checked)
            {
                GridAnagrafica.DataSource = master_anagrafica;
                return;
            }

            DateTime? mindt = null;
            DateTime? maxdt = null;


            if (TextBoxSearchYear.Text.Length > 0 && TextBoxSearchMonth.Text.Length > 0)    // mese e anno
            {
                int year = Int32.Parse(TextBoxSearchYear.Text);

                if (year < 100 && year > 50)
                    year = 1900 + year;
                else if (year < 100)
                    year = 2000 + year;

                mindt = new DateTime(year, Int32.Parse(TextBoxSearchMonth.Text), 1);
                maxdt = mindt.Value.AddMonths(1);
            }
            else if (TextBoxSearchYear.Text.Length > 0) // solo anno
            {
                int year = Int32.Parse(TextBoxSearchYear.Text);

                if (year < 100 && year > 50)
                    year = 1900 + year;
                else if (year < 100)
                    year = 2000 + year;

                mindt = new DateTime(year, 1, 1);
                maxdt = mindt.Value.AddYears(1);
            }


            String nome = String.Empty;
            String cognome = String.Empty;
            String searchcorso = TextboxSearchCorso.Text.Trim();

            if (TextboxSearchName.Text.Length > 0)
            {
                String searchstr = TextboxSearchName.Text.Trim();

                if (searchstr.Contains(' '))
                {
                    nome = searchstr.Substring(0, searchstr.IndexOf(' '));
                    cognome = searchstr.Substring(searchstr.IndexOf(' ') + 1);
                }
                else
                {
                    nome = searchstr;
                    cognome = "";
                }
            }

            GridAnagrafica.DataSource = EntryAnagrafica.cutList(master_anagrafica, mindt, maxdt, nome.ToUpper(), cognome.ToUpper(), searchcorso.ToUpper(), checkBox2.Checked);
        }


        private Timer anagraficaTimer = null;

        private void AnagraficaSearchChanged()
        {
            // per fare una ricerca solo dopo che l'utente non fa modifiche da almeno $anagraficaTimerDefault
            if (anagraficaTimer == null)
            {
                anagraficaTimer = new Timer();
                anagraficaTimer.Interval = Config.Instance.RefreshAnagrafica;
                anagraficaTimer.Tick += anagraficaTimer_Tick;
                anagraficaTimer.Start();
            }
            else
            {
                anagraficaTimer.Stop();
                anagraficaTimer.Start();
            }
        }

        private void Event_AnagraficaSearchChanged(object sender, EventArgs e)
        {
            AnagraficaSearchChanged();
        }


        private void ButtonInsertNewPerson_Click_1(object sender, EventArgs e)
        {
            OpenAnagrafica(new Persona());
        }


        public void OpenAnagrafica(Persona p)
        {
            ViewAnagrafica.Show(this, p);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            RefreshAnagrafiche();
        }

        // remove persona
        private void button20_Click(object sender, EventArgs e)
        {
            int selected_row = GridAnagrafica.SelectedRows[0].Index;
            Persona p = DB.instance.getPersona((Guid)GridAnagrafica.Rows[selected_row].Cells[0].Value);

            if (p.Iscrizioni.Count > 0)
            {
                MessageBox.Show("Impossibile rimuovere: la persona ha delle iscrizioni!", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult dialogResult = MessageBox.Show("Sei sicuro di voler eliminare la persona " + p.Nome + " " + p.Cognome + "?", "Sei sicuro?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                DB.instance.removePersona(p);
                RefreshAnagrafiche();
            }
        }

#endregion

        #region Corsi

        public void RefreshCorsi()
        {
            master_corsi = EntryCorso.createList(DB.instance.corsi);

            String nome = TextboxSearchCorsoNome.Text.Trim().ToUpper();
            String codice = TextboxSearchCorsoCodice.Text.Trim().ToUpper();
            String cartellino = comboCartellini.Text.Trim().ToUpper();
            String stagioneCercata = comboStagioni.Text.Trim().ToUpper();
            bool attivi = checkSoloCorsiAttivi.Checked;

            GridCorsi.DataSource = EntryCorso.cutList(master_corsi, nome, codice, cartellino, attivi, stagioneCercata);
            GridCorsi.AutoGenerateColumns = true;
            GridCorsi.Columns["ID"].Visible = false;

            ApplyCorsiSearchCriteria();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TextboxSearchCorsoNome.Text = string.Empty;
            TextboxSearchCorsoCodice.Text = string.Empty;
            comboCartellini.Text = string.Empty;
            checkSoloCorsiAttivi.Checked = true;
        }

        void corsiTimer_Tick(object sender, EventArgs e)
        {
            corsiTimer.Stop();
            corsiTimer = null;
            
            ApplyCorsiSearchCriteria();
        }

        private void ApplyCorsiSearchCriteria()
        {
            String nome =TextboxSearchCorsoNome.Text.Trim().ToUpper();
            String codice = TextboxSearchCorsoCodice.Text.Trim().ToUpper();
            String cartellino = comboCartellini.Text.Trim().ToUpper();
            bool attivi = checkSoloCorsiAttivi.Checked;

            String stagionecercata = null;

            foreach (Stagione s in DB.instance.stagioni)
                if (s.Descrizione.Equals(comboStagioni.Text))
                {
                    stagionecercata = s.Descrizione;
                    break;
                }

            GridCorsi.DataSource = EntryCorso.cutList(master_corsi, nome, codice, cartellino, attivi, stagionecercata);
        }

        public List<EntryCorso> master_corsi { get; private set; }
        public class EntryCorso
        {
            public object ID { get; set; }
            public String Codice { get; set; }
            public String Nome { get; set; }
            public String Cartellino { get; set; }
            public DateTime? DataInizio { get; set; }
            public DateTime? DataFine { get; set; }
            public String Orario { get; set; }
            public bool Attivo { get; set; }

            public String Stagione { get; set; }

            public EntryCorso(Corso i)
            {
                ID = i.ID;
                Codice = i.Codice ?? String.Empty;
                Nome = i.Descrizione ?? String.Empty;
                Cartellino = i.TipoCartellino ?? String.Empty;
                DataInizio = i.DataInizio;
                DataFine = i.DataFine;
                Orario = i.Orario;
                Attivo = i.Attivo!=null ? i.Attivo.Equals("S") : false;
                Stagione = i.stagione != null ? i.stagione.Descrizione : string.Empty;
            }

            public bool isSelected(String nome, String codice, String cartellino, bool attivo, String stagionecercata)
            {
                if (!String.IsNullOrWhiteSpace(stagionecercata) && !Stagione.ToUpper().Contains(stagionecercata))
                    return false;
                if (!String.IsNullOrWhiteSpace(nome) && !Nome.ToUpper().Contains(nome))
                    return false;
                if (!String.IsNullOrWhiteSpace(codice) && !Codice.ToUpper().Contains(codice))
                    return false;
                if (!String.IsNullOrWhiteSpace(cartellino) && !Cartellino.ToUpper().Contains(cartellino))
                    return false;

                if (attivo && !Attivo)
                    return false;

                return true;
            }

            public static List<EntryCorso> createList(List<Corso> corsi)
            {
                List<EntryCorso> ret = new List<EntryCorso>();

                foreach (Corso i in corsi)
                    ret.Add(new EntryCorso(i));

                return ret;
            }

            public static List<EntryCorso> cutList(List<EntryCorso> input, String nome, String codice, String cartellino, bool attivo, String stagionecercata)
            {
                List<EntryCorso> output = new List<EntryCorso>();

                foreach (EntryCorso i in input)
                    if (i.isSelected(nome, codice, cartellino, attivo, stagionecercata))
                        output.Add(i);

                return output;
            }
        }

        private Timer corsiTimer = null;

        private void CorsiSearchChanged()
        {
            // per fare una ricerca solo dopo che l'utente non fa modifiche da almeno $anagraficaTimerDefault
            if (corsiTimer == null)
            {
                corsiTimer = new Timer();
                corsiTimer.Interval = Config.Instance.RefreshAltre;
                corsiTimer.Tick += corsiTimer_Tick;
                corsiTimer.Start();
            }
            else
            {
                corsiTimer.Stop();
                corsiTimer.Start();
            }
        }

        private void Event_CorsiSearchChanged(object sender, EventArgs e)
        {
            CorsiSearchChanged();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (DB.instance.stagione_corrente == null)
            {
                MessageBox.Show("Impostare una stagione prima di creare dei corsi!", "ERRORE");
                return;
            }

            OpenCorso(new Corso { stagione = DB.instance.stagione_corrente, Attivo = Utils.AttivoTrue });
        }

        private void GridCorsi_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            OpenCorso(DB.instance.getCorso((Guid)GridCorsi.Rows[e.RowIndex].Cells[0].Value));
        }

        public void OpenCorso(Corso c)
        {
            ViewCorso.Show(this, c);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            RefreshCorsi();
        }

        // remove corso
        private void button18_Click(object sender, EventArgs e)
        {
            int selected_row = GridCorsi.SelectedRows[0].Index;
            Corso c = DB.instance.getCorso((Guid)GridCorsi.Rows[selected_row].Cells[0].Value);

            if (c.Istruttori.Count > 0)
            {
                MessageBox.Show("Impossibile rimuovere: il corso ha degli istruttori associati.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (c.Iscrizioni.Count > 0)
            {
                MessageBox.Show("Impossibile rimuovere: il corso ha degli iscritti!", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult dialogResult = MessageBox.Show("Sei sicuro di voler eliminare il corso " + c.Descrizione + "?", "Sei sicuro?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                DB.instance.removeCorso(c);
                RefreshCorsi();
            }
        }

#endregion

        #region Chiusure
        private static readonly String[] validChiusureColumn = new String[] { "Descrizione", "DataInizio", "DataFine" };
        private static readonly String[] validListiniColumn = new String[] { "Descrizione", "descrizione" };

        public void RefreshChiusure()
        {
            Stagione s = DB.instance.getStagioneByName(comboStagioniChiusureEListini.Text);

            List<Chiusura> lc = s == null ? DB.instance.chiusure : DB.instance.chiusure.Where(x => x.stagione == s).ToList<Chiusura>();
            if (lc != null)
            {
                BindingSource m_bs = new BindingSource();
                m_bs.DataSource = lc;
                gridChiusure.DataSource = m_bs;
                gridChiusure.AutoGenerateColumns = true;

                if (validChiusureColumn.Length > 0)
                    for (int i = 0; i < gridChiusure.Columns.Count; i++)
                        gridChiusure.Columns[i].Visible = validChiusureColumn.Contains(gridChiusure.Columns[i].Name);
            }
        }

        public void RefreshListini()
        {
            List<ListinoCorsi> ilistini = DB.instance.listini;
            if (ilistini != null)
            {
                BindingSource m_bs = new BindingSource();
                m_bs.DataSource = ilistini;
                gridListini.DataSource = m_bs;
                gridListini.AutoGenerateColumns = true;

                if (validListiniColumn.Length > 0)
                    for (int i = 0; i < gridListini.Columns.Count; i++)
                        gridListini.Columns[i].Visible = validListiniColumn.Contains(gridListini.Columns[i].Name);
            }
        }

        private void buttonNewChiusura_Click(object sender, EventArgs e)
        {
            if (DB.instance.stagione_corrente == null)
            {
                MessageBox.Show("Impostare una stagione prima di creare delle chiusure!", "ERRORE");
                return;
            }

            Stagione stag = null;
            try {  stag = DB.instance.getStagioneByName(comboStagioniChiusureEListini.Text); }
            catch { stag = DB.instance.stagione_corrente; }

            OpenChiusura(new Chiusura { stagione = stag });
        }

        public void OpenChiusura(Chiusura c)
        {
            ViewChiusura.Show(this, c);
        }

        private void gridChiusure_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //OpenChiusura(DB.getChiusura(gridChiusure.Rows[e.RowIndex].Cells[0].Value));
            OpenChiusura(DB.instance.getChiusura((Guid) gridChiusure.Rows[e.RowIndex].Cells[0].Value));
        }

        private void buttonRemoveChiusura(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Sei sicuro di voler eliminare la chiusura?", "Sei sicuro?", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                int selected_row = gridChiusure.SelectedRows[0].Index;
                Chiusura c = DB.instance.getChiusura((Guid)gridChiusure.Rows[selected_row].Cells[0].Value);
                DB.instance.removeChiusura(c);
                RefreshChiusure();
            }
        }
#endregion

        #region Stagioni
        private static readonly String[] validStagioniColumn = new String[] { "Descrizione", "DataInizio", "DataFine" };

        public void RefreshStagioni()
        {
            BindingSource m_bs = new BindingSource();
            m_bs.DataSource = DB.instance.stagioni;

            comboStagioni.Items.Clear();
            comboStagioni.Items.Add(String.Empty);
            List<Stagione> stagioni = DB.instance.stagioni;
            foreach (Stagione s in stagioni)
                comboStagioni.Items.Add(s.Descrizione);

            comboStagioniChiusureEListini.Items.Clear();
            foreach (Stagione s in stagioni)
                comboStagioniChiusureEListini.Items.Add(s.Descrizione);

            if (DB.instance.stagione_corrente != null)
            {
                comboStagioni.Text = DB.instance.stagione_corrente.Descrizione;
                comboStagioniChiusureEListini.Text = DB.instance.stagione_corrente.Descrizione;
            }

            gridStagioni.DataSource = m_bs;
            gridStagioni.AutoGenerateColumns = true;

            if (validStagioniColumn.Length > 0)
                for (int i = 0; i < gridStagioni.Columns.Count; i++)
                    gridStagioni.Columns[i].Visible = validStagioniColumn.Contains(gridStagioni.Columns[i].Name);
        }

        private void buttonNewStagione_Click(object sender, EventArgs e)
        {
            OpenStagione(new Stagione());
        }

        public void OpenStagione(Stagione s)
        {
            ViewStagione.Show(this, s);
        }

        private void gridStagioni_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            OpenStagione(DB.instance.getStagione((Guid)gridStagioni.Rows[e.RowIndex].Cells[0].Value));
        }

        // remove stagione
        private void button17_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Sei sicuro di voler eliminare la stagione?", "Sei sicuro?", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                int selected_row = gridStagioni.SelectedRows[0].Index;
                Stagione s = DB.instance.getStagione((Guid)gridStagioni.Rows[selected_row].Cells[0].Value);

                if (DB.instance.isStagioneCorrente(s))
                {
                    DialogResult dialogResult2 = MessageBox.Show("Stai cancellando la stagione corrente!\nSei veramente sicuro di volerlo fare?", "Sei VERAMENTE sicuro?", MessageBoxButtons.YesNo);
                    if (dialogResult2 != DialogResult.Yes)
                        return;
                }

                DB.instance.removeStagione(s);
                RefreshStagioni();
            }
        }
#endregion

        #region Istruttori

        public void RefreshIstruttori()
        {
            BindingSource m_bs = new BindingSource();
            master_istruttori = EntryIstruttore.createList(DB.instance.istruttori);
            m_bs.DataSource = master_istruttori;

            gridIstruttori.DataSource = m_bs;
            gridIstruttori.AutoGenerateColumns = true;

            for (int i = 0; i < gridIstruttori.Columns.Count; i++)
            {
                gridIstruttori.Columns[i].Visible = !(gridIstruttori.Columns[i].Name.Equals("ID") || gridIstruttori.Columns[i].Name.Equals("Note"));

                if (gridIstruttori.Columns[i].Name.Contains("Data"))
                    gridIstruttori.Columns[i].DefaultCellStyle.Format = "d";

            }

            ApplyIstruttoriSearchCriteria();
        }

        public List<EntryIstruttore> master_istruttori { get; private set; }

        public class EntryIstruttore
        {
            public object ID { get; set; }
            public String Cognome { get; set; }
            public String Nome { get; set; }
            public String NumeroCellulare { get; set; }
            public String NumeroTelefono { get; set; }

            public EntryIstruttore(Istruttore i)
            {
                ID = i.ID;
                Nome = i.Nome;
                Cognome = i.Cognome;
                NumeroCellulare = i.NumeroCellulare;
                NumeroTelefono = i.NumeroTelefono;
            }

            public bool isSelected(String nname, String ccognome, bool solo_attivi)
            {
                //if (solo_attivi && String.IsNullOrEmpty(Attivo))
                //    return false;


                if (String.IsNullOrWhiteSpace(ccognome)) // ricerca di solo nome
                {
                    if (!String.IsNullOrWhiteSpace(nname) && !Nome.ToUpper().Contains(nname) && !Cognome.ToUpper().Contains(nname))
                        return false;
                }
                else // ricerca nome e cognome
                {
                    if (!String.IsNullOrWhiteSpace(nname) && !Nome.ToUpper().Contains(nname) && !Cognome.ToUpper().Contains(nname))
                        return false;

                    if (!String.IsNullOrWhiteSpace(ccognome) && !Nome.ToUpper().Contains(ccognome) && !Cognome.ToUpper().Contains(ccognome))
                        return false;
                }

                return true;
            }

            public static List<EntryIstruttore> createList(List<Istruttore> iscrizioni)
            {
                List<EntryIstruttore> ret = new List<EntryIstruttore>();

                foreach (Istruttore i in iscrizioni)
                    ret.Add(new EntryIstruttore(i));

                return ret;
            }

            public static List<EntryIstruttore> cutList(List<EntryIstruttore> input, String nname, String ccognome, bool solo_attivi)
            {
                List<EntryIstruttore> output = new List<EntryIstruttore>();

                foreach (EntryIstruttore i in input)
                    if (i.isSelected(nname, ccognome, solo_attivi))
                        output.Add(i);

                return output;
            }
        }

        void istruttoriTimer_Tick(object sender, EventArgs e)
        {
            istruttoriTimer.Stop();
            istruttoriTimer = null;

            ApplyIstruttoriSearchCriteria();
        }

        private void ApplyIstruttoriSearchCriteria()
        {
            String nome = String.Empty;
            String cognome = String.Empty;
            String searchcorso = TextboxSearchCorso.Text.Trim();
            bool attivi = false;

            if (textBoxSearchIstruttore.Text.Length > 0)
            {
                String searchstr = textBoxSearchIstruttore.Text.Trim();

                if (searchstr.Contains(' '))
                {
                    nome = searchstr.Substring(0, searchstr.IndexOf(' '));
                    cognome = searchstr.Substring(searchstr.IndexOf(' ') + 1);
                }
                else
                {
                    nome = searchstr;
                    cognome = "";
                }
            }

            gridIstruttori.DataSource = EntryIstruttore.cutList(master_istruttori, nome.ToUpper(), cognome.ToUpper(), attivi);
        }


        private Timer istruttoriTimer = null;

        private void IstruttoriSearchChanged()
        {
            // per fare una ricerca solo dopo che l'utente non fa modifiche da almeno $anagraficaTimerDefault
            if (istruttoriTimer == null)
            {
                istruttoriTimer = new Timer();
                istruttoriTimer.Interval = Config.Instance.RefreshAltre;
                istruttoriTimer.Tick += istruttoriTimer_Tick;
                istruttoriTimer.Start();
            }
            else
            {
                istruttoriTimer.Stop();
                istruttoriTimer.Start();
            }
        }

        private void Event_IstruttoriSearchChanged(object sender, EventArgs e)
        {
            IstruttoriSearchChanged();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            OpenIstruttore(new Istruttore());
        }

        public void OpenIstruttore(Istruttore i)
        {
            ViewIstruttore.Show(this, i);
        }

        private void gridIstruttori_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            OpenIstruttore(DB.instance.getIstruttore((Guid)gridIstruttori.Rows[e.RowIndex].Cells[0].Value));
        }

        private void button8_Click(object sender, EventArgs e)
        {
            RefreshIstruttori();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBoxSearchIstruttore.Text = string.Empty;
        }

        // remove istruttore
        private void button19_Click(object sender, EventArgs e)
        {
            int selected_row = gridIstruttori.SelectedRows[0].Index;
            Istruttore i = DB.instance.getIstruttore((Guid)gridIstruttori.Rows[selected_row].Cells[0].Value);

            if (i.Corsi.Count > 0)
            {
                MessageBox.Show("Impossibile rimuovere: l'istruttore ha dei corsi associati", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult dialogResult = MessageBox.Show("Sei sicuro di voler eliminare l'istruttore " + i.Nome+" "+i.Cognome + "?", "Sei sicuro?", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                DB.instance.removeIstruttore(i);
                RefreshIstruttori();
            }
        }



        #endregion

        #region Importer
        private void buttonImportChiusure(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                DBImporter.importChiusure(openFileDialog1.FileName, null);
                RefreshChiusure();
                MessageBox.Show("Fatto!");
            }
        }

        private void buttonImportStagioni(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                DBImporter.importStagioni(openFileDialog1.FileName, null);
                RefreshStagioni();
                MessageBox.Show("Fatto!");
            }
        }

        private void buttonImportAnagrafiche(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                DBImporter.importPersone(openFileDialog1.FileName, null);
                RefreshAnagrafiche();
                MessageBox.Show("Fatto!");
            }
        }

        private void buttonImportCorsi(object sender, EventArgs e)
        {
            String titlestring = string.Empty;  // fallirà
            String assorari = string.Empty;  // fallirà

            OpenFileDialog titleDialog = new OpenFileDialog();
            titleDialog.Title = "Selezionare il file degli orari";
            if (titleDialog.ShowDialog() == DialogResult.OK)
                titlestring = titleDialog.FileName;

            OpenFileDialog assorariDialog = new OpenFileDialog();
            assorariDialog.Title = "Selezionare il file delle associazioni orari<->corsi";
            if (assorariDialog.ShowDialog() == DialogResult.OK)
                assorari = assorariDialog.FileName;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Selezionare il file dei corsi";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                DBImporter.importCorsi(openFileDialog1.FileName, titlestring, assorari, null);
                RefreshCorsi();
                MessageBox.Show("Fatto!");
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            DB.instance.Refresh();
            populateGridViews();
            sw.Stop();
            
            if (Config.Instance.ShowRefreshTime)
                MessageBox.Show("Dati ricaricati (" + sw.Elapsed.TotalSeconds.ToString("N2") + " secs)", "Ok");
        }

        // importa iscrizioni
        private void buttonImportIscrizioni(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                DBImporter.importIscrizioni(openFileDialog1.FileName, null);
                RefreshCorsi();
                RefreshAnagrafiche();
            }
        }



        private void buttonSaveAnagraficheCSV(object sender, EventArgs e)
        {
            // to excel
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.Filter = "CSV (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";
            savefile.FileName = "anagrafica.csv";

            if (savefile.ShowDialog() == DialogResult.OK)
                Utils.anagraficheToCsv(savefile.FileName, GridAnagrafica);
        }
        #endregion

        #region Backup
        private String popupOpenFilenameBox()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                return openFileDialog1.FileName;

            return string.Empty;
        }

        private String popupSaveFilenameBox()
        {
            SaveFileDialog openFileDialog1 = new SaveFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                return openFileDialog1.FileName;

            return string.Empty;
        }

        private void buttonBackupChiusure(object sender, EventArgs e)
        {
            String filename = popupSaveFilenameBox();
            if (!String.IsNullOrEmpty(filename))
            {
                DBImporter.backupChiusure(DB.instance.chiusure, filename);
                MessageBox.Show("Fatto!");
            }
        }

        private void buttonBackupStagioni(object sender, EventArgs e)
        {
            String filename = popupSaveFilenameBox();
            if (!String.IsNullOrEmpty(filename))
            {
                DBImporter.backupStagioni(DB.instance.stagioni, filename);
                MessageBox.Show("Fatto!");
            }
        }

        private void buttonBackupIstruttori(object sender, EventArgs e)
        {
            String filename = popupSaveFilenameBox();
            if (!String.IsNullOrEmpty(filename))
            {
                DBImporter.backupIstruttori(DB.instance.istruttori, filename);
                MessageBox.Show("Fatto!");
            }
        }

        private void buttonBackupCorsi(object sender, EventArgs e)
        {
            String filename = popupSaveFilenameBox();
            if (!String.IsNullOrEmpty(filename))
            {
                DBImporter.backupCorsi(DB.instance.corsi, filename);
                MessageBox.Show("Fatto!");
            }
        }

        private void buttonBackupAnagrafiche(object sender, EventArgs e)
        {
            String filename = popupSaveFilenameBox();
            if (!String.IsNullOrEmpty(filename))
            {
                DBImporter.backupPersone(DB.instance.persone, filename);
                MessageBox.Show("Fatto!");
            }
        }

        private void buttonBackupALL(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                DBImporter.backupAll(dialog.SelectedPath);
                MessageBox.Show("Fatto!");
            }
        }

        private void buttonBackupIscrizioni(object sender, EventArgs e)
        {
            String filename = popupSaveFilenameBox();
            if (!String.IsNullOrEmpty(filename))
            {
                DBImporter.backupIscrizioni(DB.instance.iscrizioni, filename);
                MessageBox.Show("Fatto!");
            }
        }

        private void buttonImportALL(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                //invalidateGridViews();

                DBImporter.importAll(dialog.SelectedPath, this);
                populateGridViews();

                MessageBox.Show("Fatto!");
            }
        }

        private void buttonRestoreALL(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                invalidateGridViews();

                DBImporter.restoreAll(dialog.SelectedPath, this);
                populateGridViews();

                MessageBox.Show("Fatto!");
            }
        }

        private void buttonRestoreChiusure(object sender, EventArgs e)
        {
            String filename = popupOpenFilenameBox();
            if (!String.IsNullOrEmpty(filename))
            {
                DBImporter.restoreChiusure(filename);
                RefreshChiusure();

                MessageBox.Show("Fatto!");
            }
        }

        private void buttonRestoreStagioni(object sender, EventArgs e)
        {
            String filename = popupOpenFilenameBox();
            if (!String.IsNullOrEmpty(filename))
            {
                DBImporter.restoreStagioni(filename);
                RefreshStagioni();

                MessageBox.Show("Fatto!");
            }
        }

        private void buttonRestoreIstruttori(object sender, EventArgs e)
        {
            String filename = popupOpenFilenameBox();
            if (!String.IsNullOrEmpty(filename))
            {
                DBImporter.restoreIstruttori(filename);
                RefreshIstruttori();

                MessageBox.Show("Fatto!");
            }
        }

        private void buttonRestoreCorsi(object sender, EventArgs e)
        {
            String filename = popupOpenFilenameBox();
            if (!String.IsNullOrEmpty(filename))
            {
                DBImporter.restoreCorsi(filename);
                RefreshCorsi();

                MessageBox.Show("Fatto!");
            }
        }

        private void buttonRestoreAnagrafiche(object sender, EventArgs e)
        {
            String filename = popupOpenFilenameBox();
            if (!String.IsNullOrEmpty(filename))
            {
                DBImporter.restorePersone(filename);
                RefreshAnagrafiche();

                MessageBox.Show("Fatto!");
            }
        }

        private void buttonRestoreIscrizioni(object sender, EventArgs e)
        {
            String filename = popupOpenFilenameBox();
            if (!String.IsNullOrEmpty(filename))
            {
                DBImporter.restoreIscrizioni(filename);
                RefreshAnagrafiche();
                RefreshCorsi();

                MessageBox.Show("Fatto!");
            }
        }
        #endregion

        private void buttonStampaLibroCorsi(object sender, EventArgs e)
        {
            UNFHibernate.Windows.ViewLibroSoci.Show(this);
        }

        private void buttonStampaIncassiGiorno(object sender, EventArgs e)
        {
            UNFHibernate.Windows.ViewIncassi.Show(this);
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Config.Instance.promptBackupOnExit)
            {
                DialogResult dialogResult = MessageBox.Show("Vuoi eseguire un backup prima di uscire?", "Backup?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                    buttonBackupALL(null, null);
            }

            if (Config.Instance.writeLogOnExit)
                Log.Instance.WriteLine(Log.LogLevel.Always, "Program Terminated");
        }

        private void comboStagioniChiusureEListini_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshChiusure();
        }

        private void ButtonResetSearch_Click(object sender, MouseEventArgs e)
        {

        }

        private void buttonListinoAdd_Click(object sender, EventArgs e)
        {
            OpenListino(new ListinoCorsi());
        }

        public void OpenListino(ListinoCorsi c)
        {
            UNFHibernate.Windows.ViewListino.Show(this, c);
        }

        private void buttonListinoDelete_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Sei sicuro di voler eliminare il listino?", "Sei sicuro?", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                int selected_row = gridListini.SelectedRows[0].Index;
                ListinoCorsi c = DB.instance.getListino((Guid)gridListini.Rows[selected_row].Cells[0].Value);
                DB.instance.removeListino(c);
                RefreshListini();
            }
        }

        private void gridListini_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            OpenListino(DB.instance.getListino((Guid)gridListini.Rows[e.RowIndex].Cells[0].Value));
        }

        private void button_DeleteALLDB(object sender, EventArgs e)
        {
            DBImporter.deleteAllAndClear(this);
        }

        private void button_BackupListini(object sender, EventArgs e)
        {
            String filename = popupSaveFilenameBox();
            if (!String.IsNullOrEmpty(filename))
            {
                DBImporter.backupListini(DB.instance.listini, filename);
                MessageBox.Show("Fatto!");
            }
        }

        private void button_RestoreListini(object sender, EventArgs e)
        {
            String filename = popupOpenFilenameBox();
            if (!String.IsNullOrEmpty(filename))
            {
                DBImporter.restoreListini(filename);
                RefreshChiusure();

                MessageBox.Show("Fatto!");
            }
        }

        private void buttonEliminaPersoneDuplicate_Click(object sender, EventArgs e)
        {
            int rimossi = DB.instance.removePersoneDuplicate();
            if (rimossi > 0)
            {
                MessageBox.Show("Rimossi: " + rimossi);
                RefreshAnagrafiche();
            }
        }
    }
}
