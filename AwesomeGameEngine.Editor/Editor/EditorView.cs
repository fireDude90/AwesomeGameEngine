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
        #region Grid
        const int LineThickness = 1;
        const int DefaultGridSize = 25;
        private double GridSize = 25;
        private double scale = 1;

        // Grid dragging
        /// <summary>
        /// Current grid offset
        /// </summary>
        public Point Position { get; private set; } // Location of grid
        public Point LastMousePosition { get; private set; }
        public Point CurrentMousePosition { get; private set; }

        // Grid colors
        Brush background = Brushes.LightGray;
        Pen gridLines = new Pen(new SolidColorBrush(Color.FromArgb(150, 255, 255, 255)), LineThickness);
        #endregion

        #region Selecting Objects
        private IEntity selectedObject;
        public IEntity SelectedObject {
            get { return selectedObject; }
            set {
                foreach (IEntity entity in Project.CurrentScene.Entities) {
                    entity.Selected = false;
                }
                selectedObject = value;
                selectedObject.Selected = true;
            }
        }
        private bool dragging = false;
        #endregion

        private Rect Rectangle { get { return new Rect(new Point(), size); } }
        private Size size; // Control size

        #region Project System
        public Project Project { get; set; }
        #endregion

        public EditorView() {
            SizeChanged += (sender, e) => {
                size = e.NewSize;
                // Start the grid centered
                Position = new Point(size.Width / 2, size.Height / 2);
                InvalidateVisual();
            };  

            // Initialize default scene
            Project = new Project();
            var scene = new Scene("DefaultScene") { IsDefaultScene = true };
            Project.Add(scene); // add curent scene to project
        }

        public void ResetProject() {
            scale = 1;
            GridSize = DefaultGridSize;

            Project = new Project();
            var scene = new Scene("Default Scene") { IsDefaultScene = true };
            Project.Add(scene);
        }

        private readonly string[] ImageExtensions = { ".jpeg", ".jpg", ".png", ".gif" };

        protected override void OnDrop(DragEventArgs e) {
            var path = (Uri)e.Data.GetData("Uri");
            string extension = System.IO.Path.GetExtension(path.ToString());
            // Check that the file is an image (PNG)
            if (ImageExtensions.Contains(extension.ToLower())) {
                var sprite = new Sprite("New Object " + Counters.NewGameObject, new BitmapImage(new Uri(Uri.UnescapeDataString(path.ToString()), UriKind.RelativeOrAbsolute)));
                sprite.Center(ScreenToWorld(e.GetPosition(this)));

                Project.CurrentScene.Add(sprite);
                InvalidateVisual();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            CurrentMousePosition = e.GetPosition(this);
            if (Mouse.LeftButton == MouseButtonState.Pressed) {
                // Check for click on object
                foreach (var entity in Project.CurrentScene.Entities) {
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
                        var delta = (Point)(CurrentMousePosition - LastMousePosition);
                        ((IDrawable)selectedObject).Position = new Point(
                            ((IDrawable)selectedObject).Position.X + (delta.X / scale),
                            ((IDrawable)selectedObject).Position.Y + (delta.Y / scale));
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

        protected override void OnMouseWheel(MouseWheelEventArgs e) {
            double amount = e.Delta / 1000d;
            var newScale = scale + amount;
            var newGridSize = DefaultGridSize * scale;
            if (newGridSize < 3 && e.Delta < 0) return;
            scale = newScale;
            GridSize = newGridSize;
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext context) {
            base.OnRender(context);
            context.DrawRectangle(background, null, new Rect(new Point(), size));

            // Draw Grid
            double x = -(int)Position.X + ((int)Position.X % GridSize);
            while (x < -Position.X + size.Width + GridSize) {
                context.DrawLine(gridLines,
                    new Point(x + Position.X, 0), new Point(x + Position.X, size.Height));
                x += GridSize;
            }

            double y = -(int)Position.Y + ((int)Position.Y % GridSize);
            while (y < -Position.Y + size.Height + GridSize) {
                context.DrawLine(gridLines,
                   new Point(0, y + Position.Y), new Point(size.Width, y + Position.Y));
                y += GridSize;
            }

            // Apply scale
            context.PushTransform(new ScaleTransform(scale, scale, Position.X, Position.Y));
            // Switch to world coordinates and draw scene
            context.PushTransform(new TranslateTransform(Position.X, Position.Y));
            // Draw center marker
            context.DrawEllipse(null, new Pen(Brushes.White, 2 * (1 / scale)), new Point(0, 0), 5, 5);
            Project.CurrentScene.Draw(context, scale);
            context.Pop();
            context.Pop();
        }

        /// <summary>
        /// Converts a point from screen space to world coordinates
        /// </summary>
        /// <param name="source">Point to convert</param>
        /// <returns>A new point which has accounted for grid scrolling and scaling</returns>
        public Point ScreenToWorld(Point source) {
            return new Point((source.X - Position.X) / scale, (source.Y - Position.Y) / scale);
        }

        /// <summary>
        /// Converts a point from world space to screen space
        /// </summary>
        /// <param name="source">Point on screen to be converted</param>
        /// <returns>A point in world space which accounts for grid scrolling and scaling</returns>
        public Point WorldToScreen(Point source) {
            return new Point((source.X + Position.X) * scale, (source.Y + Position.Y) * scale);
        }
    }
}
