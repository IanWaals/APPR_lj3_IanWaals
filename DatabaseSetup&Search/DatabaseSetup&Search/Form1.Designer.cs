namespace DatabaseSetup_Search
{
    partial class Form1
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
            this.dgvInformation = new System.Windows.Forms.DataGridView();
            this.btnSearchIwaa = new System.Windows.Forms.Button();
            this.btnResetIwaa = new System.Windows.Forms.Button();
            this.txbSearchIwaa = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInformation)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvInformation
            // 
            this.dgvInformation.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInformation.Location = new System.Drawing.Point(2, 33);
            this.dgvInformation.Name = "dgvInformation";
            this.dgvInformation.RowHeadersWidth = 51;
            this.dgvInformation.RowTemplate.Height = 24;
            this.dgvInformation.Size = new System.Drawing.Size(798, 417);
            this.dgvInformation.TabIndex = 0;
            // 
            // btnSearchIwaa
            // 
            this.btnSearchIwaa.Location = new System.Drawing.Point(194, 7);
            this.btnSearchIwaa.Name = "btnSearchIwaa";
            this.btnSearchIwaa.Size = new System.Drawing.Size(154, 23);
            this.btnSearchIwaa.TabIndex = 1;
            this.btnSearchIwaa.Text = "Search";
            this.btnSearchIwaa.UseVisualStyleBackColor = true;
            this.btnSearchIwaa.Click += new System.EventHandler(this.btnSearchIwaa_Click);
            // 
            // btnResetIwaa
            // 
            this.btnResetIwaa.Location = new System.Drawing.Point(372, 7);
            this.btnResetIwaa.Name = "btnResetIwaa";
            this.btnResetIwaa.Size = new System.Drawing.Size(154, 23);
            this.btnResetIwaa.TabIndex = 2;
            this.btnResetIwaa.Text = "Reset";
            this.btnResetIwaa.UseVisualStyleBackColor = true;
            this.btnResetIwaa.Click += new System.EventHandler(this.btnResetIwaa_Click);
            // 
            // txbSearchIwaa
            // 
            this.txbSearchIwaa.Location = new System.Drawing.Point(10, 8);
            this.txbSearchIwaa.Name = "txbSearchIwaa";
            this.txbSearchIwaa.Size = new System.Drawing.Size(153, 22);
            this.txbSearchIwaa.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.txbSearchIwaa);
            this.Controls.Add(this.btnResetIwaa);
            this.Controls.Add(this.btnSearchIwaa);
            this.Controls.Add(this.dgvInformation);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInformation)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvInformation;
        private System.Windows.Forms.Button btnSearchIwaa;
        private System.Windows.Forms.Button btnResetIwaa;
        private System.Windows.Forms.TextBox txbSearchIwaa;
    }
}

