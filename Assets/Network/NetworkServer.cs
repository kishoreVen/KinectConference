using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System;

public class NetworkServer : MonoBehaviour 
{
	#region Private Variables
    Vector3[] _Vertices; // Replace with appropriate vertices used with kinect;

	private bool _meshRequiresUpdate; // If true perform mesh update calculations
	#endregion

    #region Network Related Variables
    private Socket newsock;
    private Socket client;

    private IPEndPoint serveripep;
    private IPEndPoint newclientipep;

    private NetworkStream nsSend;
    private NetworkStream nsReceive;

    private bool isClientConnected;

	private int bytesReceived;

    byte[] dataToReceive;

	bool isReading;
	bool isWriting;
    #endregion

    #region Public Variables
    public Transform clientCube;
	#endregion

	#region Constructor
	void Start () 
	{
		isReading = false;
		isWriting = false;
		bytesReceived = 0;

        DataManager.Initialize();

        dataToReceive = new byte[DataManager.NetByteCount];

		isClientConnected = false;

		StartServer ();

		_meshRequiresUpdate = false;
	}
	#endregion
	
	#region Loop
	void Update () 
	{
		if(isClientConnected)
		{
            try
            {
                // -------------- Positional Information Block --------------------------------------//
//                SendPositionInformation(); // Use this function if you want to send the position of an object to Client
//
//				ReceivePositionInformation(); // Use this function if you want to receive the position of the client - Use with SendPositionInformation()
				// -------------- Positional Information Block --------------------------------------//

				// -------------- Depth Cloud Block --------------------------------------//
				PerformDepthCloudNetworkOps();

				if(_meshRequiresUpdate)
				{
					_meshRequiresUpdate = false;

					// Perform Mesh Computation Here
				}
            }
            catch (System.Exception ex)
            {
                isClientConnected = false;

                Debug.Log("Client Disconnected. Please restart server and client in the exact order. Error: " + ex.Message);
            }
		}
	}
	#endregion

	#region Methods
	private void StartServer()
	{
		serveripep = new IPEndPoint(IPAddress.Any, 9050);
		
		newsock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		
		newsock.Bind(serveripep);
		newsock.Listen(10);
		
		Debug.Log("Waiting for a client...");
		
		newsock.BeginAccept(AcceptConnection, null);
	}

	private void PerformDepthCloudNetworkOps()
	{
//		if(!isWriting && !isReading)
//		{
//			SendDepthCloud();
//		}

		if(nsSend.CanWrite && !isWriting)
		{
			SendDepthCloud();
		}

		if(nsReceive.CanRead && !isReading)
		{
			ReceiveDepthCloud();
		}
	}

    private void SendDepthCloud()
    {
		isWriting = true;

		_Vertices = new Vector3[13568]; // Remove Later

        DataManager.MakeVertexDepthBytes(ref _Vertices);

        nsSend.BeginWrite(DataManager.dataToSend, 0, DataManager.dataToSend.Length, AcceptAsyncWriteEnd, nsSend);        
    }

    private void SendPositionInformation()
    {
		byte[] dataToSend = DataManager.MakeVector3Byte(transform.position);

        nsSend.Write(dataToSend, 0, dataToSend.Length);   

		nsSend.Flush();
    }

    private void ReceiveDepthCloud()
    {
        //nsReceive.Read(dataToReceive, 0, dataToReceive.Length);
		isReading = true;

		nsReceive.BeginRead (dataToReceive, bytesReceived, (dataToReceive.Length - bytesReceived), AcceptAsyncReadEnd, nsReceive);
    }

    private void ReceivePositionInformation()
    {
        byte[] dataToReceive = new byte[DataManager.Vector3ByteLength];

        nsReceive.Read(dataToReceive, 0, dataToReceive.Length);

        Vector3 positionToSet = DataManager.BreakVector3Byte(dataToReceive);

        clientCube.position = positionToSet;
    }
	#endregion

	#region Callbacks
	private void AcceptConnection(IAsyncResult ar)
	{
		client = newsock.EndAccept(ar);
		newclientipep = (IPEndPoint)client.RemoteEndPoint;
		
		nsSend = new NetworkStream(client);
		nsReceive = new NetworkStream (client);
		
		Debug.Log("Message received from " + newclientipep.Address + ":" + newclientipep.Port);
		
		isClientConnected = true;
	}

	private void AcceptAsyncReadEnd(IAsyncResult ar)
	{
		NetworkStream net = (NetworkStream) ar.AsyncState;
		
		//get number of bytes read
		int nBytesRead = net.EndRead(ar);
		
		isReading = false;
		
		bytesReceived += nBytesRead;
		
		if(bytesReceived == DataManager.NetByteCount)
		{
			DataManager.BreakVertexDepthBytes(dataToReceive);

			_meshRequiresUpdate = true;
			
			bytesReceived = 0;
		}
	}

	private void AcceptAsyncWriteEnd(IAsyncResult ar)
	{
		NetworkStream net = (NetworkStream) ar.AsyncState;

		net.EndWrite(ar);

		net.Flush ();

		isWriting = false;
	}
	#endregion
}
