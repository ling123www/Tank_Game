using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using _06_坦克大战_正式.Properties;

namespace _06_坦克大战_正式
{
    class MyTank:Movething
    {

        public bool IsMoving { get; set; }//检测是否移动的布尔变量
        public int HP { get; set; }//血量变量
        
        //
        public MyTank(int x,int y,int speed)//这个构造函数调用时需要给定坐标和我的坦克的移动速度
        {
            IsMoving = false;//默认不移动
            this.X = x;//获得X坐标
            this.Y = y;//获得Y坐标           
            this.Speed = speed;//获得移动速度
            BitmapDown = Resources.MyTankDown;
            BitmapUp = Resources.MyTankUp;
            BitmapRight = Resources.MyTankRight;
            BitmapLeft = Resources.MyTankLeft;//获得上下左右的图片
            this.Dir = Direction.Up;//设定了初始方向是向上的，Direction是一个枚举，里面包含了上下左右的方向变量
            HP = 4;//初始值血量
        }
        /*
         * 这是Update的重写方法，和bullet一样，他们的移动方式是不同的，所以需要重写运行方法，里面进行了移动检测
         * 移动方法。
         */
        public override void Update()
        {
            MoveCheck();//移动检查
            Move();

            base.Update();
        }
        /*
         * MoveCheck分别检测了边界检测和与其他元素有没有发生碰撞检测
         * 其中，边界检测是先取得坦克所在的方位，然后判断y的坐标，如果
         * 超出边界则把不能移动的变量设为false
         *          */
        private void MoveCheck()
        {

          
            if (Dir == Direction.Up)
            {
                if (Y - Speed < 0)
                {
                    IsMoving = false; return;
                }
            }
            else if (Dir == Direction.Down)
            {
                if (Y + Speed + Height > 450)
                {
                    IsMoving = false; return;
                }
            }
            else if (Dir == Direction.Left)
            {
                if (X - Speed < 0)
                {
                    IsMoving = false; return;
                }
            }
            else if (Dir == Direction.Right)
            {
                if (X + Speed + Width > 450)
                {
                    IsMoving = false; return;
                }
            }
         


          /*
           *关于碰撞检测，主要是关于墙体的碰撞，先将我的坦克的长宽高存储到rect中，然后进行switch循环，如果向上
           *则一直向上。
           */

            Rectangle rect = GetRectangle();//调用GetRectangle的构造函数，在gameobject中，将当前的位置传给了rect

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
            //当这个坦克移动时，我们将这个坦克的坐标传递到IsCollidedWall方法中
            //这个方法主要是接受一个当前的坐标图片（rect），然后遍历所有的普通墙，如果和rect这个参数
            //相交，则返回这个值，那么，如果这个值不为空，则代表相撞，相撞那么我们的IsMoving就变为false
            if (GameObjectManager.IsCollidedWall(rect) != null)
            {
                IsMoving = false; return;
            }
            //同上，这里使用了贴墙碰撞检测，同样不能移动
            if (GameObjectManager.IsCollidedSteel(rect) != null)
            {
                IsMoving = false; return;
            }
            //同上，这里使用了boss检测
            if (GameObjectManager.IsCollidedBoss(rect))
            {
                IsMoving = false; return;
            }
        }
        /*
         * 这是坦克的移动方法，当我们的IsMoving标志为false时，代表没有移动，则直接返回
         * 然后调用枚举，如果向上，则向上移动，向下则向下移动。
         */
        private void Move()
        {
            if (IsMoving == false) return;

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
        /*
         * 这是Form中自带的KeyDown结构，他可以传递按下了那个按键，然后我们自己可以编写按下这个按钮后发生什么
         * 如下，检测我们的按键，按下W时就是向上，S向下，依次类推，按下J就是调用Attack方法
         */
        public void KeyDown(KeyEventArgs args)
        {
            switch (args.KeyCode)
            {
                case Keys.W:
                    Dir = Direction.Up;
                    IsMoving = true;
                    break;
                case Keys.S:
                    Dir = Direction.Down;
                    IsMoving = true;
                    break;
                case Keys.A:
                    Dir = Direction.Left;
                    IsMoving = true;
                    break;
                case Keys.D:
                    Dir = Direction.Right;
                    IsMoving = true;
                    break;
                case Keys.J:
                    Attack();
                    break;
            }
        }
        /*
         * Attack方法是为了让子弹攻击，首先，我们的子弹肯定是从坦克里发出的，所以我们先取得了坦克的坐标
         * 然后判断当前的范围，如果向上，则计算坦克那个炮口的坐标并设置为子弹的坐标，然后调用CreatBullet方法
         * 创造出子弹并发射
         */
        private void Attack()
        {
            SoundMananger.PlayFire();//调用声音
            //发射子弹
            int x = this.X;
            int y = this.Y;
            //判断子弹的方位让他移动起来
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

            //GreateBullet方法主要是接受4个参数，然后调用bullet类，传递坐标和速度后将这个子弹传入到bullList列表中管理
            //具体可以看GameObjectManager类的描述
            GameObjectManager.CreateBullet(x,y,Tag.MyTank,Dir);
        }
        //KeyUp是form自带的方法，他可以接受一个键盘输入，检测他是否松开，松开则不移动
        public void KeyUp(KeyEventArgs args)
        {
            switch (args.KeyCode)
            {
                case Keys.W:
                    IsMoving = false;
                    break;
                case Keys.S:
                    IsMoving = false;
                    break;
                case Keys.A:
                    IsMoving = false;
                    break;
                case Keys.D:
                    IsMoving = false;
                    break;
            }
        }
        //TakeDamege方法主要是一个血量检测方法，当我们遭到攻击时，则Hp自减，如果小于等于0
        //则游戏结束
        public void TakeDamage()//遭到了攻击
        {
            HP--;

            if (HP <= 0)//血量没了
            {
                 GameFramework. GameOverUpdate();
                  GameFramework.ChangeToGameOver();
            }
        }
    }
}
