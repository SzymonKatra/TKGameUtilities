using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TKGameUtilities
{
    public class GameTime
    {
        private TimeSpan m_deltaTime;
        private int m_skippedFrames;

        public TimeSpan DeltaTime
        {
            get { return m_deltaTime; }
            protected internal set { m_deltaTime = value; } 
        }
        public int SkippedFrames
        {
            get { return m_skippedFrames; }
            set { m_skippedFrames = value; }
        }
    }
}
