using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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

namespace XamDockManagerShell
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddContentToDockManager(object sender, RoutedEventArgs e)
        {
            DockManager.AddContent();
        }

        const string FileName = "saved.layout";

        private void SaveContent(object sender, RoutedEventArgs e)
        {
            var savedContent = DockManager.SaveContent();
            if (File.Exists(FileName))
            {
                File.Delete(FileName);
            }
            File.WriteAllText(FileName, JsonSerializer.Serialize(savedContent));

        }

        private void LoadContent(object sender, RoutedEventArgs e)
        {
            var file = File.ReadAllText(FileName);
            var layout = JsonSerializer.Deserialize<SavedLayout>(file);
            DockManager.LoadContent(layout);
        }

    }
}
