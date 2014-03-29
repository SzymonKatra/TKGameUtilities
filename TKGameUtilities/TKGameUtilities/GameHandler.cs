using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKGameUtilities.Graphics;

namespace TKGameUtilities
{
    public abstract class GameHandler
    {
        #region Constructors
        #endregion

        #region Properties
        private GameTime m_gameTime;

        private bool m_running = false;
        public bool Running
        {
            get { return m_running; }
        }

        private int m_fps;
        public int Fps
        {
            get { return m_fps; }
        }

        private int m_ups;
        public int Ups
        {
            get { return m_ups; }
        }

        private long m_frameDelay;
        public TimeSpan FrameDelay
        {
            get { return TimeSpan.FromTicks(m_frameDelay); }
            set { m_frameDelay = value.Ticks; }
        }

        private int m_maxSkippedFrames;
        public int MaxSkippedFrames
        {
            get { return m_maxSkippedFrames; }
            set { m_maxSkippedFrames = value; }
        }
        #endregion

        #region Methods
        public void Run(bool isFixedTimestep)
        {
            m_running = true;
            if (isFixedTimestep)
            {
                m_gameTime = new GameTime();
                m_gameTime.DeltaTime = TimeSpan.FromTicks(m_frameDelay);
                m_gameTime.SkippedFrames = 0;

                FixedTimestepLoop();
            }
            else
            {
                // TODO: variable time-step
            }
        }
        public void Exit()
        {
            m_running = false;
        }
        public void Tick(GameTime gameTime)
        {
            Update(gameTime);
            Draw(gameTime);
        }

        private void FixedTimestepLoop()
        {
            int fpsCount = 0;
            int upsCount = 0;
            long currentTime = DateTime.Now.Ticks;
            long nextTime = DateTime.Now.Ticks;
            long lastFpsCheck = DateTime.Now.Ticks;      
            int skippedFrames = 0;

            LoopEntered();

            while (m_running)
            {
                currentTime = DateTime.Now.Ticks;
                if (lastFpsCheck + TimeSpan.TicksPerSecond <= currentTime)
                {
                    lastFpsCheck = currentTime;
                    m_fps = fpsCount;
                    m_ups = upsCount;
                    fpsCount = 0;
                    upsCount = 0;
                }
                if (currentTime >= nextTime)
                {
                    nextTime += m_frameDelay;
                    Update(m_gameTime);
                    ++upsCount;
                    if ((currentTime < nextTime) || (skippedFrames >= m_maxSkippedFrames))
                    {
                        Draw(m_gameTime);
                        ++fpsCount;
                        skippedFrames = 0;
                    }
                    else
                    {
                        ++skippedFrames;
                        m_gameTime.SkippedFrames = skippedFrames;
                    }
                }
            }

            LoopExited();
        }

        protected abstract void Update(GameTime gameTime);
        protected abstract void Draw(GameTime gameTime);
        
        protected abstract void LoopEntered();
        protected abstract void LoopExited();
        #endregion
    }
}
