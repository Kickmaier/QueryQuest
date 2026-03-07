using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Collections.Generic;
using QueryQuest.ViewModels;

namespace QueryQuest;

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
}