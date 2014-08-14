using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AwesomeGameEngineEditor.Editor {
    public class EditorView : FrameworkElement {
        const int GridScrollSpeed = 5;
        private int GridSize = 25;
        const int LineThickness = 1;

        public Point position; // Location of grid
        Size size;
        Point lastMousePosition;
        bool dragging = false;

        Brush background = Brushes.LightGray;
        Pen gridLines = new Pen(Brushes.White, LineThickness);

        TextBlock log; // Reference to display text on screen

        List<IDrawable> drawables = new List<IDrawable>();

        public EditorView(MainWindow window) {
            SizeChanged += (sender, e) => {
                size = e.NewSize;
                position = new Point(size.Width / 2, size.Height / 2);
                InvalidateVisual();
            };

            // This is temporary
            log = window.Log;
            drawables.Add(new Sprite(new BitmapImage(new Uri("dragon1.png", UriKind.RelativeOrAbsolute))));
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            if (Mouse.LeftButton == MouseButtonState.Pressed) {
                if (dragging) {
                    Point newMousePosition = e.GetPosition(this);
                    position = new Point(
                        position.X + newMousePosition.X - lastMousePosition.X,
                        position.Y + newMousePosition.Y - lastMousePosition.Y);
                    lastMousePosition = newMousePosition;
                } else {
                    dragging = true;
                    Mouse.Capture(this);
                    lastMousePosition = e.GetPosition(this);
                }
                InvalidateVisual();
            } else {
                dragging = false;
                Mouse.Capture(null);
            }

            if (Mouse.RightButton == MouseButtonState.Pressed) {
                log.Text = ScreenToWorld(e.GetPosition(this)).ToString();
                log.Text += "\n" + position.ToString();
            }
        }

        protected override void OnRender(DrawingContext context) {
            base.OnRender(context);
            context.DrawRectangle(background, null, new Rect(new Point(), size));

            // Draw lines until they're not visible
            int x = -(int)position.X + ((int)position.X % GridSize);
            while (x < size.Width + GridSize) {
                context.DrawLine(gridLines,
                    new Point(x + position.X, 0), new Point(x + position.X, size.Height));
                x += GridSize;
            }

            int y = -(int)position.Y + ((int)position.Y % GridSize);
            while (y < position.Y + size.Height + GridSize) {
                context.DrawLine(gridLines,
                   new Point(0, y + position.Y), new Point(size.Width, y + position.Y));
                y += GridSize;
            }

            // Draw a dot at (0, 0)
            context.DrawEllipse(null, new Pen(Brushes.White, 2), WorldToScreen(new Point(0, 0)), 5, 5);

            // switch over to world coordinates
            context.PushTransform(new TranslateTransform(position.X, position.Y));
            // Test draw an image
            foreach (IDrawable drawable in drawables) {
                drawable.Draw(context);
            }
            context.Pop();
        }

        private Point ScreenToWorld(Point source) {
            return new Point(source.X - position.X, source.Y - position.Y);
        }

        private Point WorldToScreen(Point source) {
            return new Point(source.X + position.X, source.Y + position.Y);
        }
    }
}
