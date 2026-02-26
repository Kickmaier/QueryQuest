using QueryQuest;
namespace QueryQuest
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(QuizPage), typeof(QuizPage));
        }
        private void ToggleAmountSelection(object sender,EventArgs e)
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
        private void ToggleCategoryIdSelection(object sender, EventArgs e)
        {
            CategorySelection.IsVisible = !CategorySelection.IsVisible;
            AmountSelection.IsVisible =false;
            DifficultySelection.IsVisible =false;
        }
        private async void SetAmount(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            Settings.Amount = btn.CommandParameter.ToString();
            FindMainPage();
            AmountSelection.IsVisible = false;
        }
        private async void SetDifficulty(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            Settings.Difficulty = btn.CommandParameter.ToString();
            FindMainPage();
            DifficultySelection.IsVisible = false;
        }
        private async void SetCategoryId(object sender, EventArgs e)
        {
            var btn =(Button)sender;
            Settings.CategoryId = btn.CommandParameter.ToString();
            FindMainPage();
            CategorySelection.IsVisible = false;
        }
        private void FindMainPage()
        {
            var mainPage = Shell.Current.Navigation.NavigationStack
            .OfType<MainPage>()
.           FirstOrDefault();
            if (mainPage != null)
            {
                mainPage.RefreshUI();
            }
            else if (Shell.Current.CurrentPage is MainPage current)
            {
                current.RefreshUI();
            }
        }
    }
}
