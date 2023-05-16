using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _06_坦克大战_正式
{
    /*
     * GameObject类是一个父类，他的子类分别是Movingting和notmoving，因为他们两个的类所用的代码差不多，所有创造这个类
     * 这个类主要是设定x，y坐标，并且有绘画方法和运行方法可以重写，还包含一个构造函数可以用来存储指定图片的大小
     */
    abstract class GameObject
    {

        public int X { get; set; }
        public int Y { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }//这是图片长度的属性

        protected abstract Image GetImage();//abstract是一个抽象类，他不能有主体，他的主体类只能被子类写

        public virtual void DrawSelf()//重写绘画方法
        {
            Graphics g = GameFramework.g;//Graphice 可以封装一个绘画图片

            g.DrawImage(GetImage(),X,Y);//在这个坐标中画出，调用了这个重写的抽象类，给定了x，y坐标
        }
        public virtual void Update()
        {
            DrawSelf();
        }
        public Rectangle GetRectangle()//这是一个构造函数，用来给定一个图片的x，y坐标和长度高度
        {
            Rectangle rectangle = new Rectangle(X, Y, Width, Height);//将指定的坐标和大小存到rectangle中
            return rectangle;
        }

    }
}
