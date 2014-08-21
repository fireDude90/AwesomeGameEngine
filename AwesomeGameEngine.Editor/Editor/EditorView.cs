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
        public Point LastMousePosition { get; private set; }
        public Point CurrentMousePosition { get; private set; }

        private IEntity selectedObject;
        public IEntity SelectedObject {
            get { return selectedObject; }
            set {
                foreach (IEntity entity in CurrentScene.Entities) {
                    entity.Selected = false;
                }
                selectedObject = value;
                selectedObject.Selected = true;
            }
        }
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
            SizeChanged += (sender, e) => {
                size = e.NewSize;
                // Start the grid centered
                Position = new Point(size.Width / 2, size.Height / 2);
                InvalidateVisual();
            };

            // This is temporary
            Log = window.Log;

            Project = new Project();
            Scene scene = new Scene("TestScene");
            scene.Add(new Sprite("Test A", new BitmapImage(new Uri("dragon1.png", UriKind.RelativeOrAbsolute))));
            scene.Add(new Sprite("Test B", new BitmapImage(new Uri("dragon1.png", UriKind.RelativeOrAbsolute))));
            Project.Add(scene);
            CurrentScene = scene;
            Log.Text = Project.Serialize().ToString();
        }

        private readonly string[] ImageExtensions = { ".jpeg", ".jpg", ".png", ".gif" };

        protected override void OnDrop(DragEventArgs e) {
            Uri path = (Uri)e.Data.GetData("Uri");
            string extension = System.IO.Path.GetExtension(path.ToString());
            // Check that the file is an image (PNG)
            if (ImageExtensions.Contains(extension.ToLower())) {
                Sprite sprite = new Sprite("New Object", new BitmapImage(new Uri(Uri.UnescapeDataString(path.ToString()), UriKind.RelativeOrAbsolute)));
                sprite.Center(ScreenToWorld(e.GetPosition(this)));

                CurrentScene.Add(sprite);
                InvalidateVisual();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            CurrentMousePosition = e.GetPosition(this);
            if (Mouse.LeftButton == MouseButtonState.Pressed) {
                // Check for click on object
                foreach (IEntity entity in CurrentScene.Entities) {
                    if (entity is IDrawable) {
                        entity.Selected = false;
                        if (!dragging && ((IDrawable)entity).Rectangle.Contains(ScreenToWorld(CurrentMousePosition))) {
                            // entity was selected
                            selectedObject = entity;
                        }
                    }
                }


                // Handle drag
                Mouse.Capture(this);
                if (dragging) {
                    if (selectedObject == null) {
                        Position = new Point(
                            Position.X + CurrentMousePosition.X - LastMousePosition.X,
                            Position.Y + CurrentMousePosition.Y - LastMousePosition.Y);
                    } else {
                        selectedObject.Selected = true;
                        Point delta = (Point)(CurrentMousePosition - LastMousePosition);
                        ((IDrawable)selectedObject).Position = new Point(
                            ((IDrawable)selectedObject).Position.X + delta.X,
                            ((IDrawable)selectedObject).Position.Y + delta.Y
                            );
                    }
                } else {
                    dragging = true;
                }
                InvalidateVisual();
            } else {
                selectedObject = null;
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
