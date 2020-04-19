using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WaterskiScoringSystem.Tools {
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
