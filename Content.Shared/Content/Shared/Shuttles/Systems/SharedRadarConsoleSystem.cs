using System;
using System.Runtime.CompilerServices;
using Content.Shared.Shuttles.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.Systems
{
	// Token: 0x020001B9 RID: 441
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedRadarConsoleSystem : EntitySystem
	{
		// Token: 0x06000518 RID: 1304 RVA: 0x000136F5 File Offset: 0x000118F5
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RadarConsoleComponent, ComponentGetState>(new ComponentEventRefHandler<RadarConsoleComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<RadarConsoleComponent, ComponentHandleState>(new ComponentEventRefHandler<RadarConsoleComponent, ComponentHandleState>(this.OnHandleState), null, null);
		}

		// Token: 0x06000519 RID: 1305 RVA: 0x00013728 File Offset: 0x00011928
		private void OnHandleState(EntityUid uid, RadarConsoleComponent component, ref ComponentHandleState args)
		{
			SharedRadarConsoleSystem.RadarConsoleComponentState state = args.Current as SharedRadarConsoleSystem.RadarConsoleComponentState;
			if (state == null)
			{
				return;
			}
			component.MaxRange = state.Range;
		}

		// Token: 0x0600051A RID: 1306 RVA: 0x00013751 File Offset: 0x00011951
		private void OnGetState(EntityUid uid, RadarConsoleComponent component, ref ComponentGetState args)
		{
			args.State = new SharedRadarConsoleSystem.RadarConsoleComponentState
			{
				Range = component.MaxRange
			};
		}

		// Token: 0x0600051B RID: 1307 RVA: 0x0001376A File Offset: 0x0001196A
		protected virtual void UpdateState(RadarConsoleComponent component)
		{
		}

		// Token: 0x0600051C RID: 1308 RVA: 0x0001376C File Offset: 0x0001196C
		public void SetRange(RadarConsoleComponent component, float value)
		{
			if (component.MaxRange.Equals(value))
			{
				return;
			}
			component.MaxRange = value;
			base.Dirty(component, null);
			this.UpdateState(component);
		}

		// Token: 0x020007AB RID: 1963
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		protected sealed class RadarConsoleComponentState : ComponentState
		{
			// Token: 0x040017D0 RID: 6096
			public float Range;
		}
	}
}
