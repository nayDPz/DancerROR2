using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using Dancer.SkillStates;
using EntityStates;

namespace Dancer.Modules.Components
{
	//[RequireComponent(typeof(BezierCurveLine))]
	public class RibbonController : NetworkBehaviour
	{
		private EntityStateMachine ownerMachine;
		private float damageCoefficient = 3f;
		private void Awake()
		{
			

			this.ownerRoot = base.gameObject;
			this.ownerBody = base.GetComponent<CharacterBody>();
			this.ownerMachine = base.GetComponent<EntityStateMachine>();

			this.ribbonPrefab = Modules.Assets.ribbonLine;// Resources.Load<GameObject>("Prefabs/NetworkedObjects/TarTether");
			
		}

		private void AttachRibbon()
		{

			if (!this.nextHealthComponent)
			{
				this.nextHealthComponent = this.nextRoot.GetComponent<HealthComponent>();
			}
			if (!this.ownerHealthComponent)
			{
				this.ownerHealthComponent = this.ownerRoot.GetComponent<HealthComponent>();
			}
			if (!this.ownerBody)
			{
				this.ownerBody = this.ownerRoot.GetComponent<CharacterBody>();
			}
			if (!this.inflictorBody)
			{
				this.inflictorBody = this.inflictorRoot.GetComponent<CharacterBody>();
			}
			if (this.ownerRoot && this.inflictorRoot)
			{
				DamageInfo damageInfo = new DamageInfo
				{
					position = this.nextRoot.transform.position,
					attacker = this.inflictorRoot,
					inflictor = this.ownerRoot,
					damage = this.damageCoefficient * this.ownerBody.damage,
					damageColorIndex = DamageColorIndex.Default,
					damageType = DamageType.BlightOnHit,
					crit = this.inflictorBody.RollCrit(),
					force = Vector3.zero,
					procChainMask = default(ProcChainMask),
					procCoefficient = 0f
				};
				this.nextHealthComponent.TakeDamage(damageInfo);

				float time = Modules.Buffs.ribbonDebuffDuration;
				foreach (CharacterBody.TimedBuff buff in this.ownerBody.timedBuffs)
				{
					if (buff.buffIndex == Modules.Buffs.ribbonDebuff.buffIndex)
						time = buff.timer;
				}
				this.nextHealthComponent.body.AddTimedBuff(Modules.Buffs.ribbonDebuff, time);
				EntityStateMachine component = this.nextRoot.GetComponent<EntityStateMachine>();
				if (CanBeRibboned(this.nextHealthComponent.body) && component)
				{

					RibbonedState newNextState = new RibbonedState
					{
						duration = time,
					};
					component.SetInterruptState(newNextState, InterruptPriority.Frozen);


				}

				if (!this.nextHealthComponent.alive)
				{
					this.NetworknextRoot = null;
				}
			}						
		}

		private void LateUpdate()
        {
			if (this.nextRoot)
			{
				this.attachStopwatch += Time.deltaTime;
				Vector3 position = this.ownerRoot.transform.position;
				if (!this.ribbonAttached)
				{
					Vector3 position2 = Vector3.Lerp(position, this.GetTargetRootPosition(), this.attachStopwatch / this.attachTime);
					if (this.ribbonRenderer)
					{
						Vector3[] v = new Vector3[2];
						v[0] = this.ownerRoot.transform.position;
						v[1] = position2;
						this.ribbonInstace.GetComponent<LineRenderer>().SetPositions(v);
					}

					return;
				}
				else
				{
					if (this.bezierCurveLine)
					{
						//this.bezierCurveLine.transform.position = this.ownerRoot.transform.position;
						//this.bezierCurveLine.endTransform.position = this.nextRoot.transform.position;
					}
					if (this.ribbonRenderer)
					{
						Vector3[] v = new Vector3[2];
						v[0] = this.ownerRoot.transform.position;
						v[1] = this.nextRoot.transform.position;
						this.ribbonInstace.GetComponent<LineRenderer>().SetPositions(v);

					}
				}

			}
		}

		private Vector3 GetTargetRootPosition()
		{
			if (this.nextRoot)
			{
				Vector3 result = this.nextRoot.transform.position;
				if (this.nextHealthComponent)
				{
					result = this.nextHealthComponent.body.corePosition;
				}
				return result;
			}
			return base.transform.position;
		}

		private void Update()
		{
			
		}

		private void FixedUpdate()
		{
			if(this.nextRoot == this.ownerRoot || this.previousRoot == this.ownerRoot)
            {
				Debug.LogError("Ribbon got fucked up real bad, destroying");
				Destroy(this);
				return;
			}

			if (this.nextRoot == this.previousRoot && this.nextRoot != null)
            {
				Debug.LogError("Ribbon's next target and previous target were the same!");
				this.nextRoot = null;
			}
				

			if (this.nextRoot && this.ownerRoot)
			{

				if (this.nextRoot.GetComponent<RibbonController>() && !this.nextRoot.GetComponent<RibbonController>().previousRoot)
				{
					Debug.Log("Setting " + nextRoot.name + "'s previous to " + ownerRoot.name);
					this.nextRoot.GetComponent<RibbonController>().previousRoot = this.ownerRoot;
				}

				if (this.startRibbonWhenAvailable)
                {
					this.startRibbonWhenAvailable = false;
					this.ribbonInstace = UnityEngine.Object.Instantiate<GameObject>(this.ribbonPrefab, this.ownerBody.corePosition, Quaternion.identity);
					this.ribbonRenderer = this.ribbonInstace.GetComponent<LineRenderer>();
					this.ribbonRenderer.positionCount = 2;
					/*
					this.bezierCurveLine = this.ribbonInstace.GetComponent<BezierCurveLine>();
					this.bezierCurveLine.v0 = Vector3.zero;
					this.bezierCurveLine.v1 = Vector3.zero;
					*/
				}
				this.fixedAttachStopwatch += Time.fixedDeltaTime;
				Vector3 targetRootPosition = this.GetTargetRootPosition();
				if (!this.ribbonAttached && this.fixedAttachStopwatch >= this.attachTime)
				{
					this.AttachRibbon();
					this.ribbonAttached = true;
					return;
				}

				

			}

			if(!this.nextRoot)
            {
				if (this.ribbonInstace)
					GameObject.Destroy(this.ribbonInstace);
				this.ribbonAttached = false;
				this.startRibbonWhenAvailable = true;
				this.fixedAttachStopwatch = 0f;
				this.attachStopwatch = 0f;
            }

			if (!ownerBody.HasBuff(Modules.Buffs.ribbonDebuff))
				Destroy(this);

		}

		private void OnDestroy()
        {
			if(this.ribbonInstace)
            {
				GameObject.Destroy(this.ribbonInstace);
            }

			
			if(this.nextRoot)
            {
                RibbonController next = this.nextRoot.GetComponent<RibbonController>();
				if (next && this.previousRoot != this.ownerRoot && this.previousRoot != this.nextRoot)
				{
					if (this.previousRoot)
                    {
						next.previousRoot = this.previousRoot;
						Debug.Log("(ONDESTROY)- Setting " + nextRoot.name + "'s previous to " + ownerRoot.name + "'s previous, " + previousRoot.name);
					}
						
					else
                    {
						next.previousRoot = null;
						Debug.Log("(ONDESTROY)- Setting " + nextRoot.name + "'s previous to " + ownerRoot.name + "'s null");
					}
						

					
				}
			}
			if(this.previousRoot)
            {
				RibbonController previous = this.previousRoot.GetComponent<RibbonController>();


				if (previous && this.nextRoot != this.ownerRoot && this.nextRoot != this.previousRoot)
				{
					if (this.nextRoot)
                    {
						previous.nextRoot = this.nextRoot;
						Debug.Log("(ONDESTROY)- Setting " + previousRoot.name + "'s next to " + ownerRoot.name + "'s next, " + nextRoot.name);
					}
						
					else
                    {
						previous.nextRoot = null;
						Debug.Log("(ONDESTROY)- Setting " + previousRoot.name + "'s next to " + ownerRoot.name + "'s null");
					}
						

					
				}
			}
			
				
            
        }

		private bool CanBeRibboned(CharacterBody body)
        {
			return body.GetComponent<SetStateOnHurt>() && body.GetComponent<SetStateOnHurt>().canBeFrozen;				
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

		private float destroyTime = 0.5f;
		private float destroyStopwatch;

		private GameObject ribbonPrefab;
		private GameObject ribbonInstace;

		[SyncVar]
		public GameObject nextRoot;
		[SyncVar]
		public GameObject ownerRoot;

		public GameObject previousRoot;


		public GameObject inflictorRoot;
		private CharacterBody inflictorBody;

		public float attachTime = 0.15f;

		private float fixedAttachStopwatch;

		private float attachStopwatch;
		private bool ribbonAttached;
		private bool startRibbonWhenAvailable = true;

		private BezierCurveLine bezierCurveLine;
		private LineRenderer ribbonRenderer;

		private HealthComponent nextHealthComponent;

		private HealthComponent ownerHealthComponent;

		private HealthComponent previousHealthComponent;

		private CharacterBody ownerBody;

		private NetworkInstanceId ___nextRootNetId;
		private NetworkInstanceId ___ownerRootNetId;
		private NetworkInstanceId ___previousRootNetId;
	}
}
