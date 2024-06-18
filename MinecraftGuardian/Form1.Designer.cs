namespace MinecraftGuardian
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            labelStatus = new Label();
            buttonStart = new Button();
            buttonStop = new Button();
            SuspendLayout();
            // 
            // labelStatus
            // 
            labelStatus.AutoSize = true;
            labelStatus.Font = new Font("Microsoft JhengHei UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            labelStatus.Location = new Point(90, 54);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(100, 28);
            labelStatus.TabIndex = 0;
            labelStatus.Text = "啟動狀態";
            // 
            // buttonStart
            // 
            buttonStart.Location = new Point(51, 119);
            buttonStart.Name = "buttonStart";
            buttonStart.Size = new Size(75, 23);
            buttonStart.TabIndex = 1;
            buttonStart.Text = "開始監測";
            buttonStart.UseVisualStyleBackColor = true;
            buttonStart.Click += buttonStart_Click;
            // 
            // buttonStop
            // 
            buttonStop.Location = new Point(155, 119);
            buttonStop.Name = "buttonStop";
            buttonStop.Size = new Size(75, 23);
            buttonStop.TabIndex = 2;
            buttonStop.Text = "關閉監測";
            buttonStop.UseVisualStyleBackColor = true;
            buttonStop.Click += buttonStop_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(296, 182);
            Controls.Add(buttonStop);
            Controls.Add(buttonStart);
            Controls.Add(labelStatus);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "Minecraft小工具";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelStatus;
        private Button buttonStart;
        private Button buttonStop;
    }
}