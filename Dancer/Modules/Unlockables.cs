//using System;
//using System.Collections.Generic;
//using System.Reflection;
//using Dancer.Modules.Achievements;
//using Mono.Cecil.Cil;
//using MonoMod.Cil;
//using RoR2;
//using RoR2.Achievements;
//using UnityEngine;

//namespace Dancer.Modules
//{

// From what I understand it does nothing, none of the unlockabledefs are actually assigned to anything

//	internal static class Unlockables
//	{
//		internal struct UnlockableInfo
//		{
//			internal string Name;

//			internal Func<string> HowToUnlockString;

//			internal Func<string> UnlockedString;

//			internal int SortScore;
//		}

//		private static readonly HashSet<string> usedRewardIds = new HashSet<string>();

//		internal static List<AchievementDef> achievementDefs = new List<AchievementDef>();

//		internal static List<UnlockableDef> unlockableDefs = new List<UnlockableDef>();

//		private static readonly List<(AchievementDef achDef, UnlockableDef unlockableDef, string unlockableName)> moddedUnlocks = new List<(AchievementDef, UnlockableDef, string)>();

//		private static bool addingUnlockables;

//		internal static UnlockableDef dancerMasterySkinDef;

//		internal static UnlockableDef dancerRescueMageDef;

//		public static bool ableToAdd { get; private set; } = false;


//		public static void RegisterUnlockables()
//		{
//			dancerRescueMageDef = AddUnlockable<DancerRescueMageAchievement>(serverTracked: true);
//		}

//		internal static UnlockableDef CreateNewUnlockable(UnlockableInfo unlockableInfo)
//		{
//			UnlockableDef unlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
//			unlockableDef.nameToken = unlockableInfo.Name;
//			unlockableDef.cachedName = unlockableInfo.Name;
//			unlockableDef.getHowToUnlockString = unlockableInfo.HowToUnlockString;
//			unlockableDef.getUnlockedString = unlockableInfo.UnlockedString;
//			unlockableDef.sortScore = unlockableInfo.SortScore;
//			return unlockableDef;
//		}

//		public static UnlockableDef AddUnlockable<TUnlockable>(bool serverTracked) where TUnlockable : BaseAchievement, IModdedUnlockableDataProvider, new()
//		{
//			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
//			//IL_017e: Expected O, but got Unknown
//			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
//			//IL_0190: Expected O, but got Unknown
//			TUnlockable val = new TUnlockable();
//			string unlockableIdentifier = val.UnlockableIdentifier;
//			if (!usedRewardIds.Add(unlockableIdentifier))
//			{
//				throw new InvalidOperationException("The unlockable identifier '" + unlockableIdentifier + "' is already used by another mod or the base game.");
//			}
//			AchievementDef achievementDef = new AchievementDef
//			{
//				identifier = val.AchievementIdentifier,
//				unlockableRewardIdentifier = val.UnlockableIdentifier,
//				prerequisiteAchievementIdentifier = val.PrerequisiteUnlockableIdentifier,
//				nameToken = val.AchievementNameToken,
//				descriptionToken = val.AchievementDescToken,
//				achievedIcon = val.Sprite,
//				type = val.GetType(),
//				serverTrackerType = (serverTracked ? val.GetType() : null)
//			};
//			UnlockableInfo unlockableInfo = default(UnlockableInfo);
//			unlockableInfo.Name = val.UnlockableIdentifier;
//			unlockableInfo.HowToUnlockString = val.GetHowToUnlock;
//			unlockableInfo.UnlockedString = val.GetUnlocked;
//			unlockableInfo.SortScore = 200;
//			UnlockableDef unlockableDef = CreateNewUnlockable(unlockableInfo);
//			unlockableDefs.Add(unlockableDef);
//			achievementDefs.Add(achievementDef);
//			moddedUnlocks.Add((achievementDef, unlockableDef, val.UnlockableIdentifier));
//			if (!addingUnlockables)
//			{
//				addingUnlockables = true;
//				AchievementManager.add_CollectAchievementDefs(new Manipulator(CollectAchievementDefs));
//				UnlockableCatalog.add_Init(new Manipulator(Init_Il));
//			}
//			return unlockableDef;
//		}

//		public static ILCursor CallDel_<TDelegate>(this ILCursor cursor, TDelegate target, out int index) where TDelegate : Delegate
//		{
//			index = cursor.EmitDelegate<TDelegate>(target);
//			return cursor;
//		}

//		public static ILCursor CallDel_<TDelegate>(this ILCursor cursor, TDelegate target) where TDelegate : Delegate
//		{
//			int index;
//			return cursor.CallDel_(target, out index);
//		}

//		private static void Init_Il(ILContext il)
//		{
//			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
//			new ILCursor(il).GotoNext((MoveType)1, new Func<Instruction, bool>[1]
//			{
//			(Instruction x) => ILPatternMatchingExt.MatchCallOrCallvirt(x, typeof(UnlockableCatalog), "SetUnlockableDefs")
//			}).CallDel_(ArrayHelper.AppendDel(unlockableDefs));
//		}

//		private static void CollectAchievementDefs(ILContext il)
//		{
//			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
//			//IL_0032: Expected O, but got Unknown
//			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
//			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
//			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
//			FieldInfo field = typeof(AchievementManager).GetField("achievementIdentifiers", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
//			if ((object)field == null)
//			{
//				throw new NullReferenceException("Could not find field in AchievementManager");
//			}
//			ILCursor val = new ILCursor(il);
//			val.GotoNext((MoveType)2, new Func<Instruction, bool>[2]
//			{
//			(Instruction x) => ILPatternMatchingExt.MatchEndfinally(x),
//			(Instruction x) => ILPatternMatchingExt.MatchLdloc(x, 1)
//			});
//			val.Emit(OpCodes.Ldarg_0);
//			val.Emit(OpCodes.Ldsfld, field);
//			val.EmitDelegate<Action<List<AchievementDef>, Dictionary<string, AchievementDef>, List<string>>>((Action<List<AchievementDef>, Dictionary<string, AchievementDef>, List<string>>)EmittedDelegate);
//			val.Emit(OpCodes.Ldloc_1);
//			static void EmittedDelegate(List<AchievementDef> list, Dictionary<string, AchievementDef> map, List<string> identifiers)
//			{
//				ableToAdd = false;
//				for (int i = 0; i < moddedUnlocks.Count; i++)
//				{
//					var (achievementDef, unlockableDef, text) = moddedUnlocks[i];
//					if (achievementDef != null)
//					{
//						identifiers.Add(achievementDef.identifier);
//						list.Add(achievementDef);
//						map.Add(achievementDef.identifier, achievementDef);
//					}
//				}
//			}
//		}
//	}
//}
