using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ExtremeBrith.Object.TwistedSpear
{
    public class TwistedSpear : Spear
    {
        public CreatureFearer CreatureFearer { get; set; }

        public TwistedSpear(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
        {
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            if (CreatureFearer != null && stuckInObject == null)
            {
                CreatureFearer.Destroy();
            }
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            if (stuckIns != null)
            {
                rCam.ReturnFContainer("HUD").AddChild(stuckIns.label);
            }

            sLeaser.sprites = new FSprite[1];
            sLeaser.sprites[0] = new FSprite("TwistedSpear");
            AddToContainer(sLeaser, rCam, null);

        }

        public override bool HitSomething(SharedPhysics.CollisionResult result, bool eu)
        {
            if (result.obj != null && result.obj is Creature)
            {
                CreatureFearer = new CreatureFearer(this, result.obj as Creature, 500);
                room.AddObject(CreatureFearer);
            }
            return base.HitSomething(result, eu);
        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            color = Color.Lerp(Color.Lerp(Color.red, Color.blue, 0.5f), palette.blackColor, 0.9f);
        }
    }
}
