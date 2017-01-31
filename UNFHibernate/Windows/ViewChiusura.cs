using System;
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
    public partial class ViewChiusura : Form
    {
        public ViewChiusura()
        {
            InitializeComponent();
            mainform = null;
            chiusura = null;
        }
        
        public static void Show(MainForm m, UNFHibernate.Domain.Chiusura c)
        {
            ViewChiusura va = new ViewChiusura();
            va.setChiusura(c);
            va.mainform = m;
            va.Show();
        }

        public MainForm mainform { get; set; }
        private UNFHibernate.Domain.Chiusura chiusura;
        public UNFHibernate.Domain.Chiusura Chiusura { get { return chiusura; } }

        public void setChiusura(UNFHibernate.Domain.Chiusura c)
        {
            chiusura = c;

            comboStagioni.Items.Clear();
            List<Stagione> stagioni = DB.instance.stagioni;
            foreach (Stagione s in stagioni)
                comboStagioni.Items.Add(s.Descrizione);

            if (chiusura != null)
            {
                textDescrizione.Text = chiusura.Descrizione ?? string.Empty;

                if (chiusura.stagione != null)
                {
                    if (!comboStagioni.Items.Contains(chiusura.stagione.Descrizione))
                        comboStagioni.Items.Add(chiusura.stagione.Descrizione);
                    comboStagioni.Text = chiusura.stagione.Descrizione;
                }

                if (chiusura.DataInizio != null)
                    try
                    {
                        dateInizio.Value = chiusura.DataInizio;
                    }
                    catch (Exception)
                    {
                        dateInizio.Value = DateTime.Today;
                    }

                else
                    Utils.SetPickerValid(dateInizio, false);

                if (chiusura.DataFine != null)
                    try
                    {
                        dateFine.Value = chiusura.DataFine;
                    }
                    catch (Exception)
                    {
                        dateFine.Value = DateTime.Today;
                    }
                else
                    Utils.SetPickerValid(dateFine, false);
            }
            else
            {
                Utils.SetPickerValid(dateInizio, false);
                Utils.SetPickerValid(dateFine, false);
            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Save();
            if (mainform != null)
                mainform.RefreshChiusure();
        }

        private void buttonSaveExit_Click(object sender, EventArgs e)
        {
            if (Save()) { 
                if (mainform != null)
                    mainform.RefreshChiusure();
                Close();
            }
        }

        public void UpdateData()
        {
            if (chiusura == null)
            {
                chiusura = new UNFHibernate.Domain.Chiusura();
                chiusura.stagione = DB.instance.stagione_corrente;
            }

            foreach (Stagione s in DB.instance.stagioni)
                if (s.Descrizione.Equals(comboStagioni.Text))
                {
                    chiusura.stagione = s;
                    break;
                }

            chiusura.Descrizione = textDescrizione.Text;

            if (dateInizio.Format == DateTimePickerFormat.Short)
                chiusura.DataInizio = dateInizio.Value;

            if (dateFine.Format == DateTimePickerFormat.Short)
                chiusura.DataFine = dateFine.Value;
        }

        public bool Save()
        {
            UpdateData();
            return DB.instance.save(chiusura);
        }
    }
}
