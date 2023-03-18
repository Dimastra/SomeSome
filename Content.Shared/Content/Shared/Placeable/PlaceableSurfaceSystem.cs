using System;
using System.Runtime.CompilerServices;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Storage.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Shared.Placeable
{
	// Token: 0x02000272 RID: 626
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PlaceableSurfaceSystem : EntitySystem
	{
		// Token: 0x06000727 RID: 1831 RVA: 0x00018720 File Offset: 0x00016920
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PlaceableSurfaceComponent, AfterInteractUsingEvent>(new ComponentEventHandler<PlaceableSurfaceComponent, AfterInteractUsingEvent>(this.OnAfterInteractUsing), null, null);
			base.SubscribeLocalEvent<PlaceableSurfaceComponent, ComponentGetState>(new ComponentEventRefHandler<PlaceableSurfaceComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<PlaceableSurfaceComponent, ComponentHandleState>(new ComponentEventRefHandler<PlaceableSurfaceComponent, ComponentHandleState>(this.OnHandleState), null, null);
		}

		// Token: 0x06000728 RID: 1832 RVA: 0x0001876F File Offset: 0x0001696F
		private void OnGetState(EntityUid uid, PlaceableSurfaceComponent component, ref ComponentGetState args)
		{
			args.State = new PlaceableSurfaceComponentState(component.IsPlaceable, component.PlaceCentered, component.PositionOffset);
		}

		// Token: 0x06000729 RID: 1833 RVA: 0x0001878E File Offset: 0x0001698E
		[NullableContext(2)]
		public void SetPlaceable(EntityUid uid, bool isPlaceable, PlaceableSurfaceComponent surface = null)
		{
			if (!base.Resolve<PlaceableSurfaceComponent>(uid, ref surface, true))
			{
				return;
			}
			surface.IsPlaceable = isPlaceable;
			base.Dirty(surface, null);
		}

		// Token: 0x0600072A RID: 1834 RVA: 0x000187AC File Offset: 0x000169AC
		[NullableContext(2)]
		public void SetPlaceCentered(EntityUid uid, bool placeCentered, PlaceableSurfaceComponent surface = null)
		{
			if (!base.Resolve<PlaceableSurfaceComponent>(uid, ref surface, true))
			{
				return;
			}
			surface.PlaceCentered = placeCentered;
			base.Dirty(surface, null);
		}

		// Token: 0x0600072B RID: 1835 RVA: 0x000187CA File Offset: 0x000169CA
		[NullableContext(2)]
		public void SetPositionOffset(EntityUid uid, Vector2 offset, PlaceableSurfaceComponent surface = null)
		{
			if (!base.Resolve<PlaceableSurfaceComponent>(uid, ref surface, true))
			{
				return;
			}
			surface.PositionOffset = offset;
			base.Dirty(surface, null);
		}

		// Token: 0x0600072C RID: 1836 RVA: 0x000187E8 File Offset: 0x000169E8
		private void OnAfterInteractUsing(EntityUid uid, PlaceableSurfaceComponent surface, AfterInteractUsingEvent args)
		{
			if (args.Handled || !args.CanReach)
			{
				return;
			}
			if (!surface.IsPlaceable)
			{
				return;
			}
			if (base.HasComp<DumpableComponent>(args.Used))
			{
				return;
			}
			if (!this._handsSystem.TryDrop(args.User, args.Used, null, true, true, null))
			{
				return;
			}
			if (surface.PlaceCentered)
			{
				base.Transform(args.Used).LocalPosition = base.Transform(uid).LocalPosition + surface.PositionOffset;
			}
			else
			{
				base.Transform(args.Used).Coordinates = args.ClickLocation;
			}
			args.Handled = true;
		}

		// Token: 0x0600072D RID: 1837 RVA: 0x00018894 File Offset: 0x00016A94
		private void OnHandleState(EntityUid uid, PlaceableSurfaceComponent component, ref ComponentHandleState args)
		{
			PlaceableSurfaceComponentState state = args.Current as PlaceableSurfaceComponentState;
			if (state == null)
			{
				return;
			}
			component.IsPlaceable = state.IsPlaceable;
			component.PlaceCentered = state.PlaceCentered;
			component.PositionOffset = state.PositionOffset;
		}

		// Token: 0x0400070A RID: 1802
		[Dependency]
		private readonly SharedHandsSystem _handsSystem;
	}
}
