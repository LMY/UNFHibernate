using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UNFHibernate.Windows
{
    public partial class SelezionaIstruttore : Form
    {
        public SelezionaIstruttore()
        {
            InitializeComponent();
        }

        public static void Show(MainForm m, ViewCorso viewcorso)
        {
            SelezionaIstruttore va = new SelezionaIstruttore();
            va.mainform = m;
            va.vc = viewcorso;
            va.Show();
            va.Populate();
        }

        public MainForm mainform { get; set; }
        public ViewCorso vc { get; set; }


        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try { vc.setNewIstruttore(dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells[0].Value); }
            catch { }
            Close();
        }


        public void Populate()
        {
            BindingSource m_bs = new BindingSource();
            m_bs.DataSource = mainform.master_istruttori;
            m_bs.AllowNew = true;

            dataGridView1.DataSource = m_bs;
            dataGridView1.AutoGenerateColumns = true;

            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].Visible = !(dataGridView1.Columns[i].Name.Equals("ID") || dataGridView1.Columns[i].Name.Equals("Note"));

                if (dataGridView1.Columns[i].Name.Contains("Data"))
                    dataGridView1.Columns[i].DefaultCellStyle.Format = "d";

            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try { vc.setNewIstruttore(dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells[0].Value); }
            catch { }
            Close();
        }
    }
}
