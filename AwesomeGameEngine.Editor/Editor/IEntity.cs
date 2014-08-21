using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AwesomeGameEngine.Editor {
    public interface IEntity {
        string Name { get; set; }
        bool Selected { get; set; }
        
        /// <summary>
        /// Serializes this object
        /// </summary>
        /// <returns>XML element which can be inserted into save fiel</returns>
        XElement Serialize();
    }
}
