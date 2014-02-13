using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gravitymania.input;

namespace gravitymania.main
{
	public interface GameState
	{
		void Begin();
		void End();
		void Suspend();
		void Resume();
        void Input(InputState state);
		void Update();
		void Draw();
	}
}
