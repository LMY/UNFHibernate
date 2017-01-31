using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UNFHibernate.Domain;
using UNFHibernate.Windows;

namespace UNFHibernate.Components
{
    public partial class PanelIscrizione : UserControl
    {
        public Iscrizione iscrizione { get; private set; }
        private MainForm mainform;
        private ViewAnagrafica panelpersona;


        public PanelIscrizione()
        {
            InitializeComponent();
            iscrizione = null;
            mainform = null;
            panelpersona = null;
        }

        private Dictionary<string, long> importi = null;

        public void readImporti()
        {
            importi = new Dictionary<string, long>();

            string[] ing = iscrizione.corso.listino.ingressi.Split(';');    // 10/5000;20/8000;50/999999
            foreach (string s in ing)
            {
                string[] b = s.Split('/');
                try { importi[b[0]] = long.Parse(b[1]); }
                catch { importi[b[0]] = 0; }
            }
        }


        public void setIscrizione(Iscrizione i, MainForm mf, ViewAnagrafica pp)
        {
            iscrizione = i;
            mainform = mf;
            panelpersona = pp;

            readImporti();
            comboIngressi.Items.Clear();
            foreach (string s in importi.Keys)
                comboIngressi.Items.Add(s);  // #ingressi
                        
            comboPagamento1modalita.Items.Clear();
            comboPagamento2modalita.Items.Clear();
            comboPagamento3modalita.Items.Clear();
            String[] modalitapagamenti = Config.Instance.ModalitaPagamento;
            foreach (String s in modalitapagamenti)
            {
                comboPagamento1modalita.Items.Add(s);
                comboPagamento2modalita.Items.Add(s);
                comboPagamento3modalita.Items.Add(s);
            }

            if (iscrizione != null)
            {
                textBox1.Text = iscrizione.corso.Descrizione;

                if (!String.IsNullOrEmpty(iscrizione.primopagamento_modalita))
                {
                    if (!comboPagamento1modalita.Items.Contains(iscrizione.primopagamento_modalita))
                        comboPagamento1modalita.Items.Add(iscrizione.primopagamento_modalita);
                    comboPagamento1modalita.Text = iscrizione.primopagamento_modalita;
                }

                if (!String.IsNullOrEmpty(iscrizione.secondopagamento_modalita))
                {
                    if (!comboPagamento2modalita.Items.Contains(iscrizione.secondopagamento_modalita))
                        comboPagamento2modalita.Items.Add(iscrizione.secondopagamento_modalita);
                    comboPagamento2modalita.Text = iscrizione.secondopagamento_modalita;
                }

                if (!String.IsNullOrEmpty(iscrizione.terzopagamento_modalita))
                {
                    if (!comboPagamento3modalita.Items.Contains(iscrizione.terzopagamento_modalita))
                        comboPagamento3modalita.Items.Add(iscrizione.terzopagamento_modalita);
                    comboPagamento3modalita.Text = iscrizione.terzopagamento_modalita;
                }

                checkTassaIscrizione.Checked = iscrizione.tassa_iscrizione;
                textPagamento1importo.Text = Utils.moneylongToString(iscrizione.primopagamento_importo);
                textPagamento2importo.Text = Utils.moneylongToString(iscrizione.secondopagamento_importo);
                textPagamento3importo.Text = Utils.moneylongToString(iscrizione.terzopagamento_importo);
                textBox9.Text = iscrizione.tesseran > 0 ? iscrizione.tesseran.ToString() : String.Empty;
                textCartellino.Text = iscrizione.corso.TipoCartellino ?? string.Empty;

                Utils.SetPickerValidIfEnabled(i.data_iscrizione, ref dateIscrizione);
                Utils.SetPickerValidIfEnabled(i.data_inizio, ref dateInizio);
                Utils.SetPickerValid(dateFine, false);
                Utils.SetPickerValidIfEnabled(i.primopagamento_data, ref datePagamento1);
                Utils.SetPickerValidIfEnabled(i.secondopagamento_data, ref datePagamento2);
                Utils.SetPickerValidIfEnabled(i.terzopagamento_data, ref datePagamento3);
                Utils.SetPickerValidIfEnabled(i.data_socio, ref dateSocio);
                Utils.SetPickerValidIfEnabled(i.data_certificato, ref dateCertificato);
                Utils.SetPickerValidIfEnabled(i.data_rinuncia, ref dateRinuncia);

                comboIngressi.Text = iscrizione.ingressi.ToString(); // questa deve essere settata dopo al datePicker che triggera CalcolaGiornate() !
                //comboIngressi_TextChanged(null, null); // setta dovuto
                comboIngressi.Text = Utils.moneylongToString(iscrizione.importo);

                Colorize();
            }
            else
            {
                // questo non dovrebbe succedere
                textBox1.Text = String.Empty;
                Utils.SetPickerValid(dateCertificato, false);
                Utils.SetPickerValid(dateIscrizione, false);
                Utils.SetPickerValid(dateInizio, false);
                Utils.SetPickerValid(dateFine, false);
                Utils.SetPickerValid(datePagamento1, false);
                Utils.SetPickerValid(datePagamento2, false);
                Utils.SetPickerValid(datePagamento3, false);
                Utils.SetPickerValid(dateRinuncia, false);
                Utils.SetPickerValid(dateSocio, false);
            }

            CalcolaImporto();
        }

        private void Colorize()
        {
            // LMY: qui scegli se colore rosso per quelli non attivi o per quelli conclusi che non han ancora pagato!

            if (iscrizione.corso.DataFine < DateTime.Today)
            //if (!Utils.isAttivo(iscrizione.corso.Attivo))
            {
                if (!iscrizione.Saldato)
                    BackColor = Config.Instance.ColorNonPagato;
                else
                    BackColor = SystemColors.ControlDark;
            }
            else
                BackColor = SystemColors.Control;                
        }


        public void UpdateIscrizione()
        {
            if (iscrizione != null)
            {
                iscrizione.primopagamento_modalita = comboPagamento1modalita.Text;
                iscrizione.secondopagamento_modalita = comboPagamento2modalita.Text;
                iscrizione.terzopagamento_modalita = comboPagamento3modalita.Text;

                if (!String.IsNullOrEmpty(comboIngressi.Text))
                    try { iscrizione.ingressi = Int32.Parse(comboIngressi.Text); }
                    catch { iscrizione.ingressi = 0; }

                if (!String.IsNullOrEmpty(textBox9.Text))
                    try { iscrizione.tesseran = Int32.Parse(textBox9.Text); }
                    catch { }

                iscrizione.primopagamento_importo = Utils.moneystringToLong(textPagamento1importo.Text);
                iscrizione.secondopagamento_importo = Utils.moneystringToLong(textPagamento2importo.Text);
                iscrizione.terzopagamento_importo = Utils.moneystringToLong(textPagamento3importo.Text);

                if (dateIscrizione.Format == DateTimePickerFormat.Short)
                    iscrizione.data_iscrizione = dateIscrizione.Value;

                if (dateInizio.Format == DateTimePickerFormat.Short)
                    iscrizione.data_inizio = dateInizio.Value;

                if (datePagamento1.Format == DateTimePickerFormat.Short)
                    iscrizione.primopagamento_data = datePagamento1.Value;
                
                if (datePagamento2.Format == DateTimePickerFormat.Short)
                    iscrizione.secondopagamento_data = datePagamento2.Value;

                if (datePagamento3.Format == DateTimePickerFormat.Short)
                    iscrizione.terzopagamento_data = datePagamento3.Value;

                if (dateSocio.Format == DateTimePickerFormat.Short)
                    iscrizione.data_socio = dateSocio.Value;

                if (dateRinuncia.Format == DateTimePickerFormat.Short)
                    iscrizione.data_rinuncia = dateRinuncia.Value;

                if (dateCertificato.Format == DateTimePickerFormat.Short)
                    iscrizione.data_certificato = dateCertificato.Value;

                iscrizione.tassa_iscrizione = checkTassaIscrizione.Checked;

                iscrizione.importo = Utils.moneystringToLong(textImporto.Text);
                iscrizione.Saldato = Utils.moneystringToLong(textDovuto.Text) <= 0;
                Colorize();
            }
        }

        public void Save()
        {
            UpdateIscrizione();

            if (iscrizione != null)
                DB.instance.save(iscrizione);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (iscrizione == null)
            {
                MessageBox.Show("Impossibile rimuovere");
                return;
            }

            DialogResult dialogResult = MessageBox.Show("Sei sicuro di voler cancellare l'iscrizione?", "Sei sicuro?", MessageBoxButtons.YesNo);
            if (dialogResult != DialogResult.Yes)
                return;

            if (iscrizione != null)
            {
                Persona persona = iscrizione.persona;
                Corso corso = iscrizione.corso;

                persona.Iscrizioni.Remove(iscrizione);
                corso.Iscrizioni.Remove(iscrizione);

                DB.deleteIscrizione(iscrizione);
            }

            panelpersona.RefreshIscrizioni();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // OPEN CORSO
                mainform.OpenCorso(DB.instance.getCorso((Guid)iscrizione.corso.ID));
        }



        private void button3_Click(object sender, EventArgs e)
        {
            Printing.printCartellino(iscrizione);
        }

        private void CalcolaImporto()
        {
            // se c'è un importo salvato, quello. altrimenti calcolalo dal numero di ingressi
            long importo = iscrizione.importo > 0 ? iscrizione.importo : getDovuto(comboIngressi.Text);
            //long importo = getDovuto(comboIngressi.Text);

            long pag1 = Utils.moneystringToLong(textPagamento1importo.Text);
            long pag2 = Utils.moneystringToLong(textPagamento2importo.Text);
            long pag3 = Utils.moneystringToLong(textPagamento3importo.Text);

            long resto = importo - pag1 - pag2 - pag3;

            textImporto.Text = Utils.moneylongToString(importo);
            textDovuto.Text = resto <= 0 ? "0" : Utils.moneylongToString(resto);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            CalcolaImporto();
        }

        private void recalculateDataFine()
        {
            try {
                try
                {
                    dateFine.Value = iscrizione.getLastDate(int.Parse(comboIngressi.Text), dateInizio.Value);
                }
                catch { dateFine.Value = iscrizione.getLastDate(-1, dateInizio.Value); }
                
                Utils.SetPickerValid(dateFine, true);
            }
            catch { dateFine.Value = DateTime.Today; }
        }

        private void datePagamento1_ValueChanged(object sender, EventArgs e)
        {
            Utils.SetPickerValid(datePagamento1, true);
        }

        private void datePagamento2_ValueChanged(object sender, EventArgs e)
        {
            Utils.SetPickerValid(datePagamento2, true);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Printing.printPagamento(iscrizione, 0);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Printing.printPagamento(iscrizione, 1);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Printing.printPagamento(iscrizione, 2);
        }

        private long getDovuto(string ingressi)
        {
            try
            {
                // LMY: qui se vuoi "il più vicino" invece del più costoso
                long cmax = 0;

                foreach (var key in importi.Keys)
                {
                    if (importi[key] > cmax)
                        cmax = importi[key];

                    if (key.Equals(ingressi))
                        return importi[key];
                }

                return cmax;
            }
            catch { return 0; }
        }


        private void comboIngressi_TextChanged(object sender, EventArgs e)
        {
            CalcolaImporto();
        }

        private void setImportoReadOnlyStatus()
        {
            if (textImporto.ReadOnly)  // se la stringa non ha il ".00" in fondo... sembra più figo :)
                try { textImporto.Text = Utils.moneylongToString(Utils.moneystringToLong(textImporto.Text)); }
                catch { }
        }

        private void textImporto_DoubleClick(object sender, EventArgs e)
        {
            textImporto.ReadOnly = !textImporto.ReadOnly;
            setImportoReadOnlyStatus();
        }

        private void textImporto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)    // se premi enter, torna read only e tutto
            {
                textImporto.ReadOnly = true;
                setImportoReadOnlyStatus();
            }
        }

        private void textImporto_Leave(object sender, EventArgs e)
        {
            textImporto.ReadOnly = true;
            setImportoReadOnlyStatus();
        }

        private void comboIngressi_TextUpdate(object sender, EventArgs e)
        {
            recalculateDataFine();
        }

        private void comboIngressi_SelectedIndexChanged(object sender, EventArgs e)
        {
            recalculateDataFine();
        }

        private void dateRinuncia_ValueChanged(object sender, EventArgs e)
        {
            Utils.SetPickerValid(dateRinuncia, true);
        }

        private void dateFine_ValueChanged(object sender, EventArgs e)
        {
            Utils.SetPickerValid(dateFine, true);
        }

        private void dateCertificato_ValueChanged(object sender, EventArgs e)
        {
            Utils.SetPickerValid(dateCertificato, true);
        }

        private void datePagamento3_ValueChanged(object sender, EventArgs e)
        {
            Utils.SetPickerValid(datePagamento3, true);
        }

        private void dateIscrizione_ValueChanged(object sender, EventArgs e)
        {
            Utils.SetPickerValid(dateIscrizione, true);
        }

        private void dateInizio_ValueChanged(object sender, EventArgs e)
        {
            Utils.SetPickerValid(dateInizio, true);
            recalculateDataFine();
        }
        private void dateTimePicker5_ValueChanged(object sender, EventArgs e)
        {
            Utils.SetPickerValid(dateSocio, true);
        }


    }
}
