using System;
using System.Runtime.CompilerServices;
using Content.Shared.Examine;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Content.Shared.Interaction.Helpers
{
	// Token: 0x020003D0 RID: 976
	[NullableContext(1)]
	[Nullable(0)]
	public static class SharedUnoccludedExtensions
	{
		// Token: 0x06000B5D RID: 2909 RVA: 0x00025EA5 File Offset: 0x000240A5
		[NullableContext(2)]
		public static bool InRangeUnOccluded(this EntityUid origin, EntityUid other, float range = 16f, SharedInteractionSystem.Ignored predicate = null, bool ignoreInsideBlocker = true)
		{
			return ExamineSystemShared.InRangeUnOccluded(origin, other, range, predicate, ignoreInsideBlocker);
		}

		// Token: 0x06000B5E RID: 2910 RVA: 0x00025EB2 File Offset: 0x000240B2
		public static bool InRangeUnOccluded(this EntityUid origin, IComponent other, float range = 1.5f, [Nullable(2)] SharedInteractionSystem.Ignored predicate = null, bool ignoreInsideBlocker = true)
		{
			return ExamineSystemShared.InRangeUnOccluded(origin, other, range, predicate, ignoreInsideBlocker);
		}

		// Token: 0x06000B5F RID: 2911 RVA: 0x00025EC0 File Offset: 0x000240C0
		public static bool InRangeUnOccluded(this EntityUid origin, IContainer other, float range = 1.5f, [Nullable(2)] SharedInteractionSystem.Ignored predicate = null, bool ignoreInsideBlocker = true)
		{
			EntityUid otherEntity = other.Owner;
			return ExamineSystemShared.InRangeUnOccluded(origin, otherEntity, range, predicate, ignoreInsideBlocker);
		}

		// Token: 0x06000B60 RID: 2912 RVA: 0x00025EDF File Offset: 0x000240DF
		[NullableContext(2)]
		public static bool InRangeUnOccluded(this EntityUid origin, EntityCoordinates other, float range = 1.5f, SharedInteractionSystem.Ignored predicate = null, bool ignoreInsideBlocker = true)
		{
			return ExamineSystemShared.InRangeUnOccluded(origin, other, range, predicate, ignoreInsideBlocker);
		}

		// Token: 0x06000B61 RID: 2913 RVA: 0x00025EEC File Offset: 0x000240EC
		[NullableContext(2)]
		public static bool InRangeUnOccluded(this EntityUid origin, MapCoordinates other, float range = 1.5f, SharedInteractionSystem.Ignored predicate = null, bool ignoreInsideBlocker = true)
		{
			return ExamineSystemShared.InRangeUnOccluded(origin, other, range, predicate, ignoreInsideBlocker);
		}

		// Token: 0x06000B62 RID: 2914 RVA: 0x00025EF9 File Offset: 0x000240F9
		public static bool InRangeUnOccluded(this IComponent origin, EntityUid other, float range = 1.5f, [Nullable(2)] SharedInteractionSystem.Ignored predicate = null, bool ignoreInsideBlocker = true)
		{
			return ExamineSystemShared.InRangeUnOccluded(origin.Owner, other, range, predicate, ignoreInsideBlocker);
		}

		// Token: 0x06000B63 RID: 2915 RVA: 0x00025F0B File Offset: 0x0002410B
		public static bool InRangeUnOccluded(this IComponent origin, IComponent other, float range = 1.5f, [Nullable(2)] SharedInteractionSystem.Ignored predicate = null, bool ignoreInsideBlocker = true)
		{
			return ExamineSystemShared.InRangeUnOccluded(origin.Owner, other, range, predicate, ignoreInsideBlocker);
		}

		// Token: 0x06000B64 RID: 2916 RVA: 0x00025F20 File Offset: 0x00024120
		public static bool InRangeUnOccluded(this IComponent origin, IContainer other, float range = 1.5f, [Nullable(2)] SharedInteractionSystem.Ignored predicate = null, bool ignoreInsideBlocker = true)
		{
			EntityUid owner = origin.Owner;
			EntityUid otherEntity = other.Owner;
			return ExamineSystemShared.InRangeUnOccluded(owner, otherEntity, range, predicate, ignoreInsideBlocker);
		}

		// Token: 0x06000B65 RID: 2917 RVA: 0x00025F44 File Offset: 0x00024144
		public static bool InRangeUnOccluded(this IComponent origin, EntityCoordinates other, float range = 1.5f, [Nullable(2)] SharedInteractionSystem.Ignored predicate = null, bool ignoreInsideBlocker = true)
		{
			return ExamineSystemShared.InRangeUnOccluded(origin.Owner, other, range, predicate, ignoreInsideBlocker);
		}

		// Token: 0x06000B66 RID: 2918 RVA: 0x00025F56 File Offset: 0x00024156
		public static bool InRangeUnOccluded(this IComponent origin, MapCoordinates other, float range = 1.5f, [Nullable(2)] SharedInteractionSystem.Ignored predicate = null, bool ignoreInsideBlocker = true)
		{
			return ExamineSystemShared.InRangeUnOccluded(origin.Owner, other, range, predicate, ignoreInsideBlocker);
		}

		// Token: 0x06000B67 RID: 2919 RVA: 0x00025F68 File Offset: 0x00024168
		public static bool InRangeUnOccluded(this IContainer origin, EntityUid other, float range = 1.5f, [Nullable(2)] SharedInteractionSystem.Ignored predicate = null, bool ignoreInsideBlocker = true)
		{
			return ExamineSystemShared.InRangeUnOccluded(origin.Owner, other, range, predicate, ignoreInsideBlocker);
		}

		// Token: 0x06000B68 RID: 2920 RVA: 0x00025F7A File Offset: 0x0002417A
		public static bool InRangeUnOccluded(this IContainer origin, IComponent other, float range = 1.5f, [Nullable(2)] SharedInteractionSystem.Ignored predicate = null, bool ignoreInsideBlocker = true)
		{
			return ExamineSystemShared.InRangeUnOccluded(origin.Owner, other, range, predicate, ignoreInsideBlocker);
		}

		// Token: 0x06000B69 RID: 2921 RVA: 0x00025F8C File Offset: 0x0002418C
		public static bool InRangeUnOccluded(this IContainer origin, IContainer other, float range = 1.5f, [Nullable(2)] SharedInteractionSystem.Ignored predicate = null, bool ignoreInsideBlocker = true)
		{
			EntityUid owner = origin.Owner;
			EntityUid otherEntity = other.Owner;
			return ExamineSystemShared.InRangeUnOccluded(owner, otherEntity, range, predicate, ignoreInsideBlocker);
		}

		// Token: 0x06000B6A RID: 2922 RVA: 0x00025FB0 File Offset: 0x000241B0
		public static bool InRangeUnOccluded(this IContainer origin, EntityCoordinates other, float range = 1.5f, [Nullable(2)] SharedInteractionSystem.Ignored predicate = null, bool ignoreInsideBlocker = true)
		{
			return ExamineSystemShared.InRangeUnOccluded(origin.Owner, other, range, predicate, ignoreInsideBlocker);
		}

		// Token: 0x06000B6B RID: 2923 RVA: 0x00025FC2 File Offset: 0x000241C2
		public static bool InRangeUnOccluded(this IContainer origin, MapCoordinates other, float range = 1.5f, [Nullable(2)] SharedInteractionSystem.Ignored predicate = null, bool ignoreInsideBlocker = true)
		{
			return ExamineSystemShared.InRangeUnOccluded(origin.Owner, other, range, predicate, ignoreInsideBlocker);
		}

		// Token: 0x06000B6C RID: 2924 RVA: 0x00025FD4 File Offset: 0x000241D4
		[NullableContext(2)]
		public static bool InRangeUnOccluded(this EntityCoordinates origin, EntityUid other, float range = 1.5f, SharedInteractionSystem.Ignored predicate = null, bool ignoreInsideBlocker = true)
		{
			IEntityManager entMan = IoCManager.Resolve<IEntityManager>();
			MapCoordinates origin2 = origin.ToMap(entMan);
			MapCoordinates otherPosition = entMan.GetComponent<TransformComponent>(other).MapPosition;
			return ExamineSystemShared.InRangeUnOccluded(origin2, otherPosition, range, predicate, ignoreInsideBlocker, null);
		}

		// Token: 0x06000B6D RID: 2925 RVA: 0x00026008 File Offset: 0x00024208
		public static bool InRangeUnOccluded(this EntityCoordinates origin, IComponent other, float range = 1.5f, [Nullable(2)] SharedInteractionSystem.Ignored predicate = null, bool ignoreInsideBlocker = true)
		{
			IEntityManager entMan = IoCManager.Resolve<IEntityManager>();
			MapCoordinates origin2 = origin.ToMap(entMan);
			MapCoordinates otherPosition = entMan.GetComponent<TransformComponent>(other.Owner).MapPosition;
			return ExamineSystemShared.InRangeUnOccluded(origin2, otherPosition, range, predicate, ignoreInsideBlocker, null);
		}

		// Token: 0x06000B6E RID: 2926 RVA: 0x00026040 File Offset: 0x00024240
		public static bool InRangeUnOccluded(this EntityCoordinates origin, IContainer other, float range = 1.5f, [Nullable(2)] SharedInteractionSystem.Ignored predicate = null, bool ignoreInsideBlocker = true)
		{
			IEntityManager entMan = IoCManager.Resolve<IEntityManager>();
			MapCoordinates origin2 = origin.ToMap(entMan);
			MapCoordinates otherPosition = entMan.GetComponent<TransformComponent>(other.Owner).MapPosition;
			return ExamineSystemShared.InRangeUnOccluded(origin2, otherPosition, range, predicate, ignoreInsideBlocker, null);
		}

		// Token: 0x06000B6F RID: 2927 RVA: 0x00026078 File Offset: 0x00024278
		[NullableContext(2)]
		public static bool InRangeUnOccluded(this EntityCoordinates origin, EntityCoordinates other, float range = 1.5f, SharedInteractionSystem.Ignored predicate = null, bool ignoreInsideBlocker = true, IEntityManager entityManager = null)
		{
			IoCManager.Resolve<IEntityManager>(ref entityManager);
			MapCoordinates origin2 = origin.ToMap(entityManager);
			MapCoordinates otherPosition = other.ToMap(entityManager);
			return ExamineSystemShared.InRangeUnOccluded(origin2, otherPosition, range, predicate, ignoreInsideBlocker, null);
		}

		// Token: 0x06000B70 RID: 2928 RVA: 0x000260AA File Offset: 0x000242AA
		[NullableContext(2)]
		public static bool InRangeUnOccluded(this EntityCoordinates origin, MapCoordinates other, float range = 1.5f, SharedInteractionSystem.Ignored predicate = null, bool ignoreInsideBlocker = true, IEntityManager entityManager = null)
		{
			IoCManager.Resolve<IEntityManager>(ref entityManager);
			return ExamineSystemShared.InRangeUnOccluded(origin.ToMap(entityManager), other, range, predicate, ignoreInsideBlocker, null);
		}

		// Token: 0x06000B71 RID: 2929 RVA: 0x000260C8 File Offset: 0x000242C8
		[NullableContext(2)]
		public static bool InRangeUnOccluded(this MapCoordinates origin, EntityUid other, float range = 1.5f, SharedInteractionSystem.Ignored predicate = null, bool ignoreInsideBlocker = true)
		{
			MapCoordinates otherPosition = IoCManager.Resolve<IEntityManager>().GetComponent<TransformComponent>(other).MapPosition;
			return ExamineSystemShared.InRangeUnOccluded(origin, otherPosition, range, predicate, ignoreInsideBlocker, null);
		}

		// Token: 0x06000B72 RID: 2930 RVA: 0x000260F4 File Offset: 0x000242F4
		public static bool InRangeUnOccluded(this MapCoordinates origin, IComponent other, float range = 1.5f, [Nullable(2)] SharedInteractionSystem.Ignored predicate = null, bool ignoreInsideBlocker = true)
		{
			MapCoordinates otherPosition = IoCManager.Resolve<IEntityManager>().GetComponent<TransformComponent>(other.Owner).MapPosition;
			return ExamineSystemShared.InRangeUnOccluded(origin, otherPosition, range, predicate, ignoreInsideBlocker, null);
		}

		// Token: 0x06000B73 RID: 2931 RVA: 0x00026124 File Offset: 0x00024324
		public static bool InRangeUnOccluded(this MapCoordinates origin, IContainer other, float range = 1.5f, [Nullable(2)] SharedInteractionSystem.Ignored predicate = null, bool ignoreInsideBlocker = true)
		{
			MapCoordinates otherPosition = IoCManager.Resolve<IEntityManager>().GetComponent<TransformComponent>(other.Owner).MapPosition;
			return ExamineSystemShared.InRangeUnOccluded(origin, otherPosition, range, predicate, ignoreInsideBlocker, null);
		}

		// Token: 0x06000B74 RID: 2932 RVA: 0x00026154 File Offset: 0x00024354
		[NullableContext(2)]
		public static bool InRangeUnOccluded(this MapCoordinates origin, EntityCoordinates other, float range = 1.5f, SharedInteractionSystem.Ignored predicate = null, bool ignoreInsideBlocker = true, IEntityManager entityManager = null)
		{
			IoCManager.Resolve<IEntityManager>(ref entityManager);
			MapCoordinates otherPosition = other.ToMap(entityManager);
			return ExamineSystemShared.InRangeUnOccluded(origin, otherPosition, range, predicate, ignoreInsideBlocker, null);
		}

		// Token: 0x06000B75 RID: 2933 RVA: 0x0002617E File Offset: 0x0002437E
		[NullableContext(2)]
		public static bool InRangeUnOccluded(this MapCoordinates origin, MapCoordinates other, float range = 1.5f, SharedInteractionSystem.Ignored predicate = null, bool ignoreInsideBlocker = true)
		{
			return ExamineSystemShared.InRangeUnOccluded(origin, other, range, predicate, ignoreInsideBlocker, null);
		}
	}
}
