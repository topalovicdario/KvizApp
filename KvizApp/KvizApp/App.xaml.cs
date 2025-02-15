using KvizApp.Views;

namespace KvizApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
            
        }
    }
}
