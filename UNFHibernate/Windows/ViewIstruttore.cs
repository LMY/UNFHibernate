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
    public partial class ViewIstruttore : Form
    {
        public ViewIstruttore()
        {
            InitializeComponent();
        }

        public static void Show(MainForm m, UNFHibernate.Domain.Istruttore i)
        {
            ViewIstruttore va = new ViewIstruttore();
            va.setIstruttore(i);
            va.mainform = m;
            va.Show();
        }

        public MainForm mainform { get; set; }
        private UNFHibernate.Domain.Istruttore istruttore;
        public UNFHibernate.Domain.Istruttore Istruttore { get { return istruttore; } }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Save();
            if (mainform != null)
                mainform.RefreshIstruttori();
        }

        private void buttonSaveExit_Click(object sender, EventArgs e)
        {
            Save();
            if (mainform != null)
                mainform.RefreshIstruttori();
            Close();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void setIstruttore(UNFHibernate.Domain.Istruttore i)
        {
            istruttore = i;

            if (istruttore != null)
            {
                textBox1.Text = istruttore.Cognome ?? string.Empty;
                textBox2.Text = istruttore.Nome ?? string.Empty;

                if (istruttore.DataNascita != null)
                    dateTimePicker1.Value = istruttore.DataNascita.Value;
                else
                    Utils.SetPickerValid(dateTimePicker1, false);

                textBox3.Text = istruttore.LuogoNascita ?? string.Empty;

                textBox4.Text = istruttore.Indirizzo ?? string.Empty;
                textBox5.Text = istruttore.Comune ?? string.Empty;
                textBox6.Text = istruttore.CAP ?? string.Empty;
                textBox7.Text = istruttore.Provincia ?? string.Empty;
                textBox8.Text = istruttore.CodiceFiscale ?? string.Empty;

                textBox9.Text = istruttore.Email ?? string.Empty;
                textBox10.Text = istruttore.NumeroTelefono ?? string.Empty;
                textBox14.Text = istruttore.NumeroCellulare ?? string.Empty;
            }
            else
            {
                Utils.SetPickerValid(dateTimePicker1, false);
            }
        }



        private bool NeedToCreate
        {
            get
            {
                return (istruttore == null && (textBox1.Text.Length > 0 || textBox2.Text.Length > 0 || textBox3.Text.Length > 0 || textBox4.Text.Length > 0 ||
                    textBox5.Text.Length > 0 || textBox6.Text.Length > 0 || textBox7.Text.Length > 0 || textBox8.Text.Length > 0 || textBox9.Text.Length > 0 || textBox10.Text.Length > 0 || textBox14.Text.Length > 0 ||
                    dateTimePicker1.Format == DateTimePickerFormat.Short));
            }
        }

        public bool Modified
        {
            get
            {
                if (NeedToCreate)
                    return true;

                if (istruttore != null)
                {
                    if (textBox1.Text != istruttore.Cognome)
                        return true;
                    if (textBox2.Text != istruttore.Nome)
                        return true;
                    if (textBox3.Text != istruttore.LuogoNascita)
                        return true;
                    if (textBox4.Text != istruttore.Indirizzo)
                        return true;
                    if (textBox5.Text != istruttore.Comune)
                        return true;
                    if (textBox6.Text != istruttore.CAP)
                        return true;
                    if (textBox7.Text != istruttore.Provincia)
                        return true;
                    if (textBox8.Text != istruttore.CodiceFiscale)
                        return true;
                    if (textBox9.Text != istruttore.Email)
                        return true;
                    if (textBox10.Text != istruttore.NumeroTelefono)
                        return true;
                    if (textBox14.Text != istruttore.NumeroCellulare)
                        return true;

                    if (dateTimePicker1.Format == DateTimePickerFormat.Short && dateTimePicker1.Value != istruttore.DataNascita)
                        return true;
                }

                return false;
            }
        }

        public void UpdateData()
        {
            if (NeedToCreate)
                istruttore = new UNFHibernate.Domain.Istruttore();

            if (istruttore != null)
            {
                istruttore.Cognome = textBox1.Text;
                istruttore.Nome = textBox2.Text;
                istruttore.LuogoNascita = textBox3.Text;
                istruttore.Indirizzo = textBox4.Text;
                istruttore.Comune = textBox5.Text;
                istruttore.CAP = textBox6.Text;
                istruttore.Provincia = textBox7.Text;
                istruttore.CodiceFiscale = textBox8.Text;
                istruttore.Email = textBox9.Text;
                istruttore.NumeroTelefono = textBox10.Text;
                istruttore.NumeroCellulare = textBox14.Text;

                if (dateTimePicker1.Format == DateTimePickerFormat.Short)
                    istruttore.DataNascita = dateTimePicker1.Value;
            }
        }

        public void Save()
        {
            if (Modified)
            {
                UpdateData();

                DB.instance.save(istruttore);
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            Utils.SetPickerValid(dateTimePicker1, true);
        }
    }
}
