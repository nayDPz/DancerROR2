using KinematicCharacterController;
using RoR2;
using UnityEngine;

namespace Dancer.SkillStates.DirectionalM1
{

    public class AttackForward : BaseDirectionalM1
    {
        public override void OnEnter()
        {
            anim = 1.1f;
            damageCoefficient = 1.8f;
            baseDuration = 0.55f;
            attackStartTime = 0.1f;
            attackEndTime = 0.25f;
            earlyExitTime = 0.5f;
            attackRecoil = 2f;
            hitHopVelocity = 2f;
            hitStopDuration = 0.06f;
            pushForce = 700f;
            launchVectorOverride = true;
            swingSoundString = "SwordSwing2";
            hitSoundString = "JabHit1";
            muzzleString = "eJab1";
            swingEffectPrefab = Modules.Assets.swingEffect;
            hitEffectPrefab = Modules.Assets.hitEffect;
            impactSound = Modules.Assets.jab1HitSoundEvent.index;
            dashSpeedCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.15f, 7f), new Keyframe(0.75f, 0f), new Keyframe(1f, 0f));
            isDash = true;
            animString = "Jab1";
            hitboxName = "Jab";
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void SetSlideVector()
        {
            slideVector = inputBank.aimDirection;
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
            float num = 0.25f;
            if ((bool)characterMotor)
            {
                float num2 = Mathf.Max(100f, characterMotor.mass);
                num = num2 / 100f;
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
