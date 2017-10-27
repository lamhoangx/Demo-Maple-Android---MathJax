
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.IO;

// State object for reading client data asynchronously
public class StateObject
{
    const int PORT_NO = 8888;
    const string SERVER_IP = "127.0.0.1";
    private const int BUFFER_SIZE = 1024;
    static OpenMaple openMaple;
    static ASCIIEncoding encoding = new ASCIIEncoding();
    public static int Main(String[] args)
    {
        openMaple = new OpenMaple();
        openMaple.Open();

        IPEndPoint clientep;
        Socket client, socketReceive;
        TcpListener listener = new TcpListener(IPAddress.Parse(SERVER_IP), PORT_NO);

        IPEndPoint ip = new IPEndPoint(IPAddress.Any, PORT_NO); //Any IPAddress that connects to the server on any port
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //Initialize a new Socket

        socket.Bind(ip); //Bind to the client's IP
        socket.Listen(10); //Listen for maximum 10 connections

        Console.WriteLine("Server started on "+ listener.LocalEndpoint);
        Console.WriteLine("Waiting for a client connection ...");
        //listen
        //listener.Start();

        //client = listener.AcceptSocket();
        client = socket.Accept();
        //client = listener.AcceptTcpClient();
        clientep = (IPEndPoint)client.RemoteEndPoint;
        Console.WriteLine("Connected with {0} at port {1}", clientep.Address, clientep.Port);
        NetworkStream netStream = new NetworkStream(client);
        StreamWriter sWrite = new StreamWriter(netStream);
        StreamReader sRead = new StreamReader(netStream);
        int iReceive = 1, iSent = 0;
        string strDataReceive = "";
        while (true)
        {
            //Receive
            if (iReceive == 1)
            {
                strDataReceive = sRead.ReadLine();
                Console.WriteLine(strDataReceive);
                iSent = 1;
                iReceive = 0;
            }

            //SENT
            //string strData = Console.ReadLine();
            //string welcome = "Hoang Lam # " + strData; //This is the data we we'll respond with
            if (iSent == 1 && !strDataReceive.Equals(""))
            {
                openMaple.Run(strDataReceive);
                string welcome = OpenMaple.myResult;
                Console.WriteLine(welcome);
                sWrite.WriteLine(welcome);
                sWrite.Flush();
                iReceive = 1;
                iSent = 0;
            }
        }


        Console.WriteLine("Disconnected from {0}", clientep.Address);
        socketReceive.Close();
        listener.Stop();
        client.Close(); //Close Client
        socket.Close(); //Close socket
        
        return 0;
    }
}

//************************Backup********************//
//while (true)
//        {
//            //Console.WriteLine("Waiting for a client connection ...");

//            //Receive
//            //socketReceive = listener.AcceptSocket();
//            //Console.WriteLine("Connection received from " + socketReceive.RemoteEndPoint);
//            //receive
//            //byte[] dataRecieve = new byte[BUFFER_SIZE];
//            //client.Receive(dataRecieve);
//            //string strDataReceive = encoding.GetString(dataRecieve);

//            string strDataReceive = sRead.ReadLine();
//            Console.WriteLine(strDataReceive);
//            //Console.WriteLine("Length : " + strDataReceive.Length);


//            ////Sent
//            //client = listener.AcceptSocket();
//            //client = socket.Accept();
//            //client = listener.AcceptTcpClient();
//            //clientep = (IPEndPoint)client.RemoteEndPoint;
//            //Console.WriteLine("Connected with {0} at port {1}", clientep.Address, clientep.Port);

//            //SENT
//            //string strData = Console.ReadLine();
//            //string welcome = "Hoang Lam # " + strData; //This is the data we we'll respond with
//            //openMaple.Run(strData);
//            //welcome = OpenMaple.myResult;
//            //Console.WriteLine(welcome);
//            ////byte[] data = new byte[welcome.Length];
//            ////data = Encoding.ASCII.GetBytes(welcome); //Encode the data
//            ////client.Send(data, data.Length, SocketFlags.None); //Send the data to the client
//            //sWrite.WriteLine(welcome);
//            //sWrite.Flush();
//        }