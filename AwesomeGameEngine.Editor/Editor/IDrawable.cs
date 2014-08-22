using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace AwesomeGameEngine.Editor {
    interface IDrawable : IEntity {
        /// <summary>
        /// Position of thing
        /// </summary>
        Point Position { get; set; }
        /// <summary>
        /// Bounding box
        /// </summary>
        Rect Rectangle { get; }

        /// <summary>
        /// Draws object using the given DrawingContext
        /// </summary>
        /// <param name="context">Context to draw with</param>
        /// <param name="scale">Context scale</param>
        void Draw(DrawingContext context, double scale);
    }
}
