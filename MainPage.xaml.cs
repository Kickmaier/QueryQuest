using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using QueryQuest.Data;
using System.Web;
using System.Linq;
namespace QueryQuest
{
    public partial class MainPage : ContentPage
    {
        
        public MainPage()
        {
            InitializeComponent();
        }
        public void RefreshUI()
        {
            AmountLabel.Text = $"Längd: {Settings.AmountDisplay}";
            DifficultyLabel.Text = $"Svårighetsgrad: {Settings.DifficultyDisplay}";
            CategoryLabel.Text = $"Kategori: {Settings.CategoryIdDisplay}";

            AmountLabel.TextColor = Settings.AmountColor;
            DifficultyLabel.TextColor = Settings.DifficultyColor;
            CategoryLabel.TextColor = Settings.CategoryColor;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            RefreshUI();
        }
        private async void OnGetQuestionClicked(object sender, EventArgs e)
        {
            string amount = Settings.Amount;
            string difficulty = Settings.Difficulty;
            string category = Settings.CategoryId;
            Button btn = (Button)sender;
            btn.IsEnabled = false;
            btn.Text = "Laddar fråga...";
            
            await Shell.Current.GoToAsync($"{nameof(QuizPage)}?amount={amount}&difficulty={difficulty}&category={category}");

            btn.Text = "Hämtar fråga";
            btn.IsEnabled = true;
        }

        
    }
}
