using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gravitymania.game
{
    public class FrameAdvanceManager
    {
        public bool IsPaused { get; set; }
        public uint FrameSkip { get; set; }

        public FrameAdvanceManager()
        {
            IsPaused = false;
            AdvanceFrame = false;
            FrameSkip = 0;
            FrameCounter = 0;
        }

        public void Pause()
        {
            IsPaused = true;
        }

        public void UnPause()
        {
            IsPaused = false;
        }

        public bool ShouldUpdateThisFrame()
        {
            return (!IsPaused && FrameCounter == 0) || AdvanceFrame;
        }

        public void FrameAdvance()
        {
            IsPaused = true;
            AdvanceFrame = true;
        }

        public void Update()
        {
            ++FrameCounter;

            if (FrameCounter >= FrameSkip)
            {
                FrameCounter = 0;
            }

            AdvanceFrame = false;
        }


        private uint FrameCounter;
        private bool AdvanceFrame;
    }
}
