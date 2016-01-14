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
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Navigation;
using System.Configuration;

namespace gomoku
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        ChessBoard banco;
        public static TextBox txt = new TextBox();

        public static int type = 0;


        public static Socket socket = IO.Socket("ws://gomoku-lajosveres.rhcloud.com:8000");

        public MainWindow()
        {
            InitializeComponent();
            
            banco = new ChessBoard(this, grdBanCo);
            banco.DrawGomokuBoard();

            banco.Option.KindOfChess = ChessStyle.Chess3;

            banco.Option.GamePlay = LuatChoi.International;
            banco.Option.WhoPlayWith = Player.Human;
            grdBanCo.MouseDown += new System.Windows.Input.MouseButtonEventHandler(banco.grdBanCo_MouseDown);
            banco.WinEvent += new ChessBoard.WinEventHander(Win);

            
            if (txtMessages.Text == "")
            {
                txtMessages.Text = "Type your message here...";
            }
        }

        private void Win()
        {

            if (banco.End == Player.Human )
            {
                string temp1 = banco.Option.PlayerAName;
                txtMes.Text = temp1 + "\n" + temp1.ToUpper()  +" là người chiến thắng" + "\t" + LayThoiGian().ToString();
            }
            else if (banco.End == Player.Com)
            {
                string temp2 = banco.Option.PlayerBName ;
                txtMes.Text = temp2 + "\n" + temp2.ToUpper() + " là người chiến thắng" + "\t" + LayThoiGian().ToString();
            }
            else if (banco.End == Player.Onl)
            {
                string temp2 = banco.Option.PlayerOnline ;
                txtMes.Text = temp2 + "\n" + temp2.ToUpper()+ " là người chiến thắng" + "\t" + LayThoiGian().ToString();
            }
        
        }

        
        public string LayThoiGian()
        {
            string sTg = DateTime.Now.ToShortTimeString();
            return sTg;
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            
            connect.sendmessage(socket, txtMessages.Text, txtPlayerOnline.Text.ToUpper());
            txtMessages.Text = "";
        }

        private void btnDanhVoiMay_Click(object sender, RoutedEventArgs e)
        {
            txtPlayer1.IsEnabled = true;
            txtPlayerOnline.IsEnabled = false;
            btnChange.IsEnabled = false;
            txtPlayer1.Focus();

            if (txtPlayer1.Text == "")
            {
                MessageBox.Show("Nhập tên người chơi!");
                txtPlayer1.Focus();
            }
            txtPlayer_2.IsReadOnly = true;
            txtPlayer_2.Text = "MÁY";
            banco.Option.PlayerAName = txtPlayer1.Text;
            banco.Option.PlayerBName = txtPlayer_2.Text;
            banco.Option.WhoPlayWith = Player.Com;
            banco.NewGame();
        }

       

        private void btnDanhVoiNguoi_Click_1(object sender, RoutedEventArgs e)
        {
            txtPlayerOnline.IsEnabled = false;
            btnChange.IsEnabled = false;
            txtPlayer1.IsEnabled = true;
            txtPlayer_2.IsEnabled = true;
            
            if (txtPlayer1.Text == txtPlayer_2.Text || txtPlayer_2.Text == "" || txtPlayer1.Text == "")
            {
                MessageBox.Show("Tên người chơi không hợp lê!");
            }
            banco.Option.PlayerAName = txtPlayer1.Text;
            banco.Option.PlayerBName = txtPlayer_2.Text;

            banco.NewGame();
        }

        private void btnDanhOnline_Click(object sender, RoutedEventArgs e)
        {
            type = 1;
            socket = IO.Socket(ConfigurationManager.ConnectionStrings["con"].ConnectionString);
            connect.connected(socket, txtPlayerOnline.Text);

            txtPlayerOnline.Text.ToUpper();     
            if (txtPlayerOnline.Text =="")
            {
                MessageBox.Show("Tên người chơi chưa hợp lệ!");
                txtPlayerOnline.Focus();
            }
            

            txtPlayer1.IsReadOnly = true;
            txtPlayer_2.IsReadOnly = true;
            txtPlayerOnline.Focus();
            txtPlayer1.IsEnabled = false;
            txtPlayer_2.IsEnabled = false;
            txtPlayerOnline.IsEnabled = true;
            btnChange.IsEnabled = true;

            banco.Option.GamePlay = LuatChoi.International;

            banco.Option.WhoPlayWith = Player.Onl;
            if (connect.turn == true)
            {
                //BanCo.currPlayer = Player.Human;
            }

            else
            {
            }  
                ChessBoard.currPlayer = Player.Onl;
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            connect.changname(socket, txtPlayerOnline.Text);
        }

        private void txtMessages_LostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            if (txtMessages.Text == "")
            {

                txtMessages.Foreground = new SolidColorBrush(Color.FromRgb(135, 135, 135));
                txtMessages.Text = "Type your message here...";
            }
        }

        private void txtMessages_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtMessages.Text == "Type your message here...")
            {
                txtMessages.Text = "";
                txtMessages.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void txtMes_TextChanged(object sender, TextChangedEventArgs e)
        {
           
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            banco.ResetAllBoard();
        }
    }
}
