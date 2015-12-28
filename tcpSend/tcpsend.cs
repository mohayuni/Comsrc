using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Comsrc;

namespace tcpSend
{
	class tcpsend
	{
		//-----定数定義--------------------------------------------------------------------

		//-----プロパティの定義--------------------------------------------------------------------
		static private uint debugFlag = 0xffffffff;	//	デバックフラグ
		static private string iPName = "192.168.0.1";		//	送信先IPアドレス又はホスト名
		static private int portNum = 50000;	//	ソケットのポート番号(送信先）
		static private int blockSize = 1024;	//	1ブロックのサイズ
		static private int loopCount = 0;		//	繰り返し回数(0..無限)
		static private int timeOutMs = 0;		//	送信タイムアウト時間(ms単位　0..タイムアウト無）
		static private int intervalMs = 0;		//	送信ブロック間Sleep時間(ms単位　0..スリープ無）
		static bool bTcp = true;				//	TCP転送

		//-----メソッドの定義--------------------------------------------------------------------
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
		//	2009.08.12	typed
		//----------------------------------------------------------------------
		static private bool sbCheckArg(string[] args)
		{
			int _ii;
			string _wkStr;
			for (_ii = 0; _ii < args.Length; _ii++)
			{
				if ((args[_ii].StartsWith("/D:") == true)
					 || (args[_ii].StartsWith("/d:") == true))
				{	//	デバックフラグ
					_wkStr = args[_ii].Remove(0, args[_ii].LastIndexOf(':') + 1);
					debugFlag = uint.Parse(_wkStr, System.Globalization.NumberStyles.HexNumber);
					continue;
				}
				if ((args[_ii].StartsWith("/U:") == true)
					|| (args[_ii].StartsWith("/u:") == true))
				{	//	送信先IPアドレス

					iPName = args[_ii].Remove(0, args[_ii].LastIndexOf(':') + 1);
					continue;
				}
				if ((args[_ii].StartsWith("/N:") == true)
					|| (args[_ii].StartsWith("/n:") == true))
				{	//	送信先ポート番号
					_wkStr = args[_ii].Remove(0, args[_ii].LastIndexOf(':') + 1);
					portNum = int.Parse(_wkStr, System.Globalization.NumberStyles.Integer);
					continue;
				}
				if ((args[_ii].StartsWith("/B:") == true)
					|| (args[_ii].StartsWith("/b:") == true))
				{	//	転送ブロックサイズ
					_wkStr = args[_ii].Remove(0, args[_ii].LastIndexOf(':') + 1);
					blockSize = int.Parse(_wkStr, System.Globalization.NumberStyles.Integer);
					continue;
				}
				if ((args[_ii].StartsWith("/X:") == true)
					|| (args[_ii].StartsWith("/x:") == true))
				{	//	転送モード
					bTcp = false;	// UDP
					continue;
				}
				if ((args[_ii].StartsWith("/L:") == true)
					|| (args[_ii].StartsWith("/l:") == true))
				{	//	ループ回数
					_wkStr = args[_ii].Remove(0, args[_ii].LastIndexOf(':') + 1);
					loopCount = int.Parse(_wkStr, System.Globalization.NumberStyles.Integer);
					continue;
				}
				if ((args[_ii].StartsWith("/W:") == true)
					|| (args[_ii].StartsWith("/w:") == true))
				{	//	タイムアウト時間
					_wkStr = args[_ii].Remove(0, args[_ii].LastIndexOf(':') + 1);
					timeOutMs = int.Parse(_wkStr, System.Globalization.NumberStyles.Integer);
					continue;
				}
				if ((args[_ii].StartsWith("/I:") == true)
					|| (args[_ii].StartsWith("/i:") == true))
				{	//	転送ブロック間スリープ時間
					_wkStr = args[_ii].Remove(0, args[_ii].LastIndexOf(':') + 1);
					intervalMs = int.Parse(_wkStr, System.Globalization.NumberStyles.Integer);
					continue;
				}
				System.Console.Write("tcpsnd\n"
						+ " /U:IPアドレス( = {0})\n"
						+ " /N:Port Number( = {1})\n"
						+ " /B:転送ブロックサイズ(Byte)( = {2})\n"
						+ " /X:UDP転送\n"
						+ " /L:ループ回数(0..永久ループ)( = {3})\n"
						+ " /W:タイムアウト時間(msec)( = {4})\n"
						+ " /I:1ブロック転送毎のスリープ(msec)( = {5})\n",
					iPName, portNum, blockSize, loopCount, timeOutMs, intervalMs);
				return (false);

			}
			System.Console.WriteLine("IPアドレス = " + iPName);
			System.Console.WriteLine("転送ブロックサイズ = {0}Byte", blockSize);
			System.Console.WriteLine("Port番号 = {0}", portNum);
			if (bTcp == true)
				System.Console.WriteLine("TCP受信");
			else
				System.Console.WriteLine("UDP受信");
			System.Console.WriteLine("ループ回数 = {0}", loopCount);
			System.Console.WriteLine("タイムアウト時間 = {0}msec", timeOutMs);
			System.Console.WriteLine("1ブロック転送毎のスリープ{0}(msec)", intervalMs);
			return (true);
		}

		static	private void dispSockErr(int _errCode)
		{
			switch (_errCode)
			{
				case	(int)_com_ipsock.errStatus.argumentNullException:
					System.Console.WriteLine("+++++ argumentNullException +++++");
					break;
				case	(int)_com_ipsock.errStatus.argumentOutOfRangeException:
					System.Console.WriteLine("+++++ argumentOutOfRangeException +++++");
					break;
				case (int)_com_ipsock.errStatus.disconnected:
					System.Console.WriteLine("+++++ disconnected +++++");
					break;
				case (int)_com_ipsock.errStatus.exception:
					System.Console.WriteLine("+++++ exception +++++");
					break;
				case (int)_com_ipsock.errStatus.invalidSocket:
					System.Console.WriteLine("+++++ invalidSocket +++++");
					break;
				case (int)_com_ipsock.errStatus.noError:
					System.Console.WriteLine("+++++ noError +++++");
					break;
				case (int)_com_ipsock.errStatus.nullReferenceException:
					System.Console.WriteLine("+++++ nullReferenceException +++++");
					break;
				case (int)_com_ipsock.errStatus.objectDisposedException:
					System.Console.WriteLine("+++++ objectDisposedException +++++");
					break;
				case (int)_com_ipsock.errStatus.socketException:
					System.Console.WriteLine("+++++ socketException +++++");
					break;
				case (int)_com_ipsock.errStatus.timeOut:
					System.Console.WriteLine("+++++ timeOut +++++");
					break;
				default:
					System.Console.WriteLine("+++++ ??? ErroCode = {0} +++++", _errCode);
					break;
			}

		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
		{
			if (sbCheckArg(args) == false) return;

			_com_vdbgo.vDbgoInit(debugFlag);	//	_com_vdbgoはstaticクラス

			int	ii,jj;
			UInt32 kk = 0;

#if NOP
			for ( ii = 0 ; ii < blockSize/4 ; ii++ )
			{
				_buffer[ii] = 0;
			}
#endif
			_com_ipsock	_comscoket = new _com_ipsock(
					iPName, portNum, bTcp, false);

//			UInt32	_data32;
			int	_ret;

			byte[] _buffer8 = new byte[blockSize];
			UInt32[] _buffer32 = new UInt32[blockSize/sizeof(Int32)];

			int	_startTickCount = Environment.TickCount;

			bool _errFlag = false;

			ii = 0;
			for (; _errFlag == false; )
			{
				for ( jj = 0 ; jj < blockSize ; jj+=4, kk++ )
				{
					_buffer8[jj] = (byte)(kk & 0xff);
					_buffer8[jj+1] = (byte)((kk & 0xff00)>>8);
					_buffer8[jj + 2] = (byte)((kk & 0xff0000) >> 16);
					_buffer8[jj + 3] = (byte)((kk & 0xff000000) >> 24);
				}
				_ret = _comscoket.send(_buffer8, blockSize, timeOutMs, null);
				if (_ret <= 0)
				{
					dispSockErr(_ret);
					_errFlag = true;
					break;
				}
				if (_ret != blockSize)
				{
					System.Console.WriteLine("send Req = {0}, Send = {1}",blockSize, _ret);
					break;
				}
//				if (siNoDisp)		System.Console.Write("*");
				System.Console.Write("*");
				if (loopCount != 0)
				{
					ii++;
					if (loopCount == ii)	break;
				}
				if (intervalMs != 0)
				{
					System.Threading.Thread.Sleep(intervalMs);
				}
			}
			int	_Time = Environment.TickCount-_startTickCount;
				double	_dbRate = (double)((double)loopCount*(double)blockSize/(double)_Time *1000)/1024/1024;
//				DOUBLE	_dbRate = (double)((double)loopCount*(double)blockSize/(double)_Time *1000.)/1024./1024.;
			System.Console.WriteLine("\nTime = {0}ms	TrSize = {1} {2}Mbyte/Sec\n", _Time,loopCount*blockSize, _dbRate);
			_comscoket.close();
		}
	}
}
