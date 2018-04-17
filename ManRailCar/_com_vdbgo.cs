//----------------------------------------------------------------------
// (C) Copyright Mohayuni All rights reserved.
//----------------------------------------------------------------------
// <Module Name> 汎用デバック出力関数
//----------------------------------------------------------------------
// <File Name>   _com_vdbgo.cs
//----------------------------------------------------------------------
// <Description>
//   デバッグ出力処理
// <History>
//   2008.08.14 from vdbgo.cppより
//----------------------------------------------------------------------
// <Notes>
//　 本クラスはstaticなクラスです。
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

public delegate void vWriteLogDelegate(string _logStr, bool _AddDate);

namespace Comsrc
{

	static	class _com_vdbgo
	{
		//-----定数定義--------------------------------------------------------------------
		//	デバックフラグの定義
		//	出力レベルと、フラグの双方がONで出力される
		//	一方、双方ががOFFでは出力されない
		//	出力レベル
		private const	uint	debugErr	= 0x80000000;
		static	public uint DebugErr { get { return debugErr; } }
		private const uint debugWarn = 0x40000000;
		static public uint DebugWarn { get { return debugWarn; } }
		private const uint debugInfo = 0x20000000;
		static public uint DebugInfo { get { return debugInfo; } }
		private const uint debugMon = 0x10000000;
		static public uint DebugMon { get { return debugMon; } }
		private const uint debugLevel = 0xf0000000;
		//	出力フラグ
		//	namespace Comsrc内
		private const uint debugComsrc = 0x00000001;		//	test用
		static public uint ComsrcErr { get { return debugComsrc | debugErr; } }
		static public uint ComsrcWrn { get { return debugComsrc | debugWarn; } }
		static public uint ComsrcInf { get { return debugComsrc | debugInfo; } }
		static public uint ComsrcMon { get { return debugComsrc | debugMon; } }
		//	テストメイン用
		private const uint debugTest = 0x00000002;		//	test用
		static public uint TestErr { get { return debugTest | debugErr; } }
		static public uint TestWrn { get { return debugTest | debugWarn; } }
		static public uint TestInf { get { return debugTest | debugInfo; } }
		static public uint TestMon { get { return debugTest | debugMon; } }
		//	以下bit2～bit23の任意ビットをONし、定義することで、デバック出力フラグ
		//	レベルを定義することが可能　（以下のEtc1を部分を変更する。）
		private const uint debugEtc2 = 0x00000004;
		static	public uint Etc2Err { get { return debugEtc2 | debugErr; } }
		static	public uint Etc2Wrn { get { return debugEtc2 | debugWarn; } }
		static	public uint Etc2Inf { get { return debugEtc2 | debugInfo; } }
		static	public uint Etc2Mon { get { return debugEtc2 | debugMon; } }

		public const uint debugKind = 0x0fffffff;

		//	_com_logクラスvWriteLogを呼び出すDeletgate
		static	private	vWriteLogDelegate vWriteErrLog = null;
		static	private	vWriteLogDelegate vWriteWrnLog = null;
//		static	private	vWriteLogDelegate vWriteInfLog = null;
		static	private	vWriteLogDelegate vWriteMonLog = null;

		//-----プロパティの定義--------------------------------------------------------------------
		static private uint debugMode = 0xffffffff;	//	デバックモード
//		static	private string ipAddress = "0.0.0.0";		//	リモートデバック出力
//		private Trace cTrace;
		static private ConsoleTraceListener conTraceListener = null;
//		static private Stream p_StreamErrLog = null;

		//-----メソッドの定義--------------------------------------------------------------------
		/// <summary>
		///		svdbgo
		///		デバッグ出力初期化
		///		Notes   :
		///			ui_DEBUGModeは0xffffffffとなる
		///		History :
		///			2009.08.12 Mohayuni
		/// </summary>
		/// <param name="_debugModePrm"></param>
		static	private void vdbgo(
			uint _debugModePrm			//	デバックモード（レベル＋種別）
			)
		{
			debugMode = _debugModePrm;	//	デバックレベルの保存
			if (conTraceListener == null)	//	コンソールがListenerに登録されていないので追加
			{
				conTraceListener = new ConsoleTraceListener();
				Trace.Listeners.Add(conTraceListener);
			}
			System.Console.WriteLine("class vdbgo ({0:X}) ", debugMode);
		}
		//--------------------------------------------------------------------------------
		/// <summary>
		///		vDbgoInit()
		///		デバッグ出力初期化
		///		Notes   :
		///			ui_DEBUGModeは指0xffffffffとなる
		///			本メソッドはスレッドセーフではない
		///		History :
		///			2009.08.12 Mohayuni
		/// </summary>
		static public void vDbgoInit()
		{
			vdbgo(debugMode);	//	ディフォルトで初期化
		}
		//--------------------------------------------------------------------------------
		/// <summary>
		///		vDbgoInit()
		///		デバッグ出力初期化(デバックモード（レベル＋種別）指定
		///		Notes   :
		///			ui_DEBUGModeは指0xffffffffとなる
		///			本メソッドはスレッドセーフではない
		///		History :
		///			2009.08.12 Mohayuni
		/// </summary>
		/// <param name="_debugMode"></param>
		static public void vDbgoInit(
			uint _debugMode		//	デバックモード（レベル＋種別）
			)
		{
			vdbgo(_debugMode);	//	引数で初期化
		}
		//--------------------------------------------------------------------------------
		/// <summary>
		///		vDbgoVerbose()
		///		冗長なデバッグ出力
		///		Notes   :
		///			debugModeできょかされたレベル種別のみ出力する。
		///			CR->CRLF変換、行末へのCRLFの付加は行わない。
		///		History :
		///			2009.08.12 Mohayuni
		/// 
		/// </summary>
		/// <param name="_iMode"></param>
		/// <param name="_strFormat"></param>
		/// <param name="_args"></param>
		static	public void vDbgoVerbose(
			uint _iMode,				//	デバッグ表示レベル _DEBUG_xyz定数がOR指定される
			string _strFormat,		//	可変引数のフォーマット
			params	object[] _args	//	可変引数	
			)
		{
			if ( ((_iMode & debugLevel & debugMode) != 0)	//	デバックレベルが一致しなければSKIP
				&& ((_iMode & debugKind & debugMode) != 0))
			{
				StackFrame _Sf = new StackFrame(1, true);	//	一つ前のスタックフレームを取り出し
				string _SourceFile = _Sf.GetFileName();	//	ファイル名の取得
				int _SourceLine = _Sf.GetFileLineNumber();//	行番号の取得
				DateTime _dtNow = DateTime.Now;
				
				//	デバック文字列の整形
				//	フルパスからファイル名だけを取り出す。（最後の￥から前を削除）
				_SourceFile = Path.GetFileName(_SourceFile);
				string _debugHeader = string.Format("{0:d2}/{1:d2}/{2:d2} {3:d2}:{4:d2}:{5:d2}.{6:d3} {7} {8:d4} : ",
														_dtNow.Year, _dtNow.Month, _dtNow.Day,
														_dtNow.Hour, _dtNow.Minute, _dtNow.Second,
														_dtNow.Millisecond,
														_SourceFile, _SourceLine);
				string _debugStr = string.Format(_strFormat, _args);
				_debugStr = _debugHeader + _debugStr;

				//	デバック出力
//				Trace.WriteLine(_debugStr);
				Trace.Write(_debugStr);
				//	ログファイルへの出力
				if ( ( (_iMode & debugErr) != 0) && (vWriteErrLog != null) )	vWriteErrLog(_debugStr, false);
				if ( ( (_iMode & debugWarn) != 0) && (vWriteWrnLog != null) )	vWriteWrnLog(_debugStr, false);
//				if ( ( (_iMode & debugInfo) != 0) && (vWriteInfLog != null) )	vWriteInfLog(_debugStr, false);
				if ( ( (_iMode & debugMon) != 0) && (vWriteMonLog != null) )	vWriteMonLog(_debugStr, false);
			}
		}
		/// <summary>
		///		vDbgoLogIf()
		///		_com_logクラスログファイル書き出しメソッドの登録
		///		Notes   :
		///			エラーレベル毎に登録可能とする
		///			debugInfoについては登録しない
		///			（_com_logクラスのデバックライトで使用する為）
		///		History :
		///			2009.08.12 Mohayuni
		/// </summary>
		/// <param name="_vWriteLog"></param>
		/// <param name="_errLevel"></param>
		static public void vDbgoLogIf(vWriteLogDelegate _vWriteLog, uint _errLevel)
		{
			switch (_errLevel)
			{
			case debugErr:
				vWriteErrLog = _vWriteLog;
				break;
			case debugWarn:
				vWriteWrnLog = _vWriteLog; 
				break;
			case debugInfo:
//				vWriteInfLog = _vWriteLog;
				break;
			case debugMon:
				vWriteMonLog = _vWriteLog;
				break;
			default:
				break;
			}
		}
	}	//	end class	_com_vdbgo
}	//	end namespace	Comsrc

