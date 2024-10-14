using RoR2;
using UnityEngine;

namespace Dancer.SkillStates.DirectionalM1
{

    public class AttackBackwards : BaseDirectionalM1
    {
        public override void OnEnter()
        {
            anim = 1f;
            baseDuration = 0.7f;
            attackStartTime = 0.18f;
            attackEndTime = 0.32f;
            earlyExitTime = 0.8f;
            hitStopDuration = 0.05f;
            attackRecoil = 2f;
            hitHopVelocity = 2f;
            damageCoefficient = 2f;
            damageType = DamageType.Generic;
            pushForce = 800f;
            swingSoundString = "SwordSwing3";
            hitSoundString = "WhipHit2";
            muzzleString = "eDashAttack";
            swingEffectPrefab = Modules.Assets.dashAttackEffect;
            hitEffectPrefab = Modules.Assets.stabHitEffect;
            impactSound = Modules.Assets.jab3HitSoundEvent.index;
            dashSpeedCurve = new AnimationCurve(new Keyframe(0f, 7f), new Keyframe(0.5f, 0f), new Keyframe(1f, 0f));
            isDash = true;
            isFlinch = true;
            animString = "AttackBack";
            hitboxName = "Jab";
            canRecieveInput = false;
            base.OnEnter();
        }

        public override void SetSlideVector()
        {
            if ((slideVector = inputBank.moveVector) == Vector3.zero)
            {
                Vector3 aimDirection = inputBank.aimDirection;
                slideVector = -aimDirection;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!canRecieveInput && !(inputVector.x < -0.5f))
            {
                canRecieveInput = true;
            }
        }
    }
}
