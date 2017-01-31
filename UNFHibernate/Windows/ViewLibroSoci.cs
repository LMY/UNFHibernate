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

namespace UNFHibernate.Windows
{
    public partial class ViewLibroSoci : Form
    {
        public ViewLibroSoci()
        {
            InitializeComponent();

            dateInizio.Value = DateTime.Today.Subtract(TimeSpan.FromDays(7));
            dateFine.Value = DateTime.Today;
        }

        public static void Show(MainForm m)
        {
            ViewLibroSoci va = new ViewLibroSoci();
            va.mainform = m;
            va.RefreshList();
            va.Show();
        }

        public MainForm mainform { get; set; }

        public void RefreshList()
        {
            if (all_fucking_locked)
                return;

            BindingSource m_bs = new BindingSource();
            master_iscrizioni = EntryIscrizione.createList(DB.instance.iscrizioni);
            m_bs.DataSource = master_iscrizioni;
            dataGridView1.DataSource = m_bs;

            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].Visible = !(dataGridView1.Columns[i].Name.Equals("ID") || dataGridView1.Columns[i].Name.Equals("iscrizione"));

                if (dataGridView1.Columns[i].Name.Contains("Data"))
                    dataGridView1.Columns[i].DefaultCellStyle.Format = "d";
            }

            ApplyCorsiSearchCriteria();
        }

        private void ApplyCorsiSearchCriteria()
        {
            if (master_iscrizioni != null && !all_fucking_locked)
                dataGridView1.DataSource = EntryIscrizione.cutList(master_iscrizioni, dateInizio.Value, dateFine.Value, checkBox1.Checked);
        }

        private void Event_SearchChanged(object sender, EventArgs e)
        {
            ApplyCorsiSearchCriteria();
        }

        // save
        private bool all_fucking_locked = false;

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Continuando, la data socio verrà impostata come richiesto.\nProcedere?", "Sei sicuro?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                all_fucking_locked = true;  // non aggiorni più l'anagrafica col search!
                checkBox1.Enabled = false;
                dateInizio.Enabled = false;
                dateFine.Enabled = false;
                button4.Enabled = true;
                Save();
            }
        }

        // save and quit
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        // print
        private void button4_Click(object sender, EventArgs e)
        {
            if (dateInizio.Enabled)
            {
                DialogResult dialogResult = MessageBox.Show("Non hai impostato le date!\nProcedere con la stampa?", "Sei sicuro?", MessageBoxButtons.YesNo);
                if (dialogResult != DialogResult.Yes)
                    return;
            }

            List<Iscrizione> update = new List<Iscrizione>();
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
                try
                {
                    Guid id = (Guid)dataGridView1.Rows[i].Cells["ID"].Value;
                    Iscrizione iscr = DB.instance.getIscrizione(id);

                    update.Add(iscr);
                }
                catch { }

            Printing.printLibroSoci(update, dateTimePicker1.Value, Config.Instance.QuitWordAfterPrintLibroSoci);
        }

        public void Save()
        {
            DateTime settime = dateTimePicker1.Value;
            int tesseran = DB.instance.getFirstCartellinoLibero();

            List<Iscrizione> update = new List<Iscrizione>();

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
                try
                {
                    Guid id = (Guid)dataGridView1.Rows[i].Cells["ID"].Value;
                    Iscrizione iscr = DB.instance.getIscrizione(id);

                    iscr.data_socio = settime;
                    iscr.tesseran = tesseran++;

                    dataGridView1.Rows[i].Cells[4].Value = iscr.tesseran;
                    dataGridView1.Rows[i].Cells[5].Value = settime;

                    update.Add(iscr);
                }
                catch { }

            try
            {
                DB.instance.save(update);
            }
            catch (Exception e)
            {
                MessageBox.Show("Errore assegnando le date socio: " + e.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            RefreshList();
        }


        public List<EntryIscrizione> master_iscrizioni { get; private set; }

        public class EntryIscrizione
        {
            public Guid ID { get; set; }
            public String Cognome { get; set; }
            public String Nome { get; set; }
            public DateTime DataIscrizione { get; set; }
            //public String DataNascita { get; set; }
            //public String LuogoNascita { get; set; }
            //public String CodiceFiscale { get; set; }
            public String tesseran { get; set; }
            public String DataSocio { get; set; }
            public String Corso { get; set; }

            public EntryIscrizione(Iscrizione i)
            {
                ID = i.ID;
                Nome = i.persona.Nome;
                Cognome = i.persona.Cognome;
                DataIscrizione = i.data_iscrizione;
                //DataNascita = Utils.dateToPrintableString(i.persona.DataNascita);
                //LuogoNascita = i.persona.LuogoNascita;
                //CodiceFiscale = i.persona.CodiceFiscale;
                tesseran = i.tesseran > 0 ? i.tesseran.ToString() : string.Empty;
                DataSocio = Utils.dateToPrintableString(i.data_socio);
                Corso = i.corso.Descrizione;
            }

            public bool isSelected(DateTime mi, DateTime max, bool solononsoci)
            {
                if (solononsoci && !String.IsNullOrEmpty(tesseran))
                    return false;

                if (mi != null && max != null && DataIscrizione != null)
                {
                    if (!(DataIscrizione >= mi && DataIscrizione <= max))
                        return false;
                }

                return true;
            }

            public static List<EntryIscrizione> createList(List<Iscrizione> iscrizioni)
            {
                List<EntryIscrizione> ret = new List<EntryIscrizione>();

                foreach (Iscrizione i in iscrizioni)
                    ret.Add(new EntryIscrizione(i));

                return ret;
            }

            public static List<EntryIscrizione> cutList(List<EntryIscrizione> input, DateTime mi, DateTime max, bool solononsoci)
            {
                List<EntryIscrizione> output = new List<EntryIscrizione>();

                foreach (EntryIscrizione i in input)
                    if (i.isSelected(mi, max, solononsoci))
                        output.Add(i);

                return output;
            }
        }
    }
}
