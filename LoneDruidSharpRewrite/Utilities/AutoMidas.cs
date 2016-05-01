using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ensage;
using Ensage.Common;
using Ensage.Common.AbilityInfo;
using Ensage.Common.Extensions;
using Ensage.Common.Objects;
using Ensage.Items;

namespace LoneDruidSharpRewrite.Utilities
{
    public class AutoMidas
    {
        public Item MidasOnBear;

        public bool bearHasMidas;

        public Unit midasTarget;

        public AutoMidas()
        {
            this.FindMidasOnBear();
        }

        //
        public void FindMidasOnBear()
        {
            if(Variable.Bear == null || !Variable.Bear.IsValid || !Variable.Bear.IsAlive)
            {
                return;
            }
            var bearItems = Variable.Bear.Inventory.Items.ToList();
            this.MidasOnBear = bearItems.Where(x => x.Name == "item_hand_of_midas").DefaultIfEmpty(null).FirstOrDefault();
            this.bearHasMidas = bearItems.Any(x => x.Name == "item_hand_of_midas");
        }

        public void getMidasCreeps()
        {
            var midasTarget = ObjectManager.GetEntities<Unit>()
                        .Where(
                            x =>
                                !x.IsMagicImmune() && x.Team != Variable.Bear.Team &&
                                (x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral) && x.IsSpawned && x.IsAlive &&
                                x.Distance2D(Variable.Bear) <= this.MidasOnBear.CastRange)
                                .OrderByDescending(x => x.Health).DefaultIfEmpty(null).FirstOrDefault();

            if(midasTarget == null)
            {
                this.midasTarget = null;
                return;
            }

            this.midasTarget = midasTarget;
        }

        public void Use()
        {
            FindMidasOnBear();
            if (!this.bearHasMidas || !this.MidasOnBear.CanBeCasted() || this.midasTarget == null)
            {
                return;
            }            
            getMidasCreeps();        
            if(this.midasTarget != null)
            {
                this.MidasOnBear.UseAbility(this.midasTarget);
            }
        }
    }
}
