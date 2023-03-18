using System;
using System.Runtime.CompilerServices;
using Content.Server.Construction.Components;
using Content.Shared.Construction;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Construction.Completions
{
	// Token: 0x0200061B RID: 1563
	[DataDefinition]
	public sealed class MachineFrameRegenerateProgress : IGraphAction
	{
		// Token: 0x0600215A RID: 8538 RVA: 0x000AE7F8 File Offset: 0x000AC9F8
		[NullableContext(1)]
		public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
		{
			MachineFrameComponent machineFrame;
			if (entityManager.TryGetComponent<MachineFrameComponent>(uid, ref machineFrame))
			{
				entityManager.EntitySysManager.GetEntitySystem<MachineFrameSystem>().RegenerateProgress(machineFrame);
			}
		}
	}
}
