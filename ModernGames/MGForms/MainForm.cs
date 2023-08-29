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
                Size = new Size(500, 500),
                Location = new Point(0, 50),
                BackColor = Color.Red,
            };
            tetris.Open();
        }
    }
}