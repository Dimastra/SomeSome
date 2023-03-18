using System;
using System.Runtime.CompilerServices;
using Content.Shared.Construction.Components;
using Content.Shared.SubFloor;
using Robust.Shared.GameObjects;
using Robust.Shared.Map.Components;

namespace Content.Server.SubFloor
{
	// Token: 0x0200014A RID: 330
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SubFloorHideSystem : SharedSubFloorHideSystem
	{
		// Token: 0x06000642 RID: 1602 RVA: 0x0001E38E File Offset: 0x0001C58E
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SubFloorHideComponent, AnchorAttemptEvent>(new ComponentEventHandler<SubFloorHideComponent, AnchorAttemptEvent>(this.OnAnchorAttempt), null, null);
			base.SubscribeLocalEvent<SubFloorHideComponent, UnanchorAttemptEvent>(new ComponentEventHandler<SubFloorHideComponent, UnanchorAttemptEvent>(this.OnUnanchorAttempt), null, null);
		}

		// Token: 0x06000643 RID: 1603 RVA: 0x0001E3C0 File Offset: 0x0001C5C0
		private void OnAnchorAttempt(EntityUid uid, SubFloorHideComponent component, AnchorAttemptEvent args)
		{
			TransformComponent xform = base.Transform(uid);
			MapGridComponent grid;
			if (this.MapManager.TryGetGrid(xform.GridUid, ref grid) && base.HasFloorCover(grid, grid.TileIndicesFor(xform.Coordinates)))
			{
				args.Cancel();
			}
		}

		// Token: 0x06000644 RID: 1604 RVA: 0x0001E405 File Offset: 0x0001C605
		private void OnUnanchorAttempt(EntityUid uid, SubFloorHideComponent component, UnanchorAttemptEvent args)
		{
			if (component.IsUnderCover)
			{
				args.Cancel();
			}
		}
	}
}
