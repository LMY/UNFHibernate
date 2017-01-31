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
    public partial class ViewListino : Form
    {
        public ViewListino()
        {
            InitializeComponent();
        }

        public static void Show(MainForm m, UNFHibernate.Domain.ListinoCorsi s)
        {
            ViewListino va = new ViewListino();
            va.setListino(s);
            va.mainform = m;
            va.Show();
        }

        public MainForm mainform { get; set; }
        private UNFHibernate.Domain.ListinoCorsi listino;

        public void setListino(UNFHibernate.Domain.ListinoCorsi s)
        {
            listino = s;

            if (listino != null)
            {
                textBox1.Text = listino.descrizione;
                UpdateGrid();
            }
        }

        private List<EntryListino> lista_listino;

        private void UpdateGrid()
        {
            if (listino == null)
                return;

            BindingSource m_bs = new BindingSource();
            lista_listino = EntryListino.getListinoList(listino.ingressi);
            m_bs.DataSource = lista_listino;

            dataGridView1.DataSource = m_bs;
            dataGridView1.AutoGenerateColumns = true;
        }

        public void UpdateData()
        {
            if (listino == null)
                listino = new UNFHibernate.Domain.ListinoCorsi();

            try
            {
                listino.ingressi = EntryListino.getListinoString(lista_listino);
            }
            catch { listino.ingressi = string.Empty; }
            listino.descrizione = textBox1.Text;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (Save())
                if (mainform != null)
                    mainform.RefreshListini();
        }

        private void buttonSaveExit_Click(object sender, EventArgs e)
        {
            if (Save())
            {
                if (mainform != null)
                    mainform.RefreshListini();
                Close();
            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        public bool Save()
        {
            UpdateData();

            if (DB.instance.save(listino))
            {
                mainform.RefreshStagioni();
                mainform.RefreshCorsi();
                return true;
            }
            return false;
        }

        public class EntryListino
        {
            public int Numero { get; set; }
            public String Prezzo { get; set; }

            public EntryListino()
            {
                Numero = 0;
                Prezzo = string.Empty;
            }

            public EntryListino(String s)
            {
                Numero = 0;
                Prezzo = string.Empty;

                try
                {
                    String[] entries = s.Split('/');
                    Numero = int.Parse(entries[0]);
                    Prezzo = Utils.moneylongToString(long.Parse(entries[1]));
                }
                catch { }
            }

            public override String ToString()
            {
                return Numero.ToString() + '/' + Utils.moneystringToLong(Prezzo);
            }

            internal static List<EntryListino> getListinoList(string p)
            {
                List<EntryListino> r = new List<EntryListino>();

                if (p != null)
                {
                    String[] entries = p.Split(';');

                    foreach (String e in entries)
                        r.Add(new EntryListino(e));
                }

                return r;
            }

            internal static string getListinoString(List<EntryListino> lista)
            {
                String ret = string.Empty;

                foreach (EntryListino eo in lista)
                {
                    if (!string.IsNullOrEmpty(ret))
                        ret += ";";
                    ret += eo.ToString();
                }

                return ret;
            }
        }
    }
}
