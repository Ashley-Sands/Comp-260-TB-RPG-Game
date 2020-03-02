using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketClient : MonoBehaviour
{
	public static SocketClient ActiveSocket { get; private set; }

	private ASCIIEncoding encoder = new ASCIIEncoding();

    private Socket socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );

    private readonly string hostIp = "127.0.0.1";
    private readonly int port = 8222;

    private bool connecting = false;
    private bool connected = false;

    private bool Connecting {
        get {
            lock ( this )
            {
                return connecting;
            }
        }

        set {
            lock ( this )
            {
                connecting = value;
            }
        }
    }

    private bool Connected {
        get {
            lock ( this )
            {
                return connected;
            }
        }

        set {
            lock ( this )
            {
                connected = value;
            }
        }
    }

    private Thread connectThread;
    private Thread receiveThread;
    private Thread sendThread;

    private bool _sendThread_isRunning = false;
    private bool _reciveThread_isRunning = false;

    Queue inboundQueue;
    Queue outboundQueue;

    /// <summary>
    /// Thread safe version of _reciveThread_isRunnging :)
    /// </summary>
    private bool ReciveThread_isRunning {
        get {
            lock ( this )
            {
                return _reciveThread_isRunning;
            }
        }

        set {
            lock ( this )
            {
                _reciveThread_isRunning = value;
            }
        }

    }

    /// <summary>
    /// Thread safe version of _sendThread_isRunnging :)
    /// </summary>
    private bool SendThread_isRunning {
        get {
            lock ( this )
            {
                return _sendThread_isRunning;
            }
        }

        set {
            lock ( this )
            {
                _sendThread_isRunning = value;
            }
        }

    }

    private void Awake ()
    {
        ActiveSocket = this;
    }
    // Start is called before the first frame update
    void Start()
    {

        //socket.Connect( new IPEndPoint( IPAddress.Parse(hostIp), port ) );

        inboundQueue = Queue.Synchronized( new Queue() );
        outboundQueue = Queue.Synchronized( new Queue() );

    }

    // Update is called once per frame
    void Update()
    {

        if (!Connecting && !Connected)  // connect
        {
            Connecting = true;
            connectThread = new Thread(Connect);
            connectThread.Start();
        }
        
        if ( Connected && !ReciveThread_isRunning )
        {
            ReciveThread_isRunning = true;
            receiveThread = new Thread( ReciveMessage );
            receiveThread.Start(); 
        }

        if ( Connected && !SendThread_isRunning )
        {
            SendThread_isRunning = true;
            sendThread = new Thread( SendMessage );
            sendThread.Start();
        }

        //GetMessages();
        
    }

    public bool HasMessages => inboundQueue.Count > 0;

    public string[] GetMessages ()
    {
        List<string> messages = new List<string>();

        while ( HasMessages )
        {
            messages.Add( inboundQueue.Dequeue() as string );
            Debug.LogWarning(messages[messages.Count-1]);
        }

        return messages.ToArray();

    }

    public string GetMessage()
    {
        if ( HasMessages )
            return inboundQueue.Dequeue() as string;
        else
            return "";
    }

    public void SetMessage( string message )
    {
        outboundQueue.Enqueue( message as object );
    }

    private void Connect()
    {

        // connect to host
        while ( Connecting )
        {
            try
            {
                socket.Connect( new IPEndPoint( IPAddress.Parse( hostIp ), port ) );
                Connected = socket.Connected;
                Connecting = !Connected;
            }
            catch (System.Exception e)
            {
                Debug.LogError( e );
            }
        }

    }

    private void Disconnect ()
    {

        //socket.Disconnect(false);

        try { 
            socket.Shutdown(SocketShutdown.Both);
        }
        finally{
            socket.Close();
        }

    }

    private void ReciveMessage()
    {

        byte[] mesLenBuffer = new byte[ 2 ];
        byte[] buffer = new byte[ 1024 ];

        while (ReciveThread_isRunning && Connected)
        {
            // recive irst to bytes to see how long the message is
            socket.Receive( mesLenBuffer, 0, 2, SocketFlags.None );

            if ( System.BitConverter.IsLittleEndian )
                System.Array.Reverse( mesLenBuffer );

            //int messageLen = System.BitConverter.ToInt32(mesLenBuffer, 0);

            
            int messageLen = 0;// encoder.GetString( mesLenBuffer );

            foreach ( byte b in mesLenBuffer )
                messageLen += (int)b;
                

            Debug.LogWarning(messageLen);
            // receive the message
            int result = socket.Receive( buffer, 0, messageLen, SocketFlags.None );
            string data = encoder.GetString( buffer, 0, messageLen );

            inboundQueue.Enqueue( (object)data );
        }

        ReciveThread_isRunning = false;

    }

    private void SendMessage ()
    {

        Debug.Log( "Sending "+ outboundQueue.Count );

        while ( SendThread_isRunning && Connected )
        {
            while ( outboundQueue.Count > 0 )
            {
                string data = outboundQueue.Dequeue() as string;
                byte[] intBytes_ = System.BitConverter.GetBytes( data.Length );
                byte[] intBytes = new byte[ 2 ];


                if ( System.BitConverter.IsLittleEndian )   // We are working with Big endian
                    System.Array.Reverse(intBytes);

                intBytes[ 0 ] = intBytes_[ 1 ];
                intBytes[ 1 ] = intBytes_[ 0 ];


                Debug.LogWarning("sending message of len"+ data.Length + " int bytes size " + intBytes.Length);

                socket.Send( intBytes ); // send the length of the message
                socket.Send( encoder.GetBytes( data ) );                    // send the message
                Debug.Log( "Sent "+data );
            }
        }

        SendThread_isRunning = false;

    }

    private void OnDestroy ()
    {
        Debug.LogFormat( " preDisconnect rt: {0} st: {1} c: {2} con: {3}", receiveThread?.IsAlive, sendThread?.IsAlive, connectThread?.IsAlive, socket?.Connected );

        if ( Connecting )
        {
            Connecting = false;
            // wait for the thread to exit.
            connectThread.Join();

        }

        Disconnect();

        if ( ReciveThread_isRunning )
        {
            ReciveThread_isRunning = false;

            // wait for the thread to exit.
            receiveThread.Join();

        }

        if ( SendThread_isRunning )
        {
            SendThread_isRunning = false;

            // wait for the thread to exit.
            sendThread.Join();

        }

        Debug.LogFormat( " rt: {0} st: {1} c: {2} con: {3}", receiveThread?.IsAlive, sendThread?.IsAlive, connectThread?.IsAlive, socket?.Connected );


    }

} 
