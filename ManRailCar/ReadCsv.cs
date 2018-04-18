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
//	下記URLに記載されている車輛履歴XLSファイルをCSV化し、CtlDbクラスに
//	合わせたDB構造体を作成、登録する。
//	http://www1.odn.ne.jp/niinii/rireki-ec.htm
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
using CtlDb;

namespace ReadCsv
{

	class _ReadCsv 
	{
		//-----定数定義--------------------------------------------------------------------
		//		CSVカラムインデックス番号定義
		//		履歴情報クラスインデックス番号
		///		所属,JR会社,車番,新製/改造年月日,製造/改造所,配置区,改造前車番,
		///		転属年月日,新区,廃車/改造年月日,改造後車番
		public enum enCalumIndex
		{
			enShozoku = 0,		//	所属
			enJR,				//	JR会社
			enShaban,			//	車番
			enShinseiDate,		//	新製/改造年月日
			enSeizousho,		//	製造/改造所
			enHaitiku,			//	配置区
			enKaizoumaeShaban,	//	改造前車番
			enTenzokuDate,		//	転属年月日
			enTenzonsaki,		//	転属先
			enHaishaDate,		//	廃車改造年月日
			enKaizongoShaban,	//	改造後車番
			enMaxIndex			//	インデックスの最大値
		}

		//	_strShagouToKeishiki処理（車号から、形式と番号を分離する）で使用する
		//	対象となる形式毎に、分離するルールが異なる為、ルールを指定する際に使用する。
		public enum enConvKeishiki
		{
			enKokuden = 0,      //	新性能国電
			enKyuukou,          //	旧型国電
			enDC,               //	ディーゼル車
			enPC,               //	客車
			enEL,               //	電気機関車
			enDL,               //	電気機関車
			enSL,				//	蒸気機関車
			enOldSL,			//	9600/8620以前のSL
			enMaxIndex          //	インデックスの最大値
		}

		//-----クラスの定義--------------------------------------------------------------------
		/// <summary>

		//-----プロパティの定義--------------------------------------------------------------------

		//-----メンバー変数定義--------------------------------------------------------------------
		private string m_strFull;   //	指定CSVファイル全体を読み込んだ文字列
		private System.IO.StringReader m_streaderFull;  //	m_strFullにアクセスする為のStringReader構造体
		private _CtlDb m_cCtlDb;
		private string m_strClassName;	//	種別コード名称	(EC/DC/PC...)
		private string m_strSeriese;    //	系列名称		(101系/103系...)

		//--------------------------------------------------------------------------------
		/// <summary>
		///		_ReadCsv	コンストラクタ
		///		Notes	:
		///			指定されたファイル名をオープンし、メモリー上に読み出す
		///			
		///		History :			
		///			2015.12.27 Mohayuni
		/// </summary>
		/// <param name="strFileName">	対象となるCSVファイル名</param>
		/// <param name="strClassName">	種別コード名称	(EC/DC/PC...)</param>
		/// <param name="strSeriese">	系列名称		(101系/103系...)</param>
		/// <param name="cDb">			書込み先データベースクラス</param>
		public _ReadCsv(
			string strFileName,  //	対象ファイル名
            string strClassName,
			string strSeriese,
            _CtlDb	cDb
        )
		{
			try
			{
				using (System.IO.StreamReader cCsvReader = new System.IO.StreamReader(strFileName, System.Text.Encoding.GetEncoding("shift_jis")))
				{
					m_strFull = cCsvReader.ReadToEnd();
					m_streaderFull = new StringReader(m_strFull);
                }
				m_cCtlDb = cDb;
				m_strClassName = strClassName;
				m_strSeriese = strSeriese;
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
		///		_ConvertOneLineData	1ラインのデータをDB形式に変換し、書込み処理を呼び出す
		///		Notes	:
		///			・_ReadCsvコンストラクターで読み込んだデータから
		///			　１ラインのデータのCSVデータを読み取る。
		///			・カンマくぐりのフォーマットは以下の11カラムを想定
		///			　所属,JR会社,車番,新製/改造年月日,製造/改造所,配置区,改造前車番,
		///			　転属年月日,新区,廃車/改造年月日,改造後車番
		///			・データの妥当性については、年月日のカラムがブランク又は、日付であることで
		///			　判断する。妥当でないデータはLogに残し、スキップする。
		///			・毎回のファイルアクセスを避ける為、コンストラクターで一気に読込を行って
		///			　いる
		///			・1ラインで複数のCarriageData、HistoryDataを作成する。
		///			・種別コード、系列番号は引数で受け取ることとする。
		///		History :			
		///			2015.12.27 Mohayuni
		/// </summary>
		/// <return="false" EOF></return>
		/// <return="true" No Error></return>
		//	TRUE	起動引数は取得できた
		//	FALSE	最低一つの不正な引数があった

		public bool _ReadOneLineData(
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
			//	データ以外の行も含まれている為、の妥当性を判断する。
			DateTime DtShinsei;
			DateTime DtTenzoku;
			DateTime DtHaisha;
			bool bRetShinseiDate;
			bool bRetTenzokuDate;
			bool bRetHaishaDate;
			//	新製/改造年月日
			bRetShinseiDate = DateTime.TryParse(straData[(int)enCalumIndex.enShinseiDate], out DtShinsei);
			//	転属年月日の確認
			bRetTenzokuDate = DateTime.TryParse(straData[(int)enCalumIndex.enTenzokuDate], out DtTenzoku);
			//	廃車/改造年月日
			bRetHaishaDate = DateTime.TryParse(straData[(int)enCalumIndex.enHaishaDate], out DtHaisha);
			if ( (bRetShinseiDate == false) && (bRetTenzokuDate == false) && (bRetHaishaDate == false) )
			{
				//	一つも有効な日付を持っていない場合無効行とする。
				_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "Invalid rireki Data {0}\r\n", straData);
				return (true);		//	無効行としてLogに残し、処理は継続する。
			}
			//	データは妥当と判断
			_CtlDb._cCarriageData cCarData = new _CtlDb._cCarriageData();
			_CtlDb._cHistoryData cHistoryData = new _CtlDb._cHistoryData();
			int	iCarriageNumber = 123;	//	暫定
			//	車番がブランクではない
			if (straData[(int)enCalumIndex.enShaban] != "")
			{
				//	車輛固有情報を追加
				cCarData.strClass = m_strClassName;   //	引数の情報を使用
				cCarData.strSeriese = m_strSeriese;   //	引数の情報を使用
				if (_strShagouToKeishiki(enConvKeishiki.enKokuden,
								straData[(int)enCalumIndex.enShaban],
								out cCarData.strForm,
								out cCarData.strSerial) == false)
				{
					_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "Invalid rireki Data(車番){0} [{1}]\r\n", straData[(int)enCalumIndex.enShaban], straData);
					return (true);      //	無効行としてLogに残し、処理は継続する。
				}
				if (m_cCtlDb._WriteCarriageInfo(cCarData) == false)
				{
					_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "DBへの書込みに失敗!!\r\n");
					return (false);      //	致命的なエラーなので処理を中断させる
				}

				//	改造前車番がブランクではない
				if (straData[(int)enCalumIndex.enKaizoumaeShaban] != "")
				{
					//	改造・改番で履歴情報を追加
					//	履歴情報を作成する。
					cHistoryData.iCarriageNumber = iCarriageNumber;
					cHistoryData.strCode = _CtlDb.cstrKaizou;
					cHistoryData.iYear = DtShinsei.Year;
					cHistoryData.iMonth = DtShinsei.Month;
                    cHistoryData.iDay = DtShinsei.Day;
					cHistoryData.strPlace = straData[(int)enCalumIndex.enHaitiku];
                    cHistoryData.strFactory = straData[(int)enCalumIndex.enSeizousho];
					cHistoryData.strSummary = null;
					//	以下の2つは旧国又は国電の車号から、形式と番号を取り出す処理が必要がある。
					string[] straDataTemp = straData[(int)enCalumIndex.enKaizoumaeShaban].Split('-');
					//	配列の要素数をチェック　2未満はNG
					bool bGetKeishikiRet;
					if (straDataTemp.Length < 2)
					{   //	改造前車号は旧型付番
						bGetKeishikiRet = _strShagouToKeishiki(enConvKeishiki.enKyuukou,
									straData[(int)enCalumIndex.enKaizoumaeShaban],
									out cHistoryData.strForm,
									out cHistoryData.strSerial);
                    }
					else
					{	//	新性能国電付番
						bGetKeishikiRet = _strShagouToKeishiki(enConvKeishiki.enKokuden,
									straData[(int)enCalumIndex.enKaizoumaeShaban],
									out cHistoryData.strForm,
									out cHistoryData.strSerial);
					}
					if (bGetKeishikiRet == false)
					{
						_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "Invalid rireki Data(車番){0} [{1}]\r\n", straData[(int)enCalumIndex.enShaban], straData);
						return (true);      //	無効行としてLogに残し、処理は継続する。
					}
					//	履歴データを書き込む
					if (m_cCtlDb._WriteHistoryInfo(cHistoryData) == false)
					{
						_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "DBへの書込みに失敗!!\r\n");
						return (false);      //	致命的なエラーなので処理を中断させる
					}
				}
				else
				{   //	新製情報を追加する。
					//	履歴情報を作成する。
					cHistoryData.iCarriageNumber = iCarriageNumber;
					cHistoryData.strCode = _CtlDb.cstrShinsei;
					cHistoryData.iYear = DtShinsei.Year;
					cHistoryData.iMonth = DtShinsei.Month;
					cHistoryData.iDay = DtShinsei.Day;
					cHistoryData.strPlace = straData[(int)enCalumIndex.enHaitiku];
					cHistoryData.strFactory = straData[(int)enCalumIndex.enSeizousho];
					cHistoryData.strSummary = null;
					cHistoryData.strForm = null;
					cHistoryData.strSerial = null;
					//	履歴データを書き込む
					if (m_cCtlDb._WriteHistoryInfo(cHistoryData) == false)
					{
						_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "DBへの書込みに失敗!!\r\n");
						return (false);      //	致命的なエラーなので処理を中断させる
					}

				}
			}
			//	転属年月日がブランクではない
			if (straData[(int)enCalumIndex.enTenzokuDate] != "")
			{
				{   //	転属情報を追加する。
					//	履歴情報を作成する。
					cHistoryData.iCarriageNumber = iCarriageNumber;
					cHistoryData.strCode = _CtlDb.cstrTenzoku;
					cHistoryData.iYear = DtTenzoku.Year;
					cHistoryData.iMonth = DtTenzoku.Month;
					cHistoryData.iDay = DtTenzoku.Day;
					cHistoryData.strPlace = straData[(int)enCalumIndex.enTenzonsaki];
					cHistoryData.strFactory = null;
					cHistoryData.strSummary = null;
					cHistoryData.strForm = null;
					cHistoryData.strSerial = null;
					//	履歴データを書き込む
					if (m_cCtlDb._WriteHistoryInfo(cHistoryData) == false)
					{
						_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "DBへの書込みに失敗!!\r\n");
						return (false);      //	致命的なエラーなので処理を中断させる
					}

				}
			}
			//	廃車、改造年月日がブランクではない
			if (straData[(int)enCalumIndex.enHaishaDate] != "")
			{
				//	改造後車番がブランクではない
				if (straData[(int)enCalumIndex.enKaizongoShaban] != "")
				{
					//	改造廃車履歴を追加する。
					//	履歴情報を作成する。
					cHistoryData.iCarriageNumber = iCarriageNumber;
					cHistoryData.strCode = _CtlDb.cstrKaizouHaisha;
					cHistoryData.iYear = DtHaisha.Year;
					cHistoryData.iMonth = DtHaisha.Month;
					cHistoryData.iDay = DtHaisha.Day;
					cHistoryData.strPlace = null;
					cHistoryData.strFactory = null;
					cHistoryData.strSummary = null;
					//	以下の2つは改造後の車号から、形式と番号を取り出す処理が必要がある。
					if (_strShagouToKeishiki(enConvKeishiki.enKokuden,
									straData[(int)enCalumIndex.enKaizongoShaban],
									out cHistoryData.strForm,
									out cHistoryData.strSerial) == false)
					{
						_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "Invalid rireki Data(車番){0} [{1}]\r\n", straData[(int)enCalumIndex.enShaban], straData);
						return (true);      //	無効行としてLogに残し、処理は継続する。
					}
					//	履歴データを書き込む
					if (m_cCtlDb._WriteHistoryInfo(cHistoryData) == false)
					{
						_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "DBへの書込みに失敗!!\r\n");
						return (false);      //	致命的なエラーなので処理を中断させる
					}
				}
				else
				{   //	廃車情報を追加する。
					//	履歴情報を作成する。
					cHistoryData.iCarriageNumber = iCarriageNumber;
					cHistoryData.strCode = _CtlDb.cstrHaisha;
					cHistoryData.iYear = DtHaisha.Year;
					cHistoryData.iMonth = DtHaisha.Month;
					cHistoryData.iDay = DtHaisha.Day;
					cHistoryData.strPlace = null;
					cHistoryData.strFactory = null;
					cHistoryData.strSummary = null;
					cHistoryData.strForm = null;
					cHistoryData.strSerial = null;
					//	履歴データを書き込む
					if (m_cCtlDb._WriteHistoryInfo(cHistoryData) == false)
					{
						_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "DBへの書込みに失敗!!\r\n");
						return (false);      //	致命的なエラーなので処理を中断させる
					}

				}
			}

			return (true);
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		///		車号の文字列から、形式と番号を分離する。
		///		Notes	:
		///			・新性能国電であれば、'-'を区切り文字とする。
		///			・機関車（EL/DL）/客車/DCであれば　' 'を区切り文字とする。
		///			・旧型国電はカナ＋数字二けたを形式、以降3桁を番号とする。
		///			・9600/8620形式以降の蒸気機関車は先頭から3文字を形式、以降を番号とする。
		///			・9600/8620形式以前は桁数3,4を形式とし、全体を番号とする。
		///			・貨車についいては、当面未サポート
		///		History :			
		///			2015.12.31 Mohayuni
		/// </summary>
		/// <param name="strShagou">	対象となる車号文字列</param>
		/// <param name="strKeishiki">	分離された形式名</param>
		/// <param name="strKeishiki">	分離された番号</param>
		/// 
		public bool _strShagouToKeishiki(
            enConvKeishiki enType,
            string strShagou,
			out string strKeishiki,
			out string strSerial
		)
		{
			bool bRet = true;
			strKeishiki = null;
			strSerial = null;
            switch (enType)
			{
				case enConvKeishiki.enKokuden:  //	新性能国電
					//	文字列先頭からハイフンまでが形式名、以降が番号
					string[] straData = strShagou.Split('-');
					//	配列の要素数をチェック　2未満はNG
					if (straData.Length < 2)
					{
						_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "不正な車号文字列です。形式と番号に分離出来ません {0} \r\n", strShagou);
						bRet = false;
					}
					if (straData[0] == null	)
					{
						_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "不正な車号文字列です。形式と番号に分離出来ません {0} \r\n", strShagou);
						bRet = false;
						break;
					}
					if (straData[1] == null)
					{
						_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "不正な車号文字列です。形式と番号に分離出来ません {0} \r\n", strShagou);
						bRet = false;
						break;
					}
					strKeishiki = straData[0];
					strSerial = straData[1];
					if ( (strKeishiki == null) || (strSerial == null) )
					{
						_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "不正な車号文字列です。形式と番号に分離出来ません {0} \r\n", strShagou);
						bRet = false;
					}
					break;
				case enConvKeishiki.enKyuukou:  //	旧型国電
					//	文字数をチェック　種別を示すカナが可変ではあるが、最低2文字
					//	形式を表す数字2桁+番号は3桁なので合計7文字以上のはず
					if (strShagou.Length < 7)
					{
						_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "不正な車号文字列です。形式と番号に分離出来ません {0} \r\n", strShagou);
						bRet = false;
						break;
					}
					strKeishiki = strShagou.Substring(0, strShagou.Length - 3);
                    strSerial = strShagou.Substring(strShagou.Length-3,3);
					break;

				case enConvKeishiki.enDC:       //	ディーゼル車
					_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "この対応は未だサポートしていません {0} type = {1}\r\n", strShagou, enType);
					bRet = false;
					break;
				case enConvKeishiki.enPC:       //	客車
					_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "この対応は未だサポートしていません {0} type = {1}\r\n", strShagou, enType);
					bRet = false;
					break;
				case enConvKeishiki.enEL:       //	電気機関車
					_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "この対応は未だサポートしていません {0} type = {1}\r\n", strShagou, enType);
					bRet = false;
					break;
				case enConvKeishiki.enDL:       //	ディーゼル機関車
					_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "この対応は未だサポートしていません {0} type = {1}\r\n", strShagou, enType);
					bRet = false;
					break;
				case enConvKeishiki.enSL:       //	蒸気機関車
					_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "この対応は未だサポートしていません {0} type = {1}\r\n", strShagou, enType);
					bRet = false;
					break;
				case enConvKeishiki.enOldSL:    //	9600/8620以前の蒸気機関車
					_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "この対応は未だサポートしていません {0} type = {1}\r\n", strShagou, enType);
					bRet = false;
					break;
				default:
					_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "定義されていない車号文字列です　{0} \r\n", strShagou);
					bRet = false;
					break;
			}
			return (bRet);
		}

	}   //	end class	_ReadCsv
}	//	end namespace	ReadCsv


