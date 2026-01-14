using Fisobs.Core;
using Fisobs.Items;
using Fisobs.Properties;
using Fisobs.Sandbox;
using UnityEngine;
using Color = UnityEngine.Color;

namespace ExtremeBrith.Object.TwistedSpear
{
    public class TwistedSpearFisob : Fisob
    {
        public static readonly AbstractPhysicalObject.AbstractObjectType TwistedSpearType = new("TwistedSpear", true);

        public TwistedSpearFisob() : base(TwistedSpearType)
        {
            Icon = Icon = new SimpleIcon("icon_TwistedSpear", Color.Lerp(Color.red, Color.blue, 0.5f));


            SandboxPerformanceCost = new(linear: 0.35f, exponential: 0f);

            RegisterUnlock(SandboxUnlockID.TwistedSpear, parent: MultiplayerUnlocks.SandboxUnlockID.Spear, data: 0);
        }
        public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock unlock)
        {
            // Centi shield data is just floats separated by ; characters.
            var result = new AbstractTwistedSpear(world, null, saveData.Pos, saveData.ID);

            return result;
        }

        private static readonly TwistedSpearProperties properties = new();

        public override ItemProperties Properties(PhysicalObject forObject)
        {

            return properties;
        }
    }
}
