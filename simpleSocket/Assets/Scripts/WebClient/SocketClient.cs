using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;

public class SocketClient : MonoBehaviour
{

    private const int   MESSAGE_LEN_PACKAGE_SIZE    = 2;
    private const int   MESSAGE_TYPE_PACKAGE_SIZE   = 1;
    private const int   MESSAGE_MAX_LENGTH          = 1024;
    private const bool  LITTLE_BYTE_ORDER           = false;

    //public static SocketClient ActiveSocket { get; private set; }   // Do we really need this

	private ASCIIEncoding encoder = new ASCIIEncoding();

    private Socket socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );

    private readonly string hostIp = "127.0.0.1";
    private readonly int port = 8222;

    private bool connecting = false;
    private bool connected = false;

    /// <summary>
    /// Thread safe method to see if we are attempting to connect
    /// </summary>
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

    /// <summary>
    /// Thread safe method to check if we are connected
    /// </summary>
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
    /// Do we have any inbound messages waiting to be processed
    /// </summary>
    public bool HasMessages => inboundQueue.Count > 0;

    /// <summary>
    /// Do we have any outbound messages pendding to be sent
    /// </summary>
    public bool HasPenndingMessage => outboundQueue.Count > 0;

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
        //ActiveSocket = this;
    }
    // Start is called before the first frame update
    void Start()
    {

        // start the synchronized queues
        inboundQueue = Queue.Synchronized( new Queue() );
        outboundQueue = Queue.Synchronized( new Queue() );

    }

    // Update is called once per frame
    void Update()
    {
        // check that the required threads are running
        if (!Connecting && !Connected)  // connect
        {
            Connecting = true;
            connectThread = new Thread( Connect );
            connectThread.Start();
        }
        
        if ( Connected && !ReciveThread_isRunning )
        {
            ReciveThread_isRunning = true;
            receiveThread = new Thread( ReciveMessage );
            receiveThread.Start(); 
        }

        if ( Connected && outboundQueue.Count > 0 && !SendThread_isRunning )
        {
            SendThread_isRunning = true;
            sendThread = new Thread( SendMessage );
            sendThread.Start();
        }

        // process the inbound queue
        while (inboundQueue.Count > 0)
        {
            BaseProtocol protocol = inboundQueue.Dequeue() as BaseProtocol;
            HandleProtocol.Inst.InvokeProtocol( protocol );
        }

    }

    public string GetMessage ()
    {
        if ( HasMessages )
            return inboundQueue.Dequeue() as string;
        else
            return "";
    }

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

    public void QueueMessage( string message )
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

    private void ReciveMessage()
    {

        byte[] mesLenBuffer = new byte[ MESSAGE_LEN_PACKAGE_SIZE ];
        byte[] mesTypeBuffer = new byte[ MESSAGE_TYPE_PACKAGE_SIZE ];
        byte[] mesBuffer = new byte[ MESSAGE_MAX_LENGTH ];              // Define the message buffer out of the while loop so we dont have to realocate :)

        while ( ReciveThread_isRunning && Connected)
        {
            // recive first bytes to see how long the message is
            socket.Receive( mesLenBuffer, 0, MESSAGE_LEN_PACKAGE_SIZE, SocketFlags.None );
            // Get the next byte to see what data the message contatines
            socket.Receive( mesTypeBuffer, 0, MESSAGE_TYPE_PACKAGE_SIZE, SocketFlags.None );

            // make sure the received value is in the correct endian
            ConvertBytes( ref mesLenBuffer );
            ConvertBytes( ref mesTypeBuffer );

            int messageLen = System.BitConverter.ToInt32(mesLenBuffer, 0);
            char messageIdenity = System.BitConverter.ToChar( mesTypeBuffer, 0 );
            
            if ( messageLen > MESSAGE_MAX_LENGTH )
            {
                // TODO: send the message back to server so it can be loged as a fatal error
                // Or should i just realocate?
                Debug.LogErrorFormat("FATAL ERROR: Message has exceded the max message size. The message has been loged, and discarded as a result! (Max message size: {0} Received message size: {1})", 
                                      MESSAGE_MAX_LENGTH, messageLen);
                continue;

            }

            Debug.LogWarningFormat("Recived message Len {0}; Message Type {1}; ", messageLen, messageIdenity);

            // receive the message
            int result = socket.Receive( mesBuffer, 0, messageLen, SocketFlags.None );
            string message = encoder.GetString( mesBuffer, 0, result );

            BaseProtocol protocol = HandleProtocol.ConvertJson( messageIdenity, message );

            inboundQueue.Enqueue( (object)protocol );

        }

        ReciveThread_isRunning = false;

    }

    private void SendMessage ()
    {

        Debug.LogFormat( "Started Sending {0} messages ", outboundQueue.Count );

        while ( outboundQueue.Count > 0 )
        {
            BaseProtocol protocol = outboundQueue.Dequeue() as BaseProtocol;
            string data = protocol.GetJson( out int messageLength );

            byte[] dataLenBytes_ = System.BitConverter.GetBytes( messageLength );
            byte[] dataIdenityBytes_ = System.BitConverter.GetBytes( protocol.Idenity );

            byte[] dataLenBytes = new byte[ MESSAGE_LEN_PACKAGE_SIZE ];
            byte[] dataIdenityBytes = new byte[ MESSAGE_TYPE_PACKAGE_SIZE ];

            // TODO: Make this work for different packet sizes
            // Get the bytes that we need
            // We are working with Big endian on the server :)
            if ( System.BitConverter.IsLittleEndian )   
            {   // use first two bytes reversed for little endian
                dataLenBytes[ 0 ] = dataLenBytes_[ 1 ];
                dataLenBytes[ 1 ] = dataLenBytes_[ 0 ];
                dataIdenityBytes[ 0 ] = dataIdenityBytes_[ 0 ];

            }
            else
            {   // use last two bytes for big endian
                dataLenBytes[ 0 ] = dataLenBytes_[ 3 ];
                dataLenBytes[ 1 ] = dataLenBytes_[ 4 ];
                dataIdenityBytes[ 0 ] = dataIdenityBytes_[ 4 ];
            }

            Debug.LogWarningFormat("Sending mesage Length: {0}; Idenity: {1}", messageLength, protocol.Idenity);

            socket.Send( dataLenBytes );                                    // send the length of the message
            socket.Send( dataIdenityBytes );                                // send the idenity of the message
            socket.Send( encoder.GetBytes( data ) );                        // send the message

        }
      
        SendThread_isRunning = false;
        Debug.Log( "Send Message thread finished!" );

    }

    /// <summary>
    /// converts bytes to the correct endian is necessary
    /// </summary>
    /// <param name="bytes"></param>
    void ConvertBytes( ref byte[] bytes )
    {

        if ( System.BitConverter.IsLittleEndian && !LITTLE_BYTE_ORDER || !System.BitConverter.IsLittleEndian && LITTLE_BYTE_ORDER )
            System.Array.Reverse( bytes );

    }

    private void Disconnect ()
    {

        //socket.Disconnect(false);

        try
        {
            socket.Shutdown( SocketShutdown.Both );
        }
        finally
        {
            socket.Close();
        }

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
