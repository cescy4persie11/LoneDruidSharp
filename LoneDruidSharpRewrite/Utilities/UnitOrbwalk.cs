using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ensage.Common.Extensions;
using Ensage.Common;
using Ensage;

namespace LoneDruidSharpRewrite.Utilities
{
    public class UnitOrbwalk
    {
        static UnitOrbwalk()
        {
            Load();
        }

        private static float tick;

        public static float lastAttackStart;

        private static NetworkActivity lastActivity;

        private static float nextAttackEnd;

        private static float nextAttackRelease;

        public static void Load()
        {
            //Events.OnLoad += Events_OnLoad;
            //Events.OnClose += Events_OnClose;
            if (Game.IsInGame)
            {
                //    Events_OnLoad(null, null);
            }
        }

        public static bool AttackOnCooldown(Unit src, Entity target = null, float bonusWindupMs = 0)
        {
            if (src == null)
            {
                return false;
            }

            var turnTime = 0d;
            if (target == null) return false;
            turnTime = src.GetTurnTime(target) + (Math.Max(src.Distance2D(target) - src.GetAttackRange() - 100, 0) / src.MovementSpeed);
            return nextAttackEnd - Game.Ping - (turnTime * 1000) - 75 + bonusWindupMs >= tick;
        }

        public static bool CanCancelAnimation(float delay = 0f)
        {
            var time = tick;
            var cancelTime = nextAttackRelease - Game.Ping - delay + 50;
            return time >= cancelTime;
        }

        public static void Orbwalk(
                Unit src,
                Unit target)
        {
            if (src == null) return;
            var targetHull = 0f;
            if (target == null) return;
            targetHull = target.HullRadius;
            float distance = 0;
            var pos = Prediction.InFront(
                    src,
                    (float)((Game.Ping / 1000) + (src.GetTurnTime(target.Position) * src.MovementSpeed)));
            distance = pos.Distance2D(target) - src.Distance2D(target);
            var isValid = target != null && target.IsValid && target.IsAlive && target.IsVisible && !target.IsInvul()
                          && !target.HasModifiers(
                              new[] { "modifier_ghost_state", "modifier_item_ethereal_blade_slow" },
                              false)
                          && target.Distance2D(src)
                          <= src.GetAttackRange() + src.HullRadius + targetHull + Math.Max(distance, 0) - 200;
            if (isValid || (target != null && src.IsAttacking() && src.GetTurnTime(target.Position) < 0.1))
            {
                var canAttack = !AttackOnCooldown(src, target, 0) && !target.IsAttackImmune()
                                && !target.IsInvul() && src.CanAttack();
                if (canAttack && Utils.SleepCheck("Orbwalk.Attack"))
                {
                    src.Attack(target);
                    Utils.Sleep(
                        (UnitDatabase.GetAttackPoint(src) * 1000) + (src.GetTurnTime(target) * 1000) + Game.Ping + 100,
                        "Orbwalk.Attack");
                    Utils.Sleep(
                        (UnitDatabase.GetAttackPoint(src) * 1000) + (src.GetTurnTime(target) * 1000) + 50,
                        "Orbwalk.Move");
                    return;
                }
                if (canAttack)
                {
                    src.Attack(target);
                    return;
                }
            }
            var canCancel = (CanCancelAnimation() && AttackOnCooldown(src, target, 0))
                            || (!isValid && !src.IsAttacking() && CanCancelAnimation());
            if (!canCancel || !Utils.SleepCheck("Orbwalk.Move") || !Utils.SleepCheck("Orbwalk.Attack"))
            {
                return;
            }
            src.Move(Prediction.InFront(src, 200));
            Utils.Sleep(100, "Orbwalk.Move");
        }
    }
}
