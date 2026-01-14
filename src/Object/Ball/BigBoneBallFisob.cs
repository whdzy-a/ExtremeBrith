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

namespace ExtremeBrith.Object.Ball
{
    public class BigBoneBallFisob:Fisob
    {

        public BigBoneBallFisob() : base(AbstractBall.BallType.BigBoneBall)
        {
            Icon = Icon = new SimpleIcon("icon_BigBoneBall", Color.white);


            SandboxPerformanceCost = new(linear: 0.35f, exponential: 0f);

            RegisterUnlock(SandboxUnlockID.BigBoneBall, parent: MultiplayerUnlocks.SandboxUnlockID.Spear, data: 0);
        }
        public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock unlock)
        {
            // Centi shield data is just floats separated by ; characters.
            var result = new AbstractBall(world, AbstractBall.BallType.BigBoneBall, null, saveData.Pos, saveData.ID);

            return result;
        }

        private static readonly BoneBallProperties properties = new();

        public override ItemProperties Properties(PhysicalObject forObject)
        {

            return properties;
        }
    }
}
