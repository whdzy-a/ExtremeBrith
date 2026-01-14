using RWCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;
using Vector2 = UnityEngine.Vector2;

namespace ExtremeBrith.Slugcat.Recursion
{
    public class RecursionGraphics
    {
        public static void Hook()
        {
            On.PlayerGraphics.Update += PlayerGraphics_Update;
            On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;
            On.PlayerGraphics.InitiateSprites += PlayerGraphics_InitiateSprites;
        }

        private static void PlayerGraphics_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            orig.Invoke(self, sLeaser, rCam);
            if ((self.owner as Player).slugcatStats.name.value == "Recursion")
            {
                Array.Resize(ref sLeaser.sprites,)
            }
        }

        private static void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig.Invoke(self,sLeaser,rCam,timeStacker,camPos);
            if ((self.owner as Player).slugcatStats.name.value == "Recursion")
            {
                if (self.RenderAsPup)
                {
                    sLeaser.sprites[0].scaleY = 0.5f;
                }
                else
                {
                    sLeaser.sprites[0].scaleY = 1f;
                }
            }


        }

        private static void PlayerGraphics_Update(On.PlayerGraphics.orig_Update orig, PlayerGraphics self)
        {
            orig.Invoke(self);
            if (self.player != null && self.player.slugcatStats.name.value == "Recursion")
            {
                if (self.player.playerState.isPup)
                {
                    self.tail[0].connectionRad = 2;
                    self.tail[1].connectionRad = 3.5f;
                    self.tail[2].connectionRad = 3.5f;
                    self.tail[3].connectionRad = 3.5f;
                }
                else
                {

                    self.tail[0].connectionRad = 4;
                    self.tail[1].connectionRad = 7;
                    self.tail[2].connectionRad = 7;
                    self.tail[3].connectionRad = 7;
                }

/*                self.drawPositions[0, 0] = self.owner.bodyChunks[0].pos;
                self.drawPositions[1, 0] = self.owner.bodyChunks[1].pos;
                Vector2 pupvector = Vector2.Lerp(self.drawPositions[0, 0], self.drawPositions[1, 0], 0.35f + (0.25f - 0.5f * 0.25f));
                self.drawPositions[0, 0] = Vector2.LerpUnclamped(pupvector - (self.owner.bodyChunks[0].pos - pupvector), self.owner.bodyChunks[0].pos, Recmodule.GrowLevel);


                if (self.player.bodyMode == Player.BodyModeIndex.Stand)
                {
                    self.drawPositions[0, 0].x += (float)self.player.flipDirection * Mathf.LerpUnclamped(-2f, 6f,Recmodule.GrowLevel) * Mathf.Clamp(Mathf.Abs(self.owner.bodyChunks[1].vel.x) - 0.2f, 0f, 1f);
                    self.drawPositions[0, 0].y += Mathf.Cos(((float)self.player.animationFrame + 0f) / 6f * 2f * (float)Math.PI) * Mathf.LerpUnclamped(1f, 2f,Recmodule.GrowLevel);
                    self.drawPositions[1, 0].x -= (float)self.player.flipDirection * (1.5f - (float)self.player.animationFrame / 6f) * Mathf.LerpUnclamped(-0.5f, 1f, Recmodule.GrowLevel);
                    self.drawPositions[1, 0].y += 2f + Mathf.Sin(((float)self.player.animationFrame + 0f) / 6f * 2f * (float)Math.PI) * Mathf.LerpUnclamped(0f, 4f, Recmodule.GrowLevel);
                }

                Vector2 vector5 = Custom.DirVec(self.drawPositions[1, 0], self.drawPositions[0, 0]) * 3f;
                if (self.player.bodyMode == Player.BodyModeIndex.Crawl)
                {
                    vector5.x *= Mathf.LerpUnclamped(5f,0,Recmodule.GrowLevel);
                }
                else if (self.player.bodyMode == Player.BodyModeIndex.CorridorClimb && vector5.y < 0f)
                {
                    vector5.y *= 2f;
                }

                self.head.ConnectToPoint(Vector2.Lerp(self.drawPositions[0, 0], self.drawPositions[1, 0], 0.2f) + vector5, (self.player.animation == Player.AnimationIndex.HangFromBeam) ? 0f : 3f, push: false, 0.2f, self.owner.bodyChunks[0].vel, 0.7f, 0.1f);
*/            }
        }
    }
}
