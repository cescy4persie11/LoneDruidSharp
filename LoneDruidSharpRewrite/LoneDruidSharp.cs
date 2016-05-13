﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Objects;
using global::LoneDruidSharpRewrite.Abilities;
using global::LoneDruidSharpRewrite.Utilities;
using Ensage.Common.Menu;
using Ensage.Common.Objects.UtilityObjects;

namespace LoneDruidSharpRewrite
{
    public class LoneDruidSharp
    {

        private AutoIronTalon autoIronTalon;

        private AutoMidas autoMidas;

        private SummonSpiritBear summonSpiritBear;

        private static Dictionary<float, Orbwalker> orbwalkerDictionary = new Dictionary<float, Orbwalker>();

        private Lasthit lasthit;

        private Rabid rabid;

        private Move move;

        private bool pause;

        private TargetFind targetFind;

        private DrawText drawText;

        public LoneDruidSharp()
        {
            this.autoIronTalon = new AutoIronTalon();
            this.autoMidas = new AutoMidas();
            this.lasthit = new Lasthit();
            this.drawText = new DrawText();
        }

        private static Hero Me
        {
            get
            {
                return Variable.Hero;
            }
        }

        private static Unit Bear
        {
            get
            {
                return Variable.Bear;
            }
        }

        private Hero Target
        {
            get
            {
                return this.targetFind.Target;
            }
        }

        private bool inBearLastHitMode
        {
            get
            {
                return Variable.MenuManager.OnlyBearLastHitModeOn;
            }
        }

        private bool inCombinedLastHitMode
        {
            get
            {
                return Variable.MenuManager.CombineLastHitModeOn;
            }
        }

        public void OnClose()
        {
            this.pause = true;
        }

        public void OnDraw()
        {
            if(Variable.Hero == null || !Variable.Hero.IsValid || !Variable.Hero.IsAlive)
            {
                return;
            }
            //draw target
            
            //draw default menu
            drawText.DrawTextOnlyBearLastHitText(Variable.OnlyBearLastHitActive);
            //drawText.DrawTextCombinedLastHitText(Variable.CombinedLastHitActive);
            drawText.DrawTextAutoIronTalonText(Variable.AutoTalonActive);
            drawText.DrawTextAutoMidasText(Variable.AutoMidasModeOn);
            drawText.DrawTextBearChaseText(Variable.BearChaseModeOn);
            if (Variable.BearChaseModeOn)
            {
                this.targetFind.DrawTarget();
            }
            


        }       

        public void OnLoad()
        {
            Variable.Hero = ObjectManager.LocalHero;
            this.pause = Variable.Hero.ClassID != ClassID.CDOTA_Unit_Hero_LoneDruid;
            if (this.pause)
            {
                return;
            }
            
            Variable.EnemyTeam = Me.GetEnemyTeam();
            Variable.MenuManager = new MenuManager(Me.Name);
            Variable.SummonSpiritBear = new SummonSpiritBear(Me.Spellbook.Spell1);
            Variable.Rabid = new Rabid(Me.Spellbook.Spell2);
            Variable.MenuManager.Menu.AddToMainMenu();

            this.summonSpiritBear = new SummonSpiritBear(Me.Spellbook.Spell1);
            this.rabid = new Rabid(Me.Spellbook.Spell1);

   
            this.targetFind = new TargetFind();
            this.move = new Move(Me);
            this.autoIronTalon = new AutoIronTalon();
            //this.lasthit = new Lasthit();


            Game.PrintMessage(
                "LoneDruidSharp" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + " loaded",
                MessageType.LogMessage);
        }

        #region auto iron talon
        public void OnUpdate_IronTalon()
        {
            if (!this.pause)
            {
                this.pause = Game.IsPaused;
            }
            Variable.Bear = ObjectManager.GetEntities<Unit>().Where(unit => unit.ClassID.Equals(ClassID.CDOTA_Unit_SpiritBear)).FirstOrDefault();

            if (this.pause || Variable.Hero == null || !Variable.Hero.IsValid || !Variable.Hero.IsAlive)
            {
                this.pause = Game.IsPaused;
                return;
            }
            var CanIronTalon = Variable.MenuManager.AutoTalonActive && !Variable.BearChaseModeOn;

            if (CanIronTalon)
            {
                autoIronTalon.Execute();
            }        
        }
        #endregion

        #region automidas, defaulted
        public void OnUpdate_AutoMidas()
        {
            if (!this.pause)
            {
                this.pause = Game.IsPaused;
            }
            Variable.Bear = ObjectManager.GetEntities<Unit>().Where(unit => unit.ClassID.Equals(ClassID.CDOTA_Unit_SpiritBear)).FirstOrDefault();
            if (this.pause || Variable.Bear == null || !Variable.Bear.IsValid || !Variable.Bear.IsAlive)
            {
                this.pause = Game.IsPaused;
                return;
            }
            if (!Variable.AutoMidasModeOn) return;
            var CanMidas = !Variable.BearChaseModeOn;

            if (CanMidas)
            {
                autoMidas.Execute();
            }
        }
        #endregion

        #region lasthit
        public void OnUpdate_LastHit()
        {
            if (!this.pause)
            {
                this.pause = Game.IsPaused;
            }

            if (this.pause || Variable.Bear == null || !Variable.Bear.IsValid || !Variable.Bear.IsAlive)
            {
                this.pause = Game.IsPaused;
                return;
            }

            var onlyBearLastHitModeOn = Variable.OnlyBearLastHitActive && !Variable.CombinedLastHitActive;
            //if (!onlyBearLastHitModeOn) return;
            var combinedLastHitModeOn = Variable.CombinedLastHitActive && !Variable.OnlyBearLastHitActive;
            if (onlyBearLastHitModeOn)
            {
                if (Utils.SleepCheck("onlybearlasthit"))
                {
                    lasthit.OnlyBearLastHitExecute();
                    Utils.Sleep(100, "onlybearlasthit");

                };               
            }

            if (combinedLastHitModeOn)
            {
                if (Utils.SleepCheck("combinedlasthit"))
                {
                    lasthit.CombinedLastHitExecute();
                    Utils.Sleep(100, "combinedlasthit");
                }
            }





        }
        #endregion

        #region bear chase
        public void OnUpdate_bearChase()
        {
            if (!this.pause)
            {
                this.pause = Game.IsPaused;
            }

            if (this.pause || Variable.Bear == null || !Variable.Bear.IsValid || !Variable.Bear.IsAlive)
            {
                this.pause = Game.IsPaused;
                return;
            }
            //re openup Auto Midas and Auto IronTalon if no enemy is nearby me and Bear
            /*
            var anyEnemyNearMe = ObjectManager.GetEntities<Hero>().Any(x => x.IsAlive && x.Team != Variable.Hero.Team
                                                        && x.Distance2D(Me) <= 1000);
            var anyEnemyNearBear = ObjectManager.GetEntities<Hero>().Any(x => x.IsAlive && x.Team != Variable.Hero.Team
                                                        && x.Distance2D(Bear) <= 1000);

            if(!anyEnemyNearBear && !anyEnemyNearMe)
            {
                if (!Variable.AutoTalonActive)
                {
                    //Variable.MenuManager.AutoTalonMenu.SetValue(new KeyBind(Variable.MenuManager.AutoTalonMenu.GetValue<KeyBind>().Key, KeyBindType.Toggle, true));
                }
                if (!Variable.AutoMidasModeOn)
                {
                    //Variable.MenuManager.AutoMidasMenu.SetValue(new KeyBind(Variable.MenuManager.AutoMidasMenu.GetValue<KeyBind>().Key, KeyBindType.Toggle, true));
                }
            }
            if (Bear == null) return;
            if (!Variable.BearChaseModeOn) return;
            this.targetFind.Find();
            if (this.Target == null) return;                              
            */
            //UnitOrbwalk.Orbwalk(Bear, this.Target);
        }
        #endregion

        public void OnWndProc(WndEventArgs args)
        {
            if (this.pause || Variable.Hero == null || !Variable.Hero.IsValid || !Variable.Hero.IsAlive)
            {
                return;
            }
            
            if (this.Target == null || !this.Target.IsValid)
            {
                return;
            }
            
        }

        public void Events_OnUpdate()
        {
            if (!Variable.BearChaseModeOn) return;
            if (Bear == null) return;
            this.targetFind.Find();
            if (this.Target == null || !this.Target.IsValid) return;
            Orbwalker orbwalker = new Orbwalker(Bear);
            orbwalkerDictionary.Add(Bear.Handle, orbwalker);
            orbwalker.OrbwalkOn(this.Target, 0, 0, false, true);
        }

        public void Player_OnExecuteOrder(ExecuteOrderEventArgs args)
        {
            if (this.pause || Variable.Hero == null || !Variable.Hero.IsValid || !Variable.Hero.IsAlive)
            {
                return;
            }
            lasthit.resetAutoAttackMode();
            // re lock target
            if (Variable.BearChaseModeOn)
            {
                //should disable Auto Iron Talon and Midas until Noenemy is nearby
                if (Variable.AutoTalonActive)
                {
                    Variable.MenuManager.AutoTalonMenu.SetValue(new KeyBind(Variable.MenuManager.AutoTalonMenu.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
                }
                if (Variable.AutoMidasModeOn)
                {
                    Variable.MenuManager.AutoMidasMenu.SetValue(new KeyBind(Variable.MenuManager.AutoMidasMenu.GetValue<KeyBind>().Key, KeyBindType.Toggle, false));
                }
                this.targetFind.UnlockTarget();
                this.targetFind.Find();
                this.targetFind.LockTarget();
            }
        }

    }
}
