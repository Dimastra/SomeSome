using System;
using System.Runtime.CompilerServices;
using Content.Server.Cuffs.Components;
using Content.Server.Mind;
using Content.Server.Objectives.Interfaces;
using Content.Server.Station.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Server.Objectives.Conditions
{
	// Token: 0x020002FD RID: 765
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class EscapeShuttleCondition : IObjectiveCondition, IEquatable<IObjectiveCondition>
	{
		// Token: 0x06000FAD RID: 4013 RVA: 0x00050357 File Offset: 0x0004E557
		public IObjectiveCondition GetAssigned(Mind mind)
		{
			return new EscapeShuttleCondition
			{
				_mind = mind
			};
		}

		// Token: 0x1700022B RID: 555
		// (get) Token: 0x06000FAE RID: 4014 RVA: 0x00050365 File Offset: 0x0004E565
		public string Title
		{
			get
			{
				return Loc.GetString("objective-condition-escape-shuttle-title");
			}
		}

		// Token: 0x1700022C RID: 556
		// (get) Token: 0x06000FAF RID: 4015 RVA: 0x00050371 File Offset: 0x0004E571
		public string Description
		{
			get
			{
				return Loc.GetString("objective-condition-escape-shuttle-description");
			}
		}

		// Token: 0x1700022D RID: 557
		// (get) Token: 0x06000FB0 RID: 4016 RVA: 0x0005037D File Offset: 0x0004E57D
		public SpriteSpecifier Icon
		{
			get
			{
				return new SpriteSpecifier.Rsi(new ResourcePath("Structures/Furniture/chairs.rsi", "/"), "shuttle");
			}
		}

		// Token: 0x06000FB1 RID: 4017 RVA: 0x00050398 File Offset: 0x0004E598
		private bool IsAgentOnShuttle(TransformComponent agentXform, EntityUid? shuttle)
		{
			if (shuttle == null)
			{
				return false;
			}
			IEntityManager entMan = IoCManager.Resolve<IEntityManager>();
			MapGridComponent shuttleGrid;
			TransformComponent shuttleXform;
			if (!entMan.TryGetComponent<MapGridComponent>(shuttle, ref shuttleGrid) || !entMan.TryGetComponent<TransformComponent>(shuttle, ref shuttleXform))
			{
				return false;
			}
			Matrix3 worldMatrix = shuttleXform.WorldMatrix;
			Box2 localAABB = shuttleGrid.LocalAABB;
			return worldMatrix.TransformBox(ref localAABB).Contains(agentXform.WorldPosition, true);
		}

		// Token: 0x1700022E RID: 558
		// (get) Token: 0x06000FB2 RID: 4018 RVA: 0x000503F8 File Offset: 0x0004E5F8
		public float Progress
		{
			get
			{
				IEntityManager entMan = IoCManager.Resolve<IEntityManager>();
				Mind mind = this._mind;
				TransformComponent xform;
				if (mind == null || mind.OwnedEntity == null || !entMan.TryGetComponent<TransformComponent>(this._mind.OwnedEntity, ref xform))
				{
					return 0f;
				}
				bool shuttleContainsAgent = false;
				bool agentIsAlive = !this._mind.CharacterDeadIC;
				bool agentIsEscaping = true;
				CuffableComponent cuffed;
				if (entMan.TryGetComponent<CuffableComponent>(this._mind.OwnedEntity, ref cuffed) && cuffed.CuffedHandCount > 0)
				{
					agentIsEscaping = false;
				}
				foreach (StationDataComponent stationData in entMan.EntityQuery<StationDataComponent>(false))
				{
					if (this.IsAgentOnShuttle(xform, stationData.EmergencyShuttle))
					{
						shuttleContainsAgent = true;
						break;
					}
				}
				if (!shuttleContainsAgent || !agentIsAlive || !agentIsEscaping)
				{
					return 0f;
				}
				return 1f;
			}
		}

		// Token: 0x1700022F RID: 559
		// (get) Token: 0x06000FB3 RID: 4019 RVA: 0x000504E4 File Offset: 0x0004E6E4
		public float Difficulty
		{
			get
			{
				return 1.3f;
			}
		}

		// Token: 0x06000FB4 RID: 4020 RVA: 0x000504EC File Offset: 0x0004E6EC
		[NullableContext(2)]
		public bool Equals(IObjectiveCondition other)
		{
			EscapeShuttleCondition esc = other as EscapeShuttleCondition;
			return esc != null && object.Equals(this._mind, esc._mind);
		}

		// Token: 0x06000FB5 RID: 4021 RVA: 0x00050516 File Offset: 0x0004E716
		[NullableContext(2)]
		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((EscapeShuttleCondition)obj)));
		}

		// Token: 0x06000FB6 RID: 4022 RVA: 0x00050544 File Offset: 0x0004E744
		public override int GetHashCode()
		{
			if (this._mind == null)
			{
				return 0;
			}
			return this._mind.GetHashCode();
		}

		// Token: 0x04000927 RID: 2343
		[Nullable(2)]
		private Mind _mind;
	}
}
