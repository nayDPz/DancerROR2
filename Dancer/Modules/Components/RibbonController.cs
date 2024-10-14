using KinematicCharacterController;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace Dancer.Modules.Components
{

    public class RibbonController : NetworkBehaviour
    {
        public static bool naturalSpread = false;

        private float[] cooldowns;

        private int[] stocks;

        private bool skillOverride;

        public float timer;

        private EntityStateMachine ownerMachine;

        private float damageCoefficient = 0.75f;

        private float fixedAttachOwnerStopwatch;

        private SkillLocator ownerSkillLocator;

        public int spreadsRemaining;

        private RibbonController nextController;

        private float attachOwnerStopwatch;

        private float attachOwnerTime = 0.25f;

        private bool ownerAttached = true;

        private bool ribbonExtended;

        private static readonly List<RibbonController> instancesList = new List<RibbonController>();

        [SyncVar]
        public GameObject nextRoot;

        [SyncVar]
        public GameObject ownerRoot;

        public GameObject previousRoot;

        public GameObject inflictorRoot;

        private CharacterBody inflictorBody;

        public float attachTime = 2f;

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

        public GameObject NetworknextRoot
        {
            get
            {
                return nextRoot;
            }
            [param: In]
            set
            {
                SetSyncVarGameObject(value, ref nextRoot, 1u, ref ___nextRootNetId);
            }
        }

        public GameObject NetworkownerRoot
        {
            get
            {
                return ownerRoot;
            }
            [param: In]
            set
            {
                SetSyncVarGameObject(value, ref ownerRoot, 2u, ref ___ownerRootNetId);
            }
        }

        public GameObject NetworkpreviousRoot
        {
            get
            {
                return previousRoot;
            }
            [param: In]
            set
            {
                SetSyncVarGameObject(value, ref previousRoot, 3u, ref ___previousRootNetId);
            }
        }

        private void Start()
        {
            ribbonLineRenderer = base.transform.Find("RibbonLine").gameObject.GetComponent<LineRenderer>();
            ribbonLineRenderer.positionCount = 2;
            ribbonLineRenderer.useWorldSpace = true;
            ribbonLineRenderer.enabled = false;
            instancesList.Add(this);
        }

        private void OnDisable()
        {
            Object.Destroy(base.gameObject);
        }

        private void UnsetOverride()
        {
            if (!skillOverride)
            {
                return;
            }
            skillOverride = false;
            if (!ownerSkillLocator)
            {
                ownerSkillLocator = ownerRoot.GetComponent<SkillLocator>();
            }
            SkillLocator skillLocator = ownerSkillLocator;
            if (!skillLocator)
            {
                return;
            }
            for (int i = 0; i < skillLocator.allSkills.Length; i++)
            {
                GenericSkill genericSkill = skillLocator.allSkills[i];
                if ((bool)genericSkill)
                {
                    genericSkill.UnsetSkillOverride(genericSkill, Dancer.Modules.Survivors.Dancer.lockedSkillDef, GenericSkill.SkillOverridePriority.Replacement);
                    genericSkill.rechargeStopwatch = cooldowns[i];
                    genericSkill.stock = stocks[i];
                }
            }
        }

        private void OnDestroy()
        {
            if (NetworkServer.active && timer > 0f)
            {
                if ((bool)nextRoot)
                {
                    RibbonController ribbonController = FindRibbonController(nextRoot);
                    if ((bool)ribbonController && previousRoot != ownerRoot && previousRoot != nextRoot)
                    {
                        if ((bool)previousRoot)
                        {
                            ribbonController.NetworkpreviousRoot = previousRoot;
                        }
                        else
                        {
                            ribbonController.NetworkpreviousRoot = null;
                        }
                    }
                }
                if ((bool)previousRoot)
                {
                    RibbonController ribbonController2 = FindRibbonController(previousRoot);
                    if ((bool)ribbonController2 && nextRoot != ownerRoot && nextRoot != previousRoot)
                    {
                        if ((bool)nextRoot)
                        {
                            ribbonController2.NetworknextRoot = nextRoot;
                        }
                        else
                        {
                            ribbonController2.NetworknextRoot = null;
                        }
                    }
                }
            }
            instancesList.Remove(this);
        }

        public void StartRibbon()
        {
            if ((bool)ownerRoot)
            {
                ownerBody = ownerRoot.GetComponent<CharacterBody>();
                ownerMachine = ownerRoot.GetComponent<EntityStateMachine>();
                ownerSkillLocator = ownerRoot.GetComponent<SkillLocator>();
                if (NetworkServer.active)
                {
                    ownerBody.AddTimedBuff(Buffs.ribbonDebuff, timer);
                }
            }
            ownerAttached = ownerRoot != null;
            Util.PlaySound("WhipHit1", ownerRoot);
            SyncRibbonTimersToNewTime(timer);
            if (naturalSpread)
            {
                SearchNewTarget();
                if (spreadsRemaining > 0)
                {
                    SpeedUpRibbon(0.25f);
                    spreadsRemaining--;
                }
            }
            else
            {
                attachTime = 0.25f;
                if (spreadsRemaining > 0)
                {
                    SearchNewTarget();
                    spreadsRemaining--;
                }
            }
        }

        private void AttachRibbon()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            if (!nextHealthComponent)
            {
                nextHealthComponent = nextRoot.GetComponent<HealthComponent>();
            }
            if (!inflictorBody)
            {
                inflictorBody = inflictorRoot.GetComponent<CharacterBody>();
            }
            if ((bool)inflictorRoot && (bool)nextHealthComponent)
            {
                CharacterBody body = nextHealthComponent.body;
                Vector3 vector = (ownerRoot ? ownerRoot.transform.position : base.transform.position);
                Vector3 normalized = (vector - nextRoot.transform.position).normalized;
                normalized *= 800f;
                if ((bool)nextRoot.GetComponent<KinematicCharacterMotor>())
                {
                    nextRoot.GetComponent<KinematicCharacterMotor>().ForceUnground();
                }
                CharacterMotor characterMotor = body.characterMotor;
                float num = 0.25f;
                if ((bool)characterMotor)
                {
                    float num2 = Mathf.Max(100f, characterMotor.mass);
                    num = num2 / 100f;
                    normalized *= num;
                    characterMotor.ApplyForce(normalized);
                }
                else if ((bool)body.rigidbody)
                {
                    float num3 = Mathf.Max(50f, body.rigidbody.mass);
                    num = num3 / 200f;
                    normalized *= num;
                    body.rigidbody.AddForce(normalized, ForceMode.Impulse);
                }
                DamageInfo damageInfo = new DamageInfo
                {
                    position = nextRoot.transform.position,
                    attacker = inflictorRoot,
                    inflictor = inflictorRoot,
                    damage = damageCoefficient * inflictorBody.damage,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = DamageType.ApplyMercExpose,
                    crit = inflictorBody.RollCrit(),
                    force = normalized,
                    procChainMask = default(ProcChainMask),
                    procCoefficient = 0.3f
                };
                nextHealthComponent.TakeDamage(damageInfo);
                GlobalEventManager.instance.OnHitEnemy(damageInfo, nextRoot);
                GlobalEventManager.instance.OnHitAll(damageInfo, nextRoot);
                GameObject gameObject = Object.Instantiate(Assets.ribbonController, nextRoot.transform);
                RibbonController component = gameObject.GetComponent<RibbonController>();
                component.NetworkpreviousRoot = ownerRoot;
                component.timer = timer;
                component.NetworkownerRoot = nextRoot;
                component.inflictorRoot = inflictorRoot;
                component.spreadsRemaining = spreadsRemaining;
                NetworkServer.Spawn(gameObject);
                component.StartRibbon();
                nextController = component;
                if (!nextHealthComponent || !nextHealthComponent.alive)
                {
                    component.DetachFromOwner();
                }
            }
        }

        private Vector3 GetTargetRootPosition()
        {
            if ((bool)nextRoot)
            {
                RibbonController ribbonController = FindRibbonController(nextRoot);
                if ((bool)ribbonController)
                {
                    return ribbonController.transform.position;
                }
                Vector3 result = nextRoot.transform.position;
                if ((bool)nextHealthComponent)
                {
                    result = nextHealthComponent.body.corePosition;
                }
                return result;
            }
            return base.transform.position;
        }

        public void DetachFromOwner()
        {
            ownerAttached = false;
            base.transform.SetParent(null);
            NetworkownerRoot = null;
            ownerBody = null;
            ownerMachine = null;
        }

        private void LateUpdate()
        {
            if ((bool)ownerRoot)
            {
                if (ownerAttached)
                {
                    base.transform.position = ownerRoot.transform.position;
                }
                else
                {
                    attachOwnerStopwatch += Time.deltaTime;
                    Vector3 position = base.transform.position;
                    Vector3 position2 = Vector3.Lerp(position, ownerRoot.transform.position, attachOwnerStopwatch / attachOwnerTime);
                    base.transform.position = position2;
                }
            }
            if (!nextRoot)
            {
                return;
            }
            attachStopwatch += Time.deltaTime;
            Vector3 position3 = base.transform.position;
            if (!ribbonAttached)
            {
                Vector3 vector = Vector3.Lerp(position3, GetTargetRootPosition(), attachStopwatch / attachTime);
                if ((bool)ribbonLineRenderer)
                {
                    Vector3[] positions = new Vector3[2]
                    {
                    base.transform.position,
                    vector
                    };
                    ribbonLineRenderer.SetPositions(positions);
                }
            }
            else if ((bool)ribbonLineRenderer)
            {
                Vector3[] positions2 = new Vector3[2]
                {
                base.transform.position,
                (nextController != null) ? nextController.transform.position : nextRoot.transform.position
                };
                ribbonLineRenderer.SetPositions(positions2);
            }
        }

        public void SpeedUpRibbon(float newTime)
        {
            float num = fixedAttachStopwatch / attachTime;
            fixedAttachStopwatch = newTime * num;
            num = attachStopwatch / attachTime;
            attachStopwatch = newTime * num;
            attachTime = newTime;
        }

        private void FixedUpdate()
        {
            timer -= Time.fixedDeltaTime;
            if ((bool)nextRoot)
            {
                if (startRibbonWhenAvailable)
                {
                    startRibbonWhenAvailable = false;
                    Util.PlaySound("Play_item_proc_whip", base.gameObject);
                    ribbonLineRenderer.enabled = true;
                }
                if (!ribbonAttached)
                {
                    fixedAttachStopwatch += Time.fixedDeltaTime;
                    if (fixedAttachStopwatch >= attachTime)
                    {
                        AttachRibbon();
                        ribbonAttached = true;
                    }
                }
                if (nextRoot == previousRoot && (bool)nextRoot)
                {
                    Debug.LogError("RibbonController.FixedUpdate()- Ribbon's next target and previous target were the same!");
                    NetworknextRoot = null;
                }
            }
            if (!ownerAttached)
            {
                if (!nextRoot && !nextController)
                {
                    SearchNewTarget();
                }
                if (ribbonAttached)
                {
                    Object.Destroy(base.gameObject);
                    return;
                }
            }
            else if ((bool)ownerRoot)
            {
                if (!ownerMachine)
                {
                    ownerMachine = ownerRoot.GetComponent<EntityStateMachine>();
                }
                if (!ownerBody)
                {
                    ownerBody = ownerRoot.GetComponent<CharacterBody>();
                }
            }
            if (!nextRoot)
            {
                nextHealthComponent = null;
                if (!nextController)
                {
                    ribbonAttached = false;
                    ribbonLineRenderer.enabled = false;
                    startRibbonWhenAvailable = true;
                    fixedAttachStopwatch = 0f;
                    attachStopwatch = 0f;
                }
            }
            if (!NetworkServer.active)
            {
                return;
            }
            if (timer < 0f)
            {
                NetworkServer.Destroy(base.gameObject);
            }
            else if (ownerAttached && !ownerBody.HasBuff(Buffs.ribbonDebuff) && ownerBody.healthComponent.alive)
            {
                NetworkServer.Destroy(base.gameObject);
            }
            else if ((nextRoot == ownerRoot || previousRoot == ownerRoot) && ownerRoot != null)
            {
                Debug.LogError("RibbonController.FixedUpdate()- Ribbon got fucked up real bad, destroying");
                NetworkServer.Destroy(base.gameObject);
            }
            else if ((bool)nextRoot && (bool)ownerRoot && ownerAttached)
            {
                RibbonController ribbonController = FindRibbonController(nextRoot);
                if ((bool)ribbonController && !ribbonController.previousRoot)
                {
                    Debug.Log("Setting " + nextRoot.name + "'s previous to " + ownerRoot.name);
                    ribbonController.NetworkpreviousRoot = ownerRoot;
                }
            }
        }

        public void GetNextObjects(ref List<GameObject> list)
        {
            if ((bool)nextRoot)
            {
                list.Add(nextRoot);
                RibbonController ribbonController = FindRibbonController(nextRoot);
                if ((bool)ribbonController)
                {
                    ribbonController.GetNextObjects(ref list);
                }
            }
        }

        public void GetPreviousObjects(ref List<GameObject> list)
        {
            if ((bool)previousRoot)
            {
                list.Add(previousRoot);
                RibbonController ribbonController = FindRibbonController(previousRoot);
                if ((bool)ribbonController)
                {
                    ribbonController.GetPreviousObjects(ref list);
                }
            }
        }

        private void SetRibbonTimer(GameObject gameObject, float timeToSet)
        {
            RibbonController ribbonController = FindRibbonController(gameObject);
            if ((bool)ribbonController)
            {
                ribbonController.timer = timeToSet;
            }
            CharacterBody component = gameObject.GetComponent<CharacterBody>();
            if (!component)
            {
                return;
            }
            foreach (CharacterBody.TimedBuff timedBuff in component.timedBuffs)
            {
                if (timedBuff.buffIndex == Buffs.ribbonDebuff.buffIndex && timeToSet >= timedBuff.timer)
                {
                    timedBuff.timer = timeToSet;
                }
            }
        }

        public void SyncRibbonTimersToNewTime(float newTime)
        {
            if ((bool)ownerRoot)
            {
                if (!ownerBody)
                {
                    ownerBody = ownerRoot.GetComponent<CharacterBody>();
                }
                SetRibbonTimer(ownerRoot, newTime);
            }
            List<GameObject> list = new List<GameObject>();
            GetNextObjects(ref list);
            foreach (GameObject item in list)
            {
                SetRibbonTimer(item, newTime);
            }
            List<GameObject> list2 = new List<GameObject>();
            GetPreviousObjects(ref list2);
            foreach (GameObject item2 in list2)
            {
                SetRibbonTimer(item2, newTime);
            }
        }

        public void SearchNewTarget()
        {
            if ((bool)inflictorRoot && !inflictorBody)
            {
                inflictorBody = inflictorRoot.GetComponent<CharacterBody>();
            }
            BullseyeSearch bullseyeSearch = new BullseyeSearch();
            bullseyeSearch.searchOrigin = base.transform.position;
            bullseyeSearch.maxDistanceFilter = 50f;
            bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
            bullseyeSearch.teamMaskFilter.RemoveTeam(inflictorBody.teamComponent.teamIndex);
            bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
            bullseyeSearch.filterByLoS = false;
            bullseyeSearch.RefreshCandidates();
            if ((bool)ownerRoot)
            {
                bullseyeSearch.FilterOutGameObject(ownerRoot);
            }
            if ((bool)nextRoot)
            {
                bullseyeSearch.FilterOutGameObject(nextRoot);
            }
            if ((bool)previousRoot)
            {
                bullseyeSearch.FilterOutGameObject(previousRoot);
            }
            foreach (HurtBox result in bullseyeSearch.GetResults())
            {
                if ((bool)result.healthComponent && (bool)result.healthComponent.body)
                {
                    if (result.healthComponent.body.HasBuff(Buffs.ribbonDebuff))
                    {
                        bullseyeSearch.FilterOutGameObject(result.healthComponent.gameObject);
                    }
                    if ((bool)FindRibbonController(result.healthComponent.gameObject))
                    {
                        bullseyeSearch.FilterOutGameObject(result.healthComponent.gameObject);
                    }
                }
            }
            HurtBox hurtBox = bullseyeSearch.GetResults().FirstOrDefault();
            if ((bool)hurtBox && (bool)hurtBox.healthComponent && (bool)hurtBox.healthComponent.body && hurtBox.healthComponent.alive)
            {
                ribbonExtended = true;
                NetworknextRoot = hurtBox.healthComponent.gameObject;
                nextHealthComponent = hurtBox.healthComponent;
            }
            else
            {
                NetworknextRoot = null;
                spreadsRemaining = 0;
            }
        }

        public void SearchNewOwner()
        {
            if (!inflictorBody)
            {
                inflictorBody = inflictorRoot.GetComponent<CharacterBody>();
            }
            BullseyeSearch bullseyeSearch = new BullseyeSearch();
            bullseyeSearch.searchOrigin = base.transform.position;
            bullseyeSearch.maxDistanceFilter = 50f;
            bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
            bullseyeSearch.teamMaskFilter.RemoveTeam(inflictorBody.teamComponent.teamIndex);
            bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
            bullseyeSearch.filterByLoS = false;
            bullseyeSearch.RefreshCandidates();
            if ((bool)nextRoot)
            {
                bullseyeSearch.FilterOutGameObject(nextRoot);
            }
            if ((bool)previousRoot)
            {
                bullseyeSearch.FilterOutGameObject(previousRoot);
            }
            foreach (HurtBox result in bullseyeSearch.GetResults())
            {
                if ((bool)result.healthComponent && (bool)result.healthComponent.body)
                {
                    if (result.healthComponent.body.HasBuff(Buffs.ribbonDebuff))
                    {
                        bullseyeSearch.FilterOutGameObject(result.healthComponent.gameObject);
                    }
                    if ((bool)FindRibbonController(result.healthComponent.gameObject))
                    {
                        bullseyeSearch.FilterOutGameObject(result.healthComponent.gameObject);
                    }
                }
            }
            HurtBox hurtBox = bullseyeSearch.GetResults().FirstOrDefault();
            if ((bool)hurtBox && (bool)hurtBox.healthComponent && (bool)hurtBox.healthComponent.body && hurtBox.healthComponent.alive)
            {
                NetworkownerRoot = hurtBox.healthComponent.gameObject;
                ownerBody = hurtBox.healthComponent.body;
            }
            else
            {
                NetworkownerRoot = null;
            }
        }

        public static RibbonController FindRibbonController(GameObject victimObject)
        {
            int i = 0;
            for (int count = instancesList.Count; i < count; i++)
            {
                if (victimObject == instancesList[i].ownerRoot)
                {
                    return instancesList[i];
                }
            }
            return null;
        }

        private void UNetVersion()
        {
        }

        public override bool OnSerialize(NetworkWriter writer, bool forceAll)
        {
            if (forceAll)
            {
                writer.Write(nextRoot);
                writer.Write(ownerRoot);
                writer.Write(previousRoot);
                return true;
            }
            bool flag = false;
            if ((base.syncVarDirtyBits & (true ? 1u : 0u)) != 0)
            {
                if (!flag)
                {
                    writer.WritePackedUInt32(base.syncVarDirtyBits);
                    flag = true;
                }
                writer.Write(nextRoot);
            }
            if ((base.syncVarDirtyBits & 2u) != 0)
            {
                if (!flag)
                {
                    writer.WritePackedUInt32(base.syncVarDirtyBits);
                    flag = true;
                }
                writer.Write(ownerRoot);
            }
            if ((base.syncVarDirtyBits & 3u) != 0)
            {
                if (!flag)
                {
                    writer.WritePackedUInt32(base.syncVarDirtyBits);
                    flag = true;
                }
                writer.Write(previousRoot);
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
                ___nextRootNetId = reader.ReadNetworkId();
                ___ownerRootNetId = reader.ReadNetworkId();
                ___previousRootNetId = reader.ReadNetworkId();
                return;
            }
            int num = (int)reader.ReadPackedUInt32();
            if (((uint)num & (true ? 1u : 0u)) != 0)
            {
                nextRoot = reader.ReadGameObject();
            }
            if (((uint)num & 2u) != 0)
            {
                ownerRoot = reader.ReadGameObject();
            }
            if (((uint)num & 3u) != 0)
            {
                previousRoot = reader.ReadGameObject();
            }
        }

        public override void PreStartClient()
        {
            if (!___nextRootNetId.IsEmpty())
            {
                NetworknextRoot = ClientScene.FindLocalObject(___nextRootNetId);
            }
            if (!___ownerRootNetId.IsEmpty())
            {
                NetworkownerRoot = ClientScene.FindLocalObject(___ownerRootNetId);
            }
            if (!___previousRootNetId.IsEmpty())
            {
                NetworkpreviousRoot = ClientScene.FindLocalObject(___previousRootNetId);
            }
        }
    }
}
