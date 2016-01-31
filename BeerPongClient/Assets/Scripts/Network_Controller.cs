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
    private int notificationTimer;
    private const string GAMEOVER = "GAMEOVER";

    [SerializeField] public GameObject UserTextBox;
    [SerializeField] public GameObject OpforTextBox;
    [SerializeField] public GameObject UIButton;
    [SerializeField] public GameObject NotificationTextbox;
    [SerializeField] public PlayerController PC;

    public void Start()
    {
        SetupSocket();
        stage = Stage.GETTING_USERNAME;
        //stage = Stage.WAITING_CLIENT;
        socket.NoDelay = true;
        notificationTimer = -1;

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
			
            if (stage == Stage.WAITING_OPPONENT)
            {
                string s = recvMessage().Trim();
                if (s.Length > 0)
                {
                    if (s.Equals("GAMEOVER") || s.Equals("XDISCONNECT"))
                        Application.Quit();
                    Debug.Log("STATE: " + s);
                    Vector3 pos = new Vector3(0,0,0), vel = new Vector3(0,0,0);
                    /*string floater = "";
                    while (floater.Length <= 0)
                        floater = recvMessage();
                    floater = floater.Trim();
                    pos.x = float.Parse(floater);
                    floater = input.ReadLine();
                    pos.y = float.Parse(floater);
                    floater = input.ReadLine();
                    pos.z = float.Parse(floater);
                    floater = input.ReadLine();
                    vel.x = float.Parse(floater);
                    floater = input.ReadLine();
                    vel.y = float.Parse(floater);
                    floater = input.ReadLine();
                    vel.z = float.Parse(floater);*/
                    PC.setState(s, pos, vel);
                    stage = Stage.WAITING_CLIENT;
                }
            }
        }

        if (notificationTimer > -1)
        {
            notificationTimer -= (int)(Time.fixedDeltaTime * 1000.0f);
            if (notificationTimer <= 0)
                NotificationTextbox.SetActive(false);
        }

        if (stage == Stage.WAITING_CLIENT)
            Notify("Your turn!");
        else if (stage == Stage.WAITING_OPPONENT)
            Notify("Waiting on opponent...");
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
            bool first = response.Equals("FIRST");
			myPlayer.SetPosition (first);
            if (first)
                stage = Stage.WAITING_CLIENT;
            else
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
        UIButton.SetActive(true);
        NotificationTextbox.SetActive(false);
    }

    public void DisableGUI()
    {
       UserTextBox.SetActive(false);
       OpforTextBox.SetActive(false);
       UIButton.SetActive(false);
       NotificationTextbox.SetActive(true);
        Camera.main.GetComponent<PlayerController>().RayOff();
    }

    public void sendClientGameState(string gameState, Vector3 pos, Vector3 vel)
    {
        if (stage == Stage.WAITING_CLIENT)
        {
            string newGameState = gameState.Substring(6, 6);
            newGameState += gameState.Substring(0, 6);

            sendMessage(newGameState + "\n");
            sendMessage(pos.x.ToString() + "\n");
            sendMessage(pos.y.ToString() + "\n");
            sendMessage(pos.z.ToString() + "\n");
            sendMessage(vel.x.ToString() + "\n");
            sendMessage(vel.y.ToString() + "\n");
            sendMessage(vel.z.ToString() + "\n");
            stage = Stage.WAITING_OPPONENT;
        }
    }

    public void ClearNotification()
    {
        Text input = NotificationTextbox.GetComponent<Text>();
        input.text = "";
        NotificationTextbox.SetActive(false);
    }

    public void Notify(string notification)
    {
        NotifyWithTimer(notification, -1);
    }

    public void NotifyWithTimer(string notification, int time)
    {
        ClearNotification();
        NotificationTextbox.SetActive(true);
        Text input = NotificationTextbox.GetComponent<Text>();
        input.text = notification;
        if (time > -1)
            notificationTimer = time;
        else
            time = -1;
    }

    public bool RespectMyAutoritah()
    {
        return (stage == Stage.WAITING_CLIENT);
    }

    public void WinGame()
    {
        if (RespectMyAutoritah())
        {
            sendMessage("GAMEOVER\n");
            Application.Quit();
        }
    }
}


