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
using System.ComponentModel;
using System.Threading;
namespace gomoku
{
    enum Player
    {
        None = 0,
        Human = 1,
        Com = 2,
        Onl = 3,
    }
    struct Node
    {
        public int Row;
        public int Column;
        public Node(int rw, int cl)
        {
            this.Row = rw;
            this.Column = cl;
        }
    }

     class ChessBoard
    {
        //Các biến chính
        private int row, column; //Số hàng, cột
        private const int length = 50;//Độ dài mỗi ô
        public static Player currPlayer; //lượt đi
        public Player[,] board; //mảng lưu vị trí các con cờ
        private Player end; //biến kiểm tra trò chơi kết thúc
        private MainWindow frmParent; //Form thực hiện
        private Grid grdBanCo; // Nơi vẽ bàn cờ
        private LuongGiaBanCo eBoard; //Bảng lượng giá bàn cờ
        private cls5OWin OWin; // Kiểm tra 5 ô win

        public Setting Option; // Tùy chọn trò chơi
        //Các biến phụ

        public int[] PhongThu = new int[5] { 0, 1, 9, 85, 769 };
        public int[] TanCong = new int[5] { 0, 2, 28, 256, 2308 };
        
         //tao background worker
        private readonly BackgroundWorker worker = new BackgroundWorker();
        

        //Properties
        public Player End
        {
            get { return this.end; }
            set { this.end = value; }
        }
        public int Row
        {
            get { return this.row; }
        }
        public int Column
        {
            get { return this.column; }
        }
        //Contructors
            public ChessBoard(MainWindow frm, Grid grd)
            {
                Option = new Setting();
                OWin = new cls5OWin();
                row = column = 12;
                currPlayer = Player.None;
                end = Player.None;
                frmParent = frm;
                grdBanCo = grd;
                board = new Player[row, column];
                ResetBoard();
                eBoard = new LuongGiaBanCo(this);

                grdBanCo.MouseDown += new System.Windows.Input.MouseButtonEventHandler(grdBanCo_MouseDown);
                worker.DoWork += doi;
                worker.RunWorkerCompleted += AI_play;
            }

           
            public void ResetBoard()
             {
                 for (int i = 0; i < row; i++)
                 {
                     for (int j = 0; j < column; j++)
                     {
                         board[i, j] = Player.None;
                     }
                 }
             }
        //Bắt đầu trò chơi mới
         public void NewGame()
         {
             currPlayer = this.Option.LuotChoi;//Lấy thông tin lượt chơi
             if (this.Option.WhoPlayWith == Player.Com)//Nếu chọn kiểu chơi với máy
             {
                 if (currPlayer == Player.Com)//Nếu lược đi là máy
                 {
                     DiNgauNhien();
                 }
             }
         }

         public void PlayAgain()
         {
             //OWin = new cls5OWin();
             //grdBanCo.Children.Clear();
             //ResetBoard();
             //this.DrawGomokuBoard();
             //if (this.Option.WhoPlayWith == Player.Com)
             //{
             //    if (end == Player.None)
             //    {
             //        currPlayer = Player.Com;

             //    }
             //    if (currPlayer == Player.Com && this.Option.WhoPlayWith == Player.Com)
             //    {
             //        DiNgauNhien();
             //    }
             //}
             //else
             //{
             //    if (end == Player.None)
             //    {
             //        if (currPlayer == Player.Human)
             //        {
             //            currPlayer = Player.Com;

             //        }
             //        else if (currPlayer == Player.Com)
             //        {
             //            currPlayer = Player.Human;

             //        }
             //    }
             //}
             //end = Player.None;
         }



        //Thiết lập lại toàn bộ dữ liệu bàn cờ
         public void ResetAllBoard()
         {
             OWin = new cls5OWin();
             grdBanCo.Children.Clear();

             ResetBoard();
             end = Player.None;
             this.DrawGomokuBoard();
         }
        //Bắt đầu lại trò chơi mới
         
         public void DiNgauNhien()
         {
             if (currPlayer == Player.Com)
             {
                 board[row / 2, column / 2] = currPlayer;
                 DrawDataBoard(row / 2, column / 2,true,true);
                 currPlayer = Player.Human;
                 OnComDanhXong();//Khai báo sự kiện khi máy đánh xong
             }
         }
         Node node = new Node();
         public static int rows, columns;
          private void AI_play(object sender, RunWorkerCompletedEventArgs e) //Máy đánh
         {
             if (currPlayer == Player.Com && end == Player.None)//Nếu lượt đi là máy và trận đấu chưa kết thúc
             {
                 //Tìm đường đi cho máy
                 eBoard.ResetBoard();
                 LuongGia(Player.Com);//Lượng giá bàn cờ cho máy
                 node = eBoard.GetMaxNode();//lưu vị trí máy sẽ đánh
                 int r, c;
                 r = node.Row; c = node.Column;
                 board[r, c] = currPlayer; //Lưu loại cờ vừa đánh vào mảng
                 DrawDataBoard(r, c, true, true); //Vẽ con cờ theo lượt chơi
                 end = CheckEnd(r, c);//Kiểm tra xem trận đấu kết thúc chưa

                 if (end == Player.Com)//Nếu máy thắng
                 {
                     OnWin();//Khai báo sư kiện Lose
                 }
                 else if (end == Player.None)
                 {
                     currPlayer = Player.Human;//Thiết lập lại lượt chơi
                     OnComDanhXong();// Khai báo sự kiện người đánh xong
                 }
             }
         }
         private void doi(object sender, DoWorkEventArgs e)  //Tìm vị trí cho máy đánh
         {
             Thread.Sleep(1000);
         }
        
        //Hàm đánh cờ
         public void grdBanCo_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
         {
             System.GC.Collect();//Thu gôm rác
             if (Option.WhoPlayWith == Player.Com)//Nếu chọn kiểu chơi đánh với máy
             {
                 Point toado = e.GetPosition(grdBanCo); //Lấy tọa độ chuột
                 //Xử lý tọa độ
                 int cl = ((int)toado.X / length);
                 int rw = ((int)toado.Y / length);


                 if (board[rw, cl] == Player.None) //Nếu ô bấm chưa có cờ
                 {
                     if (currPlayer == Player.Human && end == Player.None)//Nếu lượt đi là người và trận đấu chưa kết thúc
                     {
                         board[rw, cl] = currPlayer;//Lưu loại cờ vừa đánh vào mảng
                         DrawDataBoard(rw, cl, true, true);//Vẽ con cờ theo lượt chơi
                         end = CheckEnd(rw, cl);//Kiểm tra xem trận đấu kết thúc chưa
                         if (end == Player.Human)//Nếu người thắng cuộc là người
                         {
                             OnWin();//Khai báo sự kiện Win
                             //OnWinOrLose();//Hiển thị 5 ô Win.
                         }
                         else if (end == Player.None) //Nếu trận đấu chưa kết thúc
                         {
                             currPlayer = Player.Com;//Thiết lập lại lượt chơi
                             OnHumanDanhXong(); // Khai báo sự kiện người đánh xong
                         }
                     }
                     try
                     {
                         worker.RunWorkerAsync();
                     }
                     catch
                     {

                     }

                 }
             }
             else if (this.Option.WhoPlayWith == Player.Human) //Nếu chọn kiểu chơi 2 người đánh với nhau
             {
                 //Player.Com sẽ đóng vai trò người chơi thứ 2
                 Point toado = e.GetPosition(grdBanCo);//Lấy thông tin tọa độ chuột
                 //Xử lý tọa độ
                 int cl = ((int)toado.X / length);
                 int rw = ((int)toado.Y / length);
                 if (board[rw, cl] == Player.None)//Nếu ô bấm chưa có cờ
                 {
                     if (currPlayer == Player.Human && end == Player.None)//Nếu lượt đi là người và trận đấu chưa kết thúc
                     {
                         board[rw, cl] = currPlayer;//Lưu loại cờ vừa đánh vào mảng
                         DrawDataBoard(rw, cl, true, true);//Vẽ con cờ theo lượt chơi
                         end = CheckEnd(rw, cl);//Kiểm tra xem trận đấu kết thúc chưa
                         if (end == Player.Human)//Nếu người chơi 1 thắng
                         {
                             currPlayer = Player.Human; //Thiết lập lại lượt chơi
                             OnWin();//Khai báo sư kiện Win
                             //OnWinOrLose();//Hiển thị 5 ô Win.
                         }
                         else
                         {
                             currPlayer = Player.Com;//Thiết lập lại lượt chơi
                             OnHumanDanhXong();// Khai báo sự kiện người chơi 1 đánh xong
                         }
                     }
                     else if (currPlayer == Player.Com && end == Player.None)
                     {
                         board[rw, cl] = currPlayer;//Lưu loại cờ vừa đánh vào mảng
                         DrawDataBoard(rw, cl, true, true);//Vẽ con cờ theo lượt chơi
                         end = CheckEnd(rw, cl);//Kiểm tra xem trận đấu kết thúc chưa
                         if (end == Player.Com)//Nếu người chơi 2 thắng
                         {
                             OnWin();//Khai báo sư kiện Win
                             //OnWinOrLose();//Hiển thị 5 ô Win.
                         }
                         else
                         {
                             currPlayer = Player.Human;//Thiết lập lại lượt chơi
                             OnComDanhXong();// Khai báo sự kiện người chơi 2 đánh xong
                         }
                     }
                 }
             }
             else if (this.Option.WhoPlayWith == Player.Onl) // chọn người chơi online
             {
                 
                 Point toado = e.GetPosition(grdBanCo); //Lấy tọa độ chuột
                 //Xử lý tọa độ
                 int cl = ((int)toado.X / length);
                 int rw = ((int)toado.Y / length);
                 
                 if (board[rw, cl] == Player.None) //Nếu ô bấm chưa có cờ
                 {
                     if (currPlayer == Player.Human && end == Player.None)// /Nếu lượt đi là mình và trận đấu chưa kết thúc
                     {
                         //connect.cl = cl;
                         //connect.rw = rw;
                         connect.guitoado(MainWindow.socket, rw, cl);

                         board[rw, cl] = currPlayer;//Lưu loại cờ vừa đánh vào mảng
                         DrawDataBoard(rw, cl, true, true);//Vẽ con cờ theo lượt chơi
                         end = CheckEnd(rw, cl);//Kiểm tra xem trận đấu kết thúc chưa
                         if (end == Player.Human)//Nếu người thắng cuộc là mình
                         {
                             OnWin();//Khai báo sự kiện Win
                         }
                         else if (end == Player.None) //Nếu trận đấu chưa kết thúc
                         {
                             currPlayer = Player.Onl;//Thiết lập lại lượt chơi
                             OnHumanDanhXong(); // Khai báo sự kiện người đánh xong
                         }


                     }
                     else if (currPlayer == Player.Onl && end == Player.None)//Nếu lượt đi là người chơi online và trận đấu chưa kết thúc
                     {
                         connect.guitoado(MainWindow.socket, rw, cl);
                         board[rows, columns] = currPlayer;//Lưu loại cờ vừa đánh vào mảng

                         DrawDataBoard(rows, columns, true, true);//Vẽ con cờ theo lượt chơi
                         end = CheckEnd(rows, columns);//Kiểm tra xem trận đấu kết thúc chưa

                         if (end == Player.Onl)//Nếu người chơi 2 thắng
                         {
                             OnWin();//Khai báo sư kiện Win
                         }
                         else
                         {
                             currPlayer = Player.Human;//Thiết lập lại lượt chơi
                             OnComDanhXong();// Khai báo sự kiện người chơi 2 đánh xong
                         }

                     }
                 }
             }
         }
        //delegate sự kiện Win
         public delegate void WinEventHander();
         public event WinEventHander WinEvent;
         public void OnWin()
         {
             //MessageBox.Show("gasnkdgn;a");
             if (WinEvent != null)
             {
                 WinEvent();
             }
         }
        //delegate sự kiện Replay
         public delegate void ReplayEventHander();
         public event ReplayEventHander ReplayEvent;

        //delegate sự kiện máy đánh xong
         public delegate void ComDanhXongEventHandler();
         public event ComDanhXongEventHandler ComDanhXongEvent;
         private void OnComDanhXong()
         {
             if (ComDanhXongEvent != null)
             {
                 ComDanhXongEvent();
             }
         }
         //delegate sự kiện người đánh xong
         public delegate void HumanDanhXongEventHandler();
         public event HumanDanhXongEventHandler HumanDanhXongEvent;
         private void OnHumanDanhXong()
         {
             if (HumanDanhXongEvent != null)
             {
                 HumanDanhXongEvent();
             }
         }
        //Vẽ lại thế cờ khi đọc dữ liệu từ file
         public void VeTheCo()
         {
             Player temp = currPlayer;
             for (int i = 0; i < Row; i++)
             {
                 for (int j = 0; j < Column; j++)
                 {
                     currPlayer = board[i, j];
                     DrawDataBoard(i, j,false,false);
                 }
             }
             currPlayer = temp;
         }
        
        
        //Hàm lượng giá thế cờ
         private void LuongGia(Player player)
         {
             int cntHuman = 0, cntCom = 0;//Biến đếm Human,Com
             #region Luong gia cho hang
             for (int i = 0; i < row; i++)
             {
                 for (int j = 0; j < column - 4; j++)
                 {
                     //Khởi tạo biến đếm
                     cntHuman = cntCom = 0;
                     //Đếm số lượng con cờ trên 5 ô kế tiếp của 1 hàng
                     for (int k = 0; k < 5; k++)
                     {
                         if (board[i, j + k] == Player.Human) cntHuman++;
                         if (board[i, j + k] == Player.Com) cntCom++;
                     }
                     //Lượng giá
                     //Nếu 5 ô kế tiếp chỉ có 1 loại cờ (hoặc là Human,hoặc la Com)
                     if (cntHuman * cntCom == 0 && cntHuman != cntCom)
                     {
                         //Gán giá trị cho 5 ô kế tiếp của 1 hàng
                         for (int k = 0; k < 5; k++)
                         {
                             //Nếu ô đó chưa có quân đi
                             if (board[i, j + k] == Player.None)
                             {
                                 //Nếu trong 5 ô đó chỉ tồn tại cờ của Human
                                 if (cntCom == 0)
                                 {
                                     //Nếu đối tượng lượng giá là Com
                                     if (player == Player.Com)
                                     {
                                         //Vì đối tượng người chơi là Com mà trong 5 ô này chỉ có Human
                                         //nên ta sẽ cộng thêm điểm phòng thủ cho Com
                                         eBoard.GiaTri[i, j + k] += PhongThu[cntHuman];
                                     }
                                     //Ngược lại cộng điểm phòng thủ cho Human
                                     else
                                     {
                                         eBoard.GiaTri[i, j + k] += TanCong[cntHuman];
                                     }
                                     //Nếu chơi theo luật Việt Nam
                                     //if(this.Option.GamePlay == LuatChoi.Vietnamese)
                                     //    //Xét trường hợp chặn 2 đầu
                                     //   //Nếu chận 2 đầu thì gán giá trị cho các ô đó bằng 0
                                     //    if (j - 1 >= 0 && j + 5 <= column - 1 && board[i, j - 1] == Player.Com && board[i, j + 5] == Player.Com)
                                     //    {
                                     //        eBoard.GiaTri[i, j + k] = 0;
                                     //    }

                                 }
                                 //Tương tự như trên
                                 if (cntHuman == 0) //Nếu chỉ tồn tại Com
                                 {
                                     if (player == Player.Human) //Nếu người chơi là Người
                                     {
                                         eBoard.GiaTri[i, j + k] += PhongThu[cntCom];
                                     }
                                     else
                                     {
                                         eBoard.GiaTri[i, j + k] += TanCong[cntCom];
                                     }
                                     //Trường hợp chặn 2 đầu
                                     //if (this.Option.GamePlay == LuatChoi.Vietnamese)
                                     //    if (j - 1 >= 0 && j + 5 <= column - 1 && board[i, j - 1] == Player.Human && board[i, j + 5] == Player.Human)
                                     //    {
                                     //        eBoard.GiaTri[i, j + k] = 0;
                                     //    }

                                 }
                                 if ((j + k - 1 > 0) && (j + k + 1 <= column - 1) && (cntHuman == 4 || cntCom == 4)
                                    && (board[i, j + k - 1] == Player.None || board[i, j + k + 1] == Player.None))
                                 {
                                     eBoard.GiaTri[i, j + k] *= 3;
                                 }
                             }
                         }
                     }
                 }
             }
             #endregion
             //Tương tự như lượng giá cho hàng
             #region Luong gia cho cot
             for (int i = 0; i < row - 4; i++)
             {
                 for (int j = 0; j < column; j++)
                 {
                     cntHuman = cntCom = 0;
                     for (int k = 0; k < 5; k++)
                     {
                         if (board[i + k, j] == Player.Human) cntHuman++;
                         if (board[i + k, j] == Player.Com) cntCom++;
                     }
                     if (cntHuman * cntCom == 0 && cntCom != cntHuman)
                     {
                         for (int k = 0; k < 5; k++)
                         {
                             if (board[i + k, j] == Player.None)
                             {
                                 if (cntCom == 0)
                                 {
                                     if (player == Player.Com) eBoard.GiaTri[i + k, j] += PhongThu[cntHuman];
                                     else eBoard.GiaTri[i + k, j] += TanCong[cntHuman];
                                     // Truong hop bi chan 2 dau.
                                     if ((i - 1) >= 0 && (i + 5) <= row - 1 && board[i - 1, j] == Player.Com && board[i + 5, j] == Player.Com)
                                     {
                                         eBoard.GiaTri[i + k, j] = 0;
                                     }
                                 }
                                 if (cntHuman == 0)
                                 {
                                     if (player == Player.Human) eBoard.GiaTri[i + k, j] += PhongThu[cntCom];
                                     else eBoard.GiaTri[i + k, j] += TanCong[cntCom];
                                     // Truong hop bi chan 2 dau.
                                     //if (this.Option.GamePlay == LuatChoi.Vietnamese)
                                     //    if (i - 1 >= 0 && i + 5 <= row - 1 && board[i - 1, j] == Player.Human && board[i + 5, j] == Player.Human)
                                     //        eBoard.GiaTri[i + k, j] = 0;
                                 }
                                 if ((i + k - 1) >= 0 && (i + k + 1) <= row - 1 && (cntHuman == 4 || cntCom == 4)
                                     && (board[i + k - 1, j] == Player.None || board[i + k + 1, j] == Player.None))
                                 {
                                     eBoard.GiaTri[i + k, j] *= 3;
                                 }
                             }
                         }
                     }
                 }
             }
             #endregion
             //Tương tự như lượng giá cho hàng
             #region  Luong gia tren duong cheo chinh (\)
             for (int i = 0; i < row - 4; i++)
             {
                 for (int j = 0; j < column - 4; j++)
                 {
                     cntHuman = cntCom = 0;
                     for (int k = 0; k < 5; k++)
                     {
                         if (board[i + k, j + k] == Player.Human) cntHuman++;
                         if (board[i + k, j + k] == Player.Com) cntCom++;
                     }
                     if (cntHuman * cntCom == 0 && cntCom != cntHuman)
                     {
                         for (int k = 0; k < 5; k++)
                         {
                             if (board[i + k, j + k] == Player.None)
                             {
                                 if (cntCom == 0)
                                 {
                                     if (player == Player.Com) eBoard.GiaTri[i + k, j + k] += PhongThu[cntHuman];
                                     else eBoard.GiaTri[i + k, j + k] += TanCong[cntHuman];
                                     // Truong hop bi chan 2 dau.
                                     //if(this.Option.GamePlay == LuatChoi.Vietnamese)
                                     //    if (i - 1 >= 0 && j - 1 >= 0
                                     //        && i + 5 <= row - 1 && j + 5 <= column - 1
                                     //        && board[i - 1, j - 1] == Player.Com && board[i + 5, j + 5] == Player.Com)
                                     //        eBoard.GiaTri[i + k, j + k] = 0;
                                 }
                                 if (cntHuman == 0)
                                 {
                                     if (player == Player.Human) eBoard.GiaTri[i + k, j + k] += PhongThu[cntCom];
                                     else eBoard.GiaTri[i + k, j + k] += TanCong[cntCom];
                                     // Truong hop bi chan 2 dau.
                                     //if (this.Option.GamePlay == LuatChoi.Vietnamese)
                                     //    if ((i - 1) >= 0 && j - 1 >= 0
                                     //        && i + 5 <= row - 1 && j + 5 <= column - 1
                                     //        && board[i - 1, j - 1] == Player.Human && board[i + 5, j + 5] == Player.Human)
                                     //    {
                                     //        eBoard.GiaTri[i + k, j + k] = 0;
                                     //    }
                                 }
                                 if ((i + k - 1) >= 0 && (j + k - 1) >= 0 && (i + k + 1) <= row - 1 && (j + k + 1) <= column - 1 && (cntHuman == 4 || cntCom == 4)
                                     && (board[i + k - 1, j + k - 1] == Player.None || board[i + k + 1, j + k + 1] == Player.None))
                                 {
                                     eBoard.GiaTri[i + k, j + k] *= 3;
                                 }
                             }
                         }
                     }
                 }
             }
             #endregion
             //Tương tự như lượng giá cho hàng
             #region Luong gia tren duong cheo phu (/)
             for (int i = 4; i < row - 4; i++)
             {
                 for (int j = 0; j < column - 4; j++)
                 {
                     cntCom = 0; cntHuman = 0;
                     for (int k = 0; k < 5; k++)
                     {
                         if (board[i - k, j + k] == Player.Human) cntHuman++;
                         if (board[i - k, j + k] == Player.Com) cntCom++;
                     }
                     if (cntHuman * cntCom == 0 && cntHuman != cntCom)
                     {
                         for (int k = 0; k < 5; k++)
                         {
                             if (board[i - k, j + k] == Player.None)
                             {
                                 if (cntCom == 0)
                                 {
                                     if (player == Player.Com) eBoard.GiaTri[i - k, j + k] += PhongThu[cntHuman];
                                     else eBoard.GiaTri[i - k, j + k] += TanCong[cntHuman];
                                     // Truong hop bi chan 2 dau.
                                     if (i + 1 <= row - 1&&j - 1>=0&&i-5>=0&&j+5<=column-1&& board[i + 1, j - 1] == Player.Com && board[i - 5, j + 5] == Player.Com)
                                     {
                                         eBoard.GiaTri[i - k, j + k] = 0;
                                     }
                                 }
                                 if (cntHuman == 0)
                                 {
                                     if (player == Player.Human) eBoard.GiaTri[i - k, j + k] += PhongThu[cntCom];
                                     else eBoard.GiaTri[i - k, j + k] += TanCong[cntCom];
                                     // Truong hop bi chan 2 dau.
                                     //if(this.Option.GamePlay== LuatChoi.Vietnamese)
                                     //    if (i + 1 <= row - 1 && j - 1 >= 0 && i - 5 >= 0 && j + 5 <= column - 1 && board[i + 1, j - 1] == Player.Human && board[i - 5, j + 5] == Player.Human)
                                     //    {
                                     //        eBoard.GiaTri[i - k, j + k] = 0;
                                     //    }
                                 }
                                 if ((i - k + 1) <= row - 1 && (j + k - 1) >= 0
                                     && (i - k - 1) >= 0 && (j + k + 1) <= column - 1
                                     && (cntHuman == 4 || cntCom == 4)
                                     && (board[i - k + 1, j + k - 1] == Player.None || board[i - k - 1, j + k + 1] == Player.None))
                                 {
                                     eBoard.GiaTri[i - k, j + k] *= 3;
                                 }
                             }
                         }
                     }
                 }
             }
             #endregion
         }
        //Hàm lấy đối thủ của người chơi hiện tại
         private Player DoiNgich(Player cur)
         {
             if (cur == Player.Com) return Player.Human;
             if (cur == Player.Human) return Player.Com;
             return Player.None;
         }
        //Hàm kiểm tra trận đấu kết thúc chưa
         private Player CheckEnd(int rw, int cl)
         {
             int rowTemp = rw;
             int colTemp = cl;
             int count1, count2, count3, count4;
             count1 = count2 = count3 = count4 = 1;
             Player cur = board[rw, cl];
             OWin.Reset();
             OWin.Add(new Node(rowTemp, colTemp));
             #region Kiem Tra Hang Ngang
             while (colTemp - 1 >= 0 && board[rowTemp, colTemp - 1] == cur)
             {
                 OWin.Add(new Node(rowTemp, colTemp - 1));
                 count1++;
                 colTemp--;
             }
             colTemp = cl;
             while (colTemp + 1 <= column - 1 && board[rowTemp, colTemp + 1] == cur)
             {
                 OWin.Add(new Node(rowTemp, colTemp + 1));
                 count1++;
                 colTemp++;
             }
             if (count1 == 5)
             {
                 
                 return cur;
             }
             #endregion
             #region Kiem Tra Hang Doc
             OWin.Reset();
             colTemp = cl;
             OWin.Add(new Node(rowTemp, colTemp));
            
             while (rowTemp - 1 >= 0 && board[rowTemp - 1, colTemp] == cur)
             {
                 OWin.Add(new Node(rowTemp-1, colTemp));
                 count2++;
                 rowTemp--;
             }
             rowTemp = rw;
             while (rowTemp + 1 <= row - 1 && board[rowTemp + 1, colTemp] == cur)
             {
                 OWin.Add(new Node(rowTemp+1, colTemp));
                 count2++;
                 rowTemp++;
             }
             if (count2 == 5)
             {

                 return cur;
             }
             #endregion
             #region Kiem Tra Duong Cheo Chinh (\)
             colTemp = cl;
             rowTemp = rw;
             OWin.Reset();
             OWin.Add(new Node(rowTemp, colTemp));
             while (rowTemp - 1 >= 0 && colTemp - 1 >= 0 && board[rowTemp - 1, colTemp - 1] == cur)
             {
                 OWin.Add(new Node(rowTemp - 1, colTemp - 1));
                 count3++;
                 rowTemp--;
                 colTemp--;
             }
             rowTemp = rw;
             colTemp = cl;
             while (rowTemp + 1 <= row - 1 && colTemp + 1 <= column - 1 && board[rowTemp + 1, colTemp + 1] == cur)
             {
                 OWin.Add(new Node(rowTemp + 1, colTemp + 1));
                 count3++;
                 rowTemp++;
                 colTemp++;
             }
             if (count3 == 5)
             {
                 
                 return cur;
             }
             #endregion
             #region Kiem Tra Duong Cheo Phu
             rowTemp = rw;
             colTemp = cl;
             OWin.Reset();
             OWin.Add(new Node(rowTemp, colTemp));
             while (rowTemp + 1 <= row - 1 && colTemp - 1 >= 0 && board[rowTemp + 1, colTemp - 1] == cur)
             {
                 OWin.Add(new Node(rowTemp + 1, colTemp - 1));
                 count4++;
                 rowTemp++;
                 colTemp--;
             }
             rowTemp = rw;
             colTemp = cl;
             while (rowTemp - 1 >= 0 && colTemp + 1 <= column - 1 && board[rowTemp - 1, colTemp + 1] == cur)
             {
                 OWin.Add(new Node(rowTemp - 1, colTemp + 1));
                 count4++;
                 rowTemp--;
                 colTemp++;
             }
             if (count4 == 5)
             {
                 
                 return cur;
             }
             #endregion
             return Player.None;
         }
        //Hàm lấy thông tin 5 ô Win hoặc Lose
        //Hàm xem lại trò chơi vừa đấu
         
        
         private void DrawDataBoard(int rw, int cl,bool record,bool type)
         {
             if (type == true)
             {
                 if (currPlayer == Player.Human)
                 {
                     UserControl chess;
                     chess = new ChessO_3();
                     chess.Height = length;
                     chess.Width = length;
                     chess.HorizontalAlignment = 0;
                     chess.VerticalAlignment = 0;
                     chess.Margin = new Thickness(cl * length, rw * length, 0, 0);
                     grdBanCo.Children.Add(chess);
                                         
                 }
                 else if (currPlayer == Player.Com || currPlayer == Player.Onl)
                 {
                     UserControl chess;
                     chess = new ChessX_3();
                     chess.Height = length;
                     chess.Width = length;
                     chess.HorizontalAlignment = 0;
                     chess.VerticalAlignment = 0;
                     chess.Margin = new Thickness(cl * length, rw * length, 0, 0);
                     grdBanCo.Children.Add(chess);
                  }

             }
             else
             {
                 Image Chess1 = new Image();
                 if (currPlayer == Player.Human)
                 {
                     Chess1.Source = new BitmapImage(new Uri("pack://application:,,,/Icon/Chess/Chess_0_1.png"));
                     Chess1.Width = Chess1.Height = length;
                     Chess1.HorizontalAlignment = 0;
                     Chess1.VerticalAlignment = 0;
                     Chess1.Margin = new Thickness(cl * length, rw * length, 0, 0);
                     Chess1.Opacity = 100;
                     grdBanCo.Children.Add(Chess1);
                 }
                 else if (currPlayer == Player.Com || currPlayer == Player.Onl)
                 {
                     Image Chess2 = new Image();
                     Chess2.Source = new BitmapImage(new Uri("pack://application:,,,/Icon/Chess/Chess_X_3.png"));
                     Chess2.Width = Chess2.Height = length;
                     Chess2.HorizontalAlignment = 0;
                     Chess2.VerticalAlignment = 0;
                     Chess2.Margin = new Thickness(cl * length, rw * length, 0, 0);
                     Chess2.Opacity = 100;
                     grdBanCo.Children.Add(Chess2);
                 }
             }
         }
         //Hàm vẽ bàn cờ
         public void DrawGomokuBoard()
         {
             for (int i = 0; i < row+1; i++)
             {
                 Line line = new Line();

                 line.Stroke = Brushes.Red;
                 line.X1 = 0;
                 line.Y1 = i * length;
                 line.X2 = length * row;
                 line.Y2 = i * length;
                 line.HorizontalAlignment = HorizontalAlignment.Left;
                 line.VerticalAlignment = VerticalAlignment.Top;
                 grdBanCo.Children.Add(line);
             }
             for (int i = 0; i < column+1; i++)
             {
                 Line line = new Line();
                 line.Stroke = Brushes.Red;
                 line.X1 = i * length;
                 line.Y1 = 0;
                 line.X2 = i * length;
                 line.Y2 = length * column;
                 line.HorizontalAlignment = HorizontalAlignment.Left;
                 line.VerticalAlignment = VerticalAlignment.Top;
                 grdBanCo.Children.Add(line);
             }

         }
    }
}
