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
using Microsoft.Maui.Graphics;


namespace QueryQuest.ViewModels
{
    public class QuizViewModel : ObservableObjects
    {
        private readonly IQuestionService _questionService;
        
        private IDispatcherTimer _timer;

        private double _totalSeconds = 10.0;
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
        private bool _gameOverVisible = true;
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
        public double ProgressbarProgress
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
            PlayAgainCommand = new Command(async () => await LoadQuestionAsync());
            GoToMainPageCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        }

        public async Task LoadQuestionAsync()
        {
            currentQuestionIndex = 0;
            CurrentScore= 0;
            QuizAreaVisible = true;
            GameOverVisible = false;
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
        }
        public void ShowNextQuestion()
        {
            if (currentQuestionIndex < Questions.Count)
            {
                CurrentQuestion = null;
                CurrentQuestion = Questions[currentQuestionIndex];



                QuestionCounterText = $"Fråga: {currentQuestionIndex + 1} / {Questions.Count}";
                currentCorrectAnswer = CurrentQuestion.CorrectAnswer;

                _timeLeft = _totalSeconds;
                ProgressbarProgress = 1.0;

                _timer.Start();
                currentQuestionIndex++;
            }
            else
            {
                HandleGameOver();
            }
        }

        private void UpdateTimer()
        {
            _timeLeft -= 0.1;
            ProgressbarProgress = _timeLeft/ _totalSeconds;
            
            if (ProgressbarProgress > 0.66) TimerStatus = TimerState.Good;
            
            else if (ProgressbarProgress > 0.33) TimerStatus = TimerState.Warning;
            
            else TimerStatus = TimerState.Danger;

            if (_timeLeft <= 0)
            {
                _timer.Stop();
                HandleTimeOut();
            }
        }
        private async void OnAnswerSelected(AnswerOption selectedOption)
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
                var correct = CurrentQuestion.AllAnswerOptions
                    .FirstOrDefault(a => a.Text == currentCorrectAnswer);
                if (correct != null) correct.Status = AnswerStatus.Correct;
            }
                await Task.Delay(1000);
    
                IsAnswerd = false;
                ShowNextQuestion();
            }
        private async void HandleTimeOut()
        {
            await Task.Delay(300);
            ShowNextQuestion();
        }

        private void HandleGameOver()
        {
            _timer.Stop();
            QuizAreaVisible = false;
            GameOverVisible = true;
        }        
    }
}
