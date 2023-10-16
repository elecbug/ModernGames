using ModernGames.MGControls.MGames.Chess.Field;
using ModernGames.MGControls.MGames.Chess.Support;

namespace ModernGames.MGControls.MGames.Chess.Unit
{
    internal class Knight : Unit
    {
        public Knight(Point location, Team team, bool is_alive = true)
            : base(location, Support.Type.Knight, team, is_alive) { }
        public Knight(Knight target) : base(target) { }

        public override List<Point> AbleToMove(Unit?[,] unit_matrix)
        {
            List<Point> result = new List<Point>();

            Point[] p =
            {
                new Point(this.location.X - 1, this.location.Y - 2),
                new Point(this.location.X - 2, this.location.Y - 1),
                new Point(this.location.X - 1, this.location.Y + 2),
                new Point(this.location.X - 2, this.location.Y + 1),
                new Point(this.location.X + 1, this.location.Y - 2),
                new Point(this.location.X + 2, this.location.Y - 1),
                new Point(this.location.X + 1, this.location.Y + 2),
                new Point(this.location.X + 2, this.location.Y + 1),
            };
            for (int i = 0; i < p.Length; i++)
            {
                if (FieldData.IsInside(p[i]) && unit_matrix[p[i].X, p[i].Y] == null)
                {
                    result.Add(p[i]);
                }
            }

            return result;
        }
        public override List<Point> AbleToAttack(Unit?[,] unit_matrix)
        {

            List<Point> result = new List<Point>();

            Point[] p =
            {
                new Point(this.location.X - 1, this.location.Y - 2),
                new Point(this.location.X - 2, this.location.Y - 1),
                new Point(this.location.X - 1, this.location.Y + 2),
                new Point(this.location.X - 2, this.location.Y + 1),
                new Point(this.location.X + 1, this.location.Y - 2),
                new Point(this.location.X + 2, this.location.Y - 1),
                new Point(this.location.X + 1, this.location.Y + 2),
                new Point(this.location.X + 2, this.location.Y + 1),
            };
            for (int i = 0; i < p.Length; i++)
            {
                if (FieldData.IsInside(p[i]) && unit_matrix[p[i].X, p[i].Y] != null
                    && unit_matrix[p[i].X, p[i].Y]!.Team != this.team)
                {
                    result.Add(p[i]);
                }
            }

            return result;

        }

        public override Knight? Clone()
        {
            return new Knight(this);
        }
    }
}
