using System;
using System.Net;
using System.Net.Sockets;

namespace WaterskiScoringSystem.Externalnterface {
    class SendTcpipMessage {

        public SendTcpipMessage() {
        }

        public bool SendMessage( String inServer, int inPort, String inMessage ) {
            //byte[] curBuffer = new Byte();
            TcpClient curTcpClient = new TcpClient();
            Socket curSocket = curTcpClient.Client;
            IPHostEntry curHostEntry = Dns.GetHostEntry( inServer );
            
            //curSocket.Send( curButter );

            return true;
        }

    }
}
