using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceGame
{
    public partial class Form1 : Form
    {
        //Настройки игры
        private int playerX, playerY;
        private const int playerSpeed = 7;
        private int backgroundY = 0;
        private int bulletSpeed = 10;
        private int destroyAstr = 0;
        private int shootCooldown = 0;
        private Random rand = new Random();

        //Список астероидов
        private List<Point> asteroids = new List<Point>();
        private List<int> astrSpeed = new List<int>();
        //Cписок снарядов
        private List<Point> bullets = new List<Point>();

        //Проверка нажатия клавиш
        private bool PressLeft = false;
        private bool PressRight = false;
        private bool PressSpace = false;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.WindowState = FormWindowState.Normal;

            //Стартовая позиция игрока
            playerX = this.Width / 2;
            playerY = this.Height - 150;

            //Запуск таймера
            gameTimer.Interval = 30;
            gameTimer.Start();
        }
        
        //Нажатие клавиш
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) PressLeft = true;
            if (e.KeyCode == Keys.Right) PressRight = true;
            if (e.KeyCode == Keys.Space) PressSpace = true;
        }
        //Отпускание клавиш
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) PressLeft = false;
            if (e.KeyCode == Keys.Right) PressRight = false;
            if (e.KeyCode == Keys.Space) PressSpace = false;
        }

        //Сам игровой цикл
        private void gameTimer_Tick(object sender, EventArgs e)
        {
            //Движение фона
            backgroundY += 5;
            if (backgroundY >= this.Height) backgroundY = 0;

            //Движение игрока
            if (PressLeft && playerX > 0)
            {
                playerX -= playerSpeed;
            }
            if (PressRight && playerX < this.Width - 80)
            {
                playerX += playerSpeed;
            }

            //Спавн астероидов
            if (rand.Next(100) < 2)
            {
                asteroids.Add(new Point(rand.Next(0, this.Width - 80), -100));
                astrSpeed.Add(rand.Next(5, 11));
            }

            //Спавн снарядов
            if (PressSpace && shootCooldown <= 0)
            {
                bullets.Add(new Point(playerX + 30, playerY));
                shootCooldown = 15;
            }
            if (shootCooldown > 0)
            {
                shootCooldown--;
            }

            //Движение астроидов
            for (int i = asteroids.Count - 1; i >= 0; i--)
            {
                Point ast = asteroids[i];
                asteroids[i] = new Point(ast.X, ast.Y + astrSpeed[i]);
                if (ast.Y > this.Height)
                {
                    asteroids.RemoveAt(i);
                    astrSpeed.RemoveAt(i);
                    continue;
                }

                //Обработка столкновений
                Rectangle playerRect = new Rectangle(playerX, playerY, 80, 80);
                Rectangle astRect = new Rectangle(ast.X, ast.Y, 80, 80);
                if (playerRect.IntersectsWith(astRect))
                {
                    gameTimer.Stop();
                    MessageBox.Show($"Игра окончена!\nСбито астероидов: {destroyAstr}");
                    Application.Exit();
                }
            }

            //Движение снарядов
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                Point b = bullets[i];
                bullets[i] = new Point(b.X, b.Y - bulletSpeed);
                if (b.Y < 0)
                {
                    bullets.RemoveAt(i);
                    continue;
                }

                //Обработка столкновений с астероидами
                Rectangle bulletRect = new Rectangle(b.X, b.Y, 10, 10);
                for (int j = asteroids.Count - 1; j >= 0; j--)
                {
                    Rectangle astRect = new Rectangle(asteroids[j].X, asteroids[j].Y, 80, 80);
                    if (bulletRect.IntersectsWith(astRect))
                    {
                        bullets.RemoveAt(i);
                        asteroids.RemoveAt(j);
                        astrSpeed.RemoveAt(j);
                        destroyAstr++;
                        break;
                    }
                }
            }
            //Перерисовываем форму
            this.Invalidate();
        }

        //Отрисовка игры
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            //Рисуем фон
            g.DrawImage(Properties.Resources.Space, 0, backgroundY, this.Width, this.Height);
            g.DrawImage(Properties.Resources.Space, 0, backgroundY - this.Height, this.Width, this.Height);
            
            //Рисуем игрока
            g.DrawImage(Properties.Resources.Spaceship, playerX, playerY, 80, 80);

            //Рисуем астероиды
            foreach (Point ast in asteroids)
            {
                g.DrawImage(Properties.Resources.Asteroid, ast.X, ast.Y, 80, 80);
            }

            //Рисуем снаряды
            foreach(Point b in bullets)
            {
                g.DrawImage(Properties.Resources.Bullet, b.X, b.Y, 10, 20);
            }
        }
    }
}
