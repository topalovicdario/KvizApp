using KvizApp.Services;
using KvizApp.ViewModels;

namespace KvizApp.Views;

public partial class MainPage : ContentPage
{
    private MainViewModel _viewModel;

   
       
    
    public  MainPage(MainViewModel viewModel)
	{
		InitializeComponent();
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });
         BindingContext = _viewModel = viewModel;
    }
    protected async override void OnAppearing()
    {
        base.OnAppearing();
      _viewModel.Initialize();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.Cleanup();
    }
}