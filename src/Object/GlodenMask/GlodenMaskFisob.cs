using ExtremeBrith.Object.GlodenMask;
using Fisobs.Core;
using Fisobs.Items;
using Fisobs.Properties;
using Fisobs.Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ExtremeBrith.Object.GlodenMask
{
    public class GlodenMaskFisob:Fisob
    {
        public static readonly AbstractPhysicalObject.AbstractObjectType GlodenMaskType = new("GlodenMask", true);
        public GlodenMaskFisob() : base(GlodenMaskType)
        {
            Icon = Icon = new SimpleIcon("icon_GlodenMask", Color.Lerp(Color.red, Color.blue, 0.5f));


            SandboxPerformanceCost = new(linear: 0.35f, exponential: 0f);

            RegisterUnlock(SandboxUnlockID.GlodenMask, parent: MultiplayerUnlocks.SandboxUnlockID.Spear, data: 0);
        }
        public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock unlock)
        {
            // Centi shield data is just floats separated by ; characters.
            Plugin.Log("sadsadasd");
            var result = new AbstractGlodenMask(world, null, saveData.Pos, saveData.ID);

            return result;
        }

        private static readonly GlodenMaskProperties properties = new();

        public override ItemProperties Properties(PhysicalObject forObject)
        {

            return properties;
        }

    }
}
