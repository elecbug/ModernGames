using ModernGames.MyForm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernGames.MGControls.MGames
{
    /// <summary>
    /// 모든 실시간 게임의 베이스가 되는 추상 게임 패널
    /// </summary>
    public abstract class TimerGameBasePanel : Panel
    {
        public new string Name { get; protected set; }
        public string Description { get; protected set; }

        /// <summary>
        /// 키 인풋을 체크하는 스레드
        /// </summary>
        protected Thread KeyChecker { get; private set; }
        private object locker1, locker2;
        private bool roof1, roof2;

        /// <summary>
        /// 게임 진행 타이머 스레드
        /// </summary>
        public Thread Timer { get; private set; }
        /// <summary>
        /// 키 인식 간격
        /// </summary>
        protected int KeyInterval { get; set; }
        /// <summary>
        /// 메인 스레드의 간격
        /// </summary>
        public int TimerInterval { get; protected set; }

        /// <summary>
        /// 실시간 게임 생성자
        /// </summary>
        /// <param name="name"> 표시되는 게임 이름 </param>
        /// <param name="description"> 게임 설명 문구 </param>
        /// <param name="interval"> 프레임이자 게임의 진행 스피드 </param>
        public TimerGameBasePanel(string name, string description, int interval)
        {
            this.KeyInterval = 200;
            this.Name = name;
            this.Description = description;
            this.locker1 = new object();
            this.locker2 = new object();
            this.TimerInterval = interval;
            this.Timer = new Thread(()=> 
            {
                this.roof2 = true;
                while (true)
                {
                    lock (this.locker1)
                    {
                        if (!this.roof1)
                        {
                            break;
                        }
                    }
                    lock (this.locker2)
                    {
                        if (!this.roof2)
                        {
                            continue;
                        }
                    }
                    Replace(); 
                    Task.Delay(this.TimerInterval).Wait();
                }
            });
            this.KeyChecker = new Thread(() =>
            {
                this.roof1 = true;

                while (true)
                {
                    lock (this.locker1)
                    {
                        if (!this.roof1) 
                        {
                            break;
                        }
                    }
                    foreach (Keys key in Program.KeySet)
                    {
                        if (Program.IsKeyDown(key))
                        {
                            SystemKeyDown(key);
                            Task.Delay(this.KeyInterval).Wait();
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 게임을 시작, Start 후 Replace를 Tick 시간마다 호출
        /// </summary>
        public void Open()
        {
            Start();

            this.KeyChecker.Start();
            this.Timer.Start();
        }

        /// <summary>
        /// Replace를 멈추고 End를 호출
        /// </summary>
        public void Close()
        {
            lock (this.locker2)
            {
                this.roof2 = false;
            }

            lock (this.locker1)
            {
                this.roof1 = false;
            }
            End();
        }

        /// <summary>
        /// 게임을 멈추거나 재시작
        /// </summary>
        /// <param name="wait"> true라면 정지, false라면 재시작 </param>
        public void Wait(bool wait)
        {
            if (wait)
            {
                lock (this.locker2)
                {
                    this.roof2 = false;
                }
            }
            else
            {
                lock (this.locker2)
                {
                    this.roof2 = true;
                }
            }
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
        /// <summary>
        /// 미리 예약된 몇몇 키 누름 시 호출 
        /// </summary>
        /// <param name="key"></param>
        protected abstract void SystemKeyDown(Keys key);
    }
}
