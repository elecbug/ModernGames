using ModernGames.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ModernGames.MGControls.MGames.Tetris
{
    public class Tetris : TimerGameBasePanel
    {
        /// <summary>
        /// 게임판 가로 길이
        /// </summary>
        private const int X_LENGTH = 10;
        /// <summary>
        /// 게임판 세로 길이 (실제로는 20, 데스 존 위의 Floating 공간을 위함)
        /// </summary>
        private const int Y_LENGTH = 25;
        /// <summary>
        /// 데스 라인, 이 위에 블록이 Building되면 게임 종료
        /// </summary>
        private const int DEATH_LINE = 5;
        /// <summary>
        /// 블럭의 종류
        /// </summary>
        private const int BLOCK_COUNT = 7;
        /// <summary>
        /// 한번에 오르는 점수
        /// </summary>
        private const int UP_SCORE = 10;
        /// <summary>
        /// 블록 사이즈
        /// </summary>
        private const int SIZE = 30;
        /// <summary>
        /// 블록이 처음 소환되는 위치 중 왼쪽 위
        /// </summary>
        private readonly Point SpawnLocation = new Point(X_LENGTH / 2, DEATH_LINE - 2);

        /// <summary>
        /// 게임이 진행되는 2차원 공간
        /// </summary>
        private List<List<Block>> Space;
        /// <summary>
        /// 다음 블록들의 리스트, 7개를 각각 하나씩 채우며 진행, 모두 비었을 때 다시 채움
        /// </summary>
        private List<Block> NextList;
        /// <summary>
        /// 점수
        /// </summary>
        private int Score;
        /// <summary>
        /// 콤보
        /// </summary>
        private int Combo;
        /// <summary>
        /// 점수 레이블
        /// </summary>
        private Label ScoreLabel;
        /// <summary>
        /// 다음 블록이 표시되는 상자
        /// </summary>
        private PictureBox NextBlockViewer;

        /// <summary>
        /// 테트리스 게임 시작 생성자
        /// </summary>
        /// <param name="level"> 게임의 난이도(0~99) </param>
        public Tetris(int level) : base("Tetris", "The Modern Tetris", (100 - level) * 5)
        {
            Size = new Size(750, 750);
            BackColor = Color.DarkSlateGray;
            ScoreLabel = new Label()
            {
                Parent = this,
                Visible = true,
                Location = new Point(X_LENGTH * SIZE + SIZE, SIZE),
                ForeColor = Color.White,
                AutoSize = true,
                Font = new Font("Consolas", 12),
            };
            NextBlockViewer = new PictureBox()
            {
                Parent = this,
                Visible = true,
                Location = new Point(X_LENGTH * SIZE + SIZE, SIZE * 3),
                Size = new Size(100, 100),
                BackColor = Color.White,
            };
            Disposed += TetrisDisposed;
            Space = new List<List<Block>>();
            for (int x = 0; x < X_LENGTH; x++)
            {
                Space.Add(new List<Block>(new Block[Y_LENGTH]));
            }
            NextList = new List<Block>();
        }

        private void TetrisDisposed(object? sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// NextList의 원소가 하나인 경우 뒤에서 하나씩 채움
        /// </summary>
        private void NextListSetting()
        {
            if (NextList.Count > 1)
            {
                Debug.WriteLine("다음 블록 리스트가 비지 않았는데 리스트를 채우려 했습니다.");
                return;
            }

            for (int i = 0; i < BLOCK_COUNT; i++)
            {
                NextList.Add(Block.Floating | (Block)(4 * Math.Pow(2, i)));
            }

            int n = NextList.Count;

            while (n > 2)
            {
                n--;
                int k = new Random(DateTime.Now.Millisecond).Next(1, n + 1);
                Block value = NextList[k];
                NextList[k] = NextList[n];
                NextList[n] = value;
            }
        }
        /// <summary>
        /// NextList에서 요소 하나를 반환하고, 만약 리스트가 하나 남았다면 새로 채움
        /// </summary>
        /// <returns></returns>
        private Block PopBlock()
        {
            if (NextList.Count <= 1)
            {
                NextListSetting();
            }

            Block now = NextList[0];
            NextList.RemoveAt(0);

            return now;
        }
        /// <summary>
        /// 실제 좌표상 블록 생성 메서드
        /// </summary>
        /// <param name="block"> 생성할 새 블록 </param>
        private void CreateBlock(Block block)
        {
            switch (block)
            {
                case Block.ZBlock:
                    Space[SpawnLocation.X][SpawnLocation.Y] = Block.Floating | block;
                    Space[SpawnLocation.X + 1][SpawnLocation.Y] = Block.Floating | block;
                    Space[SpawnLocation.X + 1][SpawnLocation.Y + 1] = Block.Floating | block;
                    Space[SpawnLocation.X + 2][SpawnLocation.Y + 1] = Block.Floating | block;
                    break;
                case Block.ReverseZ:
                    Space[SpawnLocation.X][SpawnLocation.Y + 1] = Block.Floating | block;
                    Space[SpawnLocation.X + 1][SpawnLocation.Y + 1] = Block.Floating | block;
                    Space[SpawnLocation.X + 1][SpawnLocation.Y] = Block.Floating | block;
                    Space[SpawnLocation.X + 2][SpawnLocation.Y] = Block.Floating | block;
                    break;
                case Block.TBlock:
                    Space[SpawnLocation.X][SpawnLocation.Y + 1] = Block.Floating | block;
                    Space[SpawnLocation.X + 1][SpawnLocation.Y + 1] = Block.Floating | block;
                    Space[SpawnLocation.X + 2][SpawnLocation.Y + 1] = Block.Floating | block;
                    Space[SpawnLocation.X + 1][SpawnLocation.Y] = Block.Floating | block;
                    break;
                case Block.BoxBlock:
                    Space[SpawnLocation.X][SpawnLocation.Y] = Block.Floating | block;
                    Space[SpawnLocation.X][SpawnLocation.Y + 1] = Block.Floating | block;
                    Space[SpawnLocation.X + 1][SpawnLocation.Y] = Block.Floating | block;
                    Space[SpawnLocation.X + 1][SpawnLocation.Y + 1] = Block.Floating | block;
                    break;
                case Block.IBlock:
                    Space[SpawnLocation.X][SpawnLocation.Y] = Block.Floating | block;
                    Space[SpawnLocation.X + 1][SpawnLocation.Y] = Block.Floating | block;
                    Space[SpawnLocation.X + 2][SpawnLocation.Y] = Block.Floating | block;
                    Space[SpawnLocation.X + 3][SpawnLocation.Y] = Block.Floating | block;
                    break;
                case Block.LBlock:
                    Space[SpawnLocation.X][SpawnLocation.Y] = Block.Floating | block;
                    Space[SpawnLocation.X + 1][SpawnLocation.Y] = Block.Floating | block;
                    Space[SpawnLocation.X + 2][SpawnLocation.Y] = Block.Floating | block;
                    Space[SpawnLocation.X][SpawnLocation.Y + 1] = Block.Floating | block;
                    break;
                case Block.ReverseL:
                    Space[SpawnLocation.X][SpawnLocation.Y] = Block.Floating | block;
                    Space[SpawnLocation.X + 1][SpawnLocation.Y] = Block.Floating | block;
                    Space[SpawnLocation.X + 2][SpawnLocation.Y] = Block.Floating | block;
                    Space[SpawnLocation.X + 2][SpawnLocation.Y + 1] = Block.Floating | block;
                    break;
            }
        }
        /// <summary>
        /// 특정 좌표 주위 4칸 내의 모든 Floating 블록을 Build하고 라인이 완성되었는 지, 아웃 판정 등을 확인
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void ChangeBuilding(int x, int y)
        {
            for (int xx = x - 3; xx < x + 4; xx++)
            {
                for (int yy = y - 3; yy < y + 4; yy++)
                {
                    if (xx >= 0 && yy >= 0 && xx < X_LENGTH && yy < Y_LENGTH)
                    {
                        if ((Space[xx][yy] & Block.Floating) != Block.Empty)
                        {
                            Space[xx][yy] ^= Block.Floating;
                            Space[xx][yy] ^= Block.Building;
                        }
                    }
                }
            }

            // 데스라인
            for (int xx = 0; xx < X_LENGTH; xx++)
            {
                if ((Space[xx][DEATH_LINE] & Block.Building) != Block.Empty)
                {
                    Close();
                }
            }

            // 라인완성
            int temp = Combo;
            bool now_claer = false;
            for (int yy = 0; yy < Y_LENGTH; yy++)
            {
                bool line_clear = true;
                for (int xx = 0; xx < X_LENGTH; xx++)
                {
                    line_clear &= (Space[xx][yy] & Block.Building) != Block.Empty;
                }
                if (line_clear)
                {
                    // 라인 지우고 점수 업
                    LineRemove(yy);
                    now_claer = true;
                    temp++;
                    Score += UP_SCORE * temp;
                }
                if (now_claer)
                {
                    Combo = temp;
                }
                else if (!now_claer)
                {
                    Combo = 0;
                }
            }
        }
        /// <summary>
        /// 특정 좌표 주위 4칸 내의 모든 Floating 블록을 다운시키다가 Building 블록을 만나면 ChangeBuilding 호출
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns> 블록이 빌딩 되었는 지 여부 </returns>
        private bool DownBlock(int x, int y)
        {
            // 블록이 빌딩될 건지 확인
            for (int xx = x - 3; xx < x + 4; xx++)
            {
                for (int yy = y + 3; yy > y - 4; yy--)
                {
                    if (xx >= 0 && yy >= 0 && xx < X_LENGTH && yy + 1 <= Y_LENGTH)
                    {
                        if ((Space[xx][yy] & Block.Floating) != Block.Empty
                            && (Space[xx][yy] & Block.Building) != Block.Building)
                        {
                            if (yy + 1 == Y_LENGTH || (Space[xx][yy + 1] & Block.Building) != Block.Empty)
                            {
                                ChangeBuilding(xx, yy);
                                return true;
                            }
                        }
                    }
                }
            }

            // 아니라면 다운
            for (int xx = x - 3; xx < x + 4; xx++)
            {
                for (int yy = y + 3; yy > y - 4; yy--)
                {
                    if (xx >= 0 && yy >= 0 && xx < X_LENGTH && yy + 1 < Y_LENGTH)
                    {
                        if ((Space[xx][yy] & Block.Floating) != Block.Empty)
                        {
                            Space[xx][yy + 1] = Space[xx][yy];
                            Space[xx][yy] = Block.Empty;
                        }
                    }
                }
            }

            return false;
        }
        /// <summary>
        /// 해당 높이의 라인을 모두 삭제
        /// </summary>
        /// <param name="y"></param>
        private void LineRemove(int y)
        {
            for (int i = 0; i < X_LENGTH; i++)
            {
                Space[i].RemoveAt(y);
                Space[i].Insert(0, Block.Empty);
            }
        }
        /// <summary>
        /// 좌로 밀착
        /// </summary>
        private void LeftMove()
        {
            int count = 0;

            for (int y = 0; y < Y_LENGTH; y++)
            {
                for (int x = 1; x < X_LENGTH; x++)
                {
                    if ((Space[x - 1][y] == Block.Empty
                        || (Space[x - 1][y] & Block.Floating) == Block.Floating)
                        && (Space[x][y] & Block.Floating) != Block.Empty)
                    {
                        count++;
                    }
                }
            }

            if (count == 4)
            {
                for (int y = 0; y < Y_LENGTH; y++)
                {
                    for (int x = 1; x < X_LENGTH; x++)
                    {
                        if (Space[x - 1][y] == Block.Empty
                            && (Space[x][y] & Block.Floating) != Block.Empty)
                        {
                            Space[x - 1][y] = Space[x][y];
                            Space[x][y] = Block.Empty;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 우로 밀착
        /// </summary>
        private void RightMove()
        {
            int count = 0;

            for (int y = 0; y < Y_LENGTH; y++)
            {
                for (int x = X_LENGTH - 2; x >= 0; x--)
                {
                    if ((Space[x + 1][y] == Block.Empty
                        || (Space[x + 1][y] & Block.Floating) == Block.Floating)
                        && (Space[x][y] & Block.Floating) != Block.Empty)
                    {
                        count++;
                    }
                }
            }

            if (count == 4)
            {
                for (int y = 0; y < Y_LENGTH; y++)
                {
                    for (int x = X_LENGTH - 2; x >= 0; x--)
                    {
                        if (Space[x + 1][y] == Block.Empty
                            && (Space[x][y] & Block.Floating) != Block.Empty)
                        {
                            Space[x + 1][y] = Space[x][y];
                            Space[x][y] = Block.Empty;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 화면 갱신 메서드
        /// </summary>
        private void GraphicDesign()
        {
            Graphics graphics = CreateGraphics();
            try
            {
                Invoke(delegate () { ScoreLabel.Text = "Score: " + Score + " next + " + Combo * 10 + "!"; });
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
            for (int y = 0; y < Y_LENGTH; y++)
            {
                for (int x = 0; x < X_LENGTH; x++)
                {
                    if ((Space[x][y] & Block.LBlock) != Block.Empty)
                    {
                        graphics.DrawRectangle(new Pen(new SolidBrush(Color.DarkRed), SIZE / 2),
                            new Rectangle(new Point(x * SIZE + SIZE / 2, y * SIZE - SIZE / 2), new Size(SIZE / 2, SIZE / 2)));
                    }
                    else if ((Space[x][y] & Block.ReverseL) != Block.Empty)
                    {
                        graphics.DrawRectangle(new Pen(new SolidBrush(Color.DarkOrange), SIZE / 2),
                            new Rectangle(new Point(x * SIZE + SIZE / 2, y * SIZE - SIZE / 2), new Size(SIZE / 2, SIZE / 2)));
                    }
                    else if ((Space[x][y] & Block.ZBlock) != Block.Empty)
                    {
                        graphics.DrawRectangle(new Pen(new SolidBrush(Color.YellowGreen), SIZE / 2),
                            new Rectangle(new Point(x * SIZE + SIZE / 2, y * SIZE - SIZE / 2), new Size(SIZE / 2, SIZE / 2)));
                    }
                    else if ((Space[x][y] & Block.ReverseZ) != Block.Empty)
                    {
                        graphics.DrawRectangle(new Pen(new SolidBrush(Color.DarkGreen), SIZE / 2),
                            new Rectangle(new Point(x * SIZE + SIZE / 2, y * SIZE - SIZE / 2), new Size(SIZE / 2, SIZE / 2)));
                    }
                    else if ((Space[x][y] & Block.IBlock) != Block.Empty)
                    {
                        graphics.DrawRectangle(new Pen(new SolidBrush(Color.DarkBlue), SIZE / 2),
                            new Rectangle(new Point(x * SIZE + SIZE / 2, y * SIZE - SIZE / 2), new Size(SIZE / 2, SIZE / 2)));
                    }
                    else if ((Space[x][y] & Block.TBlock) != Block.Empty)
                    {
                        graphics.DrawRectangle(new Pen(new SolidBrush(Color.DarkViolet), SIZE / 2),
                            new Rectangle(new Point(x * SIZE + SIZE / 2, y * SIZE - SIZE / 2), new Size(SIZE / 2, SIZE / 2)));
                    }
                    else if ((Space[x][y] & Block.BoxBlock) != Block.Empty)
                    {
                        graphics.DrawRectangle(new Pen(new SolidBrush(Color.Purple), SIZE / 2),
                            new Rectangle(new Point(x * SIZE + SIZE / 2, y * SIZE - SIZE / 2), new Size(SIZE / 2, SIZE / 2)));
                    }
                    else
                    {
                        graphics.DrawRectangle(new Pen(new SolidBrush(Color.White), SIZE / 2),
                            new Rectangle(new Point(x * SIZE + SIZE / 2, y * SIZE - SIZE / 2), new Size(SIZE / 2, SIZE / 2)));
                    }
                }
            }
        }
        /// <summary>
        /// 좌로 굴러
        /// </summary>
        private void Spin(bool is_left)
        {
            if (is_left)
            {
                try
                {
                    for (int y = 0; y < Y_LENGTH; y++)
                    {
                        for (int x = 0; x < X_LENGTH; x++)
                        {
                            if ((Space[x][y] & Block.Floating) != Block.Empty)
                            {
                                if ((Space[x][y] & Block.LBlock) != Block.Empty)
                                {
                                    if (y < Y_LENGTH - 1 && (Space[x][y + 1] & Block.LBlock) != Block.Empty)
                                    {
                                        if (y < Y_LENGTH - 2 && (Space[x][y + 2] & Block.LBlock) != Block.Empty)
                                        {
                                            // a
                                            // a    
                                            // a a

                                            if (x >= 1 && x < X_LENGTH - 1 && y >= 1 && y < Y_LENGTH - 1
                                                && Space[x - 1][y + 1] == Block.Empty
                                                && Space[x + 1][y] == Block.Empty
                                                && Space[x + 1][y + 1] == Block.Empty)
                                            {
                                                Space[x - 1][y + 1] = Block.Floating | Block.LBlock;
                                                Space[x + 1][y] = Block.Floating | Block.LBlock;
                                                Space[x + 1][y + 1] = Block.Floating | Block.LBlock;

                                                Space[x][y] = Block.Empty;
                                                Space[x][y + 2] = Block.Empty;
                                                Space[x + 1][y + 2] = Block.Empty;
                                            }
                                        }
                                        else
                                        {
                                            if (x < X_LENGTH - 1 && (Space[x + 1][y] & Block.LBlock) != Block.Empty)
                                            {
                                                // a a a
                                                // a

                                                if (x >= 0 && x < X_LENGTH - 1 && y >= 1 && y < Y_LENGTH - 2
                                                    && Space[x + 1][y - 1] == Block.Empty
                                                    && Space[x + 1][y + 1] == Block.Empty
                                                    && Space[x + 2][y + 1] == Block.Empty)
                                                {
                                                    Space[x + 1][y - 1] = Block.Floating | Block.LBlock;
                                                    Space[x + 1][y + 1] = Block.Floating | Block.LBlock;
                                                    Space[x + 2][y + 1] = Block.Floating | Block.LBlock;

                                                    Space[x][y] = Block.Empty;
                                                    Space[x][y + 1] = Block.Empty;
                                                    Space[x + 2][y] = Block.Empty;
                                                }
                                            }
                                            else
                                            {
                                                //     a
                                                // a a a

                                                if (x >= 1 && x < X_LENGTH && y >= 0 && y < Y_LENGTH - 2
                                                    && Space[x - 2][y] == Block.Empty
                                                    && Space[x - 1][y] == Block.Empty
                                                    && Space[x - 1][y + 2] == Block.Empty)
                                                {
                                                    Space[x - 2][y] = Block.Floating | Block.LBlock;
                                                    Space[x - 1][y] = Block.Floating | Block.LBlock;
                                                    Space[x - 1][y + 2] = Block.Floating | Block.LBlock;

                                                    Space[x][y] = Block.Empty;
                                                    Space[x][y + 1] = Block.Empty;
                                                    Space[x - 2][y + 1] = Block.Empty;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // a a
                                        //   a
                                        //   a

                                        if (x >= 0 && x < X_LENGTH - 2 && y >= 0 && y < Y_LENGTH - 2
                                            && Space[x][y + 1] == Block.Empty
                                            && Space[x][y + 2] == Block.Empty
                                            && Space[x + 2][y + 1] == Block.Empty)
                                        {
                                            Space[x][y + 1] = Block.Floating | Block.LBlock;
                                            Space[x][y + 2] = Block.Floating | Block.LBlock;
                                            Space[x + 2][y + 1] = Block.Floating | Block.LBlock;

                                            Space[x][y] = Block.Empty;
                                            Space[x + 1][y] = Block.Empty;
                                            Space[x + 1][y + 2] = Block.Empty;
                                        }
                                    }

                                    return;
                                }
                                else if ((Space[x][y] & Block.ReverseL) != Block.Empty)
                                {
                                    if (y < Y_LENGTH - 1 && (Space[x][y + 1] & Block.ReverseL) != Block.Empty)
                                    {
                                        if (y < Y_LENGTH - 2 && (Space[x][y + 2] & Block.ReverseL) != Block.Empty)
                                        {
                                            if (x < X_LENGTH - 1 && (Space[x + 1][y] & Block.ReverseL) != Block.Empty)
                                            {
                                                // a a
                                                // a
                                                // a

                                                if (x >= 1 && x < X_LENGTH - 1 && y >= 0 && y < Y_LENGTH - 1
                                                    && Space[x - 1][y] == Block.Empty
                                                    && Space[x - 1][y + 1] == Block.Empty
                                                    && Space[x + 1][y + 1] == Block.Empty)
                                                {
                                                    Space[x - 1][y] = Block.Floating | Block.ReverseL;
                                                    Space[x - 1][y + 1] = Block.Floating | Block.ReverseL;
                                                    Space[x + 1][y + 1] = Block.Floating | Block.ReverseL;

                                                    Space[x][y] = Block.Empty;
                                                    Space[x + 1][y] = Block.Empty;
                                                    Space[x][y + 2] = Block.Empty;
                                                }
                                            }
                                            else
                                            {
                                                //   a
                                                //   a
                                                // a a

                                                if (x >= 1 && x < X_LENGTH - 1 && y >= 0 && y < Y_LENGTH - 2
                                                    && Space[x - 1][y + 1] == Block.Empty
                                                    && Space[x + 1][y + 1] == Block.Empty
                                                    && Space[x + 1][y + 2] == Block.Empty)
                                                {
                                                    Space[x - 1][y + 1] = Block.Floating | Block.ReverseL;
                                                    Space[x + 1][y + 1] = Block.Floating | Block.ReverseL;
                                                    Space[x + 1][y + 2] = Block.Floating | Block.ReverseL;

                                                    Space[x][y] = Block.Empty;
                                                    Space[x][y + 2] = Block.Empty;
                                                    Space[x - 1][y + 2] = Block.Empty;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // a
                                            // a a a

                                            if (x >= 0 && x < X_LENGTH - 1 && y >= 0 && y < Y_LENGTH
                                                && Space[x + 1][y] == Block.Empty
                                                && Space[x][y + 2] == Block.Empty
                                                && Space[x + 1][y + 2] == Block.Empty)
                                            {
                                                Space[x + 1][y] = Block.Floating | Block.ReverseL;
                                                Space[x][y + 2] = Block.Floating | Block.ReverseL;
                                                Space[x + 1][y + 2] = Block.Floating | Block.ReverseL;

                                                Space[x][y] = Block.Empty;
                                                Space[x][y + 1] = Block.Empty;
                                                Space[x + 2][y + 1] = Block.Empty;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // a a a
                                        //     a

                                        if (x >= 0 && x < X_LENGTH - 1 && y >= 1 && y < Y_LENGTH - 1
                                            && Space[x + 1][y - 1] == Block.Empty
                                            && Space[x + 2][y - 1] == Block.Empty
                                            && Space[x + 1][y + 1] == Block.Empty)
                                        {
                                            Space[x + 1][y - 1] = Block.Floating | Block.ReverseL;
                                            Space[x + 2][y - 1] = Block.Floating | Block.ReverseL;
                                            Space[x + 1][y + 1] = Block.Floating | Block.ReverseL;

                                            Space[x][y] = Block.Empty;
                                            Space[x + 2][y] = Block.Empty;
                                            Space[x + 2][y + 1] = Block.Empty;
                                        }
                                    }

                                    return;
                                }
                                else if ((Space[x][y] & Block.ZBlock) != Block.Empty)
                                {
                                    if (y < Y_LENGTH - 1 && (Space[x][y + 1] & Block.ZBlock) != Block.Empty)
                                    {
                                        //   a
                                        // a a
                                        // a

                                        if (x >= 2 && x < X_LENGTH && y >= 0 && y < Y_LENGTH - 1
                                            && Space[x - 1][y] == Block.Empty
                                            && Space[x - 2][y] == Block.Empty)
                                        {
                                            Space[x - 1][y] = Block.Floating | Block.ZBlock;
                                            Space[x - 2][y] = Block.Floating | Block.ZBlock;

                                            Space[x][y] = Block.Empty;
                                            Space[x - 1][y + 2] = Block.Empty;
                                        }
                                    }
                                    else
                                    {
                                        // a a
                                        //   a a

                                        if (x >= 0 && x < X_LENGTH - 2 && y >= 0 && y < Y_LENGTH - 2
                                            && Space[x + 2][y] == Block.Empty
                                            && Space[x + 1][y + 2] == Block.Empty)
                                        {
                                            Space[x + 2][y] = Block.Floating | Block.ZBlock;
                                            Space[x + 1][y + 2] = Block.Floating | Block.ZBlock;

                                            Space[x][y] = Block.Empty;
                                            Space[x + 1][y] = Block.Empty;
                                        }
                                    }

                                    return;
                                }
                                else if ((Space[x][y] & Block.ReverseZ) != Block.Empty)
                                {
                                    if (x < X_LENGTH - 1 && (Space[x + 1][y] & Block.ReverseZ) != Block.Empty)
                                    {
                                        //   a a
                                        // a a

                                        if (x >= 1 && x < X_LENGTH && y >= 0 && y < Y_LENGTH - 2
                                            && Space[x - 1][y] == Block.Empty
                                            && Space[x - 1][y - 1] == Block.Empty)
                                        {
                                            Space[x - 1][y] = Block.Floating | Block.ReverseZ;
                                            Space[x - 1][y - 1] = Block.Floating | Block.ReverseZ;

                                            Space[x + 1][y] = Block.Empty;
                                            Space[x - 1][y + 1] = Block.Empty;
                                        }
                                    }
                                    else
                                    {
                                        // a
                                        // a a
                                        //   a

                                        if (x >= 0 && x < X_LENGTH - 2 && y >= 0 && y < Y_LENGTH - 1
                                            && Space[x + 2][y + 1] == Block.Empty
                                            && Space[x][y + 2] == Block.Empty)
                                        {
                                            Space[x + 2][y + 1] = Block.Floating | Block.ReverseZ;
                                            Space[x][y + 2] = Block.Floating | Block.ReverseZ;

                                            Space[x][y] = Block.Empty;
                                            Space[x][y + 1] = Block.Empty;
                                        }
                                    }

                                    return;
                                }
                                else if ((Space[x][y] & Block.IBlock) != Block.Empty)
                                {
                                    if (x < X_LENGTH - 1 && (Space[x + 1][y] & Block.IBlock) != Block.Empty)
                                    {
                                        // a a a a

                                        if (x >= 0 && x < X_LENGTH && y >= 1 && y < Y_LENGTH - 2
                                            && Space[x + 1][y - 1] == Block.Empty
                                            && Space[x + 1][y + 1] == Block.Empty
                                            && Space[x + 1][y + 2] == Block.Empty)
                                        {
                                            Space[x + 1][y - 1] = Block.Floating | Block.IBlock;
                                            Space[x + 1][y + 1] = Block.Floating | Block.IBlock;
                                            Space[x + 1][y + 2] = Block.Floating | Block.IBlock;

                                            Space[x][y] = Block.Empty;
                                            Space[x + 2][y] = Block.Empty;
                                            Space[x + 3][y] = Block.Empty;
                                        }
                                    }
                                    else
                                    {
                                        // a
                                        // a 
                                        // a
                                        // a

                                        if (x >= 1 && x < X_LENGTH - 2 && y >= 0 && y < Y_LENGTH
                                            && Space[x - 1][y + 1] == Block.Empty
                                            && Space[x + 1][y + 1] == Block.Empty
                                            && Space[x + 2][y + 1] == Block.Empty)
                                        {
                                            Space[x - 1][y + 1] = Block.Floating | Block.IBlock;
                                            Space[x + 1][y + 1] = Block.Floating | Block.IBlock;
                                            Space[x + 2][y + 1] = Block.Floating | Block.IBlock;

                                            Space[x][y] = Block.Empty;
                                            Space[x][y + 2] = Block.Empty;
                                            Space[x][y + 3] = Block.Empty;
                                        }
                                    }

                                    return;
                                }
                                else if ((Space[x][y] & Block.TBlock) != Block.Empty)
                                {
                                    if (x < X_LENGTH - 1 && (Space[x + 1][y] & Block.TBlock) != Block.Empty)
                                    {
                                        // a a a
                                        //   a

                                        if (x >= 0 && x < X_LENGTH - 1 && y >= 1 && y < Y_LENGTH - 1
                                            && Space[x + 1][y - 1] == Block.Empty)
                                        {
                                            Space[x + 1][y - 1] = Block.Floating | Block.TBlock;

                                            Space[x][y] = Block.Empty;
                                        }
                                    }
                                    else
                                    {
                                        if (y < Y_LENGTH - 2 && (Space[x][y + 2] & Block.TBlock) != Block.Empty)
                                        {
                                            if (x < X_LENGTH - 1 && (Space[x + 1][y + 1] & Block.TBlock) != Block.Empty)
                                            {
                                                // a
                                                // a a
                                                // a

                                                if (x >= 1 && x < X_LENGTH - 1 && y >= 1 && y < Y_LENGTH - 1
                                                    && Space[x - 1][y + 1] == Block.Empty)
                                                {
                                                    Space[x - 1][y + 1] = Block.Floating | Block.TBlock;

                                                    Space[x][y + 2] = Block.Empty;
                                                }
                                            }
                                            else
                                            {
                                                //   a
                                                // a a
                                                //   a

                                                if (x >= 1 && x < X_LENGTH - 1 && y >= 0 && y < Y_LENGTH - 1
                                                    && Space[x + 1][y + 1] == Block.Empty)
                                                {
                                                    Space[x + 1][y + 1] = Block.Floating | Block.TBlock;

                                                    Space[x][y] = Block.Empty;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //   a
                                            // a a a

                                            if (x >= 0 && x < X_LENGTH - 1 && y >= 0 && y < Y_LENGTH - 2
                                                && Space[x][y + 2] == Block.Empty)
                                            {
                                                Space[x][y + 2] = Block.Floating | Block.TBlock;

                                                Space[x + 1][y + 1] = Block.Empty;
                                            }
                                        }
                                    }

                                    return;
                                }
                                else if ((Space[x][y] & Block.BoxBlock) != Block.Empty)
                                {
                                    return;
                                }
                            }
                        }
                    }
                }
                catch (IndexOutOfRangeException ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            else
            {
                try
                {
                    for (int y = 0; y < Y_LENGTH; y++)
                    {
                        for (int x = 0; x < X_LENGTH; x++)
                        {
                            if ((Space[x][y] & Block.Floating) != Block.Empty)
                            {
                                if ((Space[x][y] & Block.LBlock) != Block.Empty)
                                {
                                    if (y < Y_LENGTH - 1 && (Space[x][y + 1] & Block.LBlock) != Block.Empty)
                                    {
                                        if (y < Y_LENGTH - 2 && (Space[x][y + 2] & Block.LBlock) != Block.Empty)
                                        {
                                            // a
                                            // a    
                                            // a a

                                            if (x >= 1 && x < X_LENGTH - 1 && y >= 0 && y < Y_LENGTH - 1
                                                && Space[x + 1][y + 1] == Block.Empty
                                                && Space[x - 1][y + 1] == Block.Empty
                                                && Space[x - 1][y + 2] == Block.Empty)
                                            {
                                                Space[x + 1][y + 1] = Block.Floating | Block.LBlock;
                                                Space[x - 1][y + 1] = Block.Floating | Block.LBlock;
                                                Space[x - 1][y + 2] = Block.Floating | Block.LBlock;

                                                Space[x][y] = Block.Empty;
                                                Space[x][y + 2] = Block.Empty;
                                                Space[x + 1][y + 2] = Block.Empty;
                                            }
                                        }
                                        else
                                        {
                                            if (x < X_LENGTH - 1 && (Space[x + 1][y] & Block.LBlock) != Block.Empty)
                                            {
                                                // a a a
                                                // a

                                                if (x >= 0 && x < X_LENGTH - 1 && y >= 1 && y < Y_LENGTH - 1
                                                    && Space[x + 1][y + 1] == Block.Empty
                                                    && Space[x + 1][y - 1] == Block.Empty
                                                    && Space[x][y - 1] == Block.Empty)
                                                {
                                                    Space[x + 1][y + 1] = Block.Floating | Block.LBlock;
                                                    Space[x + 1][y - 1] = Block.Floating | Block.LBlock;
                                                    Space[x][y - 1] = Block.Floating | Block.LBlock;

                                                    Space[x][y] = Block.Empty;
                                                    Space[x][y + 1] = Block.Empty;
                                                    Space[x + 2][y] = Block.Empty;
                                                }
                                            }
                                            else
                                            {
                                                //     a
                                                // a a a

                                                if (x >= 1 && x < X_LENGTH && y >= 0 && y < Y_LENGTH
                                                    && Space[x - 1][y + 2] == Block.Empty
                                                    && Space[x][y + 2] == Block.Empty
                                                    && Space[x - 1][y] == Block.Empty)
                                                {
                                                    Space[x - 1][y] = Block.Floating | Block.LBlock;
                                                    Space[x][y + 2] = Block.Floating | Block.LBlock;
                                                    Space[x - 1][y + 2] = Block.Floating | Block.LBlock;

                                                    Space[x][y] = Block.Empty;
                                                    Space[x][y + 1] = Block.Empty;
                                                    Space[x - 2][y + 1] = Block.Empty;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // a a
                                        //   a
                                        //   a

                                        if (x >= 0 && x < X_LENGTH - 2 && y >= 0 && y < Y_LENGTH - 1
                                            && Space[x + 2][y] == Block.Empty
                                            && Space[x + 2][y + 1] == Block.Empty
                                            && Space[x][y + 1] == Block.Empty)
                                        {
                                            Space[x][y + 1] = Block.Floating | Block.LBlock;
                                            Space[x + 2][y] = Block.Floating | Block.LBlock;
                                            Space[x + 2][y + 1] = Block.Floating | Block.LBlock;

                                            Space[x][y] = Block.Empty;
                                            Space[x + 1][y] = Block.Empty;
                                            Space[x + 1][y + 2] = Block.Empty;
                                        }
                                    }

                                    return;
                                }
                                else if ((Space[x][y] & Block.ReverseL) != Block.Empty)
                                {
                                    if (y < Y_LENGTH - 1 && (Space[x][y + 1] & Block.ReverseL) != Block.Empty)
                                    {
                                        if (y < Y_LENGTH - 2 && (Space[x][y + 2] & Block.ReverseL) != Block.Empty)
                                        {
                                            if (x < X_LENGTH - 1 && (Space[x + 1][y] & Block.ReverseL) != Block.Empty)
                                            {
                                                // a a
                                                // a
                                                // a

                                                if (x >= 1 && x < X_LENGTH - 2 && y >= 0 && y < Y_LENGTH - 1
                                                    && Space[x + 1][y + 1] == Block.Empty
                                                    && Space[x + 1][y + 2] == Block.Empty
                                                    && Space[x - 1][y + 1] == Block.Empty)
                                                {
                                                    Space[x + 1][y + 1] = Block.Floating | Block.ReverseL;
                                                    Space[x + 1][y + 2] = Block.Floating | Block.ReverseL;
                                                    Space[x - 1][y + 1] = Block.Floating | Block.ReverseL;

                                                    Space[x][y] = Block.Empty;
                                                    Space[x + 1][y] = Block.Empty;
                                                    Space[x][y + 2] = Block.Empty;
                                                }
                                            }
                                            else
                                            {
                                                //   a
                                                //   a
                                                // a a

                                                if (x >= 1 && x < X_LENGTH - 1 && y >= 0 && y < Y_LENGTH - 2
                                                    && Space[x - 1][y] == Block.Empty
                                                    && Space[x - 1][y + 1] == Block.Empty
                                                    && Space[x + 1][y + 1] == Block.Empty)
                                                {
                                                    Space[x - 1][y] = Block.Floating | Block.ReverseL;
                                                    Space[x - 1][y + 1] = Block.Floating | Block.ReverseL;
                                                    Space[x + 1][y + 1] = Block.Floating | Block.ReverseL;

                                                    Space[x][y] = Block.Empty;
                                                    Space[x][y + 2] = Block.Empty;
                                                    Space[x - 1][y + 2] = Block.Empty;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // a
                                            // a a a

                                            if (x >= 0 && x < X_LENGTH - 2 && y >= 0 && y < Y_LENGTH - 2
                                                && Space[x + 1][y] == Block.Empty
                                                && Space[x + 2][y] == Block.Empty
                                                && Space[x + 1][y + 2] == Block.Empty)
                                            {
                                                Space[x + 1][y] = Block.Floating | Block.ReverseL;
                                                Space[x + 2][y] = Block.Floating | Block.ReverseL;
                                                Space[x + 1][y + 2] = Block.Floating | Block.ReverseL;

                                                Space[x][y] = Block.Empty;
                                                Space[x][y + 1] = Block.Empty;
                                                Space[x + 2][y + 1] = Block.Empty;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // a a a
                                        //     a

                                        if (x >= 0 && x < X_LENGTH - 2 && y >= 1 && y < Y_LENGTH - 1
                                            && Space[x + 1][y - 1] == Block.Empty
                                            && Space[x][y + 1] == Block.Empty
                                            && Space[x + 1][y + 1] == Block.Empty)
                                        {
                                            Space[x + 1][y - 1] = Block.Floating | Block.ReverseL;
                                            Space[x][y + 1] = Block.Floating | Block.ReverseL;
                                            Space[x + 1][y + 1] = Block.Floating | Block.ReverseL;

                                            Space[x][y] = Block.Empty;
                                            Space[x + 2][y] = Block.Empty;
                                            Space[x + 2][y + 1] = Block.Empty;
                                        }
                                    }

                                    return;
                                }
                                else if ((Space[x][y] & Block.ZBlock) != Block.Empty)
                                {
                                    if (y < Y_LENGTH - 1 && (Space[x][y + 1] & Block.ZBlock) != Block.Empty)
                                    {
                                        //   a
                                        // a a
                                        // a

                                        if (x >= 2 && x < X_LENGTH && y >= 0 && y < Y_LENGTH - 1
                                            && Space[x - 1][y] == Block.Empty
                                            && Space[x - 2][y] == Block.Empty)
                                        {
                                            Space[x - 1][y] = Block.Floating | Block.ZBlock;
                                            Space[x - 2][y] = Block.Floating | Block.ZBlock;

                                            Space[x][y] = Block.Empty;
                                            Space[x - 1][y + 2] = Block.Empty;
                                        }
                                    }
                                    else
                                    {
                                        // a a
                                        //   a a

                                        if (x >= 0 && x < X_LENGTH - 2 && y >= 0 && y < Y_LENGTH - 2
                                            && Space[x + 2][y] == Block.Empty
                                            && Space[x + 1][y + 2] == Block.Empty)
                                        {
                                            Space[x + 2][y] = Block.Floating | Block.ZBlock;
                                            Space[x + 1][y + 2] = Block.Floating | Block.ZBlock;

                                            Space[x][y] = Block.Empty;
                                            Space[x + 1][y] = Block.Empty;
                                        }
                                    }

                                    return;
                                }
                                else if ((Space[x][y] & Block.ReverseZ) != Block.Empty)
                                {
                                    if (x < X_LENGTH - 1 && (Space[x + 1][y] & Block.ReverseZ) != Block.Empty)
                                    {
                                        //   a a
                                        // a a

                                        if (x >= 1 && x < X_LENGTH && y >= 0 && y < Y_LENGTH - 2
                                            && Space[x - 1][y] == Block.Empty
                                            && Space[x - 1][y - 1] == Block.Empty)
                                        {
                                            Space[x - 1][y] = Block.Floating | Block.ReverseZ;
                                            Space[x - 1][y - 1] = Block.Floating | Block.ReverseZ;

                                            Space[x + 1][y] = Block.Empty;
                                            Space[x - 1][y + 1] = Block.Empty;
                                        }
                                    }
                                    else
                                    {
                                        // a
                                        // a a
                                        //   a

                                        if (x >= 0 && x < X_LENGTH - 2 && y >= 0 && y < Y_LENGTH - 1
                                            && Space[x + 2][y + 1] == Block.Empty
                                            && Space[x][y + 2] == Block.Empty)
                                        {
                                            Space[x + 2][y + 1] = Block.Floating | Block.ReverseZ;
                                            Space[x][y + 2] = Block.Floating | Block.ReverseZ;

                                            Space[x][y] = Block.Empty;
                                            Space[x][y + 1] = Block.Empty;
                                        }
                                    }

                                    return;
                                }
                                else if ((Space[x][y] & Block.IBlock) != Block.Empty)
                                {
                                    if (x < X_LENGTH - 1 && (Space[x + 1][y] & Block.IBlock) != Block.Empty)
                                    {
                                        // a a a a

                                        if (x >= 0 && x < X_LENGTH && y >= 1 && y < Y_LENGTH - 2
                                            && Space[x + 1][y - 1] == Block.Empty
                                            && Space[x + 1][y + 1] == Block.Empty
                                            && Space[x + 1][y + 2] == Block.Empty)
                                        {
                                            Space[x + 1][y - 1] = Block.Floating | Block.IBlock;
                                            Space[x + 1][y + 1] = Block.Floating | Block.IBlock;
                                            Space[x + 1][y + 2] = Block.Floating | Block.IBlock;

                                            Space[x][y] = Block.Empty;
                                            Space[x + 2][y] = Block.Empty;
                                            Space[x + 3][y] = Block.Empty;
                                        }
                                    }
                                    else
                                    {
                                        // a
                                        // a 
                                        // a
                                        // a

                                        if (x >= 1 && x < X_LENGTH - 2 && y >= 0 && y < Y_LENGTH
                                            && Space[x - 1][y + 1] == Block.Empty
                                            && Space[x + 1][y + 1] == Block.Empty
                                            && Space[x + 2][y + 1] == Block.Empty)
                                        {
                                            Space[x - 1][y + 1] = Block.Floating | Block.IBlock;
                                            Space[x + 1][y + 1] = Block.Floating | Block.IBlock;
                                            Space[x + 2][y + 1] = Block.Floating | Block.IBlock;

                                            Space[x][y] = Block.Empty;
                                            Space[x][y + 2] = Block.Empty;
                                            Space[x][y + 3] = Block.Empty;
                                        }
                                    }

                                    return;
                                }
                                else if ((Space[x][y] & Block.TBlock) != Block.Empty)
                                {
                                    if (x < X_LENGTH - 1 && (Space[x + 1][y] & Block.TBlock) != Block.Empty)
                                    {
                                        // a a a
                                        //   a

                                        if (x >= 0 && x < X_LENGTH - 1 && y >= 1 && y < Y_LENGTH - 1
                                            && Space[x + 1][y - 1] == Block.Empty)
                                        {
                                            Space[x + 1][y - 1] = Block.Floating | Block.TBlock;

                                            Space[x + 2][y] = Block.Empty;
                                        }
                                    }
                                    else
                                    {
                                        if (y < Y_LENGTH - 2 && (Space[x][y + 2] & Block.TBlock) != Block.Empty)
                                        {
                                            if (x < X_LENGTH - 1 && (Space[x + 1][y + 1] & Block.TBlock) != Block.Empty)
                                            {
                                                // a
                                                // a a
                                                // a

                                                if (x >= 1 && x < X_LENGTH - 1 && y >= 1 && y < Y_LENGTH - 1
                                                    && Space[x - 1][y + 1] == Block.Empty)
                                                {
                                                    Space[x - 1][y + 1] = Block.Floating | Block.TBlock;

                                                    Space[x][y] = Block.Empty;
                                                }
                                            }
                                            else
                                            {
                                                //   a
                                                // a a
                                                //   a

                                                if (x >= 1 && x < X_LENGTH - 1 && y >= 0 && y < Y_LENGTH - 1
                                                    && Space[x + 1][y + 1] == Block.Empty)
                                                {
                                                    Space[x + 1][y + 1] = Block.Floating | Block.TBlock;

                                                    Space[x][y + 2] = Block.Empty;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //   a
                                            // a a a

                                            if (x >= 0 && x < X_LENGTH - 1 && y >= 0 && y < Y_LENGTH - 2
                                                && Space[x][y + 2] == Block.Empty)
                                            {
                                                Space[x][y + 2] = Block.Floating | Block.TBlock;

                                                Space[x - 1][y + 1] = Block.Empty;
                                            }
                                        }
                                    }

                                    return;
                                }
                                else if ((Space[x][y] & Block.BoxBlock) != Block.Empty)
                                {
                                    return;
                                }
                            }
                        }
                    }
                }
                catch (IndexOutOfRangeException ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }
        /// <summary>
        /// 다음 블록이 뭔지 보여주는 메서드
        /// </summary>
        private void NextBlockView()
        {
            switch (NextList[0] ^ Block.Floating)
            {
                case Block.LBlock: NextBlockViewer.BackgroundImage = Properties.Tetris.L; break;
                case Block.ReverseL: NextBlockViewer.BackgroundImage = Properties.Tetris.Lr; break;
                case Block.ZBlock: NextBlockViewer.BackgroundImage = Properties.Tetris.Z; break;
                case Block.ReverseZ: NextBlockViewer.BackgroundImage = Properties.Tetris.Zr; break;
                case Block.BoxBlock: NextBlockViewer.BackgroundImage = Properties.Tetris.Box; break;
                case Block.TBlock: NextBlockViewer.BackgroundImage = Properties.Tetris.T; break;
                case Block.IBlock: NextBlockViewer.BackgroundImage = Properties.Tetris.I; break;
            }
        }

        /// <summary>
        /// 블록의 종류, 공간은 0 ~ 2 | 4 ~ 256으로 구성됨
        /// </summary>
        private enum Block
        {
            Empty = 0,
            Floating = 1,
            Building = 2,

            LBlock = 4,
            ReverseL = 8,
            ZBlock = 16,
            ReverseZ = 32,
            TBlock = 64,
            BoxBlock = 128,
            IBlock = 256,
        }

        protected override void End()
        {
        }

        protected override void Replace()
        {
            lock (locker)
            {
                for (int x = 0; x < X_LENGTH; x++)
                {
                    for (int y = Y_LENGTH - 1; y >= 0; y--)
                    {
                        if ((Space[x][y] & Block.Floating) != Block.Empty)
                        {
                            // 블록이 하나라도 공중에 있다면 전체 다운하고 탈출
                            DownBlock(x, y);
                            goto OUT;
                        }
                    }
                }

                // 여기까지 왔으면 공중 블록이 없음 => 생성
                TimerInterval--;
                Block now = PopBlock();
                NextBlockView();
                CreateBlock(now ^ Block.Floating);
            OUT:

                // 그래픽 디자이너
                new Thread(GraphicDesign).Start();
            }
        }
        /// <summary>
        /// 빠르게 다운
        /// </summary>
        private void SpeedDown()
        {
            lock (locker)
            {
            TOP:
                for (int x = 0; x < X_LENGTH; x++)
                {
                    for (int y = Y_LENGTH - 1; y >= 0; y--)
                    {
                        if ((Space[x][y] & Block.Floating) != Block.Empty)
                        {
                            // 블록이 하나라도 공중에 있다면 전체 다운하고 탈출
                            if (!DownBlock(x, y))
                            {
                                goto TOP;
                            }
                            else
                            {
                                goto OUT;
                            }
                        }
                    }
                }

                // 여기까지 왔으면 공중 블록이 없음 => 생성
                TimerInterval--;
                Block now = PopBlock();
                NextBlockView();
                CreateBlock(now ^ Block.Floating);
            OUT:

                // 그래픽 디자이너
                new Thread(GraphicDesign).Start();
            }
        }


        protected override void Start()
        {
            // 최초 블록 생성
            Block now = PopBlock();
            CreateBlock(now ^ Block.Floating);
            NextBlockView();
        }

        private bool waiter = false;
        private object locker = new object();
        protected override void SystemKeyDown(Keys key)
        {
            switch (key)
            {
                case Keys.Left: LeftMove(); break;
                case Keys.Right: RightMove(); break;
                case Keys.Down: Replace(); break;
                case Keys.Up: SpeedDown(); break;
                case Keys.A: Spin(true); break;
                case Keys.S: Spin(false); break;
                case Keys.W: Wait(waiter = !waiter); break;
            }

            new Thread(GraphicDesign).Start();
        }

    }
}
