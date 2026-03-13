using QueryQuest.ViewModels;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;

namespace QueryQuest.Views;

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
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _quizViewModel.LoadQuestionAsync();
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
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _quizViewModel.CleanUp();
    }
}