using System;
using EntityStates;
using UnityEngine;

namespace Dancer.SkillStates
{
	public class M1Entry : BaseSkillState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			float y = base.inputBank.aimDirection.y;

			if (base.characterBody.isSprinting && base.isGrounded)
			{
				this.outer.SetNextState(new DashAttack());
			}
			else if (y > 0.575f)
			{
				if (base.characterMotor.isGrounded)
				{
					this.outer.SetNextState(new UpAir()); //this.outer.SetNextState(new Jab1());
				}
				else
				{
					this.outer.SetNextState(new UpAir());
				}
			}
			else
			{
				if (y < -0.425f)
				{
					if (base.characterMotor.isGrounded)
					{
						this.outer.SetNextState(new DownAirLand());
					}
					else if (y < -0.74f)
					{
						this.outer.SetNextState(new DownAir());
					}
					else
                    {
						this.outer.SetNextState(new FAir());
					}
				}
				else
				{
					if (!base.characterMotor.isGrounded)
					{
						this.outer.SetNextState(new FAir());
					}
					else
					{					
						this.outer.SetNextState(new Jab1());
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
