using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtremeBrith.Object.GlodenMask
{
    public class AbstractGlodenMask : AbstractPhysicalObject
    {
        public AbstractGlodenMask(World world, PhysicalObject realizedObject, WorldCoordinate pos, EntityID ID) : base(world,GlodenMaskFisob.GlodenMaskType, realizedObject, pos, ID)
        {
        }
        public override void Realize()
        {
            if (realizedObject == null)
                realizedObject = new GlodenMask(this, world);
        }

    }
}
