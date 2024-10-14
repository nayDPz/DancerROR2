using EntityStates;

namespace Dancer.SkillStates.DirectionalM1
{

    public class EnterDirectionalAttack : BaseInputEvaluation
    {
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                EvaluateInput();
                if (inputVector.y < -0.5f)
                {
                    nextState = new AttackLeft();
                }
                else if (inputVector.y > 0.5f)
                {
                    nextState = new AttackRight();
                }
                else if (inputVector.x < -0.5f)
                {
                    if (!isGrounded)
                    {
                        nextState = new AttackDown();
                    }
                    else
                    {
                        nextState = new AttackBackwards();
                    }
                }
                else if (inputVector.x > 0.5f)
                {
                    nextState = new AttackForward();
                }
                SetNextState();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}