using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Net.Sockets;
using System.IO;

public class Network_Controller : MonoBehaviour {
    private string ADDR = "130.39.94.243";
    private int PORT = 8904;
    private TcpClient socket;
    private bool isStarting = true;
    private string userName;
    private string opforName;
    StreamWriter output;
    NetworkStream netStream;

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
            Debug.Log("Is Starting");
            SetupSocket();
        }

    }

    public void SetupSocket()
    {
        try
        {
            socket = new TcpClient();
            socket.Connect(ADDR, PORT);
            netStream = socket.GetStream();
            output = new StreamWriter(netStream);
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

    public void GetServerName()
    {
        InputField input = ServerTextBox.GetComponent<InputField>();
        ADDR = input.text;
    }

    public void sendUserName()
    {
        GetUserName();
        Debug.Log(userName);
        userName += "\n";
        byte[] toSend = System.Text.Encoding.UTF8.GetBytes(userName);
        output.Write(userName);
        output.Flush();
    }

    public void sendOpforName()
    {
        GetOpforName();
        Debug.Log(opforName);
        opforName += "\n";
        byte[] toSend = System.Text.Encoding.UTF8.GetBytes(opforName);
        output.Write(opforName);
        output.Flush();
    }

    void sendMessage(string msg)
    {
        byte[] toSend = System.Text.Encoding.UTF8.GetBytes(msg);
        output.Write(msg);
        output.Flush();
    }


}


