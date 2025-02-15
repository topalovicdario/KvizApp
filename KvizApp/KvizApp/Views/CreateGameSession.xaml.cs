using KvizApp.Services;
using KvizApp.ViewModels;

namespace KvizApp.Views
{
    public partial class CreateGameSession : ContentPage
    {
        public CreateGameSession(CreateGameSessionViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
