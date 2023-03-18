using System;
using System.Runtime.CompilerServices;
using Content.Client.GPS.Components;
using Content.Client.GPS.UI;
using Content.Client.Items;
using Robust.Shared.GameObjects;

namespace Content.Client.GPS.Systems
{
	// Token: 0x020002FE RID: 766
	public sealed class HandheldGpsSystem : EntitySystem
	{
		// Token: 0x06001338 RID: 4920 RVA: 0x00072842 File Offset: 0x00070A42
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<HandheldGPSComponent, ItemStatusCollectMessage>(new ComponentEventHandler<HandheldGPSComponent, ItemStatusCollectMessage>(this.OnItemStatus), null, null);
		}

		// Token: 0x06001339 RID: 4921 RVA: 0x0007285E File Offset: 0x00070A5E
		[NullableContext(1)]
		private void OnItemStatus(EntityUid uid, HandheldGPSComponent component, ItemStatusCollectMessage args)
		{
			args.Controls.Add(new HandheldGpsStatusControl(component));
		}
	}
}
