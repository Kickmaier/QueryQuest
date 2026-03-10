using QueryQuest.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryQuest.ViewModels.Models
{
    public class QuizUIState : ObservableObjects
    {
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
        
        private string _questionCounterText;
        public string QuestionCounterText
        {
            get => _questionCounterText;
            set { _questionCounterText = value; OnPropertyChanged(); }
        }
        
        private double _progressBarProgress = 1.0;
        public double ProgressBarProgress
        {
            get => _progressBarProgress;
            set { _progressBarProgress = value; OnPropertyChanged(); }
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

        private bool _isAnswerd;
        public bool IsAnswerd
        {
            get => _isAnswerd;
            set { _isAnswerd = value; OnPropertyChanged(); }
        }


    }
}
