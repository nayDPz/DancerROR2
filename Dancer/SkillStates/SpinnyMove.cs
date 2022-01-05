using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;
using EntityStates;
using UnityEngine.Networking;
using System.Linq;
namespace Dancer.SkillStates
{
    public class SpinnyMove : BaseSkillState
    {
        private Animator animator;

        private BaseState.HitStopCachedState hitStopCachedState;
        private Vector3 storedVelocity;
        protected float hitStopDuration = 0.09f;
        private bool inHitPause;
        private float hitPauseTimer;

        private float duration;
        private float windupDuration = 0.08f;
        private float baseAttackInterval = 0.3f;
        private float attackInterval;
        private int numAttacks = 3;
        private int attacksFired;
        private float stopwatch;

        private GameObject swingEffectPrefab;


        private bool groundStart;
        private AnimationCurve dashSpeedCurve = new AnimationCurve(new Keyframe[]
            {
                new Keyframe(0f, 5f),
                new Keyframe(0.35f, .5f),
                new Keyframe(1f, .5f)
            });
        private Vector3 moveDirection;

        private float procCoefficient = 1f;
        private float damageCoefficient = 2.5f;
        private float pushForce = 2325f;
        private float attackRadius = 6f;
        private bool crit;

        public override void OnEnter()
        {
            this.animator = base.GetModelAnimator();
            base.StartAimMode(2f, true);


            foreach(EntityStateMachine e in base.gameObject.GetComponents<EntityStateMachine>())
                if (e.customName == "Weapon")
                    e.SetNextStateToMain();

            this.groundStart = base.isGrounded;

            this.windupDuration /= this.attackSpeedStat;
            this.attackInterval = this.baseAttackInterval / this.attackSpeedStat;
            this.duration = this.attackInterval * this.numAttacks + this.windupDuration;
            
            this.stopwatch = this.attackInterval;
            this.crit = base.RollCrit();
            base.PlayAnimation("FullBody, Override", "Secondary", "Spinny.playbackRate", this.duration * 1.4f);
            this.moveDirection = base.inputBank.aimDirection;

            base.OnEnter();


        }

        private void Fire()
        {
            Vector3 attackPosition = this.moveDirection * this.attackRadius + base.transform.position;
            this.PlaySwingEffect();

            if (Util.HasEffectiveAuthority(base.gameObject))
            {
                
                

                



                if (new BlastAttack
                {
                    attacker = this.gameObject,
                    procChainMask = default(ProcChainMask),
                    impactEffect = EffectIndex.Invalid,
                    losType = BlastAttack.LoSType.NearestHit,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = DamageType.Generic,
                    procCoefficient = this.procCoefficient,
                    bonusForce = Vector3.zero,
                    baseForce = 0f,
                    baseDamage = this.damageCoefficient * this.damageStat,
                    falloffModel = BlastAttack.FalloffModel.None,
                    radius = this.attackRadius,
                    position = attackPosition,
                    attackerFiltering = AttackerFiltering.Default,
                    teamIndex = base.GetTeam(),
                    inflictor = base.gameObject,
                    crit = this.crit,
                }.Fire().hitCount > 0)
                {
                    this.OnHitEnemyAuthority();
                    Util.PlaySound("SwordHit2", base.gameObject);
                }
            }

            this.attacksFired++;

            if (NetworkServer.active)
            {
                List<HealthComponent> hits = new List<HealthComponent>();
                Collider[] hit = Physics.OverlapSphere(attackPosition, this.attackRadius, LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal);
                for (int i = 0; i < hit.Length; i++)
                {
                    HurtBox hurtBox = hit[i].GetComponent<HurtBox>();
                    if (hurtBox)
                    {
                        HealthComponent healthComponent = hurtBox.healthComponent;
                        if (healthComponent)
                        {
                            TeamComponent team = healthComponent.GetComponent<TeamComponent>();
                            bool enemy = team.teamIndex != base.teamComponent.teamIndex;
                            if (enemy)
                            {
                                if (!hits.Contains(healthComponent))
                                {
                                    hits.Add(healthComponent);
                                    HealthComponent h = healthComponent;
                                    if (h)
                                    {
                                        if (!h.body.isChampion || (h.gameObject.name.Contains("Brother") && h.gameObject.name.Contains("Body")))
                                        {
                                            this.LaunchEnemy(h.body);
                                        }
                                    }
                                }
                            }
                        }
                    }

                }

            }

        }

        private void PlaySwingEffect()
        {
            string s = "Jab1";
            switch (attacksFired)
            {
                case 0:
                    EffectManager.SimpleMuzzleFlash(Modules.Assets.downTiltEffect, base.gameObject, "eJab1", false);
                    s = "PunchSwing";
                    break;
                case 1:
                    EffectManager.SimpleMuzzleFlash(Modules.Assets.downTiltEffect, base.gameObject, "eSecondary2", false);
                    s = "SwordSwing2";
                    break;
                case 2:
                    EffectManager.SimpleMuzzleFlash(Modules.Assets.downTiltEffect, base.gameObject, "eFAir", false);
                    s = "SwordSwing3";
                    break;
                default:
                    EffectManager.SimpleMuzzleFlash(Modules.Assets.downTiltEffect, base.gameObject, "eJab1", false);
                    break;
            }
            Util.PlayAttackSpeedSound(s, base.gameObject, base.attackSpeedStat);
        }

        private void OnHitEnemyAuthority()
        {
            if (!this.inHitPause && this.hitStopDuration > 0f)
            {
                this.storedVelocity = base.characterMotor.velocity;
                this.hitStopCachedState = base.CreateHitStopCachedState(base.characterMotor, this.animator, "Spinny.playbackRate");
                this.hitPauseTimer = this.hitStopDuration / this.attackSpeedStat;
                this.inHitPause = true;
            }
        }
        private void LaunchEnemy(CharacterBody body)
        {
            Vector3 direction = this.moveDirection * 50f;
            Vector3 launchVector = (direction + base.transform.position) - body.transform.position;
            launchVector = launchVector.normalized;
            launchVector *= this.pushForce;

            if (body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>())
            {
                body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>().ForceUnground();
            }

            CharacterMotor m = body.characterMotor;

            float force = 0.25f;
            if (m)
            {
                m.velocity = Vector3.zero;
                float f = Mathf.Max(100f, m.mass);
                force = f / 100f;
                launchVector *= force;
                m.ApplyForce(launchVector);
            }
            else if (body.rigidbody)
            {
                body.rigidbody.velocity = Vector3.zero;
                float f = Mathf.Max(50f, body.rigidbody.mass);
                force = f / 200f;
                launchVector *= force;
                body.rigidbody.AddForce(launchVector, ForceMode.Impulse);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
           
            if (base.fixedAge > this.windupDuration)
            {

                this.hitPauseTimer -= Time.fixedDeltaTime;
                if (this.hitPauseTimer <= 0f && this.inHitPause)
                {
                    base.ConsumeHitStopCachedState(this.hitStopCachedState, base.characterMotor, this.animator);
                    this.inHitPause = false;
                    base.characterMotor.velocity = this.storedVelocity;
                }
                if (!this.inHitPause)
                {
                    this.stopwatch += Time.fixedDeltaTime;
                }
                else
                {
                    if (base.characterMotor)
                    {
                        base.characterMotor.velocity = Vector3.zero;
                    }
                    if (this.animator)
                    {
                        this.animator.SetFloat("Spinny.playbackRate", 0f);
                    }
                }

                if (base.characterMotor && this.attacksFired < this.numAttacks && !this.inHitPause)
                {
                    float num = this.dashSpeedCurve.Evaluate(this.stopwatch / this.duration);
                    if (base.isGrounded && !this.groundStart)
                    {
                        this.moveDirection.y = 0;
                        this.moveDirection = this.moveDirection.normalized;
                    }
                    base.characterDirection.forward = this.moveDirection;
                    base.characterMotor.velocity = Vector3.zero;
                    base.characterMotor.rootMotion += num * 7f * this.moveDirection * Time.fixedDeltaTime;
                }
                if (this.stopwatch >= this.attackInterval && this.attacksFired < this.numAttacks)
                {
                    this.Fire();
                    //this.moveDirection = base.inputBank.aimDirection; //maybe maybe not
                    this.stopwatch = 0f;
                }
                if (this.attacksFired >= this.numAttacks)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
           

            
        }

        public override void OnExit()
        {
            base.OnExit();
        }

    }
}
