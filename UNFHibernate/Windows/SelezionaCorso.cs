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

namespace UNFHibernate.Windows
{
    public partial class SelezionaCorso : Form
    {
        public SelezionaCorso()
        {
            InitializeComponent();
        }

        public static void Show(MainForm m, ViewAnagrafica viewanag)
        {
            SelezionaCorso va = new SelezionaCorso();
            va.mainform = m;
            va.panel = viewanag;
            va.Show();
            va.Populate();
        }

        public MainForm mainform { get; set; }
        public ViewAnagrafica panel { get; set; }


        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Fire();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Fire();
        }

        public void Populate()
        {
            BindingSource m_bs = new BindingSource();
            m_bs.DataSource = mainform.master_corsi;
            m_bs.AllowNew = true;

            dataGridView1.DataSource = m_bs;
            dataGridView1.AutoGenerateColumns = true;

            dataGridView1.Columns["ID"].Visible = false;

            ApplyCorsiSearchCriteria();
        }

        private void ApplyCorsiSearchCriteria()
        {
            bool attivi = !checkBox1.Checked;
            dataGridView1.DataSource = MainForm.EntryCorso.cutList(mainform.master_corsi, string.Empty, string.Empty, string.Empty, attivi, string.Empty);
        }

        public void Fire()
        {
            try { panel.setNewCorso(dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells[0].Value); }
            catch { }
            Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ApplyCorsiSearchCriteria();
        }
    }
}
