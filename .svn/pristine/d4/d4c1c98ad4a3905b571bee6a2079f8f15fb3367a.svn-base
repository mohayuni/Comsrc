' <<<<<<emustbを起動するWSH>>>>>
' <<"cscript emutest.vbs"で起動する。>>

Option Explicit
Dim WshShell, oExec

Dim IpAddr
Dim PortNum
Dim Interval
Dim WaitMsec
Dim Count
Dim SendTimeOut
Dim BlockSize
Dim LoopCount

'初期値設定

IpAddr="192.168.200.21" ' 配信サーバIPアドレス
PortNum=50000       	' ポート番号
BlockSize=10240			' 転送ブロックサイズ
LoopCount=10000			' 転送ブロック送信回数(0=無限)
SendTimeOut=0			' 送信タイムアウト(ミリ秒単位 0=タイムアウト無)
Interval=0				' ブロック間のインターバル(ミリ秒単位)
Count=10                ' 起動するemustb数
WaitMsec=1200			' 次のtcpsend起動までの待機時間(ミリ秒）

Set WshShell = CreateObject("WScript.Shell")

Dim CmdStr

Do While Count <> 0
    '　起動コマンド列の作成
    CmdStr="bin\debug\tcpsend.exe"
    CmdStr=CmdStr+" /U:" & IpAddr		'　配信サーバIPアドレス指定
    CmdStr=CmdStr+" /N:" & PortNum		'  ポート番号
    CmdStr=CmdStr+" /B:" & BlockSize	'  転送ブロックサイズ
    CmdStr=CmdStr+" /L:" & LoopCount	'  転送ブロック送信回数
    CmdStr=CmdStr+" /W:" & SendTimeOut	'  ログインのみの場合、サーバへのアクセス間隔(秒単位)
    CmdStr=CmdStr+" /I:" & Interval		'  ブロック間のインターバル(ミリ秒単位)
    CmdStr=CmdStr+" /X:"				'  UDP

    Wscript.Echo( CmdStr )
    WshShell.run(CmdStr)
'    Count=Count-1
    WScript.Sleep WaitMsec
Loop

