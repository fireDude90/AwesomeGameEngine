using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace AwesomeGameEngineEditor {
    public class EditorView : FrameworkElement {
        Size size;

        double x = 0;

        public EditorView() {
            SizeChanged += (sender, e) => {
                size = e.NewSize;
                InvalidateVisual();
            };

            CompositionTarget.Rendering += Draw;
        }

        private void Draw(Object sender, EventArgs e) {
            
        }
    }
}
