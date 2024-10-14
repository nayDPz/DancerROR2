using KinematicCharacterController;
using RoR2;
using UnityEngine;

namespace Dancer.SkillStates.M1
{

    public class DashAttack : BaseM1
    {
        public override void OnEnter()
        {
            anim = 1f;
            baseDuration = 0.625f;
            attackStartTime = 0.21f;
            attackEndTime = 0.6f;
            hitStopDuration = 0.025f;
            attackRecoil = 2f;
            hitHopVelocity = 2f;
            damageCoefficient = 2.5f;
            damageType = DamageType.Generic;
            hitStopDuration = 0f;
            pushForce = 1800f;
            launchVectorOverride = true;
            earlyExitJump = true;
            stackGainAmount = 12;
            swingSoundString = "SwordSwing3";
            hitSoundString = "WhipHit2";
            critHitSoundString = "SwordHit3";
            muzzleString = "eDashAttack";
            swingEffectPrefab = Modules.Assets.dashAttackEffect;
            hitEffectPrefab = Modules.Assets.stabHitEffect;
            impactSound = Modules.Assets.jab3HitSoundEvent.index;
            dashSpeedCurve = new AnimationCurve(new Keyframe(0f, 10f), new Keyframe(0.5f, 0f), new Keyframe(1f, 0f));
            isDash = true;
            isFlinch = true;
            animString = "DashAttack";
            hitboxName = "Jab";
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            characterDirection.forward = slideVector;
        }

        public override void LaunchEnemy(CharacterBody body)
        {
            Vector3 vector = characterDirection.forward * 8f + Vector3.up * 10f;
            Vector3 normalized = (vector + transform.position - body.transform.position).normalized;
            if ((bool)body.GetComponent<KinematicCharacterMotor>())
            {
                body.GetComponent<KinematicCharacterMotor>().ForceUnground();
            }
            normalized *= pushForce;
            CharacterMotor characterMotor = body.characterMotor;
            float num = 0.3f;
            if ((bool)characterMotor)
            {
                float num2 = Mathf.Max(150f, characterMotor.mass);
                num = num2 / 150f;
                normalized *= num;
                characterMotor.ApplyForce(normalized);
            }
            else if ((bool)body.rigidbody)
            {
                float num3 = Mathf.Max(50f, body.rigidbody.mass);
                num = num3 / 200f;
                normalized *= num;
                body.rigidbody.AddForce(normalized, ForceMode.Impulse);
            }
        }
    }
}
