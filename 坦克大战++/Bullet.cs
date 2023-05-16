    using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _06_坦克大战_正式.Properties;

namespace _06_坦克大战_正式   
{
    enum Tag//我的坦克和敌人坦克子弹是不同的
    {
        MyTank,
        EnemyTank
    }
    class Bullet: Movething//子弹
    {
        public Tag Tag { get; set; }//属性，通过返回的tag来检查使用枚举的那个参数

        public bool IsDestroy { get; set; }//属性，通过返回的IsDestory来判断是否销毁,在下方有设置

        public Bullet(int x, int y, int speed,Direction dir,Tag tag)//子弹构造函数
        {
            IsDestroy = false;//默认没被销毁
            this.X = x;
            this.Y = y;
            this.Speed = speed;//得到坐标和速度
            BitmapDown = Resources.BulletDown;
            BitmapUp = Resources.BulletUp;
            BitmapRight = Resources.BulletRight;
            BitmapLeft = Resources.BulletLeft;//得到子弹左右上下的图片
            this.Dir = dir;//得到当前什么方向
            this.Tag = tag;//得到是我的坦克还是敌人的坦克

            this.X -= Width / 2;
            this.Y -= Height / 2;//得到中心位置
        }

        public override void DrawSelf()//画出自己，这是一个重写绘画的方法，定义这个类时就会被执行，关于重写是因为画动和不动的画是不一样的
        {
            base.DrawSelf();//重写的调用语法
        }

        public override void Update()//这是一个运行类，里面包括了子弹的移动和移动检测
        {
            MoveCheck();//移动检查
            Move();

            base.Update();//重写的调用语法
        }

        private void MoveCheck()//构造函数，用来进行移动判断，当用这个类定义时就会被调用（关于是否超过边界和关于其他元素的碰撞）
        {
            //关于边界检测
             
            if (Dir == Direction.Up)//当距离在上方时
            {
                if (Y +Height/2+3 < 0)//如果距离超过了y的最大范围
                {
                    IsDestroy = true ; return;//销毁属性变为true
                }
            }
            else if (Dir == Direction.Down)
            {
                if (Y + Height / 2 -3 > 450)
                {
                    IsDestroy=true; return;
                }
            }
            else if (Dir == Direction.Left)
            {
                if (X +Width/2-3 < 0)
                {
                    IsDestroy = true; return;
                }
            }
            else if (Dir == Direction.Right)//以上类似
            {
                if (X+Width/2+3 > 450)
                {
                    IsDestroy = true; return;
                }
            }



            //检查有没有和其他元素发生碰撞

            /*
             * Rectangle是一个自带类，用来存储一个矩形的大小
             * GetRectangle函数是一个在GameObject类的构造函数，他主要用于存储了一个图片的x，y坐标和高度长度
             */
            Rectangle rect = GetRectangle();//这里只是调用了这个方法来把子弹的长宽高设置进去

            rect.X = X + Width / 2 - 3;
            rect.Y = Y + Height / 2 - 3;
            rect.Height = 3;
            rect.Width = 3;//设置了子弹碰撞时的长宽高和坐标

            //1、墙 2、钢墙 3、坦克
            int xExplosion = this.X + Width / 2;//这里的X已经就设为了子弹碰撞的X坐标
            int yExplosion = this.Y + Height / 2;//y类似

            NotMovething wall = null;//调用NotMovthing的wall参数，把他的初始值设为空

            /*
             * 以下代码类似，先调用了GameObjectManager的IsCollidedWall方法，这个方法主要是
             * 用来检测传递过来的rect参数是否和墙碰撞，如果碰撞，则返回true，这样，if的判断
             * 就为true，那么就会执行if下面的代码
             */
            if ( (wall=GameObjectManager.IsCollidedWall(rect)) != null)
            {
                IsDestroy=true;//将标志位置为true
                GameObjectManager.DestroyWall(wall);//调用DestroyWall方法，这个方法是用来毁掉墙
                GameObjectManager.CreateExplosion(xExplosion, yExplosion);//调用CreateExplosion方法，这个方法是接受一个x，y的坐标
                                                                          //并在这个坐标处实现爆炸的操作。
                SoundMananger.PlayBlast();//这是一个声音方法，表示墙爆炸后发出的声音
                return;//回去继续检测
            }
            if (GameObjectManager.IsCollidedSteel(rect) != null)//这是一个铁墙，子弹与他相撞后只会发生爆炸和子弹消失
            {
                GameObjectManager.CreateExplosion(xExplosion, yExplosion);//在这个坐标实现爆炸
                IsDestroy = true;//消失的标志位为true，子弹消失
                return;
            }
            if (GameObjectManager.IsCollidedBoss(rect))//子弹与boss发生碰撞
            {
                SoundMananger.PlayBlast();//这是一个声音
                GameFramework.ChangeToGameOver(); //结束游戏的方法
                return;
            }

            /*
             * 以下代码类似，Tag表示在Bullet中的枚举标志位，定义了mytank和enemytank，不同的坦克在收到子弹攻击时发生的效果是不同的
             * if中判断Tag枚举是否为mytank或enemytank，如果是mytank（代指我的坦克发射的子弹，不是代表我的坦克被击中了）
             * 则先将EnemyTank坦克类中的tank设为null，并且调用IsCollidedEnemyTank方法
             * 这个方法的主要内容是，接受一个参数，并且遍历自己的所有tank列表，如果和他相交，返回这个tank数据
             * 返回的数据如果赋值给我们这个if里的tank不是空的画，那么代表碰撞了，接下来执行if中代码 
             */
            if (Tag == Tag.MyTank)
            {
                EnemyTank tank = null;
                if ( (tank = GameObjectManager.IsCollidedEnemyTank(rect)) != null)
                {
                    IsDestroy = true;//子弹消失
                    GameObjectManager.DestroyTank(tank);//调用清除方法，清除这个坦克（被击中的敌军坦克）
                    GameObjectManager.CreateExplosion(xExplosion, yExplosion);//在这个坐标下发生爆炸特效
                    SoundMananger.PlayHit();//这是一个tank击中的音效
                    return;
                }
            }else if(Tag== Tag.EnemyTank)//如果是敌人发射的子弹
            {
                MyTank mytank = null;
                if( (mytank = GameObjectManager.IsCollidedMyTank(rect)) != null)//不是空，发生碰撞
                {
                    IsDestroy = true;//子弹销毁
                    GameObjectManager.CreateExplosion(xExplosion, yExplosion);//爆炸效果
                    SoundMananger.PlayBlast();//声音特效
                    mytank.TakeDamage();//TakeDanmege方法是用来检测我们的血量是否低于4，如果低于4则游戏失败

                    return;
                }
            } 
        }

        

        private void Move()//子弹移动方法
        {


            switch (Dir)//检测当前状态的方位
            {
                case Direction.Up://当前方位向上则一直向上移动,以下类似
                    Y -= Speed;//Y=Y-speed一直循环，这样就会随着速度的改变，改变y的坐标
                    break;
                case Direction.Down:
                    Y += Speed;
                    break;
                case Direction.Left:
                    X -= Speed;
                    break;
                case Direction.Right:
                    X += Speed;
                    break;
            }
        }
    }
}
