using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtremeBrith.Object
{
    public class UnlockItemHook
    {
        public static void Hook()
        {
            On.MultiplayerUnlocks.SandboxItemUnlocked += MultiplayerUnlocks_SandboxItemUnlocked;
        }

        private static bool MultiplayerUnlocks_SandboxItemUnlocked(On.MultiplayerUnlocks.orig_SandboxItemUnlocked orig, MultiplayerUnlocks self, MultiplayerUnlocks.SandboxUnlockID unlockID)
        {
            
            if (unlockID == SandboxUnlockID.Shoggoth || unlockID == SandboxUnlockID.TwistedSpear || unlockID == SandboxUnlockID.SmallBoneBall || unlockID == SandboxUnlockID.MiddleBoneBall || unlockID == SandboxUnlockID.BigBoneBall)
            {
                return true;
            }
            return orig.Invoke(self, unlockID);
        }
    }
}
