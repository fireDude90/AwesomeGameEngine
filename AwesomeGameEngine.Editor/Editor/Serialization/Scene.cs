using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;

namespace AwesomeGameEngine.Editor.Serialization {
    public class Scene {
        public string Name { get; set; }
        public bool IsDefaultScene { get; set; }

        private List<IEntity> entities = new List<IEntity>();
        public List<IEntity> Entities { get { return entities; } }

        public Scene(string name) {
            this.Name = name;
        }

        public void Add(IEntity entity) {
            entities.Add(entity);
            BuildEntityView(((MainWindow)App.Current.MainWindow).EntitiesView);
        }

        private void BuildEntityView(ListView view) {
            view.Items.Clear();
            foreach (var entity in entities) {
                view.Items.Add(new ListBoxItem() { Content = entity.Name, Tag = entity });
            }
        }

        public XElement Serialize() {
            var file = new XElement("Scene");
            file.SetAttributeValue("Name", Name);
            file.SetAttributeValue("IsDefaultScene", IsDefaultScene);

            var entityNode = new XElement("Entities");
            foreach (var entity in entities) {
                entityNode.Add(entity.Serialize());
            }

            file.Add(entityNode);

            return file;
        }

        public static Scene Deserialize(XElement element) {
            var scene = new Scene(element.Attribute("Name").Value) {
                IsDefaultScene = bool.Parse(element.Attribute("IsDefaultScene").Value)
            };

            foreach (var spriteNode in element.Element("Entities").Elements("Sprite")) {
                scene.Add(Sprite.Deserialize(spriteNode));  
            }

            return scene;
        }

        public void Draw(DrawingContext context, double scale) {
            foreach (var entity in entities) {
                if (entity is IDrawable) {
                    ((IDrawable)entity).Draw(context, scale);
                }
            }
        }
    }
}
