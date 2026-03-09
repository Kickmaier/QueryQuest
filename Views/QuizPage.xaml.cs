using QueryQuest.ViewModels;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;

namespace QueryQuest.Views;

[QueryProperty(nameof(Amount), "amount")]
[QueryProperty(nameof(Difficulty), "difficulty")]
[QueryProperty(nameof(CategoryId), "category")]

public partial class QuizPage : ContentPage
{
    private readonly QuizViewModel _quizViewModel;

    public QuizPage(QuizViewModel quizViewModel)
    {
        InitializeComponent();
        _quizViewModel = quizViewModel;
        BindingContext = _quizViewModel;

        _quizViewModel.TimeOutOccurred += async (s, e) =>
        {
            await ShakeAndScale();
        };
    }
    public string Amount { set => _quizViewModel.Amount = value; }
    public string Difficulty { set => _quizViewModel.Difficulty = value; }
    public string CategoryId { set => _quizViewModel.CategoryId = value; }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_quizViewModel.Questions.Count == 0)
        {
            await _quizViewModel.LoadQuestionAsync();
        }
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _quizViewModel.StopTimer();
    }
    public async Task ShakeAndScale()
    {
        TimerBar.ScaleTo(1.1, 50);
        for (int i = 0; i < 3; i++)
        {
            await Task.Delay(100);
            TimerBar.Opacity = 0;
            await Task.Delay(100);
            TimerBar.Opacity = 1;
        }
        await TimerBar.TranslateTo(0, 0, 50);
        await TimerBar.ScaleTo(1.0, 100);
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}