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
			enShozoku = 0,		//	所属SL
			enJR,				//	JR会社
			enShaban,			//	車番
			enShinseiDate,		//	新製/改造年月日
			enSeizousho,		//	製造/改造所
			enHaitiku,			//	配置区
			enKaizoumaeShaban,	//	改造前車番
			enTenzokuDate,		//	転属年月日
			enShinku,			//	新区
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
		private _CtlDb cCtlDb;

		//--------------------------------------------------------------------------------
		/// <summary>
		///		_ReadCsv	コンストラクタ
		///		Notes	:
		///			指定されたファイル名をオープンし、メモリー上に読み出す
		///			
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
				cCtlDb = new _CtlDb();
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
		/// <param name="strClassName"></param>
		/// <param name="strSeriese"></param>
		/// 
		public bool _ReadOneLineData(
			string strClassName,
			string strSeriese
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
			DateTime Dt;
			if (
				//	新製/改造年月日
				(!(DateTime.TryParse(straData[(int)enCalumIndex.enShinseiDate], out Dt))
				 && (straData[(int)enCalumIndex.enShinseiDate] != null))
				||
				//	転属年月日の確認
				(!(DateTime.TryParse(straData[(int)enCalumIndex.enTenzokuDate], out Dt))
				 && (straData[(int)enCalumIndex.enTenzokuDate] != null))
				 ||
				//	廃車/改造年月日
				(!(DateTime.TryParse(straData[(int)enCalumIndex.enHaishaDate], out Dt))
				 && (straData[(int)enCalumIndex.enHaishaDate] != null))
				)
			{
				_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "Invalid rireki Data {0}\r\n", straData);
				return (false);
			}
			//	データは妥当と判断
			_CtlDb._cCarriageData cCarData = new _CtlDb._cCarriageData();
			_CtlDb._cHistoryData cHistoryData = new _CtlDb._cHistoryData();
			//	車番がブランク？
			if (straData[(int)enCalumIndex.enShaban] != null)
			{
				//	車輛固有情報を追加
				cCarData.strClass = strClassName;   //	引数の情報を使用
				cCarData.strSeriese = strSeriese;   //	引数の情報を使用
				if (_strShagouToKeishiki(enConvKeishiki.enKokuden,
								straData[(int)enCalumIndex.enShaban],
								out cCarData.strForm,
								out cCarData.iSerial) == false)
				{
					_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "Invalid rireki Data(車番){0} [{1}]\r\n", straData[(int)enCalumIndex.enShaban], straData);
					return (false);
				}
				if (cCtlDb._WriteCarriageInfo(cCarData) == false)
				{
					_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "DBへの書込みに失敗!!\r\n");
					return (false);
				}

				//	改造前車番がブランク
				if (straData[(int)enCalumIndex.enKaizoumaeShaban] != null)
				{
					//	改造・改番で履歴情報を追加
					if (!(DateTime.TryParse(straData[(int)enCalumIndex.enShinseiDate], out Dt))
						 && (straData[(int)enCalumIndex.enShinseiDate] != null))
					{
						_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "新製/改造年月日が不正です{0} [{1}]\r\n", straData[(int)enCalumIndex.enShinseiDate], straData);
						return (false);
					}

#if NOP

		public int iCarriageNumber = 0; //	車輛テーブルのROWID
		public string strCode = null;       //	履歴コード	(改番、改造、新製、廃車...)
		public int iYear = 0;               //	履歴年
		public int iMonth = 0;          //	履歴月
		public int iDay = 0;                //	履歴日
		public string strPlace = null;  //	所属区
		public string strFactory = null;    //	施工場所
		public string strSummary = null;  //	概要
										  //	public string strClass=null;	//	改造後種別コード(EC/DC/PC...)
										  //	public string strSeriese=0;			//	改造後系列番号	(101/103...)
		public string strForm = null;     //	改造後/前型式	(モハ101/クハ101...)
		public int iSerial = 0;         //	改造後/前車号	(1/1001...)
#endif
					cHistoryData.iCarriageNumber = 123;			//	暫定・・・
					cHistoryData.strCode = _CtlDb.cstrKaizou;
					cHistoryData.iYear = Dt.Year;
					cHistoryData.iMonth = Dt.Month;
                    cHistoryData.iDay = Dt.Day;
					cHistoryData.strPlace = straData[(int)enCalumIndex.enShozoku];
                    cHistoryData.strFactory = straData[(int)enCalumIndex.enSeizousho];
					cHistoryData.strSummary = null;
#if NOP
					//	以下の2つは旧国の車号から、形式と番号を取り出す処理が必要
					//	_strShagouToKeishikiの拡張が必要
					cHistoryData.strForm = null;
                    cHistoryData.iSerial = 0;
					cCarData.strClass = strClassName;   //	引数の情報を使用
					cCarData.strSeriese = strSeriese;   //	引数の情報を使用
					if (_strShagouToKeishiki(enConvKeishiki.enKokuden,
									straData[(int)enCalumIndex.enShaban],
									out cCarData.strForm,
									out cCarData.iSerial) == false)
					{
						_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "Invalid rireki Data(車番){0} [{1}]\r\n", straData[(int)enCalumIndex.enShaban], straData);
						return(false);
					}
					//	同じように履歴データを書き込む処理が必要
					if (cCtlDb._WriteCarriageInfo(cCarData) == false)
					{
						_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "DBへの書込みに失敗!!\r\n");
						return (false);
					}
#endif

				}
				else
				{

				}
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

		//--------------------------------------------------------------------------------
		/// <summary>
		///		車号の文字列から、形式と番号を分離する。
		///		Notes	:
		///			・新性能国電であれば、'-'を区切り文字とする。
		///			・機関車（EL/DL）/客車/DCであれば　' 'を区切り文字とする。
		///			・旧型国電はカナ＋数字二けたを形式、以降を番号とする。
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
			out int	iSerial
		)
		{
			bool bRet = true;
			strKeishiki = null;
			iSerial = 0;
            switch (enType)
			{
				case enConvKeishiki.enKokuden:  //	新性能国電
					//	文字列先頭からハイフンまでが形式名、以降が番号
					string[] straData = strShagou.Split('-');
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
					iSerial = int.Parse(straData[1]);
					if ( (strKeishiki == null) || (iSerial == 0) )
					{
						_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "不正な車号文字列です。形式と番号に分離出来ません {0} \r\n", strShagou);
						bRet = false;
					}
					break;
				case enConvKeishiki.enKyuukou:  //	旧型国電
#if NOP

					//	文字列先頭からハイフンまでが形式名、以降が番号
					string[] straData = strShagou.Split('-');
					if (straData[0] == null)
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
					iSerial = int.Parse(straData[1]);
					if ((strKeishiki == null) || (iSerial == 0))
					{
						_com_vdbgo.vDbgoVerbose(_com_vdbgo.TestErr, "不正な車号文字列です。形式と番号に分離出来ません {0} \r\n", strShagou);
						bRet = false;
					}
#endif
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


