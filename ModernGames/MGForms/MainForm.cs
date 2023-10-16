using ModernGames.MGControls;
using ModernGames.MGControls.MGames.Tetris;

namespace ModernGames
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            this.ClientSize = new Size(900, 900);
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            //Tetris tetris = new Tetris(1) 
            //{
            //    Parent = this,
            //    Visible = true,
            //    Location = new Point(50, 50),
            //};
            //Button button = new Button() 
            //{
            //    Parent = this,
            //};
            //button.Click += (s, e) => { tetris.Open(); };             

            MGControls.MGames.Chess.Manager.GameManager chess = new MGControls.MGames.Chess.Manager.GameManager(this);
        }
    }
}