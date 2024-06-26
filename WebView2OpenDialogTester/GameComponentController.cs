using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace WebView2OpenDialogTester
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class ScriptObject
    {
        Action<object[]> onMarkAllAsRead;
        Action<int> onNotificationPopupUpdated;

        internal ScriptObject()
        {

        }
    }

    internal class GameComponentController
    {
        private readonly GameComponent gameUIComponent;
        private static readonly string htmlEmbeddedFile = "WebView2OpenDialogTester.Packages.HelloWorldReactApp.build.index.html";
        private static readonly string jsEmbeddedFile = "WebView2OpenDialogTester.Packages.HelloWorldReactApp.build.main.js";
        MainWindow mainWindow;

        public GameComponentController(MainWindow view)
        {
            mainWindow = view;
            gameUIComponent = new GameComponent();
            InitializeBrowserAsync();
            gameUIComponent.webView.NavigationCompleted += WebView_NavigationCompleted;
            mainWindow.mainGrid.Children.Add(gameUIComponent);
        }

        private void WebView_NavigationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {

        }

        private async void InitializeBrowserAsync()
        {
            gameUIComponent.webView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
            await gameUIComponent.webView.EnsureCoreWebView2Async();
            gameUIComponent.webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
        }

        private void CoreWebView2_WebMessageReceived(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            String strMessage = e.TryGetWebMessageAsString();
            if (strMessage.Contains("OpenFileDialog"))
            {
                ShowDialog();
            }
        }

        private void ShowDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {

            }
        }

        private void WebView_CoreWebView2InitializationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string htmlString = string.Empty;
            var embeddedResources = assembly.GetManifestResourceNames();
            using (Stream stream = assembly.GetManifestResourceStream(htmlEmbeddedFile))
            using (StreamReader reader = new StreamReader(stream))
            {
                htmlString = reader.ReadToEnd();
            }

            using (Stream stream = assembly.GetManifestResourceStream(jsEmbeddedFile))
            using (StreamReader reader = new StreamReader(stream))
            {
                var jsString = reader.ReadToEnd();
                htmlString = htmlString.Replace("mainJs", jsString);
            }

            if (gameUIComponent.webView.CoreWebView2 != null)
            {
                // More initialization options
                // Context menu disabled
                gameUIComponent.webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;

                gameUIComponent.webView.CoreWebView2.FrameCreated += CoreWebView2_FrameCreated; ;


                gameUIComponent.webView.CoreWebView2.NavigateToString(htmlString);
                // Hosts an object that will expose the properties and methods to be called from the javascript side
                gameUIComponent.webView.CoreWebView2.AddHostObjectToScript("scriptObject",
                    new ScriptObject());

                gameUIComponent.webView.CoreWebView2.Settings.IsZoomControlEnabled = false;
            }
        }

        private void CoreWebView2_FrameCreated(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2FrameCreatedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
