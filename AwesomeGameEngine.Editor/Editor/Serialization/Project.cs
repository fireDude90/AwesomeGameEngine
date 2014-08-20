using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AwesomeGameEngine.Editor.Serialization {
    [Serializable()]
    public class Project {
        private Dictionary<string, Scene> scenes = new Dictionary<string, Scene>();
        public Scene this[string name] {
            get {
                return scenes[name];
            }
        }

        public Project() { }

        public void Add(Scene scene) {
            scenes.Add(scene.Name, scene);
        }

        public XDocument Serialize() {
            XDocument document = new XDocument(
                new XComment(" Project file created by AwesomeGameEngine ")
                );

            XElement root = new XElement("Project");
            document.Add(root);

            foreach (var scene in scenes) {
                root.Add(scene.Value.Serialize());
            }

            return document;
        }

        public static Project Deserialize(XDocument document, EditorView editor) {
            Project project = new Project();

            foreach (XElement sceneElement in document.Root.Elements("Scene")) {
                project.Add(Scene.Deserialize(sceneElement, editor));
            }

            return project;
        }
    }
}
