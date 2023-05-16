using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _06_坦克大战_正式
{
    enum Direction
    {
        Up=0,
        Down=1,
        Left=2,
        Right=3
    }
    class Movething:GameObject//Moveting类是所有需要移动的物体类的父类，因为所有移动的物体都要重复使用，所以我们写这个类并且继承GameObject类
    {
        private Object _lock = new object();//这是一个为了防止程序奔溃的锁
        public Bitmap BitmapUp { get; set; }
        public Bitmap BitmapDown { get; set; }
        public Bitmap BitmapLeft { get; set; }
        public Bitmap BitmapRight { get; set; }//获得上下左右的属性，x，y，高度和宽度已经在gameobject中继承

        public int Speed { get; set; }//获得速度的属性

        private Direction dir;//定义枚举变量dir
        public Direction Dir { get { return dir; } //定义枚举属性，当我们取得是返回dir变量
            //设定Dir时，我们是把获得的值赋值给dir在返回dir，switch循环分别循环取得不同方向的图片
            set {
                dir = value;
                Bitmap bmp = null;//将当前方向图片清空
                
                switch (dir)
                {
                    case Direction.Up:
                        bmp = BitmapUp;
                        break;
                    case Direction.Down:
                        bmp = BitmapDown;
                        break;
                    case Direction.Left:
                        bmp = BitmapLeft;
                        break;
                    case Direction.Right:
                        bmp = BitmapRight;
                        break;
                }
                lock (_lock)
                {
                    Width = bmp.Width;
                    Height = bmp.Height;//获得图片的长度和宽度
                }
                
            } 
        }

        protected override Image GetImage()//重写这个抽象类
        {
            Bitmap bitmap = null;
            switch (Dir)//检测方向
            {
                case Direction.Up:
                    bitmap = BitmapUp;//向上则返回对应图片
                    break;
                case Direction.Down:
                    bitmap =  BitmapDown;
                    break;
                case Direction.Left:
                    bitmap = BitmapLeft;
                    break;
                case Direction.Right:
                    bitmap = BitmapRight;
                    break;
            }
            bitmap.MakeTransparent(Color.Black);//将图片的黑色背景清空

            return bitmap;
        }

        public override void DrawSelf()
        {
            lock (_lock)
            {
                base.DrawSelf();
            }
        }

    }
}
