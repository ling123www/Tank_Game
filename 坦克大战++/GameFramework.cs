using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _06_坦克大战_正式
{
    enum GameState//游戏的2个状态，生成中和结束
    {
        Running,
        GameOver
    }

    class GameFramework
    {
        public static Graphics g;
        private static GameState gameState = GameState.Running;

        public static void Start()
        {
            SoundMananger.InitSound();//音效初始化
            GameObjectManager.Start();
            GameObjectManager.CreateMap();
            GameObjectManager.CreateMyTank();
            SoundMananger.PlayStart();
        }

        public static void Update()
        {
            //FPS
            //GameObjectManager.DrawMap();
            //GameObjectManager.DrawMyTank();
            

            if (gameState == GameState.Running)//游戏运行时
            {
                GameObjectManager.Update();//继续运行
            }else if(gameState == GameState.GameOver)//如果游戏结束
            {
                GameOverUpdate();//结束
            }
        }

        public  static void GameOverUpdate()//结束方法
        {
            Bitmap bmp = Properties.Resources.GameOver;
            bmp.MakeTransparent(Color.Black);//背景变黑
            int x = 450 / 2 - Properties.Resources.GameOver.Width / 2;
            int y = 450/2- Properties.Resources.GameOver.Height / 2;//设置坐标
            g.DrawImage(bmp, x, y);//画出图片
        }

        public static void ChangeToGameOver()
        {
            gameState = GameState.GameOver;
        }

    }
}
