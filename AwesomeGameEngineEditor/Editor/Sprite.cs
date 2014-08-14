using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AwesomeGameEngineEditor.Editor {
    class Sprite : IDrawable {
        public BitmapImage Image { get; protected set; }
        public Point Position { get; set; }
        public Rect Rectangle {
            get {
                return new Rect(Position, new Size(Image.Width, Image.Height));
            }
        }

        public Sprite(BitmapImage image) {
            this.Image = image;
        }

        public bool IsSelected(Point location) {
            return Rectangle.Contains(location);
        }

        public void Draw(DrawingContext context) {
            context.DrawImage(Image, Rectangle);
        }
    }
}
