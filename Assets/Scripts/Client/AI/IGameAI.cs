using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 

namespace Client
{
    public interface IGameAI
    {
        bool Activate(AutomatedGame game);
        void Deactivate();
        void Pause();
        void Resume();
        void Update();
        bool AllowPause();
    }
}
