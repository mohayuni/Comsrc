//----------------------------------------------------------------------
// (C) Copyright Mohayuni All rights reserved.
//----------------------------------------------------------------------
// <Module Name> DB制御クラス
//----------------------------------------------------------------------
// <File Name>	CtlDb.cs
//----------------------------------------------------------------------
// <Description>
//	車輛履歴DBアクセスクラス
// <History>
//	2015.12.30 typed
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

namespace CtlDb
{

	class _CtlDb 
	{
		//-----定数定義--------------------------------------------------------------------
		//		DB情報定義
		//		履歴コード定義
		public const string cstrShinsei = "新製";
		public const string cstrKaizou = "改造";
        public const string cstrKaiban = "改番";
		public const string cstrTenzoku = "転属";
		public const string cstrHaisha = "廃車";
		public const string cstrKaizouHaisha = "改造廃車";

		//-----クラスの定義--------------------------------------------------------------------
		/// <summary>
		/// 車輛固有情報クラス
		public class _cCarriageData
		{
			public string strClass=null;//	種別コード	(EC/DC/PC...)
			public string strSeriese = null;//	系列名	(101系/103系...)
			public string strForm=null; //	型式		(モハ101/クハ101...)
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
			public string strSummary=null;  //	概要
		//	public string strClass=null;	//	改造後種別コード(EC/DC/PC...)
		//	public string strSeriese=0;		//	改造後系列番号	(101/103...)
			public string strForm=null;     //	改造後/前型式	(モハ101/クハ101...)
			public int iSerial=0;			//	改造後/前車号	(1/1001...)
			
		}

		//-----プロパティの定義--------------------------------------------------------------------
		public _cCarriageData cCarriageData
		{
			get;
			set;
		}
		public	_cHistoryData[] cHistoryData
		{
			get;
			set;

		}
		//-----メンバー変数定義--------------------------------------------------------------------

		//--------------------------------------------------------------------------------
		/// <summary>
		///		_CtlDb	コンストラクタ
		///		Notes	:
		///			既定のDBをオープンする。
		///		History :			
		///			2015.12.30 Mohayuni
		/// </summary>
		public _CtlDb(
		)
		{
            try
			{
			}
			catch (InvalidCastException except)
			{
			}	

			return;
		}
#if NOP

		//--------------------------------------------------------------------------------
		/// <summary>
		///		~_CtlDb	デストラクタ
		///		Notes	:
		///			指定されたファイル名をクローズする、
		///		History :			
		///			2015.12.27 Mohayuni
		/// </summary>
		/// 
		~_CtlDb(
		)
		{
			
			return;
		}
#endif
		//--------------------------------------------------------------------------------
		/// <summary>
		///		_WriteCarriageInfo	車輛固有情報
		///		Notes	:
		///			車輛固有情報をDBに書き込む
		///		History :			
		///			2015.12.27 Mohayuni
		/// </summary>
		/// <param name="_cCarriageData">		車輛固有情報クラス</param>
		/// 
		public bool _WriteCarriageInfo(
				_cCarriageData	cCarData
        )
		{
			_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestMon, "_cCarriageData\r\n");
			_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestMon, "　　strClass = {0}\r\n", cCarData.strClass);
			_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestMon, "　　strSeriese = {0}\r\n", cCarData.strSeriese);
			_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestMon, "　　strForm = {0}\r\n", cCarData.strForm);
			_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestMon, "　　iSerial = {0}\r\n", cCarData.iSerial);
			return (true);
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		///		_WriteHistoryInfo	履歴情報
		///		Notes	:
		///			履歴情報をDBに書き込む
		///		History :			
		///			2016.1.1 Mohayuni
		/// </summary>
		/// <param name="_cCarriageData">		車輛固有情報クラス</param>
		/// 
		public bool _WriteHistoryInfo(
				_cHistoryData cHistoryData
		)
		{
			_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestMon, "_cHistoryData\r\n");
			_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestMon, "　　iCarriageNumber = {0}\r\n", cHistoryData.iCarriageNumber);
			_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestMon, "　　strCode = {0}\r\n", cHistoryData.strCode);
			_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestMon, "　　iYear = {0}\r\n", cHistoryData.iYear);
			_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestMon, "　　iMonth = {0}\r\n", cHistoryData.iMonth);
			_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestMon, "　　iDay = {0}\r\n", cHistoryData.iDay);
			_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestMon, "　　strPlace = {0}\r\n", cHistoryData.strPlace);
			_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestMon, "　　strFactory = {0}\r\n", cHistoryData.strFactory);
			_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestMon, "　　strSummary = {0}\r\n", cHistoryData.strSummary);
			_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestMon, "　　strForm = {0}\r\n", cHistoryData.strForm);
			_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestMon, "　　iSerial = {0}\r\n", cHistoryData.iSerial);

			return (true);
		}

	}   //	end class	_CtlDb
}	//	end namespace	CtlDb


