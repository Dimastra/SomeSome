using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos;
using Content.Shared.Physics;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Storage.Components
{
	// Token: 0x0200016B RID: 363
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class EntityStorageComponent : Component, IGasMixtureHolder
	{
		// Token: 0x1700013E RID: 318
		// (get) Token: 0x0600073C RID: 1852 RVA: 0x0002420C File Offset: 0x0002240C
		// (set) Token: 0x0600073D RID: 1853 RVA: 0x00024214 File Offset: 0x00022414
		[ViewVariables]
		public GasMixture Air { get; set; } = new GasMixture(70f);

		// Token: 0x0400042D RID: 1069
		public readonly float MaxSize = 1f;

		// Token: 0x0400042E RID: 1070
		public const float GasMixVolume = 70f;

		// Token: 0x0400042F RID: 1071
		public static readonly TimeSpan InternalOpenAttemptDelay = TimeSpan.FromSeconds(0.5);

		// Token: 0x04000430 RID: 1072
		public TimeSpan LastInternalOpenAttempt;

		// Token: 0x04000431 RID: 1073
		public readonly int MasksToRemove = 28;

		// Token: 0x04000432 RID: 1074
		[DataField("removedMasks", false, 1, false, false, null)]
		public int RemovedMasks;

		// Token: 0x04000433 RID: 1075
		[DataField("capacity", false, 1, false, false, null)]
		public int Capacity = 30;

		// Token: 0x04000434 RID: 1076
		[DataField("isCollidableWhenOpen", false, 1, false, false, null)]
		public bool IsCollidableWhenOpen;

		// Token: 0x04000435 RID: 1077
		[ViewVariables]
		[DataField("openOnMove", false, 1, false, false, null)]
		public bool OpenOnMove = true;

		// Token: 0x04000436 RID: 1078
		[DataField("enteringOffset", false, 1, false, false, null)]
		public Vector2 EnteringOffset = new Vector2(0f, 0f);

		// Token: 0x04000437 RID: 1079
		[DataField("enteringOffsetCollisionFlags", false, 1, false, false, null)]
		public readonly CollisionGroup EnteringOffsetCollisionFlags = CollisionGroup.TableMask;

		// Token: 0x04000438 RID: 1080
		[DataField("enteringRange", false, 1, false, false, null)]
		public float EnteringRange = 0.18f;

		// Token: 0x04000439 RID: 1081
		[DataField("showContents", false, 1, false, false, null)]
		public bool ShowContents;

		// Token: 0x0400043A RID: 1082
		[DataField("occludesLight", false, 1, false, false, null)]
		public bool OccludesLight = true;

		// Token: 0x0400043B RID: 1083
		[DataField("deleteContentsOnDestruction", false, 1, false, false, null)]
		[ViewVariables]
		public bool DeleteContentsOnDestruction;

		// Token: 0x0400043C RID: 1084
		[DataField("airtight", false, 1, false, false, null)]
		[ViewVariables]
		public bool Airtight = true;

		// Token: 0x0400043D RID: 1085
		[DataField("open", false, 1, false, false, null)]
		public bool Open;

		// Token: 0x0400043E RID: 1086
		[DataField("closeSound", false, 1, false, false, null)]
		public SoundSpecifier CloseSound = new SoundPathSpecifier("/Audio/Effects/closetclose.ogg", null);

		// Token: 0x0400043F RID: 1087
		[DataField("openSound", false, 1, false, false, null)]
		public SoundSpecifier OpenSound = new SoundPathSpecifier("/Audio/Effects/closetopen.ogg", null);

		// Token: 0x04000440 RID: 1088
		[Nullable(2)]
		[DataField("whitelist", false, 1, false, false, null)]
		public EntityWhitelist Whitelist;

		// Token: 0x04000441 RID: 1089
		[ViewVariables]
		public Container Contents;

		// Token: 0x04000442 RID: 1090
		[ViewVariables]
		public bool IsWeldedShut;
	}
}
