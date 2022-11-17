namespace ClientServiceInstaller
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.InstallBtn = new System.Windows.Forms.Button();
            this.UnInstallBtn = new System.Windows.Forms.Button();
            this.LauncherBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // InstallBtn
            // 
            this.InstallBtn.Location = new System.Drawing.Point(13, 13);
            this.InstallBtn.Name = "InstallBtn";
            this.InstallBtn.Size = new System.Drawing.Size(75, 23);
            this.InstallBtn.TabIndex = 0;
            this.InstallBtn.Text = "安装";
            this.InstallBtn.UseVisualStyleBackColor = true;
            this.InstallBtn.Click += new System.EventHandler(this.InstallBtn_Click);
            // 
            // UnInstallBtn
            // 
            this.UnInstallBtn.Location = new System.Drawing.Point(13, 43);
            this.UnInstallBtn.Name = "UnInstallBtn";
            this.UnInstallBtn.Size = new System.Drawing.Size(75, 23);
            this.UnInstallBtn.TabIndex = 1;
            this.UnInstallBtn.Text = "卸载";
            this.UnInstallBtn.UseVisualStyleBackColor = true;
            this.UnInstallBtn.Click += new System.EventHandler(this.UnInstallBtn_Click);
            // 
            // LauncherBtn
            // 
            this.LauncherBtn.Location = new System.Drawing.Point(13, 73);
            this.LauncherBtn.Name = "LauncherBtn";
            this.LauncherBtn.Size = new System.Drawing.Size(75, 23);
            this.LauncherBtn.TabIndex = 2;
            this.LauncherBtn.Text = "启动测试";
            this.LauncherBtn.UseVisualStyleBackColor = true;
            this.LauncherBtn.Click += new System.EventHandler(this.LauncherBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 151);
            this.Controls.Add(this.LauncherBtn);
            this.Controls.Add(this.UnInstallBtn);
            this.Controls.Add(this.InstallBtn);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button InstallBtn;
        private System.Windows.Forms.Button UnInstallBtn;
        private System.Windows.Forms.Button LauncherBtn;
    }
}

