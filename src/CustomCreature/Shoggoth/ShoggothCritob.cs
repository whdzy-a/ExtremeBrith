using DevInterface;
using Fisobs.Core;
using Fisobs.Creatures;
using Fisobs.Sandbox;
using RWCustom;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ExtremeBrith.CustomCreature.Shoggoth
{
    public class ShoggothCritob : Critob
    {
        public static Color colorr = Custom.HSL2RGB(0.13888f, 1f, 0.32f);

        public ShoggothCritob() : base(ShoggothTemplateType.Shoggoth)
        {
            Icon = new SimpleIcon("Kill_Shoggoth", colorr);
            LoadedPerformanceCost = 100f;
            SandboxPerformanceCost = new SandboxPerformanceCost(0.5f, 0.5f);
            RegisterUnlock(KillScore.Configurable(3), SandboxUnlockID.Shoggoth, MultiplayerUnlocks.SandboxUnlockID.GreenLizard, 0);
        }

        public override ArtificialIntelligence CreateRealizedAI(AbstractCreature acrit)
        {
            return new LizardAI(acrit, acrit.world);
        }

        public override Creature CreateRealizedCreature(AbstractCreature acrit)
        {
            return new Lizard(acrit, acrit.world);
        }
        public override CreatureState CreateState(AbstractCreature acrit)
        {
            return new LizardState(acrit);
        }

        public override CreatureTemplate CreateTemplate()
        {
            return LizardBreeds.BreedTemplate(Type, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.LizardTemplate), null, null, null);
        }

        public override Color DevtoolsMapColor(AbstractCreature acrit)
        {
            return colorr;
        }
        public override void LoadResources(RainWorld rainWorld)
        {
        }

        public override string DevtoolsMapName(AbstractCreature acrit)
        {
            return "Beb";
        }

        // Token: 0x0600000D RID: 13 RVA: 0x000022C4 File Offset: 0x000004C4
        public override IEnumerable<string> WorldFileAliases()
        {
            return new string[]
            {
                "shoggoth"
            };
        }
        public override IEnumerable<RoomAttractivenessPanel.Category> DevtoolsRoomAttraction()
        {
            return new RoomAttractivenessPanel.Category[]
            {
                RoomAttractivenessPanel.Category.Lizards,
                RoomAttractivenessPanel.Category.LikesInside
            };
        }


        public override void EstablishRelationships()
        {
            try
            {
                Relationships relationships;
                relationships = new Relationships(Type);
                foreach (CreatureTemplate creatureTemplate in StaticWorld.creatureTemplates)
                {
                    bool quantified = creatureTemplate.quantified;
                    if (quantified)
                    {
                        relationships.Ignores(creatureTemplate.type);
                        relationships.IgnoredBy(creatureTemplate.type);
                    }
                }
                relationships.Attacks(CreatureTemplate.Type.Slugcat, 1f);

                relationships.Ignores(CreatureTemplate.Type.GarbageWorm);
                relationships.IgnoredBy(CreatureTemplate.Type.GarbageWorm);

                relationships.Ignores(CreatureTemplate.Type.SmallCentipede);
                relationships.FearedBy(CreatureTemplate.Type.SmallCentipede, 1f);

                relationships.Ignores(CreatureTemplate.Type.Fly);
                relationships.IgnoredBy(CreatureTemplate.Type.Fly);

                relationships.Ignores(CreatureTemplate.Type.SmallNeedleWorm);
                relationships.FearedBy(CreatureTemplate.Type.SmallNeedleWorm, 0.8f);

                relationships.Ignores(CreatureTemplate.Type.LizardTemplate);
                relationships.FearedBy(CreatureTemplate.Type.RedLizard, 0.8f);
                relationships.FearedBy(CreatureTemplate.Type.YellowLizard, 0.8f);
                relationships.FearedBy(CreatureTemplate.Type.GreenLizard, 0.8f);
                relationships.FearedBy(CreatureTemplate.Type.CyanLizard, 0.8f);
                relationships.FearedBy(CreatureTemplate.Type.BlueLizard, 0.8f);
                relationships.FearedBy(CreatureTemplate.Type.PinkLizard, 0.8f);
                relationships.FearedBy(CreatureTemplate.Type.BlackLizard, 0.8f);
                relationships.FearedBy(CreatureTemplate.Type.WhiteLizard, 0.8f);
                relationships.IgnoredBy(ShoggothTemplateType.Shoggoth);

                relationships.Ignores(CreatureTemplate.Type.LanternMouse);
                relationships.FearedBy(CreatureTemplate.Type.LanternMouse, 1f);

                relationships.Ignores(CreatureTemplate.Type.Scavenger);
                relationships.FearedBy(CreatureTemplate.Type.Scavenger, 1f);

                relationships.Ignores(CreatureTemplate.Type.EggBug);
                relationships.FearedBy(CreatureTemplate.Type.EggBug, 1f);

                relationships.Ignores(CreatureTemplate.Type.DropBug);
                relationships.FearedBy(CreatureTemplate.Type.DropBug, 1f);

                relationships.Ignores(CreatureTemplate.Type.Centipede);
                relationships.FearedBy(CreatureTemplate.Type.Centipede, 1f);

                relationships.Ignores(CreatureTemplate.Type.DaddyLongLegs);
                relationships.IgnoredBy(CreatureTemplate.Type.DaddyLongLegs);

                relationships.Ignores(CreatureTemplate.Type.KingVulture);
                relationships.AttackedBy(CreatureTemplate.Type.KingVulture, 0.7f);

                relationships.Ignores(CreatureTemplate.Type.Vulture);
                relationships.FearedBy(CreatureTemplate.Type.Vulture, 1f);


            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
