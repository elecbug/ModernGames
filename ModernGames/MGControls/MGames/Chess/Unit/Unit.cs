using ModernGames.MGControls.MGames.Chess.Support;
using Type = ModernGames.MGControls.MGames.Chess.Support.Type;

namespace ModernGames.MGControls.MGames.Chess.Unit
{
    internal abstract class Unit : IControl
    {
        protected Point location;
        protected Type type;
        protected Team team;
        protected bool is_alived;
        protected int move_count;

        public Point Location { get => location; set => location = value; }
        public Type Type { get => type; }
        public Team Team { get => team; }
        public int MoveCount { get => move_count; }

        public Unit(Point location, Type type, Team team, bool is_alived = true)
        {
            this.location = location;
            this.type = type;
            this.team = team;
            this.is_alived = is_alived;
            move_count = 0;
        }
        public Unit(Unit target)
        {
            location = target.location;
            type = target.type;
            team = target.team;
            is_alived = target.is_alived;
            move_count = target.move_count;
        }

        public void Kill() => is_alived = false;
        public void IncreaseMove() => move_count++;

        public abstract List<Point> AbleToMove(Unit?[,] unit_matrix);
        public abstract List<Point> AbleToAttack(Unit?[,] unit_matrix);

        public abstract Unit? Clone();
    }
}
