﻿namespace Budget.Forms
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
            this.newAccountBtn = new System.Windows.Forms.Button();
            this.newCategoryBtn = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // newAccountBtn
            // 
            this.newAccountBtn.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newAccountBtn.Location = new System.Drawing.Point(12, 292);
            this.newAccountBtn.Name = "newAccountBtn";
            this.newAccountBtn.Size = new System.Drawing.Size(150, 30);
            this.newAccountBtn.TabIndex = 1;
            this.newAccountBtn.Text = "New Account";
            this.newAccountBtn.UseVisualStyleBackColor = true;
            // 
            // newCategoryBtn
            // 
            this.newCategoryBtn.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newCategoryBtn.Location = new System.Drawing.Point(12, 631);
            this.newCategoryBtn.Name = "newCategoryBtn";
            this.newCategoryBtn.Size = new System.Drawing.Size(150, 30);
            this.newCategoryBtn.TabIndex = 1;
            this.newCategoryBtn.Text = "New Category";
            this.newCategoryBtn.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1191, 673);
            this.Controls.Add(this.newCategoryBtn);
            this.Controls.Add(this.newAccountBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.ShowIcon = false;
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button newAccountBtn;
        private System.Windows.Forms.Button newCategoryBtn;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}

