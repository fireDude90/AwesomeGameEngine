using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace AwesomeGameEngineEditor.Editor {
    interface IDrawable {
        /// <summary>
        /// Position of thing
        /// </summary>
        Point Position { get; set; }

        /// <summary>
        /// Finds if this thing was selected with a given click
        /// </summary>
        /// <param name="location">The location of the click</param>
        /// <returns>True if the element was selected, false if not</returns>
        bool IsClicked(Point location);

        /// <summary>
        /// Draws object using the given DrawingContext
        /// </summary>
        /// <param name="context">Context to draw with</param>
        void Draw(DrawingContext context);
    }
}
