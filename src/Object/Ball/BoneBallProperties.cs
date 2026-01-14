using Fisobs.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtremeBrith.Object.Ball
{
    public class BoneBallProperties : ItemProperties
    {
        public override void Throwable(Player player, ref bool throwable)
       => throwable = true;

        public override void ScavCollectScore(Scavenger scavenger, ref int score)
            => score = 3;

        public override void ScavWeaponPickupScore(Scavenger scav, ref int score)
            => score = 3;

        // Don't throw shields
        public override void ScavWeaponUseScore(Scavenger scav, ref int score)
            => score = 3;

        public override void Grabability(Player player, ref Player.ObjectGrabability grabability)
        {
            // The player can only grab one centishield at a time,
            // but that shouldn't prevent them from grabbing a spear,
            // so don't use Player.ObjectGrabability.BigOneHand

            if (player.grasps.Any(g => g?.grabbed is BoneBall))
            {
                grabability = Player.ObjectGrabability.CantGrab;
            }
            else
            {
                grabability = Player.ObjectGrabability.OneHand;
            }
        }
    }
}
