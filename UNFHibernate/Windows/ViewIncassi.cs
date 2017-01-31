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
    public partial class ViewIncassi : Form
    {
        public ViewIncassi()
        {
            InitializeComponent();

            dateGiorno.Value = DateTime.Today;

            comboModalitaPagamento.Items.Clear();
            String[] modalitapagamenti = Config.Instance.ModalitaPagamento;
            foreach (String s in modalitapagamenti)
                comboModalitaPagamento.Items.Add(s);
        }

        public static void Show(MainForm m)
        {
            ViewIncassi va = new ViewIncassi();
            va.mainform = m;
            va.RefreshList();
            va.Show();
        }

        public MainForm mainform { get; set; }

        public void RefreshList()
        {
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
            if (master_iscrizioni != null)
                dataGridView1.DataSource = EntryIscrizione.cutList(master_iscrizioni, dateGiorno.Value, comboModalitaPagamento.Text);

            UpdateTotale();
        }

        private void UpdateTotale()
        {
            float totale = 0;

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                float current = 0;

                try { current = float.Parse((String)dataGridView1.Rows[i].Cells["Importo"].Value); }
                catch { }

                totale += current;
            }

            textBox1.Text = totale.ToString("N2");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ApplyCorsiSearchCriteria();
        }

        private void Event_SearchChanged(object sender, EventArgs e)
        {
            ApplyCorsiSearchCriteria();
        }


        // save and quit
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        // print
        private void button4_Click(object sender, EventArgs e)
        {
            Printing.printIncassiGiorno(EntryIscrizione.cutList(master_iscrizioni, dateGiorno.Value, comboModalitaPagamento.Text), dateGiorno.Value);
        }


        public List<EntryIscrizione> master_iscrizioni { get; private set; }

        public class EntryIscrizione
        {
            public Guid ID { get; set; }
            public String Cognome { get; set; }
            public String Nome { get; set; }
            public String Corso { get; set; }
            public DateTime DataPagamento { get; set; }
            public String Importo { get; set; }
            public String Modalita { get; set; }

            public EntryIscrizione()
            {}

            public bool isSelected(DateTime mi, String modalita)
            {
                if (mi != null && !DataPagamento.Date.Equals(mi.Date))
                    return false;

                if (!String.IsNullOrEmpty(modalita) && !Modalita.ToUpper().Contains(modalita))
                    return false;

                return true;
            }

            public static List<EntryIscrizione> createList(List<Iscrizione> iscrizioni)
            {
                List<EntryIscrizione> ret = new List<EntryIscrizione>();

                foreach (Iscrizione i in iscrizioni)
                {
                    if (i.primopagamento_data != null)
                    {
                        EntryIscrizione ei = new EntryIscrizione()
                        {
                            ID = i.ID,
                            Nome = i.persona.Nome,
                            Cognome = i.persona.Cognome,
                            Corso = i.corso.Descrizione,
                            DataPagamento = i.primopagamento_data.Value,
                            Importo = Utils.moneylongToString(i.primopagamento_importo),
                            Modalita = i.primopagamento_modalita
                        };

                        ret.Add(ei);
                    }

                    if (i.secondopagamento_data != null)
                    {
                        EntryIscrizione ei = new EntryIscrizione()
                        {
                            ID = i.ID,
                            Nome = i.persona.Nome,
                            Cognome = i.persona.Cognome,
                            Corso = i.corso.Descrizione,
                            DataPagamento = i.secondopagamento_data.Value,
                            Importo = Utils.moneylongToString(i.secondopagamento_importo),
                            Modalita = i.secondopagamento_modalita
                        };

                        ret.Add(ei);
                    }

                    if (i.terzopagamento_data != null)
                    {
                        EntryIscrizione ei = new EntryIscrizione()
                        {
                            ID = i.ID,
                            Nome = i.persona.Nome,
                            Cognome = i.persona.Cognome,
                            Corso = i.corso.Descrizione,
                            DataPagamento = i.terzopagamento_data.Value,
                            Importo = Utils.moneylongToString(i.terzopagamento_importo),
                            Modalita = i.terzopagamento_modalita
                        };

                        ret.Add(ei);
                    }
                }
                 

                return ret;
            }

            public static List<EntryIscrizione> cutList(List<EntryIscrizione> input, DateTime mi, String modalita)
            {
                List<EntryIscrizione> output = new List<EntryIscrizione>();

                String upmodalita = modalita.Trim().ToUpper();

                foreach (EntryIscrizione i in input)
                    if (i.isSelected(mi, upmodalita))
                        output.Add(i);

                return output;
            }
        }
    }
}
