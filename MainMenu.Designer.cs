﻿namespace Chess_App
{
    partial class MainMenu
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
            button1 = new Button();
            PlayerVsComputer = new Button();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(299, 139);
            button1.Name = "button1";
            button1.Size = new Size(181, 55);
            button1.TabIndex = 0;
            button1.Text = "Player vs Player";
            button1.UseVisualStyleBackColor = true;
            button1.Click += PlayerVsPlayer_Click;
            // 
            // PlayerVsComputer
            // 
            PlayerVsComputer.Location = new Point(299, 200);
            PlayerVsComputer.Name = "PlayerVsComputer";
            PlayerVsComputer.Size = new Size(181, 55);
            PlayerVsComputer.TabIndex = 1;
            PlayerVsComputer.Text = "Player vs Computer";
            PlayerVsComputer.UseVisualStyleBackColor = true;
            PlayerVsComputer.Click += PlayerVsComputer_Click;
            // 
            // MainMenu
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(PlayerVsComputer);
            Controls.Add(button1);
            Name = "MainMenu";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MainMenu";
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
        private Button PlayerVsComputer;
    }
}