using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using QueryQuest.Application.Interfaces;
using QueryQuest.Core.Models;

namespace QueryQuest.ViewModels
{
    public class MainViewModel : ObservableObjects
    {
        private readonly IGameSettingsService _gameSettings;

        public MainViewModel(IGameSettingsService gameSettings)
        {
            _gameSettings = gameSettings;
            _gameSettings.PropertyChanged += (s, e) => Refresh();
        }

        public string Amount => _gameSettings.Amount;
        public string Difficulty => _gameSettings.Difficulty;
        public string CategoryId => _gameSettings.CategoryId;

        public string AmountLabelText => $"Längd: {_gameSettings.AmountDisplay}";
        public string DifficultyLabelText => $"Svårighetsgrad: {_gameSettings.DifficultyDisplay}";
        public string CategoryLabelText => $"Kategori: {_gameSettings.CategoryIdDisplay}";
        public bool IsAmountError => _gameSettings.AmountDisplay.Contains("Fel");
        public bool IsDifficultyError => _gameSettings.DifficultyDisplay.Contains("Fel");
        public bool IsCategoryError => _gameSettings.CategoryIdDisplay == "Okänd kategori";
        public bool CanStartGame => !IsAmountError && !IsDifficultyError && !IsCategoryError;

        public void SetAmount(string value) => _gameSettings.Amount = value;
        public void SetDifficulty(string value) => _gameSettings.Difficulty = value;
        public void SetCategory(string value) => _gameSettings.CategoryId = value;

        public void Refresh()
        {
            OnPropertyChanged(nameof(AmountLabelText));
            OnPropertyChanged(nameof(DifficultyLabelText));
            OnPropertyChanged(nameof(CategoryLabelText));
            OnPropertyChanged(nameof(IsAmountError));
            OnPropertyChanged(nameof(IsDifficultyError));
            OnPropertyChanged(nameof(IsCategoryError));
            OnPropertyChanged(nameof(CanStartGame));
        }
    }
}
