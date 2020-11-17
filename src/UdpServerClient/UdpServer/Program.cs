using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UdpServer {
	class Program {
		static Socket socket;
		public const int bufSize = 8 * 1024;
		private static State state = new State();
		private static EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);
		private static Boolean keyPressed = false;
		private static AsyncCallback recv = null;

		static async Task Main(string[] args) {
			Console.WriteLine("Udp Server started!");
			Console.CancelKeyPress += Console_CancelKeyPress;

			socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

			socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
			socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000));
//			while (keyPressed == false) {
				Receive();
			//		}
			Console.ReadKey();
		}

		static void Console_CancelKeyPress(Object sender, ConsoleCancelEventArgs e) {
			Console.WriteLine(e.SpecialKey);
			if (e.SpecialKey == ConsoleSpecialKey.ControlC) {
				e.Cancel = true;
				keyPressed = true;
			}
		}

		private static void Receive() {
			socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv = (ar) => {
				State so = (State)ar.AsyncState;
				int bytes = socket.EndReceiveFrom(ar, ref epFrom);
				socket.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv, so);
				Console.WriteLine("RECV: {0}: {1}, {2}", epFrom.ToString(), bytes, Encoding.ASCII.GetString(so.buffer, 0, bytes));
			}, state);
		}
	}

	public class State {
		public byte[] buffer = new byte[Program.bufSize];
	}
}
