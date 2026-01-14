using ExtremeBrith.Object.Ball;
using RWCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ExtremeBrith.Object.Ball.BoneBall;
using Random = UnityEngine.Random;

namespace ExtremeBrith.Object.GlodenMask
{
    internal class GlodenMask : VultureMask
    {
        public GlodenMask(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
        {
        }



        public override void Update(bool eu)
        {
            base.Update(eu);
            if (grabbedBy.Count > 0)
            {
                if (TryBlock(firstChunk.pos, 50))
                {
                    foreach (var item in grabbedBy)
                    {
                        item.Release();
                    }
                }
            }
        }
        public virtual bool TryBlock(Vector2 pos, float rad)
        {
            if (room != null)
            {
                foreach (var item in room.updateList)
                {
                    if (item is Weapon && (item as Weapon).mode == Weapon.Mode.Thrown)
                    {
                        if (item is BoneBall)
                        {
                            if ((item as BoneBall).Owner != grabbedBy[0].grabber)
                            {
                                BoneBall ball = item as BoneBall;
                                if ((ball.firstChunk.pos - pos).magnitude <= rad || (ball.firstChunk.pos + ball.firstChunk.vel - pos).magnitude <= rad)
                                {
                                    firstChunk.vel += ball.firstChunk.vel;
                                    ball.firstChunk.vel = (ball.firstChunk.pos - pos).normalized * ball.firstChunk.vel;
                                    ball.firstChunk.pos += (ball.firstChunk.pos - pos).normalized * (rad - (ball.firstChunk.pos - pos).magnitude);
                                    (item as BoneBall).Attackmode = BallMode.Default;
                                    ball.ChangeMode(Weapon.Mode.Free);
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
                                grabbedBy[0].grabber.firstChunk.vel += (item as Weapon).firstChunk.vel * 0.3f;
                                firstChunk.vel += (item as Weapon).firstChunk.vel * 0.3f;
                                weapon.firstChunk.vel = (weapon.firstChunk.pos - pos).normalized * weapon.firstChunk.vel;
                                weapon.firstChunk.pos += (weapon.firstChunk.pos - pos).normalized * (rad - (weapon.firstChunk.pos - pos).magnitude);
                                weapon.ChangeMode(Weapon.Mode.Free);
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
    }
}
