//----------------------------------------------------------------------
// (C) Copyright Mohayuni All rights reserved.
//----------------------------------------------------------------------
// <Module Name> DB表示・編集画面クラス
//----------------------------------------------------------------------
// <File Name>	Editform.cs
//----------------------------------------------------------------------
// <Description>
//	DB表示、編集画面
// <History>
//	2019.11.20 typed
//----------------------------------------------------------------------
// <Notes>
//　 
//----------------------------------------------------------------------/

//----------------------------------------------------------------------
//	＃ｄｅｆｉｎｅ定義													
//----------------------------------------------------------------------

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

namespace ManRailCar
{
	public partial class EditForm : Form
	{
		//--------------------------------------------------------------------------------
		/// <summary>
		///		Form1	コンストラクタ
		///		Notes	:
		///			DB表示・編集画面クラスの初期化。
		///		History :			
		///			2019.11.20 Mohayuni
		/// </summary>

		public EditForm()
		{
			InitializeComponent();
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		///		EditForm_Load_1	初期ロード
		///		Notes	:
		///			指定DBデータを表示する。。
		///		History :			
		///			2019.11.20 Mohayuni
		/// </summary>
		/// <param name="object">	オブジェクト</param>
		/// <param name="EventArgs">	イベント引数</param>
		private void EditForm_Load_1(object sender, EventArgs e)
		{

			// カラム数を指定
			dbGridView.ColumnCount = 4;

			// カラム名を指定
			dbGridView.Columns[0].HeaderText = "教科";
			dbGridView.Columns[1].HeaderText = "点数";
			dbGridView.Columns[2].HeaderText = "氏名";
			dbGridView.Columns[3].HeaderText = "クラス名";

			// データを追加
			dbGridView.Rows.Add("国語", 90, "田中　一郎", "A");
			dbGridView.Rows.Add("数学", 50, "鈴木　二郎", "A");
			dbGridView.Rows.Add("英語", 90, "佐藤　三郎", "B");
		}

	//--------------------------------------------------------------------------------
	/// <summary>
	///		saveDB_Click	保存ボタン押下
	///		Notes	:
	///			編集結果を保存する。
	///		History :			
	///			2019.11.20 Mohayuni
	/// </summary>
	/// <param name="object">	オブジェクト</param>
	/// <param name="EventArgs">	イベント引数</param>

		private void saveDB_Click(object sender, EventArgs e)
		{

		}

		//--------------------------------------------------------------------------------
		/// <summary>
		///		cancelEdit_Click	キャンセルボタン押下
		///		Notes	:
		///			編集結果を破棄する。
		///		History :			
		///			2019.11.20 Mohayuni
		/// </summary>
		/// <param name="object">	オブジェクト</param>
		/// <param name="EventArgs">	イベント引数</param>
		private void cancelEdit_Click(object sender, EventArgs e)
		{

		}

		private void dbGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{

		}

	}
}
