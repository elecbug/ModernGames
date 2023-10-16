using ModernGames.MGControls.MGames.Chess.Field;
using ModernGames.MGControls.MGames.Chess.Support;

namespace ModernGames.MGControls.MGames.Chess.Unit
{
    internal class Rook : Unit
    {
        public Rook(Point location, Team team, bool is_alive = true)
            : base(location, Support.Type.Rook, team, is_alive) { }
        public Rook(Rook target) : base(target) { }

        public override List<Point> AbleToMove(Unit?[,] unit_matrix)
        {
            List<Point> result = new List<Point>();

            Point left = new Point(this.location.X - 1, this.location.Y);
            Point right = new Point(this.location.X + 1, this.location.Y);
            Point up = new Point(this.location.X, this.location.Y - 1);
            Point down = new Point(this.location.X, this.location.Y + 1);

            for (int i = 0; i < FieldData.MAXIMUM; i++)
            {
                if (FieldData.IsInside(left) && unit_matrix[left.X, left.Y] == null)
                {
                    result.Add(left);
                    left = new Point(left.X - 1, left.Y);
                }
                if (FieldData.IsInside(right) && unit_matrix[right.X, right.Y] == null)
                {
                    result.Add(right);
                    right = new Point(right.X + 1, right.Y);
                }
                if (FieldData.IsInside(up) && unit_matrix[up.X, up.Y] == null)
                {
                    result.Add(up);
                    up = new Point(up.X, up.Y - 1);
                }
                if (FieldData.IsInside(down) && unit_matrix[down.X, down.Y] == null)
                {
                    result.Add(down);
                    down = new Point(down.X, down.Y + 1);
                }
            }

            return result;
        }
        public override List<Point> AbleToAttack(Unit?[,] unit_matrix)
        {
            List<Point> result = new List<Point>();

            Point left = new Point(this.location.X - 1, this.location.Y);
            Point right = new Point(this.location.X + 1, this.location.Y);
            Point up = new Point(this.location.X, this.location.Y - 1);
            Point down = new Point(this.location.X, this.location.Y + 1);

            for (int i = 0; i < FieldData.MAXIMUM; i++)
            {
                if (FieldData.IsInside(left) && unit_matrix[left.X, left.Y] == null)
                {
                    left = new Point(left.X - 1, left.Y);
                }
                else if (FieldData.IsInside(left) && unit_matrix[left.X, left.Y] != null
                    && unit_matrix[left.X, left.Y]!.Team != this.team)
                {
                    result.Add(left);
                }
                if (FieldData.IsInside(right) && unit_matrix[right.X, right.Y] == null)
                {
                    right = new Point(right.X + 1, right.Y);
                }
                else if (FieldData.IsInside(right) && unit_matrix[right.X, right.Y] != null
                    && unit_matrix[right.X, right.Y]!.Team != this.team)
                {
                    result.Add(right);
                }
                if (FieldData.IsInside(up) && unit_matrix[up.X, up.Y] == null)
                {
                    up = new Point(up.X, up.Y - 1);
                }
                else if (FieldData.IsInside(up) && unit_matrix[up.X, up.Y] != null
                    && unit_matrix[up.X, up.Y]!.Team != this.team)
                {
                    result.Add(up);
                }
                if (FieldData.IsInside(down) && unit_matrix[down.X, down.Y] == null)
                {
                    down = new Point(down.X, down.Y + 1);
                }
                else if (FieldData.IsInside(down) && unit_matrix[down.X, down.Y] != null
                    && unit_matrix[down.X, down.Y]!.Team != this.team)
                {
                    result.Add(down);
                }
            }

            return result;
        }

        public override Rook? Clone()
        {
            return new Rook(this);
        }
    }
}
