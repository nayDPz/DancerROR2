using EntityStates;
using KinematicCharacterController;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Dancer.SkillStates.InterruptStates
{

    internal class SpikedState : BaseState
    {
        public float duration = 1f;

        private float stopwatch;

        private float wait = 0.2f;

        private bool hitPause;

        private float bounceForce = 5000f;

        private Vector3 storedVelocity;

        public static float damageCoefficient = 3f;

        public GameObject inflictor;

        public override void OnEnter()
        {
            base.OnEnter();
            Animator modelAnimator = GetModelAnimator();
            if ((bool)characterBody && NetworkServer.active)
            {
                characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            }
            if ((bool)modelAnimator)
            {
                int layerIndex = modelAnimator.GetLayerIndex("Body");
                modelAnimator.enabled = false;
                modelAnimator.CrossFadeInFixedTime(Random.Range(0, 2) == 0 ? "Hurt1" : "Hurt2", 0.1f);
                modelAnimator.Update(0f);
            }
            if ((bool)characterMotor)
            {
                storedVelocity = characterMotor.velocity;
                characterMotor.velocity = Vector3.zero;
            }
            else if ((bool)rigidbody)
            {
                storedVelocity = rigidbody.velocity;
                rigidbody.velocity = Vector3.zero;
            }
        }

        public override void OnExit()
        {
            if (NetworkServer.active)
            {
                characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            }
            Animator modelAnimator = GetModelAnimator();
            if ((bool)modelAnimator)
            {
                modelAnimator.enabled = true;
            }
            base.OnExit();
        }

        public void LaunchEnemy(CharacterBody body)
        {
            Vector3 up = Vector3.up;
            up *= bounceForce;
            float num = Mathf.Max(fixedAge / duration, wait);
            up *= num;
            if ((bool)body.GetComponent<KinematicCharacterMotor>())
            {
                body.GetComponent<KinematicCharacterMotor>().ForceUnground();
            }
            CharacterMotor characterMotor = body.characterMotor;
            float num2 = 0.25f;
            if ((bool)characterMotor)
            {
                characterMotor.velocity = Vector3.zero;
                float num3 = Mathf.Max(100f, characterMotor.mass);
                num2 = num3 / 100f;
                up *= num2;
                characterMotor.ApplyForce(up);
            }
            else if ((bool)body.rigidbody)
            {
                body.rigidbody.velocity = Vector3.zero;
                float num4 = Mathf.Max(50f, body.rigidbody.mass);
                num2 = num4 / 300f;
                up *= num2;
                body.rigidbody.AddForce(up, ForceMode.Impulse);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Animator modelAnimator = GetModelAnimator();
            if ((bool)modelAnimator && fixedAge < wait)
            {
                hitPause = true;
                if ((bool)characterMotor)
                {
                    characterMotor.velocity = Vector3.zero;
                }
                else if ((bool)rigidbody)
                {
                    rigidbody.velocity = Vector3.zero;
                }
                modelAnimator.enabled = false;
            }
            if (hitPause && fixedAge > wait)
            {
                hitPause = false;
                if ((bool)modelAnimator)
                {
                    modelAnimator.enabled = true;
                }
                if ((bool)characterMotor)
                {
                    characterMotor.velocity = storedVelocity;
                }
                else if ((bool)rigidbody)
                {
                    rigidbody.velocity = storedVelocity;
                }
            }
            if (isGrounded)
            {
                Util.PlaySound("Hit2", gameObject);
                if (isAuthority)
                {
                    float num = (duration - fixedAge) / duration;
                    float num2 = Mathf.Max(fixedAge / duration, wait);
                    if ((bool)inflictor)
                    {
                        DamageInfo damageInfo = new DamageInfo
                        {
                            position = transform.position,
                            attacker = inflictor,
                            inflictor = inflictor,
                            damage = damageCoefficient * damageStat * num2,
                            damageColorIndex = DamageColorIndex.Default,
                            damageType = DamageType.Stun1s,
                            crit = RollCrit(),
                            force = Vector3.zero,
                            procChainMask = default,
                            procCoefficient = 0f
                        };
                        healthComponent.TakeDamage(damageInfo);
                        GlobalEventManager.instance.OnHitEnemy(damageInfo, gameObject);
                        GlobalEventManager.instance.OnHitAll(damageInfo, gameObject);
                    }
                    LaunchEnemy(characterBody);
                    if ((bool)characterBody.GetComponent<SetStateOnHurt>() && characterBody.GetComponent<SetStateOnHurt>().canBeStunned)
                    {
                        outer.SetNextState(new StunState
                        {
                            duration = 1f
                        });
                    }
                    else
                    {
                        outer.SetNextStateToMain();
                    }
                }
                else
                {
                    outer.SetNextStateToMain();
                }
            }
            else if (fixedAge >= duration)
            {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}