using Noise;
using RWCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ExtremeBrith
{
    public class CreatureFearer : UpdatableAndDeletable
    {
        public int counter;
        public Creature crit;
        public Object.TwistedSpear.TwistedSpear Spear;
        public CreatureFearer(Object.TwistedSpear.TwistedSpear spear, Creature crit, int duration)
        {
            this.crit = crit;
            Spear = spear;
            counter = duration;
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            counter--;
            if (Spear == null)
            {
                Destroy();
            }
            else
            {
                if (counter < 1)
                {
                    Destroy();
                    if (Spear.stuckInObject == crit)
                    {
                        Spear.ChangeMode(Weapon.Mode.Free);
                        Explode();
                    }
                    return;
                }
            }

            if (crit != null && crit.abstractCreature != null && crit.abstractCreature.abstractAI != null && crit.abstractCreature.abstractAI.RealAI != null && crit.abstractCreature.abstractAI.RealAI.threatTracker != null)
            {
                MakeCreatureLeaveRoom(crit.abstractCreature.abstractAI.RealAI);
            }

            if (Random.value <= 0.02f)
            {
                crit.Stun(50);
            }

            Vector2 vector = Custom.RNV();
            for (int i = 0; i < crit.bodyChunks.Length; i++)
            {
                vector = Vector3.Slerp(-vector.normalized, Custom.RNV(), Random.value);
                vector *= Mathf.Min(3f, Random.value * 3f / Mathf.Lerp(crit.bodyChunks[i].mass, 1f, 0.5f)) * Mathf.InverseLerp(0f, 160f, counter);
                crit.bodyChunks[i].pos += vector;
                crit.bodyChunks[i].vel += vector * 0.5f;
            }

            if (crit.graphicsModule == null || crit.graphicsModule.bodyParts == null)
            {
                return;
            }

            for (int j = 0; j < crit.graphicsModule.bodyParts.Length; j++)
            {
                vector = Vector3.Slerp(-vector.normalized, Custom.RNV(), Random.value);
                vector *= Random.value * 2f * Mathf.InverseLerp(0f, 120f, counter);
                crit.graphicsModule.bodyParts[j].pos += vector;
                crit.graphicsModule.bodyParts[j].vel += vector;
                if (crit.graphicsModule.bodyParts[j] is Limb)
                {
                    (crit.graphicsModule.bodyParts[j] as Limb).mode = Limb.Mode.Dangle;
                }
            }
        }
        public void MakeCreatureLeaveRoom(ArtificialIntelligence AI)
        {
            if (AI.creature.abstractAI.destination.room != room.abstractRoom.index)
            {
                Plugin.Log("return");
                return;
            }

            int num = AI.threatTracker.FindMostAttractiveExit();
            if (num > -1 && num < room.abstractRoom.nodes.Length && room.abstractRoom.nodes[num].type == AbstractRoomNode.Type.Exit)
            {
                int num2 = room.world.GetAbstractRoom(room.abstractRoom.connections[num]).ExitIndex(room.abstractRoom.index);
                if (num2 > -1)
                {
                    Custom.Log("migrate");
                    AI.creature.abstractAI.MigrateTo(new WorldCoordinate(room.abstractRoom.connections[num], -1, -1, num2));
                }
            }
        }

        public void Explode()
        {
            if (Spear == null)
            {
                Destroy();
                return;
            }
            Vector2 vector = Spear.firstChunk.pos + Spear.rotation * (Spear.pivotAtTip ? 0f : 10f);
            room.AddObject(new SootMark(room, vector, 50f, bigSprite: false));
            room.AddObject(new Explosion(room, Spear, vector, 5, 110f, 5f, 1.1f, 60f, 0.3f, Spear.thrownBy, 0.8f, 0f, 0.7f));
            for (int i = 0; i < 14; i++)
            {
                room.AddObject(new Explosion.ExplosionSmoke(vector, Custom.RNV() * 5f * Random.value, 1f));
            }

            room.AddObject(new Explosion.ExplosionLight(vector, 160f, 1f, 3, new Color(0.99f, 1, 1)));
            room.AddObject(new ExplosionSpikes(room, vector, 9, 4f, 5f, 5f, 90f, new Color(0.99f, 1, 1)));
            room.AddObject(new ShockWave(vector, 60f, 0.045f, 4));
            for (int j = 0; j < 20; j++)
            {
                Vector2 vector2 = Custom.RNV();
                room.AddObject(new Spark(vector + vector2 * Random.value * 40f, vector2 * Mathf.Lerp(4f, 30f, Random.value), new Color(0.99f, 1, 1), null, 4, 18));
            }

            room.ScreenMovement(vector, default, 0.7f);

            room.PlaySound(SoundID.Fire_Spear_Explode, vector, Spear.abstractPhysicalObject);
            room.InGameNoise(new InGameNoise(vector, 8000f, Spear, 1f));
        }

    }


}
