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
using CtlDb;	//	Sqlite DB3アクセスクラス

namespace ManRailCar
{
    public partial class MainForm : Form
    {
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

        private void selectCSV_Click(object sender, EventArgs e)
        {

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

			cDbDialog.InitialDirectory = Application.StartupPath;
            if (cDbDialog.ShowDialog() != DialogResult.OK)
				return;	//	DBファイルは選択されていない。
			this.nameDB.Text = cDbDialog.FileName;
			_CtlDb MainCtlDb = new _CtlDb(this.nameDB.Text);

			this.selectCSV.Enabled = true;
			this.AddData.Enabled = true;
			this.Serach.Enabled = true;
		}

        private void Serach_Click(object sender, EventArgs e)
        {

        }

        private void AddData_Click(object sender, EventArgs e)
        {

        }

        private void nameDB_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
