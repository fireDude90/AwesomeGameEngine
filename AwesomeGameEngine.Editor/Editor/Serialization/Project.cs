using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AwesomeGameEngine.Editor.Serialization {
    public class Project {
        private Dictionary<string, Scene> scenes = new Dictionary<string, Scene>();
        public Scene this[string name] {
            get {
                return scenes[name];
            }
        }

        private string currentScene = null;

        public Scene CurrentScene {
            get { return scenes[currentScene]; }
        }

        public Scene DefaultScene {
            get { return scenes.Where(pair => pair.Value.IsDefaultScene).First().Value; }
        }

        public Project() { }

        public void Add(Scene scene) {
            if (currentScene == null) currentScene = scene.Name;
            scenes.Add(scene.Name, scene);
        }

        public XDocument Serialize() {
            var document = new XDocument(
                new XComment(" Project file created by AwesomeGameEngine ")
                );

            var root = new XElement("Project");
            document.Add(root);

            foreach (var scene in scenes) {
                root.Add(scene.Value.Serialize());
            }

            return document;
        }

        public static Project Deserialize(XDocument document) {
            var project = new Project();

            foreach (var sceneElement in document.Root.Elements("Scene")) {
                project.Add(Scene.Deserialize(sceneElement));
            }

            return project;
        }
    }
}
