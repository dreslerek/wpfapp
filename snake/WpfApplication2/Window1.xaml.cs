using System;
using System.Timers;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApplication1
{
    using System.Windows.Threading;

    public partial class Window1 : Window
    {
        // This list describes the Bonus Red pieces of Food on the Canvas
        private List<Point> bonusPoints = new List<Point>();

        // This list describes the body of the snake on the Canvas
        private List<Point> snakePoints = new List<Point>();


        
        private Brush snakeColor = Brushes.Blue;
        private enum SIZE
        {
            THIN = 7,
            NORMAL = 4,
            THICK = 3
        };
        private enum MOVINGDIRECTION
        {
            UPWARDS = 6,
            DOWNWARDS = 2,
            TOLEFT = 8,
            TORIGHT = 9
        };
        private Point startingPoint = new Point(50, 70);
        private Point currentPosition = new Point();

        // Movement direction initialisation
        private int direction = 0;

        /* Placeholder for the previous movement direction
         * The snake needs this to avoid its own body.  */
        private int previousDirection = 0;

        /* Here user can change the size of the snake. 
         * Possible sizes are THIN, NORMAL and THICK */
        private int headSize = (int)SIZE.THICK; 

        private long z = 20000;

        private int length = 100;
        private int score = 0;
        private Random rnd = new Random();
        DispatcherTimer timer = new DispatcherTimer();
        


        public Window1()
        {
            InitializeComponent();
            timer.Tick += new EventHandler(timer_Tick);

            /* Here user can change the speed of the snake. 
             * Possible speeds are FAST, MODERATE, SLOW and DAMNSLOW */
            timer.Interval = new TimeSpan(z);
            timer.Start();


            this.KeyDown += new KeyEventHandler(OnButtonKeyDown);
            paintSnake(startingPoint);
            currentPosition = startingPoint;

            // Tworzenie Food Objects
            for (int n = 0; n < 1; n++)
            {
                paintBonus(n);
            }
        }

        
       
        private void paintSnake(Point currentposition)
        {

            /* This method is used to paint a frame of the snake´s body
             * each time it is called. */


                Ellipse newEllipse = new Ellipse();
                newEllipse.Fill = snakeColor;
                newEllipse.Width = headSize;
                newEllipse.Height = headSize;

                Canvas.SetTop(newEllipse, currentposition.Y);
                Canvas.SetLeft(newEllipse, currentposition.X);

                int count = paintCanvas.Children.Count;
                paintCanvas.Children.Add(newEllipse);
                snakePoints.Add(currentposition);


                // Zwężanie ogona węża
                if (count > length)
                {
                    paintCanvas.Children.RemoveAt(count - length);
                    snakePoints.RemoveAt(count - length);
                }
        }


        private void paintBonus(int index)
        {
            Point bonusPoint = new Point(rnd.Next(5, 620), rnd.Next(5, 380));

                

                Ellipse newEllipse = new Ellipse();
                newEllipse.Fill = Brushes.Red;
                newEllipse.Width = headSize;
                newEllipse.Height = headSize;

                Canvas.SetTop(newEllipse, bonusPoint.Y);
                Canvas.SetLeft(newEllipse, bonusPoint.X);
                paintCanvas.Children.Insert(index, newEllipse);
                bonusPoints.Insert(index, bonusPoint);
            
        }


        private void timer_Tick(object sender, EventArgs e)
        {
            // Poszerzanie ciała węża w kierunku ruchu

                switch (direction)
                {
                    case (int)MOVINGDIRECTION.DOWNWARDS:
                        currentPosition.Y += 1;
                        paintSnake(currentPosition);
                        break;
                    case (int)MOVINGDIRECTION.UPWARDS:
                        currentPosition.Y -= 1;
                        paintSnake(currentPosition);
                        break;
                    case (int)MOVINGDIRECTION.TOLEFT:
                        currentPosition.X -= 1;
                        paintSnake(currentPosition);
                        break;
                    case (int)MOVINGDIRECTION.TORIGHT:
                        currentPosition.X += 1;
                        paintSnake(currentPosition);
                        break;
                }

                // Restrict to boundaries of the Canvas
                if ((currentPosition.X < 5) || (currentPosition.X > 620) || 
                    (currentPosition.Y < 5) || (currentPosition.Y > 380))
                    GameOver();

                // Trafienie w punkt bonusowy powoduje wydłużanie węża
                int n = 0;
                foreach (Point point in bonusPoints)
                {

                    if ((Math.Abs(point.X - currentPosition.X) < headSize) && 
                        (Math.Abs(point.Y - currentPosition.Y) < headSize))
                    {
                        length += 10;
                        score += 10;

                        // In the case of food consumption, erase the food object
                        // from the list of bonuses as well as from the canvas
                        bonusPoints.RemoveAt(n);
                        paintCanvas.Children.RemoveAt(n);
                        paintBonus(n);
                    if (z>1000)
                    z = z-500;
                    timer.Interval = new TimeSpan(z);
                    break;
                    }
                    n++;
                }

                // Ograniczenie trafień ciała węża


                for (int q = 0; q < (snakePoints.Count - headSize*2); q++)
                {
                    Point point = new Point(snakePoints[q].X, snakePoints[q].Y);
                    if ((Math.Abs(point.X - currentPosition.X) < (headSize)) &&
                         (Math.Abs(point.Y - currentPosition.Y) < (headSize)) )
                    {
                        GameOver();
                        break;
                    }

                }




        }
        
        
        
        private void OnButtonKeyDown(object sender, KeyEventArgs e)
        {

            
            
            switch (e.Key)
            {
                case Key.Down:
                    if (previousDirection != (int)MOVINGDIRECTION.UPWARDS)
                        direction = (int)MOVINGDIRECTION.DOWNWARDS;
                    break;
                case Key.Up:
                    if (previousDirection != (int)MOVINGDIRECTION.DOWNWARDS)
                        direction = (int)MOVINGDIRECTION.UPWARDS;
                    break;
                case Key.Left:
                    if (previousDirection != (int)MOVINGDIRECTION.TORIGHT)
                        direction = (int)MOVINGDIRECTION.TOLEFT;
                    break;
                case Key.Right:
                    if (previousDirection != (int)MOVINGDIRECTION.TOLEFT)
                        direction = (int)MOVINGDIRECTION.TORIGHT;
                    break;

            }
            previousDirection = direction;
            
        }



        private void GameOver()
        {
            MessageBox.Show("You Lose! Your score is "+ score.ToString(), "Game Over", MessageBoxButton.OK, MessageBoxImage.Hand);
            this.Activate();
        }
    }
}

