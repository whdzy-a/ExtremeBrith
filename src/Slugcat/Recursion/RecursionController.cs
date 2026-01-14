using HUD;
using JollyCoop.JollyHUD;
using MoreSlugcats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Player;

namespace ExtremeBrith.Slugcat.Recursion
{
    public class RecursionController
    {
        public static void TurnControl(AbstractCreature oldAC, AbstractCreature newAC)
        {
            if (oldAC == null || newAC == null || oldAC == newAC) return;

            Plugin.Log($"[Recursion] 开始切换主控 {oldAC.ID} -> {newAC.ID}");
            
            //实例化目标房间
            if (oldAC.Room != newAC.Room && newAC.Room.realizedRoom == null)
            {
                oldAC.world.ActivateRoom(newAC.Room);
            }

            if (newAC.Room.realizedRoom == null)
            {
                Plugin.Log("[Recursion] 切换失败，无法加载目标房间");
                return;
            }

            var tmpState = oldAC.state;
            oldAC.state = newAC.state;
            newAC.state = tmpState;

/*            var SlgState = (oldAC.realizedCreature as Player).slugcatStats;
            (oldAC.realizedCreature as Player).slugcatStats = (newAC.realizedCreature as Player).slugcatStats;
            newAC.state = tmpState;*/

            /* ---------- 3. 交换 creatureTemplate ---------- */
            var tmpTmpl = oldAC.creatureTemplate;
            oldAC.creatureTemplate = newAC.creatureTemplate;
            newAC.creatureTemplate = tmpTmpl;

            /* ---------- 4. 交换 AI ---------- */
            var tmpAbsAI = oldAC.abstractAI;
            var tmpRealAI = oldAC.abstractAI?.RealAI;

            oldAC.abstractAI = newAC.abstractAI;          // 把 NPC 的抽象 AI 给旧玩家
            oldAC.abstractAI.parent = oldAC;              // 把 parent 指回来
            oldAC.abstractAI.RealAI = newAC.abstractAI?.RealAI;
            oldAC.abstractAI.RealAI.creature = oldAC;     // RealAI 里的 creature 也要指回来
            oldAC.abstractAI.RealAI.pathFinder.creature = oldAC;

            newAC.abstractAI = tmpAbsAI;                  // 旧玩家的 AI 给新玩家
            if (newAC.abstractAI != null)
            {
                newAC.abstractAI.parent = newAC;
                newAC.abstractAI.RealAI = tmpRealAI;
                if (newAC.abstractAI.RealAI != null)
                    newAC.abstractAI.RealAI.creature = newAC;
            }
            if (newAC.abstractAI != null)
            {
                newAC.abstractAI.RealAI = null;
                newAC.abstractAI = null;
            }

            foreach (var item in oldAC.world.game.cameras[0].hud.parts)
            {
                if (item is PlayerSpecificMultiplayerHud)
                {
                    if ((item as PlayerSpecificMultiplayerHud).abstractPlayer == oldAC)
                    {
                        (item as PlayerSpecificMultiplayerHud).abstractPlayer = newAC;
                    }
                }
                if (item is JollyPlayerSpecificHud)
                {
                    if ((item as JollyPlayerSpecificHud).abstractPlayer == oldAC)
                    {
                        (item as JollyPlayerSpecificHud).abstractPlayer = newAC;
                    }
                }
            }


            if( newAC.realizedCreature != null && newAC.realizedCreature.room != null)
            {
                newAC.realizedCreature.room.AddObject(new PlayerRipple(newAC.realizedCreature.firstChunk, Vector2.zero, 1, Color.white));
            }
            var id = oldAC.ID;
            oldAC.ID = newAC.ID;
            newAC.ID = id;


            
            
            RainWorldGame game = newAC.Room.realizedRoom.game;

            int index = game.Players.IndexOf(oldAC);
            game.Players.Remove(oldAC);
            game.Players.Insert(index, newAC);

            if (game.cameras[0] != null)
            {
                game.cameras[0].ChangeCameraToPlayer(newAC);
                if (game.cameras[0].hud != null)
                    game.cameras[0].hud.owner = newAC.realizedCreature as Player;

                var mainMeter = game.cameras[0].hud.parts.OfType<FoodMeter>().FirstOrDefault(f => f.IsPupFoodMeter && f.abstractPup == newAC);
                if (mainMeter != null)
                {
                    mainMeter.abstractPup = oldAC;
                    mainMeter.pup = oldAC.realizedCreature as Player;
                }


            }
            for (int i = 0; i < (newAC.realizedCreature as Player).input.Length; i++)
                (newAC.realizedCreature as Player).input[i] = new InputPackage();

            RecursionModuleManeger.PlayerModules.TryGetValue(newAC, out var newRecmodule);

            RecursionModuleManeger.PlayerModules.TryGetValue(oldAC, out var oldRecmodule);


            //CheckAllSplitting(newAC.Room.world.game);


            Plugin.Log($"[Recursion] 主控切换完成 {oldAC.ID} -> {newAC.ID}");
        }

        public static void LogPlayerIdentity(AbstractCreature p)
        {
            if (p == null) { Plugin.Log("[Recursion] 玩家为 null"); return; }

            var st = p.state as PlayerState;
            Plugin.Log($"[Recursion] 生物ID={p.ID}|角色ID={st?.playerNumber ?? -1}|" +
                       $"playerState={(st == null ? "null" : $"食物={st.foodInStomach + st.quarterFoodPoints * 0.25f}|角色={(p.realizedCreature as Player).slugcatStats.name.value}|幼崽={st.isPup}")}"+$"{(Plugin.GetHostNumber(p)==-1?$"|独立个体":$"|主人为{Plugin.GetHostNumber(p)}")}");
        }

        public static void CheckAllSplitting(RainWorldGame game)
        {
            Plugin.Log($"开始检测 Recursion 分裂...");

            

            foreach (AbstractCreature host in game.Players)
            {
                if (host.realizedCreature == null) continue;

                if (host.abstractAI == null && (host.state as PlayerState).slugcatCharacter.value == "Recursion")
                {
                    if (!RecursionModuleManeger.PlayerModules.TryGetValue(host, out var RecHostmodule)) continue;

                    RecHostmodule.Splitttings.Clear();
                    RecHostmodule.Host = null;

                    List<string> splittingInfo = new List<string>();

                    foreach (var abroom in game.world.abstractRooms)
                    {
                        if (abroom == null || abroom.entities == null) continue;

                        foreach (var entity in abroom.entities)
                        {
                            if (!(entity is AbstractCreature abstractCreature)) continue;

                            if (abstractCreature.creatureTemplate.TopAncestor().type == CreatureTemplate.Type.Slugcat ||
                                (abstractCreature.creatureTemplate.TopAncestor().type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC))
                            {
                                var playerState = abstractCreature.state as PlayerState;

                                if (playerState.alive && Plugin.GetHostNumber(abstractCreature) == (host.realizedCreature as Player).playerState.playerNumber && abstractCreature.ID != host.ID) // 排除宿主自己
                                {
                                    Recursion_Hook.InitSplitting(abstractCreature.realizedCreature as Player,host.realizedCreature as Player);
                                    if (RecursionModuleManeger.PlayerModules.TryGetValue(abstractCreature, out var RecSplittingModule))
                                    {
                                        RecHostmodule.Splitttings.Add(abstractCreature);
                                        RecSplittingModule.Host = host;
                                        RecSplittingModule.Splitttings.Clear();

                                        splittingInfo.Add($"[ID:{abstractCreature.ID} 房间:{abstractCreature.Room?.name ?? "?"}]");
                                        LogPlayerIdentity(abstractCreature);
                                    }
                                }
                            }
                        }
                    }

                    // 输出宿主和分身信息
                    if (splittingInfo.Count > 0)
                    {
                        Plugin.Log($"宿主 {host.ID} 在房间 {host.Room?.name ?? "未知"} 有 {splittingInfo.Count} 个分身:");
                        foreach (var info in splittingInfo)
                        {
                            Plugin.Log($"  - {info}");
                        }
                    }
                    else
                    {
                        Plugin.Log($"宿主 {host.ID} 没有找到分身");
                    }
                }
            }
        }
    }
}
