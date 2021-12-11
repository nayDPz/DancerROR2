using System;
using EntityStates;
using UnityEngine;
using RoR2;
using EntityStates;
namespace Ridley.SkillStates.BaseStates
{
	// Token: 0x02000019 RID: 25
	internal class SkeweredState : BaseState
	{
		// Token: 0x06000064 RID: 100 RVA: 0x0000657C File Offset: 0x0000477C
		public override void OnEnter()
		{
			base.OnEnter();
			Animator modelAnimator = base.GetModelAnimator();
			int layerIndex = modelAnimator.GetLayerIndex("Body");
			modelAnimator.CrossFadeInFixedTime((UnityEngine.Random.Range(0, 2) == 0) ? "Hurt1" : "Hurt2", 0.1f);
			modelAnimator.Update(0f);
			bool flag = base.rigidbody && !base.rigidbody.isKinematic;
			if (flag)
			{
				base.rigidbody.velocity = Vector3.zero;
				bool flag2 = base.rigidbodyMotor;
				if (flag2)
				{
					base.rigidbodyMotor.moveVector = Vector3.zero;
				}
			}
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00006628 File Offset: 0x00004828
		public override void OnExit()
		{
			Animator modelAnimator = base.GetModelAnimator();
			bool flag = modelAnimator;
			if (flag)
			{
				modelAnimator.enabled = true;
			}
			base.OnExit();
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00006658 File Offset: 0x00004858
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			Animator modelAnimator = base.GetModelAnimator();
			bool flag = modelAnimator && base.fixedAge > this.wait;
			if (flag)
			{
				modelAnimator.enabled = false;
			}
			bool flag2 = base.characterMotor;
			if (flag2)
			{
				base.characterMotor.velocity = Vector3.zero;
			}
			bool flag3 = base.fixedAge >= this.duration;
			bool flag4 = flag3;
			if (flag4)
			{
				if (base.GetComponent<SetStateOnHurt>().canBeStunned)
					this.outer.SetNextState(new StunState { stunDuration = 1f });
				else
					this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06000067 RID: 103 RVA: 0x000066E4 File Offset: 0x000048E4
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Frozen;
		}

		// Token: 0x040000F2 RID: 242
		public float duration = 2f;

		// Token: 0x040000F3 RID: 243
		private float wait = 0.075f;
	}
}
