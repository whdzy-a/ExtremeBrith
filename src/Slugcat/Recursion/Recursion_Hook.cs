using ExtremeBrith.Object;
using JollyCoop;
using MoreSlugcats;
using RWCustom;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static AbstractPhysicalObject;
using static MoreSlugcats.SlugNPCAI;
using static Player;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;

namespace ExtremeBrith.Slugcat.Recursion
{
    public class Recursion_Hook
    {

        public static void Hook() 
        {
            On.Player.ctor += Player_ctor;
            On.Player.Update += Player_Update;
            On.HUD.FoodMeter.MeterCircle.Draw += MeterCircle_Draw;
            On.Player.Die += Player_Die;//更新分身列表
            On.Player.AddFood += Player_AddFood;
            On.Player.ShortCutColor += Player_ShortCutColor;//覆写分身颜色
            On.Player.NewRoom += Player_NewRoom;
        }


        private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig.Invoke(self, eu);
            RecursionController.LogPlayerIdentity(self.abstractCreature);
            //Plugin.Log($"{self.bodyChunkConnections[0].distance}");


            if (isRecMainbody(self) && RecursionModuleManeger.PlayerModules.TryGetValue(self.abstractCreature, out var Recmodule))
            {
                if (self.input[0].jmp)
                {
                    self.setPupStatus(false);
                }
                if (Recmodule.ChooseMode)//选择模式
                {
                    if (self.input[0].spec)
                    {
                        Recmodule.ChooseMode = true;
                        self.mushroomEffect = 1;
                        
                    }
                    else
                    {
                        Recmodule.ChooseMode = false;
                        self.mushroomEffect = 0;
                    }
                }
                else if(Recmodule.splittingCounter >0)//分裂模式
                {
                    if (self.input[0].spec)
                    {
                        Recmodule.splittingCounter++;
                        self.bodyChunks[0].vel += (self.bodyChunks[0].pos - self.bodyChunks[1].pos).normalized * Mathf.Cos(Mathf.Pow(Recmodule.splittingCounter / 10, 2));

                    }
                    else
                    {
                        Recmodule.splittingCounter = 0;
                    }
                    if (Recmodule.splittingCounter > 60)
                    {
                        RecursionController.CheckAllSplitting(self.abstractCreature.Room.world.game);

                        Recmodule.splittingCounter = 0;
                        if (!self.playerState.isPup)
                        {
                            Splitting(self);

                        }
/*                        else
                        {
                            if (Recmodule.Splitttings.Count > 0)
                            {
                                RecursionController.TurnControl(self.abstractCreature, Recmodule.Splitttings[Random.Range(0, Recmodule.Splitttings.Count - 1)]);
                            }
                        }
*/                    }
                }
                else//进入技能
                {
                    if (self.input[0].spec)
                    {
                        if (self.input[0].y > 0)
                        {
                            Recmodule.ChooseMode = false;
                            Recmodule.splittingCounter = 1;
                        }
                        else if (self.input[0].y == 0)
                        {
                            Recmodule.ChooseMode = true;
                            Recmodule.splittingCounter = 0;
                            Recmodule.ChoosePos = Vector2.zero;
                        }
                    }
                }



                if (!canMove(self.abstractCreature))
                {
                    if (!self.playerState.isPup)
                    {
                        AbstractCreature abstractCreature = Splitting(self);
                        RecursionController.TurnControl(self.abstractCreature, abstractCreature);

                    }
                    if (Recmodule.Splitttings.Count > 0)
                    {
                        foreach (var spltting in Recmodule.Splitttings)
                        {
                            if (isRecSpliting(spltting) && canMove(spltting))
                            {
                                RecursionController.TurnControl(self.abstractCreature, spltting);
                            }
                        }
                    }
                }

            }



        }



        private static void Player_NewRoom(On.Player.orig_NewRoom orig, Player self, Room newRoom)
        {
            orig.Invoke(self, newRoom);
/*            if (ModManager.MSC && self.slugOnBack?.slugcat != null)
            {
                Debug.Log($"NPC {self.abstractCreature.ID} carrying slug to {newRoom.abstractRoom.name}");
                self.slugOnBack.slugcat.NewRoom(newRoom);
            }*/
        }

        private static Color Player_ShortCutColor(On.Player.orig_ShortCutColor orig, Player self)
        {
            int hostNumber = Plugin.GetHostNumber(self.abstractCreature);
            if (hostNumber == -1)
            {
                return orig.Invoke(self);
            }
            return PlayerGraphics.JollyColor(hostNumber, 0);
        }



        private static void Player_AddFood(On.Player.orig_AddFood orig, Player self, int add)
        {
            if (self.slugcatStats.name.value == "Recursion")
            {
                if (self.redsIllness != null)
                {
                    self.redsIllness.AddFood(add);
                }
                else
                {
                    add = Math.Min(add, self.MaxFoodInStomach - self.playerState.foodInStomach);
                    if (ModManager.CoopAvailable && self.abstractCreature.world.game.IsStorySession && self.abstractCreature.world.game.Players[0] != self.abstractCreature && isRecMainbody(self))
                    {
                        PlayerState playerState = self.abstractCreature.world.game.Players[0].state as PlayerState;
                        add = Math.Min(add, Math.Max(self.MaxFoodInStomach - playerState.foodInStomach, 0));
                        JollyCustom.Log($"Player add food {self.playerState.playerNumber}. Amount to add {add}");
                        playerState.foodInStomach += add;
                    }

                    if (self.abstractCreature.world.game.IsStorySession && self.AI == null)
                    {
                        self.abstractCreature.world.game.GetStorySession.saveState.totFood += add;
                    }

                    self.playerState.foodInStomach += add;
                }

                if (self.FoodInStomach >= self.MaxFoodInStomach)
                {
                    self.playerState.quarterFoodPoints = 0;
                }

                if (!self.Malnourished || self.playerState.foodInStomach < ((self.redsIllness != null) ? self.redsIllness.FoodToBeOkay : self.slugcatStats.maxFood))
                {
                    return;
                }

                if (self.redsIllness != null)
                {
                    self.redsIllness.GetBetter();
                    return;
                }

                if (!self.isSlugpup)
                {
                    self.SetMalnourished(m: false);
                }

                if (self.playerState is PlayerNPCState)
                {
                    (self.playerState as PlayerNPCState).Malnourished = false;
                }
            }
            else
            {
                orig.Invoke(self, add);
            }
        }

        private static void Player_Die(On.Player.orig_Die orig, Player self)
        {
            if (isRecMainbody(self) && RecursionModuleManeger.PlayerModules.TryGetValue(self.abstractCreature, out var Recmodule))
            {
                if (!self.playerState.isPup)
                {
                    AbstractCreature abstractCreature = Splitting(self);
                    RecursionController.TurnControl(self.abstractCreature, abstractCreature);

                }
                RecursionController.CheckAllSplitting(self.room.game);
                if (Recmodule.Splitttings.Count > 0)
                {
                    foreach (var spltting in Recmodule.Splitttings)
                    {
                        if (isRecSpliting(spltting) && canMove(spltting))
                        {
                            RecursionController.TurnControl(self.abstractCreature, spltting);
                            break;
                        }
                    }
                }

            }

            orig.Invoke(self);
            //RecursionController.CheckAllSplitting(self.abstractCreature.Room.world.game);
        }

        

        public static void InitSplitting(Player splitting, Player owner)
        {

            if (splitting == null || owner == null) return;
            if(splitting.playerState == null || owner .playerState == null) return;
            splitting.playerState.playerNumber = owner.playerState.playerNumber;
            splitting.playerState.slugcatCharacter = owner.playerState.slugcatCharacter;
            splitting.SlugCatClass = owner.slugcatStats.name;
            splitting.slugcatStats.name = owner.slugcatStats.name;//设定slugname

            if (splitting.slugOnBack == null)
            {
                splitting.slugOnBack = new SlugOnBack(splitting);//新建slugonback
            }

            if (owner.room != null && owner.room.game.cameras != null)//刷新猫崽外观
            {
                foreach (var cam in owner.room.game.cameras)
                {
                    foreach (var item in cam.spriteLeasers)
                    {
                        if (item.drawableObject == splitting.graphicsModule)
                        {
                            (splitting.graphicsModule as PlayerGraphics).ApplyPalette(item, cam, cam.currentPalette);

                            Plugin.Log("刷新成功");
                        }
                    }
                }
            }

            RecursionModuleManeger.PlayerModule RecSplttingmodule;
            if (!RecursionModuleManeger.PlayerModules.TryGetValue(splitting.abstractCreature, out RecSplttingmodule))
            {
                RecursionModuleManeger.PlayerModules.Add(splitting.abstractCreature, new RecursionModuleManeger.PlayerModule(splitting.abstractCreature));
                RecursionModuleManeger.PlayerModules.TryGetValue(splitting.abstractCreature, out RecSplttingmodule);
            }

            RecursionModuleManeger.PlayerModules.TryGetValue(owner.abstractCreature, out var RecHostmodule);

            RecSplttingmodule.Host = owner.abstractCreature;
            if (!RecHostmodule.Splitttings.Contains(splitting.abstractCreature))
            {
                RecHostmodule.Splitttings.Add(splitting.abstractCreature);
            }
            if (splitting.abstractCreature.abstractAI != null)
            {
                (splitting.abstractCreature.abstractAI as SlugNPCAbstractAI).isTamed = true;
                (splitting.abstractCreature.abstractAI.RealAI as SlugNPCAI).friendTracker.friend = owner;
                var rel = splitting.playerState.socialMemory.GetOrInitiateRelationship(owner.abstractCreature.ID);
                rel.like = 1f;
                rel.tempLike = 1f;
            }

            //splitting.npcCharacterStats = owner.slugcatStats;


        }

        public static void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig.Invoke(self, abstractCreature, world);
            if (Plugin.GetHostNumber(self.abstractCreature) != -1)
            {
                self.playerState.playerNumber = Plugin.GetHostNumber(self.abstractCreature);
                self.playerState.slugcatCharacter = new SlugcatStats.Name("Recursion");

            }

            if (self.slugcatStats.name.value == "Recursion")
            {
                RecursionModuleManeger.PlayerModules.Add(self.abstractCreature, new RecursionModuleManeger.PlayerModule(self.abstractCreature));
                if (RecursionModuleManeger.PlayerModules.TryGetValue(self.abstractCreature, out var Recmodule))
                {

                }
            }
        }

        private static void MeterCircle_Draw(On.HUD.FoodMeter.MeterCircle.orig_Draw orig, HUD.FoodMeter.MeterCircle self, float timeStacker)
        {
            self.gradient.scale = 6.25f;
            self.gradient.alpha = 0.1f * Mathf.Pow(Mathf.Max(0f, Mathf.Lerp(self.meter.lastFade, self.meter.fade, timeStacker)), 2f);
            Vector2 position = self.DrawPos(timeStacker);
            self.gradient.x = position.x;
            self.gradient.y = position.y;
            if (self.backCircle != null)
            {
                self.backCircle.Draw(timeStacker);
                self.backCircle.sprite.SetPosition(position);
            }

            for (int i = 0; i < self.circles.Length; i++)
            {
                self.circles[i].Draw(timeStacker);
                if (self.meter.IsPupFoodMeter)
                {
                    self.circles[i].sprite.element = Futile.atlasManager.GetElementWithName(self.circles[i].snapGraphic.ToString());
                    self.circles[i].sprite.scale = self.circles[i].rad / self.circles[i].snapRad;
                    self.circles[i].sprite.alpha = 1f;
                    self.circles[i].sprite.shader = self.circles[i].basicShader;
                    self.circles[i].sprite.alpha = Mathf.Lerp(self.circles[i].lastFade, self.circles[i].fade, timeStacker);
                    self.circles[i].sprite.color = Custom.FadableVectorCircleColors[self.circles[i].color];
                    self.circles[i].sprite.scale /= 2f;
                }

                self.circles[i].sprite.x = position.x;
                self.circles[i].sprite.y = position.y;
                if (self.meter.IsPupFoodMeter)
                {
                    if (self.meter.pup.npcStats == null)
                    {
                        Vector3 hsl = Custom.RGB2HSL(self.meter.pup.ShortCutColor());
                        self.circles[i].sprite.color = Color.Lerp(Color.Lerp(self.circles[i].sprite.color, Custom.HSL2RGB(hsl.x, Mathf.Lerp(hsl.y, 1f, 0.8f), hsl.z), 0.5f - (float)self.circles[i].color * 0.5f), new Color(0.6f, 0.6f, 0.6f), self.meter.deathFade);
                    }
                    else
                    {
                        self.circles[i].sprite.color = Color.Lerp(Color.Lerp(self.circles[i].sprite.color, Custom.HSL2RGB(self.meter.pup.npcStats.H, Mathf.Lerp(self.meter.pup.npcStats.S, 1f, 0.8f), self.meter.pup.npcStats.Dark ? 0.3f : 0.7f), 0.5f - (float)self.circles[i].color * 0.5f), new Color(0.6f, 0.6f, 0.6f), self.meter.deathFade);

                    }
                }
            }

        }


        public static AbstractCreature Splitting(Player self)
        {
            AbstractCreature cameraFollow = null;
            if (self.abstractCreature.world.game.cameras[0].followAbstractCreature != null)
            {
                cameraFollow = self.abstractCreature.world.game.cameras[0].followAbstractCreature;
            }
            AbstractCreature abstractCreature = new AbstractCreature(self.room.world, StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.SlugNPC), null, self.abstractCreature.pos, self.room.game.GetNewID());
            self.room.abstractRoom.AddEntity(abstractCreature);
            abstractCreature.RealizeInRoom();
            for (int i = 0; i < Random.Range(5, 12); i++)
            {
                self.room.AddObject(new SplittingSpit(self.firstChunk.pos, Custom.RNV() * Random.Range(5, 9), self));
            }
            Plugin.SetHostNumber(abstractCreature, self.playerState.playerNumber);

            self.Stun(50);
            abstractCreature.realizedCreature.Stun(100);
            self.room.AddObject(new CreatureSpasmer(abstractCreature.realizedCreature, true, abstractCreature.realizedCreature.stun));
            (abstractCreature.realizedCreature as Player).setPupStatus(true);


            InitSplitting(abstractCreature.realizedCreature as Player, self);


            self.setPupStatus(true);
            if (cameraFollow != null)
            {
                self.abstractCreature.world.game.cameras[0].ChangeCameraToPlayer(cameraFollow);
            }
            return abstractCreature;
        }

        public static bool isRecSpliting(AbstractCreature player)
        {
            return Plugin.GetHostNumber(player) != -1;
        }

        public static bool isRecMainbody(Player player)
        {
            return player.slugcatStats.name.value == "Recursion" && player.abstractCreature.abstractAI == null;
        }

        public static bool canMove(AbstractCreature player)
        {
            if (player == null) return false;

            // 先判死活
            if (!player.state.alive) return false;

            // 再判是否被天敌叼住
            for (int i = 0; i < player.stuckObjects.Count; i++)
            {
                if (player.stuckObjects[i] is CreatureGripStick grip &&
                    grip.A != player)          // A 是捕食者，B 是玩家
                {
                    return false;               // 被抓住 → 不能动
                }
            }

            return true;   // 以上都通过才能动
        }
        public static void RGBtoHSL(Color rgb, out float h, out float s, out float l)
        {
            float r = rgb.r;
            float g = rgb.g;
            float b = rgb.b;

            float max = Mathf.Max(r, Mathf.Max(g, b));
            float min = Mathf.Min(r, Mathf.Min(g, b));
            float d = max - min;

            l = (max + min) * 0.5f;

            if (d < 1e-6f)
            {
                h = 0f;
                s = 0f;
                return;
            }

            s = l > 0.5f ? d / (2f - max - min) : d / (max + min);

            if (max == r) h = (g - b) / d + (g < b ? 6f : 0f);
            else if (max == g) h = (b - r) / d + 2f;
            else /* max==b */ h = (r - g) / d + 4f;

            h *= 60f;
        }
    }
}
