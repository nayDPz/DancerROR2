using System;
using UnityEngine;
using UnityEngine.Events;
using RoR2;
using RoR2.UI;

namespace Dancer.Modules.Components
{
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(HudElement))]
	public class DancerCrosshairController : MonoBehaviour
	{

		private bool aimIndicator = true;
		private bool rangeIndicator = true;
		private void Awake()
		{
			this.hudElement = base.GetComponent<HudElement>();
			this.inRangeBase = base.transform.Find("InRange");
			this.readyBase = base.transform.Find("Ready");
			this.centerBase = base.transform.Find("Center");


		}

		private void OnEnable()
        {
			
		}

		private string Aim(float y)
        {
			if (y > StaticValues.primaryAimUpAngle)
				return "Up";
			if(y < StaticValues.primaryAimDownAngle)
            {
				if (this.hudElement.targetCharacterBody.characterMotor.isGrounded)
					return "Down";
				else if (y < StaticValues.primaryAimDownAirAngle)
					return "Down";
            }
			return "Middle";
        }

		public bool enableUtilityCrosshairDirection = true;
		private void SetCrosshair(bool isAvailable, bool inRange, string direction)
        {
			this.isAvailable = this.hudElement.targetCharacterBody.skillLocator.utility.CanExecute();
			


			this.inRange = false;
			if (this.isAvailable)
			{
				RaycastHit raycastHit;
				this.inRange = this.hudElement.targetCharacterBody.inputBank.GetAimRaycast(this.range, out raycastHit);
			}

			if (this.hudElement.targetCharacterBody.skillLocator.utility.skillNameToken != Modules.Survivors.Dancer.lungeSkillDef.skillNameToken)
            {
				this.isAvailable = true;
				this.inRange = false;
			}

			if (this.hudElement.targetCharacterBody.skillLocator.primary.skillNameToken == Modules.Survivors.Dancer.primarySkillDef.skillNameToken)
				this.direction = this.Aim(this.hudElement.targetCharacterBody.inputBank.aimDirection.y);
			else
				this.direction = "Middle";

			if (isAvailable != this.isAvailable || (direction != this.direction && this.isAvailable))
			{
				Transform target = this.readyBase.Find("Ready" + this.direction);
				if (target)
                {
					if(this.currentReadyObject != target)
                    {
						if(this.currentReadyObject)
							this.currentReadyObject.gameObject.SetActive(false);
						target.gameObject.SetActive(this.isAvailable);
						this.currentReadyObject = target;
					}
					else if (this.currentReadyObject)
                    {
						this.currentReadyObject.gameObject.SetActive(this.isAvailable);
					}
				}				
			}

			if (inRange != this.inRange || (direction != this.direction && this.inRange))
            {
				Transform target = this.inRangeBase.Find("InRange" + this.direction);		
				if (target)
                {
					if (this.currentInRangeObject != target)
                    {
						if(this.currentInRangeObject)
							this.currentInRangeObject.gameObject.SetActive(false);
						target.gameObject.SetActive(this.inRange);
						this.currentInRangeObject = target;
					}
					else if (this.currentInRangeObject)
					{
						this.currentInRangeObject.gameObject.SetActive(this.inRange);
					}
				}				                
			}

			if(direction != this.direction)
            {
				Transform target = this.centerBase.Find("Center" + this.direction);
				if (target)
				{
					if (this.currentCenterObject != target)
					{
						if (this.currentCenterObject)
							this.currentCenterObject.gameObject.SetActive(false);
						target.gameObject.SetActive(true);
						this.currentCenterObject = target;
					}
					else if (this.currentCenterObject)
					{
						this.currentCenterObject.gameObject.SetActive(true);
					}
				}
			}
			

		}

		private void Update()
		{

			if (!this.hudElement.targetCharacterBody || !this.hudElement.targetCharacterBody.characterMotor || !this.inRangeBase || !this.readyBase)
			{
				this.SetCrosshair(false, false, "Middle");
				return;
			}
			if (!this.rangeIndicator)
				this.inRange = false;
			if (!this.aimIndicator)
				this.direction = "Up";

			this.SetCrosshair(this.isAvailable, this.inRange, this.direction);

			
		}

		public float range = StaticValues.dragonLungeRange;

		private Transform currentReadyObject;
		private Transform currentInRangeObject;
		private Transform currentCenterObject;

		private Transform centerBase;
		private Transform readyBase;
		private Transform inRangeBase;

		private HudElement hudElement;

		private bool isAvailable;
		private string direction;
		private bool inRange;
	}
}
