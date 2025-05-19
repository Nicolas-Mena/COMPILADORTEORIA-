namespace CompiladorFinal
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtCodigo;
        private System.Windows.Forms.Button btnAnalizar;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.ListBox lstTokens;
        private System.Windows.Forms.TextBox txtErrores;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtCodigo = new System.Windows.Forms.TextBox();
            this.btnAnalizar = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.lstTokens = new System.Windows.Forms.ListBox();
            this.txtErrores = new System.Windows.Forms.TextBox();
            this.SuspendLayout();

            // 
            // txtCodigo
            // 
            this.txtCodigo.Location = new System.Drawing.Point(12, 12);
            this.txtCodigo.Multiline = true;
            this.txtCodigo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtCodigo.Size = new System.Drawing.Size(680, 250);
            this.txtCodigo.Font = new System.Drawing.Font("Consolas", 10F);
            this.txtCodigo.WordWrap = false;
            this.txtCodigo.AcceptsTab = true;

            // 
            // btnAnalizar
            // 
            this.btnAnalizar.Location = new System.Drawing.Point(12, 270);
            this.btnAnalizar.Size = new System.Drawing.Size(100, 30);
            this.btnAnalizar.Text = "Analizar";
            this.btnAnalizar.UseVisualStyleBackColor = true;
            this.btnAnalizar.Click += new System.EventHandler(this.btnAnalizar_Click);

            // 
            // btnLimpiar
            // 
            this.btnLimpiar.Location = new System.Drawing.Point(120, 270);
            this.btnLimpiar.Size = new System.Drawing.Size(100, 30);
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.UseVisualStyleBackColor = true;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);

            // 
            // lstTokens
            // 
            this.lstTokens.Location = new System.Drawing.Point(12, 310);
            this.lstTokens.Size = new System.Drawing.Size(200, 180);
            this.lstTokens.Font = new System.Drawing.Font("Consolas", 9F);
            this.lstTokens.FormattingEnabled = true;
            this.lstTokens.HorizontalScrollbar = true;

            // 
            // txtErrores
            // 
            this.txtErrores.Location = new System.Drawing.Point(220, 310);
            this.txtErrores.Multiline = true;
            this.txtErrores.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtErrores.Size = new System.Drawing.Size(472, 180);
            this.txtErrores.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtErrores.ReadOnly = true;
            this.txtErrores.WordWrap = false;
            this.txtErrores.BackColor = System.Drawing.SystemColors.Window;

            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(704, 500);
            this.Controls.Add(this.txtCodigo);
            this.Controls.Add(this.btnAnalizar);
            this.Controls.Add(this.btnLimpiar);
            this.Controls.Add(this.lstTokens);
            this.Controls.Add(this.txtErrores);
            this.Name = "Form1";
            this.Text = "Compilador Final - C#";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

    }
}



