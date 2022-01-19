using System;

namespace WscListener {
	class Program {
		private static String myWcsWebLocation = "http://ewscdata.com:40000/";
		private static String mySanctionNum = "";
		private static String myEventSubId = "";

		//private static Quobject.SocketIoClientDotNet.Client.Socket socketClient = null;

		static void Main( string[] args ) {
			String curMethodName = "Listener.Main: ";

			if ( args.Length != 2 ) {
				Console.WriteLine( curMethodName + "Invalid number of arguements " + args.Length );
				return;
			}

			mySanctionNum = args[0];
			myEventSubId = args[1];

			Console.WriteLine(
				"Name: " + System.Reflection.Assembly.GetCallingAssembly().GetName()
				+ System.Environment.NewLine + "Sanction: " + mySanctionNum
				+ System.Environment.NewLine + "myEventSubId: " + myEventSubId
				+ System.Environment.NewLine
				); ;
		}
	}
}
