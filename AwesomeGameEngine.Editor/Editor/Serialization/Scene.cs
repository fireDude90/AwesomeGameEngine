using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;

namespace AwesomeGameEngine.Editor.Serialization {
    public class Scene {
        public string Name { get; set; }

        private List<IEntity> entities = new List<IEntity>();
        public List<IEntity> Entities { get { return entities; } }

        public Scene(string name) {
            this.Name = name;
        }

        public void Add(IEntity entity) {
            entities.Add(entity);
        }

        public XElement Serialize() {
            XElement file = new XElement("Scene");
            file.SetAttributeValue("Name", Name);

            XElement entityNode = new XElement("Entities");
            foreach (IEntity entity in entities) {
                entityNode.Add(entity.Serialize());
            }

            file.Add(entityNode);

            return file;
        }

        public static Scene Deserialize(XElement element, EditorView editor) {
            Scene scene = new Scene(element.Attribute("Name").Value);

            foreach (XElement spriteNode in element.Element("Entities").Elements("Sprite")) {
                scene.Add(Sprite.Deserialize(spriteNode, editor));  
            }

            return scene;
        }

        public void Draw(DrawingContext context) {
            foreach (IEntity entity in entities) {
                if (entity is IDrawable) {
                    ((IDrawable)entity).Draw(context);
                }
            }
        }
    }
}
