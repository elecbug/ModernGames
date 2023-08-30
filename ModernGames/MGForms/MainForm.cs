using ModernGames.MGControls;
using ModernGames.MGControls.MGames;

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
            Tetris tetris = new Tetris(1) 
            {
                Parent = this,
                Visible = true,
                Location = new Point(0, 50),
            };
            tetris.Open();
        }
    }
}