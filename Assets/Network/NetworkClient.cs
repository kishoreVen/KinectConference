using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System;

public class NetworkClient : MonoBehaviour 
{
	#region Private Variables
    Vector3[] _Vertices; // Replace with appropriate vertices used with kinect;

	private bool _meshRequiresUpdate; // If true perform mesh update calculations

	private DepthSourceView depthSource;
	#endregion

    #region Network Variables
    private IPEndPoint serveripep;

    private Socket server;

    private NetworkStream nsSend;
    private NetworkStream nsReceive;

    private bool isServerConnected;

	private int bytesReceived;

    byte[] dataToReceive;

	bool isReading;
	bool isWriting;
    #endregion

    #region Public Variables
    public string serverIP;

	public Transform serverCube;

	public Vector3 transformVec = new Vector3(0f, -10f, 45f);
	#endregion

	#region Constructor and Destructor
	void Start () 
	{
		isReading = false;
		isWriting = false;
		serverCube = GameObject.Find("ServerCube").transform;

		bytesReceived = 0;

        DataManager.Initialize();

        dataToReceive = new byte[DataManager.NetByteCount];

		isServerConnected = false;

		ConnectToServer ();

		_meshRequiresUpdate = false;

		_Vertices = depthSource.GetVertices ();
	}

	void Destroy()
	{
		nsSend.Close ();
		nsReceive.Close ();
		server.Disconnect (false);
	}
	#endregion
	
	#region Loop
	void Update () 
	{
		if(isServerConnected)
		{
            try
            {
				// -------------- Positional Information Block --------------------------------------//
				SendPositionInformation();
				ReceivePositionInformation();

				// -------------- Positional Information Block --------------------------------------//
				
				// -------------- Depth Cloud Block --------------------------------------//
				//PerformDepthCloudNetworkOps();

				if(_meshRequiresUpdate)
				{
					_meshRequiresUpdate = false;

					// Perform Mesh Computation Here
				}
            }
            catch (System.Exception ex)
            {
                isServerConnected = false;

                Debug.Log("Server Disconnected. Please restart server and client in the exact order. Error: " + ex.Message);
            }
		}
	}
	#endregion

	#region Methods
	private void ConnectToServer()
	{
		server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		
		serveripep = new IPEndPoint(IPAddress.Parse(serverIP), 9050);
		
		try
		{
			server.Connect(serveripep);
				
			nsSend = new NetworkStream(server);
			nsReceive = new NetworkStream(server);

			isServerConnected = true;
		}
		catch(System.Exception ex)
		{
			Debug.Log(ex.Message);
		}
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

        serverCube.position = positionToSet + transformVec;
    }
	#endregion

	#region Callbacks
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
