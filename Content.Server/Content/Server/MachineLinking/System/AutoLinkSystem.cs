using System;
using System.Runtime.CompilerServices;
using Content.Server.MachineLinking.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.MachineLinking.System
{
	// Token: 0x020003EE RID: 1006
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AutoLinkSystem : EntitySystem
	{
		// Token: 0x0600148C RID: 5260 RVA: 0x0006AC66 File Offset: 0x00068E66
		public override void Initialize()
		{
			base.SubscribeLocalEvent<AutoLinkTransmitterComponent, MapInitEvent>(new ComponentEventHandler<AutoLinkTransmitterComponent, MapInitEvent>(this.OnAutoLinkMapInit), null, null);
		}

		// Token: 0x0600148D RID: 5261 RVA: 0x0006AC7C File Offset: 0x00068E7C
		private void OnAutoLinkMapInit(EntityUid uid, AutoLinkTransmitterComponent component, MapInitEvent args)
		{
			TransformComponent xform = base.Transform(uid);
			foreach (AutoLinkReceiverComponent receiver in base.EntityQuery<AutoLinkReceiverComponent>(false))
			{
				if (!(receiver.AutoLinkChannel != component.AutoLinkChannel) && !(base.Transform(receiver.Owner).GridUid != xform.GridUid))
				{
					this._signalLinkerSystem.TryLinkDefaults(receiver.Owner, uid, null, null, null);
				}
			}
		}

		// Token: 0x04000CC0 RID: 3264
		[Dependency]
		private readonly SignalLinkerSystem _signalLinkerSystem;
	}
}
