namespace QueryQuest
{
    public partial class App : Microsoft.Maui.Controls.Application
    {
        private readonly AppShell _shell;
        public App(AppShell shell)
        {
            InitializeComponent();
            MainPage = shell;
            _shell = shell;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(_shell);
        }
    }
}