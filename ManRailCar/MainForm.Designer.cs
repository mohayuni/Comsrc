namespace ManRailCar
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.selectDB = new System.Windows.Forms.Button();
            this.selectCSV = new System.Windows.Forms.Button();
            this.Serach = new System.Windows.Forms.Button();
            this.AddData = new System.Windows.Forms.Button();
            this.nameDB = new System.Windows.Forms.TextBox();
            this.nameCSV = new System.Windows.Forms.TextBox();
            this.EndSys = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nameClass = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.nameSeries = new System.Windows.Forms.TextBox();
            this.SerachInfo = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // selectDB
            // 
            this.selectDB.Location = new System.Drawing.Point(300, 37);
            this.selectDB.Name = "selectDB";
            this.selectDB.Size = new System.Drawing.Size(85, 21);
            this.selectDB.TabIndex = 0;
            this.selectDB.Text = "DB選択(&D)";
            this.selectDB.UseVisualStyleBackColor = true;
            this.selectDB.Click += new System.EventHandler(this.selectDB_Click);
            // 
            // selectCSV
            // 
            this.selectCSV.Enabled = false;
            this.selectCSV.Location = new System.Drawing.Point(300, 88);
            this.selectCSV.Name = "selectCSV";
            this.selectCSV.Size = new System.Drawing.Size(85, 21);
            this.selectCSV.TabIndex = 1;
            this.selectCSV.Text = "CSV選択(&C)";
            this.selectCSV.UseVisualStyleBackColor = true;
            this.selectCSV.Click += new System.EventHandler(this.selectCSV_Click);
            // 
            // Serach
            // 
            this.Serach.Enabled = false;
            this.Serach.Location = new System.Drawing.Point(300, 184);
            this.Serach.Name = "Serach";
            this.Serach.Size = new System.Drawing.Size(85, 21);
            this.Serach.TabIndex = 2;
            this.Serach.Text = "検索(&S)";
            this.Serach.UseVisualStyleBackColor = true;
            this.Serach.Click += new System.EventHandler(this.Serach_Click);
            // 
            // AddData
            // 
            this.AddData.Enabled = false;
            this.AddData.Location = new System.Drawing.Point(300, 265);
            this.AddData.Name = "AddData";
            this.AddData.Size = new System.Drawing.Size(85, 21);
            this.AddData.TabIndex = 3;
            this.AddData.Text = "データ追加(&A)";
            this.AddData.UseVisualStyleBackColor = true;
            this.AddData.Click += new System.EventHandler(this.AddData_Click);
            // 
            // nameDB
            // 
            this.nameDB.Location = new System.Drawing.Point(105, 37);
            this.nameDB.Name = "nameDB";
            this.nameDB.Size = new System.Drawing.Size(167, 19);
            this.nameDB.TabIndex = 4;
            this.nameDB.TextChanged += new System.EventHandler(this.nameDB_TextChanged);
            // 
            // nameCSV
            // 
            this.nameCSV.Location = new System.Drawing.Point(105, 93);
            this.nameCSV.Name = "nameCSV";
            this.nameCSV.Size = new System.Drawing.Size(167, 19);
            this.nameCSV.TabIndex = 5;
            // 
            // EndSys
            // 
            this.EndSys.Location = new System.Drawing.Point(300, 315);
            this.EndSys.Name = "EndSys";
            this.EndSys.Size = new System.Drawing.Size(85, 21);
            this.EndSys.TabIndex = 6;
            this.EndSys.Text = "終了(&X)";
            this.EndSys.UseVisualStyleBackColor = true;
            this.EndSys.Click += new System.EventHandler(this.EndSys_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "DB名称";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 93);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "CSVファイル名";
            // 
            // nameClass
            // 
            this.nameClass.Location = new System.Drawing.Point(105, 118);
            this.nameClass.Name = "nameClass";
            this.nameClass.Size = new System.Drawing.Size(167, 19);
            this.nameClass.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 121);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "種別コード";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 149);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "系列";
            // 
            // nameSeries
            // 
            this.nameSeries.Location = new System.Drawing.Point(105, 149);
            this.nameSeries.Name = "nameSeries";
            this.nameSeries.Size = new System.Drawing.Size(167, 19);
            this.nameSeries.TabIndex = 12;
            // 
            // SerachInfo
            // 
            this.SerachInfo.Location = new System.Drawing.Point(104, 185);
            this.SerachInfo.Multiline = true;
            this.SerachInfo.Name = "SerachInfo";
            this.SerachInfo.Size = new System.Drawing.Size(167, 63);
            this.SerachInfo.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(25, 188);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 14;
            this.label5.Text = "検索条件";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(426, 369);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.SerachInfo);
            this.Controls.Add(this.nameSeries);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nameClass);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.EndSys);
            this.Controls.Add(this.nameCSV);
            this.Controls.Add(this.nameDB);
            this.Controls.Add(this.AddData);
            this.Controls.Add(this.Serach);
            this.Controls.Add(this.selectCSV);
            this.Controls.Add(this.selectDB);
            this.Name = "Form1";
            this.Text = "車輌管理システム";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button selectDB;
        private System.Windows.Forms.Button selectCSV;
        private System.Windows.Forms.Button Serach;
        private System.Windows.Forms.Button AddData;
        private System.Windows.Forms.TextBox nameDB;
        private System.Windows.Forms.TextBox nameCSV;
        private System.Windows.Forms.Button EndSys;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox nameClass;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox nameSeries;
        private System.Windows.Forms.TextBox SerachInfo;
        private System.Windows.Forms.Label label5;
    }
}

