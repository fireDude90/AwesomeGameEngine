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
using AwesomeGameEngine.Editor.Serialization;

namespace AwesomeGameEngine.Editor {
    public class EditorView : FrameworkElement {
        // Grid
        const int GridScrollSpeed = 5;
        const int GridSize = 25;
        const int LineThickness = 1;
        // Grid dragging
        /// <summary>
        /// Current grid offset
        /// </summary>
        public Point Position { get; private set; } // Location of grid
        /// <summary>
        /// Set to true if someone else is dragging.
        /// </summary>
        public bool CanDrag { get; set; }
        public Point LastMousePosition { get; private set; }
        public Point CurrentMousePosition { get; private set; }
        private bool dragging = false;

        // Grid colors
        Brush background = Brushes.LightGray;
        Pen gridLines = new Pen(new SolidColorBrush(Color.FromArgb(150, 255, 255, 255)), LineThickness);

        private Rect Rectangle { get { return new Rect(new Point(), size); } }
        private Size size; // Control size

        public Project Project { get; set; }
        public Scene CurrentScene { get; set; }

        // Temporary
        public TextBox Log { get; private set; } // Reference to display text on screen

        public EditorView(MainWindow window) {
            CanDrag = true;
            SizeChanged += (sender, e) => {
                size = e.NewSize;
                // Start the grid centered
                Position = new Point(size.Width / 2, size.Height / 2);
                InvalidateVisual();
            };

            // This is temporary
            Log = window.Log;
            List<IDrawable> drawables = new List<IDrawable>();


            Project = new Project();
            Scene scene = new Scene("TestScene");
            CurrentScene = scene;
            scene.Add(new Sprite("Test A", new BitmapImage(new Uri("dragon1.png", UriKind.RelativeOrAbsolute)), this));
            scene.Add(new Sprite("Test B", new BitmapImage(new Uri("dragon1.png", UriKind.RelativeOrAbsolute)), this));
            Project.Add(scene);
            Log.Text = Project.Serialize().ToString();
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            CurrentMousePosition = e.GetPosition(this);
            if (Mouse.LeftButton == MouseButtonState.Pressed) {
                // Check for click on object
                foreach (IEntity entity in CurrentScene.Entities) {
                    if (entity is IDrawable) {
                        if (((IDrawable)entity).Rectangle.Contains(CurrentMousePosition)) {

                        }
                    }
                }

                // Handle drag
                Mouse.Capture(this);
                if (dragging && CanDrag) {
                    Position = new Point(
                        Position.X + CurrentMousePosition.X - LastMousePosition.X,
                        Position.Y + CurrentMousePosition.Y - LastMousePosition.Y);
                } else {
                    dragging = true;
                }
                InvalidateVisual();
            } else {
                dragging = false;
                Mouse.Capture(null);
            }
            LastMousePosition = CurrentMousePosition;
        }

        protected override void OnRender(DrawingContext context) {
            base.OnRender(context);
            context.DrawRectangle(background, null, new Rect(new Point(), size));

            // Draw Grid
            int x = -(int)Position.X + ((int)Position.X % GridSize);
            while (x < -Position.X + size.Width + GridSize) {
                context.DrawLine(gridLines,
                    new Point(x + Position.X, 0), new Point(x + Position.X, size.Height));
                x += GridSize;
            }

            int y = -(int)Position.Y + ((int)Position.Y % GridSize);
            while (y < -Position.Y + size.Height + GridSize) {
                context.DrawLine(gridLines,
                   new Point(0, y + Position.Y), new Point(size.Width, y + Position.Y));
                y += GridSize;
            }

            context.DrawEllipse(null, new Pen(Brushes.White, 2), WorldToScreen(new Point(0, 0)), 5, 5);

            // Switch to world coordinates and draw scene
            context.PushTransform(new TranslateTransform(Position.X, Position.Y));
            CurrentScene.Draw(context);
            context.Pop();
        }

        public Point ScreenToWorld(Point source) {
            return new Point(source.X - Position.X, source.Y - Position.Y);
        }

        public Point WorldToScreen(Point source) {
            return new Point(source.X + Position.X, source.Y + Position.Y);
        }
    }
}
