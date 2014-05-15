using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gravitymania.main;
using gravitymania.input;

namespace gravitymania.mapedit
{
    class MapEditor : GameState
    {
        private GameRoot Root;
        private GUIEventManager InputManager;

        public MapEditor(GameRoot root)
        {
            Root = root;
            InputManager = new GUIEventManager();
        }

        public void Begin()
        {
        }

        public void End()
        {
        }

        public void Suspend()
        {
        }

        public void Resume()
        {
            InputManager.Flush();
        }

        public void Input(InputState state)
        {
            InputManager.Update(state);
        }

        public void Update()
        {

        }

        public void Draw()
        {

        }
    }
}
