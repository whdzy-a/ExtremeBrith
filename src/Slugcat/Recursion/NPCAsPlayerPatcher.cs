using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtremeBrith.Slugcat.Recursion
{
    public class NPCAsPlayerPatcher
    {
        private static Hook isNPCHook;

        public static void Initialize()
        {
            // 方法1：使用MonoMod Hook（需要引用MonoMod.RuntimeDetour）
            isNPCHook = new Hook(
                typeof(Player).GetProperty("isNPC").GetGetMethod(),
                typeof(NPCAsPlayerPatcher).GetMethod("IsNPC_Override")
            );

            // 方法2：使用Harmony（如果你已经使用Harmony）
            // var harmony = new Harmony("com.you.npcplayer");
            // harmony.Patch(
            //     typeof(Player).GetProperty("isNPC").GetGetMethod(),
            //     prefix: new HarmonyMethod(typeof(NPCAsPlayerPatcher), "IsNPC_Prefix")
            // );
        }

        // Hook方法的实现
        public static bool IsNPC_Override(Func<Player, bool> orig, Player self)
        {
            // 获取原始值
            bool originalValue = orig(self);

            // 如果是特定NPC，返回false（当作玩家）
            if (IsSpecialNPC(self))
            {
                return false;
            }

            return originalValue;
        }


        private static bool IsSpecialNPC(Player player)
        {
            if (player.slugcatStats.name.value == "Recursion")
            {
                return true;
            }

            return false;
        }

    }
}
