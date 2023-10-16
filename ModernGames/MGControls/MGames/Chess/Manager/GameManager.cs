using ModernGames.MGControls.MGames.Chess.Field;
using ModernGames.MGControls.MGames.Chess.Support;

namespace ModernGames.MGControls.MGames.Chess.Manager
{
    internal class GameManager
    {
        private Form parent;
        private FieldUI front;
        private FieldData back;
        private Team turn;

        public Team Turn { get => turn; }
        public FieldData Data => back;

        public GameManager(Form parent)
        {
            back = new FieldData(this);
            front = new FieldUI(this, parent.ClientSize)
            {
                Parent = parent,
                Visible = true,
                Dock = DockStyle.Fill,
            };

            this.parent = parent;
            turn = Team.First;
        }

        public void ChangeTurn()
        {
            turn = turn == Team.First ? Team.Last : Team.First;

            switch (back.IsMated())
            {
                case GameState.FirstWin: new Thread(() => { MessageBox.Show("First Win!"); }).Start(); break;
                case GameState.LastWin: new Thread(() => { MessageBox.Show("Last Win!"); }).Start(); break;
                case GameState.Draw: new Thread(() => { MessageBox.Show("Draw..."); }).Start(); break;
            }
        }
    }
}
