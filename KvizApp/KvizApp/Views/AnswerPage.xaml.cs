using KvizApp.Services;
using KvizApp.ViewModels;

namespace KvizApp.Views;

public partial class AnswerPage : ContentPage
{
    private readonly AnswerViewModel _viewModel;
    private readonly int _answer;
    private readonly int _score;
    private readonly int _id;
    private readonly int questionNumber;
    public AnswerPage(int answer, int score, int id,int questionNumber)
    {this.questionNumber = questionNumber;
        _answer = answer;
        _score = score;
        _id = id;

        // DI će automatski injektirati AnswerViewModel
        _viewModel = Application.Current?.Handler?.MauiContext?.Services.GetService<AnswerViewModel>();
        BindingContext = _viewModel;

        InitializeComponent();
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.InitializeAsync(_answer, _score, _id,questionNumber);

        // Pričekaj 10 sekundi prije nego se vratiš nazad
        await Task.Delay(5000);
        if (questionNumber == 10)
        {
            await Shell.Current.GoToAsync(nameof(CreateGameSession));
        }
        else
        {
            await Navigation.PopAsync();
        }
       
    }
}
