using ModernGames.MGControls.MGames.Chess.Field;
using ModernGames.MGControls.MGames.Chess.Support;

namespace ModernGames.MGControls.MGames.Chess.Unit
{
    internal class Queen : Unit
    {
        public Queen(Point location, Team team, bool is_alive = true)
            : base(location, Support.Type.Queen, team, is_alive) { }
        public Queen(Queen target) : base(target) { }

        public override List<Point> AbleToMove(Unit?[,] unit_matrix)
        {
            List<Point> result = new List<Point>();

            Point left = new Point(this.location.X - 1, this.location.Y);
            Point right = new Point(this.location.X + 1, this.location.Y);
            Point up = new Point(this.location.X, this.location.Y - 1);
            Point down = new Point(this.location.X, this.location.Y + 1);
            Point left_up = new Point(this.location.X - 1, this.location.Y - 1);
            Point right_up = new Point(this.location.X + 1, this.location.Y - 1);
            Point left_down = new Point(this.location.X - 1, this.location.Y + 1);
            Point right_down = new Point(this.location.X + 1, this.location.Y + 1);

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
                if (FieldData.IsInside(left_up) && unit_matrix[left_up.X, left_up.Y] == null)
                {
                    result.Add(left_up);
                    left_up = new Point(left_up.X - 1, left_up.Y - 1);
                }
                if (FieldData.IsInside(right_up) && unit_matrix[right_up.X, right_up.Y] == null)
                {
                    result.Add(right_up);
                    right_up = new Point(right_up.X + 1, right_up.Y - 1);
                }
                if (FieldData.IsInside(left_down) && unit_matrix[left_down.X, left_down.Y] == null)
                {
                    result.Add(left_down);
                    left_down = new Point(left_down.X - 1, left_down.Y + 1);
                }
                if (FieldData.IsInside(right_down) && unit_matrix[right_down.X, right_down.Y] == null)
                {
                    result.Add(right_down);
                    right_down = new Point(right_down.X + 1, right_down.Y + 1);
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
            Point left_up = new Point(this.location.X - 1, this.location.Y - 1);
            Point right_up = new Point(this.location.X + 1, this.location.Y - 1);
            Point left_down = new Point(this.location.X - 1, this.location.Y + 1);
            Point right_down = new Point(this.location.X + 1, this.location.Y + 1);

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
                if (FieldData.IsInside(left_up) && unit_matrix[left_up.X, left_up.Y] == null)
                {
                    left_up = new Point(left_up.X - 1, left_up.Y - 1);
                }
                else if (FieldData.IsInside(left_up) && unit_matrix[left_up.X, left_up.Y] != null
                    && unit_matrix[left_up.X, left_up.Y]!.Team != this.team)
                {
                    result.Add(left_up);
                }
                if (FieldData.IsInside(right_up) && unit_matrix[right_up.X, right_up.Y] == null)
                {
                    right_up = new Point(right_up.X + 1, right_up.Y - 1);
                }
                else if (FieldData.IsInside(right_up) && unit_matrix[right_up.X, right_up.Y] != null
                    && unit_matrix[right_up.X, right_up.Y]!.Team != this.team)
                {
                    result.Add(right_up);
                }
                if (FieldData.IsInside(left_down) && unit_matrix[left_down.X, left_down.Y] == null)
                {
                    left_down = new Point(left_down.X - 1, left_down.Y + 1);
                }
                else if (FieldData.IsInside(left_down) && unit_matrix[left_down.X, left_down.Y] != null
                    && unit_matrix[left_down.X, left_down.Y]!.Team != this.team)
                {
                    result.Add(left_down);
                }
                if (FieldData.IsInside(right_down) && unit_matrix[right_down.X, right_down.Y] == null)
                {
                    right_down = new Point(right_down.X + 1, right_down.Y + 1);
                }
                else if (FieldData.IsInside(right_down) && unit_matrix[right_down.X, right_down.Y] != null
                    && unit_matrix[right_down.X, right_down.Y]!.Team != this.team)
                {
                    result.Add(right_down);
                }
            }

            return result;
        }

        public override Queen? Clone()
        {
            return new Queen(this);
        }
    }
}
