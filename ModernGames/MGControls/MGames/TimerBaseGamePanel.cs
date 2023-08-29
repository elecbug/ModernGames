using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timer = System.Windows.Forms.Timer;

namespace ModernGames.MGControls.MGames
{
    /// <summary>
    /// 모든 실시간 게임의 베이스가 되는 추상 게임 패널
    /// </summary>
    public abstract class TimerBaseGamePanel : Panel
    {
        public new string Name { get; protected set; }
        public string Description { get; protected set; }

        public Timer Timer { get; private set; }

        /// <summary>
        /// 실시간 게임 생성자
        /// </summary>
        /// <param name="name"> 표시되는 게임 이름 </param>
        /// <param name="description"> 게임 설명 문구 </param>
        /// <param name="interval"> 프레임이자 게임의 진행 스피드 </param>
        public TimerBaseGamePanel(string name, string description, int interval)
        {
            this.Name = name;
            this.Description = description;
            this.Timer = new Timer() 
            {
                Interval = interval,
            };
        }

        /// <summary>
        /// 게임을 시작, Start 후 Replace를 Tick 시간마다 호출
        /// </summary>
        public void Open()
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
        public void Close()
        {
            this.Timer.Stop();
            End();
        }

        /// <summary>
        /// 게임을 멈추거나 재시작
        /// </summary>
        /// <param name="wait"> true라면 정지, false라면 재시작 </param>
        public void Wait(bool wait)
        {
            if (wait)
                this.Timer.Stop();
            else
                this.Timer.Start();
        }

        /// <summary>
        /// 게임이 최초 시작될 때 한번 호출되는 메서드
        /// </summary>
        protected abstract void Start();
        /// <summary>
        /// Timer의 Interval이 지날 때마다 호출되는 메서드
        /// </summary>
        protected abstract void Replace();
        /// <summary>
        /// 게임을 끝낼 때 한번 호출되는 메서드
        /// </summary>
        protected abstract void End();
    }
}
