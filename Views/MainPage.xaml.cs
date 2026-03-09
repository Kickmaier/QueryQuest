using QueryQuest.ViewModels;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace QueryQuest.Views;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _mainViewModel;
    public MainPage(MainViewModel mainViewModel)
    {
        InitializeComponent();
        _mainViewModel = mainViewModel;
        BindingContext = _mainViewModel;
    }

    private async void OnGetQuestionClicked(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        btn.Text = "Laddar fråga...";

        string amount = _mainViewModel.Amount;
        string difficulty = _mainViewModel.Difficulty;
        string category = _mainViewModel.CategoryId;
        
        await Shell.Current.GoToAsync($"{nameof(QuizPage)}?amount={amount}&difficulty={difficulty}&category={category}");

        btn.Text = "Starta Quiz";
    }
    private async void OnToggleMenu(object sender, EventArgs e)
    {
        bool isOpening = SideMenu.TranslationX < 0;

        if (isOpening)
        {
            MenuOverlay.InputTransparent = false;
            await Task.WhenAll(
                SideMenu.TranslateTo(0, 0, 1000, Easing.CubicOut),
                MainView.TranslateTo(280, 0, 1000, Easing.CubicOut),
                MenuOverlay.FadeTo(0.7, 300)
            );
        }
        else
        {
            MenuOverlay.InputTransparent = true;
            await Task.WhenAll(
                SideMenu.TranslateTo(-280, 0, 300, Easing.CubicIn),
                MainView.TranslateTo(0,0,300, Easing.CubicIn),
                MenuOverlay.FadeTo(0, 300)
            );
        }
    }
    private void SetAmount(object sender, EventArgs e)
    {
        if (sender is Button btn)
            _mainViewModel.SetAmount(btn.CommandParameter.ToString());
            AmountSelection.IsVisible = false;
        
    }

    private void SetDifficulty(object sender, EventArgs e)
    {
        if (sender is Button btn)
            _mainViewModel.SetDifficulty(btn.CommandParameter.ToString());
            DifficultySelection.IsVisible = false;
    }

    private void SetCategory(object sender, EventArgs e)
    {
        if (sender is Button btn)
            _mainViewModel.SetCategory(btn.CommandParameter.ToString());
            CategorySelection.IsVisible = false;
    }

    private void ToggleAmountSelection(object sender, EventArgs e)
    {
        AmountSelection.IsVisible = !AmountSelection.IsVisible;
        DifficultySelection.IsVisible = false;
        CategorySelection.IsVisible = false;
    }

    private void ToggleDifficultySelection(object sender, EventArgs e)
    {
        DifficultySelection.IsVisible = !DifficultySelection.IsVisible;
        AmountSelection.IsVisible = false;
        CategorySelection.IsVisible = false;
    }

    private void ToggleCategorySelection(object sender, EventArgs e)
    {
        CategorySelection.IsVisible = !CategorySelection.IsVisible;
        AmountSelection.IsVisible = false;
        DifficultySelection.IsVisible = false;
    }
}


