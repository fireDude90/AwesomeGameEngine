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

        public bool Selected { get; set; }

        public XElement Serialize() {
            var element = new XElement("Sprite");
            element.SetAttributeValue("Name", Name);

            var image = new XElement("Image");
            image.SetAttributeValue("Source", Image.UriSource.ToString());
            element.Add(image);

            var position = new XElement("Position");
            position.SetAttributeValue("X", Position.X);
            position.SetAttributeValue("Y", Position.Y);
            element.Add(position);

            return element;
        }

        public static Sprite Deserialize(XElement element) {
            var image = new BitmapImage(new Uri(element.Element("Image").Attribute("Source").Value, UriKind.RelativeOrAbsolute));
            var sprite = new Sprite(element.Attribute("Name").Value, image);

            var position = element.Element("Position");

            sprite.Position = new Point(double.Parse(position.Attribute("X").Value),
                double.Parse(position.Attribute("Y").Value));

            return sprite;
        }


        public Sprite(string name, BitmapImage image) {
            this.Name = name;
            this.Image = image;
        }

        public void Center(Point location) {
            Position = new Point(location.X - (Image.Width / 2), location.Y - (Image.Height / 2));
        }

        public void Draw(DrawingContext context, double scale) {
            context.DrawImage(Image, Rectangle);
            if (Selected) context.DrawRectangle(null, new Pen(Brushes.LightCoral, 1.5 / scale), Rectangle);
        }

        public override string ToString() {
            return String.Format("{0}", Name);
        }
    }
}
