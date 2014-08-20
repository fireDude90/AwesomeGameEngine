using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace AwesomeGameEngine.Editor {
    class Sprite : IDrawable {
        public string Name { get; set; }
        public BitmapImage Image { get; protected set; }
        public Point Position { get; set; }
        public Rect Rectangle {
            get {
                return new Rect(Position, new Size(Image.Width, Image.Height));
            }
        }

        private bool clicked;
        private IDrawable selectedObject;
        private EditorView editor;

        public XElement Serialize() {
            XElement element = new XElement("Sprite");
            element.SetAttributeValue("Name", Name);

            XElement image = new XElement("Image");
            image.SetAttributeValue("Source", Image.UriSource.ToString());
            element.Add(image);

            XElement position = new XElement("Position");
            position.SetAttributeValue("X", Position.X);
            position.SetAttributeValue("Y", Position.Y);
            element.Add(position);

            return element;
        }

        public static Sprite Deserialize(XElement element, EditorView editor) {
            BitmapImage image = new BitmapImage(new Uri(element.Element("Image").Attribute("Source").Value, UriKind.RelativeOrAbsolute));
            Sprite sprite = new Sprite(element.Attribute("Name").Value, image, editor);

            XElement position = element.Element("Position");

            sprite.Position = new Point(double.Parse(position.Attribute("X").Value),
                double.Parse(position.Attribute("Y").Value));

            return sprite;
        }


        public Sprite(string name, BitmapImage image, EditorView editor) {
            this.Name = name;
            this.Image = image;
            this.editor = editor;

            editor.MouseMove += MouseMove;
            editor.MouseDown += MouseDown;
        }

        public void MouseMove(object sender, MouseEventArgs e) {
            if (Mouse.LeftButton == MouseButtonState.Pressed) {
                if (clicked) {
                    var point = editor.ScreenToWorld(editor.CurrentMousePosition);
                    // Center on mouse
                    Position = new Point(point.X - (Image.Width / 2), point.Y - (Image.Height / 2));
                    editor.InvalidateVisual();
                }
            } else if (clicked) { clicked = false; editor.CanDrag = true; }
        }

        public void MouseDown(object sender, MouseButtonEventArgs e) {
            clicked = IsClicked(editor.ScreenToWorld(e.GetPosition(editor)));
            if (clicked) editor.CanDrag = false;
            editor.InvalidateVisual();
        }

        public bool IsClicked(Point location) {
            return Rectangle.Contains(location);
        }

        public void Draw(DrawingContext context) {
            context.DrawImage(Image, Rectangle);
            if (clicked) context.DrawRectangle(null, new Pen(Brushes.LightCoral, 1), Rectangle);
        }
    }
}
