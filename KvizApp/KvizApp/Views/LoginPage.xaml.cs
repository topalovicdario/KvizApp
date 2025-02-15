using KvizApp.ViewModels;
using Microsoft.Maui.Controls;
namespace KvizApp.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        
    }
}