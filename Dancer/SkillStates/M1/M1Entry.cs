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

			
			if (y > Modules.StaticValues.primaryAimUpAngle)
			{
				this.outer.SetNextState(new UpAir());
			}
			else
			{
				if (y < Modules.StaticValues.primaryAimDownAngle)
				{
					if (base.characterMotor.isGrounded)
					{
						this.outer.SetNextState(new DownAirLand());
					}
					else if (y < Modules.StaticValues.primaryAimDownAirAngle)
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
						if (base.characterBody.isSprinting)
						{
							this.outer.SetNextState(new DashAttack());
						}
						else
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
