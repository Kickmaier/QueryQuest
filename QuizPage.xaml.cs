using QueryQuest.Data;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Collections.Generic;
namespace QueryQuest;

[QueryProperty(nameof(Amount), "amount")]
[QueryProperty(nameof(Difficulty), "difficulty")]
[QueryProperty(nameof(CategoryId), "category")]

public partial class QuizPage : ContentPage
{
    public string Amount { get; set; }
    public string Difficulty { get; set; }
    public string CategoryId { get; set; }

    private readonly OpenTriviaDbApiConnection _apiConnection = new OpenTriviaDbApiConnection();

    string currentCorrectAnswer = "";
    int currentScore = 0;
    IDispatcherTimer timer;
    double totalSeconds = 10.0;
    double timeLeft;
    List<TriviaData.Result> totalQuestions = new();
    int currentQuestionIndex = 0;
    public QuizPage()
	{
		InitializeComponent();
        timer = Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromMilliseconds(100);
        timer.Tick += (s, e) => UpdateTimer();     
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetQuestionFromApiAsync();
    }
    public async Task GetQuestionFromApiAsync()
    {
        currentQuestionIndex = 0;
        currentScore = 0;
        
        try
        {
            var questions = await _apiConnection.GetQuestionFromApiAsync(Amount, Difficulty, CategoryId);
           
            if (questions != null && questions.Count > 0)
            {
                totalQuestions = questions;
                ShowNextQuestion();     
            }
            else 
            {
                await DisplayAlert("Info", "Inga frågor hittades", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Kunde inte hämta frågor" + ex.Message, "OK");
        }

    }
    private void ShowNextQuestion()
    {
        if (currentQuestionIndex < totalQuestions.Count)
        {
            QuestionCounter.Text = $"Fråga: {currentQuestionIndex + 1} / {totalQuestions.Count}";
            var currentQuestion = totalQuestions[currentQuestionIndex];

            QuestionLabel.Text = HttpUtility.HtmlDecode(currentQuestion.question);

            currentCorrectAnswer = currentQuestion.correct_answer;
            var allAnswers = new List<string>(currentQuestion.incorrect_answers);
            allAnswers.Add(currentQuestion.correct_answer);
            var shuffledAnswers = allAnswers.OrderBy(a => Guid.NewGuid()).ToList();

            AnswerGrid.Children.Clear();
            for (int i = 0; i < shuffledAnswers.Count; i++)
            {
                var btn = new Button
                {
                    Text = HttpUtility.HtmlDecode(shuffledAnswers[i]),
                    HeightRequest = 100,
                    CornerRadius = 15,
                    BackgroundColor = Color.FromArgb("#2c3e50"),
                    TextColor = Colors.White

                };
                btn.Clicked += OnAnswerClicked;

                int row = i / 2;
                int col = i % 2;

                AnswerGrid.Add(btn, col, row);
            }
            timeLeft = totalSeconds;
            Progressbar.Progress = 1.0;
            timer.Start();
            currentQuestionIndex++;
        }
        else
        {
            HandleGameOver();
        }
    }
    private async void OnAnswerClicked(object sender, EventArgs e)
    {
        timer.Stop();
        var button = (Button)sender;
        string decodedCorrectAnswer = HttpUtility.HtmlDecode(currentCorrectAnswer);
        if (button.Text == decodedCorrectAnswer)
        {
            currentScore++;
            ScoreLabel.Text = "Poäng: " + currentScore;
        }
        if (button.Text != decodedCorrectAnswer)
        {
            button.BackgroundColor = Colors.Red;
        }
        foreach (var child in AnswerGrid.Children)
        {
            if (child is Button btn)
            {
                btn.IsEnabled = false;
                if (btn.Text == decodedCorrectAnswer)
                {
                    btn.BackgroundColor = Colors.Green;
                }
            }
        }
        await Task.Delay(500);
        ShowNextQuestion();
    }
    private void UpdateTimer()
    {
        if (Progressbar == null) return;
        timeLeft -= 0.1;
        Progressbar.Progress = timeLeft/totalSeconds;
        Progressbar.ProgressColor = (timeLeft / totalSeconds) switch
        {
           <= 0.33 => Colors.Red,
           <= 0.66 => Colors.Orange,
            _ => Colors.Gold,
        };
        if (timeLeft <= 0)
        {
            timer.Stop();
            HandleTimeOut();
            
        }
    }
    private async void HandleTimeOut()
    {
        await Progressbar.ProgressTo(1, 100, Easing.Linear);
        await Task.Delay(300);
        ShowNextQuestion();
    }
    private async void HandleGameOver()
    {
        QuizArea.IsVisible = false;
        GameOverArea.IsVisible = true;
        FinalScoreLabel.Text = ($"Du fick {currentScore} / {totalQuestions.Count} rätt!");
    }
    private async void OnPlayAgainClicked(object sender, EventArgs e)
    {
        GameOverArea.IsVisible = false;
        QuizArea.IsVisible = true;

        await GetQuestionFromApiAsync();
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (timer != null)
        {
            timer.Stop();
        }
    }

    private void OnClickedGoToMainPage(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
}