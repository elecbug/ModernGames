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
        private int X_LENGTH = 10;
        /// <summary>
        /// 게임판 세로 길이 (실제로는 20, 데스 존 위의 Floating 공간을 위함)
        /// </summary>
        private int Y_LENGTH = 25;
        /// <summary>
        /// 데스 라인, 이 위에 블럭이 Building되면 게임 종료
        /// </summary>
        private int DEATH_LINE = 20;
        /// <summary>
        /// 블럭의 종류
        /// </summary>
        private int BlockCount = 7;

        /// <summary>
        /// 게임이 진행되는 2차원 공간
        /// </summary>
        private Block[,] Space;
        /// <summary>
        /// 다음 블록들의 리스트, 7개를 각각 하나씩 채우며 진행, 모두 비었을 때 다시 채움
        /// </summary>
        private List<Block> NextList;

        /// <summary>
        /// 테트리스 게임 시작 생성자
        /// </summary>
        /// <param name="level"> 게임의 난이도(0~99) </param>
        public Tetris(int level) : base("Tetris", "The Modern Tetris", 100 - level)
        {
            this.Space = new Block[X_LENGTH, Y_LENGTH];
            this.NextList = new List<Block>();
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
                list[i] = Block.Building | (Block)(4 * Math.Pow(2, i));
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
            throw new NotImplementedException();
        }

        protected override void Replace()
        {
            throw new NotImplementedException();
        }

        protected override void Start()
        {
            throw new NotImplementedException();
        }
    }
}
