using RWCustom;
using System;
using UnityEngine;
using static PhysicalObject;

namespace ExtremeBrith.Slugcat.DeathKnell
{
    public class DeathKnellGraphics_AddSpine
    {
        public static int startindex;
        public static void Hook()
        {
            On.Player.ctor += Player_ctor;
            On.PlayerGraphics.AddToContainer += PlayerGraphics_AddToContainer;
            On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;
            On.PlayerGraphics.InitiateSprites += PlayerGraphics_InitiateSprites;
        }

        private static void PlayerGraphics_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            orig.Invoke(self, sLeaser, rCam);
            if ((self.owner as Player).slugcatStats.name.value == "DeathKnell")
            {
                Custom.LogWarning("has Init sprite");
                startindex = sLeaser.sprites.Length;
                Array.Resize(ref sLeaser.sprites, startindex + 4);
                sLeaser.sprites[startindex] = new FSprite("Spine");
                sLeaser.sprites[startindex + 1] = new FSprite("Spine");
                sLeaser.sprites[startindex + 2] = new FSprite("Spine");
                sLeaser.sprites[startindex + 3] = new FSprite("Spine");
                self.AddToContainer(sLeaser, rCam, null);
            }
        }

        private static void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);
            if ((self.owner as Player).slugcatStats.name.value == "DeathKnell")
            {
                Player player = self.owner as Player;
                for (int i = startindex; i < startindex + 4; i++)
                {
                    Vector2 vector = default;
                    vector.x = Mathf.Lerp((player.bodyChunks[i - startindex + 2].lastPos.x + player.bodyChunks[i - startindex + 3].lastPos.x) / 2, (player.bodyChunks[i - startindex + 2].pos.x + player.bodyChunks[i - startindex + 3].pos.x) / 2, timeStacker) - camPos.x;
                    vector.y = Mathf.Lerp((player.bodyChunks[i - startindex + 2].lastPos.y + player.bodyChunks[i - startindex + 3].lastPos.y) / 2, (player.bodyChunks[i - startindex + 2].pos.y + player.bodyChunks[i - startindex + 3].pos.y) / 2, timeStacker) - camPos.y;

                    sLeaser.sprites[i].color = Plugin.getBoneColor(player);
                    sLeaser.sprites[i].x = vector.x;
                    sLeaser.sprites[i].y = vector.y;
                    sLeaser.sprites[i].scaleY = Custom.Dist(player.bodyChunks[i - startindex + 3].pos, player.bodyChunks[i - startindex + 2].pos) / 55f;
                    sLeaser.sprites[i].scaleX = 0.375f - 0.05f * (i - startindex);
                    sLeaser.sprites[i].rotation = Custom.VecToDeg(player.bodyChunks[i - startindex + 2].pos - player.bodyChunks[i - startindex + 3].pos);
                }
                Vector2 offset = -10 * (player.bodyChunks[0].pos - player.bodyChunks[1].pos).normalized;


            }
        }

        private static void PlayerGraphics_AddToContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            orig.Invoke(self, sLeaser, rCam, newContatiner);
            if ((self.owner as Player).slugcatStats.name.value == "DeathKnell")
            {
                if (startindex > 0 && startindex < sLeaser.sprites.Length)
                {
                    rCam.ReturnFContainer("Background").AddChild(sLeaser.sprites[startindex]);
                    rCam.ReturnFContainer("Background").AddChild(sLeaser.sprites[startindex + 1]);
                    rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[startindex + 2]);
                    rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[startindex + 3]);

                }
            }
        }

        private static void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig.Invoke(self, abstractCreature, world);
            if (self.slugcatStats.name.value == "DeathKnell")
            {


                float num = 0.7f * self.slugcatStats.bodyWeightFac;
                self.bodyChunks = new BodyChunk[7];
                self.bodyChunks[0] = new BodyChunk(self, 0, new Vector2(0f, 0f), 9f, num / 2f);
                self.bodyChunks[1] = new BodyChunk(self, 1, new Vector2(0f, 0f), 8f, num / 2f);
                for (int i = 2; i < 7; i++)
                {
                    self.bodyChunks[i] = new BodyChunk(self, i, new Vector2(0f, 0f), 5f, 0.001f);
                    self.bodyChunks[i].lastPos = self.bodyChunks[i].pos;
                    self.bodyChunks[i].pos = self.bodyChunks[0].pos;
                    self.bodyChunks[i].collideWithTerrain = true;
                    self.bodyChunks[i].collideWithObjects = true;

                }
                self.bodyChunkConnections = new BodyChunkConnection[7];
                self.bodyChunkConnections[0] = new BodyChunkConnection(self.bodyChunks[0], self.bodyChunks[1], 17f, BodyChunkConnection.Type.Normal, 1f, 0.5f);
                self.bodyChunkConnections[1] = new BodyChunkConnection(self.bodyChunks[1], self.bodyChunks[2], 4, BodyChunkConnection.Type.Pull, 0.9f, 0f);
                for (int i = 2; i < 6; i++)
                {

                    BodyChunkConnection chunkConnection = new(self.bodyChunks[i], self.bodyChunks[i + 1], 19 - 2 * i, BodyChunkConnection.Type.Pull, 0.8f, 0.3f);
                    self.bodyChunkConnections[i] = chunkConnection;
                }
                BodyChunkConnection chunkConnection6 = new(self.bodyChunks[6], self.bodyChunks[6], 13, BodyChunkConnection.Type.Pull, 0.8f, 0.3f);
                self.bodyChunkConnections[6] = chunkConnection6;


            }
        }
    }
}
