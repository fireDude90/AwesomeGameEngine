using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AwesomeGameEngine.Editor;
using AwesomeGameEngine.Editor.Serialization;

namespace AwesomeGameEngine {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            EditorView.Content = new EditorView(this);
            InitializeFiles();
        }

        #region File Tree
        private void InitializeFiles() {
            FilesTree.Items.Add(InitializeDirectory(new DirectoryInfo(".")));
        }

        private TreeViewItem InitializeDirectory(DirectoryInfo information, DirectoryInfo root = null) {
            root = root == null ? information : root;

            StackPanel folderStack = new StackPanel() { Orientation = Orientation.Horizontal };
            Label folderLabel = new Label() { Content = information.Name };
            folderStack.Children.Add(new Image() { Source = new BitmapImage(new Uri("Icons/Files/Folder.png", UriKind.RelativeOrAbsolute)) });
            folderStack.Children.Add(folderLabel);

            TreeViewItem item = new TreeViewItem() { Header = folderStack };

            foreach (FileInfo file in information.EnumerateFiles()) {
                StackPanel filePanel = new StackPanel() { Orientation = Orientation.Horizontal };
                Label fileLabel = new Label() { Content = file.Name };

                filePanel.Children.Add(new Image() { Source = new BitmapImage(new Uri("Icons/Files/Document.png", UriKind.RelativeOrAbsolute)) });
                filePanel.Children.Add(fileLabel);

                item.Items.Add(new TreeViewItem() { Header = filePanel, Tag = new Uri(root.FullName + '/').MakeRelativeUri(new Uri(file.FullName)) });
            }

            foreach (DirectoryInfo directory in information.EnumerateDirectories()) {
                item.Items.Add(InitializeDirectory(directory, root));
            }

            return item;
        }

        private void ListPreviewMouseMove(object sender, MouseEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                //ListView list = sender as ListView;
                TreeViewItem item = FindAncestor<TreeViewItem>((DependencyObject)e.OriginalSource);
                if (item == null) return;

                Uri path = (Uri)item.Tag;
                if (path == null) return;
                DataObject data = new DataObject("Uri", path);
                DragDrop.DoDragDrop(item, data, DragDropEffects.Copy);
            }
        }

        private T FindAncestor<T>(DependencyObject current) where T : DependencyObject {
            do {
                if (current is T) return (T)current;
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        #endregion

        public void BuildProject(object sender, RoutedEventArgs e) {
            this.Log.Text = (this.EditorView.Content as EditorView).Project.Serialize().ToString();
        }

        public void TestProjectLoad(object sender, RoutedEventArgs e) {
            var editor = this.EditorView.Content as EditorView;
            editor.Project = Project.Deserialize(System.Xml.Linq.XDocument.Parse(Log.Text), editor);
            editor.CurrentScene = editor.Project["TestScene"];
            editor.InvalidateVisual();
        }

        private void EntitiesClick(object sender, RoutedEventArgs e) {
            var item = (ListBoxItem)((ListView)sender).SelectedItem;
            if (item != null) {
                ((EditorView)EditorView.Content).SelectedObject = (IEntity)(item.Tag);
            }
        }
    }
}
