using EntityStates.LockedMage;
using MonoMod.RuntimeDetour;
using R2API.Utils;
using RoR2;
using RoR2.CharacterAI;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Dancer.Modules.Components
{

    public class LockedMageTracker
    {
        private delegate int RunInstanceReturnInt(Run self);

        public CharacterBody mageOwnerBody;

        public CharacterMaster mageOwnerMaster;

        public CharacterMaster mageBuddy;

        public static LockedMageTracker instance;

        public static bool mageFreed;

        private static RunInstanceReturnInt origLivingPlayerCountGetter;

        private static RunInstanceReturnInt origParticipatingPlayerCountGetter;

        public LockedMageTracker()
        {
            instance = this;
            Run.onRunStartGlobal += Run_onRunStartGlobal;
            UnlockingMage.onOpened += SpawnArtificerBuddy;
            Stage.onStageStartGlobal += Stage_onStageStartGlobal;
            Hook val = new Hook((MethodBase)Reflection.GetMethodCached(typeof(Run), "get_livingPlayerCount"), Reflection.GetMethodCached(typeof(LockedMageTracker), "GetLivingPlayerCountHook"));
            origLivingPlayerCountGetter = val.GenerateTrampoline<RunInstanceReturnInt>();
            Hook val2 = new Hook((MethodBase)Reflection.GetMethodCached(typeof(Run), "get_participatingPlayerCount"), Reflection.GetMethodCached(typeof(LockedMageTracker), "GetParticipatingPlayerCountHook"));
            origParticipatingPlayerCountGetter = val2.GenerateTrampoline<RunInstanceReturnInt>();
        }

        private static int GetLivingPlayerCountHook(Run self)
        {
            return origLivingPlayerCountGetter(self);
        }

        private static int GetParticipatingPlayerCountHook(Run self)
        {
            return origParticipatingPlayerCountGetter(self) + (mageFreed ? 1 : 0);
        }

        private void SpawnArtificerBuddy(Interactor interactor)
        {
            mageFreed = true;
            mageOwnerBody = interactor.GetComponent<CharacterBody>();
            mageOwnerMaster = mageOwnerBody.master;
            mageBuddy = new MasterSummon
            {
                masterPrefab = MasterCatalog.FindMasterPrefab("MageMonsterMaster"),
                summonerBodyObject = mageOwnerBody.gameObject,
                ignoreTeamMemberLimit = true,
                inventoryToCopy = mageOwnerBody.inventory,
                useAmbientLevel = true,
                position = interactor.transform.position + Vector3.up,
                rotation = Quaternion.identity,
                preSpawnSetupCallback = delegate (CharacterMaster master)
                {
                    List<AISkillDriver> list = master.aiComponents[0].skillDrivers.ToList();
                    master.gameObject.AddComponent<AIOwnership>().ownerMaster = mageOwnerBody.master;
                    list.AddRange((from ai2 in Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiWalkerTurretMaster.prefab").WaitForCompletion().GetComponents<AISkillDriver>()
                                   where ai2.moveTargetType == AISkillDriver.TargetType.CurrentLeader
                                   select ai2).ToList());
                    master.aiComponents[0].skillDrivers = list.ToArray();
                    Object.DontDestroyOnLoad(master);
                    master.destroyOnBodyDeath = false;
                    master.killedByUnsafeArea = false;
                }
            }.Perform();
        }

        private void Stage_onStageStartGlobal(Stage stage)
        {
            SceneDef sceneDef = stage.sceneDef;
            if ((bool)mageOwnerMaster)
            {
                mageOwnerBody = mageOwnerMaster.GetBody();
            }
            if (mageFreed && (bool)mageBuddy && !stage.sceneDef.suppressPlayerEntry && stage.sceneDef.suppressNpcEntry)
            {
                mageBuddy.Respawn(stage.GetPlayerSpawnTransform().position, Quaternion.identity);
            }
            if (sceneDef.baseSceneName == "bazaar" && !mageFreed)
            {
                GameObject gameObject = GameObject.Find("HOLDER: Store/HOLDER: Store Platforms/LockedMage/");
                Object.Destroy(gameObject.GetComponent<GameObjectUnlockableFilter>());
                gameObject.SetActive(value: true);
            }
        }

        private void Run_onRunStartGlobal(Run obj)
        {
            mageFreed = false;
        }
    }
}
