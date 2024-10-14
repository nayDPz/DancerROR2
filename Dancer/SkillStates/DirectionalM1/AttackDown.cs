using Dancer.SkillStates.InterruptStates;
using EntityStates;
using KinematicCharacterController;
using RoR2;
using UnityEngine;

namespace Dancer.SkillStates.DirectionalM1
{

    public class AttackDown : BaseDirectionalM1
    {
        public override void OnEnter()
        {
            anim = 1.1f;
            baseDuration = 0.67f;
            attackStartTime = 0.11f;
            attackEndTime = 0.6f;
            earlyExitTime = 0.7f;
            hitStopDuration = 0.05f;
            attackRecoil = 6f;
            hitHopVelocity = 3f;
            damageCoefficient = 2f;
            pushForce = 2250f;
            isFlinch = true;
            swingSoundString = "SwordSwing3";
            hitSoundString = "SwordHit3";
            swingEffectPrefab = Modules.Assets.swingEffect;
            hitEffectPrefab = Modules.Assets.bigHitEffect;
            impactSound = Modules.Assets.sword1HitSoundEvent.index;
            dashSpeedCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 4f), new Keyframe(0.75f, 0f), new Keyframe(1f, 0f));
            isDash = true;
            launchVectorOverride = true;
            animString = "AttackDown";
            hitboxName = "FAir";
            muzzleString = "eFAir";
            canCombo = false;
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!canCombo && !(inputVector.x < -0.5f))
            {
                canCombo = true;
            }
        }

        public override void SetSlideVector()
        {
            slideVector = Vector3.down;
        }

        public override void LaunchEnemy(CharacterBody body)
        {
            Vector3 vector = characterDirection.forward * 10f + Vector3.down * 15f;
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
                characterMotor.velocity = Vector3.zero;
                float num2 = Mathf.Max(150f, characterMotor.mass);
                num = num2 / 150f;
                normalized *= num;
                characterMotor.ApplyForce(normalized);
                if ((bool)body)
                {
                    EntityStateMachine component = body.GetComponent<EntityStateMachine>();
                    if ((bool)body.GetComponent<SetStateOnHurt>() && (bool)component)
                    {
                        SpikedState newNextState = new SpikedState
                        {
                            inflictor = gameObject
                        };
                        component.SetInterruptState(newNextState, InterruptPriority.Death);
                    }
                }
            }
            else if ((bool)body.rigidbody)
            {
                body.rigidbody.velocity = Vector3.zero;
                float num3 = Mathf.Max(50f, body.rigidbody.mass);
                num = num3 / 200f;
                normalized *= num;
                body.rigidbody.AddForce(normalized, ForceMode.Impulse);
            }
        }
    }
}
