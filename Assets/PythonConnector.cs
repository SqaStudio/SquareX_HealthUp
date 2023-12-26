using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using Debug = System.Diagnostics.Debug;
using Vector2 = System.Numerics.Vector2;

public class SocketConnection
{
    public string connectionIP;
    public int connectionPort;
    public delegate void PosChanged(string v);
    public PosChanged posChangedCallBack;

    private Thread mThread;
    //private IPAddress localAddress;
    private TcpListener _listener;
    private TcpClient _client;
    private byte[] buffer = new byte[65536];

    public SocketConnection(string ip,  int port)
    {
        connectionIP = ip;
        connectionPort = port;
        ThreadStart ts = new ThreadStart(GetInfo);
        mThread = new Thread(ts);
        mThread.Start();
    }

    private void GetInfo()
    {
        //localAddress = IPAddress.Parse(connectionIP);
        _listener = new TcpListener(IPAddress.Any, connectionPort);
        _listener.Start();

        _client = _listener.AcceptTcpClient();

        while (_client.Connected)
        {
            string dataReceived = SendAndReceiveData();
            if (string.IsNullOrWhiteSpace(dataReceived))
            {
                Debug.WriteLine("DATA NULL");
                break;
            }
            else
            {
                //Debug.WriteLine("GOT DATA: " + dataReceived);
                var value = dataReceived;
                posChangedCallBack.Invoke(value);
            }
            // Add your data processing logic here
        }
    }
    private string SendAndReceiveData()
    {
        NetworkStream mwStream = _client.GetStream();

        // RECEIVING DATA FROM THE HOST
        int bytesRead = mwStream.Read(buffer, 0, _client.ReceiveBufferSize);
        return Encoding.UTF8.GetString(buffer, 0, bytesRead);
    }
}

public class PythonConnector : MonoBehaviour
{
    public static PythonConnector instance;
    private Vector2 RecentPos;
    public Vector2[] EyePoses = new Vector2[4];
    private SocketConnection socketConnection;
    private bool initialized;

    private Vector2 pointX;
    private Vector2 pointY;

    public PythonConnector()
    {
        Debug.WriteLine("INSTANCE");
        socketConnection = new SocketConnection("127.0.0.1", 7469);
        RegisterFuncOnCallback(InitializeRecentGazePos);
    }
    
    public void RegisterFuncOnCallback(SocketConnection.PosChanged func)
    {
        socketConnection.posChangedCallBack += func;
    }

    public void DeregisterFuncOnCallback(SocketConnection.PosChanged func)
    {
        socketConnection.posChangedCallBack -= func;
    }

    public void InitializeRecentGazePos(string v)
    {
        UnityEngine.Debug.Log(v);
    }
}