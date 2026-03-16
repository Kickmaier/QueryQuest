using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using QueryQuest.Application.Interfaces;
using QueryQuest.Core.Interfaces;
using QueryQuest.Core.Models;
using QueryQuest.ViewModels.Models;

namespace QueryQuest.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly IGameSettingsService _gameSettings;
        private readonly ITriviaService _questionService;
        public MainUIState UI { get; }
        public MainViewModel(IGameSettingsService gameSettings, ITriviaService questionService, MainUIState mainUIState)
        {
            _gameSettings = gameSettings;
            _questionService = questionService;
            UI = mainUIState;
            _gameSettings.PropertyChanged += async (s, e) =>
            {
                if (e.PropertyName == nameof(_gameSettings.CategoryId))
                {
                    _gameSettings.Amount = "10";
                    _gameSettings.Difficulty = "";
                }
                if (e.PropertyName == nameof(_gameSettings.CategoryId) ||
                    e.PropertyName == nameof(_gameSettings.Amount) ||
                    e.PropertyName == nameof(_gameSettings.Difficulty))
                {
                    OnPropertyChanged(string.Empty);
                    await UpdateDifficultyAvailabilityAsync();
                }
            };
        }

        private bool _canSelectEasy = true;
        public bool CanSelectEasy
        {
            get => _canSelectEasy;
            set { _canSelectEasy = value; OnPropertyChanged(); }
        }

        private bool _canSelectMedium = true;
        public bool CanSelectMedium
        {
            get => _canSelectMedium;
            set { _canSelectMedium = value; OnPropertyChanged(); }
        }

        private bool _canSelectHard = true;
        public bool CanSelectHard
        {
            get => _canSelectHard;
            set { _canSelectHard = value; OnPropertyChanged(); }
        }
        public string Amount => _gameSettings.Amount;
        public string Difficulty => _gameSettings.Difficulty;
        public string CategoryId => _gameSettings.CategoryId;

        public string AmountLabelText => $"Längd: {_gameSettings.AmountDisplay}";
        public string DifficultyLabelText => $"Svårighetsgrad: {_gameSettings.DifficultyDisplay}";
        public string CategoryLabelText => $"Kategori: {_gameSettings.CategoryIdDisplay}";
        public bool IsAmountError => _gameSettings.AmountDisplay == _gameSettings.AmountError;
        public bool IsDifficultyError => _gameSettings.DifficultyDisplay == _gameSettings.DifficultyError;
        public bool IsCategoryError => _gameSettings.CategoryIdDisplay == _gameSettings.UnknownCategory;
        public bool CanStartGame => !IsAmountError && !IsDifficultyError && !IsCategoryError;

        private async Task UpdateDifficultyAvailabilityAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(_gameSettings.CategoryId))
                {
                    CanSelectEasy = CanSelectMedium = CanSelectHard = true;
                    return;
                }

                int catId = int.Parse(_gameSettings.CategoryId);
                var response = await _questionService.GetCategoryStatsAsync(catId);

                int required = int.Parse(_gameSettings.Amount);

                CanSelectEasy = response.EasyCount >= required;
                CanSelectMedium = response.MediumCount >= required;
                CanSelectHard = response.HardCount >= required;
            }
            catch(Exception ex)
            {
                CanSelectEasy = CanSelectMedium = CanSelectHard = true;
                System.Diagnostics.Debug.WriteLine($"Statistik-fel: {ex.Message}");
            }
        }

        public void SetAmount(string value) { _gameSettings.Amount = value; UI.IsAmountVisible = false; }
        
        public void SetDifficulty(string value) { _gameSettings.Difficulty = value; UI.IsDifficultyVisible = false; }
        
        public void SetCategory(string value) { _gameSettings.CategoryId = value; UI.IsCategoryVisible = false; }

        public void ToggleMenu() => UI.IsMenuVisible = !UI.IsMenuVisible;
    }
}
