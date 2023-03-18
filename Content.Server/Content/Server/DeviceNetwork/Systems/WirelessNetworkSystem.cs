using System;
using System.Runtime.CompilerServices;
using Content.Server.DeviceNetwork.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Server.DeviceNetwork.Systems
{
	// Token: 0x0200058A RID: 1418
	public sealed class WirelessNetworkSystem : EntitySystem
	{
		// Token: 0x06001DC0 RID: 7616 RVA: 0x0009E7CD File Offset: 0x0009C9CD
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<WirelessNetworkComponent, BeforePacketSentEvent>(new ComponentEventHandler<WirelessNetworkComponent, BeforePacketSentEvent>(this.OnBeforePacketSent), null, null);
		}

		// Token: 0x06001DC1 RID: 7617 RVA: 0x0009E7EC File Offset: 0x0009C9EC
		[NullableContext(1)]
		private void OnBeforePacketSent(EntityUid uid, WirelessNetworkComponent component, BeforePacketSentEvent args)
		{
			Vector2 ownPosition = args.SenderPosition;
			TransformComponent xform = base.Transform(uid);
			WirelessNetworkComponent sendingComponent;
			if (xform.MapID != args.SenderTransform.MapID || !base.TryComp<WirelessNetworkComponent>(args.Sender, ref sendingComponent) || (ownPosition - xform.WorldPosition).Length > (float)sendingComponent.Range)
			{
				args.Cancel();
			}
		}
	}
}
