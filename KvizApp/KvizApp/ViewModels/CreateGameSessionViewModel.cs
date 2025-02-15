using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KvizApp.Services;
using KvizApp.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KvizApp.ViewModels
{
   public  partial class CreateGameSessionViewModel : ObservableObject
    {
        [ObservableProperty]
        private List<String> onlinePlayers;
        [ObservableProperty]
        private List<String> categories;
        [ObservableProperty]
        private List<String> dificulty;
        [ObservableProperty]
        private List<String> duration;
        [ObservableProperty]
        private string selectedPlayer;
        [ObservableProperty]
        private string selectedCategory;
        [ObservableProperty]
        private string selectedDifficulty;
        [ObservableProperty]
        private string selectedDuration;

        private ICreatable gameService;
        public CreateGameSessionViewModel(ICreatable service)
        {
            gameService = service;
            onlinePlayers = new List<string>();
            categories = new List<string>();
            dificulty = new List<string>();
            duration = new List<string>();
            onlinePlayers.Add("Franjo");
            onlinePlayers.Add("Ivan");
            onlinePlayers.Add("Marko");
            onlinePlayers.Add("Petar");
            categories.Add("Science: Computers");
            categories.Add("Science: Mathematics");
            categories.Add("Science: Gadgets");
            categories.Add("Science: General");
            duration.Add("10");
            duration.Add("20");
            duration.Add("30");
            dificulty.Add("easy");
            dificulty.Add("medium");
            dificulty.Add("hard");
            SelectedCategory = categories[0];
            SelectedDifficulty = dificulty[0];
            SelectedDuration = duration[0];
            SelectedPlayer = onlinePlayers[0];



        }
        [RelayCommand]
        private async Task CreateGame()
        {
            if (await gameService.CreateGame("dario",selectedPlayer,selectedCategory,selectedDifficulty,selectedDuration))
            {
                await Shell.Current.GoToAsync(nameof(MainPage));
            }
            else
            {
                Debug.WriteLine("Error creating game");
            }
            
        }
        

        }
}
