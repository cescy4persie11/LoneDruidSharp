using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ensage;
using Ensage.Common;
using SharpDX;

namespace LoneDruidSharpRewrite.Abilities
{
    public class Rabid
    {
        private readonly Ability ability;

        private readonly DotaTexture abilityIcon;

        private bool attacked;

        private Vector2 iconSize;

        public Rabid(Ability ability)
        {
            this.ability = ability;
            this.abilityIcon = Drawing.GetTexture("materials/ensage_ui/spellicons/lone_druid_rabid");
            this.iconSize = new Vector2(HUDInfo.GetHpBarSizeY() * 2);
        }
    }
}
