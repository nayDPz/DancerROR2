using RoR2.UI;
using UnityEngine;

namespace Dancer.Modules.Components
{

    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(HudElement))]
    public class DancerCrosshairController : MonoBehaviour
    {
        private bool aimIndicator = true;

        private bool rangeIndicator = true;

        public bool enableUtilityCrosshairDirection = true;

        public float range = 70f;

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

        private void Awake()
        {
            hudElement = GetComponent<HudElement>();
            inRangeBase = base.transform.Find("InRange");
            readyBase = base.transform.Find("Ready");
            centerBase = base.transform.Find("Center");
        }

        private void OnEnable()
        {
        }

        private string Aim(float y)
        {
            if (y > 0.575f)
            {
                return "Up";
            }
            if (y < -0.425f)
            {
                if (hudElement.targetCharacterBody.characterMotor.isGrounded)
                {
                    return "Down";
                }
                if (y < -0.74f)
                {
                    return "Down";
                }
            }
            return "Middle";
        }

        private void SetCrosshair(bool isAvailable, bool inRange, string direction)
        {
            this.isAvailable = hudElement.targetCharacterBody.skillLocator.utility.CanExecute();
            this.inRange = false;
            if (this.isAvailable)
            {
                this.inRange = hudElement.targetCharacterBody.inputBank.GetAimRaycast(range, out var _);
            }
            if (hudElement.targetCharacterBody.skillLocator.utility.skillNameToken != Dancer.Modules.Survivors.Dancer.lungeSkillDef.skillNameToken)
            {
                this.isAvailable = true;
                this.inRange = false;
            }
            if (hudElement.targetCharacterBody.skillLocator.primary.skillNameToken == Dancer.Modules.Survivors.Dancer.primarySkillDef.skillNameToken)
            {
                this.direction = Aim(hudElement.targetCharacterBody.inputBank.aimDirection.y);
            }
            else
            {
                this.direction = "Middle";
            }
            if (isAvailable != this.isAvailable || (direction != this.direction && this.isAvailable))
            {
                Transform transform = readyBase.Find("Ready" + this.direction);
                if ((bool)transform)
                {
                    if (currentReadyObject != transform)
                    {
                        if ((bool)currentReadyObject)
                        {
                            currentReadyObject.gameObject.SetActive(value: false);
                        }
                        transform.gameObject.SetActive(this.isAvailable);
                        currentReadyObject = transform;
                    }
                    else if ((bool)currentReadyObject)
                    {
                        currentReadyObject.gameObject.SetActive(this.isAvailable);
                    }
                }
            }
            if (inRange != this.inRange || (direction != this.direction && this.inRange))
            {
                Transform transform2 = inRangeBase.Find("InRange" + this.direction);
                if ((bool)transform2)
                {
                    if (currentInRangeObject != transform2)
                    {
                        if ((bool)currentInRangeObject)
                        {
                            currentInRangeObject.gameObject.SetActive(value: false);
                        }
                        transform2.gameObject.SetActive(this.inRange);
                        currentInRangeObject = transform2;
                    }
                    else if ((bool)currentInRangeObject)
                    {
                        currentInRangeObject.gameObject.SetActive(this.inRange);
                    }
                }
            }
            if (!(direction != this.direction))
            {
                return;
            }
            Transform transform3 = centerBase.Find("Center" + this.direction);
            if (!transform3)
            {
                return;
            }
            if (currentCenterObject != transform3)
            {
                if ((bool)currentCenterObject)
                {
                    currentCenterObject.gameObject.SetActive(value: false);
                }
                transform3.gameObject.SetActive(value: true);
                currentCenterObject = transform3;
            }
            else if ((bool)currentCenterObject)
            {
                currentCenterObject.gameObject.SetActive(value: true);
            }
        }

        private void Update()
        {
            if (!hudElement.targetCharacterBody || !hudElement.targetCharacterBody.characterMotor || !inRangeBase || !readyBase)
            {
                SetCrosshair(isAvailable: false, inRange: false, "Middle");
                return;
            }
            if (!rangeIndicator)
            {
                inRange = false;
            }
            if (!aimIndicator)
            {
                direction = "Up";
            }
            SetCrosshair(isAvailable, inRange, direction);
        }
    }
}
