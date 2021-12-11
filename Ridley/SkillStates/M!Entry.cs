using System;
using EntityStates;
using UnityEngine;

namespace Ridley.SkillStates
{
	// Token: 0x02000017 RID: 23
	public class M1Entry : BaseSkillState
	{
		// Token: 0x06000052 RID: 82 RVA: 0x00005D30 File Offset: 0x00003F30
		public override void OnEnter()
		{
			base.OnEnter();
			float y = base.inputBank.aimDirection.y;
			bool flag = y > 0.5f;
			if (flag)
			{
				bool isGrounded = base.characterMotor.isGrounded;
				if (isGrounded)
				{
					this.outer.SetNextState(new UpTilt());
				}
				else
				{
					this.outer.SetNextState(new UpAir());
				}
			}
			else
			{
				bool flag2 = y < -0.7f;
				if (flag2)
				{
					bool isGrounded2 = base.characterMotor.isGrounded;
					if (isGrounded2)
					{
						this.outer.SetNextState(new DownTilt());
					}
					else
					{
						this.outer.SetNextState(new NAir());
					}
				}
				else
				{
					bool flag3 = !base.characterMotor.isGrounded;
					if (flag3)
					{
						bool flag4 = base.inputBank.moveVector != Vector3.zero;
						if (flag4)
						{
							this.outer.SetNextState(new FAir());
						}
						else
						{
							this.outer.SetNextState(new NAir());
						}
					}
					else
					{
						bool isSprinting = base.characterBody.isSprinting;
						if (isSprinting)
						{
							this.outer.SetNextState(new DashAttack());
						}
						else
						{
							this.outer.SetNextState(new Jab1());
						}
					}
				}
			}
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00005E76 File Offset: 0x00004076
		public override void FixedUpdate()
		{
			base.FixedUpdate();
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00005E80 File Offset: 0x00004080
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00005E8C File Offset: 0x0000408C
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040000D0 RID: 208
		private float charge;
	}
}
