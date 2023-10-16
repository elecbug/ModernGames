namespace ModernGames.MGControls.MGames.Chess.Unit
{
    internal interface IControl
    {
        public abstract List<Point> AbleToMove(Unit?[,] unit_matrix);
        public abstract List<Point> AbleToAttack(Unit?[,] unit_matrix);
    }
}
