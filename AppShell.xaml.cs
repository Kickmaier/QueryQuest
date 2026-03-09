using QueryQuest;
using QueryQuest.Application.Interfaces;
using QueryQuest.Application.Services;
using QueryQuest.Views;
namespace QueryQuest
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(QuizPage), typeof(QuizPage));
        }
    }
}
