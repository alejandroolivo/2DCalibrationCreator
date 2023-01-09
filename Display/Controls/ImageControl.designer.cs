namespace TFM
{
    partial class ImageControl
    {
        /// <summary> 
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageControl));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.chkbDibujar = new System.Windows.Forms.CheckBox();
            this.lblCrdY = new System.Windows.Forms.Label();
            this.lblCrdX = new System.Windows.Forms.Label();
            this.lblY = new System.Windows.Forms.Label();
            this.lblX = new System.Windows.Forms.Label();
            this.btnFit = new System.Windows.Forms.Button();
            this.btnReducir = new System.Windows.Forms.Button();
            this.btnAmpliar = new System.Windows.Forms.Button();
            this.ptbImagen = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ptbImagen)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.CadetBlue;
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Cursor = System.Windows.Forms.Cursors.Default;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.DarkSlateGray;
            this.splitContainer1.Panel1.Controls.Add(this.chkbDibujar);
            this.splitContainer1.Panel1.Controls.Add(this.lblCrdY);
            this.splitContainer1.Panel1.Controls.Add(this.lblCrdX);
            this.splitContainer1.Panel1.Controls.Add(this.lblY);
            this.splitContainer1.Panel1.Controls.Add(this.lblX);
            this.splitContainer1.Panel1.Controls.Add(this.btnFit);
            this.splitContainer1.Panel1.Controls.Add(this.btnReducir);
            this.splitContainer1.Panel1.Controls.Add(this.btnAmpliar);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.Black;
            this.splitContainer1.Panel2.Controls.Add(this.ptbImagen);
            this.splitContainer1.Panel2.Cursor = System.Windows.Forms.Cursors.Cross;
            this.splitContainer1.Size = new System.Drawing.Size(494, 443);
            this.splitContainer1.SplitterDistance = 40;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 1;
            // 
            // chkbDibujar
            // 
            this.chkbDibujar.AutoSize = true;
            this.chkbDibujar.Dock = System.Windows.Forms.DockStyle.Right;
            this.chkbDibujar.Font = new System.Drawing.Font("Consolas", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkbDibujar.ForeColor = System.Drawing.Color.White;
            this.chkbDibujar.Location = new System.Drawing.Point(398, 0);
            this.chkbDibujar.Name = "chkbDibujar";
            this.chkbDibujar.Size = new System.Drawing.Size(94, 38);
            this.chkbDibujar.TabIndex = 6;
            this.chkbDibujar.Text = "Dibujar";
            this.chkbDibujar.UseVisualStyleBackColor = true;
            this.chkbDibujar.Visible = false;
            this.chkbDibujar.CheckedChanged += new System.EventHandler(this.chkbDibujar_CheckedChanged);
            // 
            // lblCrdY
            // 
            this.lblCrdY.AutoSize = true;
            this.lblCrdY.Font = new System.Drawing.Font("Consolas", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCrdY.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblCrdY.Location = new System.Drawing.Point(334, 8);
            this.lblCrdY.Name = "lblCrdY";
            this.lblCrdY.Size = new System.Drawing.Size(0, 20);
            this.lblCrdY.TabIndex = 5;
            // 
            // lblCrdX
            // 
            this.lblCrdX.AutoSize = true;
            this.lblCrdX.Font = new System.Drawing.Font("Consolas", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCrdX.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblCrdX.Location = new System.Drawing.Point(242, 8);
            this.lblCrdX.Name = "lblCrdX";
            this.lblCrdX.Size = new System.Drawing.Size(0, 20);
            this.lblCrdX.TabIndex = 4;
            // 
            // lblY
            // 
            this.lblY.AutoSize = true;
            this.lblY.Font = new System.Drawing.Font("Consolas", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblY.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblY.Location = new System.Drawing.Point(303, 8);
            this.lblY.Name = "lblY";
            this.lblY.Size = new System.Drawing.Size(27, 20);
            this.lblY.TabIndex = 4;
            this.lblY.Text = "Y:";
            // 
            // lblX
            // 
            this.lblX.AutoSize = true;
            this.lblX.Font = new System.Drawing.Font("Consolas", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblX.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblX.Location = new System.Drawing.Point(210, 8);
            this.lblX.Name = "lblX";
            this.lblX.Size = new System.Drawing.Size(27, 20);
            this.lblX.TabIndex = 3;
            this.lblX.Text = "X:";
            // 
            // btnFit
            // 
            this.btnFit.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnFit.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnFit.BackgroundImage")));
            this.btnFit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnFit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFit.FlatAppearance.BorderSize = 0;
            this.btnFit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFit.Location = new System.Drawing.Point(135, -2);
            this.btnFit.Name = "btnFit";
            this.btnFit.Size = new System.Drawing.Size(62, 38);
            this.btnFit.TabIndex = 2;
            this.btnFit.UseVisualStyleBackColor = true;
            this.btnFit.Click += new System.EventHandler(this.btnFit_Click);
            // 
            // btnReducir
            // 
            this.btnReducir.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnReducir.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnReducir.BackgroundImage")));
            this.btnReducir.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnReducir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReducir.FlatAppearance.BorderSize = 0;
            this.btnReducir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReducir.Location = new System.Drawing.Point(68, 0);
            this.btnReducir.Name = "btnReducir";
            this.btnReducir.Size = new System.Drawing.Size(61, 38);
            this.btnReducir.TabIndex = 1;
            this.btnReducir.UseVisualStyleBackColor = true;
            this.btnReducir.Click += new System.EventHandler(this.btnReducir_Click);
            // 
            // btnAmpliar
            // 
            this.btnAmpliar.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnAmpliar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnAmpliar.BackgroundImage")));
            this.btnAmpliar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAmpliar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAmpliar.FlatAppearance.BorderSize = 0;
            this.btnAmpliar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAmpliar.Location = new System.Drawing.Point(0, 0);
            this.btnAmpliar.Name = "btnAmpliar";
            this.btnAmpliar.Size = new System.Drawing.Size(62, 38);
            this.btnAmpliar.TabIndex = 0;
            this.btnAmpliar.UseVisualStyleBackColor = true;
            this.btnAmpliar.Click += new System.EventHandler(this.btnAmpliar_Click);
            // 
            // ptbImagen
            // 
            this.ptbImagen.BackColor = System.Drawing.Color.Black;
            this.ptbImagen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ptbImagen.InitialImage = null;
            this.ptbImagen.Location = new System.Drawing.Point(0, 2);
            this.ptbImagen.Name = "ptbImagen";
            this.ptbImagen.Size = new System.Drawing.Size(490, 397);
            this.ptbImagen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ptbImagen.TabIndex = 0;
            this.ptbImagen.TabStop = false;
            this.ptbImagen.Paint += new System.Windows.Forms.PaintEventHandler(this.ptbImagen_Paint);
            this.ptbImagen.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ptbImagen_MouseDown);
            this.ptbImagen.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ptbImagen_MouseMove);
            this.ptbImagen.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ptbImagen_MouseUp);
            // 
            // ImageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "ImageControl";
            this.Size = new System.Drawing.Size(494, 443);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ptbImagen)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.CheckBox chkbDibujar;
        private System.Windows.Forms.Label lblCrdY;
        private System.Windows.Forms.Label lblCrdX;
        private System.Windows.Forms.Label lblY;
        private System.Windows.Forms.Label lblX;
        private System.Windows.Forms.Button btnReducir;
        private System.Windows.Forms.Button btnAmpliar;
        private System.Windows.Forms.PictureBox ptbImagen;
        private System.Windows.Forms.Button btnFit;
    }
}
