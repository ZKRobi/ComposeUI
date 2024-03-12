using System.Collections.Generic;

namespace MorganStanley.ComposeUI.Shell.Layout;

public class SavedLayout
{
    public string Layout { get; set; }
    public Dictionary<string, string> Panes { get; set; } = new Dictionary<string, string>();
}
