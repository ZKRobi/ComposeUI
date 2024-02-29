// Morgan Stanley makes this available to you under the Apache License,
// Version 2.0 (the "License"). You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0.
// 
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership. Unless required by applicable law or agreed
// to in writing, software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
// or implied. See the License for the specific language governing permissions
// and limitations under the License.


using Infragistics.Windows.DockManager;
using Infragistics.Windows.DockManager.Events;
using System.Windows.Controls;

namespace XamDockManagerShell;

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

    public void AddContent()
    {
        var name = "pane" + paneCount++.ToString();
        var newContent = this.Root.AddDocument(name, new Grid() { Height = 200, Width = 200 });
        newContent.Content = new TextBlock { Text = name };
        newContent.Name = name;
        newContent.SerializationId = name;
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

    private SavedLayout _loading;
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
