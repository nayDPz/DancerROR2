using BepInEx;
using R2API.Utils;
using RoR2;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.Networking;
using RoR2.Projectile;
using RoR2.Orbs;
using Dancer.Modules.Components;
using System.Collections.Generic;
using System.Linq;
using Dancer.SkillStates;
using EntityStates;
using System;
using R2API.Networking.Interfaces;
using R2API.Networking;
using RoR2.CharacterAI;
using MonoMod.RuntimeDetour;

namespace Dancer.Modules.Components
{
    public class LockedMageTracker // copied playercount hooks from multitudes, arti AI from fetchafriend
    {
        public CharacterBody mageOwnerBody;
        public CharacterMaster mageOwnerMaster;
        public CharacterMaster mageBuddy;
        public static LockedMageTracker instance;
        public static bool mageFreed;

        private delegate int RunInstanceReturnInt(Run self);

        private static RunInstanceReturnInt origLivingPlayerCountGetter;
        private static RunInstanceReturnInt origParticipatingPlayerCountGetter;
        public LockedMageTracker()
        {
            LockedMageTracker.instance = this;
            RoR2.Run.onRunStartGlobal += Run_onRunStartGlobal;
            EntityStates.LockedMage.UnlockingMage.onOpened += SpawnArtificerBuddy;
            RoR2.Stage.onStageStartGlobal += Stage_onStageStartGlobal;
            On.RoR2.GenericPickupController.GrantItem += GenericPickupController_GrantItem;

            var getLivingPlayerCountHook = new Hook(typeof(Run).GetMethodCached("get_livingPlayerCount"),
                typeof(LockedMageTracker).GetMethodCached(nameof(GetLivingPlayerCountHook)));
            origLivingPlayerCountGetter = getLivingPlayerCountHook.GenerateTrampoline<RunInstanceReturnInt>();

            var getParticipatingPlayerCount = new Hook(typeof(Run).GetMethodCached("get_participatingPlayerCount"),
                typeof(LockedMageTracker).GetMethodCached(nameof(GetParticipatingPlayerCountHook)));
            origParticipatingPlayerCountGetter = getParticipatingPlayerCount.GenerateTrampoline<RunInstanceReturnInt>();
        }

        private void GenericPickupController_GrantItem(On.RoR2.GenericPickupController.orig_GrantItem orig, GenericPickupController self, CharacterBody body, Inventory inventory)
        {
            orig(self, body, inventory);
            if (mageFreed && this.mageBuddy && this.mageOwnerBody)
            {
                if (this.mageOwnerBody == body)
                {
                    Debug.Log("giving " + ItemCatalog.GetItemDef(PickupCatalog.GetPickupDef(self.pickupIndex).itemIndex).name + " to mage buddy :D");
                    this.mageBuddy.inventory.GiveItem(ItemCatalog.GetItemDef(PickupCatalog.GetPickupDef(self.pickupIndex).itemIndex));
                }
            }
        }


        private static int GetLivingPlayerCountHook(Run self) => origLivingPlayerCountGetter(self) + (mageFreed ? 1 : 0);
        private static int GetParticipatingPlayerCountHook(Run self) => origParticipatingPlayerCountGetter(self) + (mageFreed ? 1 : 0);

        private void SpawnArtificerBuddy(Interactor interactor)
        {
            mageFreed = true;
            this.mageOwnerBody = interactor.GetComponent<CharacterBody>();
            this.mageOwnerMaster = this.mageOwnerBody.master;

            this.mageBuddy = new MasterSummon
            {

                masterPrefab = MasterCatalog.FindMasterPrefab("MageMonsterMaster"),
                summonerBodyObject = this.mageOwnerBody.gameObject,
                ignoreTeamMemberLimit = true,
                inventoryToCopy = this.mageOwnerBody.inventory,
                useAmbientLevel = new bool?(true),
                position = interactor.transform.position + Vector3.up,
                rotation = Quaternion.identity,
                preSpawnSetupCallback = (master) => {
                    List<AISkillDriver> ai = master.aiComponents[0].skillDrivers.ToList();
                    master.gameObject.AddComponent<AIOwnership>().ownerMaster = this.mageOwnerBody.master;
                    ai.AddRange(Resources.Load<GameObject>("prefabs/charactermasters/engiwalkerturretmaster").GetComponents<AISkillDriver>().Where(ai2 => ai2.moveTargetType == AISkillDriver.TargetType.CurrentLeader).ToList());
                    master.aiComponents[0].skillDrivers = ai.ToArray();
                    UnityEngine.Object.DontDestroyOnLoad(master);
                    master.destroyOnBodyDeath = false;
                    master.killedByUnsafeArea = false;
                }
            }.Perform();
        }


        private void Stage_onStageStartGlobal(Stage stage)
        {
            RoR2.SceneDef sceneDef = stage.sceneDef;
            if (this.mageOwnerMaster)
                this.mageOwnerBody = this.mageOwnerMaster.GetBody();

            if(mageFreed && this.mageBuddy && !stage.sceneDef.suppressPlayerEntry && stage.sceneDef.suppressNpcEntry)
            {
                this.mageBuddy.Respawn(stage.GetPlayerSpawnTransform().position, Quaternion.identity);
            }
            if (sceneDef.baseSceneName == "bazaar" && !mageFreed)
            {
                GameObject mage = GameObject.Find("HOLDER: Store/HOLDER: Store Platforms/LockedMage/");
                GameObject.Destroy(mage.GetComponent<GameObjectUnlockableFilter>());
                mage.SetActive(true);

            }
        }

        private void Run_onRunStartGlobal(RoR2.Run obj)
        {
            mageFreed = false;
        }
    }
}
