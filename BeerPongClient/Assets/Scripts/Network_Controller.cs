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
    private bool isStarting = true;
    private string userName;
    private string opforName;
    private byte[] recvBuffer;

    [SerializeField]
    public GameObject UserTextBox;

    [SerializeField]
    public GameObject OpforTextBox;

    [SerializeField]
    public GameObject ServerTextBox;


    public void Update()
    {
        if(isStarting)
        {
            //GetServerName();
            Debug.Log("Is Starting");
            SetupSocket();
        }

        if(socket.Available > 0)
        {
            readMessage();
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
                isStarting = false;
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
        Debug.Log("\'" + userName + "\'" + "has been set to" + ADDR);
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
        Debug.Log("\'" + opforName + "\'" + "has been set to" + ADDR);
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
        recvBuffer = new byte[bytesToRead];
        netStream.Read(recvBuffer, 0, bytesToRead);
        string msg = System.Text.Encoding.Default.GetString(recvBuffer);
        Debug.Log(msg);
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

}


