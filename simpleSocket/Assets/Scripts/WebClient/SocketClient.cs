﻿using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;

public enum ConnectionStatus { None, Connecting, Connected, Error }

public class SocketClient : MonoBehaviour
{

    private const int   MESSAGE_LEN_PACKAGE_SIZE    = 2;
    private const int   MESSAGE_TYPE_PACKAGE_SIZE   = 1;
    private const int   MESSAGE_MAX_LENGTH          = 1024;
    private const bool  LITTLE_BYTE_ORDER           = true;

    [SerializeField] private GameData gameData;

    public static SocketClient ActiveSocket { get; private set; }
    public static GameData ActiveGameData { get; private set; }


    private ASCIIEncoding encoder = new ASCIIEncoding();

    private Socket socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );

    private readonly string hostIp = "127.0.0.1";
    private readonly int port = 8222;

    private bool _reset = false;
    private bool _running = false;    // need to add this to the threads.
    private bool _connecting = false;
    private bool _connected = false;

    public float connectingCooldown;
    private float _reconnectCooldown = 0;

    public bool Reset{
        get {
            lock ( this )
            {
                return _reset;
            }
        }
        set {
            lock ( this )
            {
                _reset = value;
            }
        }
    }

    public bool Running {
        get{
            lock (this)
            {
                return _running;
            }
        }
        set {
            lock (this)
            {
                _running = value;
            }
        }
    }
    /// <summary>
    /// Thread safe method to see if we are attempting to connect
    /// </summary>
    private bool Connecting {
        get {
            lock ( this )
            {
                return _connecting;
            }
        }

        set {
            lock ( this )
            {
                _connecting = value;
            }
        }
    }

    /// <summary>
    /// Thread safe method to check if we are connected
    /// </summary>
    public bool Connected {
        get {
            lock ( this )
            {
                return _connected;
            }
        }

        set {
            lock ( this )
            {
                _connected = value;
            }
        }
    }

    /// <summary>
    /// Thread safe method to set and check reconnect cool down
    /// </summary>
    public float ReconnectCooldown{

        get{
            lock (this)
            {
                return _reconnectCooldown;
            }
        }
        set{
            lock (this)
            {
                _reconnectCooldown = value;
            }
        }

    }

    private Thread connectThread;
    private Thread receiveThread;
    private Thread sendThread;

    private bool _sendThread_isRunning = false;
    private bool _reciveThread_isRunning = false;

    private Queue inboundQueue;
    private Queue outboundQueue;

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
        ActiveSocket = this;
        ActiveGameData = Instantiate<GameData>( gameData );
        ActiveGameData.Init();

        DontDestroyOnLoad( this );
    }
    private void Start()
    {

        // start the synchronized queues
        inboundQueue = Queue.Synchronized( new Queue() );
        outboundQueue = Queue.Synchronized( new Queue() );

    }

    // Update is called once per frame
    void Update()
    {

        if ( Reset )
        {
            ResetSocket();
            Reset = false;
        }
        // always process the inbound queue, as the game can inject protocols,
        // as a means of notifcation, ie when the connection is lost.
        while ( inboundQueue.Count > 0 )
        {
            BaseProtocol protocol = inboundQueue.Dequeue() as BaseProtocol;
            HandleProtocol.Inst.InvokeProtocol( protocol );
        }

        if ( !Running ) return;

        if (ReconnectCooldown > 0)
            ReconnectCooldown -= Time.deltaTime;

        // check that the required threads are running
        if ( ReconnectCooldown <= 0 && !Connecting && !Connected )  // connect
        {
            ActiveGameData.SetConnectionStatus( ConnectionStatus.Connecting );

            Connecting = true;
            connectThread = new Thread( Connect );
            connectThread.Start();
        }
        else if ( Connected )
        {
            ActiveGameData.SetConnectionStatus( ConnectionStatus.Connected );

            if ( !ReciveThread_isRunning )
            {
                ReciveThread_isRunning = true;
                receiveThread = new Thread( ReciveMessage );
                receiveThread.Start();
            }

            if ( outboundQueue.Count > 0 && !SendThread_isRunning )
            {
                SendThread_isRunning = true;
                sendThread = new Thread( SendMessage );
                sendThread.Start();
            }

        }
    }

    /// <summary>
    /// sends a message to server
    /// </summary>
    /// <param name="message"></param>
    public void QueueMessage( BaseProtocol message )
    {
        if (Connected)
            outboundQueue.Enqueue( message );
    }

    /// <summary>
    /// Sends a message to self
    /// </summary>
    /// <param name="message"></param>
    public void QueueLocalMessage( BaseProtocol message )
    {
        inboundQueue.Enqueue( message );
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
                ReconnectCooldown = connectingCooldown;
                Connecting = false;
                break;
            }
        }

    }

    private void ReciveMessage()
    {

        byte[] mesLenBuffer = new byte[ 4 ]; // MESSAGE_LEN_PACKAGE_SIZE ];
        byte[] mesTypeBuffer = new byte[ 4 ]; // MESSAGE_TYPE_PACKAGE_SIZE ];
        byte[] mesBuffer = new byte[ MESSAGE_MAX_LENGTH ];              // Define the message buffer out of the while loop so we dont have to realocate :)

        while ( ReciveThread_isRunning && Connected)
        {
            // recive first bytes to see how long the message is
            try
            {
                socket.Receive( mesLenBuffer, 0, MESSAGE_LEN_PACKAGE_SIZE, SocketFlags.None );
                // Get the next byte to see what data the message contatines
                socket.Receive( mesTypeBuffer, 0, MESSAGE_TYPE_PACKAGE_SIZE, SocketFlags.None );
            }
            catch( System.Exception e )
            {
                Debug.LogError( e );
                ErrorDisconnect();
                break;
            }

            // make sure the received value is in the correct endian
            ConvertBytes( ref mesLenBuffer );
            //ConvertBytes( ref mesTypeBuffer );

            // if would apear that if we have an invaild connection bitconverter fails.
            // TODO: fix this. its the whole reciveing 0 bytes thing. 
            // I would love to do it now but its 1am and iv got to get up erly :`(

            int messageLen = System.BitConverter.ToInt32(mesLenBuffer, 0);
            char messageIdenity = System.BitConverter.ToChar( mesTypeBuffer, 0 );

            if ( messageLen > MESSAGE_MAX_LENGTH )
            {
                // TODO: send the message back to server so it can be loged as a fatal error
                // Or should i just realocate?
                Debug.LogErrorFormat("FATAL ERROR: Message has exceded the max message size. The message has been loged, and discarded as a result! (Max message size: {0} Received message size: {1})", 
                                      MESSAGE_MAX_LENGTH, messageLen);
                //continue;

            }

            Debug.LogWarningFormat("Recived message Len {0}; Identity {1}; ", messageLen, messageIdenity);

            int result = 0;

            // receive the message
            try
            {
                result = socket.Receive( mesBuffer, 0, messageLen, SocketFlags.None );
            }
            catch( System.Exception e )
            {
                Debug.LogError( e );
                ErrorDisconnect();
                break;
            }

            string message = encoder.GetString( mesBuffer, 0, result );

            BaseProtocol protocol = HandleProtocol.ConvertJson( messageIdenity, message );

            inboundQueue.Enqueue( protocol );
            Debug.Log("Inbound Message: "+ message );
        }

        ReciveThread_isRunning = false;

    }

    private void SendMessage ()
    {

        Debug.LogFormat( "Started Sending {0} messages ", outboundQueue.Count );

        while ( outboundQueue.Count > 0 )
        {
            BaseProtocol protocol = (BaseProtocol)outboundQueue.Dequeue();
            string data = protocol.GetJson( out int messageLength );

            byte[] dataLenBytes_ = System.BitConverter.GetBytes( messageLength );
            byte[] dataIdenityBytes_ = System.BitConverter.GetBytes( protocol.Identity );

            byte[] dataLenBytes = new byte[ MESSAGE_LEN_PACKAGE_SIZE ];
            byte[] dataIdenityBytes = new byte[ MESSAGE_TYPE_PACKAGE_SIZE ];
            
            // TODO: Make this work for different packet sizes
            // Get the bytes that we need
            // We are working with Big endian on the server :)
            if ( System.BitConverter.IsLittleEndian )   
            {   // use first two bytes reversed for little endian
                byte temp = dataLenBytes_[ 0 ];
                dataLenBytes[ 0 ] = dataLenBytes_[ 1 ];
                dataLenBytes[ 1 ] = temp;
                dataIdenityBytes[ 0 ] = dataIdenityBytes_[ 0 ];

            }
            else
            {   // use last two bytes for big endian
                dataLenBytes[ 0 ] = dataLenBytes_[ 3 ];
                dataLenBytes[ 1 ] = dataLenBytes_[ 4 ];
                dataIdenityBytes[ 0 ] = dataIdenityBytes_[ 4 ];
            }
            
            Debug.LogWarningFormat("Sending mesage Length: {0}; Idenity: {1}", messageLength, protocol.Identity);
            Debug.Log( "Outbound Message: " + data );

            try
            {
                socket.Send( dataLenBytes );                                    // send the length of the message
                socket.Send( dataIdenityBytes );                                // send the idenity of the message
                socket.Send( encoder.GetBytes( data ) );                        // send the message
            }
            catch(System.Exception e)
            {
                Debug.LogError( e );
                ErrorDisconnect();
                break;
            }

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

        if ( System.BitConverter.IsLittleEndian )
        {   // use first two bytes reversed for little endian
            byte tempByte = bytes[ 0 ];
            bytes[ 0 ] = bytes[ 1 ];
            bytes[ 1 ] = tempByte;
        }

    }

    private void ErrorDisconnect()
    {
        
        // make sure the socket is dead.
        try
        {
            socket.Close();
        }
        catch{}

        Reset = true;
        
        print( "Helloo World");

        // Add a server error to the inbound cue.
        // to let the game know that an error has occored.
        StatusProtocol serverStatus = new StatusProtocol();
        serverStatus.ok = false;
        serverStatus.message = "Server disconnected!";
        serverStatus.SetMessageType( StatusProtocol.Type.Server );
        serverStatus.from_client = GameData.GAME_CLIENT_NAME;

        inboundQueue.Enqueue( serverStatus );



    }

    private void Disconnect ()
    {

        //socket.Disconnect(false);

        try
        {
            socket.Shutdown( SocketShutdown.Both );
        }
        catch( System.Exception e)
        {
            Debug.LogError( e );
        }
        finally
        {
            socket.Close();
        }

    }

    private void ResetSocket()
    {

        Running = false;
        Connected = false;

        CloseThreads(false);    // make sure all the threads have stoped

        socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
        print("Socketttttttttttttttttttttttttttttttttttttttt"); 

    }

    private void CloseThreads( bool disconnect )
    {
        if ( Connecting )
        {
            Connecting = false;
            // wait for the thread to exit.
            connectThread.Join();

        }

        if (disconnect)
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

        print( "ALL threads stoped!" );
        
    }

    private void OnDestroy ()
    {
        Debug.LogFormat( " preDisconnect rt: {0} st: {1} c: {2} con: {3}", receiveThread?.IsAlive, sendThread?.IsAlive, connectThread?.IsAlive, socket?.Connected );

        CloseThreads( true );

        Debug.LogFormat( " rt: {0} st: {1} c: {2} con: {3}", receiveThread?.IsAlive, sendThread?.IsAlive, connectThread?.IsAlive, socket?.Connected );


    }

} 
