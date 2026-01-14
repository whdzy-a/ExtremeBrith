using ExtremeBrith.Object.Ball;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static ExtremeBrith.Slugcat.DeathKnell.DeathKnell_Hook;

namespace ExtremeBrith.Slugcat.DeathKnell
{
    public class DeathKnellModuleManeger
    {
        public static readonly ConditionalWeakTable<Player, PlayerModule> PlayerModules = new ConditionalWeakTable<Player, PlayerModule>();


        public class PlayerModule
        {
            WeakReference<Player> playerRef;

            public PlayerModule(Player player)
            {
                playerRef = new WeakReference<Player>(player);
            }

            public BoneBall ConnectedBall;
            public AbstractObjectOnTail BallStick;


        }
    }
}
