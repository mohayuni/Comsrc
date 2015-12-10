//----------------------------------------------------------------------
// (C) Copyright Mohayuni All rights reserved.
//----------------------------------------------------------------------
// <Module Name> LOGファイル作成クラス
//----------------------------------------------------------------------
// <File Name>   _com_log.cs
//----------------------------------------------------------------------
// <Description>
//   ログファイルを作成する
// <History>
//   2008.08.14 from clog.cppより
//----------------------------------------------------------------------
// <Notes>
//	 以下のオプションがある
//	　・ログは日単位で作成、指定日経過後のファイルは自動削除
//    ・ログはサイズ単位で作成、自動削除は無し
//　　・ログは行数単位で作成、自動削除は無し
//    ・使用可能な_com_vdbgoのエラーレベルはComsrcInfのみとする
//----------------------------------------------------------------------/

//----------------------------------------------------------------------
//	＃ｄｅｆｉｎｅ定義													
//----------------------------------------------------------------------

//----------------------------------------------------------------------
// usingディレクティブ宣言
//----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;			//	Stream
using System.Diagnostics;	//	CallStack
using System.Threading;		//	Semaphore

namespace Comsrc
{

	class _com_log
	{
		//-----定数定義--------------------------------------------------------------------
		private const int logOptionNone = 0;
		static	public int LogOptionNone { get { return logOptionNone; } }
		private const int logOptionDay = 1;
		static public int LogOptionDay { get { return logOptionDay; } }
		private const int logOptionSize = 2;
		static public int LogOptionSize { get { return logOptionSize; } }
		private const int logOptionLine = 3;
		static public int LogOptionLine { get { return logOptionLine; } }

		//-----プロパティの定義--------------------------------------------------------------------
		private string baseName = "";		//	基本ログファイル名
		private string extName = "";		//	基本ログファイル拡張子名
		private string dirName = "";		//	ログディレクトリ
		private int option = logOptionNone;	//	作成モード	
		private int optionPrm = 0;			//	オプションパラメータ
											//	logOptionDay..　ログ保持日数
											//	LogOptionSize..	ログファイルサイズ
											//	LogOptionLine..	ログライン数
		private DateTime preCheckDate = DateTime.FromBinary(0);
											//	直前のログチェック日時
		private long	serialNum = 0;		//	LogOptionSize/LogOptionLineのシリアル番号
		private long	logFileSize = 0;	//	LogOptionSizeのファイルサイズ
		private int		logFileLine = 0;	//	LogOptionLineのライン数
		private StreamWriter	logFileStream = null;
											//	ログファイルのストリームライトクラス
		private Semaphore semLogFile = null;//	ファイルアクセスへのセマフォークラス

		//-----メソッドの定義--------------------------------------------------------------------
		/// <summary>
		///		checkAndDeleteLog
		///		古い日付、不正な日付のログファイル削除
		///		History :
		///			2009.08.14 SKYware
		/// </summary>
		/// <param name="CurLogName"></param>
		private void checkAndDeleteLog(
			string	CurLogName	//　現在のログファイル名
			)
		{
			DateTime _dtNow = DateTime.Now;	//	現在日時
			//	前回のチェックより日付が変わっているかを確認
			//	変わっていなければ、チェックは不要
			if ((_dtNow.Year == preCheckDate.Year)
				&& (_dtNow.Month == preCheckDate.Month)
				&& (_dtNow.Day == preCheckDate.Day))
				return;
			//	不要となる日付を決める。これ以前の日付ログは削除
			DateTime _dtLimit = _dtNow.AddDays(-optionPrm);

			//	比較用のファイル名を作成
			string	_LimitFilleName = string.Format("{0}{1:d4}{2:d2}{3:d2}{4}{5}",
										dirName, _dtLimit.Year, _dtLimit.Month, _dtLimit.Day,
										baseName, extName);

			//	ディレクトリからファイル名を取得
			string[] _files = Directory.GetFiles(dirName);
			foreach (string _pathFileName in _files)
			{
				//	基本ログファイル名を含まない場合は無視する
				if (_pathFileName.IndexOf(baseName) == -1) continue;
				//	古いログは削除
				if (String.Compare(_pathFileName, _LimitFilleName) <= 0)
				{
					_com_vdbgo.vDbgoVerbose(_com_vdbgo.ComsrcInf, "Delete Old Log= {0}\r\n", _pathFileName);
					File.Delete(_pathFileName);
				}
				//	未来のログは無条件に削除
				if (String.Compare(_pathFileName, CurLogName) > 0)
				{
					_com_vdbgo.vDbgoVerbose(_com_vdbgo.ComsrcInf, "Delete too New Log= {0}\r\n", _pathFileName);
					File.Delete(_pathFileName);
				}
			}
			//	チェック日付を保存
			preCheckDate = _dtNow;
		}
		//--------------------------------------------------------------------------------
		/// <summary>
		///		getLastLogInfo
		///		最終ログ情報の取得
		///		History :
		///			2009.08.12 SKYware
		/// </summary>
		private void getLastLogInfo()
		{
			//	ディレクトリからファイル名を取得
			string[] _files = Directory.GetFiles(dirName);
			string _maxSerialFileName = null;

			foreach(string pathFileName in _files)
			{
				//	シリアル番号を取得　最大のシリアル番号のファイル名を保存
				string	_fileName = Path.GetFileName(pathFileName);
				//_com_vdbgo.vDbgoVerbose(_com_vdbgo.ComsrcInf, "{0}\r\n", _fileName);
				if (_fileName.StartsWith(baseName) == true)
				{
					string	_wkStr = _fileName.Remove(0, baseName.Length);
					_wkStr = _wkStr.Remove(_wkStr.IndexOf('.'));
					long	_number = long.Parse(_wkStr, System.Globalization.NumberStyles.Number);
//					//	_com_vdbgo.vDbgoVerbose(_com_vdbgo.ComsrcInf, "シリアル = {0:d}\r\n", _number);
					if (serialNum < _number)
					{
						serialNum = _number;
						_maxSerialFileName = pathFileName;
					}
				}
			}
			if (serialNum > 0)
			{
//				_com_vdbgo.vDbgoVerbose(_com_vdbgo.ComsrcInf, "シリアル = {0} {1:d}\r\n", _maxSerialFileName, serialNum);
				switch (option)
				{
				case	logOptionSize:
					//	過去のログファイル有なので、サイズを取得
					try
					{
						FileInfo _logFileInfo = new FileInfo(_maxSerialFileName);
						_logFileInfo.Refresh();
						logFileSize = _logFileInfo.Length;
//						_com_vdbgo.vDbgoVerbose(_com_vdbgo.ComsrcInf, "ログファイルサイズ = {0:d}\r\n", logFileSize);
					}
					catch(Exception e)
					{
						_com_vdbgo.vDbgoVerbose(_com_vdbgo.ComsrcInf, "getLastLogInfo(logOptionSize) error {0}\r\n", e.Message);
						//	エラー処理は削除が望ましいか？
					}
					if (logFileSize >= optionPrm)
					{	//	閾値を超えていればシリアル番号を更新する
						serialNum++;
						logFileSize = 0;
					}
					break;
				case	logOptionLine:
					//	過去のログファイル有なので、行数を取得
					try
					{
						string[] _logStrings = File.ReadAllLines(_maxSerialFileName);
						logFileLine = _logStrings.Length;
//						_com_vdbgo.vDbgoVerbose(_com_vdbgo.ComsrcWrn, "ログファイル行数 = {0:d}\r\n", logFileLine);
					}
					catch (Exception e)
					{
						_com_vdbgo.vDbgoVerbose(_com_vdbgo.ComsrcInf, "getLastLogInfo(logOptionLine) error {0}\r\n", e.Message);
						//	エラー処理は削除が望ましいか？
					}
					if (logFileLine >= optionPrm)
					{	//	閾値を超えていればシリアル番号を更新する
						serialNum++;
						logFileLine = 0;
					}
					break;
				default:
					break;
				}
			}
		}
		//--------------------------------------------------------------------------------
		/// <summary>
		///		LogConst
		///		初期化処理実装部
		///		History :
		///			2009.08.12 SKYware
		/// </summary>
		/// <param name="_baseName"></param>
		/// <param name="_extName"></param>
		/// <param name="_dirName"></param>
		/// <param name="_Option"></param>
		/// <param name="_OptionPrm"></param>
		private void LogConst(
			string _baseName,	//	基本ログファイル名
			string _extName,	//	基本ログファイル拡張子名
			string _dirName,	//	ログディレクトリ
			int _Option,		//	作成モード	
			int _OptionPrm		//	オプションパラメータ
			)
		{
			//	ログ作成情報の保存
			if (_baseName != null)	baseName = _baseName;
			if (_extName != null)
			{	//	拡張子の先頭は'.'であってほしい
				if (_extName.IndexOf('.') != 0)
					extName = _extName.Insert(0,".");
				else
					extName = _extName;
			}
			if (_dirName != null)
			{	//	ディレクトリの最後は\がほしい
				if (_dirName.LastIndexOf('\\') != _dirName.Length - 1)
					dirName = _dirName + "\\";
				else
					dirName = _dirName;
			}
			option = _Option;
			optionPrm = _OptionPrm;
			switch (option)
			{
			case	logOptionNone:
			default:
				break;
			case logOptionDay:
				//	ファイル名は　年(4桁)月(2桁)日(2桁)+基本ファイル名+拡張子名
				DateTime _dtNow = DateTime.Now;
				string	_logFileName = string.Format("{0}{1:d4}{2:d2}{3:d2}{4}{5}",
										dirName, _dtNow.Year, _dtNow.Month, _dtNow.Day,
										baseName, extName);
				//	古いファイルの削除
				checkAndDeleteLog(_logFileName);
				break;
			case logOptionSize:
				//	ディレクトリ内の最終ログファイル名とファイルサイズの取得
				getLastLogInfo();
				break;
			case logOptionLine:
				//	ディレクトリ内の最終ログファイル名とファイルサイズの取得
				getLastLogInfo();
				break;
			}
			//	
			//	スレッドセーフの為、ファイルアクセスを排他で行う為のセマフォーを
			//	初期化する。
			semLogFile = new Semaphore(1, 1);
		}
		//--------------------------------------------------------------------------------
		/// <summary>
		///		vWriteLog	ログの書き出し
		///		Notes   :
		///			CR->CRLF変換、行末へのCRLFの付加は行わない。
		///		History :			
		///			2009.08.12 SKYware
		/// </summary>
		/// <param name="_logStr"></param>
		/// <param name="_AddDate"></param>
		public void vWrtiteLog(
			string	_logStr,	//	書き出しログ文字列
			bool	_AddDate	//	日付情報負荷フラグ
			)
		{
			// セマフォで排他制御が必要であればここで行う
			semLogFile.WaitOne();
			//	ファイルストリームがnullであれば、ファイルを作成する。
			//	コンストラクタの最初、および、前回の書込み後、ログファイルがクローズ
			//	されている場合nullとなる。
			string	_logFileName = "";
			switch(option)
			{
			case	logOptionNone:
			default:
				if (logFileStream == null)	_logFileName = dirName+baseName+extName;
				break;
			case	logOptionDay:
				//	日付が代わっていれば新しいファイル名を作成する
				if (logFileStream == null)
				{
					//	ファイル名は　年(4桁)月(2桁)日(2桁)+基本ファイル名+拡張子名
					DateTime _dtNow = DateTime.Now;
					_logFileName = string.Format("{0}{1:d4}{2:d2}{3:d2}{4}{5}",
										dirName, _dtNow.Year, _dtNow.Month, _dtNow.Day,
										baseName, extName);
				}
				//	古いファイルの削除
				checkAndDeleteLog(_logFileName);
				break;
			case	logOptionSize:
			case	logOptionLine:
				if (logFileStream == null)
				{
					//	ファイル名は　基本ファイル名_+シリアル番号+拡張子名
					_logFileName = string.Format("{0}{1}{2:d}{3}",
										dirName, baseName, serialNum, extName);

				}
				break;
			}
			if (logFileStream == null)
				_com_vdbgo.vDbgoVerbose(_com_vdbgo.ComsrcInf, "Make _logFileName = {0}\r\n", _logFileName);
			//	ログファイルのオープン
			string _writeString = _logStr;
			try
			{
				if (logFileStream == null)
				{
					logFileStream = File.AppendText(_logFileName);
				}
				if (_AddDate)
				{	//	日付を追加する
					DateTime _dtNow = DateTime.Now;
					_writeString = string.Format("{0:d2}/{1:d2}/{2:d2} {3:d2}:{4:d2}:{5:d2}.{6:d3} {7}",
														_dtNow.Year, _dtNow.Month, _dtNow.Day,
														_dtNow.Hour, _dtNow.Minute, _dtNow.Second,
														_dtNow.Millisecond,_logStr);
				}
				else
				{
					_writeString = _logStr;
				}
				//	ファイルへ書き出し
				logFileStream.Write(_writeString);
				//　フラッシュしておくファイルはクローズしない
				logFileStream.Flush();
			}
			catch(Exception e)
			{
				_com_vdbgo.vDbgoVerbose(_com_vdbgo.ComsrcInf, "log file access error (vWrtiteLog) error {0}\r\n", e.Message);
				//	書込みに失敗したファイルをクローズしておく
				logFileStream.Dispose();
				logFileStream = null;
				//	後処理はとりあえず行うこととする。（書き込めているかもしれないので）
			}
			//	サイズの確認
			switch (option)
			{
			case logOptionNone:
			case logOptionDay:
			default:
				//	何もすることはない...
				break;
			case logOptionSize:
				if (optionPrm != 0)
				{
					logFileSize += _writeString.Length;
					if (logFileSize >= optionPrm)
					{	//	ファイルサイズが閾値を超えたので新しいファイルを作成する
						//	既存のファイルをクローズし、シリアル番号をインクリメントする
						logFileStream.Close();
						logFileStream = null;
						serialNum++;
						logFileSize = 0;
					}
				}
				break;
			case logOptionLine:
				if (optionPrm != 0)
				{	//	文字列中の0x0aの数を数える？
					string[] _tmpString = _writeString.Split(new Char[] { '\n' });
//					_com_vdbgo.vDbgoVerbose(_com_vdbgo.ComsrcWrn, "logOptionLine Count = {0}\r\n", _tmpString.Length);
					//	改行が無ければ'1'が戻されるので、1以上で行数を追加とする
					if (_tmpString.Length > 1)
					{
						logFileLine += _tmpString.Length - 1;	//	0x0aの数+1が_tmpString.Lengthの値
//						_com_vdbgo.vDbgoVerbose(_com_vdbgo.ComsrcWrn, "logFileLine Count = {0}\r\n", logFileLine);
						if (logFileLine >= optionPrm)
						{	//	ファイルライン数が閾値を超えたので新しいファイルを作成する
							//	既存のファイルをクローズし、シリアル番号をインクリメントする
							logFileStream.Close();
							logFileStream = null;
							serialNum++;
							logFileLine = 0;
						}
					}
				}
				break;
			}
			// セマフォで排他解除
			semLogFile.Release();
		}
		/// <summary>
		///		close	ログファイルのクローズ
		///		Notes   :
		///			ログファイルをクローズする。
		///		History :			
		///			2009.09.03 SKYware
		/// 
		/// </summary>
		public void close()
		{
			if (logFileStream != null)
			{
				logFileStream.Close();
				logFileStream = null;
			}

		}
		//--------------------------------------------------------------------------------
		/// <summary>
		///		_com_log	ログファイル名指定のみのコンストラクタ
		///		History :
		///			2009.08.12 SKYware
		/// </summary>
		/// <param name="_baseName"></param>
		public _com_log(
			string _baseName		//	基本ログファイル名
			)
		{
			LogConst(
				_baseName,		//	基本ログファイル名
				null,			//	基本ログファイル拡張子名
				null,			//	ログディレクトリ
				logOptionNone,	//	作成モード	
				0			//	オプションパラメータ
				);

		}
		//--------------------------------------------------------------------------------
		/// <summary>
		///		_com_log	フルオプション指定有のコンストラクタ
		///		History :
		///			2009.08.12 SKYware
		/// </summary>
		/// <param name="_baseName"></param>
		public _com_log(
			string _baseName,	//	基本ログファイル名
			string _extName,	//	基本ログファイル拡張子名
			string _dirName,	//	ログディレクトリ
			int _Mode,			//	作成モード	
			int _Option			//	オプションパラメータ
			)
		{
			LogConst(
				_baseName,	//	基本ログファイル名
				_extName,		//	基本ログファイル拡張子名
				_dirName,		//	ログディレクトリ
				_Mode,			//	作成モード	
				_Option			//	オプションパラメータ
				);
		}
	}
}

