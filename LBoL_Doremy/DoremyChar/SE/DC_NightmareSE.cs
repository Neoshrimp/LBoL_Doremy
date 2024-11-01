using HarmonyLib;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.StatusEffects.Enemy;
using LBoL.Presentation.UI.Widgets;
using LBoL.Presentation.Units;
using LBoL_Doremy.ExtraAssets;
using LBoL_Doremy.RootTemplates;
using LBoL_Doremy.Utils;
using LBoLEntitySideloader.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LBoL_Doremy.DoremyChar.SE
{
    public sealed class DC_NightmareSEDef : DStatusEffectDef
    {
        public override StatusEffectConfig PreConfig()
        {
            var con = DefaultConfig();
            con.Type = LBoL.Base.StatusEffectType.Special;

            con.HasCount = true;
            con.CountStackType = LBoL.Base.StackType.Keep;

            return con;
        }
    }

    [EntityLogic(typeof(DC_NightmareSEDef))]
    public sealed class DC_NightmareSE : DStatusEffect
    {

        protected override void OnAdding(Unit unit)
        {
            base.OnAdding(unit);
        }
        protected override void OnAdded(Unit unit)
        {
            NightmareSource = Battle.Player;
            ReactOwnerEvent(unit.DamageReceived, DamageReceived, (GameEventPriority)(-99));
            ReactOwnerEvent(unit.HealingReceived, HealingReceived, (GameEventPriority)(-99));
            // ui bar
            ReactOwnerEvent(unit.BlockShieldGained, BlockShieldGained, (GameEventPriority)(-99));
            ReactOwnerEvent(unit.BlockShieldLost, BlockShieldLost, (GameEventPriority)(-99));


            UpdateOrCreateNightmareBar();

            React(DoKill());
        }

/*        private IEnumerable<BattleAction> OnSelfAdded(StatusEffectApplyEventArgs args)
        {
            if (args.Effect != this)
                yield break;
            var source = args.ActionSource;
            if (source.TrickleDownActionSource() is Card)
                NightmareSource = Battle.Player;
            else if (source is UltimateSkill || source is Exhibit)
                NightmareSource = Battle.Player;
            else if (source is Unit u)
                NightmareSource = u;
        }*/

        public readonly static string nightmareBarGoName = "NightmareBar";
        //public readonly static string actualHpGoName = "ActualHp";


        public void UpdateOrCreateNightmareBar()
        {
            if (Owner?.View is UnitView view)
            {
                var sw = view._statusWidget;
                var hpBar = sw.hpBar;
                var hpBarGo = hpBar.gameObject;

                var nBarGo = hpBarGo.transform.Find(nightmareBarGoName)?.gameObject;
                if (nBarGo == null)
                {
                    nBarGo = GameObject.Instantiate(hpBarGo.transform.Find("HealthBarHealth").gameObject, hpBarGo.transform, worldPositionStays: true);
                    nBarGo.name = nightmareBarGoName;
                    nBarGo.GetComponent<Image>().sprite = AssetManager.DoremyAssets.purpleBar;

                }

                var nBarImage = nBarGo.GetComponent<Image>();
                var targetHpFill = PrecalculateHpBarFill(hpBar, Owner.Hp, Owner.MaxHp, Owner.Shield, Owner.Block);
                nBarImage.fillAmount = (Math.Clamp(Level - 1, 0, Owner.Hp) / (float)Owner.Hp) * targetHpFill;

            }
        }


        static float PrecalculateHpBarFill(HealthBar healthBar, int hp, int maxHp, int shield, int block)
        {
            int curHp = healthBar._hp;
            float num = (float)hp / (float)maxHp;
            float num2 = 0.3f;
            if (1f - num > 0.3f)
            {
                num2 = 1f - num;
            }
            if (1f - num > 0.6f)
            {
                num2 = 0.6f;
            }
            float hpBarFill = num;
            float num4 = 0f;
            float num5 = 0f;
            if (shield + block != 0)
            {
                float num6 = num2 * (float)(shield + block) / ((float)(shield + block) + 20f);
                float num7 = num6 * (float)shield / (float)(shield + block);
                float num8 = num6 * (float)block / (float)(shield + block);
                if (num6 > 1f - num)
                {
                    hpBarFill = 1f - num6;
                }
                num4 = hpBarFill + num7;
                num5 = num4 + num8;
            }
            return hpBarFill;
        }

        public override void NotifyChanged()
        {
            base.NotifyChanged();
            UpdateOrCreateNightmareBar();
        }

        private Unit _nightmareSource;
        public Unit NightmareSource 
        { 
            get 
            { 
                if (_nightmareSource == null) 
                {
                    Log.LogWarning($"{this.Name} doesn't have NightmareSource set. Defaulting to PlayerUnit.");
                    return Battle.Player;
                } 
                return _nightmareSource;
            }
            private set => _nightmareSource = value;
        }

        

        public override bool Stack(StatusEffect other)
        {
            var rez = base.Stack(other);
            React(DoKill());

            return rez;
        }

        public bool CheckKill()
        {
            Count = Math.Clamp(Owner.Hp + 1 - Level, 0, Owner.Hp);
            return Level > Owner.Hp;
        }



        IEnumerable<BattleAction> DoKill()
        {
            if (CheckKill())
            {
                NotifyActivating();
                yield return new ForceKillAction(NightmareSource, Owner);
            }
        }

        private IEnumerable<BattleAction> HealingReceived(HealEventArgs args)
        {
            return DoKill();
        }

        private IEnumerable<BattleAction> DamageReceived(DamageEventArgs args)
        {
            return DoKill();
        }

        private IEnumerable<BattleAction> BlockShieldLost(BlockShieldEventArgs args)
        {
            NotifyChanged();
            yield break;
        }

        private IEnumerable<BattleAction> BlockShieldGained(BlockShieldEventArgs args)
        {
            NotifyChanged();
            yield break;
        }

        [HarmonyPatch]
        class MaxHp_Patch
        {

            [HarmonyPatch(typeof(Unit), nameof(Unit.SetMaxHp))]
            [HarmonyPostfix]
            static void EnemyPostfix(Unit __instance)
            {
                CheckNightmare(__instance);
            }


            private static void CheckNightmare(Unit unit)
            {
                if (unit.Battle != null && unit.TryGetStatusEffect<DC_NightmareSE>(out var nightmareSE))
                {
                    nightmareSE.UpdateOrCreateNightmareBar();
                    foreach (var a in nightmareSE.DoKill())
                        // technically is not correct and will react outside of action stack where SetMaxHp was invoked.
                        // but currently there's no action for setting max hp so w/e
                        // ALSO action source will be null
                        nightmareSE.Battle.RequestDebugAction(a, "Nightmare maxHp check");
                }
            }
        }



        [HarmonyPatch]
        class UncapCountAndLevel_Patch
        {

            static IEnumerable<MethodBase> TargetMethods()
            {
                yield return AccessTools.PropertySetter(typeof(StatusEffect), nameof(StatusEffect.Count));
                yield return AccessTools.PropertySetter(typeof(StatusEffect), nameof(StatusEffect.Level));
                yield return AccessTools.Method(typeof(StatusEffect), nameof(StatusEffect.Stack));
                yield return AccessTools.Method(typeof(StatusEffect), nameof(StatusEffect.ClampMax));

            }



            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var matcher = new CodeMatcher(instructions);
                matcher.Start();
                while (matcher.IsValid)
                { 
                    matcher.MatchEndForward(new CodeInstruction(OpCodes.Ldc_I4, 999));
                    if(matcher.IsValid)
                        matcher.SetInstruction(new CodeInstruction(OpCodes.Ldc_I4, int.MaxValue));
                }

                return matcher.InstructionEnumeration();
            }


        }


        //[HarmonyPatch(typeof(DeathExplode), nameof(DeathExplode.OnDying), MethodType.Enumerator)]
        class DeathExplode_DoubleDmgFix_Patch
        {

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
            {
                return new CodeMatcher(instructions)
                    .MatchEndForward(new CodeInstruction(OpCodes.Ldstr, "2"))
                    .MatchEndBackwards(new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(DieEventArgs), nameof(DieEventArgs.Source))))

                    .Advance(1)
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DeathExplode_DoubleDmgFix_Patch), nameof(DeathExplode_DoubleDmgFix_Patch.CheckSourceType))))

                    .MatchEndForward(OpCodes.Beq)
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Pop))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_0))

                    .InstructionEnumeration();
            }

            private static int CheckSourceType(Unit source)
            {
                Log.LogDebug($"deez {source}");
                return source is EnemyUnit ? 1 : 0;
            }
        }



    }

}
