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
        private readonly IQuestionService _questionService;
        private readonly IGameSettingsService _gameSettings;
        public ScoreHandler SH { get; }
        public QuestionManager QM { get; }
        public QuizUIState UI {  get; } 

        private IDispatcherTimer _timer;

        private double _totalTime = 100;
        private double _timeLeft;
        private string _selectedAnswer;
        public string SelectedAnswer
        {
            get => _selectedAnswer;
            set { _selectedAnswer = value; OnPropertyChanged(); } 
        }
        public ICommand AnswerSelectedCommand { get; }
        public ICommand PlayAgainCommand { get; }
        public ICommand GoToMainPageCommand { get; }
        public QuizViewModel (IQuestionService questionService, IGameSettingsService gameSettings, ScoreHandler scoreHandler, QuestionManager questionManager, QuizUIState quizUIState)
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
                    QM.PrepareQuestion(getQuestions);
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


                string error = ex.Message.Contains("429")
                    ? "Api:et är upptaget vänta några sekunder" : (ex.Message == "Inga frågor hittades" ? "Inga frågor hittades"  
                    : "Kunde inte ladda frågor. Kontrollera din anslutning.");
                
                HandleGameOver("Hoppsan", error);
            }
        }
        public void ShowNextQuestion()
        {
            try
            {
                if(QM.SetNextQuestion())
                {
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
            try
            {
                if (selectedOption == null || UI.IsAnswerd) return;
                UI.IsAnswerd = true;
                _timer.Stop();

                if (selectedOption.Text == QM.CurrentCorrectAnswer)
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
                await Task.Delay(1000);

                UI.IsAnswerd = false;
                ShowNextQuestion();
            }
            catch(Exception ex)
            {
                HandleGameOver("Ett fel uppstod", "Spelet var tvunget att avbrytas.");
                Debug.WriteLine($"Fel i OnAnswerSelected: {ex.Message}");
            }
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
            ShowNextQuestion();
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
