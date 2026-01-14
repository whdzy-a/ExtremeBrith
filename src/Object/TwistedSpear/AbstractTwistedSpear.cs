using Fisobs.Core;
using UnityEngine;


namespace ExtremeBrith.Object.TwistedSpear
{
    public class AbstractTwistedSpear : AbstractSpear
    {
        public AbstractTwistedSpear(World world, Spear realizedObject, WorldCoordinate pos, EntityID ID) : base(world, realizedObject, pos, ID, false)
        {
        }
        public override void Realize()
        {
            if (realizedObject == null)
                realizedObject = new TwistedSpear(this, world);
        }

    }
}
