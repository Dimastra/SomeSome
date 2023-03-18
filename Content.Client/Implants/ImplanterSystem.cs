using System;
using System.Runtime.CompilerServices;
using Content.Client.Implants.UI;
using Content.Client.Items;
using Content.Shared.Implants;
using Content.Shared.Implants.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Client.Implants
{
	// Token: 0x020002C4 RID: 708
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ImplanterSystem : SharedImplanterSystem
	{
		// Token: 0x060011D1 RID: 4561 RVA: 0x000698E5 File Offset: 0x00067AE5
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ImplanterComponent, ComponentHandleState>(new ComponentEventRefHandler<ImplanterComponent, ComponentHandleState>(this.OnHandleImplanterState), null, null);
			base.SubscribeLocalEvent<ImplanterComponent, ItemStatusCollectMessage>(new ComponentEventHandler<ImplanterComponent, ItemStatusCollectMessage>(this.OnItemImplanterStatus), null, null);
		}

		// Token: 0x060011D2 RID: 4562 RVA: 0x00069918 File Offset: 0x00067B18
		private void OnHandleImplanterState(EntityUid uid, ImplanterComponent component, ref ComponentHandleState args)
		{
			ImplanterComponentState implanterComponentState = args.Current as ImplanterComponentState;
			if (implanterComponentState == null)
			{
				return;
			}
			component.CurrentMode = implanterComponentState.CurrentMode;
			component.ImplantOnly = implanterComponentState.ImplantOnly;
			component.UiUpdateNeeded = true;
		}

		// Token: 0x060011D3 RID: 4563 RVA: 0x00069954 File Offset: 0x00067B54
		private void OnItemImplanterStatus(EntityUid uid, ImplanterComponent component, ItemStatusCollectMessage args)
		{
			args.Controls.Add(new ImplanterStatusControl(component));
		}
	}
}
