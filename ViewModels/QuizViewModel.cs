using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using QueryQuest.Application.Interfaces;
using QueryQuest.Core.Interfaces;
using QueryQuest.Core.Models;
using QueryQuest.ViewModels.Models;
using QueryQuest.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QueryQuest.ViewModels
{
    public class QuizViewModel : ObservableObjects
    {
        private readonly ITriviaService _questionService;
        private readonly IGameSettingsService _gameSettings;
        public IQuestionService QM { get; }
        public ScoreHandler SH { get; }
        public QuizUIState UI {  get; } 

        private IDispatcherTimer _timer;
        private double _totalTime = 100;
        private double _timeLeft;
        private string _selectedAnswer;

        public ICommand AnswerSelectedCommand { get; }
        public ICommand PlayAgainCommand { get; }
        public ICommand GoToMainPageCommand { get; }
        public QuizViewModel (ITriviaService questionService, IGameSettingsService gameSettings, ScoreHandler scoreHandler, IQuestionService questionManager, QuizUIState quizUIState)
        {
            _gameSettings = gameSettings;
            _questionService = questionService;
            SH = scoreHandler;
            QM = questionManager;
            UI = quizUIState;
            _timer = Dispatcher.GetForCurrentThread().CreateTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += (s, e) => UpdateTimer();

            AnswerSelectedCommand = new Command<AnswerOption>(OnAnswerSelected);
            PlayAgainCommand = new Command(async () => await ResetGame());
            GoToMainPageCommand = new Command(async () => await Shell.Current.GoToAsync("//MainPage"));

        }
        public event EventHandler TimeOutOccurred;
        
        public async Task LoadQuestionAsync()
        {
            UI.QuizAreaVisible = true;
            UI.GameOverVisible = false;

            try
            {

                var getQuestions = await _questionService.GetQuestionAsync(_gameSettings.Amount, _gameSettings.Difficulty, _gameSettings.CategoryId);

                if (getQuestions != null && getQuestions.Count > 0)
                {
                    
                    QM.PrepareQuestion(getQuestions, (int.Parse(_gameSettings.Amount)));
                    ShowNextQuestion();
                }
                else
                {
                    throw new Exception("Inga frågor hittades");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Fel vid laddning av frågor: {ex.Message} \n {ex.StackTrace}");


                string error = ex.Message switch
                {
                    var message when message.Contains("429") => "Api:et är upptaget vänta några sekunder",
                    "Inga frågor hittades"                   => "Inga frågor hittades",
                    "Fel på frågor, försök igen"             => "Fel på frågor, försök igen",
                    _                                        => "Kunde inte ladda frågor. Kontrollera din anslutning."
                };

                HandleGameOver("Hoppsan", error);
            }
        }
        public void ShowNextQuestion()
        {
            try
            {
                if(QM.HasMoreQuestions)
                {
                    QM.SetNextQuestion();
                    UI.ProgressBarProgress = 0;
                    UI.QuestionCounterText = QM.GetCurrentText();
                    _timeLeft = _totalTime;
                    _timer.Start();
                }
                else
                {
                    HandleGameOver();
                }
            }
            catch(Exception ex) 
            {
                HandleGameOver("Ett fel uppstod", "Spelet var tvunget att avbrytas.");
                Debug.WriteLine($"Fel i ShowNextQuestion: {ex.Message}");
            }
        }

        public async Task ResetGame()
        {
            _timer.Stop();

            await LoadQuestionAsync();

        }

        private void UpdateTimer()
        {
            if (UI.IsAnswerd) return;
            _timeLeft -= 1;
            double elapsedProgress = (_totalTime - _timeLeft) / _totalTime;
            UI.ProgressBarProgress = elapsedProgress;

            if (UI.ProgressBarProgress > 0.66) UI.TimerStatus = TimerState.Danger;

            else if (UI.ProgressBarProgress > 0.33) UI.TimerStatus = TimerState.Warning;

            else UI.TimerStatus = TimerState.Good;
            
            if (_timeLeft <= 0)
            {
                _timer.Stop();
                HandleTimeOut();
            }
        }
        private async void OnAnswerSelected(AnswerOption selectedOption)
        {
            if (IsInvalid(selectedOption)) return;
            try
            {
                PrepareCheck();
                bool isCorrect = ScoreAndCorrect(selectedOption);
                await Task.Delay(1000);

                PrepareForNextQuestion();
                
            }
            catch(Exception ex)
            {
                HandleGameOver("Ett fel uppstod", "Spelet var tvunget att avbrytas.");
                Debug.WriteLine($"Fel i OnAnswerSelected: {ex.Message}");
            }
        }
        private bool IsInvalid(AnswerOption selectedOption) => (selectedOption == null || UI.IsAnswerd);
        private void PrepareCheck()
        {
            UI.IsAnswerd = true;
            _timer.Stop();
        }
        private bool ScoreAndCorrect(AnswerOption selectedOption)
        {
            bool isCorrect = QM.CheckAnswer(selectedOption);
            if (isCorrect)
            {
                selectedOption.Status = AnswerStatus.Correct;
                SH.AddCorrectAnswer();
            }
            else
            {
                SH.HandleWrongAnswer();
                selectedOption.Status = AnswerStatus.Wrong;
                ShowAnswer(selectedOption);
            }
            return isCorrect;
        }
        private void PrepareForNextQuestion()
        {
            UI.IsAnswerd = false;
            ShowNextQuestion();
        }
        private void ShowAnswer(AnswerOption? selectedOption)
        {
            var correct = QM.CurrentQuestion.AllAnswerOptions
                    .FirstOrDefault(a => a.Text == QM.CurrentCorrectAnswer);
            if (correct != null) correct.Status = AnswerStatus.Correct;

        }
        private async void HandleTimeOut()
        {
            TimeOutOccurred?.Invoke(this, EventArgs.Empty);
            ShowAnswer(null);
            await Task.Delay(1000);
            PrepareForNextQuestion();
        }

        private void HandleGameOver(string? header = null, string? body = null)
        {
            UI.StatusHeader = header ?? "Spelet är över";
            UI.StatusBody = body ?? $"Slutresultat: {SH.CurrentScore}";
            UI.RetryButtonText = header != null ?"Försök igen" : "Spela igen";
            CleanUp();
            UI.QuizAreaVisible = false;
            UI.GameOverVisible = true;
        }
        public void CleanUp()
        { 
            _timer.Stop();
            SH.Reset();
            QM.Reset();
            UI.IsAnswerd = false;
            UI.ProgressBarProgress = 0;
            UI.TimerStatus = TimerState.Good;
        }
    }
}
