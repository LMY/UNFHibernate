using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UNFHibernate
{
    public partial class ViewStagione : Form
    {
        public ViewStagione()
        {
            InitializeComponent();
        }

        public static void Show(MainForm m, UNFHibernate.Domain.Stagione s)
        {
            ViewStagione va = new ViewStagione();
            va.setStagione(s);
            va.mainform = m;
            va.Show();
        }

        public MainForm mainform { get; set; }
        public UNFHibernate.Domain.Stagione Stagione { get { return stagione; } }
        private UNFHibernate.Domain.Stagione stagione;

        public void setStagione(UNFHibernate.Domain.Stagione s)
        {
            stagione = s;

            if (stagione != null)
            {
                textDescrizione.Text = stagione.Descrizione ?? string.Empty;

                if (stagione.DataInizio != null)
                    try
                    {
                        dateInizio.Value = stagione.DataInizio;
                    }
                    catch (Exception)
                    {
                        dateInizio.Value = DateTime.Today;
                    }
                else
                    Utils.SetPickerValid(dateInizio, false);

                if (stagione.DataFine != null)
                    try
                    {
                        dateFine.Value = stagione.DataFine;
                    }
                    catch (Exception)
                    {
                        dateFine.Value = DateTime.Today;
                    }

                else
                    Utils.SetPickerValid(dateFine, false);

                if (stagione.FineQuadrimestre != null)
                    try
                    {
                        dateFineQuadrimestre.Value = stagione.FineQuadrimestre;
                    }
                    catch (Exception)
                    {
                        dateFineQuadrimestre.Value = DateTime.Today;
                    }
                else
                    Utils.SetPickerValid(dateFineQuadrimestre, false);
            }
            else
            {
                Utils.SetPickerValid(dateInizio, false);
                Utils.SetPickerValid(dateFine, false);
                Utils.SetPickerValid(dateFineQuadrimestre, false);
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (Save())
                if (mainform != null)
                    mainform.RefreshStagioni();
        }

        private void buttonSaveExit_Click(object sender, EventArgs e)
        {
            if (Save())
            {
                if (mainform != null)
                    mainform.RefreshStagioni();
                Close();
            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Close();
        }
        
        public void UpdateData()
        {
            if (stagione == null)
                stagione = new UNFHibernate.Domain.Stagione();

            stagione.Descrizione = textDescrizione.Text;

            if (dateInizio.Format == DateTimePickerFormat.Short)
                stagione.DataInizio = dateInizio.Value;

            if (dateFine.Format == DateTimePickerFormat.Short)
                stagione.DataFine = dateFine.Value;

            if (dateFineQuadrimestre.Format == DateTimePickerFormat.Short)
                stagione.FineQuadrimestre = dateFineQuadrimestre.Value;
        }

        public bool Save()
        {
            UpdateData();

            if (stagione.DataFine < stagione.DataInizio ||
                ((stagione.FineQuadrimestre != null) && (stagione.FineQuadrimestre < stagione.DataInizio || stagione.FineQuadrimestre > stagione.DataFine)))
            {
                MessageBox.Show("Controllare le date!", "ERRORE");
                return false;
            }

            if (DB.instance.save(stagione))
            {
                mainform.RefreshStagioni();
                mainform.RefreshCorsi();
                return true;
            }
            return false;
        }
    }
}
