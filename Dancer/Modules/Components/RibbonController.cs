using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using Dancer.SkillStates;
using EntityStates;
using System.Collections.Generic;
using System.Linq;
using R2API.Networking;
using R2API.Networking.Interfaces;
namespace Dancer.Modules.Components
{
	public class RibbonController : NetworkBehaviour
	{

		public float timer;
		private EntityStateMachine ownerMachine;
		private float damageCoefficient = StaticValues.ribbonDamageCoefficient;

		private float fixedAttachOwnerStopwatch;
		private SkillLocator ownerSkillLocator;

		private RibbonController nextController;
		private float attachOwnerStopwatch;
		private float attachOwnerTime = 0.25f;
		private bool ownerAttached = true;
		private bool ribbonExtended;

		private void Start()
        {
			this.ribbonLineRenderer = base.transform.Find("RibbonLine").gameObject.GetComponent<LineRenderer>();
			this.ribbonLineRenderer.positionCount = 2;
			this.ribbonLineRenderer.useWorldSpace = true;
			this.ribbonLineRenderer.enabled = false;

			RibbonController.instancesList.Add(this);

		}
		private void OnDisable()
        {
			Destroy(base.gameObject);
        }

		private void OnDestroy()
		{

			if (this.ownerSkillLocator)
			{
				foreach (GenericSkill skill in this.ownerSkillLocator.allSkills)
				{
					if (skill)
						skill.UnsetSkillOverride(skill, Modules.Survivors.Dancer.lockedSkillDef, GenericSkill.SkillOverridePriority.Replacement);
				}
			}

			if (NetworkServer.active)
            {
				if(this.timer > 0)
                {
					if (this.nextRoot)
					{
						RibbonController next = RibbonController.FindRibbonController(this.nextRoot);
						if (next && this.previousRoot != this.ownerRoot && this.previousRoot != this.nextRoot)
						{
							if (this.previousRoot)
							{
								next.NetworkpreviousRoot = this.previousRoot;
								//Debug.Log("RibbonController.OnDestroy()- Setting " + nextRoot.name + "'s previous to " + previousRoot.name);
							}
							else
							{
								next.NetworkpreviousRoot = null;
								//Debug.Log("RibbonController.OnDestroy()- Setting " + nextRoot.name + "'s previous to null");
							}
						}
					}
					if (this.previousRoot)
					{
						RibbonController previous = RibbonController.FindRibbonController(this.previousRoot);
						if (previous && this.nextRoot != this.ownerRoot && this.nextRoot != this.previousRoot)
						{
							if (this.nextRoot)
							{
								previous.NetworknextRoot = this.nextRoot;
								//Debug.Log("RibbonController.OnDestroy()- Setting " + previousRoot.name + "'s next to " + nextRoot.name);
							}
							else
							{
								previous.NetworknextRoot = null;
								//Debug.Log("RibbonController.OnDestroy()- Setting " + previousRoot.name + "'s next to null");
							}
						}
					}
				}
				
			}


			RibbonController.instancesList.Remove(this);
		}

		public void StartRibbon()
        {
			if(this.ownerRoot)
            {
				this.ownerBody = this.ownerRoot.GetComponent<CharacterBody>();
				this.ownerMachine = this.ownerRoot.GetComponent<EntityStateMachine>();
				this.ownerSkillLocator = this.ownerRoot.GetComponent<SkillLocator>();

				if (NetworkServer.active)
					this.ownerBody.AddTimedBuff(Modules.Buffs.ribbonDebuff, this.timer);

				if (false)//this.ownerBody.isChampion)
				{
					if (this.ownerMachine)
					{
						RibbonedState newNextState = new RibbonedState
						{
							duration = Modules.Buffs.ribbonBossCCDuration,
						};
						this.ownerMachine.SetInterruptState(newNextState, InterruptPriority.Death);
					}
				}	
				
				
			}

			this.ownerAttached = true;
			Util.PlaySound("WhipHit1", this.ownerRoot);
			this.SyncRibbonTimersToNewTime(this.timer);
		}


        private void AttachRibbon()
		{
			if(NetworkServer.active)
            {
				if (!this.nextHealthComponent) this.nextHealthComponent = this.nextRoot.GetComponent<HealthComponent>();

				if (!this.inflictorBody) this.inflictorBody = this.inflictorRoot.GetComponent<CharacterBody>();

				if (this.inflictorRoot && this.nextHealthComponent)
				{
					DamageInfo damageInfo = new DamageInfo
					{
						position = this.nextRoot.transform.position,
						attacker = this.inflictorRoot,
						inflictor = this.inflictorRoot,
						damage = this.damageCoefficient * this.inflictorBody.damage,
						damageColorIndex = DamageColorIndex.Default,
						damageType = DamageType.ApplyMercExpose,
						crit = this.inflictorBody.RollCrit(),
						force = Vector3.zero,
						procChainMask = default(ProcChainMask),
						procCoefficient = 0.67f
					};
					this.nextHealthComponent.TakeDamage(damageInfo);
					GlobalEventManager.instance.OnHitEnemy(damageInfo, this.nextRoot);
					GlobalEventManager.instance.OnHitAll(damageInfo, this.nextRoot);


					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Modules.Assets.ribbonController, this.nextRoot.transform);
					RibbonController newRibbon = gameObject.GetComponent<RibbonController>();
					newRibbon.NetworkpreviousRoot = this.ownerRoot;
					newRibbon.timer = this.timer;
					newRibbon.NetworkownerRoot = this.nextRoot;
					newRibbon.inflictorRoot = this.inflictorRoot;
					NetworkServer.Spawn(gameObject);
					newRibbon.StartRibbon();

					this.nextController = newRibbon;
					if(!this.nextHealthComponent || !this.nextHealthComponent.alive)
                    {
						newRibbon.DetachFromOwner();
                    }
				}
			}
									
		}
		
		
		

		private Vector3 GetTargetRootPosition()
		{
			if (this.nextRoot)
			{
				RibbonController r = RibbonController.FindRibbonController(this.nextRoot);
				if (r)
					return r.transform.position;
				Vector3 result = this.nextRoot.transform.position;
				if (this.nextHealthComponent)
				{
					result = this.nextHealthComponent.body.corePosition;
				}
				return result;
			}
			return base.transform.position;
		}


		

		public void DetachFromOwner()
        {
			this.ownerAttached = false;
			base.transform.SetParent(null);
			this.NetworkownerRoot = null;
			this.ownerBody = null;
			this.ownerMachine = null;
		}

		private void LateUpdate()
		{
			if (this.ownerRoot)
			{
				if (this.ownerAttached)
					base.transform.position = this.ownerRoot.transform.position;
				else
				{
					this.attachOwnerStopwatch += Time.deltaTime;
					Vector3 position = base.transform.position;
					Vector3 position2 = Vector3.Lerp(position, this.ownerRoot.transform.position, this.attachOwnerStopwatch / this.attachOwnerTime);
					base.transform.position = position2;
				}


			}

			if (this.nextRoot)
			{
				this.attachStopwatch += Time.deltaTime;
				Vector3 position = base.transform.position;
				if (!this.ribbonAttached)
				{
					Vector3 position2 = Vector3.Lerp(position, this.GetTargetRootPosition(), this.attachStopwatch / this.attachTime);
					if (this.ribbonLineRenderer)
					{
						Vector3[] v = new Vector3[2];
						v[0] = base.transform.position;
						v[1] = position2;
						this.ribbonLineRenderer.SetPositions(v);
					}
				}
				else
				{
					if (this.ribbonLineRenderer)
					{
						Vector3[] v = new Vector3[2];
						v[0] = base.transform.position;
						v[1] = this.nextController != null ? this.nextController.transform.position : this.nextRoot.transform.position;
						this.ribbonLineRenderer.SetPositions(v);

					}
				}

			}
		}

		private void FixedUpdate()
		{
			this.timer -= Time.fixedDeltaTime;
						

			if (this.nextRoot)
			{
				if (this.startRibbonWhenAvailable)
				{
					this.startRibbonWhenAvailable = false;
					Util.PlaySound("Play_item_proc_whip", base.gameObject);
					this.ribbonLineRenderer.enabled = true;
				}
				if (!this.ribbonAttached)
				{
					this.fixedAttachStopwatch += Time.fixedDeltaTime;
					if (this.fixedAttachStopwatch >= this.attachTime)
					{
						this.AttachRibbon();
						this.ribbonAttached = true;

					}
				}
				if (this.nextRoot == this.previousRoot && this.nextRoot)
				{
					Debug.LogError("RibbonController.FixedUpdate()- Ribbon's next target and previous target were the same!");
					this.NetworknextRoot = null;
				}
			}

			if (!this.ownerAttached)
			{
				if (!this.nextRoot && !this.nextController)
				{
					this.SearchNewTarget();
				}
				if (this.ribbonAttached)
				{
					Destroy(base.gameObject);
					return;
				}				
			}
            else if (this.ownerRoot)
            {
				if (!this.ownerMachine) this.ownerMachine = this.ownerRoot.GetComponent<EntityStateMachine>();
				if (!this.ownerBody) this.ownerBody = this.ownerRoot.GetComponent<CharacterBody>();
				if (this.timer < Buffs.ribbonDebuffDuration - Buffs.ribbonBossCCDuration && this.ownerBody && this.ownerBody.isChampion)
				{
					if (this.ownerSkillLocator)
					{
						foreach (GenericSkill skill in this.ownerSkillLocator.allSkills)
						{
							if (skill)
								skill.UnsetSkillOverride(skill, Modules.Survivors.Dancer.lockedSkillDef, GenericSkill.SkillOverridePriority.Replacement);
						}
					}
				}
			}

			if(!this.nextRoot)
			{
				this.nextHealthComponent = null;
				if(!this.nextController)
                {
					this.ribbonAttached = false;
					this.ribbonLineRenderer.enabled = false;
					this.startRibbonWhenAvailable = true;
					this.fixedAttachStopwatch = 0f;
					this.attachStopwatch = 0f;
				}
				
			}

			if(NetworkServer.active)
            {
				if (this.timer < 0f)
				{
					NetworkServer.Destroy(base.gameObject);
					return;
				}
				if (this.ownerAttached && !this.ownerBody.HasBuff(Modules.Buffs.ribbonDebuff) && this.ownerBody.healthComponent.alive)
				{
					//Debug.LogError("RibbonController.FixedUpdate()- Ribbon debuff expired, destroying");
					NetworkServer.Destroy(base.gameObject);
					return;
				}
				if (this.nextRoot == this.ownerRoot || this.previousRoot == this.ownerRoot)
				{
					if (this.ownerRoot != null)
					{
						Debug.LogError("RibbonController.FixedUpdate()- Ribbon got fucked up real bad, destroying");
						NetworkServer.Destroy(base.gameObject);
						return;
					}
				}
				
				if (this.nextRoot && this.ownerRoot && this.ownerAttached)
				{
					RibbonController ribbon = RibbonController.FindRibbonController(this.nextRoot);
					if (ribbon && !ribbon.previousRoot)
					{
						Debug.Log("Setting " + nextRoot.name + "'s previous to " + ownerRoot.name);
						ribbon.NetworkpreviousRoot = this.ownerRoot;
					}				
				}			
			}
		
			
			
		}

		public void GetNextObjects(ref List<GameObject> list)
		{

			if (this.nextRoot)
			{
				list.Add(nextRoot);
				RibbonController controller = RibbonController.FindRibbonController(this.nextRoot);
				if (controller)
				{
					controller.GetNextObjects(ref list);
				}
			}

		}

		public void GetPreviousObjects(ref List<GameObject> list)
		{
			if (this.previousRoot)
			{
				list.Add(previousRoot);
				RibbonController controller = RibbonController.FindRibbonController(this.previousRoot);
				if (controller)
				{

					controller.GetPreviousObjects(ref list);
				}
			}

		}

		private void SetRibbonTimer(GameObject gameObject, float timeToSet)
		{
			
			RibbonController controller = RibbonController.FindRibbonController(gameObject);
			if (controller)
			{
				controller.timer = timeToSet;
			}
			

			CharacterBody body = gameObject.GetComponent<CharacterBody>();
			if (body)
			{
				foreach (CharacterBody.TimedBuff buff in body.timedBuffs)
				{
					if (buff.buffIndex == Modules.Buffs.ribbonDebuff.buffIndex && timeToSet >= buff.timer)
					{
						buff.timer = timeToSet;
						DotController dotController = DotController.FindDotController(gameObject);
						bool flag = false;
						if (dotController)
						{
							flag = dotController.HasDotActive(Modules.Buffs.ribbonDotIndex);
						}
						if (!flag)
						{
							InflictDotInfo inflictDotInfo = new InflictDotInfo
							{
								attackerObject = this.inflictorRoot,
								victimObject = gameObject,
								dotIndex = Buffs.ribbonDotIndex,
								duration = timeToSet,
								damageMultiplier = 1f
							};
							DotController.InflictDot(ref inflictDotInfo);
						}
						else
						{
							for (int i = dotController.dotStackList.Count - 1; i >= 0; i--)
							{
								DotController.DotStack dotStack = dotController.dotStackList[i];
								if (dotStack.dotIndex == Buffs.ribbonDotIndex)
								{
									dotStack.timer = timeToSet;
								}
							}
						}
						SkillLocator skillLocator = gameObject.GetComponent<SkillLocator>();
						if (skillLocator)
						{
							foreach (GenericSkill skill in skillLocator.allSkills)
							{
								if (skill)
									skill.SetSkillOverride(skill, Modules.Survivors.Dancer.lockedSkillDef, GenericSkill.SkillOverridePriority.Replacement);
							}
						}
						/*
						EntityStateMachine e = gameObject.GetComponent<EntityStateMachine>();
						if (e && !body.isChampion)
						{
							if (!(e.state is RibbonedState))
							{
								RibbonedState newNextState = new RibbonedState
								{
									duration = timeToSet,
								};
								e.SetInterruptState(newNextState, InterruptPriority.Frozen);
							}
							else
							{
								RibbonedState ribbonedState = e.state as RibbonedState;
								if (ribbonedState.timer < timeToSet)
								{
									ribbonedState.SetNewTimer(timeToSet);
								}
							}
						}
						*/
					}
				}
			}
		}
		public void SyncRibbonTimersToNewTime(float newTime)
		{
			if (this.ownerRoot)
			{
				if (!this.ownerBody)
					this.ownerBody = this.ownerRoot.GetComponent<CharacterBody>();

				this.SetRibbonTimer(this.ownerRoot, newTime);
			}


			List<GameObject> next = new List<GameObject>();
			this.GetNextObjects(ref next);
			foreach (GameObject gameObject in next)
				this.SetRibbonTimer(gameObject, newTime);

			List<GameObject> previous = new List<GameObject>();
			this.GetPreviousObjects(ref previous);
			foreach (GameObject gameObject in previous)
				this.SetRibbonTimer(gameObject, newTime);
		}

		public void SearchNewTarget()
        {
			if(this.inflictorRoot && !this.inflictorBody)
				this.inflictorBody = this.inflictorRoot.GetComponent<CharacterBody>();
			BullseyeSearch bullseyeSearch = new BullseyeSearch();
			bullseyeSearch.searchOrigin = base.transform.position;
			bullseyeSearch.maxDistanceFilter = Modules.StaticValues.ribbonSpreadRange;
			bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
			bullseyeSearch.teamMaskFilter.RemoveTeam(this.inflictorBody.teamComponent.teamIndex);
			bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
			bullseyeSearch.filterByLoS = false;
			bullseyeSearch.RefreshCandidates();
			if(this.ownerRoot)
				bullseyeSearch.FilterOutGameObject(this.ownerRoot);
			if (this.nextRoot)
				bullseyeSearch.FilterOutGameObject(this.nextRoot);
			if (this.previousRoot)
				bullseyeSearch.FilterOutGameObject(this.previousRoot);
			foreach (HurtBox hurtBox in bullseyeSearch.GetResults())
			{
				if (hurtBox.healthComponent && hurtBox.healthComponent.body)
				{
					if (hurtBox.healthComponent.body.HasBuff(Modules.Buffs.ribbonDebuff))
						bullseyeSearch.FilterOutGameObject(hurtBox.healthComponent.gameObject);
					if (RibbonController.FindRibbonController(hurtBox.healthComponent.gameObject))
						bullseyeSearch.FilterOutGameObject(hurtBox.healthComponent.gameObject);
				}
			}
			HurtBox target = bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
			if (target && target.healthComponent && target.healthComponent.body && target.healthComponent.alive)
			{
				this.ribbonExtended = true;
				this.NetworknextRoot = target.healthComponent.gameObject;
				this.nextHealthComponent = target.healthComponent;
			}
			else
				this.NetworknextRoot = null;
		}

		public void SearchNewOwner()
		{
			if (!this.inflictorBody)
				this.inflictorBody = this.inflictorRoot.GetComponent<CharacterBody>();
			BullseyeSearch bullseyeSearch = new BullseyeSearch();
			bullseyeSearch.searchOrigin = base.transform.position;
			bullseyeSearch.maxDistanceFilter = Modules.StaticValues.ribbonSpreadRange;
			bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
			bullseyeSearch.teamMaskFilter.RemoveTeam(this.inflictorBody.teamComponent.teamIndex);
			bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
			bullseyeSearch.filterByLoS = false;
			bullseyeSearch.RefreshCandidates();
			if(this.nextRoot)
				bullseyeSearch.FilterOutGameObject(this.nextRoot);
			if (this.previousRoot)
				bullseyeSearch.FilterOutGameObject(this.previousRoot);
			foreach (HurtBox hurtBox in bullseyeSearch.GetResults())
			{
				if (hurtBox.healthComponent && hurtBox.healthComponent.body)
				{
					if (hurtBox.healthComponent.body.HasBuff(Modules.Buffs.ribbonDebuff))
						bullseyeSearch.FilterOutGameObject(hurtBox.healthComponent.gameObject);
					if (RibbonController.FindRibbonController(hurtBox.healthComponent.gameObject))
						bullseyeSearch.FilterOutGameObject(hurtBox.healthComponent.gameObject);
				}
			}
			HurtBox target = bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
			if (target && target.healthComponent && target.healthComponent.body && target.healthComponent.alive)
			{
				this.NetworkownerRoot = target.healthComponent.gameObject;
				this.ownerBody = target.healthComponent.body;
			}
			else
				this.NetworkownerRoot = null;
		}


		private void UNetVersion()
		{
		}

		public GameObject NetworknextRoot
		{
			get
			{
				return this.nextRoot;
			}
			[param: In]
			set
			{
				base.SetSyncVarGameObject(value, ref this.nextRoot, 1U, ref this.___nextRootNetId);
			}
		}

		public GameObject NetworkownerRoot
		{
			get
			{
				return this.ownerRoot;
			}
			[param: In]
			set
			{
				base.SetSyncVarGameObject(value, ref this.ownerRoot, 2U, ref this.___ownerRootNetId);
			}
		}

		public GameObject NetworkpreviousRoot
		{
			get
			{
				return this.previousRoot;
			}
			[param: In]
			set
			{
				base.SetSyncVarGameObject(value, ref this.previousRoot, 3U, ref this.___previousRootNetId);
			}
		}

		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.nextRoot);
				writer.Write(this.ownerRoot);
				writer.Write(this.previousRoot);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.nextRoot);
			}
			if ((base.syncVarDirtyBits & 2U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.ownerRoot);
			}
			if ((base.syncVarDirtyBits & 3U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.previousRoot);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				
				this.___nextRootNetId = reader.ReadNetworkId();
				this.___ownerRootNetId = reader.ReadNetworkId();
				this.___previousRootNetId = reader.ReadNetworkId();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.nextRoot = reader.ReadGameObject();
			}
			if ((num & 2) != 0)
			{
				this.ownerRoot = reader.ReadGameObject();
			}
			if ((num & 3) != 0)
			{
				this.previousRoot = reader.ReadGameObject();
			}
		}

		public override void PreStartClient()
		{
			if (!this.___nextRootNetId.IsEmpty())
			{
				this.NetworknextRoot = ClientScene.FindLocalObject(this.___nextRootNetId);
			}
			if (!this.___ownerRootNetId.IsEmpty())
			{
				this.NetworkownerRoot = ClientScene.FindLocalObject(this.___ownerRootNetId);
			}
			if (!this.___previousRootNetId.IsEmpty())
			{
				this.NetworkpreviousRoot = ClientScene.FindLocalObject(this.___previousRootNetId);
			}
		}

		private static readonly List<RibbonController> instancesList = new List<RibbonController>();

		public static RibbonController FindRibbonController(GameObject victimObject)
		{
			if (false)//!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'Dancer.RibbonController Dancer.RibbonController::FindRibbonController(UnityEngine.GameObject)' called on client");
				return null;
			}
			int i = 0;
			int count = RibbonController.instancesList.Count;
			while (i < count)
			{
				if (victimObject == RibbonController.instancesList[i].ownerRoot)
				{
					return RibbonController.instancesList[i];
				}
				i++;
			}
			return null;
		}

		private float destroyTime = 0.5f;
		private float destroyStopwatch;

		[SyncVar]
		public GameObject nextRoot;
		[SyncVar]
		public GameObject ownerRoot;

		public GameObject previousRoot;


		public GameObject inflictorRoot;
		private CharacterBody inflictorBody;

		public float attachTime = 0.25f;

		private float fixedAttachStopwatch;

		private float attachStopwatch;
		public bool ribbonAttached;
		private bool startRibbonWhenAvailable = true;

		private BezierCurveLine bezierCurveLine;
		private LineRenderer ribbonLineRenderer;

		private HealthComponent nextHealthComponent;

		private HealthComponent ownerHealthComponent;

		private HealthComponent previousHealthComponent;

		private CharacterBody ownerBody;

		private NetworkInstanceId ___nextRootNetId;
		private NetworkInstanceId ___ownerRootNetId;
		private NetworkInstanceId ___previousRootNetId;
	}
}
