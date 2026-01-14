using ExtremeBrith.Slugcat.DeathKnell;
using RWCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ExtremeBrith.Slugcat.DeathKnell.DeathKnell_Hook;
using Random = UnityEngine.Random;

namespace ExtremeBrith.Object.Ball
{
    public class BoneBall : Weapon
    {
        public Color[] BallColors;
        public SoundID[] Sounds;
        public float Carrymass = 0.4f;
        public float Origimass = 1.5f;
        public float DeteriorationCounter;
        public int originalGrasb;
        public int TerrainImpactCounter;
        public class BallMode : ExtEnum<Mode>
        {
            public static Mode Default = new Mode("Deafult", register: true);
            public static Mode Attact = new Mode("Attact", register: true);
            public static Mode LowAttact = new Mode("LowAttact", register: true);
            public static Mode HighAttact = new Mode("HighAttact", register: true);
            public static Mode TurnAttact = new Mode("TurnAttact", register: true);
            public static Mode FlipAttact = new Mode("FlipAttact", register: true);
            public static Mode SlideAttact = new Mode("SlideAttact", register: true);
            public static Mode RollAttact = new Mode("RollAttact", register: true);
            public static Mode Throw = new Mode("FlipThrow", register: true);
            public static Mode LedgeCrawlThrow = new Mode("LedgeCrawlThrow", register: true);

            public BallMode(string value, bool register = false)
                : base(value, register)
            {

            }
        }

        public int AttackCounter;
        public bool Attack;
        public Vector2Int AttackDir;
        public Color? SparkColor { get; set; }
        public float Radius { get; set; }
        public int Grabbedlevel { get; set; }
        public float GetfoodPerHit { get; set; }
        public bool canDeterioration { get; set; }
        public float deteriorationTime { get; set; }
        public bool isRollwhenConnected { get; set; }
        public float Rotation { get; set; }
        public float LastRotation { get; set; }
        public Player Owner { get; set; }
        public Player Connecter { get; set; }
        public Mode Attackmode { get; set; }
        public override bool HeavyWeapon => true;

        public float roll;

        public override int DefaultCollLayer => 0;
        public BoneBall(AbstractBall abstractPhysicalObject, World world)
    : base(abstractPhysicalObject, world)
        {
            isRollwhenConnected = false;
            canDeterioration = true;
            deteriorationTime = 10;
            GetfoodPerHit = 0.5f;
            Grabbedlevel = 3;

            collisionLayer = 0;
            canBeHitByWeapons = true;
            Radius = 13f;
            bodyChunks = new BodyChunk[1];
            bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), Radius, Origimass);

            bodyChunks[0].lastPos = bodyChunks[0].pos;
            bodyChunks[0].collideWithTerrain = true;
            bodyChunks[0].collideWithSlopes = true;
            canBeHitByWeapons = true;
            bodyChunkConnections = new BodyChunkConnection[0];
            CollideWithObjects = true;
            CollideWithSlopes = true;
            CollideWithTerrain = true;
            airFriction = 0.999f;
            gravity = 0.9f;
            bounce = 0.1f;
            surfaceFriction = 0.4f;
            waterFriction = 0.94f;
            buoyancy = 0.99f;
            Attackmode = BallMode.Default;
            BallColors = new Color[5]
            {
                new Color(1f,0.784f,0.745f),
                new Color(1f,0.784f,0.745f),
                new Color(1f,0.784f,0.745f),
                new Color(1f,0.784f,0.745f),
                abstractPhysicalObject.color,
            };
            Sounds = new SoundID[2]
                {
                    SoundID.Rock_Hit_Creature,
                    SoundID.Rock_Hit_Wall,
                };
        }
        public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            for (int i = 0; i < 4; i++)
            {
                rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[i]);//线
            }



            rCam.ReturnFContainer("Items").AddChild(sLeaser.sprites[4]);//本体
        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                Color color = Color.green;
                if (BallColors.Length > i)
                    color = BallColors[i];

                sLeaser.sprites[i].color = color;

            }

        }
        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            Vector2 vector = default;
            float rota = Mathf.Lerp(LastRotation, Rotation, timeStacker);
            vector.x = Mathf.Lerp(firstChunk.lastPos.x, firstChunk.pos.x, timeStacker) - camPos.x;
            vector.y = Mathf.Lerp(firstChunk.lastPos.y, firstChunk.pos.y, timeStacker) - camPos.y;
            for (int i = 0; i < 4; i++)
            {
                Vector2 origPoint = default;
                if (isRollwhenConnected)
                {
                    origPoint = vector + Custom.DegToVec(rota + i * 90f) * Radius / 4;
                }
                else
                {
                    origPoint = vector + Custom.DegToVec(rota - 30 + i * 20f) * Radius;

                }
                Vector2 aimPoint = vector;
                if (Connecter != null && Connecter.bodyChunks.Length >= 7)
                {
                    aimPoint.x = Mathf.Lerp(Connecter.bodyChunks[6].lastPos.x, Connecter.bodyChunks[6].pos.x, timeStacker) - camPos.x;
                    aimPoint.y = Mathf.Lerp(Connecter.bodyChunks[6].lastPos.y, Connecter.bodyChunks[6].pos.y, timeStacker) - camPos.y;
                }
                Vector2 linePos = (origPoint + aimPoint) / 2;
                float lineRotation = Custom.VecToDeg(aimPoint - origPoint);

                sLeaser.sprites[i].x = linePos.x;
                sLeaser.sprites[i].y = linePos.y;
                sLeaser.sprites[i].rotation = lineRotation;
                sLeaser.sprites[i].scaleY = Custom.Dist(aimPoint, origPoint) / 64f;
                sLeaser.sprites[i].scaleX = 0.5f;


            }


            for (int i = 4; i < sLeaser.sprites.Length; i++)
            {
                sLeaser.sprites[i].x = vector.x;
                sLeaser.sprites[i].y = vector.y;
                sLeaser.sprites[i].rotation = Rotation;

            }

            if (slatedForDeletetion || room != rCam.room)
            {
                sLeaser.CleanSpritesAndRemove();
            }
        }
        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[5];
            for (int i = 0; i < 4; i++)
            {
                sLeaser.sprites[i] = new FSprite("ConnectLine");
            }
            sLeaser.sprites[4] = new FSprite("BoneBall");
            AddToContainer(sLeaser, rCam, null);
        }

        public override void Update(bool eu)
        {
            IntVector2 contactPoint = firstChunk.ContactPoint;

            base.Update(eu);
            LastRotation = Rotation;
            Rotation = Rotation + roll;
            TerrainImpactCounter = Mathf.Max(0, TerrainImpactCounter - 1);

            TryBlock(firstChunk.pos, Radius * 2);


            if (Connecter != null)//设定持有者为连接者
            {
                Owner = Connecter;
                bool hasstuck = false;
                foreach (var item in abstractPhysicalObject.stuckObjects)
                {
                    if (item.A == Connecter.abstractPhysicalObject || item.B == Connecter.abstractPhysicalObject)
                    {
                        hasstuck = true;
                    }
                }
                if (!hasstuck)
                {
                    if (DeathKnellModuleManeger.PlayerModules.TryGetValue(Connecter, out var module))
                    {
                        module.BallStick = new AbstractObjectOnTail(Connecter.abstractCreature, abstractPhysicalObject);
                    }
                }
            }
            else if (Attackmode == BallMode.Default && grabbedBy.Count <= 0)
            {
                Owner = null;
            }


            if (contactPoint != new IntVector2(0, 0) && grabbedBy.Count == 0)//滚地旋转
            {
                roll = firstChunk.vel.x * (float)Math.PI * 0.7f * (0f - Mathf.Sign(contactPoint.y));

            }
            if (Connecter != null && !isRollwhenConnected)//如果连接，自动面向尾部
            {
                if (Attackmode == BallMode.FlipAttact)
                {
                    if (grabbedBy.Count == 0)
                    {
                        if (Connecter.bodyChunks.Length >= 7)
                        {
                            float BallAimRotation = Custom.VecToDeg(Connecter.bodyChunks[5].pos - Connecter.bodyChunks[6].pos);

                            Rotation = BallAimRotation;
                            roll = 0;
                        }
                    }

                }
                else if (Attackmode == BallMode.Throw)
                {
                    float BallAimRotation = Custom.VecToDeg(firstChunk.vel) + 180;

                    Rotation = BallAimRotation;
                    roll = 0;
                }
                else
                {
                    if (grabbedBy.Count == 0)
                    {
                        if (Connecter.bodyChunks.Length >= 7)
                        {
                            float BallAimRotation = Custom.VecToDeg(Connecter.bodyChunks[6].pos - firstChunk.pos);

                            roll += (BallAimRotation - Rotation) / 3;

                        }
                    }
                    else
                    {
                        Rotation = Custom.VecToDeg(Connecter.bodyChunks[6].pos - firstChunk.pos);
                        roll = 0;
                    }
                }
            }
            else
            {
                if (Attackmode == BallMode.FlipAttact)
                {
                    if (grabbedBy.Count == 0)
                    {
                        float BallAimRotation = Custom.VecToDeg(Owner.bodyChunks[0].pos - Owner.bodyChunks[1].pos);

                        Rotation = BallAimRotation;
                        roll = 0;
                    }

                }

            }
            roll *= 0.8f;

            bool catchbyDeath = false;//减轻重量
            foreach (var item in grabbedBy)
            {
                if (item.grabber != null && item.grabber is Player && (item.grabber as Player).slugcatStats.name.value == "DeathKnell")
                {
                    catchbyDeath = true;
                    break;
                }
            }

            if (catchbyDeath)
            {
                firstChunk.mass = Carrymass;
            }
            else
            {
                firstChunk.mass = Origimass;
            }



            if (Connecter != null && Attackmode == BallMode.Default)//自动释放
            {
                if (Connecter.animation == Player.AnimationIndex.BellySlide)
                {
                    Attackmode = BallMode.SlideAttact;
                    foreach (var item in grabbedBy)
                    {
                        item.Release();
                    }
                }
            }
            if (Owner != null && Attackmode == BallMode.Default)
            {
                if (Owner.animation == Player.AnimationIndex.Flip)
                {
                    Attackmode = BallMode.FlipAttact;
                    if (Owner.grasps[0] != null && Owner.grasps[0].grabbed != null && Owner.grasps[0].grabbed == this)
                    {
                        originalGrasb = 1;
                    }
                    if (Owner.grasps[1] != null && Owner.grasps[1].grabbed != null && Owner.grasps[1].grabbed == this)
                    {
                        originalGrasb = -1;
                    }

                    foreach (var item in grabbedBy)
                    {
                        item.Release();
                    }
                }
                if (Owner.animation == Player.AnimationIndex.Roll)
                {
                    Attackmode = BallMode.RollAttact;
                    if (Owner.grasps[0] != null && Owner.grasps[0].grabbed != null && Owner.grasps[0].grabbed == this)
                    {
                        originalGrasb = 1;
                    }
                    if (Owner.grasps[1] != null && Owner.grasps[1].grabbed != null && Owner.grasps[1].grabbed == this)
                    {
                        originalGrasb = -1;
                    }
                    foreach (var item in grabbedBy)
                    {
                        item.Release();
                    }
                }

            }


            if (Connecter != null)
            {
                Connecter.bodyChunkConnections[6].distance = Radius;
            }




            if (Attackmode == BallMode.Attact)//攻击模式
            {
                if (Owner == null)
                {
                    Attackmode = BallMode.Default;
                }
                else
                {
                    AttackCounter += 15;
                    firstChunk.vel += getForce(firstChunk.pos, Owner.firstChunk.pos, 30, 0.4f, 2, 1);
                    ChangeMode(Mode.Thrown);
                    thrownBy = Owner;
                    if (AttackCounter == 0)
                    {
                        firstChunk.pos = getAttactPoint(Owner, AttackCounter, AttackDir);
                        firstChunk.vel = Vector2.zero;
                    }
                    if (AttackCounter < 120)
                    {
                        firstChunk.vel += getForce(firstChunk.pos, getAttactPoint(Owner, AttackCounter, AttackDir), 4, 0.4f, 2, 1);
                    }
                    if (AttackCounter >= 180)
                    {
                        AttackCounter = 0;
                        ReturntoHand();
                    }
                }
            }
            else if (Attackmode == BallMode.LowAttact)
            {
                if (Owner == null)
                {
                    Attackmode = BallMode.Default;
                }
                else
                {
                    AttackCounter += 10;
                    firstChunk.vel += getForce(firstChunk.pos, Owner.firstChunk.pos, 30, 0.4f, 2, 1);
                    ChangeMode(Mode.Thrown);
                    thrownBy = Owner;
                    if (AttackCounter < 90)
                    {
                        firstChunk.vel += getForce(firstChunk.pos, getLowAttactPoint(Owner, AttackCounter, AttackDir), 4, 0.4f, 2, 1);
                    }
                    if (AttackCounter >= 90)
                    {
                        AttackCounter = 0;
                        ReturntoHand();
                    }
                }
            }
            else if (Attackmode == BallMode.HighAttact)
            {
                if (Owner == null)
                {
                    Attackmode = BallMode.Default;
                }
                else
                {
                    AttackCounter += 10;
                    firstChunk.vel += getForce(firstChunk.pos, Owner.firstChunk.pos, 30, 0.4f, 2, 1);
                    ChangeMode(Mode.Thrown);
                    thrownBy = Owner;
                    if (AttackCounter == 30)
                    {
                        Owner.firstChunk.vel += Vector2.up * 20;
                    }
                    if (AttackCounter < 90)
                    {
                        firstChunk.vel += getForce(firstChunk.pos, getHighAttactPoint(Owner, AttackCounter, AttackDir), 4, 0.4f, 2, 1);
                    }
                    if (AttackCounter >= 90)
                    {
                        AttackCounter = 0;
                        ReturntoHand();
                    }
                }
            }
            else if (Attackmode == BallMode.TurnAttact)
            {
                if (Owner == null)
                {
                    Attackmode = BallMode.Default;
                }
                else
                {
                    AttackCounter += 1;
                    firstChunk.vel += getForce(firstChunk.pos, Owner.firstChunk.pos, 30, 0.4f, 2, 1);
                    Owner.firstChunk.vel += getForce(Owner.firstChunk.pos, firstChunk.pos, 30, 0.4f, 2, 1) * 0.5f;
                    firstChunk.vel *= 0.8f;
                    Owner.firstChunk.vel.x *= 0.5f;
                    ChangeMode(Mode.Thrown);
                    thrownBy = Owner;
                    if (AttackCounter == 0)
                    {
                        firstChunk.pos = getTurnAttactPoint(Owner, AttackCounter, AttackDir);
                        firstChunk.vel = Vector2.zero;
                    }
                    if (AttackCounter < 14)
                    {
                        firstChunk.vel += getForce(firstChunk.pos, getTurnAttactPoint(Owner, AttackCounter, AttackDir), 4, 0.4f, 2, 1);
                    }
                    if (AttackCounter >= 10 && AttackCounter <= 14)
                    {
                        if (Owner.input[0].thrw)
                        {
                            AttackCounter = 1;
                        }
                    }
                    if (AttackCounter >= 14)
                    {
                        AttackCounter = 0;
                        ReturntoHand();
                    }
                }

            }
            else if (Attackmode == BallMode.FlipAttact)
            {
                if (Connecter == null)
                {
                    if (Owner == null)
                    {
                        Attackmode = BallMode.Default;

                    }
                    else
                    {
                        if (Owner.animation != Player.AnimationIndex.Flip)
                        {
                            AttackCounter--;
                            if (AttackCounter <= 0)
                            {
                                ReturntoHand();

                            }
                        }
                        else
                        {
                            firstChunk.vel += getForce(firstChunk.pos, getRollAttactPoint(Owner, AttackCounter, AttackDir), 4, 0.4f, 2, 1);

                            ChangeMode(Mode.Thrown);
                            thrownBy = Owner;
                        }
                    }

                }
                else
                {
                    if (Connecter.animation != Player.AnimationIndex.Flip)
                    {
                        AttackCounter--;
                        if (AttackCounter <= 0)
                        {
                            ReturntoHand();

                        }
                    }
                    else
                    {
                        if (Connecter.input[0].thrw)
                        {
                            Attackmode = BallMode.Throw;
                            Vector2 throwdir = new Vector2(Owner.input[0].x, Owner.input[0].y);
                            firstChunk.vel += throwdir * 20f;
                        }
                        ChangeMode(Mode.Thrown);
                        thrownBy = Owner;

                    }
                }
            }
            else if (Attackmode == BallMode.SlideAttact)
            {
                if (Connecter == null)
                {
                    Attackmode = BallMode.Default;
                }
                else
                {
                    if (Connecter.animation != Player.AnimationIndex.BellySlide)
                    {
                        AttackCounter--;
                        if (Connecter.animation == Player.AnimationIndex.RocketJump)
                        {
                            AttackCounter = 0;
                            ReturntoHand();
                        }
                        if (AttackCounter <= 0)
                        {
                            Attackmode = BallMode.Default;

                        }
                        firstChunk.vel += (Connecter.bodyChunks[0].pos - Connecter.bodyChunks[1].pos).normalized * 2f;

                    }
                    else
                    {
                        AttackCounter = 15;
                    }
                    if (Connecter.input[0].thrw)
                    {
                        Attackmode = BallMode.Throw;
                        Vector2 throwdir = new Vector2(Connecter.input[0].x, Connecter.input[0].y);
                        firstChunk.vel += throwdir * 20f;
                    }
                    ChangeMode(Mode.Thrown);
                    thrownBy = Connecter;

                }

            }
            else if (Attackmode == BallMode.RollAttact)
            {
                if (Owner == null)
                {
                    Attackmode = BallMode.Default;
                }
                else
                {
                    if (Owner.animation != Player.AnimationIndex.Roll)
                    {
                        AttackCounter--;
                        if (Owner.animation == Player.AnimationIndex.RocketJump || Connecter == null)
                        {
                            AttackCounter = 0;
                            ReturntoHand();
                        }
                        if (AttackCounter <= 0)
                        {
                            Attackmode = BallMode.Default;

                        }
                        firstChunk.vel += (Owner.bodyChunks[0].pos - Owner.bodyChunks[1].pos).normalized * 2f;

                    }
                    else
                    {
                        AttackCounter = 15;
                    }
                    firstChunk.vel += getForce(firstChunk.pos, getRollAttactPoint(Owner, AttackCounter, AttackDir), 4, 0.4f, 2, 1);

                    ChangeMode(Mode.Thrown);
                    thrownBy = Owner;

                }

            }
            else if (Attackmode == BallMode.Throw)
            {
                if (Connecter != null)
                {
                    thrownBy = Connecter;
                    ChangeMode(Mode.Thrown);
                    if (Connecter.bodyChunks[6] != null)
                    {
                        firstChunk.vel += getForce(firstChunk.pos, Connecter.bodyChunks[6].pos, 9999, 0.05f, 150, 1f);
                    }
                    else
                    {
                        firstChunk.vel += getForce(firstChunk.pos, Connecter.bodyChunks[1].pos, 9999, 0.05f, 120, 1f);
                    }
                    if (firstChunk.vel.magnitude <= 0.2f)
                    {
                        Attackmode = BallMode.Default;
                    }
                }


            }
            else
            {
                AttackCounter = 0;
                thrownBy = Connecter;
                if (firstChunk.vel.magnitude >= 20)
                {
                    ChangeMode(Mode.Thrown);
                    Attackmode = BallMode.Throw;
                }
            }
        }

        public void ReturntoHand()
        {
            Attackmode = BallMode.Default;
            firstChunk.vel = Vector2.zero;

            if (Owner != null && CanIPickThisBall(Owner))
            {
                firstChunk.pos = Owner.firstChunk.pos;
                ChangeMode(Mode.Carried);
                if (AttackDir.x > 0)
                {
                    for (int i = Owner.grasps.Length - 1; i < Owner.grasps.Length; i--)
                    {
                        if (Owner.grasps[i] == null || Owner.grasps[i].grabbed == null)
                        {
                            bool grabed = Owner.Grab(this, i, 0, Creature.Grasp.Shareability.CanOnlyShareWithNonExclusive, 0.5f, true, false);
                            if (grabed)
                            {
                                break;
                            }
                        }
                    }

                }
                else
                {
                    for (int i = 0; i < Owner.grasps.Length; i++)
                    {
                        if (Owner.grasps[i] == null || Owner.grasps[i].grabbed == null)
                        {
                            bool grabed = Owner.Grab(this, i, 0, Creature.Grasp.Shareability.CanOnlyShareWithNonExclusive, 0.5f, true, false);
                            if (grabed)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        public Vector2 getAttactPoint(Player owner, int counter, Vector2Int dir)
        {
            Vector2 aimpoint = default(Vector2Int);
            if (dir.x >= 0)
            {
                aimpoint = owner.mainBodyChunk.pos + Custom.DegToVec(-Custom.VecToDeg(dir) + counter + 30) * 25;
            }
            else
            {
                aimpoint = owner.mainBodyChunk.pos + Custom.DegToVec(Custom.VecToDeg(dir) - counter + 160) * 25;

            }
            return aimpoint;
        }

        public Vector2 getLowAttactPoint(Player owner, int counter, Vector2Int dir)
        {
            Vector2 aimpoint = default(Vector2Int);
            aimpoint = owner.mainBodyChunk.pos + (Vector2)dir * (80 - counter * 0.8f);
            return aimpoint;

        }

        public Vector2 getHighAttactPoint(Player owner, int counter, Vector2Int dir)
        {
            Vector2 aimpoint = default(Vector2Int);
            aimpoint = owner.mainBodyChunk.pos + Vector2.up * (40 - counter * 0.4f);
            return aimpoint;

        }

        public Vector2 getTurnAttactPoint(Player owner, int counter, Vector2Int dir)
        {
            Vector2 aimpoint = default(Vector2Int);
            if (counter <= 5)
            {
                aimpoint = owner.mainBodyChunk.pos + (Vector2)dir * 30;
            }
            else if (counter <= 15)
            {
                aimpoint = owner.mainBodyChunk.pos + (Vector2)dir * -20;

            }
            else
            {
                aimpoint = owner.mainBodyChunk.pos + (Vector2)dir * 20;
            }
            return aimpoint;

        }


        public Vector2 getRollAttactPoint(Player owner, int counter, Vector2Int dir)
        {
            Vector2 aimpoint = default(Vector2Int);
            if (originalGrasb >= 0)
            {
                aimpoint = owner.mainBodyChunk.pos + (owner.bodyChunks[1].pos - owner.bodyChunks[0].pos).normalized * 30;
            }
            else
            {
                aimpoint = owner.mainBodyChunk.pos + (owner.bodyChunks[0].pos - owner.bodyChunks[1].pos).normalized * 30;
            }
            return aimpoint;

        }

        public virtual bool TryBlock(Vector2 pos, float rad)
        {
            if (room != null)
            {
                foreach (var item in room.updateList)
                {
                    if (item is Weapon && (item as Weapon).mode == Mode.Thrown)
                    {
                        if (item is BoneBall)
                        {
                            if ((item as BoneBall).Owner != Owner)
                            {
                                BoneBall ball = item as BoneBall;
                                if ((ball.firstChunk.pos - pos).magnitude <= rad || (ball.firstChunk.pos + ball.firstChunk.vel - pos).magnitude <= rad)
                                {
                                    firstChunk.vel += ball.firstChunk.vel;
                                    ball.firstChunk.vel = (ball.firstChunk.pos - pos).normalized * ball.firstChunk.vel;
                                    ball.firstChunk.pos += (ball.firstChunk.pos - pos).normalized * (rad - (ball.firstChunk.pos - pos).magnitude);
                                    Attackmode = BallMode.Default;
                                    ChangeMode(Mode.Free);
                                    (item as BoneBall).Attackmode = BallMode.Default;
                                    ball.ChangeMode(Mode.Free);
                                    for (int m = 0; m < 4; m++)
                                    {
                                        room.AddObject(new Spark(ball.firstChunk.pos, ball.firstChunk.vel * Random.value + Custom.RNV() * Random.value, new Color(1f, 1f, 1f), null, 6, 18));
                                    }
                                    room.PlaySound(SoundID.Weapon_Skid, firstChunk.pos, 1f, 1f);
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            Weapon weapon = item as Weapon;
                            if ((weapon.firstChunk.pos - pos).magnitude <= rad || (weapon.firstChunk.pos + weapon.firstChunk.vel - pos).magnitude <= rad)
                            {
                                if (Owner != null)
                                {
                                    Owner.firstChunk.vel += (item as Weapon).firstChunk.vel * 0.3f;
                                }
                                firstChunk.vel += (item as Weapon).firstChunk.vel * 0.3f;
                                weapon.firstChunk.vel = (weapon.firstChunk.pos - pos).normalized * weapon.firstChunk.vel;
                                weapon.firstChunk.pos += (weapon.firstChunk.pos - pos).normalized * (rad - (weapon.firstChunk.pos - pos).magnitude);
                                weapon.ChangeMode(Mode.Free);
                                for (int m = 0; m < 4; m++)
                                {
                                    room.AddObject(new Spark(weapon.firstChunk.pos, weapon.firstChunk.vel * Random.value + Custom.RNV() * Random.value, new Color(1f, 1f, 1f), null, 6, 18));
                                }
                                room.PlaySound(SoundID.Weapon_Skid, firstChunk.pos, 1f, 1f);
                                return true;

                            }
                        }
                    }
                }
            }

            return false;
        }


        public override bool HitSomething(SharedPhysics.CollisionResult result, bool eu)
        {
            if (result.obj == null || result.chunk == null)
            {
                return false;
            }

            if (result.obj is Creature)
            {
                float stunBonus = firstChunk.vel.magnitude * Origimass;
                float HurtBouns = firstChunk.vel.magnitude * 0.06f * firstChunk.mass;
                Vector2 SparkDir = default;
                Vector2 CreatureDir = default;

                if (Attackmode == BallMode.Attact)//确定血液飞溅方向
                {
                    SparkDir = Vector2.up;
                    CreatureDir = Vector2.up * 30f;
                }
                else if (Attackmode == BallMode.LowAttact)
                {
                    SparkDir = (Vector2)AttackDir;
                    CreatureDir = (Vector2)AttackDir * 10f;
                }
                else if (Attackmode == BallMode.HighAttact)
                {
                    SparkDir = Vector2.up;
                    CreatureDir = Vector2.up * 30f;
                }
                else if (Attackmode == BallMode.FlipAttact)
                {
                    SparkDir = firstChunk.vel.normalized;
                    CreatureDir = firstChunk.vel * 2;
                }
                else if (Attackmode == BallMode.TurnAttact)
                {
                    SparkDir = firstChunk.vel.normalized;
                    CreatureDir = firstChunk.vel * 0.5f;
                }
                else if (Attackmode == BallMode.SlideAttact)
                {
                    SparkDir = firstChunk.vel.normalized;
                    CreatureDir = firstChunk.vel * 0.5f;
                }
                else if (Attackmode == BallMode.RollAttact)
                {
                    SparkDir = firstChunk.vel.normalized;
                    CreatureDir = firstChunk.vel * 0.5f;
                }
                else if (Attackmode == BallMode.Throw)
                {
                    SparkDir = firstChunk.vel.normalized;
                }
                room.PlaySound(Sounds[0], firstChunk.pos, firstChunk.vel.magnitude / 20f, 1);

                if (Attackmode == BallMode.Throw)
                {
                    stunBonus *= 2f;
                }


                for (int i = 0; i < HurtBouns * 50f + Random.Range(10, -10); i++)
                {
                    /*                    if (SparkColor != null)
                                        {
                                            room.AddObject(new Spark(result.collisionPoint, Custom.DegToVec(Custom.VecToDeg(SparkDir) + Random.Range(-25, 25)) * Random.Range(10, 40), SparkColor.Value, null, 10, 20));
                                        }
                                        else
                                        {
                                            room.AddObject(new Spark(result.collisionPoint, Custom.DegToVec(Custom.VecToDeg(SparkDir) + Random.Range(-25, 25)) * Random.Range(10, 40), (result.obj as Creature).ShortCutColor(), null, 10, 20));
                                        }
                    */
                    if (SparkColor != null)
                    {
                        room.AddObject(new PlayerBubble(result.chunk, Custom.DegToVec(Custom.VecToDeg(SparkDir) + Random.Range(-25, 25)) * Random.Range(10, 40), HurtBouns, 0.1f, 5, SparkColor.Value, 5, 50));
                    }
                    else
                    {
                        room.AddObject(new PlayerBubble(result.chunk, Custom.DegToVec(Custom.VecToDeg(SparkDir) + Random.Range(-25, 25)) * Random.Range(10, 40), HurtBouns, 0.1f, 5, (result.obj as Creature).ShortCutColor(), 5, 50));
                    }
                }
                result.chunk.vel += CreatureDir / 2.5f * Origimass;
                (result.obj as Creature).Violence(firstChunk, firstChunk.vel * firstChunk.mass, result.chunk, result.onAppendagePos, Creature.DamageType.Blunt, HurtBouns, stunBonus);
            }
            else
            {
                result.chunk.vel += (result.collisionPoint - Owner.firstChunk.pos).normalized * firstChunk.vel.magnitude / result.obj.TotalMass;
                room.PlaySound(SoundID.Spear_Bounce_Off_Wall, firstChunk);
                if (result.obj is SeedCob)
                {
                    (result.obj as SeedCob).AbstractCob.opened = true;
                    (result.obj as SeedCob).AbstractCob.dead = true;
                    (result.obj as SeedCob).AbstractCob.minCycles = 1;
                    (result.obj as SeedCob).AbstractCob.maxCycles = 3;
                    (result.obj as SeedCob).AbstractCob.Consume();
                    (result.obj as SeedCob).canBeHitByWeapons = false;
                    (result.obj as SeedCob).bodyChunks[0].vel += Custom.RNV() * 2f;
                    (result.obj as SeedCob).AbstractCob.spawnedUtility = true;
                    for (int i = 0; i < 4; i++)
                    {
                        AbstractConsumable abstractConsumable = new AbstractConsumable(room.world, DLCSharedEnums.AbstractObjectType.Seed, null, room.GetWorldCoordinate((result.obj as SeedCob).placedPos), room.game.GetNewID(), -1, -1, null);
                        room.abstractRoom.AddEntity(abstractConsumable);
                        abstractConsumable.pos = room.GetWorldCoordinate((result.obj as SeedCob).placedPos);
                        abstractConsumable.RealizeInRoom();
                        //abstractConsumable.realizedObject.firstChunk.HardSetPosition(Vector2.Lerp(base.bodyChunks[0].pos, base.bodyChunks[1].pos, (float)i / 5f));
                        abstractConsumable.realizedObject.firstChunk.vel += Custom.RNV() * Random.Range(5f, 15f);
                    }


                    if (Connecter != null)
                    {
                        for (int i = 0; i < GetfoodPerHit * 5 / 0.25; i++)
                        {
                            Connecter.AddQuarterFood();
                        }
                    }
                }
            }
            return true;
        }


        public override void Thrown(Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
        {
            base.Thrown(thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
            if (thrownBy is Player && ((thrownBy as Player).slugcatStats.name.value == "DeathKnell" || firstChunk.mass <= 0.5f))
            {
                if (AttackCounter == 0 && Owner != null)
                {
                    firstChunk.vel *= 0;
                    if (thrownBy != null)
                    {
                        if (Owner.animation == Player.AnimationIndex.Flip)
                        {
                            Attackmode = BallMode.Throw;
                            Vector2 throwdir = new Vector2(Owner.input[0].x, Owner.input[0].y);
                            Owner.firstChunk.vel += throwdir * 10;
                            firstChunk.vel += throwdir * 10f;

                        }
                        else if (Owner.animation == Player.AnimationIndex.RocketJump)
                        {
                            Attackmode = BallMode.Throw;
                            Vector2 throwdir = new Vector2(Owner.input[0].x, Owner.input[0].y);
                            Owner.firstChunk.vel += throwdir * 10;
                            if (Connecter != null)
                            {
                                firstChunk.vel += throwdir * 40f;
                            }
                            else
                            {
                                firstChunk.vel += throwdir * 10f;

                            }


                        }
                        else if (Owner.bodyMode == Player.BodyModeIndex.Stand || Owner.bodyMode == Player.BodyModeIndex.Default)
                        {
                            if (Owner.input[0].y > 0)
                            {
                                Attackmode = BallMode.HighAttact;

                            }
                            else if (Owner.slideCounter > 0)
                            {
                                Attackmode = BallMode.TurnAttact;
                            }
                            else
                            {
                                Attackmode = BallMode.Attact;
                            }
                        }
                        else if (Owner.bodyMode == Player.BodyModeIndex.Swimming || Owner.bodyMode == Player.BodyModeIndex.ClimbingOnBeam)
                        {
                            Attackmode = BallMode.Attact;
                        }
                        else if (Owner.bodyMode == Player.BodyModeIndex.ClimbIntoShortCut)
                        {
                            Attackmode = BallMode.LowAttact;
                        }
                        else if (Owner.bodyMode == Player.BodyModeIndex.Crawl)
                        {
                            Attackmode = BallMode.LowAttact;
                        }
                        AttackDir = new Vector2Int(throwDir.x, throwDir.y);
                    }
                }
            }
            else
            {
                firstChunk.vel *= 0.1f;
            }
        }

        public override void HitWall()
        {
            if (room.BeingViewed)
            {
                for (int i = 0; i < 7; i++)
                {
                    room.AddObject(new Spark(firstChunk.pos + throwDir.ToVector2() * (firstChunk.rad - 1f), Custom.DegToVec(Random.value * 360f) * 20f * Random.value + -throwDir.ToVector2() * 10f, new Color(1f, 1f, 1f), null, 2, 4));
                }
            }

            room.ScreenMovement(firstChunk.pos, throwDir.ToVector2() * 1.5f, 0f);
            SetRandomSpin();
            forbiddenToPlayer = 10;
        }


        public override void TerrainImpact(int chunk, IntVector2 direction, float speed, bool firstContact)
        {
            base.TerrainImpact(chunk, direction, speed, firstContact);
            if (firstChunk.vel.magnitude >= 20f && TerrainImpactCounter <= 0)
            {
                TerrainImpactCounter = Attackmode == BallMode.FlipAttact ? 10 : 25;
                room.PlaySound(Sounds[1], firstChunk.pos, firstChunk.vel.magnitude / 20f, 1);
            }
        }
        public override void PlaceInRoom(Room placeRoom)
        {
            base.PlaceInRoom(placeRoom);
            for (int i = 0; i < bodyChunks.Length; i++)
            {
                bodyChunks[i].HardSetPosition(placeRoom.MiddleOfTile(abstractPhysicalObject.pos));
            }
        }


        public override void PickedUp(Creature upPicker)
        {
            base.PickedUp(upPicker);
            if (upPicker is Player)
            {
                if (Connecter != null && upPicker != Connecter && Connecter.dead)
                {
                    ReleaseTail(Connecter);
                    room.PlaySound(SoundID.Vulture_Grab_Player, firstChunk);
                }
                Owner = upPicker as Player;
            }

        }
        public Vector2 getForce(Vector2 start, Vector2 end, float Elastic_dis, float k, float stop_dis, float f)
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

        public bool CanIPickThisBall(Player self)
        {
            int level = 0;
            foreach (var item in self.grasps)
            {
                if (item != null && item.grabbed != null && item.grabbed is BoneBall)
                {
                    level += (item.grabbed as BoneBall).Grabbedlevel;
                }
            }
            if (Grabbedlevel + level <= 4)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
