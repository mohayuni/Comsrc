//----------------------------------------------------------------------
// (C) Copyright Mohayuni All rights reserved.
//----------------------------------------------------------------------
// <Module Name> GUIメイン画面クラス
//----------------------------------------------------------------------
// <File Name>	Form1.cs
//----------------------------------------------------------------------
// <Description>
//	DB操作GUIメインダイアログ
// <History>
//	2017.02.04 typed
//----------------------------------------------------------------------
// <Notes>
//　 
//----------------------------------------------------------------------/

//----------------------------------------------------------------------
//	＃ｄｅｆｉｎｅ定義													
//----------------------------------------------------------------------
//#define	NOP_FUNC

//----------------------------------------------------------------------
// usingディレクティブ宣言
//----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using Comsrc;    //	Sqlite DB3アクセスクラス
using CtlDb;    //	Sqlite DB3アクセスクラス
using ReadCsv;	//	

namespace ManRailCar
{
    public partial class MainForm : Form
    {
        private const string DebugTest = "test";

        //--------------------------------------------------------------------------------
        /// <summary>
        ///		Form1	コンストラクタ
        ///		Notes	:
        ///			フォームクラスの初期化。
        ///		History :			
        ///			2017.02.04 Mohayuni
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
		}

        //--------------------------------------------------------------------------------
        /// <summary>
        ///		selectCSV_Click	CSV選択ボタン押下
        ///		Notes	:
        ///			nameCSVで指定されたDBをオープンする
        ///		History :			
        ///			2018.04.17 Mohayuni
        /// </summary>
        /// <param name="object">	オブジェクト</param>
        /// <param name="EventArgs">	イベント引数</param>
		private void selectCSV_Click(object sender, EventArgs e)
        {
			//	コモンダイアログを開き、CSVファイル名を取得する。
			OpenFileDialog cCsvDialog = new OpenFileDialog();

			cCsvDialog.InitialDirectory = Application.StartupPath;
			if (cCsvDialog.ShowDialog() != DialogResult.OK)
				return; //	CSVファイルは選択されていない。
			this.nameCSV.Text = cCsvDialog.FileName;
		}

		private void EndSys_Click(object sender, EventArgs e)
        {
			this.Close();
        }

        //--------------------------------------------------------------------------------
        /// <summary>
        ///		selectDB_Click	DB選択ボタン押下
        ///		Notes	:
        ///			nameDBで指定されたDBをオープンする
        ///		History :			
        ///			2018.02.01 Mohayuni
        /// </summary>
        /// <param name="object">	オブジェクト</param>
        /// <param name="EventArgs">	イベント引数</param>
        private void selectDB_Click(object sender, EventArgs e)
        {
			//	コモンダイアログを開き、DBファイル名を取得する。
			OpenFileDialog cDbDialog = new OpenFileDialog();

			cDbDialog.CheckFileExists = false;
			cDbDialog.InitialDirectory = Application.StartupPath;
            cDbDialog.Filter = "DB ファイル（*.db)|*.db";
            cDbDialog.Title = "DBファイルを選択/新規の場合ファイル名を入力";
            //  存在しないファイル=新規ファイルとして扱う
            //　ファイル名がブランクの場合はダイアログで「開く」が押下出来ない
            //  キャンセルの場合は、DialogResultがOKにならない。
            cDbDialog.CheckFileExists = false;
            if (cDbDialog.ShowDialog() != DialogResult.OK)
				return;	//	DBファイルは選択されていない。
			this.nameDB.Text = cDbDialog.FileName;

			this.selectCSV.Enabled = true;
			this.AddData.Enabled = true;
			this.Serach.Enabled = true;
		}

        private void Serach_Click(object sender, EventArgs e)
        {
			//           MessageBox.Show("Serach Test");  
			EditForm EditForm = new EditForm();
			EditForm.Show();
        }

   

        //--------------------------------------------------------------------------------
        /// <summary>
        ///		AddData_Click	データ追加ボタン押下
        ///		Notes	:
        ///			指定データベースにCSVを追加する
        ///		History :			
        ///			2018.04.18 Mohayuni
        /// </summary>
        /// <param name="object">	オブジェクト</param>
        /// <param name="EventArgs">	イベント引数</param>
        private void AddData_Click(object sender, EventArgs e)
        {
			_CtlDb cMainCtlDb = new _CtlDb(this.nameDB.Text);

			_ReadCsv cReadCsv = new _ReadCsv(this.nameCSV.Text, this.nameClass.Text, this.nameSeries.Text, cMainCtlDb);

			cMainCtlDb._BeginInsert();
			progressBarExecute.Value = 0;

			for (int iLine = 0; ; iLine++)
			{
				if (cReadCsv._ReadOneLineData() == false) break;
				progressBarExecute.Value = iLine* progressBarExecute.Maximum / cReadCsv.m_iLineCount;
			}
			progressBarExecute.Value = progressBarExecute.Maximum;
			cMainCtlDb._EndInsert();

		}

		private void nameDB_TextChanged(object sender, EventArgs e)
        {

        }

		private void MainForm_Load(object sender, EventArgs e)
		{

		}
#if NOP
		private void label6_Click(object sender, EventArgs e)
		{

		}
#endif
	}
}
