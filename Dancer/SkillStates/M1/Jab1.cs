using KinematicCharacterController;
using RoR2;
using UnityEngine;

namespace Dancer.SkillStates.M1
{

    public class Jab1 : BaseM1
    {
        public override void OnEnter()
        {
            anim = 1.1f;
            damageCoefficient = 1.8f;
            baseDuration = 0.55f;
            attackStartTime = 0.1f;
            attackEndTime = 0.6f;
            hitStopDuration = 0.025f;
            attackRecoil = 2f;
            hitHopVelocity = 2f;
            stackGainAmount = 6;
            hitStopDuration = 0.06f;
            pushForce = 1400f;
            launchVectorOverride = true;
            swingSoundString = "SwordSwing2";
            hitSoundString = "JabHit1";
            critHitSoundString = "JabHit2";
            muzzleString = "eJab1";
            cancelledFromSprinting = true;
            earlyExitJump = true;
            swingEffectPrefab = Modules.Assets.swingEffect;
            hitEffectPrefab = Modules.Assets.hitEffect;
            impactSound = Modules.Assets.jab1HitSoundEvent.index;
            dashSpeedCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.15f, 9f), new Keyframe(0.75f, 0f), new Keyframe(1f, 0f));
            isCombo = true;
            nextState = new Jab2();
            isDash = true;
            animString = "Jab1";
            hitboxName = "Jab";
            base.OnEnter();
        }

        public override void LaunchEnemy(CharacterBody body)
        {
            Vector3 vector = characterDirection.forward * 10f;
            Vector3 normalized = (vector + transform.position - body.transform.position).normalized;
            normalized *= pushForce;
            if ((bool)body.GetComponent<KinematicCharacterMotor>())
            {
                body.GetComponent<KinematicCharacterMotor>().ForceUnground();
            }
            CharacterMotor characterMotor = body.characterMotor;
            if ((bool)characterMotor)
            {
                float num = Mathf.Max(100f, characterMotor.mass);
                float num2 = num / 100f;
                normalized *= num2;
                characterMotor.ApplyForce(normalized);
            }
            else if ((bool)body.rigidbody)
            {
                float num3 = Mathf.Max(50f, body.rigidbody.mass);
                float num4 = num3 / 200f;
                normalized *= num4;
                body.rigidbody.AddForce(normalized, ForceMode.Impulse);
            }
        }
    }
}