using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernGames.MGControls.MGames
{
    public class Tetris : TimerBaseGamePanel
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
        private const int BlockCount = 7;
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
        /// 테트리스 게임 시작 생성자
        /// </summary>
        /// <param name="level"> 게임의 난이도(0~99) </param>
        public Tetris(int level) : base("Tetris", "The Modern Tetris", (100 - level) * 5)
        {
            this.Space = new List<List<Block>>();
            for (int x = 0; x < X_LENGTH; x++)
            {
                this.Space.Add(new List<Block>(new Block[Y_LENGTH]));
            }
            this.NextList = new List<Block>();

            this.test = new RichTextBox()
            {
                Parent = this,
                Visible = true,
                AutoSize = true,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                Font = new Font("a", 5, FontStyle.Bold),
            };
            this.test.KeyDown += (s, e) =>
            {
                switch (e.KeyCode)
                {
                    case Keys.A: LeftMove(); break;
                    case Keys.D: RightMove(); break;
                }
            };
        } 

        /// <summary>
        /// NextList의 원소가 없는 경우 하나씩 채움
        /// </summary>
        private void NextListSetting()
        {
            if (this.NextList.Count != 0)
            {
                Debug.WriteLine("다음 블록 리스트가 비지 않았는데 리스트를 채우려 했습니다.");
                return;
            }

            List<Block> list = new List<Block>(new Block[BlockCount]);
            
            for (int i = 0; i < BlockCount; i++)
            {
                list[i] = Block.Floating | (Block)(4 * Math.Pow(2, i));
            }

            int n = list.Count;

            while (n > 1)
            {
                n--;
                int k = new Random(DateTime.Now.Millisecond).Next(n + 1);
                Block value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            this.NextList = list;
        }
        /// <summary>
        /// NextList에서 요소 하나를 반환하고, 만약 리스트가 비었다면 새로 채움
        /// </summary>
        /// <returns></returns>
        private Block PopBlock()
        {
            if (this.NextList.Count == 0)
            {
                NextListSetting();
            }

            Block now = this.NextList[0];
            this.NextList.RemoveAt(0);

            return now;
        }
        /// <summary>
        /// 블록 생성 메서드
        /// </summary>
        /// <param name="block"> 생성할 새 블록 </param>
        private void CreateBlock(Block block)
        {
            switch (block)
            {
                case Block.ZBlock:
                    this.Space[SpawnLocation.X][SpawnLocation.Y] = Block.Floating | block;
                    this.Space[SpawnLocation.X + 1][SpawnLocation.Y] = Block.Floating | block;
                    this.Space[SpawnLocation.X + 1][SpawnLocation.Y + 1] = Block.Floating | block;
                    this.Space[SpawnLocation.X + 2][SpawnLocation.Y + 1] = Block.Floating | block;
                    break;
                case Block.ReverseZ:
                    this.Space[SpawnLocation.X][SpawnLocation.Y + 1] = Block.Floating | block;
                    this.Space[SpawnLocation.X + 1][SpawnLocation.Y + 1] = Block.Floating | block;
                    this.Space[SpawnLocation.X + 1][SpawnLocation.Y] = Block.Floating | block;
                    this.Space[SpawnLocation.X + 2][SpawnLocation.Y] = Block.Floating | block;
                    break;
                case Block.TBlock:
                    this.Space[SpawnLocation.X][SpawnLocation.Y + 1] = Block.Floating | block;
                    this.Space[SpawnLocation.X + 1][SpawnLocation.Y + 1] = Block.Floating | block;
                    this.Space[SpawnLocation.X + 2][SpawnLocation.Y + 1] = Block.Floating | block;
                    this.Space[SpawnLocation.X + 1][SpawnLocation.Y] = Block.Floating | block;
                    break;
                case Block.BoxBlock:
                    this.Space[SpawnLocation.X][SpawnLocation.Y] = Block.Floating | block;
                    this.Space[SpawnLocation.X][SpawnLocation.Y + 1] = Block.Floating | block;
                    this.Space[SpawnLocation.X + 1][SpawnLocation.Y] = Block.Floating | block;
                    this.Space[SpawnLocation.X + 1][SpawnLocation.Y + 1] = Block.Floating | block;
                    break;
                case Block.IBlock:
                    this.Space[SpawnLocation.X][SpawnLocation.Y] = Block.Floating | block;
                    this.Space[SpawnLocation.X + 1][SpawnLocation.Y] = Block.Floating | block;
                    this.Space[SpawnLocation.X + 2][SpawnLocation.Y] = Block.Floating | block;
                    this.Space[SpawnLocation.X + 3][SpawnLocation.Y] = Block.Floating | block;
                    break;
                case Block.LBlock:
                    this.Space[SpawnLocation.X][SpawnLocation.Y] = Block.Floating | block;
                    this.Space[SpawnLocation.X + 1][SpawnLocation.Y] = Block.Floating | block;
                    this.Space[SpawnLocation.X + 2][SpawnLocation.Y] = Block.Floating | block;
                    this.Space[SpawnLocation.X][SpawnLocation.Y + 1] = Block.Floating | block;
                    break;
                case Block.ReverseL:
                    this.Space[SpawnLocation.X][SpawnLocation.Y] = Block.Floating | block;
                    this.Space[SpawnLocation.X + 1][SpawnLocation.Y] = Block.Floating | block;
                    this.Space[SpawnLocation.X + 2][SpawnLocation.Y] = Block.Floating | block;
                    this.Space[SpawnLocation.X + 2][SpawnLocation.Y + 1] = Block.Floating | block;
                    break;
            }
        }
        /// <summary>
        /// 특정 좌표 주위 4칸 내의 모든 Floating 블록을 Build시킴
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void ChangeBuilding(int x, int y)
        {
            for (int xx = x - 3; xx <  x + 4; xx++) 
            {
                for (int yy = y - 3; yy < y + 4; yy++)
                {
                    if (xx >= 0 && yy >= 0 && xx < X_LENGTH && yy < Y_LENGTH)
                    {
                        if ((this.Space[xx][yy] & Block.Floating) != Block.Empty)
                        {
                            this.Space[xx][yy] ^= Block.Floating;
                            this.Space[xx][yy] ^= Block.Building;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 특정 좌표 주위 4칸 내의 모든 Floating 블록을 다운시키다가 Building 블록을 만나면 ChangeBuilding 호출
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void DownBlock(int x, int y)
        {
            // 블록이 빌딩될 건지 확인
            for (int xx = x - 3; xx < x + 4; xx++)
            {
                for (int yy = y + 3; yy > y - 4; yy--)
                {
                    if (xx >= 0 && yy >= 0 && xx < X_LENGTH && yy + 1 <= Y_LENGTH)
                    {
                        if ((this.Space[xx][yy] & Block.Floating) != Block.Empty 
                            && (this.Space[xx][yy] & Block.Building) != Block.Building)
                        {
                            if (yy + 1 == Y_LENGTH || (this.Space[xx][yy + 1] & Block.Building) != Block.Empty)
                            {
                                ChangeBuilding(xx, yy);
                                return;
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
                        if ((this.Space[xx][yy] & Block.Floating) != Block.Empty)
                        {
                            this.Space[xx][yy + 1] = this.Space[xx][yy];
                            this.Space[xx][yy] = Block.Empty;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 해당 높이의 라인을 모두 삭제
        /// </summary>
        /// <param name="y"></param>
        private void LineRemove(int y)
        {
            for (int i = 0; i < X_LENGTH; i++)
            {
                this.Space[i].RemoveAt(y);
            }
            for (int yy = y - 1; yy > 0; yy--)
            {
                for (int xx = 0; xx < X_LENGTH; xx++)
                {
                    this.Space[xx][yy + 1] = this.Space[xx][yy];
                    this.Space[xx][yy] = Block.Empty;
                }
            }
        }
        /// <summary>
        /// 좌로 밀착
        /// </summary>
        private void LeftMove()
        {
        }
        /// <summary>
        /// 우로 밀착
        /// </summary>
        private void RightMove()
        {
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
            // 데스라인
            for (int x = 0; x < X_LENGTH; x++)
            {
                if ((this.Space[x][DEATH_LINE] & Block.Building) != Block.Empty)
                {
                    Close();
                }
            }

            // 라인완성
            for (int y = 0; y < Y_LENGTH; y++)
            {
                bool line_clear = true;
                for (int x = 0; x < X_LENGTH; x++)
                {
                    line_clear &= (this.Space[x][y] & Block.Building) != Block.Empty;
                }
                if (line_clear)
                {
                    LineRemove(y);
                    y--;
                }
            }

            for (int x = 0; x < X_LENGTH; x++)
            {
                for (int y = Y_LENGTH - 1; y >= 0; y--)
                {
                    if ((this.Space[x][y] & Block.Floating) != Block.Empty)
                    {
                        // 블록이 하나라도 공중에 있다면 전체 다운하고 탈출
                        DownBlock(x, y);
                        goto OUT;
                    }
                }
            }
            // 여기까지 왔으면 공중 블록이 없음 => 생성
            Block now = PopBlock();
            CreateBlock(now ^ Block.Floating);
        OUT:

            // 테스터
            test.Text = "";
            for (int y = 0; y < Y_LENGTH; y++)
            {
                bool line_clear = true;
                for (int x = 0; x < X_LENGTH; x++)
                {
                    test.Text += (this.Space[x][y] == Block.Empty ? "□" : "■");
                    line_clear &= (this.Space[x][y] & Block.Building) != Block.Empty;
                }
                test.Text += ("\r\n");
            }
        }

        protected override void Start()
        {
            // 최초 블록 생성
            Block now = PopBlock();
            CreateBlock(now ^ Block.Floating);
        }

        private RichTextBox test;
    }
}
