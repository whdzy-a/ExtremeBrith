using RWCustom;
using UnityEngine;

namespace ExtremeBrith.Slugcat.DeathKnell
{

    internal class DeathKnellGraphics
    {
        private static FAtlas atlas;
        public static void Hook()
        {
            On.PlayerGraphics.InitiateSprites += PlayerGraphics_InitiateSprites;
            On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;
            On.PlayerGraphics.ctor += PlayerGraphics_ctor;

        }

        private static void PlayerGraphics_ctor(On.PlayerGraphics.orig_ctor orig, PlayerGraphics self, PhysicalObject ow)
        {
            orig.Invoke(self, ow);
            if ((self.owner as Player).slugcatStats.name.value == "DeathKnell")
            {
                self.tail = new TailSegment[16];
                /*                self.tail[1] = new TailSegment(self, 8f, 7f, self.tail[0], 0.85f, 1f, 0.5f, pullInPreviousPosition: true);
                                self.tail[2] = new TailSegment(self, 6f, 7f, self.tail[1], 0.85f, 1f, 0.5f, pullInPreviousPosition: true);
                                self.tail[3] = new TailSegment(self, 3f, 7f, self.tail[2], 0.85f, 1f, 0.5f, pullInPreviousPosition: true);
                */
                self.tail[0] = new TailSegment(self, 4f, 4f, null, 0.85f, 1f, 1f, pullInPreviousPosition: true);
                self.tail[1] = new TailSegment(self, 2f, 7f, self.tail[0], 0.85f, 1f, 0.5f, pullInPreviousPosition: true);
                self.tail[2] = new TailSegment(self, 1f, 7f, self.tail[1], 0.85f, 1f, 0.5f, pullInPreviousPosition: true);
                self.tail[3] = new TailSegment(self, 0.5f, 7f, self.tail[2], 0.85f, 1f, 0.5f, pullInPreviousPosition: true);

                self.tail[4] = new TailSegment(self, 4f, 4f, null, 0.85f, 1f, 0.5f, pullInPreviousPosition: true);
                self.tail[5] = new TailSegment(self, 2f, 7f, self.tail[4], 0.85f, 1f, 0.5f, pullInPreviousPosition: true);
                self.tail[6] = new TailSegment(self, 1f, 7f, self.tail[5], 0.85f, 1f, 0.5f, pullInPreviousPosition: true);
                self.tail[7] = new TailSegment(self, 0.5f, 7f, self.tail[6], 0.85f, 1f, 0.5f, pullInPreviousPosition: true);

                self.tail[8] = new TailSegment(self, 4f, 4f, null, 0.85f, 1f, 0.5f, pullInPreviousPosition: true);
                self.tail[9] = new TailSegment(self, 2f, 7f, self.tail[8], 0.85f, 1f, 0.5f, pullInPreviousPosition: true);
                self.tail[10] = new TailSegment(self, 1f, 7f, self.tail[9], 0.85f, 1f, 0.5f, pullInPreviousPosition: true);
                self.tail[11] = new TailSegment(self, 0.5f, 7f, self.tail[10], 0.85f, 1f, 0.5f, pullInPreviousPosition: true);

                self.tail[12] = new TailSegment(self, 4f, 4f, null, 0.85f, 1f, 0.5f, pullInPreviousPosition: true);
                self.tail[13] = new TailSegment(self, 2f, 7f, self.tail[12], 0.85f, 1f, 0.5f, pullInPreviousPosition: true);
                self.tail[14] = new TailSegment(self, 1f, 7f, self.tail[13], 0.85f, 1f, 0.5f, pullInPreviousPosition: true);
                self.tail[15] = new TailSegment(self, 0.5f, 7f, self.tail[14], 0.85f, 1f, 0.5f, pullInPreviousPosition: true);
            }
        }

        private static void PlayerGraphics_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            orig.Invoke(self, sLeaser, rCam);
            if ((self.owner as Player).slugcatStats.name.value == "DeathKnell")
            {
                sLeaser.sprites[0].scaleX = 1.4f;
                sLeaser.sprites[1].scaleX = 1.5f;

                TriangleMesh.Triangle[] tris = new TriangleMesh.Triangle[52]
                    {
                new TriangleMesh.Triangle(0, 1, 2),
                new TriangleMesh.Triangle(1, 2, 3),
                new TriangleMesh.Triangle(4, 5, 6),
                new TriangleMesh.Triangle(5, 6, 7),
                new TriangleMesh.Triangle(8, 9, 10),
                new TriangleMesh.Triangle(9, 10, 11),
                new TriangleMesh.Triangle(12, 13, 14),
                new TriangleMesh.Triangle(2, 3, 4),
                new TriangleMesh.Triangle(3, 4, 5),
                new TriangleMesh.Triangle(6, 7, 8),
                new TriangleMesh.Triangle(7, 8, 9),
                new TriangleMesh.Triangle(10, 11, 12),
                new TriangleMesh.Triangle(11, 12, 13),

                new TriangleMesh.Triangle(0, 1, 15),
                new TriangleMesh.Triangle(1, 15, 16),
                new TriangleMesh.Triangle(17, 18, 19),
                new TriangleMesh.Triangle(18, 19, 20),
                new TriangleMesh.Triangle(21, 22, 23),
                new TriangleMesh.Triangle(22, 23, 24),
                new TriangleMesh.Triangle(25 ,26, 27),
                new TriangleMesh.Triangle(15, 16, 17),
                new TriangleMesh.Triangle(16, 17, 18),
                new TriangleMesh.Triangle(19, 20, 21),
                new TriangleMesh.Triangle(20, 21, 22),
                new TriangleMesh.Triangle(23, 24, 25),
                new TriangleMesh.Triangle(24, 25, 26),

                new TriangleMesh.Triangle(0, 1, 28),
                new TriangleMesh.Triangle(1, 28, 29),
                new TriangleMesh.Triangle(30, 31, 32),
                new TriangleMesh.Triangle(31, 32, 33),
                new TriangleMesh.Triangle(34, 35, 36),
                new TriangleMesh.Triangle(35, 36, 37),
                new TriangleMesh.Triangle(38, 39, 40),
                new TriangleMesh.Triangle(28, 29, 30),
                new TriangleMesh.Triangle(29, 30, 31),
                new TriangleMesh.Triangle(32, 33, 34),
                new TriangleMesh.Triangle(33, 34, 35),
                new TriangleMesh.Triangle(36, 37, 38),
                new TriangleMesh.Triangle(37, 38, 39),

                new TriangleMesh.Triangle(0, 1, 41),
                new TriangleMesh.Triangle(1, 41, 42),
                new TriangleMesh.Triangle(43, 44, 45),
                new TriangleMesh.Triangle(44, 45, 46),
                new TriangleMesh.Triangle(47, 48, 49),
                new TriangleMesh.Triangle(48, 49, 50),
                new TriangleMesh.Triangle(51, 52, 53),
                new TriangleMesh.Triangle(41, 42, 43),
                new TriangleMesh.Triangle(42, 43, 44),
                new TriangleMesh.Triangle(45, 46, 47),
                new TriangleMesh.Triangle(46, 47, 48),
                new TriangleMesh.Triangle(49, 50, 51),
                new TriangleMesh.Triangle(50, 51, 52),

                };
                TriangleMesh triangleMesh = new TriangleMesh("Futile_White", tris, customColor: false);
                sLeaser.sprites[2] = triangleMesh;
                self.AddToContainer(sLeaser, rCam, null);
            }
        }
        private static void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);
            if (self.owner != null && self.owner is Player && (self.owner as Player).slugcatStats.name.value == "DeathKnell")
            {
                Player player = self.owner as Player;
                Vector2 force = PerpendicularVector((player.bodyChunks[0].pos - player.bodyChunks[1].pos).normalized);
                self.tail[3].vel += force * 0.08f;
                self.tail[7].vel += force * -0.08f;
                self.tail[11].vel += force * 0.3f;
                self.tail[15].vel += force * -0.3f;

                self.tail[4].connectedPoint = self.drawPositions[1, 0];
                self.tail[8].connectedPoint = self.drawPositions[1, 0];
                self.tail[12].connectedPoint = self.drawPositions[1, 0];

                sLeaser.sprites[0].scale = 1.4f;
                sLeaser.sprites[1].scaleX = 1.5f;

                Vector2 vector = Vector2.Lerp(self.drawPositions[0, 1], self.drawPositions[0, 0], timeStacker);//身体的位置
                Vector2 vector2 = Vector2.Lerp(self.drawPositions[1, 1], self.drawPositions[1, 0], timeStacker);//臀部的位置
                Vector2 vector3 = Vector2.Lerp(self.head.lastPos, self.head.pos, timeStacker);//头的位置
                Vector2 vector4 = (vector2 * 3f + vector) / 4f;//尾巴根部位置
                float num3 = 1f - 0.2f * self.malnourished;
                float num4 = 6f;

                for (int i = 0; i < 4; i++)//第一条
                {
                    Vector2 vector5 = Vector2.Lerp(self.tail[i].lastPos, self.tail[i].pos, timeStacker);//尾巴节位置
                    Vector2 normalized = (vector5 - vector4).normalized;//根部至节方向
                    Vector2 vector6 = PerpendicularVector(normalized);//垂直
                    float num5 = Vector2.Distance(vector5, vector4) / 4f;//根部至尖端距离
                    if (i == 0)
                    {
                        num5 = 0f;
                    }

                    (sLeaser.sprites[2] as TriangleMesh).MoveVertice(i * 4, vector4 - vector6 * num4 * num3 + normalized * num5 - camPos);
                    (sLeaser.sprites[2] as TriangleMesh).MoveVertice(i * 4 + 1, vector4 + vector6 * num4 * num3 + normalized * num5 - camPos);
                    if (i < 3)
                    {
                        (sLeaser.sprites[2] as TriangleMesh).MoveVertice(i * 4 + 2, vector5 - vector6 * self.tail[i].StretchedRad * num3 - normalized * num5 - camPos);
                        (sLeaser.sprites[2] as TriangleMesh).MoveVertice(i * 4 + 3, vector5 + vector6 * self.tail[i].StretchedRad * num3 - normalized * num5 - camPos);
                    }
                    else
                    {
                        (sLeaser.sprites[2] as TriangleMesh).MoveVertice(i * 4 + 2, vector5 - camPos);
                    }

                    num4 = self.tail[i].StretchedRad;
                    vector4 = vector5;
                }

                vector4 = (vector2 * 3f + vector) / 4f;//第二条
                for (int i = 4; i < 8; i++)
                {
                    Vector2 vector5 = Vector2.Lerp(self.tail[i].lastPos, self.tail[i].pos, timeStacker);//尾巴节位置
                    Vector2 normalized = (vector5 - vector4).normalized;//根部至节方向
                    Vector2 vector6 = PerpendicularVector(normalized);//垂直
                    float num5 = Vector2.Distance(vector5, vector4) / 4f;//根部至尖端距离

                    if (i > 4)
                    {
                        (sLeaser.sprites[2] as TriangleMesh).MoveVertice((i - 1) * 4 + 1, vector4 - vector6 * num4 * num3 + normalized * num5 - camPos);
                        (sLeaser.sprites[2] as TriangleMesh).MoveVertice((i - 1) * 4 + 2, vector4 + vector6 * num4 * num3 + normalized * num5 - camPos);
                    }
                    if (i < 7)
                    {
                        (sLeaser.sprites[2] as TriangleMesh).MoveVertice((i - 1) * 4 + 3, vector5 - vector6 * self.tail[i].StretchedRad * num3 - normalized * num5 - camPos);
                        (sLeaser.sprites[2] as TriangleMesh).MoveVertice((i - 1) * 4 + 4, vector5 + vector6 * self.tail[i].StretchedRad * num3 - normalized * num5 - camPos);
                    }
                    else
                    {
                        (sLeaser.sprites[2] as TriangleMesh).MoveVertice((i - 1) * 4 + 3, vector5 - camPos);
                    }

                    num4 = self.tail[i].StretchedRad;
                    vector4 = vector5;
                }

                vector4 = (vector2 * 3f + vector) / 4f;//第三条
                for (int i = 8; i < 12; i++)
                {
                    Vector2 vector5 = Vector2.Lerp(self.tail[i].lastPos, self.tail[i].pos, timeStacker);//尾巴节位置
                    Vector2 normalized = (vector5 - vector4).normalized;//根部至节方向
                    Vector2 vector6 = PerpendicularVector(normalized);//垂直
                    float num5 = Vector2.Distance(vector5, vector4) / 4f;//根部至尖端距离
                    if (i > 8)
                    {
                        (sLeaser.sprites[2] as TriangleMesh).MoveVertice((i - 2) * 4 + 2, vector4 - vector6 * num4 * num3 + normalized * num5 - camPos);
                        (sLeaser.sprites[2] as TriangleMesh).MoveVertice((i - 2) * 4 + 3, vector4 + vector6 * num4 * num3 + normalized * num5 - camPos);
                    }
                    if (i < 11)
                    {
                        (sLeaser.sprites[2] as TriangleMesh).MoveVertice((i - 2) * 4 + 4, vector5 - vector6 * self.tail[i].StretchedRad * num3 - normalized * num5 - camPos);
                        (sLeaser.sprites[2] as TriangleMesh).MoveVertice((i - 2) * 4 + 5, vector5 + vector6 * self.tail[i].StretchedRad * num3 - normalized * num5 - camPos);
                    }
                    else
                    {
                        (sLeaser.sprites[2] as TriangleMesh).MoveVertice((i - 2) * 4 + 4, vector5 - camPos);
                    }

                    num4 = self.tail[i].StretchedRad;
                    vector4 = vector5;
                }

                vector4 = (vector2 * 3f + vector) / 4f;//第四条
                for (int i = 12; i < 16; i++)
                {
                    Vector2 vector5 = Vector2.Lerp(self.tail[i].lastPos, self.tail[i].pos, timeStacker);//尾巴节位置
                    Vector2 normalized = (vector5 - vector4).normalized;//根部至节方向
                    Vector2 vector6 = PerpendicularVector(normalized);//垂直
                    float num5 = Vector2.Distance(vector5, vector4) / 4f;//根部至尖端距离
                    if (i > 12)
                    {
                        (sLeaser.sprites[2] as TriangleMesh).MoveVertice((i - 3) * 4 + 3, vector4 - vector6 * num4 * num3 + normalized * num5 - camPos);
                        (sLeaser.sprites[2] as TriangleMesh).MoveVertice((i - 3) * 4 + 4, vector4 + vector6 * num4 * num3 + normalized * num5 - camPos);
                    }
                    if (i < 15)
                    {
                        (sLeaser.sprites[2] as TriangleMesh).MoveVertice((i - 3) * 4 + 5, vector5 - vector6 * self.tail[i].StretchedRad * num3 - normalized * num5 - camPos);
                        (sLeaser.sprites[2] as TriangleMesh).MoveVertice((i - 3) * 4 + 6, vector5 + vector6 * self.tail[i].StretchedRad * num3 - normalized * num5 - camPos);
                    }
                    else
                    {
                        (sLeaser.sprites[2] as TriangleMesh).MoveVertice((i - 3) * 4 + 5, vector5 - camPos);
                    }

                    num4 = self.tail[i].StretchedRad;
                    vector4 = vector5;
                }


                if (DeathKnell_Hook.GetBallinHand(self.player) == null)//吐骨块变脸
                {
                    int handnum = 0;
                    foreach (var item in self.player.grasps)
                    {
                        if (item == null || item.grabbed == null)
                        {
                            handnum++;
                        }
                    }
                    if (self.player.input[0].pckp && self.player.input[0].y == 0 && self.player.input[0].x == 0 && handnum >= 1 && self.player.objectInStomach == null)
                    {
                        sLeaser.sprites[9].element = Futile.atlasManager.GetElementWithName("FaceB0");
                    }
                }

            }

        }

        public static Vector2 PerpendicularVector(Vector2 v)//沿y = -x反转
        {
            v.Normalize();
            return new Vector2(0f - v.y, v.x);
        }

    }
}
