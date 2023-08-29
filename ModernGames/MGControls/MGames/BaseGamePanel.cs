using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timer = System.Windows.Forms.Timer;

namespace ModernGames.MGControls.MGames
{
    /// <summary>
    /// 모든 게임의 베이스가 되는 추상 게임 패널
    /// </summary>
    public abstract class BaseGamePanel : Panel
    {
        public new string Name { get; protected set; } = "";
        public string Description { get; protected set; } = "";

        public Timer Timer { get; private set; } = new Timer() 
        {
            Interval = 10,
        };

        /// <summary>
        /// 게임을 시작, Start 후 Replace를 Tick 시간마다 호출
        /// </summary>
        public void Run()
        {
            Start();

            this.Timer.Tick += (s, e) =>
            {
                Replace();
            };
            this.Timer.Start();
        }

        /// <summary>
        /// Replace를 멈추고 End를 호출
        /// </summary>
        public void Stop()
        {
            this.Timer.Stop();
            End();
        }

        protected abstract void Start();
        protected abstract void Replace();
        protected abstract void End();
    }
}
