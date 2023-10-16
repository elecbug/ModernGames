using ModernGames.MGControls.MGames.Chess.Field;
using ModernGames.MGControls.MGames.Chess.Support;

namespace ModernGames.MGControls.MGames.Chess.Unit
{
    internal class Pawn : Unit
    {
        public Pawn(Point location, Team team, bool is_alive = true)
            : base(location, Support.Type.Pawn, team, is_alive) { }
        public Pawn(Pawn target) : base(target) { }

        public override List<Point> AbleToMove(Unit?[,] unit_matrix)
        {
            List<Point> result = new List<Point>();

            if (this.team == Team.First)
            {
                Point p1 = new Point(this.location.X, this.location.Y - 1);
                Point p2 = new Point(this.location.X, this.location.Y - 2);

                if (FieldData.IsInside(p1) && unit_matrix[p1.X, p1.Y] == null)
                {
                    result.Add(p1);

                    if (FieldData.IsInside(p2) && unit_matrix[p2.X, p2.Y] == null && this.move_count == 0)
                    {
                        result.Add(p2);
                    }
                }
            }
            else if (this.team == Team.Last)
            {
                Point p1 = new Point(this.location.X, this.location.Y + 1);
                Point p2 = new Point(this.location.X, this.location.Y + 2);

                if (FieldData.IsInside(p1) && unit_matrix[p1.X, p1.Y] == null)
                {
                    result.Add(p1);

                    if (FieldData.IsInside(p2) && unit_matrix[p2.X, p2.Y] == null && this.move_count == 0)
                    {
                        result.Add(p2);
                    }
                }
            }

            return result;
        }
        public override List<Point> AbleToAttack(Unit?[,] unit_matrix)
        {
            List<Point> result = new List<Point>();

            if (this.team == Team.First)
            {
                Point p1 = new Point(this.location.X + 1, this.location.Y - 1);
                Point p2 = new Point(this.location.X - 1, this.location.Y - 1);

                if (FieldData.IsInside(p1) && unit_matrix[p1.X, p1.Y] != null
                    && unit_matrix[p1.X, p1.Y]!.Team != this.team)
                {
                    result.Add(p1);
                }
                if (FieldData.IsInside(p2) && unit_matrix[p2.X, p2.Y] != null
                    && unit_matrix[p2.X, p2.Y]!.Team != this.team)
                {
                    result.Add(p2);
                }
            }
            else if (this.team == Team.Last)
            {
                Point p1 = new Point(this.location.X + 1, this.location.Y + 1);
                Point p2 = new Point(this.location.X - 1, this.location.Y + 1);

                if (FieldData.IsInside(p1) && unit_matrix[p1.X, p1.Y] != null
                    && unit_matrix[p1.X, p1.Y]!.Team != this.team)
                {
                    result.Add(p1);
                }
                if (FieldData.IsInside(p2) && unit_matrix[p2.X, p2.Y] != null
                    && unit_matrix[p2.X, p2.Y]!.Team != this.team)
                {
                    result.Add(p2);
                }
            }

            // En Passant
            if (this.location.Y == FieldData.MINIMUM + 3 || this.location.Y == FieldData.MAXIMUM - 4)
            {
                Point p1 = new Point(this.location.X + 1, this.location.Y);
                Point p2 = new Point(this.location.X - 1, this.location.Y);

                if (FieldData.IsInside(p1) && unit_matrix[p1.X, p1.Y] != null
                    && unit_matrix[p1.X, p1.Y]!.Team != this.team && unit_matrix[p1.X, p1.Y]!.Type == Support.Type.Pawn
                    && unit_matrix[p1.X, p1.Y]!.MoveCount == 1)
                {
                    result.Add(p1);
                }
                if (FieldData.IsInside(p2) && unit_matrix[p2.X, p2.Y] != null
                    && unit_matrix[p2.X, p2.Y]!.Team != this.team && unit_matrix[p2.X, p2.Y]!.Type == Support.Type.Pawn
                    && unit_matrix[p2.X, p2.Y]!.MoveCount == 1)
                {
                    result.Add(p2);
                }
            }

            return result;
        }

        public override Pawn? Clone()
        {
            return new Pawn(this);
        }
    }
}
