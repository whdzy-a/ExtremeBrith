using System.Globalization;
using UnityEngine;

namespace ExtremeBrith.Object.Ball
{
    public class AbstractBall : AbstractPhysicalObject
    {
        public class BallType : ExtEnum<AbstractObjectType>
        {
            public static AbstractObjectType SmallBoneBall = new AbstractObjectType("SmollBoneBall", register: true);
            public static AbstractObjectType MiddleBoneBall = new AbstractObjectType("MiddleBoneBall", register: true);
            public static AbstractObjectType BigBoneBall = new AbstractObjectType("BigBoneBall", register: true);
        }

        public float saturation;
        public float hue;
        public float scaleX;
        public float scaleY;
        public Color color = Color.white;

        public AbstractBall(World world, AbstractObjectType type, Player Creater, WorldCoordinate pos, EntityID ID) : base(world, type, null, pos, ID)
        {
            scaleX = 1;
            scaleY = 1;
            saturation = 0.5f;
            hue = 1f;
            base.type = type;
            if (Creater != null)
            {
                color = PlayerGraphics.JollyColor(Creater.playerState.playerNumber, 2);
            }
        }
        public override void Realize()
        {
            base.Realize();
            if (realizedObject == null)
            {
                if (type == BallType.SmallBoneBall)
                {
                    realizedObject = new SmallBoneBall(this, Room.world);
                }
                if (type == BallType.MiddleBoneBall)
                {
                    realizedObject = new MiddleBoneBall(this, Room.world);
                }
                if (type == BallType.BigBoneBall)
                {
                    realizedObject = new BoneBall(this, Room.world);
                }


            }
        }
        public override void Destroy()
        {
            LoseAllStuckObjects();
            base.Destroy();
        }

        public override string ToString()
        {
            string baseString = string.Format(CultureInfo.InvariantCulture, "{0}<oA>{1}<oA>{2}", IDAndRippleLayerString, type.ToString(), pos.SaveToString());
            baseString = SaveState.SetCustomData(this, baseString);
            return SaveUtils.AppendUnrecognizedStringAttrs(baseString, "<oA>", unrecognizedAttributes);
        }

    }
}
