using System;
using System.Runtime.CompilerServices;
using Content.Shared.Shuttles.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.Systems
{
	// Token: 0x020001BB RID: 443
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedShuttleSystem : EntitySystem
	{
		// Token: 0x06000523 RID: 1315 RVA: 0x0001382F File Offset: 0x00011A2F
		public override void Initialize()
		{
			base.Initialize();
			this.InitializeIFF();
		}

		// Token: 0x06000524 RID: 1316 RVA: 0x0001383D File Offset: 0x00011A3D
		private void InitializeIFF()
		{
			base.SubscribeLocalEvent<IFFComponent, ComponentGetState>(new ComponentEventRefHandler<IFFComponent, ComponentGetState>(this.OnIFFGetState), null, null);
			base.SubscribeLocalEvent<IFFComponent, ComponentHandleState>(new ComponentEventRefHandler<IFFComponent, ComponentHandleState>(this.OnIFFHandleState), null, null);
		}

		// Token: 0x06000525 RID: 1317 RVA: 0x00013867 File Offset: 0x00011A67
		protected virtual void UpdateIFFInterfaces(EntityUid gridUid, IFFComponent component)
		{
		}

		// Token: 0x06000526 RID: 1318 RVA: 0x00013869 File Offset: 0x00011A69
		[NullableContext(2)]
		public void SetIFFColor(EntityUid gridUid, Color color, IFFComponent component = null)
		{
			if (component == null)
			{
				component = base.EnsureComp<IFFComponent>(gridUid);
			}
			if (component.Color.Equals(color))
			{
				return;
			}
			component.Color = color;
			base.Dirty(component, null);
			this.UpdateIFFInterfaces(gridUid, component);
		}

		// Token: 0x06000527 RID: 1319 RVA: 0x0001389D File Offset: 0x00011A9D
		[NullableContext(2)]
		public void AddIFFFlag(EntityUid gridUid, IFFFlags flags, IFFComponent component = null)
		{
			if (component == null)
			{
				component = base.EnsureComp<IFFComponent>(gridUid);
			}
			if ((component.Flags & flags) == flags)
			{
				return;
			}
			component.Flags |= flags;
			base.Dirty(component, null);
			this.UpdateIFFInterfaces(gridUid, component);
		}

		// Token: 0x06000528 RID: 1320 RVA: 0x000138D5 File Offset: 0x00011AD5
		[NullableContext(2)]
		public void RemoveIFFFlag(EntityUid gridUid, IFFFlags flags, IFFComponent component = null)
		{
			if (!base.Resolve<IFFComponent>(gridUid, ref component, false))
			{
				return;
			}
			if ((component.Flags & flags) == IFFFlags.None)
			{
				return;
			}
			component.Flags &= ~flags;
			base.Dirty(component, null);
			this.UpdateIFFInterfaces(gridUid, component);
		}

		// Token: 0x06000529 RID: 1321 RVA: 0x00013910 File Offset: 0x00011B10
		private void OnIFFHandleState(EntityUid uid, IFFComponent component, ref ComponentHandleState args)
		{
			SharedShuttleSystem.IFFComponentState state = args.Current as SharedShuttleSystem.IFFComponentState;
			if (state == null)
			{
				return;
			}
			component.Flags = state.Flags;
			component.Color = state.Color;
		}

		// Token: 0x0600052A RID: 1322 RVA: 0x00013945 File Offset: 0x00011B45
		private void OnIFFGetState(EntityUid uid, IFFComponent component, ref ComponentGetState args)
		{
			args.State = new SharedShuttleSystem.IFFComponentState
			{
				Flags = component.Flags,
				Color = component.Color
			};
		}

		// Token: 0x020007AD RID: 1965
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		private sealed class IFFComponentState : ComponentState
		{
			// Token: 0x040017D2 RID: 6098
			public IFFFlags Flags;

			// Token: 0x040017D3 RID: 6099
			public Color Color;
		}
	}
}
