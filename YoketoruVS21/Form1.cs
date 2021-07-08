using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace YoketoruVS21
{
    public partial class Form1 : Form
    {
        const bool isDebug = true;

        const int SpeedMax = 20;

        const int PlayerMax = 1;
        const int EnemyMax = 3;
        const int ItemMax = 3;
        const int ChrMax = PlayerMax + EnemyMax + ItemMax;

        Label[] chrs = new Label[ChrMax];
        int[] vx = new int[ChrMax];
        int[] vy = new int[ChrMax];
        int itemCount = 0;

        const int PlayerIndex = 0;
        const int EnemyIndex = PlayerMax;
        const int ItemIndex = EnemyIndex + EnemyMax;

        const string PlayerText = "( ^)o(^ )";
        const string EnemyText = "◆";
        const string ItemText = "★";

        static Random rand = new Random();

        enum State
        {
            None = -1,  //無効
            Title,    //タイトル
            Game,     //ゲーム
            Gameover, //ゲームオーバー
            Clear,    //クリア
        }
        State currentState = State.None;
        State nextState = State.Title;

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        public Form1()
        {
            InitializeComponent();

            for (int i = 0; i < ChrMax; i++)
            {
                chrs[i] = new Label();
                chrs[i].AutoSize = true;
                if (i == PlayerIndex)
                {
                    chrs[i].Text = PlayerText;
                }
                else if (i < ItemIndex)
                {
                    chrs[i].Text = EnemyText;
                }
                else
                {
                    chrs[i].Text = ItemText;
                }
                chrs[i].Font = tempLabel.Font;
                Controls.Add(chrs[i]);
            }
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            nextState = State.Game;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (isDebug)
            {
                if (GetAsyncKeyState((int)Keys.O) < 0)
                {
                    nextState = State.Gameover;
                }
                else if (GetAsyncKeyState((int)Keys.C) < 0)
                {
                    nextState = State.Clear;
                }
            }


            if (nextState != State.None)
            {
                initProc();
            }

            if (currentState == State.Game)
            {
                UpdateGame();
            }
        }

        void initProc()
        {
            currentState = nextState;
            nextState = State.None;



            switch (currentState)
            {
                case State.Title:
                    timeLabel.Visible = true;
                    StartButton.Visible = true;
                    copyrightLabel.Visible = true;
                    hiLabel.Visible = true;
                    gameOverLabel.Visible = false;
                    titleButton.Visible = false;
                    clearLabel.Visible = false;
                    break;

                case State.Game:
                    titleLabel.Visible = false;
                    StartButton.Visible = false;
                    copyrightLabel.Visible = false;
                    hiLabel.Visible = false;

                    for (int i = EnemyIndex; i < ChrMax; i++)
                    {
                        chrs[i].Left = rand.Next(ClientSize.Width - chrs[i].Width);
                        chrs[i].Top = rand.Next(ClientSize.Height - chrs[i].Height);
                        vx[i] = rand.Next(-SpeedMax, SpeedMax + 1);
                        vy[i] = rand.Next(-SpeedMax, SpeedMax + 1);
                    }

                    itemCount = ItemMax;
                    break;

                case State.Gameover:
                    //MessageBox.Show("GameOver");
                    gameOverLabel.Visible = true;
                    titleButton.Visible = true;
                    break;

                case State.Clear:
                    //MessageBox.Show("Clear");
                    clearLabel.Visible = true;
                    titleButton.Visible = true;
                    break;
            }
        }

        void UpdateGame()
        {
            Point mp = PointToClient(MousePosition);

            //TODO:mpがプレイヤーのラベルの中心になるように設定

            chrs[PlayerIndex].Left = mp.X - chrs[PlayerIndex].Width / 2;
            chrs[PlayerIndex].Top = mp.Y - chrs[PlayerIndex].Height / 2;

            for (int i = EnemyIndex; i < ChrMax; i++)
            {
                chrs[i].Left += vx[i];
                chrs[i].Top += vy[i];

                if (chrs[i].Left < 0)
                {
                    vx[i] = Math.Abs(vx[i]);
                }
                if (chrs[i].Top < 0)
                {
                    vy[i] = Math.Abs(vy[i]);
                }
                if (chrs[i].Right > ClientSize.Width)
                {
                    vx[i] = -Math.Abs(vx[i]);
                }
                if (chrs[i].Bottom > ClientSize.Height)
                {
                    vy[i] = -Math.Abs(vy[i]);
                }

                //当たり判定
                if ((mp.X >= chrs[i].Left)
                    && (mp.X < chrs[i].Right)
                    && (mp.Y >= chrs[i].Top)
                    && (mp.Y < chrs[i].Bottom))
                {
                    //MessageBox.Show("重なった!!");
                    if (i < ItemIndex)
                    {
                        nextState = State.Gameover;
                    }
                    else
                    {
                        chrs[i].Visible = false;
                        itemCount--;
                        if(itemCount<=0)
                        {
                            nextState = State.Clear;
                        }
                        leftLabel.Text = "★:" + itemCount;
                    }
                }

            }
        }

            private void titleButton_Click(object sender, EventArgs e)
            {
                nextState = State.Title;
            }
        }
}
