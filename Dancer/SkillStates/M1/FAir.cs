using Dancer.SkillStates.InterruptStates;
using EntityStates;
using KinematicCharacterController;
using RoR2;
using UnityEngine;

namespace Dancer.SkillStates.M1
{

    public class FAir : BaseM1
    {
        public bool hop;

        public override void OnEnter()
        {
            if (isAuthority)
            {
                Util.PlayAttackSpeedSound("ForwardAirStart", gameObject, attackSpeedStat);
            }
            SmallHop(characterMotor, 4.5f);
            anim = 1.1f;
            baseDuration = 0.9f;
            attackStartTime = 0.375f;
            attackEndTime = 0.7f;
            hitStopDuration = 0.15f;
            attackRecoil = 6f;
            hitHopVelocity = 12f;
            damageCoefficient = 1.8f;
            hitStopDuration = 0.08f;
            stackGainAmount = 3;
            pushForce = 6000f;
            isAerial = true;
            isSus = true;
            isFlinch = true;
            launchVectorOverride = true;
            swingSoundString = "SwordSwing3";
            hitSoundString = "SwordHit3";
            critHitSoundString = "SwordHit3";
            swingEffectPrefab = Modules.Assets.swingEffect;
            hitEffectPrefab = Modules.Assets.bigHitEffect;
            impactSound = Modules.Assets.sword1HitSoundEvent.index;
            animString = "FAir";
            hitboxName = "FAir";
            muzzleString = "eFAir";
            base.OnEnter();
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