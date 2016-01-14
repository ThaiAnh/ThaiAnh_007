using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using System.Windows.Threading;
using System.IO;
using System.Windows.Media.Imaging;
using System.Drawing;



namespace gomoku 
{
    class connect 
    {
        public static bool turn;
        public static string copy;
        public static string Ten;
        public static int rw = -1, cl = -1;
        public static void connected(Quobject.SocketIoClientDotNet.Client.Socket socket, string name)
        {
     
            //socket.On(Socket.EVENT_CONNECT, () =>
            //{
            //    MessageBox.Show("connected", "Thông báo ");

            //});
            socket.On(Socket.EVENT_MESSAGE, (data) =>
            {
                MessageBox.Show(data.ToString());
                
            });
            socket.On(Socket.EVENT_CONNECT_ERROR, (data) =>
            {
                MessageBox.Show(data.ToString());
            });

            socket.On("ChatMessage", (data) =>
            {
                string s = "You are the first player!";
                turn = data.ToString().Contains(s);
                if (MainWindow.type == 4 && turn == true)
                {
                    Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        MainWindow.txt.Text = data.ToString();
                    }));
                }

                string ten = "";
                var o = JObject.Parse(data.ToString());
                if ((string)o["from"] != null)
                {
                    ten = (string)o["from"];
                }
                else
                {
                    ten = "Server";
                }
                var jobject = data as JToken;
                AppSetting.mes.Mes += ten + "\t\t" + DateTime.Now.ToString() + "\n" + jobject.Value<String>("message") + "\n\t***************************************\n\n";

                if (((Newtonsoft.Json.Linq.JObject)data)["message"].ToString() == "Welcome!")
                {
                    socket.Emit("MyNameIs", name);
                    socket.Emit("ConnectToOtherPlayer");
                    Ten = name;

                }

            });
          
            socket.On(Socket.EVENT_ERROR, (data) =>
            {
                MessageBox.Show(data.ToString());
            });

            //ham nhan toa do 
            socket.On("NextStepIs", (data) =>
            {
                var o = JObject.Parse(data.ToString());
                if (MainWindow.type == 1)
                {
                    ChessBoard.currPlayer = Player.Onl;
                    //BanCo.currPlayer = o.Value<int>("player");
                    ChessBoard.rows = (int)o["row"];
                    ChessBoard.columns = (int)o["col"];
                }

                
            });
            
        }
        public static void guitoado(Quobject.SocketIoClientDotNet.Client.Socket socket, int row, int col)
        {

            socket.On(Socket.EVENT_ERROR, (data) =>
            {
                MessageBox.Show(data.ToString());
            });

            socket.Emit("MyStepIs", JObject.FromObject(new { row = row, col = col }));
        }
        public static void changname(Quobject.SocketIoClientDotNet.Client.Socket socket, string name)
        {
            socket.Emit("MyNameIs", name);
            socket.Emit("message:", copy + "is now called" + name);
            copy = name;
        }
        public static void sendmessage (Quobject.SocketIoClientDotNet.Client.Socket socket,string txt, string name)
        {            
            socket.Emit("ChatMessage", txt);
            socket.Emit("message:" + txt.ToUpper(), "from:" +name.ToUpper());        
        }
    }
}
