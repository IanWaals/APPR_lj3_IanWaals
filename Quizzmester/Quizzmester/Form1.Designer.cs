namespace Quizzmester
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
            this.gbxLoginOrRegisterIwaa = new System.Windows.Forms.GroupBox();
            this.lblQuizzmesterIwaa = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnLoginIwaa = new System.Windows.Forms.Button();
            this.btnRegisterIwaa = new System.Windows.Forms.Button();
            this.btnPlayAsGuestIwaa = new System.Windows.Forms.Button();
            this.gbxLoginOrRegisterIwaa.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxLoginOrRegisterIwaa
            // 
            this.gbxLoginOrRegisterIwaa.Controls.Add(this.btnRegisterIwaa);
            this.gbxLoginOrRegisterIwaa.Controls.Add(this.btnLoginIwaa);
            this.gbxLoginOrRegisterIwaa.Controls.Add(this.label1);
            this.gbxLoginOrRegisterIwaa.Location = new System.Drawing.Point(12, 113);
            this.gbxLoginOrRegisterIwaa.Name = "gbxLoginOrRegisterIwaa";
            this.gbxLoginOrRegisterIwaa.Size = new System.Drawing.Size(524, 325);
            this.gbxLoginOrRegisterIwaa.TabIndex = 0;
            this.gbxLoginOrRegisterIwaa.TabStop = false;
            this.gbxLoginOrRegisterIwaa.Text = "Login or Register";
            // 
            // lblQuizzmesterIwaa
            // 
            this.lblQuizzmesterIwaa.AutoSize = true;
            this.lblQuizzmesterIwaa.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQuizzmesterIwaa.Location = new System.Drawing.Point(12, 9);
            this.lblQuizzmesterIwaa.Name = "lblQuizzmesterIwaa";
            this.lblQuizzmesterIwaa.Size = new System.Drawing.Size(699, 69);
            this.lblQuizzmesterIwaa.TabIndex = 2;
            this.lblQuizzmesterIwaa.Text = "Welcome to QuizzMester";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(7, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(207, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Log in and play";
            // 
            // btnLoginIwaa
            // 
            this.btnLoginIwaa.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.btnLoginIwaa.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoginIwaa.Location = new System.Drawing.Point(13, 78);
            this.btnLoginIwaa.Name = "btnLoginIwaa";
            this.btnLoginIwaa.Size = new System.Drawing.Size(221, 236);
            this.btnLoginIwaa.TabIndex = 1;
            this.btnLoginIwaa.Text = "Login to your account";
            this.btnLoginIwaa.UseVisualStyleBackColor = false;
            // 
            // btnRegisterIwaa
            // 
            this.btnRegisterIwaa.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.btnRegisterIwaa.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRegisterIwaa.Location = new System.Drawing.Point(285, 78);
            this.btnRegisterIwaa.Name = "btnRegisterIwaa";
            this.btnRegisterIwaa.Size = new System.Drawing.Size(221, 236);
            this.btnRegisterIwaa.TabIndex = 2;
            this.btnRegisterIwaa.Text = "Register for an account";
            this.btnRegisterIwaa.UseVisualStyleBackColor = false;
            // 
            // btnPlayAsGuestIwaa
            // 
            this.btnPlayAsGuestIwaa.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.btnPlayAsGuestIwaa.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPlayAsGuestIwaa.Location = new System.Drawing.Point(563, 191);
            this.btnPlayAsGuestIwaa.Name = "btnPlayAsGuestIwaa";
            this.btnPlayAsGuestIwaa.Size = new System.Drawing.Size(195, 236);
            this.btnPlayAsGuestIwaa.TabIndex = 3;
            this.btnPlayAsGuestIwaa.Text = "Play as guest";
            this.btnPlayAsGuestIwaa.UseVisualStyleBackColor = false;
            this.btnPlayAsGuestIwaa.Click += new System.EventHandler(this.btnPlayAsGuestIwaa_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.HotTrack;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnPlayAsGuestIwaa);
            this.Controls.Add(this.lblQuizzmesterIwaa);
            this.Controls.Add(this.gbxLoginOrRegisterIwaa);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.gbxLoginOrRegisterIwaa.ResumeLayout(false);
            this.gbxLoginOrRegisterIwaa.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxLoginOrRegisterIwaa;
        private System.Windows.Forms.Label lblQuizzmesterIwaa;
        private System.Windows.Forms.Button btnRegisterIwaa;
        private System.Windows.Forms.Button btnLoginIwaa;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnPlayAsGuestIwaa;
    }
}

