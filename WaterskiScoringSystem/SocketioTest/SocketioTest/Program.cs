using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
//using WebApiContrib.Formatting;
using Quobject.SocketIoClientDotNet.Client;

namespace SocketioTest {
	class Program {
        private static String ewcsUri = "http://ewscdata.com:40000";
        private static Socket socketClient = null;
        private static Dictionary<string, string> parameters = new Dictionary<string, string> {
                    { "loggingdetail", "no" }
                    , { "mode", "Tournament" }
                    , { "eventid", "20E016" }
                    , { "provider", "WSTIMS" }
                    , { "application", "WSTIMS" }
                    , { "version", "6.0.1.0" }
                    , { "username", "mawsa@comcast.net" }
                };

        static void Main(string[] args) {
			Console.WriteLine("Hello World!");
            String methodName = "openEwscConnection: ";
            String jsonParameters = new JavaScriptSerializer().Serialize(parameters);

            try {
                socketClient = IO.Socket(ewcsUri);
                socketClient.On(Socket.EVENT_CONNECT, () => {
                    Console.WriteLine(String.Format("Connecting to {0}", ewcsUri));
                    socketClient.Emit("manual_connection_parameter", jsonParameters);
                    Console.WriteLine(String.Format("Connection confirm sent to {0}", jsonParameters));
                });

                socketClient.On("connect_confirm", (data) => {
                    Console.WriteLine(String.Format("Connection confirm {0}", data));
                    socketClient.Close();
                });

                Console.WriteLine("Goodbye World!");
                Console.WriteLine("");

            } catch (Exception ex) {
                Console.WriteLine("Exception encountered: " + ex.Message);
            }
        }
    }
}
