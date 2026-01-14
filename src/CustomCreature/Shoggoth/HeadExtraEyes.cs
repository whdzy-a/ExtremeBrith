using LizardCosmetics;
using RWCustom;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ExtremeBrith.CustomCreature.Shoggoth
{
    public class HeadExtraEyes : Template
    {
        public GenericBodyPart[] eyes;

        public Vector2[] eyeDirections;

        public Vector2[] eyePositions;

        public int[] eyeSides;

        public float[,] eyeProps;

        public float[,,] eyeLightUp;

        public int amount;

        public HeadExtraEyes(LizardGraphics lGraphics, int startSprite)
            : base(lGraphics, startSprite)
        {
            spritesOverlap = SpritesOverlap.InFront;
            amount = Random.Range(6, 14);
            eyes = new GenericBodyPart[amount];
            eyeSides = new int[amount];
            eyeDirections = new Vector2[amount];
            eyePositions = new Vector2[amount];
            eyeProps = new float[amount, 5];
            eyeLightUp = new float[amount, 2, 2];
            for (int i = 0; i < amount; i++)
            {
                eyes[i] = new GenericBodyPart(lGraphics, 1f, 0.6f, 0.9f, lGraphics.lizard.mainBodyChunk);
                eyeDirections[i] = Custom.DegToVec(Mathf.Lerp(4f, 100f, Random.value));
                if (i == 0)
                {
                    eyePositions[i] = new Vector2(Random.Range(-25f, 5f) * lGraphics.iVars.headSize, Random.Range(-0f, 10f) * lGraphics.iVars.headSize);
                }
                else
                {
                    do
                    {
                        eyePositions[i] = new Vector2(Random.Range(-25f, 5f) * lGraphics.iVars.headSize, Random.Range(-0f, 10f) * lGraphics.iVars.headSize);

                    }
                    while ((eyePositions[i - 1] - eyePositions[i]).magnitude < 6);
                }
                eyeSides[i] = Random.Range(0, 1);
                eyeProps[i, 0] = Custom.ClampedRandomVariation(0.5f, 0.4f, 0.5f) * 40f;
                eyeProps[i, 1] = Mathf.Lerp(-0.5f, 0.8f, Random.value);
                eyeProps[i, 2] = Mathf.Lerp(11f, 720f, Mathf.Pow(Random.value, 1.5f)) / eyeProps[i, 0];
                eyeProps[i, 3] = Random.value;
                eyeProps[i, 4] = Mathf.Lerp(0.6f, 1.2f, Mathf.Pow(Random.value, 1.6f));
                if (i <= 0)
                {
                    continue;
                }

            }

            numberOfSprites = amount * 2;
        }

        public override void Reset()
        {
            base.Reset();
            for (int i = 0; i < amount; i++)
            {
                eyes[i].Reset(AnchorPoint(i, 1f));
            }
        }

        public override void Update()
        {
            for (int i = 0; i < amount; i++)
            {
                eyes[i].vel += whiskerDir(0, i, 1f) * eyeProps[i, 2];
                if (lGraphics.lizard.room.PointSubmerged(eyes[i].pos))
                {
                    eyes[i].vel *= 0.8f;
                }
                else
                {
                    eyes[i].vel.y -= 0.6f;
                }

                eyes[i].Update();
                eyes[i].ConnectToPoint(AnchorPoint(i, 1f), 0, push: false, 0f, lGraphics.lizard.mainBodyChunk.vel, 0f, 0f);
                if (!Custom.DistLess(lGraphics.head.pos, eyes[i].pos, 200f))
                {
                    eyes[i].pos = lGraphics.head.pos;
                }

            }
        }

        public Vector2 whiskerDir(int side, int m, float timeStacker)
        {
            float num = Mathf.Lerp(lGraphics.lastHeadDepthRotation, lGraphics.headDepthRotation, timeStacker);
            return RotateAroundOrigo(new Vector2((side == 0 ? -1f : 1f) * (1f - Mathf.Abs(num)) * eyeDirections[m].x + num * eyeProps[m, 1], eyeDirections[m].y).normalized, Custom.AimFromOneVectorToAnother(Vector2.Lerp(lGraphics.drawPositions[0, 1], lGraphics.drawPositions[0, 0], timeStacker), Vector2.Lerp(lGraphics.head.lastPos, lGraphics.head.pos, timeStacker)));
        }
        public static Vector2 RotateAroundOrigo(Vector2 vec, float degAng)
        {
            degAng *= -(float)Math.PI / 180f;
            float num = Mathf.Cos(degAng);
            float num2 = Mathf.Sin(degAng);
            return new Vector2(num * vec.x - num2 * vec.y, num2 * vec.x + num * vec.y);
        }

        public Vector2 AnchorPoint(int m, float timeStacker)
        {
            int side = 0;
            Vector2 headDir = Custom.DegToVec(lGraphics.HeadRotation(timeStacker));
            Vector2 verticalheadDir = PerpendicularVector(Custom.DegToVec(lGraphics.HeadRotation(timeStacker)));
            if (ModManager.MMF)
            {
                return Vector2.Lerp(lGraphics.head.lastPos, lGraphics.head.pos, timeStacker) + headDir * eyePositions[m].x + verticalheadDir * eyePositions[m].y + whiskerDir(side, m, timeStacker);
            }

            return Vector2.Lerp(lGraphics.head.lastPos, lGraphics.head.pos, timeStacker) + whiskerDir(side, m, timeStacker) * 3f * lGraphics.iVars.headSize;
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            for (int num = startSprite + amount * 2 - 1; num >= startSprite; num--)
            {
                sLeaser.sprites[num] = new FSprite("JetFishEyeA");
            }
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            Vector2 vector = Custom.DegToVec(lGraphics.HeadRotation(timeStacker));
            for (int i = 0; i < amount; i++)
            {
                Vector2 vector2 = Vector2.Lerp(eyes[i].lastPos, eyes[i].pos, timeStacker);
                Vector2 vector3 = whiskerDir(0, i, timeStacker);
                Vector2 vector4 = AnchorPoint(i, timeStacker);
                vector3 = (vector3 + vector).normalized;
                Vector2 vector5 = vector4;
                float num = eyeProps[i, 4];
                float num2 = 1f;
                for (int k = 0; k < 4; k++)
                {
                    Vector2 vector6;
                    if (k < 3)
                    {
                        vector6 = Vector2.Lerp(vector4, vector2, (k + 1) / 4f);
                        vector6 += vector3 * num2 * eyeProps[i, 0] * 0.2f;
                    }
                    else
                    {
                        vector6 = vector2;
                    }

                    num2 *= 0.7f;
                    Vector2 normalized = (vector6 - vector5).normalized;
                    Vector2 vector7 = PerpendicularVector(normalized);
                    float num3 = Vector2.Distance(vector6, vector5) / (k == 0 ? 1f : 5f);
                    float num4 = Custom.LerpMap(k, 0f, 3f, eyeProps[i, 4], 0.5f);
                    /*                    for (int l = k * 4; l < k * 4 + ((k == 3) ? 3 : 4); l++)
                                        {
                                            (sLeaser.sprites[startSprite + i * 2 + j] as TriangleMesh).verticeColors[l] = Color.Lerp(lGraphics.HeadColor(timeStacker), new Color(1f, 1f, 1f), (float)(k - 1) / 2f * Mathf.Lerp(eyeLightUp[i, j, 1], eyeLightUp[i, j, 0], timeStacker));
                                        }*/

                    Vector2 pos = vector5 - vector7 * (num4 + num) * 0.5f + normalized * num3;
                    sLeaser.sprites[startSprite + (i + 1) * 2 - 2].color = Color.Lerp(Color.yellow, lGraphics.effectColor, 0.6f);
                    sLeaser.sprites[startSprite + (i + 1) * 2 - 2].SetPosition(pos - camPos);
                    sLeaser.sprites[startSprite + (i + 1) * 2 - 2].rotation = lGraphics.HeadRotation(timeStacker);
                    sLeaser.sprites[startSprite + (i + 1) * 2 - 2].scaleY = 0.8f;

                    sLeaser.sprites[startSprite + (i + 1) * 2 - 1].color = Color.Lerp(Color.black, lGraphics.effectColor, 0.4f);
                    sLeaser.sprites[startSprite + (i + 1) * 2 - 1].SetPosition(pos + (lGraphics.lookPos - pos).normalized - camPos);
                    sLeaser.sprites[startSprite + (i + 1) * 2 - 1].rotation = lGraphics.HeadRotation(timeStacker);
                    sLeaser.sprites[startSprite + (i + 1) * 2 - 1].scaleX = 0.5f;
                    sLeaser.sprites[startSprite + (i + 1) * 2 - 1].scaleY = 0.25f;

                    num = num4;
                    vector5 = vector6;
                }

            }
        }
        public static Vector2 PerpendicularVector(Vector2 v)
        {
            v.Normalize();
            return new Vector2(0f - v.y, v.x);
        }

    }
}
