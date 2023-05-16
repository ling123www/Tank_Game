using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _06_坦克大战_正式
{
    //EnemyTank和MyTank类相似，因为他们有相同的移动，继承了相同的Moving类；不过也有不同，比如EnemyTank是ai控制，我们需要
    //编写一个ai，且子弹发生频率也不同
    class EnemyTank:Movething
    {
        public int ChangeDirSpeed { get; set; }//改变移动属性
        private int changeDirCount = 0;//方向改变计数器
        public int AttackSpeed { get; set; }//攻击速度属性
        private int attackCount = 0;//攻击计数器
        private Random r = new Random();//为了取得随机数的类，取名为r
        //和Mytank类似，得到坐标和图片
        public EnemyTank(int x, int y, int speed,Bitmap bmpDown,Bitmap bmpUp,Bitmap bmpRight,Bitmap bmpLeft)
        {
            this.X = x;
            this.Y = y;
            this.Speed = speed;
            BitmapDown = bmpDown;
            BitmapUp = bmpUp;
            BitmapRight = bmpRight;
            BitmapLeft = bmpLeft;
            this.Dir = Direction.Down;//初始方向为下
            AttackSpeed = 60;//攻击速度，一秒一次
            ChangeDirSpeed = 70;//方向改变速度，大约一秒判断一次
        }

        //
        public override void Update()//重写GameObject中的Update方法，因为每个物体的画法是不同的
        {
            MoveCheck();//移动检查
            Move();//移动
            AttackCheck();//攻击检测
            AutoChangeDirection();//方向判断，这是自动行走的ai

            base.Update();
        }

        private void MoveCheck()//和Mytank相同
        {

 
            if (Dir == Direction.Up)
            {   
                if (Y - Speed < 0)
                {
                    ChangeDirection(); return;
                }
            }
            else if (Dir == Direction.Down)
            {
                if (Y + Speed + Height > 450)
                {
                    ChangeDirection(); return;
                }
            }
            else if (Dir == Direction.Left)
            {
                if (X - Speed < 0)
                {
                    ChangeDirection(); return;
                }
            }
            else if (Dir == Direction.Right)
            {
                if (X + Speed + Width > 450)
                {
                    ChangeDirection(); return;
                }
            }



            //检查有没有和其他元素发生碰撞
            //和Mytank相同

            Rectangle rect = GetRectangle();

            switch (Dir)
            {
                case Direction.Up:
                    rect.Y -= Speed;
                    break;
                case Direction.Down:
                    rect.Y += Speed;
                    break;
                case Direction.Left:
                    rect.X -= Speed;
                    break;
                case Direction.Right:
                    rect.X += Speed;
                    break;
            }

            if (GameObjectManager.IsCollidedWall(rect) != null)
            {
                ChangeDirection(); return;
            }
            if (GameObjectManager.IsCollidedSteel(rect) != null)
            {
                ChangeDirection(); return;
            }
            if (GameObjectManager.IsCollidedBoss(rect))
            {
                ChangeDirection(); return;
            }
            
        }
        
        /*
         * AutoChangeDireaction方法是一个移动改变方法，首先计数加一，如果小于60，返回，也就是一秒后跳出if
         * 然后调用ChangeDirection方法，这个方法主要是随机生成0-4这几个数字，因为枚举和数组一样，都是有下标的
         * 当我们随机的数字和枚举的当前方向相同时，将随机的数字赋值给当前方向，然后再将计数器值0，这样就实现了
         * 一秒判断一次方向
         */
        private void AutoChangeDirection()
        {
            changeDirCount++;
            if (changeDirCount < ChangeDirSpeed) return;
            ChangeDirection();
            changeDirCount = 0;
        }
        private void ChangeDirection()
        {
            //4
            //Random r = new Random(); 种子
            //算法 伪随机数1000  
            while (true)
            {
                Direction dir = (Direction)r.Next(0, 4);
                if (dir == Dir)
                {
                    continue;
                }
                {
                    Dir = dir;break;
                }
            }
            // 0 1 2 3
            MoveCheck();//判断方向后进行移动检测
        }

        //移动方法，和Mytank类似
        private void Move()
        {
            

            switch (Dir)
            {
                case Direction.Up:
                    Y -= Speed;
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
        //攻击方法，和Mytank类似
        private void Attack()
        {
            //发射子弹
            int x = this.X;
            int y = this.Y;

            switch (Dir)
            {
                case Direction.Up:
                    x = x + Width / 2;
                    break;
                case Direction.Down:
                    x = x + Width / 2;
                    y += Height;
                    break;
                case Direction.Left:
                    y = y + Height / 2;
                    break;
                case Direction.Right:
                    x += Width;
                    y = y + Height / 2;
                    break;
            }


            GameObjectManager.CreateBullet(x, y, Tag.EnemyTank, Dir);
        }
          //攻击检测方法，这个方法主要是为了让坦克规定时间内发射，先让计数器一直自增，当达到规定数字后调用Attack方法，然后再将
          //计数器清0，实现了攻击检测
        private void AttackCheck()
        {
            attackCount++;
            if (attackCount < AttackSpeed) return;

            Attack();
            attackCount = 0;
        }

       
    }
}
