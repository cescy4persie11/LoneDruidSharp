using System;
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

namespace LoneDruidSharpRewrite
{
    public class LoneDruidSharp
    {
        private readonly Sleeper lasthitSleeper;

        private readonly Sleeper autoIronTalonSleeper;

        private readonly Sleeper onlyBearLastHitSleeper;

        private readonly Sleeper combinedLastHitSleeper;

        private readonly Sleeper bearChaseSleeper;

        private readonly Sleeper autoMidasSleeper;

        private AutoIronTalon autoIronTalon;

        private AutoMidas autoMidas;

        private SummonSpiritBear summonSpiritBear;

        private Lasthit lasthit;

        private Rabid rabid;

        private Move move;

        private bool pause;

        private TargetFind targetFind;

        private DrawText drawText;

        public LoneDruidSharp()
        {
            this.lasthitSleeper = new Sleeper();
            this.autoIronTalonSleeper = new Sleeper();
            this.onlyBearLastHitSleeper = new Sleeper();
            this.combinedLastHitSleeper = new Sleeper();
            this.bearChaseSleeper = new Sleeper();
            this.autoMidasSleeper = new Sleeper();
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
            drawText.DrawTextCombinedLastHitText(Variable.CombinedLastHitActive);
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
            Variable.Bear = ObjectManager.GetEntities<Unit>().Where(unit => unit.ClassID.Equals(ClassID.CDOTA_Unit_SpiritBear)).FirstOrDefault();
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

            if(this.pause || Variable.Hero == null || !Variable.Hero.IsValid || !Variable.Hero.IsAlive)
            {
                this.pause = Game.IsPaused;
                return;
            }

            var meCanAction = !Me.IsInvisible() && !Me.IsChanneling();
            var meIronTalonMode = Variable.MenuManager.AutoTalonActive && !Variable.BearChaseModeOn;

            if (meIronTalonMode && meCanAction)
            {
                if (this.autoIronTalonSleeper.Sleeping)
                {
                    return;
                }
                autoIronTalon.Use();
                this.autoIronTalonSleeper.Sleep(Game.Ping + 100);
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

            if (this.pause || Variable.Bear == null || !Variable.Bear.IsValid || !Variable.Bear.IsAlive)
            {
                this.pause = Game.IsPaused;
                return;
            }

            var bearCanAction = !Bear.IsInvisible() && !Bear.IsChanneling();

            if (bearCanAction)
            {
                if (this.autoMidasSleeper.Sleeping)
                {
                    return;
                }
                autoMidas.Use();
                this.autoMidasSleeper.Sleep(Game.Ping + 1000);
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

            this.targetFind.Find();

            if(this.Target == null)
            {
                return;
            }
            //Bear Keep Chasing
            //Console.WriteLine("target is " + this.Target.Name);

            if(Bear.CanAttack() && !Bear.IsAttacking() && Bear.Distance2D(this.Target) <= 150)
            {
                if (Utils.SleepCheck("Attack"))
                {
                    Bear.Attack(this.Target);
                    Utils.Sleep(500, "Attack");
                }
                else
                {
                    if (Utils.SleepCheck("Move"))
                    {
                        Bear.Move(this.Target.Position);
                        Utils.Sleep(500, "Move");
                    }
                }
            }





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

        public void Player_OnExecuteOrder(ExecuteOrderEventArgs args)
        {
            if (this.pause || Variable.Hero == null || !Variable.Hero.IsValid || !Variable.Hero.IsAlive)
            {
                return;
            }
            lasthit.resetAutoAttackMode();

            if (Variable.BearChaseModeOn)
            {
                if (this.Target.Distance2D(Game.MousePosition) < 200)
                {
                    this.targetFind.LockTarget();
                    Console.WriteLine("Locked");
                }
                else
                {
                    this.targetFind.UnlockTarget();
                    this.targetFind.Find();
                    if (this.Target.Distance2D(Game.MousePosition) < 200)
                    {
                        this.targetFind.LockTarget();
                    }
                }
            }

        }


    }
}
