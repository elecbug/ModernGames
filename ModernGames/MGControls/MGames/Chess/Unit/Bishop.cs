using ModernGames.MGControls.MGames.Chess.Field;
using ModernGames.MGControls.MGames.Chess.Support;

namespace ModernGames.MGControls.MGames.Chess.Unit
{
    internal class Bishop : Unit
    {
        public Bishop(Point location, Team team, bool is_alive = true)
            : base(location, Support.Type.Bishop, team, is_alive) { }
        public Bishop(Bishop target) : base(target) { }

        public override List<Point> AbleToMove(Unit?[,] unit_matrix)
        {
            List<Point> result = new List<Point>();

            Point left_up = new Point(this.location.X - 1, this.location.Y - 1);
            Point right_up = new Point(this.location.X + 1, this.location.Y - 1);
            Point left_down = new Point(this.location.X - 1, this.location.Y + 1);
            Point right_down = new Point(this.location.X + 1, this.location.Y + 1);

            for (int i = 0; i < FieldData.MAXIMUM; i++)
            {
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
        public override List<Point> AbleToAttack(Unit?[,] units_matrix)
        {
            List<Point> result = new List<Point>();

            Point left_up = new Point(this.location.X - 1, this.location.Y - 1);
            Point right_up = new Point(this.location.X + 1, this.location.Y - 1);
            Point left_down = new Point(this.location.X - 1, this.location.Y + 1);
            Point right_down = new Point(this.location.X + 1, this.location.Y + 1);

            for (int i = 0; i < FieldData.MAXIMUM; i++)
            {
                if (FieldData.IsInside(left_up) && units_matrix[left_up.X, left_up.Y] == null)
                {
                    left_up = new Point(left_up.X - 1, left_up.Y - 1);
                }
                else if (FieldData.IsInside(left_up) && units_matrix[left_up.X, left_up.Y] != null
                    && units_matrix[left_up.X, left_up.Y]!.Team != this.team)
                {
                    result.Add(left_up);
                }
                if (FieldData.IsInside(right_up) && units_matrix[right_up.X, right_up.Y] == null)
                {
                    right_up = new Point(right_up.X + 1, right_up.Y - 1);
                }
                else if (FieldData.IsInside(right_up) && units_matrix[right_up.X, right_up.Y] != null
                    && units_matrix[right_up.X, right_up.Y]!.Team != this.team)
                {
                    result.Add(right_up);
                }
                if (FieldData.IsInside(left_down) && units_matrix[left_down.X, left_down.Y] == null)
                {
                    left_down = new Point(left_down.X - 1, left_down.Y + 1);
                }
                else if (FieldData.IsInside(left_down) && units_matrix[left_down.X, left_down.Y] != null
                    && units_matrix[left_down.X, left_down.Y]!.Team != this.team)
                {
                    result.Add(left_down);
                }
                if (FieldData.IsInside(right_down) && units_matrix[right_down.X, right_down.Y] == null)
                {
                    right_down = new Point(right_down.X + 1, right_down.Y + 1);
                }
                else if (FieldData.IsInside(right_down) && units_matrix[right_down.X, right_down.Y] != null
                    && units_matrix[right_down.X, right_down.Y]!.Team != this.team)
                {
                    result.Add(right_down);
                }
            }

            return result;
        }

        public override Bishop? Clone()
        {
            return new Bishop(this);
        }
    }
}
