using System;
using BepInEx;
using Fisobs.Core;
using RWCustom;
using ExtremeBrith.Slugcat.DeathKnell;
using ExtremeBrith.CustomCreature.Shoggoth;
using ExtremeBrith.Object.TwistedSpear;
using System.Collections.Generic;
using SlugBase.SaveData;
using ExtremeBrith.Object.Ball;
using ExtremeBrith.Object;
using UnityEngine;
using JollyCoop;
using ExtremeBrith.Slugcat.Recursion;

namespace ExtremeBrith
{

    public class DeathKnellSvaeData
    {
        public AbstractBall ConnectBall { get; set; }
    }

    public class RecursionSvaeData
    {
        public int HostNumber { get; set; }
    }

    [BepInPlugin(MOD_ID, "Extreme_Brith", "0.1.0")]
    class Plugin : BaseUnityPlugin
    {
        private const string MOD_ID = "Extreme_Brith";

        public bool IsInit = false;


        // Add hooks
        public void OnEnable()
        {
            On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);
            On.Player.Update += Player_Update;
            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
            On.RainWorld.PostModsInit += ReorderUnlocks;

            DeathKnellGraphics_AddSpine.Hook();
            DeathKnell_Hook.Hook();
            DeathKnellGraphics.Hook();

            RecursionGraphics.Hook();
            Recursion_Hook.Hook();

            UnlockItemHook.Hook();

            //NPCAsPlayerPatcher.Initialize();
        }


        public void loadSprites()
        {
            Futile.atlasManager.LoadAtlas("atlases/Kill_Shoggoth");
            Futile.atlasManager.LoadAtlas("atlases/ShoggothEyes");

            Futile.atlasManager.LoadAtlas("atlases/icon_TwistedSpear");
            Futile.atlasManager.LoadAtlas("atlases/TwistedSpear");


            Futile.atlasManager.LoadAtlas("atlases/DeathKnell/BoneBall");
            Futile.atlasManager.LoadAtlas("atlases/DeathKnell/icon_SmallBoneBall");
            Futile.atlasManager.LoadAtlas("atlases/DeathKnell/icon_MiddleBoneBall");
            Futile.atlasManager.LoadAtlas("atlases/DeathKnell/icon_BigBoneBall");
            
            Futile.atlasManager.LoadAtlas("atlases/DeathKnell/Spine");
            Futile.atlasManager.LoadAtlas("atlases/Wing_skeleton");
            Futile.atlasManager.LoadAtlas("atlases/DeathKnell/ConnectLine");


        }
        private void ReorderUnlocks(On.RainWorld.orig_PostModsInit orig, RainWorld self)
        {
            orig.Invoke(self);
            OrganizeCreatureUnlocks(MultiplayerUnlocks.SandboxUnlockID.Fly, SandboxUnlockID.Shoggoth);
            OrganizeItemUnlocks(MultiplayerUnlocks.SandboxUnlockID.ScavengerBomb, SandboxUnlockID.TwistedSpear);
            OrganizeItemUnlocks(MultiplayerUnlocks.SandboxUnlockID.ScavengerBomb, SandboxUnlockID.SmallBoneBall);
            OrganizeItemUnlocks(MultiplayerUnlocks.SandboxUnlockID.ScavengerBomb, SandboxUnlockID.MiddleBoneBall);
            OrganizeItemUnlocks(MultiplayerUnlocks.SandboxUnlockID.ScavengerBomb, SandboxUnlockID.BigBoneBall);
        }
        public void OrganizeCreatureUnlocks(MultiplayerUnlocks.SandboxUnlockID moveToBeforeThis, MultiplayerUnlocks.SandboxUnlockID unlockToMove)
        {
            MultiplayerUnlocks.CreatureUnlockList.Remove(unlockToMove);
            MultiplayerUnlocks.CreatureUnlockList.Insert(MultiplayerUnlocks.CreatureUnlockList.IndexOf(moveToBeforeThis), unlockToMove);
        }
        public void OrganizeItemUnlocks(MultiplayerUnlocks.SandboxUnlockID moveToBeforeThis, MultiplayerUnlocks.SandboxUnlockID unlockToMove)
        {
            MultiplayerUnlocks.ItemUnlockList.Remove(unlockToMove);
            MultiplayerUnlocks.ItemUnlockList.Insert(MultiplayerUnlocks.ItemUnlockList.IndexOf(moveToBeforeThis), unlockToMove);

        }

        private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig.Invoke(self);
            try
            {
                bool isInit = IsInit;
                if (isInit)
                {
                    return;
                }
                IsInit = true;
                loadSprites();
            }
            catch (Exception ex)
            {
                throw;
            }
            Content.Register(new IContent[]
            {
                new ShoggothCritob(),
                new TwistedSpearFisob(),
                new SmallBoneBallFisob(),
                new MiddleBoneBallFisob(),
                new BigBoneBallFisob()
            });
            ShoggothHook.Hook();

        }

        private void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            bool isjump = self.input[0].spec;
            orig.Invoke(self, eu);
            if (!isjump && self.input[0].spec)
            {
                /*                Log("生成生物");
                                AbstractCreature abstractCreature = new AbstractCreature(self.room.world, StaticWorld.GetCreatureTemplate(ShoggothTemplateType.Shoggoth), null, self.abstractCreature.pos, self.room.game.GetNewID());
                                self.room.abstractRoom.AddEntity(abstractCreature);
                                abstractCreature.RealizeInRoom();
                                Log("生成生物成功");
                Log("生成物品");
                AbstractTwistedSpear abstractTwistedSpear = new(self.room.world, null, self.abstractCreature.pos, self.room.game.GetNewID());
                self.room.abstractRoom.AddEntity(abstractTwistedSpear);
                abstractTwistedSpear.RealizeInRoom();
                Log("生成物品成功");*/
            }

        }

        // Load any resources, such as sprites or sounds
        private void LoadResources(RainWorld rainWorld)
        {
        }

        public static AbstractBall GetConnectBall(Room room, int playerindex)
        {
            DeathKnellSvaeData data = null;
            StoryGameSession session;
            SlugBaseSaveData DeathKnell_savedata;
            if (room != null && room.game.IsStorySession)
            {
                session = room.game.GetStorySession;
                DeathKnell_savedata = session.saveState.miscWorldSaveData.GetSlugBaseData();
                if (!DeathKnell_savedata.TryGet($"DeathKnell{playerindex}", out data))
                {
                    if (data == null)
                    {
                        data = new();
                        data.ConnectBall = null;
                    }
                    DeathKnell_savedata.Set($"DeathKnell{playerindex}", data);
                }
            }

            if (data != null)
            {
                return data.ConnectBall;
            }

            return null;

        }
        public static void SetConnectBall(Room room, int playerindex)
        {
            DeathKnellSvaeData data = null;
            StoryGameSession session;
            SlugBaseSaveData DeathKnell_savedata;
            if (room != null && room.game.IsStorySession)
            {
                session = room.game.GetStorySession;
                DeathKnell_savedata = session.saveState.miscWorldSaveData.GetSlugBaseData();
                if (!DeathKnell_savedata.TryGet($"DeathKnell{playerindex}", out data))
                {
                    if (data == null)
                    {
                        data = new();
                        data.ConnectBall = null;
                    }
                }
                DeathKnell_savedata.Set($"DeathKnell{playerindex}", data);
            }
        }

        public static int GetHostNumber(AbstractCreature splitting)
        {
            RecursionSvaeData data = null;
            StoryGameSession session;
            SlugBaseSaveData Recursion_savedata;
            if (splitting.Room != null && splitting.Room.world.game.IsStorySession)
            {
                session = splitting.Room.world.game.GetStorySession;
                Recursion_savedata = session.saveState.miscWorldSaveData.GetSlugBaseData();
                if (!Recursion_savedata.TryGet($"Recursion{splitting.ID}", out data))
                {
                    if (data == null)
                    {
                        data = new();
                        data.HostNumber = -1;
                    }
                    Recursion_savedata.Set($"Recursion{splitting.ID}", data);
                }
            }
            if(data != null)
            {
                return data.HostNumber;
            }

            return -1;

        }
        public static void SetHostNumber(AbstractCreature splitting, int playerNumber)
        {
            RecursionSvaeData data = null;
            StoryGameSession session;
            SlugBaseSaveData Recursion_savedata;
            if (splitting.Room != null && splitting.Room.world.game.IsStorySession)
            {
                session = splitting.Room.world.game.GetStorySession;
                Recursion_savedata = session.saveState.miscWorldSaveData.GetSlugBaseData();
                if (!Recursion_savedata.TryGet($"Recursion{splitting.ID}", out data))
                {
                    if (data == null)
                    {
                        data = new();
                        data.HostNumber = playerNumber;
                    }
                }
                        data.HostNumber = playerNumber;
                Recursion_savedata.Set($"Recursion{splitting.ID}", data);
            }
        }



        public static Color getBoneColor(Player self)
        {
            Color nowColor;
            if (ModManager.MMF && PlayerGraphics.CustomColorsEnabled())
            {
                return PlayerGraphics.CustomColorSafety(2);
            }
            else if (Custom.rainWorld.options.jollyColorMode == Options.JollyColorMode.CUSTOM)
            {
                return PlayerGraphics.JollyColor(self.playerState.playerNumber, 2);
            }
            else if (Custom.rainWorld.options.jollyColorMode == Options.JollyColorMode.AUTO)
            {
                ColorUtility.TryParseHtmlString("#FFE99B", out nowColor);
                if (self.playerState.playerNumber == 0)
                {
                    return nowColor;
                }
                else
                {
                    nowColor = PlayerGraphics.JollyColor(self.playerState.playerNumber, 0);
                    HSLColor hSLColor = JollyCustom.RGB2HSL(nowColor);
                    float num = hSLColor.hue + 0.2383333f;
                    if (num > 0)
                    {
                        num -= 1;
                    }
                    hSLColor.hue = num;
                    hSLColor.saturation = 1;
                    hSLColor.lightness = 0.8039f;
                    return hSLColor.rgb;
                }
            }
            else if (self.room != null && self.room.game.IsArenaSession)
            {
                switch (self.playerState.playerNumber)
                {
                    case 0:
                        ColorUtility.TryParseHtmlString("#FFFFFF", out nowColor);
                        return nowColor;
                    case 1:
                        ColorUtility.TryParseHtmlString("#FFDC63", out nowColor);
                        return nowColor;
                    case 2:
                        ColorUtility.TryParseHtmlString("#FF5353", out nowColor);
                        return nowColor;
                    case 3:
                        ColorUtility.TryParseHtmlString("#7F37DB", out nowColor);
                        return nowColor;
                }
            }
            ColorUtility.TryParseHtmlString("#FFE99B", out nowColor);
            return nowColor;
        }
        public static void Log(string text)
        {
            Custom.LogWarning("Extreme Brith:" + text);
        }

    }
}