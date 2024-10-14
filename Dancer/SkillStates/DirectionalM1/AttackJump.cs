using KinematicCharacterController;
using RoR2;
using UnityEngine;

namespace Dancer.SkillStates.DirectionalM1
{

    public class AttackJump : BaseDirectionalM1
    {
        public override void OnEnter()
        {
            hitHopVelocity = 0f;
            if (characterMotor.jumpCount <= characterBody.maxJumpCount)
            {
                characterMotor.Jump(1f, 1f);
                characterMotor.jumpCount++;
            }
            else
            {
                hitHopVelocity = characterBody.jumpPower;
            }
            anim = 1.1f;
            damageCoefficient = 2f;
            baseDuration = 0.65f;
            attackStartTime = 0.07f;
            attackEndTime = 0.2f;
            earlyExitTime = 0.67f;
            attackRecoil = 2f;
            canRecieveInput = false;
            hitStopDuration = 0.06f;
            pushForce = 2200f;
            swingSoundString = "SwordSwing2";
            hitSoundString = "JabHit1";
            muzzleString = "JumpAttack";
            launchVectorOverride = true;
            swingEffectPrefab = Modules.Assets.swingEffect;
            hitEffectPrefab = Modules.Assets.hitEffect;
            impactSound = Modules.Assets.jab1HitSoundEvent.index;
            animString = "AttackJump";
            hitboxName = "FAir";
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void LaunchEnemy(CharacterBody body)
        {
            Vector3 vector = Vector3.up * 10f;
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
