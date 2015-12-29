//----------------------------------------------------------------------
// (C) Copyright Mohayuni All rights reserved.
//----------------------------------------------------------------------
// <Module Name> Csv読込クラス
//----------------------------------------------------------------------
// <File Name>	ReadCsv.cs
//----------------------------------------------------------------------
// <Description>
//	特定のCSVファイルを読み込み内部クラスに保持する。
// <History>
//	2015.12.27 typed
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
using System.Linq;
using System.Text;
using System.IO;			//	Stream
using System.Diagnostics;	//	CallStack
using Comsrc;

namespace ReadCsv
{

	class _ReadCsv 
	{
		//-----定数定義--------------------------------------------------------------------
		//		出力フラグ

		//-----クラスの定義--------------------------------------------------------------------
		/// <summary>
		/// 車輛固有情報クラス
		public class _cCarriageData
		{
			public string strClass=null;//	種別コード	(EC/DC/PC...)
			public int iSeriese=0;		//	系列番号	(101系/103系...)
			public string strForm=null;	//	型式		(モハ/クハ...)
			public int iSerial=0;		//	車号		(1/1001...)
		}
		/// <summary>
		/// 履歴情報クラス
		public class _cHistoryData
		{
			public int iCarriageNumber=0;	//	車輛テーブルのROWID
			public string strCode=null;		//	履歴コード	(改番、改造、新製、廃車...)
			public int iYear=0;				//	履歴年
			public int iMonth=0;			//	履歴月
			public int iDay=0;				//	履歴日
			public string strPlace=null;	//	所属区
			public string strFactory=null;	//	施工場所
			public string strSummary=null;	//	概要
		//	public string strClass=null;	//	改造後種別コード(EC/DC/PC...)
		//	public int iSeriese=0;			//	改造後系列番号	(101系/103系...)
			public string strForm=null;		//	改造後/前型式	(モハ/クハ...)
			public int iSerial=0;			//	改造後/前車号	(1/1001...)
			
		}

		//-----プロパティの定義--------------------------------------------------------------------
		public _cCarriageData cCarriageData
		{
			get;
			private set;
		}
		public _cHistoryData cHistoryData
		{
			get;
			private set;
		}

		//-----メンバー変数定義--------------------------------------------------------------------
		private string m_strFull;   //	指定CSVファイル全体を読み込んだ文字列
		private System.IO.StringReader m_streaderFull;  //	m_strFullにアクセスする為のStringReader構造体

		//--------------------------------------------------------------------------------
		/// <summary>
		///		_ReadCsv	コンストラクタ
		///		Notes	:
		///			指定されたファイル名をオープンする、
		///		History :			
		///			2015.12.27 Mohayuni
		/// </summary>
		/// <param name="strFileName"></param>
		public _ReadCsv(
			string strFileName	//	対象ファイル名
		)
		{
			try
			{
				using (System.IO.StreamReader cCsvReader = new System.IO.StreamReader(strFileName, System.Text.Encoding.GetEncoding("shift_jis")))
				{
					m_strFull = cCsvReader.ReadToEnd();
					m_streaderFull = new StringReader(m_strFull);
                }

			}
			catch (InvalidCastException except)
			{
				_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "文字列{0} 読込エラー ({1})\r\n", 256, strFileName, except.Message);
			}	

			return;
		}
#if NOP

		//--------------------------------------------------------------------------------
		/// <summary>
		///		~_ReadCsv	デストラクタ
		///		Notes	:
		///			指定されたファイル名をクローズする、
		///		History :			
		///			2015.12.27 Mohayuni
		/// </summary>
		/// 
		~_ReadCsv(
		)
		{
			
			return;
		}
#endif       
		//--------------------------------------------------------------------------------
		/// <summary>
		///		_ReadOneLineData	1ライン読込
		///		Notes	:
		///			１ラインのデータのCSVデータを読み取る
		///		History :			
		///			2015.12.27 Mohayuni
		/// </summary>
		/// <param name="strFileName"></param>
		/// 
		public bool	_ReadOneLineData(
        )
		{
			//	一行分のデータを取得
			string strOneLine = m_streaderFull.ReadLine();
			if (strOneLine == null)
			{   //	EOF
				return (false);
			}

			//	','区切りで分割
			string[] straData = strOneLine.Split(',');

			//	debug
			_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "[{0}] Item数 {1}\r\n", strOneLine, straData.Length);


			// データを確認する
			int ii = 0;
			foreach (string stData in straData)
			{
				ii++;
				_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "   {0}: {1}\r\n", ii, stData);
			}
#if NOP
			_cCarriageData cCarData = new _cCarriageData();
			cCarData.strClass = "EC";
			cCarData.iSeriese = 101;
			cCarData.strForm = "クモハ";
			cCarData.iSerial = 2;
#endif
			return (true);
		}

	}   //	end class	_ReadCsv
}	//	end namespace	ReadCsv


