using ExtremeBrith.Object.Ball;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ExtremeBrith.Slugcat.DeathKnell.DeathKnell_Hook;

namespace ExtremeBrith.Slugcat.Recursion
{
    public class RecursionModuleManeger
    {
        public static readonly ConditionalWeakTable<AbstractCreature, PlayerModule> PlayerModules = new ConditionalWeakTable<AbstractCreature, PlayerModule>();


        public class PlayerModule
        {
            WeakReference<AbstractCreature> playerRef;

            public PlayerModule(AbstractCreature abstractPlayer)
            {
                playerRef = new WeakReference<AbstractCreature>(abstractPlayer);
            }

            public int splittingCounter;
            public AbstractCreature Host;
            public List<AbstractCreature> Splitttings = new List<AbstractCreature>();
            public Vector2 ChoosePos;
            public bool ChooseMode;
            public bool isChoosed;


        }
    }
}
