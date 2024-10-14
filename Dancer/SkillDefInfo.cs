using EntityStates;
using UnityEngine;

internal class SkillDefInfo
{
    public string skillName;

    public string skillNameToken;

    public string skillDescriptionToken;

    public Sprite skillIcon;

    public SerializableEntityStateType activationState;

    public string activationStateMachineName;

    public int baseMaxStock;

    public float baseRechargeInterval;

    public bool beginSkillCooldownOnSkillEnd;

    public bool canceledFromSprinting;

    public bool forceSprintDuringState;

    public bool fullRestockOnAssign;

    public InterruptPriority interruptPriority;

    public bool resetCooldownTimerOnUse;

    public bool isCombatSkill;

    public bool mustKeyPress;

    public bool cancelSprintingOnActivation;

    public int rechargeStock;

    public int requiredStock;

    public int stockToConsume;

    public string[] keywordTokens;
}
