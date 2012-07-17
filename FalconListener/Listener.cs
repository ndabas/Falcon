using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Web;

namespace FalconListener
{
    class Listener
    {
        private UdpClient client;
        private WebClient web = new WebClient();

        public void Start()
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 8192);
            client = new UdpClient(endpoint);
            client.BeginReceive(new AsyncCallback(this.Receive), endpoint);
        }

        public void Stop()
        {
            client.Close();
        }

        private void Receive(IAsyncResult ar)
        {
            IPEndPoint endpoint = (IPEndPoint)ar.AsyncState;
            byte[] receiveBytes = client.EndReceive(ar, ref endpoint);
            client.BeginReceive(new AsyncCallback(this.Receive), endpoint);
            string receiveString = Encoding.ASCII.GetString(receiveBytes);

            try
            {
                string url = "http://track.gpsconvergence.com/TrackingData/Report2?data=";
                web.DownloadString(url + HttpUtility.UrlEncode(receiveString));
            }
            catch (Exception)
            {
            }

            Console.Write(receiveString);         
        }
    }
}
