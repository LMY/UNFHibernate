namespace UNFHibernate
{
    partial class ViewAnagrafica
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.textLuogoNascita = new System.Windows.Forms.TextBox();
            this.textCodiceFiscale = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.panelIscrizioni = new System.Windows.Forms.TabControl();
            this.checkBoxCorsiNonSaldati = new System.Windows.Forms.CheckBox();
            this.checkBoxCorsiAttivi = new System.Windows.Forms.CheckBox();
            this.buttonAddIstruttore = new System.Windows.Forms.Button();
            this.textCognome = new System.Windows.Forms.TextBox();
            this.LabelNome = new System.Windows.Forms.Label();
            this.dateNascita = new System.Windows.Forms.DateTimePicker();
            this.textNumeroTelefono = new System.Windows.Forms.TextBox();
            this.textNumeroCellulare = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.textEmail = new System.Windows.Forms.TextBox();
            this.textNome = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textGenitoreLuogoNascita = new System.Windows.Forms.TextBox();
            this.textGenitoreCognome = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.dateNascitaGenitore = new System.Windows.Forms.DateTimePicker();
            this.textGenitoreNome = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textProvincia = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textCAP = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textComune = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textIndirizzo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.LabelNatoA = new System.Windows.Forms.Label();
            this.checkBoxMaschio = new System.Windows.Forms.CheckBox();
            this.textNote = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.groupAnagrafica = new System.Windows.Forms.GroupBox();
            this.groupBox6.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupAnagrafica.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button1.BackgroundImage = global::UNFHibernate.Properties.Resources.icon_save;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button1.Location = new System.Drawing.Point(6, 592);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(108, 58);
            this.button1.TabIndex = 0;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(139, 108);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(55, 23);
            this.button4.TabIndex = 6;
            this.button4.Text = "genera";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button_calcCF);
            // 
            // textLuogoNascita
            // 
            this.textLuogoNascita.Location = new System.Drawing.Point(6, 71);
            this.textLuogoNascita.Name = "textLuogoNascita";
            this.textLuogoNascita.Size = new System.Drawing.Size(157, 20);
            this.textLuogoNascita.TabIndex = 2;
            // 
            // textCodiceFiscale
            // 
            this.textCodiceFiscale.Location = new System.Drawing.Point(6, 110);
            this.textCodiceFiscale.Name = "textCodiceFiscale";
            this.textCodiceFiscale.Size = new System.Drawing.Size(123, 20);
            this.textCodiceFiscale.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 94);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(26, 13);
            this.label6.TabIndex = 24;
            this.label6.Text = "C.F.";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.panelIscrizioni);
            this.groupBox6.Controls.Add(this.checkBoxCorsiNonSaldati);
            this.groupBox6.Controls.Add(this.checkBoxCorsiAttivi);
            this.groupBox6.Controls.Add(this.buttonAddIstruttore);
            this.groupBox6.Location = new System.Drawing.Point(3, 297);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(691, 289);
            this.groupBox6.TabIndex = 27;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Iscrizioni";
            // 
            // panelIscrizioni
            // 
            this.panelIscrizioni.Location = new System.Drawing.Point(6, 47);
            this.panelIscrizioni.Name = "panelIscrizioni";
            this.panelIscrizioni.SelectedIndex = 0;
            this.panelIscrizioni.Size = new System.Drawing.Size(679, 236);
            this.panelIscrizioni.TabIndex = 26;
            // 
            // checkBoxCorsiNonSaldati
            // 
            this.checkBoxCorsiNonSaldati.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxCorsiNonSaldati.AutoSize = true;
            this.checkBoxCorsiNonSaldati.Location = new System.Drawing.Point(310, 17);
            this.checkBoxCorsiNonSaldati.Name = "checkBoxCorsiNonSaldati";
            this.checkBoxCorsiNonSaldati.Size = new System.Drawing.Size(92, 23);
            this.checkBoxCorsiNonSaldati.TabIndex = 2;
            this.checkBoxCorsiNonSaldati.Text = "Solo non saldati";
            this.checkBoxCorsiNonSaldati.UseVisualStyleBackColor = true;
            this.checkBoxCorsiNonSaldati.CheckedChanged += new System.EventHandler(this.Event_SearchCorsiChanged);
            // 
            // checkBoxCorsiAttivi
            // 
            this.checkBoxCorsiAttivi.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxCorsiAttivi.AutoSize = true;
            this.checkBoxCorsiAttivi.Checked = true;
            this.checkBoxCorsiAttivi.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCorsiAttivi.Location = new System.Drawing.Point(241, 17);
            this.checkBoxCorsiAttivi.Name = "checkBoxCorsiAttivi";
            this.checkBoxCorsiAttivi.Size = new System.Drawing.Size(63, 23);
            this.checkBoxCorsiAttivi.TabIndex = 1;
            this.checkBoxCorsiAttivi.Text = "Solo attivi";
            this.checkBoxCorsiAttivi.UseVisualStyleBackColor = true;
            this.checkBoxCorsiAttivi.CheckedChanged += new System.EventHandler(this.Event_SearchCorsiChanged);
            // 
            // buttonAddIstruttore
            // 
            this.buttonAddIstruttore.BackgroundImage = global::UNFHibernate.Properties.Resources.icon_blueadd;
            this.buttonAddIstruttore.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonAddIstruttore.Location = new System.Drawing.Point(3, 17);
            this.buttonAddIstruttore.Name = "buttonAddIstruttore";
            this.buttonAddIstruttore.Size = new System.Drawing.Size(191, 23);
            this.buttonAddIstruttore.TabIndex = 0;
            this.buttonAddIstruttore.UseVisualStyleBackColor = true;
            this.buttonAddIstruttore.Click += new System.EventHandler(this.buttonAddIstruttore_Click);
            // 
            // textCognome
            // 
            this.textCognome.Location = new System.Drawing.Point(169, 32);
            this.textCognome.Name = "textCognome";
            this.textCognome.Size = new System.Drawing.Size(169, 20);
            this.textCognome.TabIndex = 1;
            // 
            // LabelNome
            // 
            this.LabelNome.AutoSize = true;
            this.LabelNome.Location = new System.Drawing.Point(6, 16);
            this.LabelNome.Name = "LabelNome";
            this.LabelNome.Size = new System.Drawing.Size(35, 13);
            this.LabelNome.TabIndex = 16;
            this.LabelNome.Text = "Nome";
            // 
            // dateNascita
            // 
            this.dateNascita.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateNascita.Location = new System.Drawing.Point(169, 70);
            this.dateNascita.Name = "dateNascita";
            this.dateNascita.Size = new System.Drawing.Size(97, 20);
            this.dateNascita.TabIndex = 3;
            this.dateNascita.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // textNumeroTelefono
            // 
            this.textNumeroTelefono.Location = new System.Drawing.Point(6, 110);
            this.textNumeroTelefono.Name = "textNumeroTelefono";
            this.textNumeroTelefono.Size = new System.Drawing.Size(157, 20);
            this.textNumeroTelefono.TabIndex = 4;
            // 
            // textNumeroCellulare
            // 
            this.textNumeroCellulare.Location = new System.Drawing.Point(169, 110);
            this.textNumeroCellulare.Name = "textNumeroCellulare";
            this.textNumeroCellulare.Size = new System.Drawing.Size(156, 20);
            this.textNumeroCellulare.TabIndex = 5;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 133);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(32, 13);
            this.label13.TabIndex = 18;
            this.label13.Text = "Email";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 94);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(49, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Telefono";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(169, 94);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(47, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Cellulare";
            // 
            // textEmail
            // 
            this.textEmail.Location = new System.Drawing.Point(6, 149);
            this.textEmail.Name = "textEmail";
            this.textEmail.Size = new System.Drawing.Size(319, 20);
            this.textEmail.TabIndex = 6;
            // 
            // textNome
            // 
            this.textNome.Location = new System.Drawing.Point(6, 32);
            this.textNome.Name = "textNome";
            this.textNome.Size = new System.Drawing.Size(157, 20);
            this.textNome.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textGenitoreLuogoNascita);
            this.groupBox3.Controls.Add(this.textGenitoreCognome);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.dateNascitaGenitore);
            this.groupBox3.Controls.Add(this.textGenitoreNome);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Location = new System.Drawing.Point(351, 186);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(342, 105);
            this.groupBox3.TabIndex = 22;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Dati Genitore";
            // 
            // textGenitoreLuogoNascita
            // 
            this.textGenitoreLuogoNascita.Location = new System.Drawing.Point(9, 71);
            this.textGenitoreLuogoNascita.Name = "textGenitoreLuogoNascita";
            this.textGenitoreLuogoNascita.Size = new System.Drawing.Size(157, 20);
            this.textGenitoreLuogoNascita.TabIndex = 2;
            // 
            // textGenitoreCognome
            // 
            this.textGenitoreCognome.Location = new System.Drawing.Point(177, 32);
            this.textGenitoreCognome.Name = "textGenitoreCognome";
            this.textGenitoreCognome.Size = new System.Drawing.Size(157, 20);
            this.textGenitoreCognome.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(174, 55);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(33, 13);
            this.label9.TabIndex = 7;
            this.label9.Text = "Data:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(174, 16);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(55, 13);
            this.label14.TabIndex = 1;
            this.label14.Text = "Cognome:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 16);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(35, 13);
            this.label10.TabIndex = 1;
            this.label10.Text = "Nome";
            // 
            // dateNascitaGenitore
            // 
            this.dateNascitaGenitore.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateNascitaGenitore.Location = new System.Drawing.Point(177, 71);
            this.dateNascitaGenitore.Name = "dateNascitaGenitore";
            this.dateNascitaGenitore.Size = new System.Drawing.Size(97, 20);
            this.dateNascitaGenitore.TabIndex = 3;
            this.dateNascitaGenitore.ValueChanged += new System.EventHandler(this.dateTimePicker2_ValueChanged);
            // 
            // textGenitoreNome
            // 
            this.textGenitoreNome.Location = new System.Drawing.Point(9, 32);
            this.textGenitoreNome.Name = "textGenitoreNome";
            this.textGenitoreNome.Size = new System.Drawing.Size(157, 20);
            this.textGenitoreNome.TabIndex = 0;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 55);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(43, 13);
            this.label12.TabIndex = 4;
            this.label12.Text = "Nascita";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(169, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Data:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textEmail);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.textNumeroCellulare);
            this.groupBox2.Controls.Add(this.textNumeroTelefono);
            this.groupBox2.Controls.Add(this.textProvincia);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.textCAP);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.textComune);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.textIndirizzo);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(351, 1);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(343, 179);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Residenza";
            // 
            // textProvincia
            // 
            this.textProvincia.Location = new System.Drawing.Point(6, 71);
            this.textProvincia.Name = "textProvincia";
            this.textProvincia.Size = new System.Drawing.Size(157, 20);
            this.textProvincia.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Provincia";
            // 
            // textCAP
            // 
            this.textCAP.Location = new System.Drawing.Point(169, 70);
            this.textCAP.Name = "textCAP";
            this.textCAP.Size = new System.Drawing.Size(38, 20);
            this.textCAP.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(169, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "CAP";
            // 
            // textComune
            // 
            this.textComune.Location = new System.Drawing.Point(169, 32);
            this.textComune.Name = "textComune";
            this.textComune.Size = new System.Drawing.Size(156, 20);
            this.textComune.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(169, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Comune";
            // 
            // textIndirizzo
            // 
            this.textIndirizzo.Location = new System.Drawing.Point(6, 32);
            this.textIndirizzo.Name = "textIndirizzo";
            this.textIndirizzo.Size = new System.Drawing.Size(157, 20);
            this.textIndirizzo.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Indirizzo";
            // 
            // LabelNatoA
            // 
            this.LabelNatoA.AutoSize = true;
            this.LabelNatoA.Location = new System.Drawing.Point(6, 55);
            this.LabelNatoA.Name = "LabelNatoA";
            this.LabelNatoA.Size = new System.Drawing.Size(42, 13);
            this.LabelNatoA.TabIndex = 20;
            this.LabelNatoA.Text = "Nato a:";
            // 
            // checkBoxMaschio
            // 
            this.checkBoxMaschio.AutoSize = true;
            this.checkBoxMaschio.Location = new System.Drawing.Point(272, 73);
            this.checkBoxMaschio.Name = "checkBoxMaschio";
            this.checkBoxMaschio.Size = new System.Drawing.Size(66, 17);
            this.checkBoxMaschio.TabIndex = 4;
            this.checkBoxMaschio.Text = "Maschio";
            this.checkBoxMaschio.UseVisualStyleBackColor = true;
            // 
            // textNote
            // 
            this.textNote.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textNote.Location = new System.Drawing.Point(3, 16);
            this.textNote.Multiline = true;
            this.textNote.Name = "textNote";
            this.textNote.Size = new System.Drawing.Size(337, 125);
            this.textNote.TabIndex = 0;
            // 
            // button3
            // 
            this.button3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button3.BackgroundImage = global::UNFHibernate.Properties.Resources.icon_cancel;
            this.button3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button3.Location = new System.Drawing.Point(577, 586);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(108, 64);
            this.button3.TabIndex = 2;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button2.BackgroundImage = global::UNFHibernate.Properties.Resources.icon_ok;
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button2.Location = new System.Drawing.Point(120, 592);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(108, 58);
            this.button2.TabIndex = 1;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textNote);
            this.groupBox1.Location = new System.Drawing.Point(2, 147);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(343, 144);
            this.groupBox1.TabIndex = 28;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Note:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(169, 16);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(52, 13);
            this.label11.TabIndex = 16;
            this.label11.Text = "Cognome";
            // 
            // groupAnagrafica
            // 
            this.groupAnagrafica.Controls.Add(this.LabelNome);
            this.groupAnagrafica.Controls.Add(this.LabelNatoA);
            this.groupAnagrafica.Controls.Add(this.checkBoxMaschio);
            this.groupAnagrafica.Controls.Add(this.label1);
            this.groupAnagrafica.Controls.Add(this.button4);
            this.groupAnagrafica.Controls.Add(this.textNome);
            this.groupAnagrafica.Controls.Add(this.textLuogoNascita);
            this.groupAnagrafica.Controls.Add(this.dateNascita);
            this.groupAnagrafica.Controls.Add(this.textCodiceFiscale);
            this.groupAnagrafica.Controls.Add(this.label11);
            this.groupAnagrafica.Controls.Add(this.label6);
            this.groupAnagrafica.Controls.Add(this.textCognome);
            this.groupAnagrafica.Location = new System.Drawing.Point(2, 1);
            this.groupAnagrafica.Name = "groupAnagrafica";
            this.groupAnagrafica.Size = new System.Drawing.Size(343, 140);
            this.groupAnagrafica.TabIndex = 29;
            this.groupAnagrafica.TabStop = false;
            this.groupAnagrafica.Text = "Dati Anagrafici";
            // 
            // ViewAnagrafica
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(699, 653);
            this.Controls.Add(this.groupAnagrafica);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "ViewAnagrafica";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Anagrafica";
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupAnagrafica.ResumeLayout(false);
            this.groupAnagrafica.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TextBox textLuogoNascita;
        private System.Windows.Forms.TextBox textCodiceFiscale;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button buttonAddIstruttore;
        private System.Windows.Forms.TextBox textCognome;
        private System.Windows.Forms.Label LabelNome;
        private System.Windows.Forms.DateTimePicker dateNascita;
        private System.Windows.Forms.TextBox textNumeroTelefono;
        private System.Windows.Forms.TextBox textNumeroCellulare;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textEmail;
        private System.Windows.Forms.TextBox textNome;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textGenitoreLuogoNascita;
        private System.Windows.Forms.TextBox textGenitoreCognome;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.DateTimePicker dateNascitaGenitore;
        private System.Windows.Forms.TextBox textGenitoreNome;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textProvincia;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textCAP;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textComune;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textIndirizzo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label LabelNatoA;
        private System.Windows.Forms.CheckBox checkBoxMaschio;
        private System.Windows.Forms.TextBox textNote;
        private System.Windows.Forms.CheckBox checkBoxCorsiNonSaldati;
        private System.Windows.Forms.CheckBox checkBoxCorsiAttivi;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.GroupBox groupAnagrafica;
        private System.Windows.Forms.TabControl panelIscrizioni;


    }
}