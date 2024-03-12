using Infragistics.Windows.DockManager.Events;
using Infragistics.Windows.DockManager;
using System.Windows.Controls;

namespace MorganStanley.ComposeUI.Shell.Layout
{
    /// <summary>
    /// Interaction logic for DockManager.xaml
    /// </summary>
    public partial class DockManager : UserControl
    {
        private static int paneCount = 0;
        public DockManager()
        {
            InitializeComponent();
        }

        public void AddContent(string header, string serializationId, object content)
        {
            var name = "pane" + paneCount++.ToString();
            var newContent = Root.AddDocument(header, content);
            newContent.Name = name;
            newContent.SerializationId = serializationId;
        }


        public SavedLayout SaveContent()
        {
            var layout = new SavedLayout();
            foreach (var pane in Root.GetPanes(PaneNavigationOrder.VisibleOrder))
            {
                layout.Panes.Add(pane.SerializationId, pane.Name);
            }
            layout.Layout = Root.SaveLayout();
            return layout;
        }

        private SavedLayout? _loading;
        public void LoadContent(SavedLayout layout)
        {
            _loading = layout;
            Root.LoadLayout(layout.Layout);
        }

        private void Root_InitializePaneContent(object sender, InitializePaneContentEventArgs e)
        {
            var id = e.NewPane.SerializationId;
            e.NewPane.TabHeader = e.NewPane.SerializationId;
            e.NewPane.Content = new TextBox() { Text = _loading.Panes[e.NewPane.SerializationId] };
            paneCount++;
        }
    }
}
