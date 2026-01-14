using RWCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ExtremeBrith
{
    public class PlayerRipple : CosmeticSprite
    {

        public BodyChunk chunk;

        public float rad;

        public float lastRad;

        public float radVel;


        public float initRad;

        public float lifeTime;

        public float lastLife;

        public float life;

        public float intensity;


        public Color col;

        public Color blackCol;

        public PlayerRipple(BodyChunk chunk, Vector2 vel, float intensity, Color col)
        {
            this.chunk = chunk;
            pos = chunk.pos;
            lastPos = pos;
            base.vel = vel;
            this.intensity = intensity;
            this.col = col;
            radVel = Mathf.Lerp(1.4f, 4.2f, intensity);
            initRad = Mathf.Lerp(8f, 12f, intensity);
            rad = initRad;
            lastRad = initRad;
            life = 1f;
            lastLife = 0f;
            lifeTime = Mathf.Lerp(6f, 30f, Mathf.Pow(intensity, 4f));
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            lastRad = rad;
            rad += radVel;
            radVel *= 0.92f;
            radVel -= Mathf.InverseLerp(0.6f + 0.3f * intensity, 0f, life) * Mathf.Lerp(0.2f, 0.6f, intensity);
            lastLife = life;
            life = Mathf.Max(0f, life - 1f / lifeTime);
            if (lastLife <= 0f && life <= 0f)
            {
                Destroy();
            }
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[1];
            sLeaser.sprites[0] = new FSprite("Futile_White");
            sLeaser.sprites[0].shader = rCam.game.rainWorld.Shaders["VectorCircle"];
            AddToContainer(sLeaser, rCam, rCam.ReturnFContainer("Foreground"));
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            sLeaser.sprites[0].x = Mathf.Lerp(lastPos.x, pos.x, timeStacker) - camPos.x;
            sLeaser.sprites[0].y = Mathf.Lerp(lastPos.y, pos.y, timeStacker) - camPos.y;
            float num = Mathf.Lerp(lastLife, life, timeStacker);
            float num2 = Mathf.InverseLerp(0f, 0.75f, num);
            sLeaser.sprites[0].color = Color.Lerp(num2 > 0.5f ? GetRippleColor() : blackCol, Color.Lerp(blackCol, col, 0.5f + 0.5f * intensity), Mathf.Sin(num2 * (float)Math.PI));
            float num3 = Mathf.Lerp(lastRad, rad, timeStacker);
            sLeaser.sprites[0].scale = num3 / 8f;
            sLeaser.sprites[0].alpha = Mathf.Sin(Mathf.Pow(num, 2f) * (float)Math.PI) * 2f / num3;
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
        }
        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            base.ApplyPalette(sLeaser, rCam, palette);
            blackCol = palette.blackColor;
        }
        public Color GetRippleColor()
        {
            return Color.white;
        }
    }
}
