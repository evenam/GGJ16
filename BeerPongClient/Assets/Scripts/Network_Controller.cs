using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Net.Sockets;
using System.IO;

public class Network_Controller : MonoBehaviour {
    StreamWriter output;
    StreamReader input;
    NetworkStream netStream;

    private string ADDR = "130.39.94.243";
    private int PORT = 8904;
    private TcpClient socket;
	private Camera playerObject;
	private PlayerController myPlayer;

    private enum Stage
    {
        GETTING_USERNAME,
        GETTING_OPPONENTNAME,
        WAITING_CLIENT,
        WAITING_OPPONENT
    }

    Stage stage;

    private string userName;
    private string opforName;
    private byte[] recvBuffer;

    private byte tag;
	private byte first;
    private short gamedata;
    private int xPos;
    private int yPos;
    private int zPos;
    private int xVel;
    private int yVel;
    private int zVel;
    private const string GAMEOVER = "GAMEOVER";

    [SerializeField] public GameObject UserTextBox;
    [SerializeField] public GameObject OpforTextBox;
    [SerializeField] public GameObject ServerTextBox;
    [SerializeField] public GameObject UIButton;

    public void Start()
    {
        InputField i = ServerTextBox.GetComponent<InputField>();
        i.text = ADDR;
        SetupSocket();
        stage = Stage.GETTING_USERNAME;
        socket.NoDelay = true;

		playerObject = Camera.main;
		myPlayer = playerObject.GetComponent<PlayerController>();
    }

    public void Update()
    {
        if (stage == Stage.GETTING_OPPONENTNAME || stage == Stage.GETTING_USERNAME)
            EnableGUI();
        else
        {

            DisableGUI();
			// stages
            sendMessage("YOLO\n");
            recvMessage();

        }
    }

    public void InitializeConnection()
    {
        // handshake
        string response = "USERNAME_REJECTED";

        if (stage == Stage.GETTING_USERNAME)
        {
            sendUserName();
            response = input.ReadLine().Trim();
            if (response.Equals("USERNAME_REJECTED"))
            {
                return;
            }
            stage = Stage.GETTING_OPPONENTNAME;
        }

        Debug.Log("Sent in username");


        if (stage == Stage.GETTING_OPPONENTNAME)
        {
            sendOpforName();
            response = input.ReadLine().Trim();
            if (response.Equals("OPPONENT_REJECTED"))
            {
                return;
            }
            stage = Stage.WAITING_CLIENT;
        }

        Debug.Log("Sent in opponent name");

		if (stage == Stage.WAITING_CLIENT) {
			response = input.ReadLine().Trim ();
			myPlayer.SetPosition (response.Equals ("FIRST"));
			if (response.Equals ("FIRST"))
				stage = Stage.WAITING_OPPONENT;
		}
    }

    /**
    *Socket init function
    */
    public void SetupSocket()
    {
        try
        {
            socket = new TcpClient();
            socket.Connect(ADDR, PORT);
            netStream = socket.GetStream();
            output = new StreamWriter(netStream);
            input = new StreamReader(netStream);
            if (socket.Connected)
            {
                Debug.Log("Connected");

            }
            
        } catch(Exception e) {
            Debug.Log("Connection Failed");
        }
    }


    /**
    *Gets user name fromm text inputfield
    */
    public void GetUserName()
    {
        InputField input = UserTextBox.GetComponent<InputField>();
        userName = input.text;
    }

    /**
    *Gets opfor name from text inputfield
    */
    public void GetOpforName()
    {
        InputField input = OpforTextBox.GetComponent<InputField>();
        opforName = input.text;
    }


    /**
    *Gets server name from text inputfield
    */
    public void GetServerName()
    {
        InputField input = ServerTextBox.GetComponent<InputField>();
        ADDR = input.text;
    }


    /**
    *Sends user name from text inputfield
    *to server.
    */
    public void sendUserName()
    {
        GetUserName();
        Debug.Log("\'" + userName + "\'" + "has been sent to " + ADDR);
        userName += "\n";
        byte[] toSend = System.Text.Encoding.UTF8.GetBytes(userName);
        output.Write(userName);
        output.Flush();
    }

    /**
    *Gets opfor name from text inputfield
    *to server.
    */
    public void sendOpforName()
    {
        GetOpforName();
        Debug.Log("\'" + opforName + "\'" + "has been sent to " + ADDR);
        opforName += "\n";
        byte[] toSend = System.Text.Encoding.UTF8.GetBytes(opforName);
        output.Write(opforName);
        output.Flush();
    }

    /**
    *Default message sender
    */
    public void sendMessage(string msg)
    {

        byte[] toSend = System.Text.Encoding.UTF8.GetBytes(msg);
        output.Write(msg);
        output.Flush();
    }

    /**
    *recives message from server
    */
    public string recvMessage()
    {
        int bytesToRead = socket.Available;
        if (bytesToRead <= 0) return "";
        recvBuffer = new byte[bytesToRead];
        netStream.Read(recvBuffer, 0, bytesToRead);
        string msg = System.Text.Encoding.Default.GetString(recvBuffer);
        Debug.Log("Received: " + msg);
        return msg;
    }

    /**
    *reads message from Server
    */
    public void readMessage()
    {
        string msg = recvMessage();
        string msgs = msg.Trim();
        if (!msgs.Equals(""))
        {
            Debug.Log(Time.time + " Got Message: " + msgs + " with length " + msgs.Length + " and index " );
        }
    }


    /**
    *Decodes data 
    */
    public void streamHandler(string msg)
    {
        byte[] data = System.Text.Encoding.Default.GetBytes(msg);

        tag = data[0];
        if(tag == 'G')
        {
            Debug.Log("GAMEOVER: Disconnecting " + userName);
            socket.Close();
            Debug.Log("Disconnected");
        }

        else if(tag == 'D')
        {
            gamedata = BitConverter.ToInt16(new byte[] { data[2], data[1] }, 0);
            xPos = BitConverter.ToInt32(new byte[] { data[6], data[5], data[4], data[3] }, 0);
            yPos = BitConverter.ToInt32(new byte[] { data[10], data[9], data[8], data[7] }, 0);
            zPos = BitConverter.ToInt32(new byte[] { data[14], data[13], data[12], data[11] }, 0);
            xVel = BitConverter.ToInt32(new byte[] { data[18], data[17], data[16], data[15] }, 0);
            yVel = BitConverter.ToInt32(new byte[] { data[22], data[21], data[20], data[19] }, 0);
            zVel = BitConverter.ToInt32(new byte[] { data[26], data[25], data[24], data[23] }, 0);

			GameStateUpdate ();
            Debug.Log("Data Received: " + gamedata + " " + xPos + " " + yPos + " " + zPos + " " + xVel + " " + yVel + " " + zVel);
        }   
        else if (tag == 'X')
        {
            stage = Stage.GETTING_USERNAME;
        }
           
    }

    public void OnApplicationQuit()
    {
        Debug.Log("Bye");
        sendMessage("DISCONNECT");
        socket.Close();
    }

    public void GameStateUpdate()
    {
		myPlayer.GameStateUpdate(gamedata, xPos, yPos, zPos, xVel, yVel, zVel);
    }

    public void EnableGUI()
    {
        UserTextBox.SetActive(true);
        OpforTextBox.SetActive(true);
        ServerTextBox.SetActive(true);
        UIButton.SetActive(true);
    }

    public void DisableGUI()
    {
       UserTextBox.SetActive(false);
       OpforTextBox.SetActive(false);
       ServerTextBox.SetActive(false);
       UIButton.SetActive(false);
       Camera.main.GetComponent<PlayerController>().RayOff();
    }
}


