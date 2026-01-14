using LizardCosmetics;
using RWCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ExtremeBrith.CustomCreature.Shoggoth
{
    public class ShoggothHook
    {

        public static Dictionary<string, float> lizInfo = new Dictionary<string, float>();

        public static Color colorr = Custom.HSL2RGB(0.13888f, 1f, 0.32f);
        public static void Hook()
        {
            On.LizardLimb.ctor += LizardLimb_ctor;
            On.LizardVoice.GetMyVoiceTrigger += LizardVoice_GetMyVoiceTrigger;
            On.LizardBreeds.BreedTemplate_Type_CreatureTemplate_CreatureTemplate_CreatureTemplate_CreatureTemplate += LizardBreeds_BreedTemplate_Type_CreatureTemplate_CreatureTemplate_CreatureTemplate_CreatureTemplate;
            On.Lizard.ctor += Lizard_ctor;
            On.LizardGraphics.ctor += LizardGraphics_ctor;
            On.LizardGraphics.ApplyPalette += LizardGraphics_ApplyPalette;
            On.LizardGraphics.HeadColor += LizardGraphics_HeadColor;
            On.LizardGraphics.DrawSprites += LizardGraphics_DrawSprites;
            On.LizardGraphics.Update += LizardGraphics_Update;

        }

        private static void LizardGraphics_Update(On.LizardGraphics.orig_Update orig, LizardGraphics self)
        {
            orig.Invoke(self);
            if (self.lizard.Template.type == ShoggothTemplateType.Shoggoth)
            {

                float num4 = 180f;
                if (self.creatureLooker.lookCreature != null && self.lizard.Consious)
                {
                    num4 -= 80f;
                    if (self.creatureLooker.lookCreature.VisualContact)
                    {
                        num4 -= 50f;
                        if (self.lizard.Template.CreatureRelationship(self.creatureLooker.lookCreature.representedCreature.creatureTemplate).type == CreatureTemplate.Relationship.Type.Eats || self.lizard.Template.CreatureRelationship(self.creatureLooker.lookCreature.representedCreature.creatureTemplate).intensity > 0.5f)
                        {
                            self.eyeBeamsActive = 1f;
                        }
                        else
                        {
                            self.eyeBeamsActive = Mathf.Clamp(self.eyeBeamsActive + 1f / 30f, 0f, 1f);
                        }
                    }
                    else
                    {
                        self.eyeBeamsActive = Mathf.Clamp(self.eyeBeamsActive - 0.0125f, 0f, 1f);
                    }
                }
                else
                {
                    self.eyeBeamsActive = Mathf.Clamp(self.eyeBeamsActive - 0.05f, 0f, 1f);
                }


                for (int l = 2; l < 10; l++)
                {
                    Vector2 vector = self.lizard.bodyChunks[0].pos + DirVec(self.lizard.bodyChunks[1].pos, self.lizard.bodyChunks[0].pos) * (300f - l * 20);
                    self.eyes[l, 1] = self.eyes[l, 0];
                    self.eyes[l, 2] = Vector2.Lerp(self.eyes[l, 2], Vector2.ClampMagnitude(self.eyes[l, 2] + Custom.DegToVec(Random.value * 360f) * num4 * 0.5f * Random.value, num4), 0.5f);
                    if (l == 0)
                    {
                        self.eyes[l, 0] = Vector2.Lerp(self.eyes[l, 0], self.lookPos + self.eyes[l, 2], 0.6f);
                    }
                    else
                    {
                        self.eyes[l, 0] = Vector2.Lerp(self.eyes[l, 0], Vector2.Lerp(self.lookPos, vector, Mathf.InverseLerp(25f, 180f, num4)) + self.eyes[l, 2], 0.3f);
                    }

                    if (self.creatureLooker.lookCreature != null && self.creatureLooker.lookCreature.VisualContact)
                    {
                        self.eyes[l, 2] *= 0.5f;
                    }

                    if (Vector2.Dot((self.lizard.bodyChunks[0].pos - self.lizard.bodyChunks[1].pos).normalized, (self.eyes[l, 0] - self.lizard.bodyChunks[1].pos).normalized) < self.lizard.lizardParams.periferalVisionAngle)
                    {
                        self.eyes[l, 0] = Vector2.Lerp(self.eyes[l, 0], vector, 0.1f);
                        self.eyes[l, 2] *= 0.85f;
                    }

                    if (!Custom.DistLess(self.eyes[l, 0], vector, 300f))
                    {
                        self.eyes[l, 0] = Vector2.Lerp(self.eyes[l, 0], vector, 0.1f);
                    }

                    if (Custom.DistLess(self.eyes[l, 0], self.lizard.bodyChunks[0].pos, 100f))
                    {
                        self.eyes[l, 0] = self.lizard.bodyChunks[0].pos + DirVec(self.lizard.bodyChunks[0].pos, self.eyes[l, 0]) * 100f;
                    }
                }
            }
        }

        private static void LizardGraphics_DrawSprites(On.LizardGraphics.orig_DrawSprites orig, LizardGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);
            if (self.lizard.Template.type == ShoggothTemplateType.Shoggoth)
            {
                //self.ColorBody(sLeaser, rCam.PixelColorAtCoordinate(self.lizard.mainBodyChunk.pos));
                sLeaser.sprites[self.SpriteHeadStart + 0].isVisible = false;
                sLeaser.sprites[self.SpriteHeadStart + 1].isVisible = false;
                sLeaser.sprites[self.SpriteHeadStart + 2].isVisible = false;
                float num = Mathf.Lerp(self.lastHeadDepthRotation, self.headDepthRotation, timeStacker);

                int num14 = 3 - (int)(Mathf.Abs(num) * 3.9f);

                sLeaser.sprites[self.SpriteHeadStart + 4].element = Futile.atlasManager.GetElementWithName("ShoggothEyes" + num14);

            }

        }


        private static Color LizardGraphics_HeadColor(On.LizardGraphics.orig_HeadColor orig, LizardGraphics self, float timeStacker)
        {
            if (self.lizard.Template.type == ShoggothTemplateType.Shoggoth)
            {
                float a = 1f - Mathf.Pow(0.5f + 0.5f * Mathf.Sin(Mathf.Lerp(self.lastBlink, self.blink, timeStacker) * 2f * (float)Math.PI), 1.5f + self.lizard.AI.excitement * 1.5f);
                if (self.headColorSetter != 0f)
                {
                    a = Mathf.Lerp(a, self.headColorSetter > 0f ? 1f : 0f, Mathf.Abs(self.headColorSetter));
                }

                if (self.flicker > 10)
                {
                    a = self.flickerColor;
                }

                a = Mathf.Lerp(a, Mathf.Pow(Mathf.Max(0f, Mathf.Lerp(self.lastVoiceVisualization, self.voiceVisualization, timeStacker)), 0.75f), Mathf.Lerp(self.lastVoiceVisualizationIntensity, self.voiceVisualizationIntensity, timeStacker));
                return Color.Lerp(Color.Lerp(self.HeadColor2, Color.black, 0.3f), self.HeadColor2, a);
            }
            return orig.Invoke(self, timeStacker);

        }

        private static void LizardGraphics_ApplyPalette(On.LizardGraphics.orig_ApplyPalette orig, LizardGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            orig.Invoke(self, sLeaser, rCam, palette);
            if (self.lizard.Template.type == ShoggothTemplateType.Shoggoth)
            {
                self.ColorBody(sLeaser, Color.Lerp(self.effectColor, Color.black, 0.6f));
            }
        }

        private static void LizardGraphics_ctor(On.LizardGraphics.orig_ctor orig, LizardGraphics self, PhysicalObject ow)
        {
            orig.Invoke(self, ow);
            if (self.lizard.Template.type == ShoggothTemplateType.Shoggoth)
            {

                self.eyes = new Vector2[10, 3];

                self.cosmetics.Clear();
                self.extraSprites = 0;

                // 安全备份原有肢体
                var oldLimbs = self.limbs;
                var bodyPartsList = self.bodyParts.ToList();

                // 创建新数组（6条腿）
                self.limbs = new LizardLimb[12];
                Array.Resize(ref self.lizard.LizardState.limbHealth, 12);
                for (int i = 0; i < self.lizard.LizardState.limbHealth.Length; i++)
                {
                    self.lizard.LizardState.limbHealth[i] = 1f;
                }
                // 复制原有四肢
                for (int i = 0; i < oldLimbs.Length; i++)
                {
                    self.limbs[i] = oldLimbs[i];
                }

                // 添加新肢体（从index 4开始，即第5、6条腿）
                for (int i = oldLimbs.Length; i < self.limbs.Length; i++)
                {
                    // 连接点分配：偶数腿连到chunk0，奇数腿连到chunk2
                    int connectChunk = i % 2 == 0 ? 0 : 2;

                    self.limbs[i] = new LizardLimb(
                        self,
                        self.owner.bodyChunks[connectChunk],
                        i,
                        2.5f + Random.Range(-0.25f, 0.25f),
                        0.7f + Random.Range(-0.25f, 0.25f),
                        0.99f + Random.Range(-0.25f, 0.25f),
                        self.lizard.lizardParams.limbSpeed + Random.Range(-0.25f, 0.25f),
                        self.lizard.lizardParams.limbQuickness + Random.Range(-0.25f, 0.25f),
                        i % 2 == 1 ? self.limbs[i - 1] : null
                    );

                    // 在原有最后一个肢体后插入新肢体
                    bodyPartsList.Insert(
                        bodyPartsList.IndexOf(oldLimbs[oldLimbs.Length - 1]) + 1,
                        self.limbs[i]
                    );
                }

                int num = self.startOfExtraSprites;
                num = self.AddCosmetic(num, new BigWhiskers(self, num));
                num = self.AddCosmetic(num, new BigAxolotlGills(self, num));
                num = self.AddCosmetic(num, new WingScales(self, num));
                for (int num9 = 0; num9 < 5; num9++)
                {
                    //num = self.AddCosmetic(num, new ShortBodyScales(self, num));
                    num = self.AddCosmetic(num, new TailTuft(self, num));
                }


                //num = self.AddCosmetic(num, new HeadExtraEyes(self, num));
                //num = self.AddCosmetic(num, new BodyExtraEyes(self, num));

                self.bodyParts = bodyPartsList.ToArray();
            }
        }

        private static void Lizard_ctor(On.Lizard.orig_ctor orig, Lizard self, AbstractCreature abstractCreature, World world)
        {
            orig.Invoke(self, abstractCreature, world);
            if (self.Template.type == ShoggothTemplateType.Shoggoth)
            {
                self.effectColor = Custom.HSL2RGB(Custom.WrappedRandomVariation(0.2222f, 0.1111f, 0.6f), Custom.ClampedRandomVariation(0.7f, 0.3f, 0.1f), Custom.ClampedRandomVariation(0.075f, 0.1f, 0.1f));
            }
        }

        private static CreatureTemplate LizardBreeds_BreedTemplate_Type_CreatureTemplate_CreatureTemplate_CreatureTemplate_CreatureTemplate(On.LizardBreeds.orig_BreedTemplate_Type_CreatureTemplate_CreatureTemplate_CreatureTemplate_CreatureTemplate orig, CreatureTemplate.Type type, CreatureTemplate lizardAncestor, CreatureTemplate pinkTemplate, CreatureTemplate blueTemplate, CreatureTemplate greenTemplate)
        {
            bool flag = type == ShoggothTemplateType.Shoggoth;
            CreatureTemplate result;
            if (flag)
            {
                List<TileTypeResistance> list = new List<TileTypeResistance>();
                List<TileConnectionResistance> list2 = new List<TileConnectionResistance>();
                CreatureTemplate creatureTemplate = orig.Invoke(CreatureTemplate.Type.BlueLizard, lizardAncestor, pinkTemplate, blueTemplate, greenTemplate);
                CreatureTemplate creatureTemplate2 = new CreatureTemplate(type, lizardAncestor, list, list2, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Ignores, 0f));
                LizardBreedParams lizardBreedParams = creatureTemplate.breedParameters as LizardBreedParams;
                lizardBreedParams.template = type;
                lizardBreedParams.baseSpeed = 2.6f;
                lizardBreedParams.terrainSpeeds[1] = new LizardBreedParams.SpeedMultiplier(1f, 1f, 1f, 1f);
                list.Add(new TileTypeResistance((AItile.Accessibility)1, 1f, 0));
                lizardBreedParams.terrainSpeeds[2] = new LizardBreedParams.SpeedMultiplier(1f, 1f, 1f, 1f);
                list.Add(new TileTypeResistance((AItile.Accessibility)2, 1.1f, 0));
                lizardBreedParams.terrainSpeeds[3] = new LizardBreedParams.SpeedMultiplier(1f, 1f, 1f, 1f);
                list.Add(new TileTypeResistance((AItile.Accessibility)3, 1f, 0));
                lizardBreedParams.terrainSpeeds[4] = new LizardBreedParams.SpeedMultiplier(0.9f, 0.9f, 0.9f, 0.9f);
                list.Add(new TileTypeResistance((AItile.Accessibility)4, 0.9f, 0));
                lizardBreedParams.terrainSpeeds[5] = new LizardBreedParams.SpeedMultiplier(0.9f, 0.9f, 0.9f, 0.9f);
                list.Add(new TileTypeResistance((AItile.Accessibility)5, 0.9f, 0));
                list2.Add(new TileConnectionResistance((MovementConnection.MovementType)6, 20f, 0));
                list2.Add(new TileConnectionResistance((MovementConnection.MovementType)7, 2f, 0));
                list2.Add(new TileConnectionResistance((MovementConnection.MovementType)13, 15f, 0));
                list2.Add(new TileConnectionResistance((MovementConnection.MovementType)1, 1.1f, 0));
                list2.Add(new TileConnectionResistance((MovementConnection.MovementType)2, 1.1f, 0));
                list2.Add(new TileConnectionResistance((MovementConnection.MovementType)4, 1.1f, 0));
                list2.Add(new TileConnectionResistance((MovementConnection.MovementType)12, 2f, 0));
                list2.Add(new TileConnectionResistance((MovementConnection.MovementType)6, 100f, (PathCost.Legality)4));
                list2.Add(new TileConnectionResistance((MovementConnection.MovementType)9, 60f, 0));
                lizardBreedParams.biteDelay = 5;//啃咬延迟
                lizardBreedParams.biteInFront = 25f;//啃咬距离
                lizardBreedParams.biteRadBonus = 20f;//啃咬半径
                lizardBreedParams.biteHomingSpeed = 1f;//啃咬追击速度
                lizardBreedParams.biteChance = 0.8f;//咬死几率
                lizardBreedParams.attemptBiteRadius = 220f;//尝试咬合半径
                lizardBreedParams.getFreeBiteChance = 1f;//咬空几率（？）
                lizardBreedParams.biteDamage = 0f;//啃咬伤害
                lizardBreedParams.biteDamageChance = 0.5f;
                lizardBreedParams.toughness = 1.5f;
                lizardBreedParams.stunToughness = 8f;
                lizardBreedParams.regainFootingCounter = 1;
                lizardBreedParams.bodyMass = 3f;
                lizardBreedParams.bodySizeFac = 1.2f;
                lizardBreedParams.bodyLengthFac = 1.2f;
                lizardBreedParams.floorLeverage = 5f;
                lizardBreedParams.maxMusclePower = 15f;
                lizardBreedParams.wiggleSpeed = 0.2f;
                lizardBreedParams.wiggleDelay = 20;
                lizardBreedParams.bodyStiffnes = 0.1f;
                lizardBreedParams.swimSpeed = 2.0f;
                lizardBreedParams.idleCounterSubtractWhenCloseToIdlePos = 10;
                lizardBreedParams.danger = 1f;//
                lizardBreedParams.aggressionCurveExponent = 0.7f;
                lizardBreedParams.headShieldAngle = 0f;
                lizardBreedParams.canExitLounge = false;
                lizardBreedParams.canExitLoungeWarmUp = true;
                lizardBreedParams.loungeDistance = 200f;
                lizardBreedParams.preLoungeCrouch = 20;
                lizardBreedParams.preLoungeCrouchMovement = -0.4f;//
                lizardBreedParams.loungeSpeed = 8.0f;
                lizardBreedParams.loungeMaximumFrames = 10;
                lizardBreedParams.loungePropulsionFrames = 5;
                lizardBreedParams.loungeJumpyness = 0.5f;
                lizardBreedParams.loungeDelay = 15;
                lizardBreedParams.loungeTendensy = 1f;
                lizardBreedParams.riskOfDoubleLoungeDelay = 0.1f;
                lizardBreedParams.postLoungeStun = 0;
                lizardBreedParams.perfectVisionAngle = Mathf.Lerp(1.5f, -1.5f, 0);
                lizardBreedParams.periferalVisionAngle = Mathf.Lerp(1.5f, -1.5f, 1);
                lizardBreedParams.biteDominance = 1f;
                lizardBreedParams.limbSize = 1.5f;
                lizardBreedParams.stepLength = 0.3f;
                lizardBreedParams.liftFeet = 0.3f;
                lizardBreedParams.feetDown = 0.5f;
                lizardBreedParams.noGripSpeed = 0.1f;
                lizardBreedParams.limbSpeed = 6f;
                lizardBreedParams.limbQuickness = 1f;
                lizardBreedParams.limbGripDelay = 1;
                lizardBreedParams.smoothenLegMovement = true;
                lizardBreedParams.legPairDisplacement = 0.3f;
                lizardBreedParams.walkBob = 1.5f;
                lizardBreedParams.tailSegments = 10;//尾巴段数
                lizardBreedParams.tailStiffness = 5f;
                lizardBreedParams.tailStiffnessDecline = 0.25f;
                lizardBreedParams.tailLengthFactor = 0.8f;
                lizardBreedParams.tailColorationStart = 0f;
                lizardBreedParams.tailColorationExponent = 8f;
                lizardBreedParams.headSize = 1.2f;
                lizardBreedParams.neckStiffness = 0.7f;
                lizardBreedParams.jawOpenAngle = 0f;
                lizardBreedParams.jawOpenLowerJawFac = 0.7666667f;
                lizardBreedParams.jawOpenMoveJawsApart = 15f;
                lizardBreedParams.framesBetweenLookFocusChange = 20;
                lizardBreedParams.tamingDifficulty = 999f;
                lizardBreedParams.standardColor = colorr;
                creatureTemplate.visualRadius = 3000f;
                creatureTemplate.waterVision = 0.7f;
                creatureTemplate.throughSurfaceVision = 0.95f;
                creatureTemplate.waterPathingResistance = 1f;
                creatureTemplate.dangerousToPlayer = lizardBreedParams.danger;
                creatureTemplate.doPreBakedPathing = false;
                creatureTemplate.requireAImap = true;
                creatureTemplate.meatPoints = 5;
                creatureTemplate.preBakedPathingAncestor = StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.YellowLizard);
                creatureTemplate.type = type;
                //creatureTemplate.ConnectionResistance(MovementConnection.MovementType.ReachUp).resistance = 50f;
                creatureTemplate.name = "Shoggoth";
                creatureTemplate.waterRelationship = CreatureTemplate.WaterRelationship.AirAndSurface;
                creatureTemplate.baseDamageResistance = 2.7f;
                result = creatureTemplate;
            }
            else
            {
                result = orig.Invoke(type, lizardAncestor, pinkTemplate, blueTemplate, greenTemplate);
            }
            return result;
        }

        private static void LizardLimb_ctor(On.LizardLimb.orig_ctor orig, LizardLimb self, GraphicsModule owner, BodyChunk connectionChunk, int num, float rad, float sfFric, float aFric, float huntSpeed, float quickness, LizardLimb otherLimbInPair)
        {
            orig.Invoke(self, owner, connectionChunk, num, rad, sfFric, aFric, huntSpeed, quickness, otherLimbInPair);
            LizardGraphics lizardGraphics = owner as LizardGraphics;
            bool flag = lizardGraphics != null;
            if (flag)
            {
                Lizard lizard = lizardGraphics.lizard;
                bool flag2 = (lizard != null ? lizard.Template.type : null) == ShoggothTemplateType.Shoggoth;
                if (flag2)
                {
                    self.grabSound = SoundID.Lizard_Green_Foot_Grab;
                    self.releaseSeound = SoundID.Lizard_Green_Foot_Release;
                }
            }
        }

        private static SoundID LizardVoice_GetMyVoiceTrigger(On.LizardVoice.orig_GetMyVoiceTrigger orig, LizardVoice self)
        {
            SoundID result = orig.Invoke(self);
            Lizard lizard = self.lizard;
            bool flag = lizard != null && lizard.Template.type == ShoggothTemplateType.Shoggoth;
            if (flag)
            {
                string[] array = new string[]
                {
                        "A",
                        "B",
                        "C",
                        "D",
                        "E"
                };
                List<SoundID> list = new List<SoundID>();
                for (int i = 0; i < array.Length; i++)
                {
                    SoundID soundID = SoundID.None;
                    string text = "Lizard_Voice_Pink_" + array[i];
                    bool flag2 = ExtEnum<SoundID>.values.entries.Contains(text);
                    if (flag2)
                    {
                        soundID = new SoundID(text, false);
                    }
                    bool flag3 = soundID != SoundID.None && soundID.Index != -1 && lizard.abstractCreature.world.game.soundLoader.workingTriggers[soundID.Index];
                    if (flag3)
                    {
                        list.Add(soundID);
                    }
                }
                bool flag4 = list.Count == 0;
                if (flag4)
                {
                    result = SoundID.None;
                }
                else
                {
                    result = list[Random.Range(0, list.Count)];
                }
            }
            return result;
        }
        public static float getVariable(EntityID lizId, string name)
        {
            return lizInfo[string.Format("{0}{1}", name, lizId.ToString())];
        }

        public static void setVariable(EntityID lizId, string name, float newVal)
        {
            lizInfo[string.Format("{0}{1}", name, lizId.ToString())] = newVal;
        }

        public static Vector2 DirVec(Vector2 p1, Vector2 p2)
        {
            if (p1 == p2)
            {
                return new Vector2(0f, 1f);
            }

            return new Vector2(p2[0] - p1[0], p2[1] - p1[1]).normalized;
        }

    }
}
