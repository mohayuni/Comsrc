//----------------------------------------------------------------------
// (C) Copyright Mohayuni All rights reserved.
//----------------------------------------------------------------------
// <Module Name> Windows 汎用関数テストメイン
//----------------------------------------------------------------------
// <File Name>	Program.cs
//----------------------------------------------------------------------
// <Description>
//	特定のCSVファイルをSqlite3DBへ保存する。
// <History>
//	 2015.12.27	typed
//----------------------------------------------------------------------
// <Notes>
//	.
//----------------------------------------------------------------------

//----------------------------------------------------------------------
// usingディレクティブ宣言
//----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Comsrc;
using ReadCsv;

namespace CsvToSqlite
{
	class CsvToDb
	{
		static private uint debugFlag = 0xffffffff;
		static private string strCsvFileName = null;
		static private _ReadCsv cReadCsv;

		//----------------------------------------------------------------------
		// メソッド: sbChkArg
		//----------------------------------------------------------------------
		// Summary :
		//	起動引数の取得
		// OutLine :
		//	起動引数を取得
		// Return	:
		//	TRUE	起動引数は取得できた
		//	FALSE	最低一つの不正な引数があった
		// Notes	:
		// History :
		//	2015.12.27	typed
		//----------------------------------------------------------------------
		static private bool sbCheckArg(string[] args)
		{
			int _ii;
			string _wkStr;
			for (_ii = 0; _ii < args.Length; _ii++)
			{
				if (args[_ii].StartsWith("/D:") == true)
				{
					_wkStr = args[_ii].Remove(0, args[_ii].LastIndexOf(':') + 1);
					debugFlag = uint.Parse(_wkStr, System.Globalization.NumberStyles.HexNumber);
				}
				if (args[_ii].StartsWith("/F:") == true)
				{
					_wkStr = args[_ii].Remove(0, args[_ii].LastIndexOf(':') + 1);
					strCsvFileName = _wkStr;
				}
#if NOP
				else if()
				{
				}
#endif
				else
				{
				}
			}
			//	引数の有効性をチェック
			if (strCsvFileName == null)
			{
				_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "対象ファイルが指定されていません\r\n");
				return (false);
			}	
			return (true);
		}

		//----------------------------------------------------------------------
		// メソッド: Main
		//----------------------------------------------------------------------
		// Summary :
		//	指定CSVファイルデータをSqliteデータベースに保存する。
		// OutLine :
		//	指定CSVファイルデータを読出し、構造体に格納
		//	格納したデータをSqliteデータベースに格納する。
		// Return	:
		//	TRUE	DBへの保存成功
		//	FALSE	最低一つの不正な引数があった
		// Notes	:
		// History :
		//	2009.08.12	typed
		//----------------------------------------------------------------------
		static void Main(string[] args)
		{
			if (sbCheckArg(args) == false) return;

			_com_vdbgo.vDbgoInit(debugFlag);	//	_com_vdbgoはstaticクラス

			//	エラーログクラスの作成
			_com_log clogErr = new _com_log("Err", "log", "f:\\work\\tk\\Comsrc\\log", Comsrc._com_log.LogOptionDay, 30);
			//	動作ログクラスの作成
			_com_log clogOpe = new _com_log("Operation", "log", "f:\\work\\tk\\Comsrc\\log", Comsrc._com_log.LogOptionDay, 10);

			//	ログクラスの登録
			_com_vdbgo.vDbgoLogIf(clogErr.vWrtiteLog, _com_vdbgo.DebugErr);
			_com_vdbgo.vDbgoLogIf(clogOpe.vWrtiteLog, _com_vdbgo.DebugMon);

			_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestMon, "変換ファイル {0}\r\n", strCsvFileName);

			cReadCsv = new _ReadCsv(strCsvFileName);

			for (;;)
			{
				if (cReadCsv._ReadOneLineData() == false) break;
            }
		}
	}
}
