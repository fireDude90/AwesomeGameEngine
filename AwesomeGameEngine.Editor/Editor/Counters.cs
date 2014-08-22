using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwesomeGameEngine.Editor {
    public static class Counters {
        private static int newGameObject = 1;
        public static int NewGameObject {
            get {
                return newGameObject++;
            }
        }
    }
}
