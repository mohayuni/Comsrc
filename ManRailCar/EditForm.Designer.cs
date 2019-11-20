namespace ManRailCar
{
	partial class EditForm
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
			this.saveDB = new System.Windows.Forms.Button();
			this.cancelEdit = new System.Windows.Forms.Button();
			this.dbGridView = new System.Windows.Forms.DataGridView();
			((System.ComponentModel.ISupportInitialize)(this.dbGridView)).BeginInit();
			this.SuspendLayout();
			// 
			// saveDB
			// 
			this.saveDB.Location = new System.Drawing.Point(713, 12);
			this.saveDB.Name = "saveDB";
			this.saveDB.Size = new System.Drawing.Size(75, 23);
			this.saveDB.TabIndex = 0;
			this.saveDB.Text = "保存(&S)";
			this.saveDB.UseVisualStyleBackColor = true;
			this.saveDB.Click += new System.EventHandler(this.saveDB_Click);
			// 
			// cancelEdit
			// 
			this.cancelEdit.Location = new System.Drawing.Point(713, 50);
			this.cancelEdit.Name = "cancelEdit";
			this.cancelEdit.Size = new System.Drawing.Size(75, 23);
			this.cancelEdit.TabIndex = 1;
			this.cancelEdit.Text = "キャンセル(&C)";
			this.cancelEdit.UseVisualStyleBackColor = true;
			this.cancelEdit.Click += new System.EventHandler(this.cancelEdit_Click);
			// 
			// dbGridView
			// 
			this.dbGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dbGridView.Location = new System.Drawing.Point(25, 25);
			this.dbGridView.Name = "dbGridView";
			this.dbGridView.RowTemplate.Height = 21;
			this.dbGridView.Size = new System.Drawing.Size(660, 294);
			this.dbGridView.TabIndex = 2;
			this.dbGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dbGridView_CellContentClick);
			// 
			// EditForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.dbGridView);
			this.Controls.Add(this.cancelEdit);
			this.Controls.Add(this.saveDB);
			this.Name = "EditForm";
			this.Text = "DB表示・編集fフォーム";
			this.Load += new System.EventHandler(this.EditForm_Load_1);
			((System.ComponentModel.ISupportInitialize)(this.dbGridView)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button saveDB;
		private System.Windows.Forms.Button cancelEdit;
		private System.Windows.Forms.DataGridView dbGridView;
	}
}