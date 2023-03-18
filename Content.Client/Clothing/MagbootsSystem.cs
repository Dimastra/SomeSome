using System;
using System.Runtime.CompilerServices;
using Content.Shared.Clothing;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Client.Clothing
{
	// Token: 0x020003B6 RID: 950
	public sealed class MagbootsSystem : SharedMagbootsSystem
	{
		// Token: 0x06001798 RID: 6040 RVA: 0x00087903 File Offset: 0x00085B03
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<MagbootsComponent, ComponentHandleState>(new ComponentEventRefHandler<MagbootsComponent, ComponentHandleState>(this.OnHandleState), null, null);
		}

		// Token: 0x06001799 RID: 6041 RVA: 0x00087920 File Offset: 0x00085B20
		[NullableContext(1)]
		private void OnHandleState(EntityUid uid, MagbootsComponent component, ref ComponentHandleState args)
		{
			MagbootsComponent.MagbootsComponentState magbootsComponentState = args.Current as MagbootsComponent.MagbootsComponentState;
			if (magbootsComponentState == null)
			{
				return;
			}
			if (component.On == magbootsComponentState.On)
			{
				return;
			}
			component.On = magbootsComponentState.On;
			base.OnChanged(component);
		}
	}
}
