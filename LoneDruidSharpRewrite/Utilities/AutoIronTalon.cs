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
    public class AutoIronTalon
    {
        public Item meIronTalon;

        public Item bearIronTalon;

        public bool hasIronTalon;



        public AutoIronTalon()
        {
            this.FindItems();
            //this.getIronTalonUnit();
            //ObjectManager.OnAddEntity += this.ObjectManager_OnAddEntity;
            //ObjectManager.OnRemoveEntity += this.ObjectManager_OnRemoveEntity;
        }

        public bool Pause { get; set; }

        public int Owner{ get; set; }

        public void FindItems()
        {
            if(Variable.Hero == null || !Variable.Hero.IsValid)
            {
                return;
            }
            List<Item> bearItems;
            var myHeroItems = Variable.Hero.Inventory.Items.ToList();
            
            if(Variable.Bear == null || !Variable.Bear.IsAlive || !Variable.Bear.IsValid)
            {
                bearItems = new List<Item>();
            }
            else
            {
                bearItems = Variable.Bear.Inventory.Items.ToList();
            }
            //0 none, 1 hero, 2 bear, 3 all
            if (myHeroItems.Any(x => x.Name == "item_iron_talon") && !bearItems.Any(x => x.Name == "item_iron_talon"))
            {
                this.Owner = 1;
            }
            else if (bearItems.Any(x => x.Name == "item_iron_talon") && !myHeroItems.Any(x => x.Name == "item_iron_talon"))
            {
                this.Owner = 2;
            }
            else if (bearItems.Any(x => x.Name == "item_iron_talon") && myHeroItems.Any(x => x.Name == "item_iron_talon"))
            {
                this.Owner = 3;
            }
            else
            {
                this.Owner = 0;
            }
            this.meIronTalon = (this.Owner == 1 || this.Owner == 3) ? myHeroItems.Where(x => x.Name == "item_iron_talon").FirstOrDefault() : null;
            this.bearIronTalon = (this.Owner == 2 || this.Owner == 3) ? bearItems.Where(x => x.Name == "item_iron_talon").FirstOrDefault() : null;
            this.hasIronTalon = myHeroItems.Any(x => x.Name == "item_iron_talon") || bearItems.Any(x => x.Name == "item_iron_talon");       
        }

        private void ObjectManager_OnAddEntity(EntityEventArgs args)
        {
            if (this.Pause || Variable.Hero == null || !Variable.Hero.IsValid || !this.hasIronTalon)
            {
                return;
            }
        }

        private void ObjectManager_OnRemoveEntity(EntityEventArgs args)
        {
            if (this.Pause || Variable.Hero == null || !Variable.Hero.IsValid || !this.hasIronTalon)
            {
                return;
            }
        }

        public Unit myIronTalonTarget { get; set; }

        public Unit bearIronTalonTarget { get; set; }

        public void getMyIronTalonUnit()
        {
            if (!this.meIronTalon.CanBeCasted() || this.Owner == 0) return;
            var meTarget = ObjectManager.GetEntities<Unit>().Where(x =>
                                        !x.IsMagicImmune() && x.Team != Variable.Hero.Team &&
                                        (x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane ||
                                         x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege ||
                                         x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral) && x.IsSpawned && x.IsAlive
                                         && x.Distance2D(Variable.Hero) <= this.meIronTalon.CastRange)
                                        .OrderByDescending(x => 0.4 * x.Health / x.MaximumHealth).FirstOrDefault();  

            if (meTarget == null || this.meIronTalon == null)
            {
                this.myIronTalonTarget = null;
                return;              
            }
            else
            {
                this.myIronTalonTarget = meTarget;
            }
        }

        public void getBearIronTalonUnit()
        {
            if (!this.bearIronTalon.CanBeCasted() || this.Owner == 0) return;
            var bearTarget = ObjectManager.GetEntities<Unit>().Where(x =>
                                        !x.IsMagicImmune() && x.Team != Variable.Hero.Team &&
                                        (x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane ||
                                         x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege ||
                                         x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral) && x.IsSpawned && x.IsAlive
                                         && x.Distance2D(Variable.Bear) <= this.bearIronTalon.CastRange)
                                        .OrderByDescending(x => 0.4 * x.Health / x.MaximumHealth).DefaultIfEmpty(null).FirstOrDefault();
            if (bearTarget == null || this.bearIronTalon == null)
            {
                this.bearIronTalonTarget = null;
                return;
            }
            else
            {
                this.bearIronTalonTarget = bearTarget;
            }
        }


        public void Use()
        {
            FindItems();
            getMyIronTalonUnit();
            if (this.meIronTalon.CanBeCasted() && this.myIronTalonTarget != null)
            {
                this.meIronTalon.UseAbility(this.myIronTalonTarget);
            }
            getBearIronTalonUnit();
            if (this.bearIronTalon.CanBeCasted() && this.bearIronTalonTarget != null)
            {
                this.bearIronTalon.UseAbility(this.bearIronTalonTarget);
            }                
        }




    }
}
