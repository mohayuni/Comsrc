using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;	//	Socket
using System.Net;			//	IpAddress
using System.Threading;


using Comsrc;

namespace tcprecv
{
	class tcpRecv
	{
		//-----定数定義--------------------------------------------------------------------

		//-----プロパティの定義--------------------------------------------------------------------
		static private uint debugFlag = 0xffffffff;	//	デバックフラグ
		static private string iPName = "0.0.0.0";		//	送信先IPアドレス又はホスト名
		static private int portNum = 50000;	//	ソケットのポート番号(送信先）
		static private int blockSize = 1024;	//	1ブロックのサイズ
		static private int timeOutMs = 0;		//	送信タイムアウト時間(ms単位　0..タイムアウト無）
		static private int intervalMs = 0;		//	送信ブロック間Sleep時間(ms単位　0..スリープ無）
		static private int mesureBlockCount = 0;//	速度測定ブロック数
		static private bool bChkData = false;	//	受信データチェックフラグ
		static private bool bTcp = true;		//	TCP受信フラグ(UDP=false)
		static private bool bChkAverate = false;//	平均速度チェック
		static private bool bThreadRecv = false;//	スレッド受信

		//-----メソッドの定義--------------------------------------------------------------------
		static	private	_com_ipsock comscoket;
		static	private	Socket accSocket;
		static	private bool getSockInfo;		//	受信スレッドが受信用ソケットを保存したことを示す
		static private int threadIndex;		//	スレッド起動数
		//----------------------------------------------------------------------
		// メソッド: sbChkArg
		//----------------------------------------------------------------------
		// Summary :
		//   起動引数の取得
		// OutLine :
		//   起動引数を取得
		// Return  :
		//   true	起動引数は取得できた
		//   false	最低一つの不正な引数があった
		// Notes   :
		// H_speedBlocktory :
		//   2009.08.12	typed
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
					bChkData = true;
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
				if ((args[_ii].StartsWith("/T:") == true)
					|| (args[_ii].StartsWith("/T:") == true))
				{	//	スレッド受信
					bThreadRecv = true;
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
				if ((args[_ii].StartsWith("/X:") == true)
					|| (args[_ii].StartsWith("/x:") == true))
				{	//	TCP/UDP
					bTcp = false;
					continue;
				}
				if ((args[_ii].StartsWith("/P:") == true)
					|| (args[_ii].StartsWith("/p:") == true))
				{	//	スピード測定ループ回数
					_wkStr = args[_ii].Remove(0, args[_ii].LastIndexOf(':') + 1);
					mesureBlockCount = int.Parse(_wkStr, System.Globalization.NumberStyles.Integer);
					continue;
				}
				if ((args[_ii].StartsWith("/C:") == true)
					|| (args[_ii].StartsWith("/c:") == true))
				{	//	平均速度監視
					bChkAverate = true;
					continue;
				}

				System.Console.Write("tcpsnd\n"
						+ " /U:IPアドレス( = {0})\n"
						+ " /N:Port Number( = {1})\n"
						+ " /B:転送ブロックサイズ(Byte)( = {2})\n"
						+ " /T:スレッドで受信\n"
						+ " /W:タイムアウト時間(msec)( = {3})\n"
						+ " /I:1ブロック転送毎のスリープ(msec)( = {4})\n"
						+ " /X:UDP\n"
						+ " /P:Speed測定 (= {5})\n"
						+ " /C:平均速度監視 (/P:が必要)\n",
					iPName, portNum, blockSize, timeOutMs, intervalMs, mesureBlockCount);
				return (false);

			}
			System.Console.WriteLine("IPアドレス = " + iPName);
			System.Console.WriteLine("Port番号 = {0}", portNum);
			System.Console.WriteLine("転送ブロックサイズ = {0}Byte", blockSize);
			if (bThreadRecv == true)
				System.Console.WriteLine("スレッドで受信");
			System.Console.WriteLine("タイムアウト時間 = {0}msec", timeOutMs);
			System.Console.WriteLine("1ブロック転送毎のスリープ = {0}msec", intervalMs);
			if (bTcp == true)
				System.Console.WriteLine("TCP受信");
			else
				System.Console.WriteLine("UDP受信");
			System.Console.WriteLine("1ブロック転送毎のスリープ{0}(msec)", intervalMs);
			System.Console.WriteLine("速度測定間隔{0}ブロック数", mesureBlockCount);

			return (true);
		}

		static private void d_speedBlockpSockErr(int _errCode)
		{
			switch (_errCode)
			{
				case (int)_com_ipsock.errStatus.argumentNullException:
					System.Console.WriteLine("+++++ argumentNullException +++++");
					break;
				case (int)_com_ipsock.errStatus.argumentOutOfRangeException:
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
					System.Console.WriteLine("+++++ objectD_speedBlockposedException +++++");
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
		///		受信処理
		/// </summary>
		static	public	int	recv(Socket			_accSocket,
								int		_threadIndex)
		{
			int		_ret;
			int		ii,jj,_speedBlock,_avarage;
			UInt32 _ulkk;
			int		_recvedSize;
			double	_aveTime = 0.0;
			double	_sumAveTime = 0.0;
			byte[] _buffer8 = new byte[blockSize];
			bool	_err;
			bool	_first = true;

			_ulkk = 0;
			_speedBlock = 0;
			_avarage = 0;
			_recvedSize = 0;
			_err = false;

			int	_time;
   	        double	_rate;

			int	_startTickCount = Environment.TickCount;

			for ( ii = 0  ; _err == false ; )
			{
				if ( (_ret = comscoket.recv(_buffer8, blockSize, timeOutMs, _accSocket)) <= 0)
				{
					d_speedBlockpSockErr(_ret);
					return(_ret);
				}

				if (mesureBlockCount != 0)
				{
					_speedBlock++;
					if (_speedBlock == mesureBlockCount)
					{
						_time = Environment.TickCount-_startTickCount;
						_rate = (double)((double)_speedBlock * (double)blockSize / (double)_time * 1000) / 1024 / 1024;
						System.Console.WriteLine("{3} : Time = {0}ms  RecvSize = {1} {2}Mbyte/Sec", _time, _speedBlock * blockSize, _rate, _threadIndex);
#if false
						if ( (!bChkAverate) || 
							((bChkAverate) && (_aveTime != 0.0) && (_aveTime*2 < _time)) )
#else
						if (((bChkAverate) && (_aveTime != 0.0) && (_aveTime * 2 < _time)))
#endif
						{
							System.Console.WriteLine("{4} : socket = {0:x} Time = {1}ms  TrSize = {2} {3}Mbyte/Sec", _accSocket, (int)_time, _speedBlock * blockSize, _rate, _threadIndex);
						}

						if ( (bChkAverate)	&& (_aveTime == 0.0) )
						{
							_avarage++;
							_sumAveTime += _time;
							if (_avarage > 10)
							{
								_aveTime = _sumAveTime/_avarage;
								System.Console.WriteLine("{4} : socket = {0:x} Avarage Time = {1}ms  TrSize = {2} {3}Mbyte/Sec", _accSocket, (int)_aveTime, _speedBlock * blockSize, _rate, _threadIndex);
							}
						}
						_startTickCount = Environment.TickCount;
						_speedBlock = 0;

					}
				}

				if (bChkData)
				{
					_recvedSize += _ret;
					if (_ret != blockSize)
					{
						System.Console.WriteLine("{2} : Req = {0}, Recv = {1}", blockSize, _ret, _threadIndex);
						break;
					}

					if (_recvedSize == blockSize)
					{
						ii++;
						if (_first == true)
						{
							_ulkk = ((UInt32)_buffer8[3] << 24)
									+((UInt32)_buffer8[2] << 16)
									+((UInt32)_buffer8[1] << 8)
									+((UInt32)_buffer8[0]);
							_first = false;
							System.Console.WriteLine("{1} : FirstData = {0:x}", _ulkk, _threadIndex);
						}
						for ( jj = 0 ; jj < blockSize ; jj+=4, _ulkk++ )
						{
							UInt32	_tmp;
							_tmp = ((UInt32)_buffer8[jj+3] << 24)
									+((UInt32)_buffer8[jj+2] << 16)
									+((UInt32)_buffer8[jj+1] << 8)
									+((UInt32)_buffer8[jj+0]);
							if (_tmp != _ulkk)
							{
								System.Console.WriteLine("{2} : Recive Data Err Expect = {0:x}, Recv = {1:x}", _ulkk, _tmp, _threadIndex);
								_err = true;
								break;
							}
						}
						_recvedSize = 0;
		//				ulkk = 0;
					}
					if (_recvedSize > blockSize)
					{
						System.Console.WriteLine("{0} : !!!!!!!", _threadIndex);
					}
				}
				else	ii++;
				if (intervalMs != 0)
				{
					System.Threading.Thread.Sleep(intervalMs);
				}
			}
			_time = Environment.TickCount-_startTickCount;
			_rate = (double)((double)_speedBlock * (double)blockSize / (double)_time * 1000) / 1024 / 1024;
			System.Console.WriteLine("{3} : Time = {0}ms  RecvSize = {1} {2}Mbyte/Sec", _time, _speedBlock * blockSize, _rate, _threadIndex);

			System.Console.WriteLine("{1} : LoopCount = {0}", ii, _threadIndex);
			return((int)_com_ipsock.errStatus.noError);
		}

		static void threadRecv()
		{
			System.Console.WriteLine("{0} : Thread Star", threadIndex);
			Socket _accSocket = new Socket(AddressFamily.InterNetwork,
											SocketType.Stream,
											ProtocolType.Tcp);
			_accSocket = accSocket;		//	受信処理ソケットを保存
			getSockInfo = true;			//	受信処理ソケット取得済フラグON
			int _threadIndex = threadIndex;

			if (bTcp)
				recv(_accSocket, _threadIndex);
			else
				recv(null, _threadIndex);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
		{
			if (sbCheckArg(args) == false) return;

			_com_vdbgo.vDbgoInit(debugFlag);	//	_com_vdbgoはstaticクラス

			UInt32[] _buffer = new UInt32[blockSize];
			int	_ret;
			accSocket = new Socket(AddressFamily.InterNetwork,
											SocketType.Stream,
											ProtocolType.Tcp);
			IPEndPoint _endPoint = new IPEndPoint(0, 0);
			Thread _thread;		//	受信スレッド
			threadIndex = 0;
		
			comscoket = new _com_ipsock(
					iPName, portNum, bTcp, true);

			for ( ;; )
			{
				if (bTcp == false)
				{
				}
				else
				{
					_ret = (int)comscoket.accept(ref accSocket, ref _endPoint);
					if (_ret != (int)_com_ipsock.errStatus.noError)
					{
						d_speedBlockpSockErr(_ret);
						break;
					}
					System.Console.WriteLine("Sender IP = {0} Port = {1}", _endPoint.Address, _endPoint.Port );
				}

				//	受信のスレッドを起動
				if (bThreadRecv)
				{
					if (bTcp == false)
					{
						System.Console.Write("Hit any-key+return  start UDP recv thread!! ");
						System.Console.Read();
					}
					getSockInfo = false;
					threadIndex++;
					_thread = new Thread(new ThreadStart(threadRecv));
					_thread.Start();
					//	スレッドが受信用ソケットを受け取るのを待つ
					for (; getSockInfo == false; )
					{	//
						Thread.Sleep(100);
					}
					continue;
				}
				else
					break;
			}
			if (bThreadRecv == false)
			{
				if (bTcp == true)
				{
					recv(accSocket, threadIndex);
					comscoket.closeAccSocket(accSocket);
				}
				else
				{
					recv(null, threadIndex);
				}
			}
			else
			{
				//	スレッドの停止は？
			}
			comscoket.close();
		}
	}
}
