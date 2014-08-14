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
        const int GridSize = 25;
        const int LineThickness = 1;

        public Point Position { get; private set; } // Location of grid

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
                Position = new Point(size.Width / 2, size.Height / 2);
                InvalidateVisual();
            };

            // This is temporary
            log = window.Log;
            drawables.Add(new Sprite(new BitmapImage(new Uri("dragon1.png", UriKind.RelativeOrAbsolute))));
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            if (Mouse.LeftButton == MouseButtonState.Pressed) {
                Point newMousePosition = e.GetPosition(this);
                
                // Check if the user is hovering over an object
                foreach (IDrawable drawable in drawables) {
                    if (drawable.IsClicked(ScreenToWorld(newMousePosition))) {
                    }
                }

                // Handle drag
                if (dragging) {
                    Position = new Point(
                        Position.X + newMousePosition.X - lastMousePosition.X,
                        Position.Y + newMousePosition.Y - lastMousePosition.Y);
                } else {
                    dragging = true;
                    Mouse.Capture(this);
                }
                lastMousePosition = newMousePosition;
                InvalidateVisual();
            } else {
                dragging = false;
                Mouse.Capture(null);
            }

            if (Mouse.RightButton == MouseButtonState.Pressed) {
                log.Text = ScreenToWorld(e.GetPosition(this)).ToString();
                log.Text += "\n" + Position.ToString();
            }
        }

        protected override void OnRender(DrawingContext context) {
            base.OnRender(context);
            context.DrawRectangle(background, null, new Rect(new Point(), size));

            // Draw lines until they're not visible
            int x = -(int)Position.X + ((int)Position.X % GridSize);
            while (x < size.Width + GridSize) {
                context.DrawLine(gridLines,
                    new Point(x + Position.X, 0), new Point(x + Position.X, size.Height));
                x += GridSize;
            }

            int y = -(int)Position.Y + ((int)Position.Y % GridSize);
            while (y < Position.Y + size.Height + GridSize) {
                context.DrawLine(gridLines,
                   new Point(0, y + Position.Y), new Point(size.Width, y + Position.Y));
                y += GridSize;
            }

            // Draw a dot at (0, 0)
            context.DrawEllipse(null, new Pen(Brushes.White, 2), WorldToScreen(new Point(0, 0)), 5, 5);

            // switch over to world coordinates
            context.PushTransform(new TranslateTransform(Position.X, Position.Y));
            // Test draw an image
            foreach (IDrawable drawable in drawables) {
                drawable.Draw(context);
            }
            context.Pop();
        }

        private Point ScreenToWorld(Point source) {
            return new Point(source.X - Position.X, source.Y - Position.Y);
        }

        private Point WorldToScreen(Point source) {
            return new Point(source.X + Position.X, source.Y + Position.Y);
        }
    }
}
