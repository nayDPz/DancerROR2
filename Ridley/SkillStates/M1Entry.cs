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
			if (y > 0.5f)
			{
				if (base.characterMotor.isGrounded)
				{
					this.outer.SetNextState(new Jab1());
				}
				else
				{
					this.outer.SetNextState(new UpAir());
				}
			}
			else
			{
				if (y < -0.575f)
				{
					if (base.characterMotor.isGrounded)
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
					if (!base.characterMotor.isGrounded)
					{
						if (base.inputBank.moveVector != Vector3.zero)
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
						if (base.characterBody.isSprinting)
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

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}
	}
}
