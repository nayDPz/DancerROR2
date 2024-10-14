using EntityStates;
using UnityEngine;

namespace Dancer.SkillStates.DirectionalM1
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
            EvaluateInput();
        }

        protected virtual void SetNextState()
        {
            outer.SetNextState(nextState);
        }

        protected virtual void EvaluateInput()
        {
            Vector3 moveVector = inputBank.moveVector;
            Vector3 aimDirection = inputBank.aimDirection;
            Vector3 normalized = new Vector3(aimDirection.x, 0f, aimDirection.z).normalized;
            Vector3 up = transform.up;
            Vector3 normalized2 = Vector3.Cross(up, normalized).normalized;
            inputVector = new Vector2(Vector3.Dot(moveVector, normalized), Vector3.Dot(moveVector, normalized2));
            if (!inputBank.skill1.down)
            {
                return;
            }
            if (inputBank.sprint.down && !(this is AttackSprint))
            {
                nextState = new AttackSprint();
            }
            else if (inputVector.x < -0.5f)
            {
                if (!isGrounded && canCombo)
                {
                    if (this is AttackDown)
                    {
                        nextState = new AttackDown2();
                    }
                    else
                    {
                        nextState = new AttackDown();
                    }
                }
                else if (inputBank.jump.down)
                {
                    if (!(this is AttackJump) && !(this is AttackJump2))
                    {
                        nextState = new AttackJump();
                    }
                    else if (inputBank.jump.justPressed)
                    {
                        nextState = new AttackJump2();
                    }
                }
            }
            else if (inputBank.jump.down)
            {
                if (!(this is AttackJump) && !(this is AttackJump2))
                {
                    nextState = new AttackJump();
                }
                else if (inputBank.jump.justPressed)
                {
                    nextState = new AttackJump2();
                }
            }
        }
    }
}