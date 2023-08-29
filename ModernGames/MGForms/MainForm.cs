using ModernGames.MGControls;

namespace ModernGames
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            ExitButton exitButton = new ExitButton()
            { 
                Parent = this,
                Size = new Size(50, 50),
            };
        }
    }
}