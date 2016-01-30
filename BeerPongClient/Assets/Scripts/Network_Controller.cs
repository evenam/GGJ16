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
    public GameObject TestBox1;

    [SerializeField]
    public GameObject TestBox2;


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

    public void GetUserName()
    {
        InputField input = TestBox1.GetComponent<InputField>();
        userName = input.text;
    }

    public void GetOpforName()
    {
        InputField input = TestBox2.GetComponent<InputField>();
        opforName = input.text;
    }

    void sendMessage(string msg)
    {
        byte[] toSend = System.Text.Encoding.UTF8.GetBytes(msg);
        output.Write(msg);
        output.Flush();
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


}


