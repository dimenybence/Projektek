namespace Blackjack
{
    partial class Jatek
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Jatek));
            this.bKilep = new System.Windows.Forms.Button();
            this.bMegall = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.bKerek = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.bUjJatek = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bKilep
            // 
            this.bKilep.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bKilep.Font = new System.Drawing.Font("Britannic Bold", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bKilep.Location = new System.Drawing.Point(1620, 956);
            this.bKilep.Name = "bKilep";
            this.bKilep.Size = new System.Drawing.Size(200, 120);
            this.bKilep.TabIndex = 0;
            this.bKilep.Text = "Kilép";
            this.bKilep.UseVisualStyleBackColor = true;
            this.bKilep.Click += new System.EventHandler(this.bKilep_Click);
            // 
            // bMegall
            // 
            this.bMegall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bMegall.Font = new System.Drawing.Font("Britannic Bold", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bMegall.Location = new System.Drawing.Point(1620, 430);
            this.bMegall.Name = "bMegall";
            this.bMegall.Size = new System.Drawing.Size(200, 120);
            this.bMegall.TabIndex = 0;
            this.bMegall.Text = "Megállok";
            this.bMegall.UseVisualStyleBackColor = true;
            this.bMegall.Click += new System.EventHandler(this.bMegall_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Location = new System.Drawing.Point(20, 650);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1800, 300);
            this.panel1.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Location = new System.Drawing.Point(20, 10);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1800, 300);
            this.panel2.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Britannic Bold", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(176, 570);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 67);
            this.label1.TabIndex = 4;
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bKerek
            // 
            this.bKerek.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bKerek.Font = new System.Drawing.Font("Britannic Bold", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bKerek.Location = new System.Drawing.Point(1340, 430);
            this.bKerek.Name = "bKerek";
            this.bKerek.Size = new System.Drawing.Size(200, 120);
            this.bKerek.TabIndex = 0;
            this.bKerek.Text = "Kérek";
            this.bKerek.UseVisualStyleBackColor = true;
            this.bKerek.Click += new System.EventHandler(this.bKerek_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Britannic Bold", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(176, 358);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 67);
            this.label2.TabIndex = 4;
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bUjJatek
            // 
            this.bUjJatek.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bUjJatek.Enabled = false;
            this.bUjJatek.Font = new System.Drawing.Font("Britannic Bold", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bUjJatek.Location = new System.Drawing.Point(1060, 430);
            this.bUjJatek.Name = "bUjJatek";
            this.bUjJatek.Size = new System.Drawing.Size(200, 120);
            this.bUjJatek.TabIndex = 0;
            this.bUjJatek.Text = "Új játék";
            this.bUjJatek.UseVisualStyleBackColor = true;
            this.bUjJatek.Click += new System.EventHandler(this.bUjJatek_Click);
            // 
            // Jatek
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(1920, 1080);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bUjJatek);
            this.Controls.Add(this.bKerek);
            this.Controls.Add(this.bMegall);
            this.Controls.Add(this.bKilep);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Jatek";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Jatek_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bKilep;
        private System.Windows.Forms.Button bMegall;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bKerek;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button bUjJatek;
    }
}