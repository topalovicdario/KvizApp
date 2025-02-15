
using CommunityToolkit.Mvvm.ComponentModel;
using KvizApp.Models;
using KvizApp.Services;
using KvizApp.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Media.Protection.PlayReady;

namespace KvizApp.ViewModels
{
   public partial class AnswerViewModel: ObservableObject
    {
        

        private IDispatcherTimer _timer;
        private int _answer;
      

        private int id_question;
        public int Score { get; set; }
        public int EnemyScore { get; set; }

       
        public bool IsCorrect { get; set; }
        [ObservableProperty]
        private string rezultat;
        [ObservableProperty]
        private string rezultatscore;
        [ObservableProperty]
        private string correctAnswer;
       
        private readonly IQuizService _quizService;

        // Svojstva ostaju ista
       

        public AnswerViewModel(IQuizService quizService)
        {
            _quizService = quizService;
        }


        public async Task InitializeAsync(int answer, int score, int id,int questionNumber)
        {

            Debug.WriteLine("ulazim u metodu koja poziva api "+answer+" id"+ id );
            var odgovor = await _quizService.ProvjeriOdgovor(answer, id, score);

            CorrectAnswer = odgovor.corect_ans;
            EnemyScore = odgovor.enemy_score;
            Score = odgovor.score;
            IsCorrect = odgovor.is_correct;

            Rezultat = $"Vaš odgovor je: {(IsCorrect ? "točan" : "netočan")}";
            Rezultatscore = $"{Score} : {EnemyScore}";
           


        }

        
            
        


    }
}
