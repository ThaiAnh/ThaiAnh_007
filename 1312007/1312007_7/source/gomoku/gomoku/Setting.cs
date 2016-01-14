using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gomoku
{
    enum LuatChoi
    {
        None,
        International,
        //Vietnamese,
    }
    enum ChessStyle
    {
        Chess3,
    }

    class Setting
    {
        private LuatChoi gamePlay = LuatChoi.None;//Luật chơi
        private Player whoPlayWith = Player.None;//Kiểu chơi
        private ChessStyle kindOfChess = ChessStyle.Chess3;//Loại cờ

        //private int time=20;//Thời gian
        private string playerA = "";//Tên người chơi thứ 1
        private string playerB = "";//Tên người chơi thứ 2
        private string playerOnline = ""; //Tên người chơi online

        public string PlayerOnline
        {
            get { return playerOnline; }
            set { playerOnline = value; }
        }
        private Player luotChoi= Player.Human;//Lượt đi


        public Player LuotChoi
        {
            get{return this.luotChoi;}
            set{this.luotChoi=value;}
        }
        public LuatChoi GamePlay
        {
            get { return this.gamePlay; }
            set { this.gamePlay = value; }
        }
        public Player WhoPlayWith
        {
            get { return this.whoPlayWith; }
            set { this.whoPlayWith = value; }
        }

        public ChessStyle KindOfChess
        {
            get { return this.kindOfChess; }
            set { this.kindOfChess = value; }
        }

        public string PlayerAName
        {
            get { return this.playerA; }
            set { this.playerA = value; }
        }
        public string PlayerBName
        {
            get { return this.playerB; }
            set { this.playerB = value; }
        }
    }
}
