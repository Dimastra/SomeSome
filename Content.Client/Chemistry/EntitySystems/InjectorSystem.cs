using System;
using System.Runtime.CompilerServices;
using Content.Client.Chemistry.Components;
using Content.Client.Chemistry.UI;
using Content.Client.Items;
using Content.Shared.Chemistry.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Client.Chemistry.EntitySystems
{
	// Token: 0x020003E0 RID: 992
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class InjectorSystem : EntitySystem
	{
		// Token: 0x06001879 RID: 6265 RVA: 0x0008D604 File Offset: 0x0008B804
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<InjectorComponent, ComponentHandleState>(new ComponentEventRefHandler<InjectorComponent, ComponentHandleState>(this.OnHandleInjectorState), null, null);
			base.SubscribeLocalEvent<InjectorComponent, ItemStatusCollectMessage>(new ComponentEventHandler<InjectorComponent, ItemStatusCollectMessage>(this.OnItemInjectorStatus), null, null);
			base.SubscribeLocalEvent<HyposprayComponent, ComponentHandleState>(new ComponentEventRefHandler<HyposprayComponent, ComponentHandleState>(this.OnHandleHyposprayState), null, null);
			base.SubscribeLocalEvent<HyposprayComponent, ItemStatusCollectMessage>(new ComponentEventHandler<HyposprayComponent, ItemStatusCollectMessage>(this.OnItemHyposprayStatus), null, null);
		}

		// Token: 0x0600187A RID: 6266 RVA: 0x0008D668 File Offset: 0x0008B868
		private void OnHandleInjectorState(EntityUid uid, InjectorComponent component, ref ComponentHandleState args)
		{
			SharedInjectorComponent.InjectorComponentState injectorComponentState = args.Current as SharedInjectorComponent.InjectorComponentState;
			if (injectorComponentState == null)
			{
				return;
			}
			component.CurrentVolume = injectorComponentState.CurrentVolume;
			component.TotalVolume = injectorComponentState.TotalVolume;
			component.CurrentMode = injectorComponentState.CurrentMode;
			component.UiUpdateNeeded = true;
		}

		// Token: 0x0600187B RID: 6267 RVA: 0x0008D6B0 File Offset: 0x0008B8B0
		private void OnItemInjectorStatus(EntityUid uid, InjectorComponent component, ItemStatusCollectMessage args)
		{
			args.Controls.Add(new InjectorStatusControl(component));
		}

		// Token: 0x0600187C RID: 6268 RVA: 0x0008D6C4 File Offset: 0x0008B8C4
		private void OnHandleHyposprayState(EntityUid uid, HyposprayComponent component, ref ComponentHandleState args)
		{
			HyposprayComponentState hyposprayComponentState = args.Current as HyposprayComponentState;
			if (hyposprayComponentState == null)
			{
				return;
			}
			component.CurrentVolume = hyposprayComponentState.CurVolume;
			component.TotalVolume = hyposprayComponentState.MaxVolume;
			component.UiUpdateNeeded = true;
		}

		// Token: 0x0600187D RID: 6269 RVA: 0x0008D700 File Offset: 0x0008B900
		private void OnItemHyposprayStatus(EntityUid uid, HyposprayComponent component, ItemStatusCollectMessage args)
		{
			args.Controls.Add(new HyposprayStatusControl(component));
		}
	}
}
