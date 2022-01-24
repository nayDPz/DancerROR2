using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;
using EntityStates;
namespace Dancer.SkillStates
{
    public class BaseInputEvaluation : BaseSkillState
    {
		protected Vector2 inputVector;
		private bool inAir;
		protected EntityState nextState;
		protected bool canCombo = true;

		public override void FixedUpdate()
        {
            base.FixedUpdate();

			this.EvaluateInput();
        }

		protected virtual void SetNextState()
		{
			this.outer.SetNextState(this.nextState);
		}

		protected virtual void EvaluateInput()
		{
			Vector3 moveVector = base.inputBank.moveVector;
			Vector3 aimDirection = base.inputBank.aimDirection;
			Vector3 normalized = new Vector3(aimDirection.x, 0f, aimDirection.z).normalized;
			Vector3 up = base.transform.up;
			Vector3 normalized2 = Vector3.Cross(up, normalized).normalized;
			this.inputVector = new Vector2(Vector3.Dot(moveVector, normalized), Vector3.Dot(moveVector, normalized2));

			if (base.inputBank.skill1.down)
			{
				if (base.inputBank.sprint.down && !(this is AttackSprint))
				{
					this.nextState = (new AttackSprint());
				}
				else if (this.inputVector.x < -0.5f)
				{
					if(!base.isGrounded && this.canCombo)
                    {
						if (this is AttackDown)
							this.nextState = new AttackDown2();
						else
							this.nextState = new AttackDown();
					}
					else if (base.inputBank.jump.down)
					{
						if (!(this is AttackJump) && !(this is AttackJump2))
						{
							this.nextState = new AttackJump();
						}
						else if (base.inputBank.jump.justPressed)
							this.nextState = new AttackJump2();
					}
				}
				else if (base.inputBank.jump.down)
				{
					if (!(this is AttackJump) && !(this is AttackJump2))
                    {
						this.nextState = new AttackJump();
					}						
					else if (base.inputBank.jump.justPressed)
						this.nextState = new AttackJump2();
				}
				
				
			}

		}
	}
}
