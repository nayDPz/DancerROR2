using Dancer.Modules;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Dancer.SkillStates
{
    public class ChargeParry : BaseSkillState
    {
        private float duration;

        private Animator animator;

        private ChildLocator childLocator;

        private protected GameObject chargeEffectInstance;

        public GameObject chargeEffectPrefab;

        public string chargeSoundString;

        public float baseDuration = 1f;

        public GameObject crosshairOverridePrefab;

        public float minChargeDuration = 1f;

        private GameObject defaultCrosshairPrefab;

        private float timer;

        private float perfectTime = 0.25f;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            timer = duration;
            animator = GetModelAnimator();
            childLocator = GetModelChildLocator();
            PlayChargeAnimation();
            Util.PlayAttackSpeedSound("FireballCharge", base.gameObject, attackSpeedStat);
            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(Buffs.parryBuff);
            }
            if ((bool)crosshairOverridePrefab)
            {
            }
            StartAimMode(duration + 2f);
        }

        public override void OnExit()
        {
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(Buffs.parryBuff);
            }
            if ((bool)base.characterBody)
            {
            }
            base.OnExit();
        }

        protected float CalcCharge()
        {
            if (base.fixedAge <= perfectTime)
            {
                return 1f;
            }
            return Mathf.Clamp01(timer / duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            timer -= Time.fixedDeltaTime;
            float charge = CalcCharge();
            if (Util.HasEffectiveAuthority(base.gameObject) && ((!IsKeyDownAuthority() && base.fixedAge >= minChargeDuration) || !base.characterBody.HasBuff(Buffs.parryBuff)))
            {
                Riposte riposte = new Riposte();
                riposte.charge = charge;
                outer.SetNextState(riposte);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        protected virtual void PlayChargeAnimation()
        {
            PlayAnimation("Head, Override", "NSpecStart", "Slash.playbackRate", 0.225f);
        }
    }
}