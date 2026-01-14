using ExtremeBrith.Object.Ball;
using RWCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static PhysicalObject;

namespace ExtremeBrith.Slugcat.DeathKnell
{




    public class DeathKnell_Hook
    {
        public static bool isCreateClock;
        public static void Hook()
        {
            On.Player.CanIPickThisUp += Player_CanIPickThisUp;//普通玩家无法抓起重的球
            On.Player.Update += Player_Update;
            On.Player.Update += Player_skill;
            On.Player.ctor += Player_ctor;
            On.Creature.Violence += Creature_Violence;//玩家尾骨免伤
            On.Spear.HitSomething += Spear_HitSomething;//玩家弹开矛

        }
        private static void Creature_Violence(On.Creature.orig_Violence orig, Creature self, BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, PhysicalObject.Appendage.Pos hitAppendage, Creature.DamageType type, float damage, float stunBonus)
        {
            if (hitChunk != null && hitChunk.owner != null&&hitChunk.owner is Player && (hitChunk.owner as Player).slugcatStats.name.value == "DeathKnell")
            {
                Player player = hitChunk.owner as Player;
                bool isBoneTile = false;
                for (int i = 2; i < player.bodyChunks.Length; i++)
                {
                    if (hitChunk == player.bodyChunks[i])
                    {
                        isBoneTile = true;
                        break;
                    }
                }
                if (isBoneTile)
                {
                    self.room.PlaySound(SoundID.Rock_Bounce_Off_Creature_Shell);
                    hitChunk.vel += source.vel;
                    source.vel = -0.3f * source.vel + Custom.RNV() * 5;

                }
                else
                {
                    orig.Invoke(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                }
            }
            else
            {
                orig.Invoke(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
            }
        }
        private static bool Spear_HitSomething(On.Spear.orig_HitSomething orig, Spear self, SharedPhysics.CollisionResult result, bool eu)
        {
            if (result.obj != null && result.obj is Player && (result.obj as Player).slugcatStats.name.value == "DeathKnell")
            {
                Player player = result.obj as Player;

                bool isBoneTile = false;
                for (int i = 2; i < player.bodyChunks.Length; i++)
                {
                    if (result.chunk == player.bodyChunks[i])
                    {
                        isBoneTile = true;
                        break;
                    }
                }
                if (isBoneTile)
                {
                    self.room.PlaySound(SoundID.Spear_Bounce_Off_Creauture_Shell);
                    result.chunk.vel += self.firstChunk.vel;
                    self.firstChunk.vel = -0.3f * self.firstChunk.vel + Custom.RNV() * 5;
                    for (int i = 0; i < UnityEngine.Random.Range(5, 7); i++)
                    {
                        self.room.AddObject(new Spark(self.firstChunk.pos + Custom.DegToVec(UnityEngine.Random.value * 360f) * 5f * UnityEngine.Random.value, self.firstChunk.vel * -0.1f + Custom.DegToVec(UnityEngine.Random.value * 360f) * Mathf.Lerp(0.2f, 0.4f, UnityEngine.Random.value) * self.firstChunk.vel.magnitude, new Color(1f, 1f, 1f), null, 10, 170));
                    }
                    self.mode = Spear.Mode.Free;
                    return false;
                }
                else
                {
                    return orig.Invoke(self, result, eu);
                }
            }
            else
            {
                return orig.Invoke(self, result, eu);
            }
        }
        private static bool Player_CanIPickThisUp(On.Player.orig_CanIPickThisUp orig, Player self, PhysicalObject obj)
        {
            if (self.slugcatStats.name.value == "DeathKnell")
            {
                if (obj is BoneBall)
                {
                    int level = 0;
                    foreach (var item in self.grasps)
                    {
                        if (item != null && item.grabbed != null && item.grabbed is BoneBall)
                        {
                            level += (item.grabbed as BoneBall).Grabbedlevel;
                        }
                    }
                    if ((obj as BoneBall).Grabbedlevel + level <= 3)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (obj is Creature)
                {
                    if ((obj as Creature).stun > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return orig.Invoke(self, obj);
                    }
                }
                else
                {
                    return orig.Invoke(self, obj);
                }
            }
            else
            {
                if (obj is BoneBall && (obj as BoneBall).Owner != null && !(obj as BoneBall).Owner.dead) 
                {
                    return false;
                }
                return orig.Invoke(self, obj);
            }
        }

        public static void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig.Invoke(self, abstractCreature, world);
            if (self.slugcatStats.name.value == "DeathKnell")
            {
                DeathKnellModuleManeger.PlayerModules.Add(self, new DeathKnellModuleManeger.PlayerModule(self));
                DeathKnellModuleManeger.PlayerModules.TryGetValue(self, out var module);

                isCreateClock = false;
                module.ConnectedBall = null;
            }
        }
        private static void Player_skill(On.Player.orig_Update orig, Player self, bool eu)
        {
            bool isFlip = self.animation == Player.AnimationIndex.Flip;
            orig.Invoke(self, eu);
            if (self.slugcatStats.name.value == "DeathKnell")
            {
                if (!isFlip && self.animation == Player.AnimationIndex.Flip)
                {
                    self.firstChunk.vel += Vector2.up * 5f;
                }
                if (self.dead)
                {
                    self.bodyChunkConnections[1].weightSymmetry = 0.5f;
                    self.bodyChunkConnections[2].weightSymmetry = 0.5f;
                    self.bodyChunkConnections[3].weightSymmetry = 0.5f;
                    self.bodyChunkConnections[4].weightSymmetry = 0.5f;
                    self.bodyChunkConnections[5].weightSymmetry = 0.5f;
                }
                else
                {
                    self.bodyChunkConnections[1].weightSymmetry = 0f;
                    self.bodyChunkConnections[2].weightSymmetry = 0.3f;
                    self.bodyChunkConnections[3].weightSymmetry = 0.3f;
                    self.bodyChunkConnections[4].weightSymmetry = 0.3f;
                    self.bodyChunkConnections[5].weightSymmetry = 0.3f;
                }
            }
        }

        public static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig.Invoke(self, eu);
            if (self.slugcatStats.name.value == "DeathKnell")
            {
                DeathKnellModuleManeger.PlayerModules.TryGetValue(self, out var module);

                if (!isCreateClock)//生成钟
                {
                    isCreateClock = true;
                    Plugin.Log($"Create a Ball to{self}");
                    AbstractBall A = new(self.room.world, AbstractBall.BallType.SmallBoneBall, self, self.abstractCreature.pos, self.room.game.GetNewID());
                    self.room.abstractRoom.AddEntity(A);
                    A.RealizeInRoom();
                    A.realizedObject.firstChunk.pos = self.firstChunk.pos;
                    AbstractBall B = new(self.room.world, AbstractBall.BallType.MiddleBoneBall, self, self.abstractCreature.pos, self.room.game.GetNewID());
                    self.room.abstractRoom.AddEntity(B);
                    B.RealizeInRoom();
                    B.realizedObject.firstChunk.pos = self.firstChunk.pos;
                    AbstractBall C = new(self.room.world, AbstractBall.BallType.BigBoneBall, self, self.abstractCreature.pos, self.room.game.GetNewID());
                    self.room.abstractRoom.AddEntity(C);
                    C.RealizeInRoom();
                    C.realizedObject.firstChunk.pos = self.firstChunk.pos;



                    ConnectWith(self, C.realizedObject as BoneBall);

                    module.ConnectedBall = C.realizedObject as BoneBall;
                }



                /*                self.bodyChunks[2].pos = self.bodyChunks[1].pos;//连接玩家和尾巴
                                if (self.animation != Player.AnimationIndex.Flip)
                                {
                                    self.bodyChunks[1].vel += self.bodyChunks[2].vel * 0.3f;

                                }
                                self.bodyChunks[2].vel = Vector2.zero;*/


                if (module.ConnectedBall != null)//连接尾巴和钟(如果有的话)
                {
                    BoneBall clock = module.ConnectedBall as BoneBall;

                    if (clock.Attackmode == BoneBall.BallMode.Default || clock.Attackmode == BoneBall.BallMode.FlipAttact)
                    {
                        self.bodyChunkConnections[6].elasticity = 0.9f;
                    }
                    else
                    {
                        self.bodyChunkConnections[6].elasticity = 0.1f;
                    }
                }

            }
        }
        public static Vector2 getForce(Vector2 start, Vector2 end, float Elastic_dis, float k, float stop_dis, float f)
        {
            Vector2 dir = new Vector2(end[0] - start[0], end[1] - start[1]).normalized;
            float dis = Custom.Dist(start, end);
            if (dis < stop_dis)
            {
                return Vector2.zero;
            }
            else if (dis < Elastic_dis)
            {
                return dir * f;
            }
            else
            {
                return dir * (dis - Elastic_dis) * k + dir * f;
            }
        }

        public static void ReleaseTail(Player self)
        {
            if (self != null && self.slugcatStats.name.value == "DeathKnell" && DeathKnellModuleManeger.PlayerModules.TryGetValue(self, out var module))
            {
                if (module.BallStick != null)
                {
                    module.BallStick.Deactivate();
                    self.room.PlaySound(SoundID.Slugcat_Bite_Centipede, self.bodyChunks[6]);
                    for (int m = 0; m < 7; m++)
                    {
                        self.room.AddObject(new Spark(module.ConnectedBall.firstChunk.pos, Custom.RNV() * UnityEngine.Random.value * 3f, new Color(1f, 1f, 1f), null, 6, 18));
                    }
                }
                if (module.ConnectedBall != null && module.ConnectedBall.Connecter != null)
                {
                    module.ConnectedBall.Connecter = null;
                    module.ConnectedBall = null;
                }
                BodyChunkConnection chunkConnection6 = new(self.bodyChunks[6], self.bodyChunks[6], 13, BodyChunkConnection.Type.Pull, 0.8f, 0.3f);
                self.bodyChunkConnections[6] = chunkConnection6;

            }
        }
        public static void ConnectWith(Player self, BoneBall Connected)
        {
            if (self != null && Connected != null && self.slugcatStats.name.value == "DeathKnell" && DeathKnellModuleManeger.PlayerModules.TryGetValue(self, out var module))
            {
                if (module.ConnectedBall != null)
                    ReleaseTail(self);
                module.ConnectedBall = Connected;
                Connected.Connecter = self;
                BodyChunkConnection chunkConnection6 = new(self.bodyChunks[6], Connected.firstChunk, 13, BodyChunkConnection.Type.Pull, 0.8f, 0.3f);
                self.bodyChunkConnections[6] = chunkConnection6;
                self.room.PlaySound(SoundID.Tube_Worm_Detatch_Tongue_Creature, self.bodyChunks[6]);
                for (int m = 0; m < 7; m++)
                {
                    self.room.AddObject(new Spark(Connected.firstChunk.pos, Custom.RNV() * UnityEngine.Random.value * 3f, new Color(1f, 1f, 1f), null, 6, 18));
                }
                module.BallStick = new AbstractObjectOnTail(self.abstractCreature, Connected.abstractPhysicalObject);

            }
        }
        public static BoneBall GetBallinHand(Player self)
        {
            if (self != null)
            {
                foreach (var item in self.grasps)
                {
                    if (item != null && item.grabbed != null && item.grabbed is BoneBall)
                    {
                        return (BoneBall)item.grabbed;
                    }
                }
            }

            return null;
        }

        public class AbstractObjectOnTail : AbstractPhysicalObject.AbstractObjectStick
        {
            public AbstractObjectOnTail(AbstractPhysicalObject player, AbstractPhysicalObject abobject)
            : base(player, abobject)
            {

            }
        }
    }
}
