//----------------------------------------------------------------------
// (C) Copyright Mohayuni All rights reserved.
//----------------------------------------------------------------------
// <Module Name> Windows 汎用関数テストメイン
//----------------------------------------------------------------------
// <File Name>   Program.cs
//----------------------------------------------------------------------
// <Description>
//   _comsrcのテストメイン
// <History>
//	 2009.08.12	typed
//----------------------------------------------------------------------
// <Notes>
//   .
//----------------------------------------------------------------------

//----------------------------------------------------------------------
// usingディレクティブ宣言
//----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace Comsrc
{
	class tp_comsrc
	{
		static	private uint debugFlag = 0xffffffff;
		//----------------------------------------------------------------------
		// メソッド: sbChkArg
		//----------------------------------------------------------------------
		// Summary :
		//   起動引数の取得
		// OutLine :
		//   起動引数を取得
		// Return  :
		//   TRUE	起動引数は取得できた
		//   FALSE	最低一つの不正な引数があった
		// Notes   :
		// History :
		//   2009.08.12	typed
		//----------------------------------------------------------------------
		static	private		bool sbCheckArg(string[] args)
		{
			int _ii;
			string	_wkStr;
			for (_ii = 0; _ii < args.Length; _ii++)
			{
				if (args[_ii].StartsWith("/D:") == true)
				{
					_wkStr = args[_ii].Remove(0, args[_ii].LastIndexOf(':')+1);
					debugFlag = uint.Parse(_wkStr, System.Globalization.NumberStyles.HexNumber);
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
			return (true);
		}
		static	void Main(string[] args)
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
			_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "数字 {0:x}h, 文字列{1}\r\n", 256, "\ttestErr");
			_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestWrn, "数字 {0:x}h, 文字列{1}\r\n", 256, "\ttestWrn");
			_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestInf, "数字 {0:x}h, 文字列{1}\r\n", 256, "\ttestInf");
			_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestMon, "数字 {0:x}h, 文字列{1}\r\n", 256, "\ttestMon～");
			_com_vdbgo.vDbgoVerbose(_com_vdbgo.DebugErr, "数字 {0:x}h, 文字列{1}\r\n", 256, "\tDebugErr～");

			_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "数字 {0:x}h, 文字列{1}\r\n", 256, "\ttestErr-2");

			for (int _ii = 0; _ii < 100; _ii++)
			{
				_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "cLogSize = " + _ii.ToString() + "\r\n");
				Thread.Sleep(500);
			}


#if NOP
			_com_log clogSize = new _com_log("logSize_",".log","f:\\tk\\Comsrc\\log", Comsrc._com_log.LogOptionSize, 1024);
			_com_log clogLine = new _com_log("logLine_", "log", "f:\\tk\\Comsrc\\log\\", Comsrc._com_log.LogOptionLine, 10);
			_com_log clogDay = new _com_log("logDay_", "log", "f:\\tk\\Comsrc\\log", Comsrc._com_log.LogOptionDay, 10);
			_com_log clogNone = new _com_log("logNon_", "log", "f:\\tk\\Comsrc\\log", Comsrc._com_log.LogOptionNone, 10);

			System.Console.WriteLine("Hello World!");
			_com_vdbgo.vDbgoVerbose(0xffffffff, "数字 {0:d}, 文字列{1}\r\n", 100, "text");
			Thread.Sleep(100);
			_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "数字 {0:x}h, 文字列{1}\r\n", 256, "\ttestErr");
			_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestWrn, "数字 {0:x}h, 文字列{1}\r\n", 256, "\ttestWrn");
			_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestInf, "数字 {0:x}h, 文字列{1}\r\n", 256, "\ttestInf");
			_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestMon, "数字 {0:x}h, 文字列{1}\r\n", 256, "\ttestMon～");
			_com_vdbgo.vDbgoVerbose(_com_vdbgo.DebugErr, "数字 {0:x}h, 文字列{1}\r\n", 256, "\tDebugErr～");

			clogSize.vWrtiteLog("clogSize\r\n", true);
			clogLine.vWrtiteLog("clogLine\r\n", true);
			clogDay.vWrtiteLog("clogDay\r\n", true);
			clogNone.vWrtiteLog("clogNone\r\n", true);

			clogSize.vWrtiteLog("clogSize 2CRLF\n", false);
			clogSize.vWrtiteLog("clogSize 3\rLF", false);
			clogSize.vWrtiteLog("clogSize 4\nCR", false);
			clogLine.vWrtiteLog("clogLine 2テスト", false);
			clogDay.vWrtiteLog("clogDay 2テスト", false);
			clogNone.vWrtiteLog("clogNone 2テスト", false);

			_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestMon, "clogLine test\r\n");
			clogLine.vWrtiteLog("clogLine0\r\n", true);
			clogLine.vWrtiteLog("clogLine1\r\n", true);
			clogLine.vWrtiteLog("clogLine2\r\n\r\n", true);
			clogLine.vWrtiteLog("clogLine3\n\r\n", true);
			clogLine.vWrtiteLog("clogLine4\r", true);

			_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestMon, "clogSize test\r\n");
			for (int _ii = 0; _ii < 100; _ii++)
			{
				clogSize.vWrtiteLog("cLogSize = " + _ii.ToString() + "\r\n", true);
			}
#endif
		}
	}
}
