using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Interaction;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Construction
{
	// Token: 0x0200056E RID: 1390
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedConstructionSystem : EntitySystem
	{
		// Token: 0x060010F7 RID: 4343 RVA: 0x00037FF8 File Offset: 0x000361F8
		[NullableContext(2)]
		public SharedInteractionSystem.Ignored GetPredicate(bool canBuildInImpassable, MapCoordinates coords)
		{
			if (!canBuildInImpassable)
			{
				return null;
			}
			MapGridComponent grid;
			if (!this._mapManager.TryFindGridAt(coords, ref grid))
			{
				return null;
			}
			HashSet<EntityUid> ignored = grid.GetAnchoredEntities(coords).ToHashSet<EntityUid>();
			return (EntityUid e) => ignored.Contains(e);
		}

		// Token: 0x04000FD8 RID: 4056
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x02000841 RID: 2113
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class TryStartStructureConstructionMessage : EntityEventArgs
		{
			// Token: 0x06001932 RID: 6450 RVA: 0x0004FA99 File Offset: 0x0004DC99
			public TryStartStructureConstructionMessage(EntityCoordinates loc, string prototypeName, Angle angle, int ack)
			{
				this.Location = loc;
				this.PrototypeName = prototypeName;
				this.Angle = angle;
				this.Ack = ack;
			}

			// Token: 0x0400194E RID: 6478
			public readonly EntityCoordinates Location;

			// Token: 0x0400194F RID: 6479
			public readonly string PrototypeName;

			// Token: 0x04001950 RID: 6480
			public readonly Angle Angle;

			// Token: 0x04001951 RID: 6481
			public readonly int Ack;
		}

		// Token: 0x02000842 RID: 2114
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class TryStartItemConstructionMessage : EntityEventArgs
		{
			// Token: 0x06001933 RID: 6451 RVA: 0x0004FABE File Offset: 0x0004DCBE
			public TryStartItemConstructionMessage(string prototypeName)
			{
				this.PrototypeName = prototypeName;
			}

			// Token: 0x04001952 RID: 6482
			public readonly string PrototypeName;
		}

		// Token: 0x02000843 RID: 2115
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		public sealed class AckStructureConstructionMessage : EntityEventArgs
		{
			// Token: 0x06001934 RID: 6452 RVA: 0x0004FACD File Offset: 0x0004DCCD
			public AckStructureConstructionMessage(int ghostId)
			{
				this.GhostId = ghostId;
			}

			// Token: 0x04001953 RID: 6483
			public readonly int GhostId;
		}

		// Token: 0x02000844 RID: 2116
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class RequestConstructionGuide : EntityEventArgs
		{
			// Token: 0x06001935 RID: 6453 RVA: 0x0004FADC File Offset: 0x0004DCDC
			public RequestConstructionGuide(string constructionId)
			{
				this.ConstructionId = constructionId;
			}

			// Token: 0x04001954 RID: 6484
			public readonly string ConstructionId;
		}

		// Token: 0x02000845 RID: 2117
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class ResponseConstructionGuide : EntityEventArgs
		{
			// Token: 0x06001936 RID: 6454 RVA: 0x0004FAEB File Offset: 0x0004DCEB
			public ResponseConstructionGuide(string constructionId, ConstructionGuide guide)
			{
				this.ConstructionId = constructionId;
				this.Guide = guide;
			}

			// Token: 0x04001955 RID: 6485
			public readonly string ConstructionId;

			// Token: 0x04001956 RID: 6486
			public readonly ConstructionGuide Guide;
		}
	}
}
