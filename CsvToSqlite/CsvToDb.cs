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
using CtlDb;

namespace CsvToSqlite
{
	class CsvToDb
	{
		//-----メンバー変数定義--------------------------------------------------------------------
		static private uint m_debugFlag = 0xffffffff;
		static private string m_strCsvFileName = null;
		static private string m_strDbFileName = null;
		static private _ReadCsv m_cReadCsv;
		static private string m_strClassName = null;	//	種別コード名称	(EC/DC/PC...)
		static private string m_strSeriese = null;		//	系列名称		(101系/103系...)
		static private _CtlDb m_cMyDb;                  //	使用するデータベースクラス

		//--------------------------------------------------------------------------------
		/// <summary>
		///		sbChkArg	引数の取得
		///		Notes	:
		///			引数をデコードし、内部メンバーに設定する。
		/// 		"/D:Debug Out Mode (Option)"
		///			"/F:CSVファイル名[必須]"
		///			"/T:種別コード(EC/DC/PC...)[必須]"
		///			"/S:系列名(101系/103系...)[必須]"
		///		History :			
		///			2015.12.27 Mohayuni
		/// </summary>
		/// <param name="string[] args">	引数</param>
		static private bool sbCheckArg(string[] args)
		{
			int _ii;
			string _wkStr;
			bool bRet = true;
			for (_ii = 0; _ii < args.Length; _ii++)
			{
				if (args[_ii].StartsWith("/D:") == true)
				{
					_wkStr = args[_ii].Remove(0, args[_ii].LastIndexOf(':') + 1);
					m_debugFlag = uint.Parse(_wkStr, System.Globalization.NumberStyles.HexNumber);
				}
				else if (args[_ii].StartsWith("/F:") == true)
				{
					_wkStr = args[_ii].Remove(0, args[_ii].LastIndexOf(':') + 1);
					m_strCsvFileName = _wkStr;
				}
				else if (args[_ii].StartsWith("/B:") == true)
				{
					_wkStr = args[_ii].Remove(0, args[_ii].LastIndexOf(':') + 1);
					m_strDbFileName = _wkStr;
				}
				else if (args[_ii].StartsWith("/T:") == true)
				{
					_wkStr = args[_ii].Remove(0, args[_ii].LastIndexOf(':') + 1);
					m_strClassName = _wkStr;
				}
				else if (args[_ii].StartsWith("/S:") == true)
				{
					_wkStr = args[_ii].Remove(0, args[_ii].LastIndexOf(':') + 1);
					m_strSeriese = _wkStr;
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
			if (m_strCsvFileName == null)
			{
				_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "対象ファイルが指定されていません\r\n");
				bRet = false;
			}
			if (m_strClassName == null)
			{
				_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "種別コード	(EC/DC/PC...)が指定されていません\r\n");
				bRet = false;
			}
			if (m_strSeriese == null)
			{
				_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "系列名	(101系/103系...)が指定されていません\r\n");
				bRet = false;
			}
			if (bRet == false)
			{
				Console.WriteLine("パラメータ");
				Console.WriteLine("/D:Debug Out Mode (Option)");
				Console.WriteLine("/F:CSVファイル名[必須]");
				Console.WriteLine("/B:DBファイル名[必須]");
				Console.WriteLine("/T:種別コード(EC/DC/PC...)[必須]");
				Console.WriteLine("/S:系列名(101系/103系...)[必須]");
			}
			return (bRet);
		}
		///--------------------------------------------------------------------------------
		/// <summary>
		///		Main	指定CSVファイルデータをSqliteデータベースに保存する
		///		Notes	:
		///			指定CSVファイルデータを読出し、Sqliteデータベースに格納する。
		///		History :			
		///			2015.12.27 Mohayuni
		/// </summary>
		/// <param name="string[] args">	引数</param>
		static void Main(string[] args)
		{
			if (sbCheckArg(args) == false) return;

			_com_vdbgo.vDbgoInit(m_debugFlag);	//	_com_vdbgoはstaticクラス

			//	エラーログクラスの作成
			_com_log clogErr = new _com_log("Err", "log", "f:\\work\\tk\\Comsrc\\log", Comsrc._com_log.LogOptionDay, 30);
			//	動作ログクラスの作成
			_com_log clogOpe = new _com_log("Operation", "log", "f:\\work\\tk\\Comsrc\\log", Comsrc._com_log.LogOptionDay, 10);

			//	ログクラスの登録
			_com_vdbgo.vDbgoLogIf(clogErr.vWrtiteLog, _com_vdbgo.DebugErr);
			_com_vdbgo.vDbgoLogIf(clogOpe.vWrtiteLog, _com_vdbgo.DebugMon);

			_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestMon, "変換ファイル {0}\r\n", m_strCsvFileName);

			m_cMyDb = new _CtlDb(m_strDbFileName);

			m_cReadCsv = new _ReadCsv(m_strCsvFileName, m_strClassName,m_strSeriese, m_cMyDb);

			m_cMyDb._BeginInsert();

            for (;;)
			{
				if (m_cReadCsv._ReadOneLineData() == false) break;
            }
			m_cMyDb._EndInsert();
		}
	}
}
