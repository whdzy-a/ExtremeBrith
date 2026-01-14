using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtremeBrith.Object.Ball
{
    public class MiddleBoneBall : BoneBall
    {
        public MiddleBoneBall(AbstractBall abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
        {
            Origimass = 1f;
            Radius = 10f;
            Carrymass = 0.3f;
            Grabbedlevel = 2;
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[5];
            for (int i = 0; i < 4; i++)
            {
                sLeaser.sprites[i] = new FSprite("ConnectLine");
            }

            sLeaser.sprites[4] = new FSprite("MiddleBoneBall");
            AddToContainer(sLeaser, rCam, null);
        }
    }
}
