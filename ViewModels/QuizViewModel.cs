using QueryQuest.Core.Interfaces;
using QueryQuest.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using QueryQuest.Views;
using Microsoft.Maui.Graphics;


namespace QueryQuest.ViewModels
{
    public class QuizViewModel : ObservableObjects
    {
        private readonly IQuestionService _questionService;
        
        private IDispatcherTimer _timer;

        private double _totalTime = 100;
        private double _timeLeft;
        private int currentQuestionIndex = 0;
        private string currentCorrectAnswer = "";

        public ObservableCollection<Question> Questions { get; set; } = new();

        private bool _quiAreaVisible = true;
        public bool QuizAreaVisible
        {
            get { return _quiAreaVisible; }
            set { _quiAreaVisible = value; OnPropertyChanged(); }
        }
        private bool _gameOverVisible = false;
        public bool GameOverVisible
        {
            get { return _gameOverVisible; }
            set { _gameOverVisible = value; OnPropertyChanged(); }
        }

        private int _currentScore = 0;
        public int CurrentScore
        {
            get => _currentScore;
            set { _currentScore = value; OnPropertyChanged(); }
        }
        private Question _currentQuestion;
        public Question CurrentQuestion
        {
            get => _currentQuestion;
            set { _currentQuestion = value; OnPropertyChanged(); }
        }

        private TimerState _timerStatus = TimerState.Good;
        public TimerState TimerStatus
        {
            get => _timerStatus;
            set
            {
                if (_timerStatus != value)
                {
                    _timerStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _progressBarProgress = 1.0;
        public double ProgressBarProgress
        {
            get => _progressBarProgress;
            set { _progressBarProgress = value; OnPropertyChanged(); }
        }

        private string _questionCounterText;
        public string QuestionCounterText
        {
            get => _questionCounterText;
            set { _questionCounterText = value; OnPropertyChanged(); }
        }

        private string _selectedAnswer;
        public string SelectedAnswer
        {
            get => _selectedAnswer;
            set { _selectedAnswer = value; OnPropertyChanged(); } 
        }

        private bool _isAnswerd;
        public bool IsAnswerd
        {
            get => _isAnswerd;
            set { _isAnswerd = value; OnPropertyChanged(); }
        }

        private string _statusHeader;
        public string StatusHeader
        {
            get => _statusHeader;
            set { _statusHeader = value; OnPropertyChanged(); }
        }

        private string _statusBody;
        public string StatusBody
        {
            get => _statusBody;
            set { _statusBody = value; OnPropertyChanged(); }
        }

        private string _retryButtonText;
        public string RetryButtonText
        {
            get => _retryButtonText;
            set { _retryButtonText = value; OnPropertyChanged(); }
        }

        private string _amount;
        public string Amount 
        { 
            get => _amount; 
            set { _amount = value; OnPropertyChanged(); } 
        }
        private string _difficulty;
        public string Difficulty 
        {
            get => _difficulty; 
            set { _difficulty = value; OnPropertyChanged(); } 
        }
        private string _categoryId;
        public string CategoryId
        {
            get => _categoryId;
            set { _categoryId = value; OnPropertyChanged(); }
        }
        public ICommand AnswerSelectedCommand { get; }
        public ICommand PlayAgainCommand { get; }
        public ICommand GoToMainPageCommand { get; }
        public QuizViewModel (IQuestionService questionService)
        {
            _questionService = questionService;
            _timer = Dispatcher.GetForCurrentThread().CreateTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += (s, e) => UpdateTimer();

            AnswerSelectedCommand = new Command<AnswerOption>(OnAnswerSelected);
            PlayAgainCommand = new Command(async () => await ResetGame());
            GoToMainPageCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        }
        public event EventHandler TimeOutOccurred;
        
        public async Task LoadQuestionAsync()
        {
            currentQuestionIndex = 0;
            CurrentScore = 0;
            QuizAreaVisible = true;
            GameOverVisible = false;
            
            try
            {
                
                var getQuestions = await _questionService.GetQuestionAsync(Amount, Difficulty, CategoryId);

                Questions.Clear();
                if (getQuestions != null && getQuestions.Count > 0)
                {
                    foreach (var q in getQuestions)
                    {
                        Questions.Add(q);
                    }
                    ShowNextQuestion();
                }
                else
                {
                    throw new Exception("Inga frågor hittades");
                }
            }
            catch (Exception ex)
            {
                string error = (ex.Message == "Inga frågor hittades")
                    ? ex.Message
                    : "Kunde inte ladda frågor. Kontrollera din anslutning.";
                
                HandleGameOver("Hoppsan", error);
            }
        }
        public void ShowNextQuestion()
        {
            try
            {
                if (currentQuestionIndex < Questions.Count)
                {
                    ProgressBarProgress = 0;
                    CurrentQuestion = null;
                    CurrentQuestion = Questions[currentQuestionIndex];



                    QuestionCounterText = $"Fråga: {currentQuestionIndex + 1} / {Questions.Count}";
                    currentCorrectAnswer = CurrentQuestion.CorrectAnswer;

                    _timeLeft = _totalTime;


                    _timer.Start();
                    currentQuestionIndex++;
                }
                else
                {
                    HandleGameOver();
                }
            }
            catch
            {
                HandleGameOver("Ett fel uppstod", "Spelet var tvunget att avbrytas.");
            }
        }

        public async Task ResetGame()
        {
            _timer.Stop();

            await LoadQuestionAsync();

        }

        private void UpdateTimer()
        {
            if (IsAnswerd) return;
            _timeLeft -= 1;
            double elapsedProgress = (_totalTime - _timeLeft) / _totalTime;
            ProgressBarProgress = elapsedProgress;

            if (ProgressBarProgress > 0.66) TimerStatus = TimerState.Danger;

            else if (ProgressBarProgress > 0.33) TimerStatus = TimerState.Warning;

            else TimerStatus = TimerState.Good;
            
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
                if (selectedOption == null || IsAnswerd) return;
                IsAnswerd = true;
                _timer.Stop();



                if (selectedOption.Text == currentCorrectAnswer)
                {
                    selectedOption.Status = AnswerStatus.Correct;
                    CurrentScore++;
                }
                else
                {
                    selectedOption.Status = AnswerStatus.Wrong;
                    ShowAnswer(selectedOption);
                }
                await Task.Delay(1000);

                IsAnswerd = false;
                ShowNextQuestion();
            }
            catch
            {
                HandleGameOver("Ett fel uppstod", "Spelet var tvunget att avbrytas.");
            }
        }
        private void ShowAnswer(AnswerOption? selectedOption)
        {
            var correct = CurrentQuestion.AllAnswerOptions
                    .FirstOrDefault(a => a.Text == currentCorrectAnswer);
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
            StatusHeader = header ?? "Spelet är över";
            StatusBody = body ?? $"Slutresultat: {CurrentScore}";
            RetryButtonText = header != null ?"Försök igen" : "Spela igen";
            CurrentQuestion = null;
            ProgressBarProgress = 1.0;
            _timer.Stop();
            QuizAreaVisible = false;
            GameOverVisible = true;
        }

        public void StopTimer()
        {
            _timer?.Stop();
        }

    }
}
