//----------------------------------------------------------------------
// (C) Copyright 2009 SKYware Co.,Ltd. All rights reserved.
//----------------------------------------------------------------------
// <Module Name> TCP/IP・UDP/IPソケットクラス
//----------------------------------------------------------------------
// <File Name>   _com_ipsock.cs
//----------------------------------------------------------------------
// <Description>
//   TCP/IP・UDP/IPソケットクラスを実装する
// <History>
//   2008.09.02 from cipsock.cppより
//----------------------------------------------------------------------
// <Notes>
//	 以下のオプションがある
//	　・TCP/UDP
//    ・サーバ動作/クライアント動作
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
using System.Security;		//	SecurityException
using System.IO;			//	Stream
using System.Diagnostics;	//	CallStack
using System.Threading;		//	Semaphore
using System.Net.Sockets;	//	Socket
using System.Net;			//	IpAddress

namespace Comsrc
{
	class _com_ipsock
	{
	//-----定数定義--------------------------------------------------------------------
		public enum errStatus
		{
			noError = 0,					//	エラー無
			socketException = -100,			//	SocketException
			argumentNullException,			//	ArgumentNullException
			argumentOutOfRangeException,	//	ArgumentOutOfRangeException
			nullReferenceException,			//	NullReferenceException
			objectDisposedException,		//	ObjectDisposedException
			securityException,				//	SecurityException
			exception,						//	other Exception
			invalidSocket,					//	不正ソケットクラス
			timeOut,						//	タイムアウトエラー
			disconnected,					//	接続先が切断
		}

	//-----プロパティの定義--------------------------------------------------------------------
//		TcpClient tcpClient = null;			//	TcpClientクラス
//		TcpListener tcpServer = null;		//	TcpListenerクラス

		
//		NetworkStream networkstream =null;	//	Read/WriteStreamクラス
		private	Socket	socket = null;		//	基本のソケットクラス
		private const int backLog = 5;		//	保留中の接続のキューの最大長。
											//	ディフォルトは5? 5～200が適切な値？
		private bool tcpSocket = true;		//	TCPのソケット
		private bool serverSocket = true;	//	サーバ側のソケット
		private IPEndPoint endPoint;		//	送信先エンドポイント/受信エンドポイント

	//-----メソッドの定義--------------------------------------------------------------------

		//--------------------------------------------------------------------------------
		/// <summary>
		///		_com_ipsock	コンストラクタ
		///		Notes   :
		///			指定された条件でソケットを作成する
		///			指定条件は
		///				サーバのIPアドレス又はホスト名
		///				さーばのアクセスポート
		///				TCP/UDPプロトコル
		///				クライアント/サーバ
		///			注意点
		///				サーバ指定で、複数のインタフェースが存在する環境で、ホスト名
		///				を指定した場合、最初のインタフェースが使用される
		///				期待したインターフェースを使用する場合は、IPアドレス指定が
		///				望ましい
		///				マルチキャストを行う場合
		///				IPアドレスは限定スコープアドレス 239.0.0.0 ～ 239.255.255.255 
		///				ポート番号
		///		History :			
		///			2009.09.03 SKYware
		/// </summary>
		/// <param name="iPName"></param>
		/// <param name="portNum"></param>
		/// <param name="tcp"></param>
		/// <param name="server"></param>
		public	_com_ipsock(
			string	iPName,		//	送信先又は自局のIPアドレス又はホスト名
			int		portNum,	//	ソケットのポート番号(送信先、又は自局）
			bool	tcp,		//	true..TCP/false..UDP
			bool	server		//	true..Sever/false..Client
				)
		{
			tcpSocket = tcp;
			serverSocket = server;
			//	ソケットのオープン
			retry:
			try
			{
				if (tcpSocket == true)	//	TCP
				{	
					if (serverSocket == true)	//	Server
					{
						socket = new Socket(AddressFamily.InterNetwork,
											SocketType.Stream,
											ProtocolType.Tcp);
						endPoint = new IPEndPoint(IPAddress.Parse(iPName), portNum);
						socket.Bind(endPoint);
						socket.Listen(backLog);
					}
					else	//	client
					{
						socket = new Socket(AddressFamily.InterNetwork,
											SocketType.Stream,
											ProtocolType.Tcp);
						//	iPNameはIP名
						IPAddress _ipAddress = IPAddress.Parse(iPName);
						endPoint = new IPEndPoint(_ipAddress, portNum);
						socket.Connect(_ipAddress, portNum);
//						socket.Blocking = false;
					}
				}
				else
				//	UDP
				{
					if (serverSocket == true)	//	Server
					{
						socket = new Socket(AddressFamily.InterNetwork,
											SocketType.Dgram,
											ProtocolType.Udp);
						endPoint = new IPEndPoint(IPAddress.Parse(iPName), portNum);
						socket.Bind(endPoint);
					}
					else
					{
						socket = new Socket(AddressFamily.InterNetwork,
											SocketType.Dgram,
											ProtocolType.Udp);
						endPoint = new IPEndPoint(IPAddress.Parse(iPName), portNum);
					}
				}
			}
			
		    catch(SocketException e) 
			{
				_com_vdbgo.vDbgoVerbose(_com_vdbgo.ComsrcInf, 
					"SocketException caught!!! [_com_ipsock] Source : {0} Message : {1} ErrCode : {2}\r\n",
					e.Source, e.Message, e.ErrorCode);
				if (e.ErrorCode == 10035) goto retry;
				if (socket != null)
				{
					socket.Close();
					socket = null;
				}
			}
			catch(ArgumentNullException e)
			{
				_com_vdbgo.vDbgoVerbose(_com_vdbgo.ComsrcInf, 
					"ArgumentNullException caught!!! [_com_ipsock] Source : {0} Message : {1}\r\n",
					e.Source, e.Message);
				if (socket != null)
				{
					socket.Close();
					socket = null;
				}
			}
			catch(NullReferenceException e)
			{
				_com_vdbgo.vDbgoVerbose(_com_vdbgo.ComsrcInf, 
					"NullReferenceException caught!!! [_com_ipsock] Source : {0} Message : {1}\r\n",
					e.Source, e.Message);
				if (socket != null)
				{
					socket.Close();
					socket = null;
				}
			}
			catch(Exception e)
			{
				_com_vdbgo.vDbgoVerbose(_com_vdbgo.ComsrcInf, 
					"Exception caught!!! [_com_ipsock] Source : {0} Message : {1}\r\n",
					e.Source, e.Message);
				if (socket != null)
				{
					socket.Close();
					socket = null;
				}
			}
		}

#if false
		

	if (m_iModeFlag & CIPSOCK_MODE_UDP)
		m_sSd = socket(AF_INET,SOCK_DGRAM,IPPROTO_UDP);
	else
		m_sSd = socket(AF_INET,SOCK_STREAM,IPPROTO_TCP);
	if ( m_sSd == -1)
	{
		int	iSaveError = m_iGetErrCode();
		VDBGO(((DEBUG_SOCKET|DEBUG_ERR) MYFN "socket open err!!\n"));	
		m_vStrSockError( __FILE__, __LINE__,iSaveError);
		WSACleanup();
		return;
	}

    SockAddr.sin_family = AF_INET;
	SockAddr.sin_port = htons((unsigned short)iPortNum);

	//	IPアドレスを取得する
		struct hostent* lpHostEnt;
		if ( (lpHostEnt = gethostbyname(pcIPName)) == NULL)
		{
			int	iSaveError = m_iGetErrCode();
			VDBGO(((DEBUG_SOCKET|DEBUG_ERR) MYFN
				 "%s gethostbyname err!!\n", pcIPName));	
			m_vStrSockError( __FILE__, __LINE__, iSaveError);
			m_vCloseSd();
			WSACleanup();
			return;
		}
		int	ii;
		VDBGO(((DEBUG_SOCKET|DEBUG_INFO) MYFN "lpHostEnt h_name = %s\n",lpHostEnt->h_name));
		for (ii = 1 ;; ii++ )
		{
			if (lpHostEnt->h_aliases[ii-1] == 0)	break;
			VDBGO(((DEBUG_SOCKET|DEBUG_INFO) MYFN "lpHostEnt h_aliases[%d] = %s\n",ii, lpHostEnt->h_aliases[ii-1]));
		}

		VDBGO(((DEBUG_SOCKET|DEBUG_INFO) MYFN "lpHostEnt h_addrtype = %d\n",lpHostEnt->h_addrtype));
		VDBGO(((DEBUG_SOCKET|DEBUG_INFO) MYFN "lpHostEnt h_length = %d\n",lpHostEnt->h_length));
		for (ii = 1 ;; ii++ )
		{
			if (lpHostEnt->h_addr_list[ii-1] == 0)	break;
			VDBGO(((DEBUG_SOCKET|DEBUG_INFO) MYFN "lpHostEnt h_addr_list[%d] = \"%d.%d.%d.%d\"\n",ii, 
				(unsigned char)lpHostEnt->h_addr_list[ii-1][0],
				(unsigned char)lpHostEnt->h_addr_list[ii-1][1],
				(unsigned char)lpHostEnt->h_addr_list[ii-1][2],
				(unsigned char)lpHostEnt->h_addr_list[ii-1][3]));
		}
		//	先頭のアドレスを取得？
		char	caIP[32];
//	VC .Net2005　対応
		sprintf_s(caIP,sizeof(caIP)-1,"%d.%d.%d.%d",
									(unsigned char)lpHostEnt->h_addr_list[0][0],
									(unsigned char)lpHostEnt->h_addr_list[0][1],
									(unsigned char)lpHostEnt->h_addr_list[0][2],
									(unsigned char)lpHostEnt->h_addr_list[0][3]);
		SockAddr.sin_addr.s_addr = inet_addr(caIP); // My address

	VDBGO(((DEBUG_SOCKET|DEBUG_INFO) MYFN
		 "Sock Ip = %s Port = %d\n",
		 inet_ntoa(SockAddr.sin_addr), ntohs(SockAddr.sin_port)));	

	if (!(m_iModeFlag & CIPSOCK_MODE_RECV))
	{
		if ( !(m_iModeFlag & CIPSOCK_MODE_UDP) )
		{
			//	TCP 送信なのでコネクトする
			if(connect(m_sSd,(struct sockaddr *)&SockAddr,
				sizeof(sockaddr_in)) == -1)
			{
				int	iSaveError = m_iGetErrCode();
				VDBGO(((DEBUG_SOCKET|DEBUG_ERR) MYFN
					 "%s connect err!!\n", pcIPName));	
				m_vStrSockError( __FILE__, __LINE__, iSaveError);
				m_vCloseSd();
				WSACleanup();
				return;
			}
		}
		else	
			m_SockAddrSendTo = SockAddr;	// UDP送信に備えて保存
		//	Windows
		//	Windowsではノンブロッキングモード・ブロッキングモードをダイナミックに変更
		//	出来ない（推奨されない）ので、常時ノンブロッキングモードとする
		unsigned long	ulArg = TRUE;
		if (ioctlsocket(m_sSd, FIONBIO, &ulArg) == SOCKET_ERROR)
		{
			int	iSaveError = m_iGetErrCode();
			VDBGO(((DEBUG_SOCKET|DEBUG_ERR) MYFN
				 "%s ioctlsocket err!!\n", pcIPName));	
			m_vStrSockError( __FILE__, __LINE__, iSaveError);
			m_vCloseSd();
			WSACleanup();
			return;
		}
	}
	else
	{	//　受信なので・・・

		// bind the name to the socket
		//	前回スレッドでオープンのまま終了した場合、bindがエラーとなる
		//	ことがある、何度か繰り返して行なう

		for ( int iWait= 0 ; ;  )
		{
			if (bind(m_sSd,(struct sockaddr *)&SockAddr,
				 sizeof(sockaddr_in)) == -1)
			{
				int	iSaveError = m_iGetErrCode();
				if (iSaveError == WSAEADDRINUSE)
				{
					iWait++;
					if (iWait < 60)
					{	
						VDBGO(((DEBUG_SOCKET|DEBUG_WARN) MYFN
						 "%s bind err ( already use ) retry(%d)??\n", pcIPName, iWait));	
						Sleep(1000);	// Wait 1Sec
						continue;
					}
					//	幾らなんでもダメ！！
				}
				VDBGO(((DEBUG_SOCKET|DEBUG_ERR) MYFN "%s bind err!!\n", pcIPName));	
				m_vStrSockError( __FILE__, __LINE__, iSaveError);
				m_vCloseSd();
				WSACleanup();
				return;
			}
			break;
		}

		if (!(m_iModeFlag & CIPSOCK_MODE_UDP))
		{	//	TCPでの受信なので、listenする
			if ( listen(m_sSd, SOMAXCONN) == -1 )
			{
				int	iSaveError = m_iGetErrCode();
				VDBGO(((DEBUG_SOCKET|DEBUG_ERR) MYFN
					 "%s listen err!!\n", pcIPName));	
				m_vStrSockError( __FILE__, __LINE__, iSaveError);
				m_vCloseSd();
				WSACleanup();

				return;
			}
		}
		else
		{	//	UDPの場合Acceptが無いのでここで、ノンブロッキングモードに指定する
			unsigned long	ulArg = TRUE;
			if (ioctlsocket(m_sSd, FIONBIO, &ulArg) == SOCKET_ERROR)
			{
				int	iSaveError = m_iGetErrCode();
				VDBGO(((DEBUG_SOCKET|DEBUG_ERR) MYFN
					 "%x ioctlsocket err!!\n", m_sAccSd));	
				m_vStrSockError( __FILE__, __LINE__,iSaveError);
				m_vCloseSd();
				WSACleanup();
			}
		}
	}
#endif
		//--------------------------------------------------------------------------------
		/// <summary>
		///		accept	アクセプト待ち
		///		Notes   :
		///			アクセプト待ち　ソケットが張られるのを待つ
		///			アクセプト時はソケットハンドルクラスを戻す
		///		又、接続先のエンドポイントの情報も戻す
		///		History :			
		///			2009.09.03 SKYware
		/// </summary>
		/// <param name="_remoteEndPoint"></param>
		/// <returns></returns>
		public errStatus accept(
			ref Socket		_acceptSocket,	//	アクセプトしたソケットハンドル
			ref IPEndPoint	_remoteEndPoint	//	接続先のエンドポイント
			)
		{
			try
			{	
				_acceptSocket = socket.Accept();
				_remoteEndPoint = (IPEndPoint)_acceptSocket.RemoteEndPoint;
				_acceptSocket.Blocking = false;
			}
			catch(Exception e)
			{
				_com_vdbgo.vDbgoVerbose(_com_vdbgo.ComsrcInf, 
					"Exception caught!!! [accept] Source : {0} Message : {1}\r\n",
					e.Source, e.Message);
				return (errStatus.exception);
			}
			return(errStatus.noError);
		}

#if	USE_SLEEP_TIMER
		/// <summary>
		///		送受信タイマータイムアップ処理
		///		Notes   :
		///			最初のコールバック時に呼び出される
		///			Objectで渡されるイベントをセットする
		///		History :			
		///			2009.09.03 SKYware
		///		
		/// </summary>
		/// <param name="_event"></param>
		/// 
		private void	timeUpCallback(Object _event)
		{
			AutoResetEvent	_autoResetEvent = (AutoResetEvent)_event;
			_autoResetEvent.Set();
		}
#endif

		//--------------------------------------------------------------------------------
		/// <summary>
		///		recv	ソケット受信処理
		///		Notes   :
		///			タイマー付で待つ単位はmsとする
		///		History :			
		///			2009.09.03 SKYware
		///		
		/// </summary>
		/// <param name="_recvBuf"></param>
		/// <param name="_bufSize"></param>
		/// <param name="_SecTimer"></param>
		/// <param name="_recvSoclet"></param>
		/// <returns></returns>
		public	int	recv(
			byte[]	_recvBuf,		//　受信バッファ
			int		_bufSize,		//	受信バッファサイズ
			int		_mSecTimer,		//	タイムアウト時間(msec)"0"でブロックモード
			Socket	_recvSocket		//	受信ソケットクラス
		)
		{
			int	_recved = 0;
			int	_ret;

//			if ((socket == null) || (socket.Connected == false))
			if (socket == null)
			{
				_com_vdbgo.vDbgoVerbose(_com_vdbgo.ComsrcErr,
					"Invalid socket {0} abort send \r\n", socket);
				return ((int)errStatus.invalidSocket);
			}

			//	UDPでは受信ソケットクラスはソケットとなるので、置き換え
			if (_recvSocket == null) _recvSocket = socket;

			_recvSocket.ReceiveTimeout = _mSecTimer;
			_recvSocket.Blocking = true;
			_ret = 0;
			try
			{
				for ( ;_bufSize != 0 ; )
				{
					if (tcpSocket == true)	//	TCP
						_ret = _recvSocket.Receive(_recvBuf, _recved, _bufSize, SocketFlags.None);
					else				//	UDP
					{
						IPEndPoint _sender = new IPEndPoint(IPAddress.Any, 0);
						EndPoint _senderRemote = (EndPoint)_sender;

						_ret = _recvSocket.ReceiveFrom(_recvBuf, _recved, _bufSize, SocketFlags.None, ref _senderRemote);
					}
					if (_ret > 0)
					{
						_bufSize -= _ret;
						_recved += _ret;
					}
					else
					{
						_com_vdbgo.vDbgoVerbose(_com_vdbgo.ComsrcErr,
							"Recv Return is minus?? {0} abort recv \r\n", _ret);
						break;
					}

				}
			}
			catch (ArgumentNullException e)
			{
				_com_vdbgo.vDbgoVerbose(_com_vdbgo.ComsrcInf,
					"ArgumentNullException caught!!! [recv] Source : {0} Message : {1}\r\n",
					e.Source, e.Message);
				return ((int)errStatus.argumentNullException);
			}
			catch (ArgumentOutOfRangeException e)
			{
				_com_vdbgo.vDbgoVerbose(_com_vdbgo.ComsrcInf,
					"ArgumentOutOfRangeException caught!!! [recv] Source : {0} Message : {1}\r\n",
					e.Source, e.Message);
				return ((int)errStatus.argumentNullException);
			}
			catch (SocketException e)
			{
				_com_vdbgo.vDbgoVerbose(_com_vdbgo.ComsrcInf,
					"SocketException caught!!! [recv] Source : {0} Message : {1} ErrorCode : {2}\r\n",
					e.Source, e.Message, e.ErrorCode);
				//	タイムアウトエラー?
				switch (e.ErrorCode)
				{
					case 10060: return ((int)errStatus.timeOut);
					case 10054: return ((int)errStatus.disconnected);
				}
				return ((int)errStatus.socketException);
			}
			catch (ObjectDisposedException e)
			{
				_com_vdbgo.vDbgoVerbose(_com_vdbgo.ComsrcInf,
					"ObjectDisposedException caught!!! [recv] Source : {0} Message : {1}\r\n",
					e.Source, e.Message);
				return ((int)errStatus.objectDisposedException);
			}
			catch (SecurityException e)
			{
				_com_vdbgo.vDbgoVerbose(_com_vdbgo.ComsrcInf,
					"SecurityException caught!!! [recv] Source : {0} Message : {1}\r\n",
					e.Source, e.Message);
				return ((int)errStatus.securityException);
			}
			return(_recved);
		}
		//--------------------------------------------------------------------------------
		/// <summary>
		///		send	ソケット送信処理
		///		Notes   :
		///			タイマー付で待つ単位はmsとする0でBlockとなる
		///			(500ms以下は500msとなる、Socketクラスの仕様制限）
		///		History :			
		///			2009.09.03 SKYware
		/// 
		/// </summary>
		/// <param name="_sendBuf"></param>
		/// <param name="_bufSize"></param>
		/// <param name="_mSecTimer"></param>
		/// <param name="_recvSoclet"></param>
		/// <returns>
		///		転送したバイト数
		/// </returns>
		public int send(
			byte[]	_sendBuf,		//　送信バッファ
			int		_bufSize,		//	受信バッファサイズ
			int		_mSecTimer,		//	タイムアウト時間(msec)"0"でブロックモード
			Socket	_sendSocket		//	受信ソケットクラス
			)
		{
			int	_sended = 0;
			int	_ret;

			if ((socket == null) || ((tcpSocket == true) && (socket.Connected == false)))
			{
				_com_vdbgo.vDbgoVerbose(_com_vdbgo.ComsrcErr,
					"Invalid socket {0} abort send \r\n", socket);
				return ((int)errStatus.invalidSocket);
			}

			socket.SendTimeout = _mSecTimer;
			_ret = 0;
			try
			{
				for ( ;_bufSize > 0 ; )
				{
					_ret = -1;
					if (tcpSocket == true)	//	TCP
						_ret = socket.Send(_sendBuf, _sended, _bufSize, SocketFlags.Partial);
					else				//	UDP
					{
						_ret = socket.SendTo(_sendBuf, _sended, _bufSize, SocketFlags.None, endPoint); 
					}
					if (_ret > 0)
					{
						_sended += _ret;	//	送信開始位置を更新
						_bufSize -= _ret;	//	未送信のバッファ数を更新
					}
					else
					{
						_com_vdbgo.vDbgoVerbose(_com_vdbgo.ComsrcErr,
							"Send Return is minus?? {0} abort send \r\n",_ret);
						break;
					}
				}
			}
			catch (ArgumentNullException e)
			{
				_com_vdbgo.vDbgoVerbose(_com_vdbgo.ComsrcInf,
					"ArgumentNullException caught!!! [send] Source : {0} Message : {1}\r\n",
					e.Source, e.Message);
				return ((int)errStatus.argumentNullException);
			}
			catch (ArgumentOutOfRangeException e)
			{
				_com_vdbgo.vDbgoVerbose(_com_vdbgo.ComsrcInf,
					"ArgumentOutOfRangeException caught!!! [send] Source : {0} Message : {1}\r\n",
					e.Source, e.Message);
				return ((int)errStatus.argumentNullException);
			}
			catch (SocketException e)
			{
				_com_vdbgo.vDbgoVerbose(_com_vdbgo.ComsrcInf,
					"SocketException caught!!! [send] Source : {0} Message : {1} ErrorCode : {2}\r\n",
					e.Source, e.Message,e.ErrorCode);
				//	タイムアウトエラー?
				switch(e.ErrorCode)
				{
					case	10060:	return ((int)errStatus.timeOut);
					case	10054:	return ((int)errStatus.disconnected);
				}
				return ((int)errStatus.socketException);
			}
			catch (ObjectDisposedException e)
			{
				_com_vdbgo.vDbgoVerbose(_com_vdbgo.ComsrcInf,
					"ObjectDisposedException caught!!! [send] Source : {0} Message : {1}\r\n",
					e.Source, e.Message);
				return ((int)errStatus.objectDisposedException);
			}

			return(_sended);
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		///		Acceptで作成されたソケットをクローズする
		/// </summary>
		/// <param name="AccSocket"></param>
		public void	closeAccSocket(Socket accSocket)
		{
			if (accSocket !=null)	accSocket.Close();
		}

		/// <summary>
		///		ソケットをクローズする
		/// </summary>
		public	void	close()
		{
			if (socket == null)	return;
			socket.Close();
		}
	}
}

#if false

//----------------------------------------------------------------------
// 関数名: iAccept
//----------------------------------------------------------------------
// Summary :
//   アクセプト待ち　ソケットが張られるのを待つ
// OutLine :
//   指定サイズ送信終了まで戻らない（エラーを除く）
// Return  :
//   受信ソケットディスクリプタ
//   Windowsの場合常にノンブロッキングモードで動作させる
// Notes   :
//   エラーになっても、ソケットはクローズしない
//   サーバで複数ソケットが張られている可能性があるので
//	 この処理で取得したディスクリプタは呼び出し側が、vCloseSd()でクローズ
//   すること！！
//   Windowsで受信の場合はAcceptの後、ノンブロッキングモードとする
//   アクセプト中は常にブロッキングモードとする
//   LinuxでNonブロックで待つ場合あaccept()はerr=11
//   "Resource temporarily unavailable"となる
// History :
//   2004.08.20 typed
//----------------------------------------------------------------------
int	CIpSock::iAccept(
			struct	sockaddr_in	*pFromSock,	//	送信元ソケット情報す
			int		iBlockFlag				//	ブロックフラグ
	)
{
	struct sockaddr_in	FromSock;
	int		iLen;

	if (m_sSd == 0)	return(FALSE);

	//	Non ブロッキングモードの設定
	if (iBlockFlag == FALSE)
	{
	}
	iLen = sizeof(struct sockaddr);
	if ( (m_sAccSd	= accept(	m_sSd,
								(struct sockaddr *)&FromSock,
								(socklen_t*)&iLen)) == INVALID_SOCKET)

	{
		int	iSaveError = m_iGetErrCode();
		VDBGO(((DEBUG_SOCKET|DEBUG_ERR) MYFN
			 "%x accept err!!\n", m_sSd));	
		m_vStrSockError( __FILE__, __LINE__,iSaveError);
		m_sAccSd = FALSE;
	}
	else
	{
		//	acceptで受け取ったFromSockは正しくないことがあるので・・
		//　湯原さん直伝の方法で・・・
		int	size = sizeof(FromSock);
		if ( getpeername(m_sAccSd, (struct sockaddr *)&FromSock,(socklen_t *)&size) == -1)
		{
			int	iSaveError = m_iGetErrCode();
			VDBGO(((DEBUG_SOCKET|DEBUG_ERR) MYFN
				 "%x getpeername err!!\n", m_sSd));	
			m_vStrSockError( __FILE__, __LINE__,iSaveError);
			m_sAccSd = FALSE;
		}
		else
		{
			// 送信元情報
			*pFromSock = m_SenderSock = FromSock;
	
			VDBGO(((DEBUG_SOCKET|DEBUG_INFO) MYFN "Sender Ip = %s Port = %d\n",
				 inet_ntoa(pFromSock->sin_addr), ntohs(pFromSock->sin_port)));	
		}
	}
	//	Windows
	//	Windowsではノンブロッキングモード・ブロッキングモードをダイナミックに変更
	//	出来ない（推奨されない）ので、常時ノンブロッキングモードとする
	if (m_sAccSd)
	{
		unsigned long	ulArg = TRUE;
		if (ioctlsocket(m_sAccSd, FIONBIO, &ulArg) == SOCKET_ERROR)
		{
			int	iSaveError = m_iGetErrCode();
			VDBGO(((DEBUG_SOCKET|DEBUG_ERR) MYFN
				 "%x ioctlsocket err!!\n", m_sAccSd));	
			m_vStrSockError( __FILE__, __LINE__,iSaveError);
			return(FALSE);
		}
	}

	return(m_sAccSd);
}

//----------------------------------------------------------------------
// 関数名: CloseAccSd()
//----------------------------------------------------------------------
// Summary :
//   Acceptで作成されたソケットディスクリプタをクローズする
// OutLine :
//   
// Return  :
//   void
// Notes   :
//   .
// History :
//   2004.08.20 typed
//----------------------------------------------------------------------
void	CIpSock::vCloseAccSd(SOCKET iAccSd)
{
	if (iAccSd)
	{
		closesocket(iAccSd);
	}
}

//----------------------------------------------------------------------
// 関数名: m_CloseSd()
//----------------------------------------------------------------------
// Summary :
//   ソケットディスクリプタをクローズする
// OutLine :
//   オープンしているソケットをクローズする
// Return  :
//   void
// Notes   :
//   .
// History :
//   2004.08.20 typed
//----------------------------------------------------------------------
void	CIpSock::m_vCloseSd()
{
//	if (m_sAccSd)
//	{
//		close(m_sAccSd);
//		m_sAccSd = 0;
//	}
	if (m_sSd)
	{
		closesocket(m_sSd);
		m_sSd = 0;
	}
}

//----------------------------------------------------------------------
// 関数名: CIpSock
//----------------------------------------------------------------------
// Summary :
//   CIpSockクラスコンストラクタ
// OutLine :
//   指定されたデバイス名でソケットを作成する
// Return  :
//   void
// Notes   :
//   複数のデバイス（カード）がインストールされているので、指定された
//   カードにアクセスするよう、制御する
//   Windowsで送信の場合この時点でノンブロッキングモードとする
//　 Windowsで受信の場合はAcceptの後、ノンブロッキングモードとする
// History :
//   2004.08.20 typed
//   2005.11.28 Add UDP
//----------------------------------------------------------------------
CIpSock::CIpSock(
		const char	*pcIPName,	// 送信先又は自局のIPアドレス又はホスト名
		int			iPortNum,	// ソケットのポート番号(送信先、又は自局）
		int			iModeFlag	// bit0
								//	1で受信
								//	0で送信
								// bit1
								//	1でpcIPNameはデバイス名(linuxのみ）
								//	0でpcIPNameはIPアドレス又はホスト名
								// bit2
								//	1でUDP
								//	0でTCP

	):
	m_iModeFlag(iModeFlag),
	m_sSd(FALSE),	
	m_sAccSd(FALSE)
{
	sockaddr_in	SockAddr;

	WSADATA	wsaData;
	WORD wVersionRequested = MAKEWORD(1,1);
	int	iRet;

	if ( (iRet = WSAStartup(wVersionRequested, &wsaData)) != 0)
	{
		VDBGO(((DEBUG_SOCKET|DEBUG_ERR) MYFN "WSAStartup Err!!\n"));	
		m_vStrSockError( __FILE__, __LINE__, iRet);
		return;
	}
	if (wsaData.wVersion != wVersionRequested)
	{	
		VDBGO(((DEBUG_SOCKET|DEBUG_ERR) MYFN "wVersionRequested(=%d) Actual(%d) miss match \n",
			wVersionRequested, wsaData.wVersion));	
		WSACleanup();
		return;
	}

	//	ソケットのオープン
	if (m_iModeFlag & CIPSOCK_MODE_UDP)
		m_sSd = socket(AF_INET,SOCK_DGRAM,IPPROTO_UDP);
	else
		m_sSd = socket(AF_INET,SOCK_STREAM,IPPROTO_TCP);
	if ( m_sSd == -1)
	{
		int	iSaveError = m_iGetErrCode();
		VDBGO(((DEBUG_SOCKET|DEBUG_ERR) MYFN "socket open err!!\n"));	
		m_vStrSockError( __FILE__, __LINE__,iSaveError);
		WSACleanup();
		return;
	}

    SockAddr.sin_family = AF_INET;
	SockAddr.sin_port = htons((unsigned short)iPortNum);

	//	IPアドレスを取得する
		struct hostent* lpHostEnt;
		if ( (lpHostEnt = gethostbyname(pcIPName)) == NULL)
		{
			int	iSaveError = m_iGetErrCode();
			VDBGO(((DEBUG_SOCKET|DEBUG_ERR) MYFN
				 "%s gethostbyname err!!\n", pcIPName));	
			m_vStrSockError( __FILE__, __LINE__, iSaveError);
			m_vCloseSd();
			WSACleanup();
			return;
		}
		int	ii;
		VDBGO(((DEBUG_SOCKET|DEBUG_INFO) MYFN "lpHostEnt h_name = %s\n",lpHostEnt->h_name));
		for (ii = 1 ;; ii++ )
		{
			if (lpHostEnt->h_aliases[ii-1] == 0)	break;
			VDBGO(((DEBUG_SOCKET|DEBUG_INFO) MYFN "lpHostEnt h_aliases[%d] = %s\n",ii, lpHostEnt->h_aliases[ii-1]));
		}

		VDBGO(((DEBUG_SOCKET|DEBUG_INFO) MYFN "lpHostEnt h_addrtype = %d\n",lpHostEnt->h_addrtype));
		VDBGO(((DEBUG_SOCKET|DEBUG_INFO) MYFN "lpHostEnt h_length = %d\n",lpHostEnt->h_length));
		for (ii = 1 ;; ii++ )
		{
			if (lpHostEnt->h_addr_list[ii-1] == 0)	break;
			VDBGO(((DEBUG_SOCKET|DEBUG_INFO) MYFN "lpHostEnt h_addr_list[%d] = \"%d.%d.%d.%d\"\n",ii, 
				(unsigned char)lpHostEnt->h_addr_list[ii-1][0],
				(unsigned char)lpHostEnt->h_addr_list[ii-1][1],
				(unsigned char)lpHostEnt->h_addr_list[ii-1][2],
				(unsigned char)lpHostEnt->h_addr_list[ii-1][3]));
		}
		//	先頭のアドレスを取得？
		char	caIP[32];
//	VC .Net2005　対応
		sprintf_s(caIP,sizeof(caIP)-1,"%d.%d.%d.%d",
									(unsigned char)lpHostEnt->h_addr_list[0][0],
									(unsigned char)lpHostEnt->h_addr_list[0][1],
									(unsigned char)lpHostEnt->h_addr_list[0][2],
									(unsigned char)lpHostEnt->h_addr_list[0][3]);
		SockAddr.sin_addr.s_addr = inet_addr(caIP); // My address

	VDBGO(((DEBUG_SOCKET|DEBUG_INFO) MYFN
		 "Sock Ip = %s Port = %d\n",
		 inet_ntoa(SockAddr.sin_addr), ntohs(SockAddr.sin_port)));	

	if (!(m_iModeFlag & CIPSOCK_MODE_RECV))
	{
		if ( !(m_iModeFlag & CIPSOCK_MODE_UDP) )
		{
			//	TCP 送信なのでコネクトする
			if(connect(m_sSd,(struct sockaddr *)&SockAddr,
				sizeof(sockaddr_in)) == -1)
			{
				int	iSaveError = m_iGetErrCode();
				VDBGO(((DEBUG_SOCKET|DEBUG_ERR) MYFN
					 "%s connect err!!\n", pcIPName));	
				m_vStrSockError( __FILE__, __LINE__, iSaveError);
				m_vCloseSd();

				WSACleanup();

				return;
			}
		}
		else	
			m_SockAddrSendTo = SockAddr;	// UDP送信に備えて保存

		//	Windows
		//	Windowsではノンブロッキングモード・ブロッキングモードをダイナミックに変更
		//	出来ない（推奨されない）ので、常時ノンブロッキングモードとする
		unsigned long	ulArg = TRUE;
		if (ioctlsocket(m_sSd, FIONBIO, &ulArg) == SOCKET_ERROR)
		{
			int	iSaveError = m_iGetErrCode();
			VDBGO(((DEBUG_SOCKET|DEBUG_ERR) MYFN
				 "%s ioctlsocket err!!\n", pcIPName));	
			m_vStrSockError( __FILE__, __LINE__, iSaveError);
			m_vCloseSd();
			WSACleanup();
			return;
		}

	}
	else
	{	//　受信なので・・・

		// bind the name to the socket
		//	前回スレッドでオープンのまま終了した場合、bindがエラーとなる
		//	ことがある、何度か繰り返して行なう

		for ( int iWait= 0 ; ;  )
		{
			if (bind(m_sSd,(struct sockaddr *)&SockAddr,
				 sizeof(sockaddr_in)) == -1)
			{
				int	iSaveError = m_iGetErrCode();
				if (iSaveError == WSAEADDRINUSE)
				{
					iWait++;
					if (iWait < 60)
					{	
						VDBGO(((DEBUG_SOCKET|DEBUG_WARN) MYFN
						 "%s bind err ( already use ) retry(%d)??\n", pcIPName, iWait));	
						Sleep(1000);	// Wait 1Sec
						continue;
					}
					//	幾らなんでもダメ！！
				}
				VDBGO(((DEBUG_SOCKET|DEBUG_ERR) MYFN "%s bind err!!\n", pcIPName));	
				m_vStrSockError( __FILE__, __LINE__, iSaveError);
				m_vCloseSd();

				WSACleanup();

				return;
			}
			break;
		}

		if (!(m_iModeFlag & CIPSOCK_MODE_UDP))
		{	//	TCPでの受信なので、listenする
			if ( listen(m_sSd, SOMAXCONN) == -1 )
			{
				int	iSaveError = m_iGetErrCode();
				VDBGO(((DEBUG_SOCKET|DEBUG_ERR) MYFN
					 "%s listen err!!\n", pcIPName));	
				m_vStrSockError( __FILE__, __LINE__, iSaveError);
				m_vCloseSd();

				WSACleanup();

				return;
			}
		}

		else
		{	//	UDPの場合Acceptが無いのでここで、ノンブロッキングモードに指定する
			unsigned long	ulArg = TRUE;
			if (ioctlsocket(m_sSd, FIONBIO, &ulArg) == SOCKET_ERROR)
			{
				int	iSaveError = m_iGetErrCode();
				VDBGO(((DEBUG_SOCKET|DEBUG_ERR) MYFN
					 "%x ioctlsocket err!!\n", m_sAccSd));	
				m_vStrSockError( __FILE__, __LINE__,iSaveError);
				m_vCloseSd();
				WSACleanup();
			}
		}

	}
}


//----------------------------------------------------------------------
// 関数名: ~CIpSock
//----------------------------------------------------------------------
// Summary :
//   CIpSockクラスデストラクタ
// OutLine :
//   .
// Return  :
//   void
// Notes   :
//   .
// History :
//   2004.08.20 typed
//----------------------------------------------------------------------
CIpSock::~CIpSock()
{
	m_vCloseSd();
	WSACleanup();
}	
#endif
//----------------------------------------------------------------------
// 富士通静岡エンジニアリング「C++仕様書工房v1」用の記述形式
//----------------------------------------------------------------------
