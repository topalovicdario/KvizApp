using KvizApp.Views;

namespace KvizApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register only necessary routes (avoid duplicates)
            Routing.RegisterRoute(nameof(AnswerPage), typeof(AnswerPage));
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(CreateGameSession), typeof(CreateGameSession));
            // Prevent back navigation only when on AnswerPage

        }

    }
}
