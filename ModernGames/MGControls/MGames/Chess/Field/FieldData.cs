using ModernGames.MGControls.MGames.Chess.Manager;
using ModernGames.MGControls.MGames.Chess.Support;

namespace ModernGames.MGControls.MGames.Chess.Field
{
    internal class FieldData
    {
        public const int MINIMUM = 0, MAXIMUM = 8;
        public static bool IsInside(Point point)
            => point.X >= MINIMUM && point.Y >= MINIMUM
            && point.X < MAXIMUM && point.Y < MAXIMUM;

        private GameManager manager;
        private Unit.Unit?[,] unit_matrix;
        private Unit.Unit? choose_unit;
        private List<Point> attack_points;
        private List<Point> move_points;
        private Unit.King? first_king, last_king;

        public Unit.Unit? Choosed => choose_unit;
        public List<Point> AttackPoints => attack_points;
        public List<Point> MovePoints => move_points;

        public FieldData(GameManager manager)
        {
            this.manager = manager;

            attack_points = new List<Point>();
            move_points = new List<Point>();

            unit_matrix = new Unit.Unit[MAXIMUM, MAXIMUM];
            choose_unit = null;

            DefaultCase();
            //TestCase();
        }
        public FieldData(FieldData target)
        {
            manager = target.manager;
            unit_matrix = new Unit.Unit?[MAXIMUM, MAXIMUM];
            for (int x = MINIMUM; x < MAXIMUM; x++)
            {
                for (int y = MINIMUM; y < MAXIMUM; y++)
                {
                    unit_matrix[x, y] = target.unit_matrix[x, y]?.Clone();
                }
            }
            if (target.choose_unit != null)
            {
                choose_unit
                    = target.unit_matrix[target.choose_unit!.Location.X, target.choose_unit!.Location.Y];
            }
            attack_points = new List<Point>(target.attack_points);
            move_points = new List<Point>(target.move_points);
            first_king = unit_matrix[target.first_king!.Location.X, target.first_king!.Location.Y] as Unit.King;
            last_king = unit_matrix[target.last_king!.Location.X, target.last_king!.Location.Y] as Unit.King;
        }

        public Unit.Unit? Unit(Point point)
        {
            return unit_matrix[point.X, point.Y];
        }
        public Unit.Unit? Unit(int x, int y)
        {
            return unit_matrix[x, y];
        }

        public void ControlUnit(int x, int y, bool in_check = false)
        {
            bool used_turn = false;
            ref Unit.Unit? ref_select = ref unit_matrix[x, y];

            // 재선택(선택 해제)
            if (choose_unit != null && choose_unit!.Location == new Point(x, y))
            {
                choose_unit = null;

                move_points = new List<Point>();
                attack_points = new List<Point>();
            }
            // 내 턴일 때만 가능
            else if (choose_unit != null && manager.Turn == choose_unit!.Team)
            {
                // 이미 말 집고 다른 말 선택
                if (ref_select != null && choose_unit != null)
                {
                    // castling
                    if (choose_unit!.GetType() == typeof(Unit.King)
                        && move_points.Contains(ref_select.Location))
                    {
                        foreach (Unit.Unit? unit in unit_matrix)
                        {
                            if (unit != null && manager.Turn == IsChecked(this, unit!, choose_unit.Location))
                            {
                                return;
                            }
                        }
                        if (ref_select.Location.X < choose_unit.Location.X)
                        {
                            unit_matrix[choose_unit.Location.X, choose_unit.Location.Y] = null;
                            unit_matrix[MINIMUM + 1, choose_unit.Location.Y] = choose_unit;
                            choose_unit!.IncreaseMove();
                            choose_unit!.Location = new Point(MINIMUM + 1, choose_unit.Location.Y);

                            Unit.Unit select = ref_select!;

                            unit_matrix[ref_select.Location.X, ref_select.Location.Y] = null;
                            unit_matrix[MINIMUM + 2, select.Location.Y] = select;
                            select.IncreaseMove();
                            select.Location = new Point(MINIMUM + 2, choose_unit.Location.Y);
                        }
                        else if (ref_select.Location.X > choose_unit.Location.X)
                        {
                            unit_matrix[choose_unit.Location.X, choose_unit.Location.Y] = null;
                            unit_matrix[MAXIMUM - 4, choose_unit.Location.Y] = choose_unit;
                            choose_unit!.IncreaseMove();
                            choose_unit!.Location = new Point(MAXIMUM - 4, choose_unit.Location.Y);

                            Unit.Unit select = ref_select!;

                            unit_matrix[ref_select.Location.X, ref_select.Location.Y] = null;
                            unit_matrix[MAXIMUM - 3, select.Location.Y] = select;
                            select.IncreaseMove();
                            select.Location = new Point(MAXIMUM - 3, select.Location.Y);
                        }

                        move_points = new List<Point>();
                        attack_points = new List<Point>();

                        used_turn = true;
                    }
                    // 공격 시도
                    else if (ref_select!.Team != choose_unit!.Team)
                    {
                        // 사정거리 내에 적이 있을 경우
                        if (attack_points.Contains(new Point(x, y)))
                        {
                            ref_select!.Kill();

                            // en passant!
                            if (choose_unit.GetType() == typeof(Unit.Pawn) && choose_unit.Location.Y == y
                                && (choose_unit.Team == Team.First && unit_matrix[x, y - 1] == null
                                 || choose_unit.Team == Team.Last && unit_matrix[x, y + 1] == null))
                            {
                                ref_select = null;

                                if (choose_unit.Team == Team.First)
                                {
                                    unit_matrix[x, y - 1] = choose_unit!;
                                    unit_matrix[choose_unit.Location.X, choose_unit.Location.Y] = null;

                                    choose_unit.Location = new Point(x, y - 1);
                                }
                                else if (choose_unit.Team == Team.Last)
                                {
                                    unit_matrix[x, y + 1] = choose_unit!;
                                    unit_matrix[choose_unit.Location.X, choose_unit.Location.Y] = null;

                                    choose_unit.Location = new Point(x, y + 1);
                                }
                            }
                            // 일반 공격
                            else
                            {
                                ref_select = choose_unit!;
                                unit_matrix[choose_unit.Location.X, choose_unit.Location.Y] = null;

                                choose_unit.Location = new Point(x, y);
                            }
                            if (!in_check && choose_unit!.GetType() == typeof(Unit.Pawn)
                                && (choose_unit!.Location.Y == MINIMUM || choose_unit!.Location.Y == MAXIMUM - 1))
                            {
                                Promotion(choose_unit);
                            }

                            choose_unit!.IncreaseMove();
                            choose_unit = null;

                            move_points = new List<Point>();
                            attack_points = new List<Point>();

                            used_turn = true;
                        }
                        // 사정거리 내에 적이 없을 경우
                        else
                        {
                            choose_unit = ref_select;

                            move_points = new List<Point>();
                            attack_points = new List<Point>();
                        }
                    }
                    // 선택 변경
                    else if (ref_select!.Team == choose_unit!.Team)
                    {
                        choose_unit = ref_select;

                        if (ref_select!.Team == manager.Turn)
                        {
                            attack_points = choose_unit!.AbleToAttack(unit_matrix);
                            move_points = choose_unit!.AbleToMove(unit_matrix);

                            for (int i = 0; i < move_points.Count; i++)
                            {
                                if (!in_check && IsChecked(this, choose_unit, move_points[i]) == manager.Turn)
                                {
                                    move_points.RemoveAt(i);
                                    i--;
                                }
                            }
                            for (int i = 0; i < attack_points.Count; i++)
                            {
                                if (!in_check && IsChecked(this, choose_unit, attack_points[i]) == manager.Turn)
                                {
                                    attack_points.RemoveAt(i);
                                    i--;
                                }
                            }
                        }
                    }
                }
                // 이동 시도
                else if (ref_select == null && choose_unit != null
                    && move_points.Contains(new Point(x, y)))
                {
                    ref_select = choose_unit;

                    unit_matrix[choose_unit.Location.X, choose_unit.Location.Y] = null;
                    choose_unit!.Location = new Point(x, y);

                    if (!in_check && choose_unit!.GetType() == typeof(Unit.Pawn)
                        && (choose_unit!.Location.Y == MINIMUM || choose_unit!.Location.Y == MAXIMUM - 1))
                    {
                        Promotion(choose_unit);
                    }

                    choose_unit!.IncreaseMove();
                    choose_unit = null;

                    move_points = new List<Point>();
                    attack_points = new List<Point>();

                    used_turn = true;
                }
            }
            // 연관 없는 선택 혹은 최초 선택
            else if (ref_select != null)
            {
                choose_unit = ref_select;

                if (ref_select!.Team == manager.Turn)
                {
                    attack_points = choose_unit!.AbleToAttack(unit_matrix);
                    move_points = choose_unit!.AbleToMove(unit_matrix);

                    for (int i = 0; i < move_points.Count; i++)
                    {
                        if (!in_check && IsChecked(this, choose_unit, move_points[i]) == manager.Turn)
                        {
                            move_points.RemoveAt(i);
                            i--;
                        }
                    }
                    for (int i = 0; i < attack_points.Count; i++)
                    {
                        if (!in_check && IsChecked(this, choose_unit, attack_points[i]) == manager.Turn)
                        {
                            attack_points.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
            // 맨 땅(그 외)
            else
            {
                choose_unit = null;

                move_points = new List<Point>();
                attack_points = new List<Point>();
            }

            if (used_turn)
            {
                choose_unit = null;
                move_points = new List<Point>();
                attack_points = new List<Point>();

                if (!in_check)
                {
                    manager.ChangeTurn();
                }
            }
        }

        private void Promotion(Unit.Unit choose_unit)
        {
            Support.Type what = Support.Promotion.DialogBox("Promotion", "What do you want?");

            switch (what)
            {
                case Support.Type.Queen:
                    unit_matrix[choose_unit.Location.X, choose_unit.Location.Y]
                        = new Unit.Queen(new Point(choose_unit.Location.X, choose_unit.Location.Y), choose_unit.Team);
                    this.choose_unit = unit_matrix[choose_unit.Location.X, choose_unit.Location.Y];
                    this.choose_unit!.IncreaseMove();
                    break;
                case Support.Type.Knight:
                    unit_matrix[choose_unit.Location.X, choose_unit.Location.Y]
                        = new Unit.Knight(new Point(choose_unit.Location.X, choose_unit.Location.Y), choose_unit.Team);
                    this.choose_unit = unit_matrix[choose_unit.Location.X, choose_unit.Location.Y];
                    this.choose_unit!.IncreaseMove();
                    break;
                case Support.Type.Rook:
                    unit_matrix[choose_unit.Location.X, choose_unit.Location.Y]
                        = new Unit.Rook(new Point(choose_unit.Location.X, choose_unit.Location.Y), choose_unit.Team);
                    this.choose_unit = unit_matrix[choose_unit.Location.X, choose_unit.Location.Y];
                    this.choose_unit!.IncreaseMove();
                    break;
                case Support.Type.Bishop:
                    unit_matrix[choose_unit.Location.X, choose_unit.Location.Y]
                        = new Unit.Bishop(new Point(choose_unit.Location.X, choose_unit.Location.Y), choose_unit.Team);
                    this.choose_unit = unit_matrix[choose_unit.Location.X, choose_unit.Location.Y];
                    this.choose_unit!.IncreaseMove();
                    break;
            }

        }

        private void MakeUnit(Point location, Support.Type type, Team team)
        {
            switch (type)
            {
                case Support.Type.Pawn:
                    unit_matrix[location.X, location.Y] = new Unit.Pawn(location, team); break;
                case Support.Type.Rook:
                    unit_matrix[location.X, location.Y] = new Unit.Rook(location, team); break;
                case Support.Type.Knight:
                    unit_matrix[location.X, location.Y] = new Unit.Knight(location, team); break;
                case Support.Type.Bishop:
                    unit_matrix[location.X, location.Y] = new Unit.Bishop(location, team); break;
                case Support.Type.Queen:
                    unit_matrix[location.X, location.Y] = new Unit.Queen(location, team); break;
                case Support.Type.King:
                    unit_matrix[location.X, location.Y] = new Unit.King(location, team);
                    if (team == Team.First) { first_king = unit_matrix[location.X, location.Y] as Unit.King; }
                    else if (team == Team.Last) { last_king = unit_matrix[location.X, location.Y] as Unit.King; }
                    break;
            }
        }
        private void DefaultCase()
        {
            for (int x = 0; x < 8; x++)
            {
                MakeUnit(new Point(x, 1), Support.Type.Pawn, Team.Last);
                MakeUnit(new Point(x, 6), Support.Type.Pawn, Team.First);
            }

            unit_matrix[0, 0] = new Unit.Rook(new Point(0, 0), Team.Last);
            unit_matrix[7, 0] = new Unit.Rook(new Point(7, 0), Team.Last);
            unit_matrix[0, 7] = new Unit.Rook(new Point(0, 7), Team.First);
            unit_matrix[7, 7] = new Unit.Rook(new Point(7, 7), Team.First);

            unit_matrix[1, 0] = new Unit.Knight(new Point(1, 0), Team.Last);
            unit_matrix[6, 0] = new Unit.Knight(new Point(6, 0), Team.Last);
            unit_matrix[1, 7] = new Unit.Knight(new Point(1, 7), Team.First);
            unit_matrix[6, 7] = new Unit.Knight(new Point(6, 7), Team.First);

            unit_matrix[2, 0] = new Unit.Bishop(new Point(2, 0), Team.Last);
            unit_matrix[5, 0] = new Unit.Bishop(new Point(5, 0), Team.Last);
            unit_matrix[2, 7] = new Unit.Bishop(new Point(2, 7), Team.First);
            unit_matrix[5, 7] = new Unit.Bishop(new Point(5, 7), Team.First);

            unit_matrix[4, 0] = new Unit.Queen(new Point(4, 0), Team.Last);
            unit_matrix[4, 7] = new Unit.Queen(new Point(4, 7), Team.First);

            unit_matrix[3, 0] = last_king = new Unit.King(new Point(3, 0), Team.Last);
            unit_matrix[3, 7] = first_king = new Unit.King(new Point(3, 7), Team.First);
        }
        private void TestCase()
        {
            MakeUnit(new Point(0, 1), Support.Type.King, Team.Last);
            MakeUnit(new Point(7, 7), Support.Type.King, Team.First);
            MakeUnit(new Point(1, 7), Support.Type.Rook, Team.First);
            MakeUnit(new Point(2, 7), Support.Type.Rook, Team.First);
            MakeUnit(new Point(7, 1), Support.Type.Pawn, Team.First);
        }

        private Team IsChecked(FieldData target, Unit.Unit unit, Point point)
        {
            FieldData futures = new FieldData(target);
            futures.choose_unit = futures.unit_matrix[unit.Location.X, unit.Location.Y];
            futures.move_points = futures.choose_unit!.AbleToMove(futures.unit_matrix);
            futures.attack_points = futures.choose_unit!.AbleToAttack(futures.unit_matrix);

            futures.ControlUnit(point.X, point.Y, true);

            for (int x = MINIMUM; x < MAXIMUM; x++)
            {
                for (int y = MINIMUM; y < MAXIMUM; y++)
                {
                    if (futures.unit_matrix[x, y] != null && futures.unit_matrix[x, y]!.Team != manager.Turn)
                    {
                        List<Point> points = futures.unit_matrix[x, y]!.AbleToAttack(futures.unit_matrix);

                        if (points.Contains(futures.last_king!.Location))
                        {
                            return Team.Last;
                        }
                        else if (points.Contains(futures.first_king!.Location))
                        {
                            return Team.First;
                        }
                    }
                }
            }

            return Team.NA;
        }

        public GameState IsMated()
        {
            Unit.Unit? unit = null;
            Unit.King? king = null;
            for (int x = MINIMUM; x < MAXIMUM; x++)
            {
                for (int y = MINIMUM; y < MAXIMUM; y++)
                {
                    if (unit_matrix[x, y] != null && unit_matrix[x, y]!.GetType() == typeof(Unit.King)
                        && unit_matrix[x, y]!.Team == manager.Turn)
                    {
                        king = (Unit.King)unit_matrix[x, y]!;
                    }

                    if (unit_matrix[x, y] != null && unit_matrix[x, y]!.Team == manager.Turn)
                    {
                        unit = unit_matrix[x, y];

                        List<Point> attack_points = unit_matrix[x, y]!.AbleToAttack(unit_matrix);
                        List<Point> move_points = unit_matrix[x, y]!.AbleToMove(unit_matrix);

                        for (int i = 0; i < move_points.Count; i++)
                        {
                            if (IsChecked(this, unit!, move_points[i]) == manager.Turn)
                            {
                                move_points.RemoveAt(i);
                                i--;
                            }
                        }
                        for (int i = 0; i < attack_points.Count; i++)
                        {
                            if (IsChecked(this, unit!, attack_points[i]) == manager.Turn)
                            {
                                attack_points.RemoveAt(i);
                                i--;
                            }
                        }

                        if (attack_points.Count + move_points.Count != 0)
                        {
                            unit = null;
                            return GameState.NA;
                        }
                    }
                }
            }

            for (int x = MINIMUM; x < MAXIMUM; x++)
            {
                for (int y = MINIMUM; y < MAXIMUM; y++)
                {
                    if (unit_matrix[x, y] != null && unit_matrix[x, y]!.Team != manager.Turn)
                    {
                        List<Point> points = unit_matrix[x, y]!.AbleToAttack(unit_matrix);

                        if (points.Contains(king!.Location))
                        {
                            unit = null;
                            switch (manager.Turn)
                            {
                                case Team.First: return GameState.LastWin;
                                case Team.Last: return GameState.FirstWin;
                            }
                        }
                    }
                }
            }

            unit = null;
            return GameState.Draw;
        }
    }
}