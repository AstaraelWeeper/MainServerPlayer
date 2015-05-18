namespace SocketTutorial.FormsServer
{
    partial class MainView
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
            this.txtJSONin = new System.Windows.Forms.TextBox();
            this.txtJSONout = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtJSONin
            // 
            this.txtJSONin.Location = new System.Drawing.Point(12, 2);
            this.txtJSONin.Multiline = true;
            this.txtJSONin.Name = "txtJSONin";
            this.txtJSONin.Size = new System.Drawing.Size(710, 108);
            this.txtJSONin.TabIndex = 0;
            // 
            // txtJSONout
            // 
            this.txtJSONout.Location = new System.Drawing.Point(12, 116);
            this.txtJSONout.Multiline = true;
            this.txtJSONout.Name = "txtJSONout";
            this.txtJSONout.Size = new System.Drawing.Size(710, 133);
            this.txtJSONout.TabIndex = 1;
            // 
            // MainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 261);
            this.Controls.Add(this.txtJSONout);
            this.Controls.Add(this.txtJSONin);
            this.Name = "MainView";
            this.Text = "MainView";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtJSONin;
        private System.Windows.Forms.TextBox txtJSONout;
    }
}