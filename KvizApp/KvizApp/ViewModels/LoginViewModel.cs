using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using KvizApp.Models;
    using KvizApp.Services;
using KvizApp.Views;

namespace KvizApp.ViewModels
{
    
    public partial class LoginViewModel : ObservableObject
    {



        // ILI ako koristite Dependency Injection:
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        [ObservableProperty]
        private string _username;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsStatusVisible))]
        private string _statusMessage;

        public bool IsStatusVisible => !string.IsNullOrEmpty(StatusMessage);
        private readonly IAuthService _authService;

        public LoginViewModel(IAuthService authService)
        {
            
            _authService = authService; _username = string.Empty;
            _password = string.Empty;
             // Simulacija mrežnog zahtjeva
           
        }

        [RelayCommand]
        private async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                StatusMessage = "Molimo unesite korisničko ime i lozinku!";
                return;
            }

            var user = new User { NickName = Username, Password = Password };
            IsLoading = true;
            // Simulacija prijave
            await Task.Delay(200); // Simulacija mrežnog zahtjeva
            
            bool isAuthenticated = await _authService.AuthenticateAsync(user);

            if (isAuthenticated)
                await Shell.Current.GoToAsync(nameof(CreateGameSession));

            else
            {
                IsLoading = false;
                StatusMessage = "Neispravni podaci za prijavu!";
            }
        }
    }
}
