using System;
using System.Runtime.CompilerServices;
using Content.Server.Disposal.Unit.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Server.Disposal.Tube.Components
{
	// Token: 0x0200055E RID: 1374
	[NullableContext(1)]
	public interface IDisposalTubeComponent : IComponent
	{
		// Token: 0x1700045C RID: 1116
		// (get) Token: 0x06001D0F RID: 7439
		Container Contents { get; }

		// Token: 0x06001D10 RID: 7440
		Direction NextDirection(DisposalHolderComponent holder);

		// Token: 0x06001D11 RID: 7441
		bool CanConnect(Direction direction, IDisposalTubeComponent with);

		// Token: 0x06001D12 RID: 7442
		void PopupDirections(EntityUid entity);
	}
}
