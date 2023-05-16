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
    /*
     * GameObejectManager可以说是项目中最核心的类，它不仅包含了所有移动的物体，也包含了所有不移动的物体，实现了我的坦克创建
     * 敌人坦克创建，很多的类都需要调用他的方法，实现了各个资源的管理和运行
     */
    class GameObjectManager
    {

        private static List<NotMovething> wallList = new List<NotMovething>();//对普通墙体资源做一个列表存储
        private static List<NotMovething> steelList = new List<NotMovething>();//对刚体墙做一个列表存储
        private static NotMovething boss;//引用不能移动的物体中的物体，名为boss（用notmovething声明了一个变量）
        private static MyTank myTank;//引用MyTank类，变量名为mytank

        private static List<EnemyTank> tankList = new List<EnemyTank>();//对敌人坦克做一个列表存储
        private static List<Bullet> bulletList = new List<Bullet>();//对子弹做一个列表存储
        private static Object _bulletLock = new object();//确保对共享资源的独占访问权限

        private static List<Explosion> expList = new List<Explosion>();//对爆炸的资源做一个列表存储

        private static int enemyBornSpeed = 40;//敌人生成速度
        private static int enemyBornCount = 60;//敌人生成计数器

        private static Point[] points = new Point[3];//定义了3个坐标，这些坐标是敌人tank生成的位置

        public static void Start()
        {
            points[0].X = 0; points[0].Y = 0;

            points[1].X = 7 * 30; points[1].Y = 0;

            points[2].X = 14 * 30; points[2].Y = 0;//分别定义敌人坦克生成的坐标

        }


        public static void Update()
        {
            for (int i = 0; i < wallList.Count; i++)
            {
                wallList[i].Update();//这里的Update并不是递归，而是调用了Object的Update画的方法，表示画出所有的列表中的普通墙
            }
            for (int i = 0; i < steelList.Count; i++)
            {
                steelList[i].Update();//画出列表中的铁墙
            }

            for (int i = 0; i < tankList.Count; i++)
            {
                tankList[i].Update();//画出所有的敌军坦克

            }

            CheckAndDestroyBullet();//检测子弹是否被摧毁
            for (int i = 0; i < bulletList.Count; i++)
            {
                bulletList[i].Update();//画出所有的子弹
            }

            for(int i = 0; i < expList.Count; i++)
            {
                expList[i].Update();//画出所有的爆炸
            }
            CheckAndDestroyExplosion();//检测是否发生爆炸和摧毁

            boss.Update();//画出Notmoving 的Update方法，由于NotMoving是继承了object，所以调用object的Update方法

            myTank.Update();//调用了mytank类的重写update方法，进行了移动检测和移动

            EnemyBorn();//敌人生成
        }
        /*
         * ChenckAdnDestroyBullet方法是检测子弹是否被摧毁的方法，先建立一个neddToDestroy列表他的类型是Bullet类型
         * 循环遍历所有的子弹列表，并且判断Isdestroy标志位是否为true，为true则代表破坏了，那么则把这个子弹添加到needToDestroy列表中
         * 在用bullet遍历所有的NeedToDestroy列表，然后移除所有需要清除的子弹
         */
        private static void CheckAndDestroyBullet()
        {
            List<Bullet> needToDestroy = new List<Bullet>();
            foreach(Bullet bullet in bulletList)
            {
                if (bullet.IsDestroy == true)
                {
                    needToDestroy.Add(bullet);
                }
            }
            foreach(Bullet bullet in needToDestroy)
            {
                bulletList.Remove(bullet);
            }
        }
        /*
         * 检测是否发生爆炸和产生爆炸，同上，遍历所有的爆炸列表，将需要销毁的爆炸放在neetodestroy列表中，然后在遍历需要爆炸的列表
         * 将他们移除
         */
        private static void CheckAndDestroyExplosion()
        {
            List<Explosion> needToDestroy = new List<Explosion>();
            foreach (Explosion exp in expList)
            {
                if (exp.IsNeedDestroy == true)
                {
                    needToDestroy.Add(exp);
                }
            }
            foreach (Explosion exp in needToDestroy)
            {
                expList.Remove(exp);
            }
        }
        //产生爆炸方法，传递一个坐标，在坐标位置发生爆炸，并且将这些爆炸添加到explist列表中
        public static void CreateExplosion(int x,int y)
        {
            Explosion exp = new Explosion(x, y);
            expList.Add(exp);
        }
        //同上，创造子弹列表，此方法被mytank调用
        public static void CreateBullet(int x,int y,Tag tag,Direction dir)
        {
            Bullet bullet = new Bullet(x, y, 8, dir, tag);//Bullet类引用时需要传递坐标和速度，有当前方向，和判断那个坦克发出的
            
            bulletList.Add(bullet);//将所有子弹存入到bulletList列表中
            
        }
        //毁掉墙的方法，此方法被bullet类调用，调用时，将给过来的摧毁的墙移除墙列表
        public static void DestroyWall(NotMovething wall)
        {
            wallList.Remove(wall);
        }
        //同上，将传过来的tank，也就是被摧毁的tank移除
        public static void DestroyTank(EnemyTank tank)
        {
            tankList.Remove(tank);
        }
        //敌人生成方法，在
        private static void EnemyBorn()
        {
            enemyBornCount++;//敌人计数器自增
            if (enemyBornCount < enemyBornSpeed) return;//当计数器没有增加到指定值返回

            SoundMananger.PlayAdd();//计数器到，启动坦克创建的声音
            //0-2
            Random rd = new Random();
            int index = rd.Next(0, 3);//在0-3中给定一个随机数，这里是随机在设定的3个位置中生成
            Point position = points[index];//在0-3的随机位置生成
            int enemyType = rd.Next(1, 5);//给定1-5的随机数，这里是随机在设定的4个坦克中生成
            switch( enemyType)//为1生成坦克1在目标位置，为2次生成坦克2.。。。
            {
                case 1:
                    CreateEnemyTank1(position.X, position.Y);
                    break;
                case 2:
                    CreateEnemyTank2(position.X, position.Y);
                    break;
                case 3:
                    CreateEnemyTank3(position.X, position.Y);
                    break;
                case 4:
                    CreateEnemyTank4(position.X, position.Y);
                    break;

            }

            enemyBornCount = 0;//重置计数器
        }

        private static void CreateEnemyTank1(int x,int y)//已经接收到了xy坐标，在生成方法时
        {
            //引用Enemytank传递x，y坐标和速度，并且还有上下左右的图片
            EnemyTank tank = new EnemyTank(x, y, 2, Resources.GrayDown, Resources.GrayUp, Resources.GrayRight, Resources.GrayLeft);
            tankList.Add(tank);//将这些坦克传递到tanklist列表中管理
        }
        private static void CreateEnemyTank2(int x, int y)//同上，只是坦克颜色不同
        {

            EnemyTank tank = new EnemyTank(x, y, 2, Resources.GreenDown, Resources.GreenUp, Resources.GreenRight, Resources.GreenLeft);
            tankList.Add(tank);
        }
        private static void CreateEnemyTank3(int x, int y)//同上，这是快速的坦克
        {

            EnemyTank tank = new EnemyTank(x, y, 4, Resources.QuickDown, Resources.QuickUp, Resources.QuickRight, Resources.QuickLeft);
            tankList.Add(tank);
        }
        private static void CreateEnemyTank4(int x, int y)//同上，这是慢速的坦克
        {

            EnemyTank tank = new EnemyTank(x, y, 1, Resources.SlowDown, Resources.SlowUp, Resources.SlowRight, Resources.SlowLeft);
            tankList.Add(tank);
        }
        /*
         * 以下5个方法均在Bullet中调用，主要是检测2个对应的元素是否相交
         * 具体相交发生什么参见buttle类
         */
        public static NotMovething IsCollidedWall(Rectangle rt)//传递过来的参数是一个子弹图片的长宽高
        {
            foreach (NotMovething wall in wallList)//遍历所有的墙
            {
                if (wall.GetRectangle().IntersectsWith(rt))//如果墙的参数和传递过来的子弹参数相交为true
                {
                    return wall;//返回这个墙，交给Bullet类处理
                }
            }
            return null;
        }
        //与上面方法相同，不过铁墙不会破坏，所以Buttel中的方法中没有移除这个墙
        public static NotMovething IsCollidedSteel(Rectangle rt)
        {
            foreach (NotMovething wall in steelList)
            {
                if (wall.GetRectangle().IntersectsWith(rt))
                {
                    return wall;
                }
            }
            return null;
        }
        //由于boss只有一个，所以不用遍历
        public static bool IsCollidedBoss(Rectangle rt)
        {
            return boss.GetRectangle().IntersectsWith(rt);
        }
        //同boss
        public static MyTank IsCollidedMyTank(Rectangle rt)
        {
            if (myTank.GetRectangle().IntersectsWith(rt)) return myTank;
            else return null;
        }
        //同普通墙
        public static EnemyTank IsCollidedEnemyTank(Rectangle rt)
        {
            foreach(EnemyTank tank in tankList)
            {
                if (tank.GetRectangle().IntersectsWith(rt))
                {
                    return tank;
                }
            }
            return null;
        }

        //创造我的坦克，这里设置了初始坐标，引用了MyTank类，将坐标和速度传了过去，并且存放到了mytank中
        public static void CreateMyTank()
        {
            int x = 5 * 30;
            int y = 14 * 30;

            myTank = new MyTank(x, y, 2);
        }
        //CreateMap方法主要是一个画墙的方法，他调用了CreateWall方法和CreateBoss方法，传递了参数，在那个方法中进行的绘出
        public static void CreateMap()
        {
            CreateWall(1, 1, 5, Resources.wall ,wallList);
            CreateWall(3, 1, 5, Resources.wall, wallList);
            CreateWall(5, 1, 4, Resources.wall, wallList);
            CreateWall(7, 1, 3, Resources.wall, wallList);
            CreateWall(9, 1, 4, Resources.wall, wallList);
            CreateWall(11, 1, 5, Resources.wall, wallList);
            CreateWall(13, 1, 5, Resources.wall, wallList);

            CreateWall(7, 5, 1, Resources.steel, steelList);

            CreateWall(0, 7, 1, Resources.steel, steelList);

            CreateWall(2, 7, 1, Resources.wall, wallList);
            CreateWall(3, 7, 1, Resources.wall, wallList);
            CreateWall(4, 7, 1, Resources.wall, wallList);
            CreateWall(6, 7, 1, Resources.wall, wallList);
            CreateWall(7, 6, 2, Resources.wall, wallList);
            CreateWall(8, 7, 1, Resources.wall, wallList);
            CreateWall(10, 7, 1, Resources.wall, wallList);
            CreateWall(11, 7, 1, Resources.wall, wallList);
            CreateWall(12, 7, 1, Resources.wall, wallList);

            CreateWall(14, 7, 1, Resources.steel, steelList);

            CreateWall(1, 9, 5, Resources.wall, wallList);
            CreateWall(3, 9, 5, Resources.wall, wallList);
            CreateWall(5, 9, 3, Resources.wall, wallList);

            CreateWall(6, 10, 1, Resources.wall, wallList);
            CreateWall(7, 10, 2, Resources.wall, wallList);
            CreateWall(8, 10, 1, Resources.wall, wallList);

            CreateWall(9, 9, 3, Resources.wall, wallList);
            CreateWall(11, 9, 5, Resources.wall, wallList);
            CreateWall(13, 9, 5, Resources.wall, wallList);


            CreateWall(6, 13, 2, Resources.wall, wallList);
            CreateWall(7, 13, 1, Resources.wall, wallList);
            CreateWall(8, 13, 2, Resources.wall, wallList);

            CreateBoss(7, 14, Resources.Boss);
        }

        //接受到坐标和图片，然后将这个坐标引用NotMovething类，将坐标和图片传递过去，并用boss接收
        private static void CreateBoss(int x,int y,Image img)
        {
            int xPosition = x * 30;
            int yPosition = y * 30;
            boss = new NotMovething(xPosition, yPosition, img);
        }
        //接受到坐标和图片，将这个坐标引用NotMovething类,将这些墙放到对应列表中
        private static void CreateWall(int x,int y,int count,Image img, List<NotMovething> wallList)
        {
            int xPosition = x * 30;
            int yPosition = y * 30;
            for(int i = yPosition; i < yPosition + count * 30; i += 15)
            {
                // i xPosition     i xPosition+15
                NotMovething wall1 = new NotMovething(xPosition, i, img);
                NotMovething wall2 = new NotMovething(xPosition+15, i, img);
                wallList.Add(wall1);
                wallList.Add(wall2);
            }
        }

        //按键按下时，调用mytank1的keydown方法，松开时同理
        public static void KeyDown(KeyEventArgs args)
        {
            myTank.KeyDown(args);
        }
        public static void KeyUp(KeyEventArgs args)
        {
            myTank.KeyUp(args);
        }
    }
}
