using gpstrackerd;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class TrackerListener
{
    TcpListener server = null;
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public delegate void dg_TrackingInfoReceived(TrackerMessage message);
    public event dg_TrackingInfoReceived TrackingInfoReceived;
    
    public string IP { get; set; }
    public int Port { get; set; }

    public TrackerListener(string ip, int port)
    {
        this.IP = ip;
        this.Port = port;
    }
    public void StartListener()
    {
        IPAddress localAddr = IPAddress.Parse(this.IP);
        server = new TcpListener(localAddr, this.Port);
        log.InfoFormat("Listening on {0}:{1}", this.IP, this.Port);
        server.Start();
        try
        {
            while (true)
            {
                
                TcpClient client = server.AcceptTcpClient();
                log.InfoFormat("Accepted connection from {0}", client.Client.RemoteEndPoint.ToString());
                Thread t = new Thread(new ParameterizedThreadStart(HandleGPSTracker));
                t.Start(client);
            }
        }
        catch (SocketException e)
        {
            log.ErrorFormat("SocketException: {0}", e);
            server.Stop();
        }
    }
    public void HandleGPSTracker(Object obj)
    {
        TcpClient client = (TcpClient)obj;
        var stream = client.GetStream();
        string deviceID = "";

        //client.ReceiveTimeout = 500;

        try
        {
            string command = "";
            byte receivedChar;
            while (true)
            {
                try
                {
                    int receivedData = stream.ReadByte();
                    if (receivedData == -1)
                        continue;
                    receivedChar = (byte)receivedData;
                    if (receivedChar == '*')
                    {
                        command = "";
                    } else if (receivedChar == '#')
                    {
                        // process full message
                        log.Debug(command);
                        string[] segments = command.Split(',');
                        // 0  1          2  3      4 5         6 7          8 9      10
                        // HQ,6028189208,V1,154919,A,5103.6043,N,00343.9613,E,000.40,344,130221,FFFFFBFF,206,20,1501,51814,05,27
                        if (segments.Length == 19)
                        {
                            
                            if (segments[1] != deviceID)
                            {
                                log.DebugFormat("Client sent deviceID: {0}", segments[1]);
                                if (deviceID != "")
                                {
                                    log.WarnFormat("Device ID changed in-flight! old: {0} new: {1}", deviceID, segments[1]);
                                }
                                deviceID = segments[1];
                            }
                            if (segments[2] != "V1")
                                continue;

                            // check if receiver has lock on GPS
                            if (segments[4] != "A")
                                continue;

                            var trackerMessage = new TrackerMessage(deviceID);
                            trackerMessage.SetLatLong(segments[5], segments[6], segments[7], segments[8]);
                            trackerMessage.Speed = Convert.ToDouble(segments[9]);
                            trackerMessage.Direction = Convert.ToInt32(segments[10]);
                            log.InfoFormat("Received tracker message: {0}", trackerMessage.ToString());
                            if (TrackingInfoReceived != null)
                            {
                                TrackingInfoReceived(trackerMessage);
                            }
                        }
                    }
                    else
                    {
                        command += ASCIIEncoding.ASCII.GetString(new byte[] { receivedChar });
                    }
                } catch(Exception e)
                {
                    log.Debug(e);
                    //log.Info("Didn't receive anything");
                }
            }
        } 
        catch(SocketException e)
        {
            log.ErrorFormat("SocketException: {0}", e);
        }
        
    }

    
}
