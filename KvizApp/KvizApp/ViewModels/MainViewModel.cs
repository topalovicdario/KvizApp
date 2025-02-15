using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KvizApp.Services;
using System.Threading.Tasks;
using System.Timers;
using System;
using Timer = System.Timers.Timer;
using System.Diagnostics;
using KvizApp.Models;
using KvizApp.Views;
using Microsoft.Maui.Graphics.Text;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Reflection;
namespace KvizApp.ViewModels
{
   public partial class MainViewModel : ObservableObject
    {   
        private  IQuizService _quizService;
        private Timer _timer;
        private int _remainingTime;
        private int id;
        [ObservableProperty]
        private string trenutnoPitanje;

        [ObservableProperty]
        private ObservableCollection<string> odgovori;

        [ObservableProperty]
        private int questionNumber;

        

        [ObservableProperty]
        private double tajmerProgress;
        public bool _isButtonVisible { get; private set; }
        public ICommand StartGameCommand { get; }
        public ICommand OdgovorCommand { get; }
        public ICommand Joker5050Command { get; }
        public ICommand JokerUltimate { get; }
        private string diff;
        private string cat;

        public MainViewModel(IQuizService quizService)
        {
            
            _quizService = quizService;
            odgovori = new ObservableCollection<string>();
            _isButtonVisible = true;

             
            OdgovorCommand = new RelayCommand<string>(OdaberiOdgovor);
            Joker5050Command = new RelayCommand(UseJoker5050);
            JokerUltimate = new RelayCommand(UseUltimateJoker);
            
         
        }
        public void Initialize()
        {
          
            Cleanup();
            GetQuestion();
        }


        public void Cleanup()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
        }

        private async void GetQuestion()

        { 
            QuestionNumber++;



            Debug.WriteLine("Dohvacam pitanje");   
           
            
            var pitanje=await  _quizService.DohvatiPitanje();
            Debug.WriteLine("Pitanje: " + TrenutnoPitanje);
            id=pitanje.Id;
            TrenutnoPitanje = pitanje.Text;
            Odgovori
             = new ObservableCollection<string>(pitanje.AllAnswers);
            
            _remainingTime = 20;
            TajmerProgress = 1.0;
            _timer = new Timer(1000);
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
        }

        private async void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _remainingTime--;

             TajmerProgress = _remainingTime / 20.0;
           

            if (_remainingTime <= 0)
            {
                
                _timer.Stop();
                Debug.WriteLine("da id je " + id);
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.Navigation.PushAsync(
                        new AnswerPage(0, 0, id,QuestionNumber));
                });
                if (QuestionNumber == 3)
                {
                    QuestionNumber = 0;
                }
            }
            else if (_remainingTime <= 6)
            {

            }
        }

        private async void OdaberiOdgovor(string index)
        {
            _timer.Stop();

            await Application.Current.MainPage.Navigation.PushAsync(new AnswerPage(int.Parse(index), 0, id, QuestionNumber));
            if (QuestionNumber == 10)
            {
                QuestionNumber = 0;
            }
            

        }
        public void OnHideButtonClicked()
        {
            _isButtonVisible = false;
        }
        private void UseJoker5050()
        {
            // Implementacija 50:50 jokera
        }

        private void UseUltimateJoker()
        {
            // Implementacija pitaj publiku jokera
        }

        
    }
}
//ako pritisne dugme odgovor, onda se poziva metoda OdaberiOdgovor koja provjerava je li odgovor dobar, koja prima index odgovora koji je korisnik odabrao,
//cekamo drugog igraca da odabere odgovor, kad i on odabere odgovor, onda se prikazuje screen sa poenima i tacnim odgovorom
//ako je odgovor tacan, onda se povecava broj poena, ako je netacan, onda se ne povecava broj poena, to traje 10sec
//nakon tog screena koji pokazuje odgovor koji je tocan i poene, prelazi se na sljedece pitanje,
//ako istekne vrijeme a nema odgovora, onda se uzima kao da je korisnik odabrao netacan odgovor
//nakon zadnjeg pitanja, prikazuje se screen sa ukupnim brojem poena i dugme za povratak na pocetni screen
//kad korisnik pritisne dugme za odgovor onda taj odgovor postaje crvene boje a ostali odgovori se blokiraju
