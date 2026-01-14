using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtremeBrith.CustomCreature.Shoggoth
{
    public static class ShoggothTemplateType
    {
        public static void UnregisterValues()
        {
            bool flag = Shoggoth != null;
            if (flag)
            {
                Shoggoth.Unregister();
                Shoggoth = null;
            }
        }

        public static CreatureTemplate.Type Shoggoth = new CreatureTemplate.Type("Shoggoth", true);
    }
}
