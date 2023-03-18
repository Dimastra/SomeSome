using System;
using System.Runtime.CompilerServices;
using Content.Server.Construction.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Destructible.Thresholds.Behaviors
{
	// Token: 0x020005A2 RID: 1442
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	[Serializable]
	public sealed class ChangeConstructionNodeBehavior : IThresholdBehavior
	{
		// Token: 0x1700046F RID: 1135
		// (get) Token: 0x06001E03 RID: 7683 RVA: 0x0009EFC7 File Offset: 0x0009D1C7
		// (set) Token: 0x06001E04 RID: 7684 RVA: 0x0009EFCF File Offset: 0x0009D1CF
		[DataField("node", false, 1, false, false, null)]
		public string Node { get; private set; } = string.Empty;

		// Token: 0x06001E05 RID: 7685 RVA: 0x0009EFD8 File Offset: 0x0009D1D8
		public void Execute(EntityUid owner, DestructibleSystem system, EntityUid? cause = null)
		{
			ConstructionComponent construction;
			if (string.IsNullOrEmpty(this.Node) || !system.EntityManager.TryGetComponent<ConstructionComponent>(owner, ref construction))
			{
				return;
			}
			system.ConstructionSystem.ChangeNode(owner, null, this.Node, true, construction);
		}
	}
}
