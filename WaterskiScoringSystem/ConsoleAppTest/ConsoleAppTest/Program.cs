using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quobject.SocketIoClientDotNet.Client;
using Quobject.EngineIoClientDotNet;
using Newtonsoft.Json;

namespace ConsoleAppTest
{
    class Program
    {
        private static String ewcsUri = "http://ewscdata.com:40000/";
        private static Quobject.SocketIoClientDotNet.Client.Socket socketClient = null;
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
            int count = 1;
            String jsonParameters = JsonConvert.SerializeObject(parameters);

            try {
                socketClient = IO.Socket("http://ewscdata.com:40000");

                socketClient.On(Socket.EVENT_CONNECT, () => {
                    Console.WriteLine(String.Format("Connected to {0}", ewcsUri));
                    socketClient.Emit("manual_connection_parameter", jsonParameters);
                    Console.WriteLine(String.Format("manual_connection_parameter confirm sent with parameters... {0}", jsonParameters));
                });

                socketClient.On("connect_confirm", (data) => {
                    Console.WriteLine(String.Format("connect_confirm: {0}", data));
                });

                count = 1;
                while (count < 10) {
                    System.Threading.Thread.Sleep(2000);
                    Console.WriteLine(String.Format("Waiting patiently {0}", count));
                    count++;
                }

            } catch (Exception ex) {
                Console.WriteLine("Exception encountered: " + ex.Message);
            }

            Dictionary<string, string> sendAthleteMsg = new Dictionary<string, string> {
                    { "athleteId", "700040630" }
                    , { "athleteName", "David Allen" }
                    , { "event", "slalom" }
                    , { "passNumber", "2" }
                    , { "speed", "52" }
                    , { "rope", "18.25" }
                    , { "split", "0" }
                };
            jsonParameters = JsonConvert.SerializeObject(sendAthleteMsg);
            Console.WriteLine(String.Format("send_preview {0}", jsonParameters));
            socketClient.Emit("athlete_data", jsonParameters);

            socketClient.On("send_confirm", (data) => {
                Console.WriteLine(String.Format("send_confirm {0}", data));
            });

            socketClient.On("trickscoring_score", (data) => {
                Console.WriteLine(String.Format("trickscoring_score {0}", data));
            });
            
            socketClient.On("jumpmeasurement_score", (data) => {
                Console.WriteLine(String.Format("jumpmeasurement_score {0}", data));
            });
            
            count = 1;
            while (count < 10) {
                System.Threading.Thread.Sleep(2000);
                Console.WriteLine(String.Format("Waiting patiently again {0}", count));
                count++;
            }
            socketClient.Close();

        }
    }
}
