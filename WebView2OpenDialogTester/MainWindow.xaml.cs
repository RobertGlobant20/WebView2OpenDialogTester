using System.Windows;

namespace WebView2OpenDialogTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameComponentController controller;
        public MainWindow()
        {
            InitializeComponent();
            controller = new GameComponentController(this);
        }
    }
}