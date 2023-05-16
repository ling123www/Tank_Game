using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _06_坦克大战_正式
{
    //这个类是winform的窗口，用来设置窗口的所有属性和事件
    public partial class Form1 : Form
    {
        //由于背景和主线程要同时进行也就是GanmeFramwork类也要同时进行，所以使用线程
        private Thread t;//定义一个线程，命名为t
        private static Graphics windowG;

        private static Bitmap tempBmp;

        public Form1()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;

            //阻塞

            windowG = this.CreateGraphics();//返回Graphics


            tempBmp = new Bitmap(450, 450);//定义地图大小
            Graphics bmpG = Graphics.FromImage(tempBmp);//创建这个地图
            GameFramework.g = bmpG;

            t = new Thread(new ThreadStart(GameMainThread));
            t.Start();
        }

        private static void GameMainThread()
        {

            //GameFramework

            GameFramework.Start();//次线程运行，也就是我们的背景

            int sleepTime = 1000 / 60;//这里是我们的刷新率

            //60
            while (true)
            {
                GameFramework.g.Clear(Color.Black);//循环刷新黑背景

                GameFramework.Update();// 调用游戏是否运行方法

                windowG.DrawImage(tempBmp, 0, 0);//初始坐标

                Thread.Sleep(sleepTime);//执行刷新率
            }

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)//当程序关闭时，关闭线程
        {
            t.Abort();
        }

        //事件  消息  事件消息
        private void Form1_KeyDown(object sender, KeyEventArgs e)//当对应按键按下时，调用GameoObjectMananger类的KeyDown方法
        {
            GameObjectManager.KeyDown(e);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)//松开同理
        {
            GameObjectManager.KeyUp(e);
        }
    }
}
