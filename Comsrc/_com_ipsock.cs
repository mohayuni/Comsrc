//----------------------------------------------------------------------
// (C) Copyright 2009 SKYware Co.,Ltd. All rights reserved.
//----------------------------------------------------------------------
// <Module Name> TCP/IP�EUDP/IP�\�P�b�g�N���X
//----------------------------------------------------------------------
// <File Name>   _com_ipsock.cs
//----------------------------------------------------------------------
// <Description>
//   TCP/IP�EUDP/IP�\�P�b�g�N���X����������
// <History>
//   2008.09.02 from cipsock.cpp���
//----------------------------------------------------------------------
// <Notes>
//	 �ȉ��̃I�v�V����������
//	�@�ETCP/UDP
//    �E�T�[�o����/�N���C�A���g����
//----------------------------------------------------------------------/

//----------------------------------------------------------------------
//	����������������`													
//----------------------------------------------------------------------

//----------------------------------------------------------------------
// using�f�B���N�e�B�u�錾
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
	//-----�萔��`--------------------------------------------------------------------
		public enum errStatus
		{
			noError = 0,					//	�G���[��
			socketException = -100,			//	SocketException
			argumentNullException,			//	ArgumentNullException
			argumentOutOfRangeException,	//	ArgumentOutOfRangeException
			nullReferenceException,			//	NullReferenceException
			objectDisposedException,		//	ObjectDisposedException
			securityException,				//	SecurityException
			exception,						//	other Exception
			invalidSocket,					//	�s���\�P�b�g�N���X
			timeOut,						//	�^�C���A�E�g�G���[
			disconnected,					//	�ڑ��悪�ؒf
		}

	//-----�v���p�e�B�̒�`--------------------------------------------------------------------
//		TcpClient tcpClient = null;			//	TcpClient�N���X
//		TcpListener tcpServer = null;		//	TcpListener�N���X

		
//		NetworkStream networkstream =null;	//	Read/WriteStream�N���X
		private	Socket	socket = null;		//	��{�̃\�P�b�g�N���X
		private const int backLog = 5;		//	�ۗ����̐ڑ��̃L���[�̍ő咷�B
											//	�f�B�t�H���g��5? 5�`200���K�؂Ȓl�H
		private bool tcpSocket = true;		//	TCP�̃\�P�b�g
		private bool serverSocket = true;	//	�T�[�o���̃\�P�b�g
		private IPEndPoint endPoint;		//	���M��G���h�|�C���g/��M�G���h�|�C���g

	//-----���\�b�h�̒�`--------------------------------------------------------------------

		//--------------------------------------------------------------------------------
		/// <summary>
		///		_com_ipsock	�R���X�g���N�^
		///		Notes   :
		///			�w�肳�ꂽ�����Ń\�P�b�g���쐬����
		///			�w�������
		///				�T�[�o��IP�A�h���X���̓z�X�g��
		///				���[�΂̃A�N�Z�X�|�[�g
		///				TCP/UDP�v���g�R��
		///				�N���C�A���g/�T�[�o
		///			���ӓ_
		///				�T�[�o�w��ŁA�����̃C���^�t�F�[�X�����݂�����ŁA�z�X�g��
		///				���w�肵���ꍇ�A�ŏ��̃C���^�t�F�[�X���g�p�����
		///				���҂����C���^�[�t�F�[�X���g�p����ꍇ�́AIP�A�h���X�w�肪
		///				�]�܂���
		///				�}���`�L���X�g���s���ꍇ
		///				IP�A�h���X�͌���X�R�[�v�A�h���X 239.0.0.0 �` 239.255.255.255 
		///				�|�[�g�ԍ�
		///		History :			
		///			2009.09.03 SKYware
		/// </summary>
		/// <param name="iPName"></param>
		/// <param name="portNum"></param>
		/// <param name="tcp"></param>
		/// <param name="server"></param>
		public	_com_ipsock(
			string	iPName,		//	���M�斔�͎��ǂ�IP�A�h���X���̓z�X�g��
			int		portNum,	//	�\�P�b�g�̃|�[�g�ԍ�(���M��A���͎��ǁj
			bool	tcp,		//	true..TCP/false..UDP
			bool	server		//	true..Sever/false..Client
				)
		{
			tcpSocket = tcp;
			serverSocket = server;
			//	�\�P�b�g�̃I�[�v��
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
						//	iPName��IP��
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

	//	IP�A�h���X���擾����
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
		//	�擪�̃A�h���X���擾�H
		char	caIP[32];
//	VC .Net2005�@�Ή�
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
			//	TCP ���M�Ȃ̂ŃR�l�N�g����
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
			m_SockAddrSendTo = SockAddr;	// UDP���M�ɔ����ĕۑ�
		//	Windows
		//	Windows�ł̓m���u���b�L���O���[�h�E�u���b�L���O���[�h���_�C�i�~�b�N�ɕύX
		//	�o���Ȃ��i��������Ȃ��j�̂ŁA�펞�m���u���b�L���O���[�h�Ƃ���
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
	{	//�@��M�Ȃ̂ŁE�E�E

		// bind the name to the socket
		//	�O��X���b�h�ŃI�[�v���̂܂܏I�������ꍇ�Abind���G���[�ƂȂ�
		//	���Ƃ�����A���x���J��Ԃ��čs�Ȃ�

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
					//	���Ȃ�ł��_���I�I
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
		{	//	TCP�ł̎�M�Ȃ̂ŁAlisten����
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
		{	//	UDP�̏ꍇAccept�������̂ł����ŁA�m���u���b�L���O���[�h�Ɏw�肷��
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
		///		accept	�A�N�Z�v�g�҂�
		///		Notes   :
		///			�A�N�Z�v�g�҂��@�\�P�b�g��������̂�҂�
		///			�A�N�Z�v�g���̓\�P�b�g�n���h���N���X��߂�
		///		���A�ڑ���̃G���h�|�C���g�̏����߂�
		///		History :			
		///			2009.09.03 SKYware
		/// </summary>
		/// <param name="_remoteEndPoint"></param>
		/// <returns></returns>
		public errStatus accept(
			ref Socket		_acceptSocket,	//	�A�N�Z�v�g�����\�P�b�g�n���h��
			ref IPEndPoint	_remoteEndPoint	//	�ڑ���̃G���h�|�C���g
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
		///		����M�^�C�}�[�^�C���A�b�v����
		///		Notes   :
		///			�ŏ��̃R�[���o�b�N���ɌĂяo�����
		///			Object�œn�����C�x���g���Z�b�g����
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
		///		recv	�\�P�b�g��M����
		///		Notes   :
		///			�^�C�}�[�t�ő҂P�ʂ�ms�Ƃ���
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
			byte[]	_recvBuf,		//�@��M�o�b�t�@
			int		_bufSize,		//	��M�o�b�t�@�T�C�Y
			int		_mSecTimer,		//	�^�C���A�E�g����(msec)"0"�Ńu���b�N���[�h
			Socket	_recvSocket		//	��M�\�P�b�g�N���X
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

			//	UDP�ł͎�M�\�P�b�g�N���X�̓\�P�b�g�ƂȂ�̂ŁA�u������
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
				//	�^�C���A�E�g�G���[?
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
		///		send	�\�P�b�g���M����
		///		Notes   :
		///			�^�C�}�[�t�ő҂P�ʂ�ms�Ƃ���0��Block�ƂȂ�
		///			(500ms�ȉ���500ms�ƂȂ�ASocket�N���X�̎d�l�����j
		///		History :			
		///			2009.09.03 SKYware
		/// 
		/// </summary>
		/// <param name="_sendBuf"></param>
		/// <param name="_bufSize"></param>
		/// <param name="_mSecTimer"></param>
		/// <param name="_recvSoclet"></param>
		/// <returns>
		///		�]�������o�C�g��
		/// </returns>
		public int send(
			byte[]	_sendBuf,		//�@���M�o�b�t�@
			int		_bufSize,		//	��M�o�b�t�@�T�C�Y
			int		_mSecTimer,		//	�^�C���A�E�g����(msec)"0"�Ńu���b�N���[�h
			Socket	_sendSocket		//	��M�\�P�b�g�N���X
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
						_sended += _ret;	//	���M�J�n�ʒu���X�V
						_bufSize -= _ret;	//	�����M�̃o�b�t�@�����X�V
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
				//	�^�C���A�E�g�G���[?
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
		///		Accept�ō쐬���ꂽ�\�P�b�g���N���[�Y����
		/// </summary>
		/// <param name="AccSocket"></param>
		public void	closeAccSocket(Socket accSocket)
		{
			if (accSocket !=null)	accSocket.Close();
		}

		/// <summary>
		///		�\�P�b�g���N���[�Y����
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
// �֐���: iAccept
//----------------------------------------------------------------------
// Summary :
//   �A�N�Z�v�g�҂��@�\�P�b�g��������̂�҂�
// OutLine :
//   �w��T�C�Y���M�I���܂Ŗ߂�Ȃ��i�G���[�������j
// Return  :
//   ��M�\�P�b�g�f�B�X�N���v�^
//   Windows�̏ꍇ��Ƀm���u���b�L���O���[�h�œ��삳����
// Notes   :
//   �G���[�ɂȂ��Ă��A�\�P�b�g�̓N���[�Y���Ȃ�
//   �T�[�o�ŕ����\�P�b�g�������Ă���\��������̂�
//	 ���̏����Ŏ擾�����f�B�X�N���v�^�͌Ăяo�������AvCloseSd()�ŃN���[�Y
//   ���邱�ƁI�I
//   Windows�Ŏ�M�̏ꍇ��Accept�̌�A�m���u���b�L���O���[�h�Ƃ���
//   �A�N�Z�v�g���͏�Ƀu���b�L���O���[�h�Ƃ���
//   Linux��Non�u���b�N�ő҂ꍇ��accept()��err=11
//   "Resource temporarily unavailable"�ƂȂ�
// History :
//   2004.08.20 typed
//----------------------------------------------------------------------
int	CIpSock::iAccept(
			struct	sockaddr_in	*pFromSock,	//	���M���\�P�b�g���
			int		iBlockFlag				//	�u���b�N�t���O
	)
{
	struct sockaddr_in	FromSock;
	int		iLen;

	if (m_sSd == 0)	return(FALSE);

	//	Non �u���b�L���O���[�h�̐ݒ�
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
		//	accept�Ŏ󂯎����FromSock�͐������Ȃ����Ƃ�����̂ŁE�E
		//�@�������񒼓`�̕��@�ŁE�E�E
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
			// ���M�����
			*pFromSock = m_SenderSock = FromSock;
	
			VDBGO(((DEBUG_SOCKET|DEBUG_INFO) MYFN "Sender Ip = %s Port = %d\n",
				 inet_ntoa(pFromSock->sin_addr), ntohs(pFromSock->sin_port)));	
		}
	}
	//	Windows
	//	Windows�ł̓m���u���b�L���O���[�h�E�u���b�L���O���[�h���_�C�i�~�b�N�ɕύX
	//	�o���Ȃ��i��������Ȃ��j�̂ŁA�펞�m���u���b�L���O���[�h�Ƃ���
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
// �֐���: CloseAccSd()
//----------------------------------------------------------------------
// Summary :
//   Accept�ō쐬���ꂽ�\�P�b�g�f�B�X�N���v�^���N���[�Y����
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
// �֐���: m_CloseSd()
//----------------------------------------------------------------------
// Summary :
//   �\�P�b�g�f�B�X�N���v�^���N���[�Y����
// OutLine :
//   �I�[�v�����Ă���\�P�b�g���N���[�Y����
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
// �֐���: CIpSock
//----------------------------------------------------------------------
// Summary :
//   CIpSock�N���X�R���X�g���N�^
// OutLine :
//   �w�肳�ꂽ�f�o�C�X���Ń\�P�b�g���쐬����
// Return  :
//   void
// Notes   :
//   �����̃f�o�C�X�i�J�[�h�j���C���X�g�[������Ă���̂ŁA�w�肳�ꂽ
//   �J�[�h�ɃA�N�Z�X����悤�A���䂷��
//   Windows�ő��M�̏ꍇ���̎��_�Ńm���u���b�L���O���[�h�Ƃ���
//�@ Windows�Ŏ�M�̏ꍇ��Accept�̌�A�m���u���b�L���O���[�h�Ƃ���
// History :
//   2004.08.20 typed
//   2005.11.28 Add UDP
//----------------------------------------------------------------------
CIpSock::CIpSock(
		const char	*pcIPName,	// ���M�斔�͎��ǂ�IP�A�h���X���̓z�X�g��
		int			iPortNum,	// �\�P�b�g�̃|�[�g�ԍ�(���M��A���͎��ǁj
		int			iModeFlag	// bit0
								//	1�Ŏ�M
								//	0�ő��M
								// bit1
								//	1��pcIPName�̓f�o�C�X��(linux�̂݁j
								//	0��pcIPName��IP�A�h���X���̓z�X�g��
								// bit2
								//	1��UDP
								//	0��TCP

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

	//	�\�P�b�g�̃I�[�v��
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

	//	IP�A�h���X���擾����
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
		//	�擪�̃A�h���X���擾�H
		char	caIP[32];
//	VC .Net2005�@�Ή�
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
			//	TCP ���M�Ȃ̂ŃR�l�N�g����
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
			m_SockAddrSendTo = SockAddr;	// UDP���M�ɔ����ĕۑ�

		//	Windows
		//	Windows�ł̓m���u���b�L���O���[�h�E�u���b�L���O���[�h���_�C�i�~�b�N�ɕύX
		//	�o���Ȃ��i��������Ȃ��j�̂ŁA�펞�m���u���b�L���O���[�h�Ƃ���
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
	{	//�@��M�Ȃ̂ŁE�E�E

		// bind the name to the socket
		//	�O��X���b�h�ŃI�[�v���̂܂܏I�������ꍇ�Abind���G���[�ƂȂ�
		//	���Ƃ�����A���x���J��Ԃ��čs�Ȃ�

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
					//	���Ȃ�ł��_���I�I
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
		{	//	TCP�ł̎�M�Ȃ̂ŁAlisten����
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
		{	//	UDP�̏ꍇAccept�������̂ł����ŁA�m���u���b�L���O���[�h�Ɏw�肷��
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
// �֐���: ~CIpSock
//----------------------------------------------------------------------
// Summary :
//   CIpSock�N���X�f�X�g���N�^
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
// �x�m�ʐÉ��G���W�j�A�����O�uC++�d�l���H�[v1�v�p�̋L�q�`��
//----------------------------------------------------------------------
