using System;
using System.Runtime.CompilerServices;
using Content.Shared.Body.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Destructible.Thresholds.Behaviors
{
	// Token: 0x020005A9 RID: 1449
	[DataDefinition]
	public sealed class GibBehavior : IThresholdBehavior
	{
		// Token: 0x06001E18 RID: 7704 RVA: 0x0009F3D8 File Offset: 0x0009D5D8
		[NullableContext(1)]
		public void Execute(EntityUid owner, DestructibleSystem system, EntityUid? cause = null)
		{
			BodyComponent body;
			if (system.EntityManager.TryGetComponent<BodyComponent>(owner, ref body))
			{
				system.BodySystem.GibBody(new EntityUid?(owner), this._recursive, body, false);
			}
		}

		// Token: 0x04001344 RID: 4932
		[DataField("recursive", false, 1, false, false, null)]
		private bool _recursive = true;
	}
}
