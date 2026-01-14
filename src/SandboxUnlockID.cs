using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtremeBrith
{
    public static class SandboxUnlockID
    {
        public static void UnregisterValues()
        {
                Shoggoth.Unregister();
                TwistedSpear.Unregister();
                GlodenMask.Unregister();
            SmallBoneBall.Unregister();
            MiddleBoneBall.Unregister();
            BigBoneBall.Unregister();
                Shoggoth = null;
                TwistedSpear = null;
                GlodenMask = null;
                SmallBoneBall = null;
                MiddleBoneBall = null;
                BigBoneBall = null;
        }

        // Token: 0x04000008 RID: 8
        public static MultiplayerUnlocks.SandboxUnlockID Shoggoth = new MultiplayerUnlocks.SandboxUnlockID("Shoggoth", true);

        public static MultiplayerUnlocks.SandboxUnlockID TwistedSpear = new MultiplayerUnlocks.SandboxUnlockID("TwistedSpear", true);
        public static MultiplayerUnlocks.SandboxUnlockID GlodenMask = new MultiplayerUnlocks.SandboxUnlockID("GlodenMask", true);
        public static MultiplayerUnlocks.SandboxUnlockID SmallBoneBall = new MultiplayerUnlocks.SandboxUnlockID("SmallBoneBall", true);
        public static MultiplayerUnlocks.SandboxUnlockID MiddleBoneBall = new MultiplayerUnlocks.SandboxUnlockID("MiddleBoneBall", true);
        public static MultiplayerUnlocks.SandboxUnlockID BigBoneBall = new MultiplayerUnlocks.SandboxUnlockID("BigBoneBall", true);


    }
}
