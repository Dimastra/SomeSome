using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Power.NodeGroups;
using Content.Server.Power.Pow3r;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.Power.Components
{
	// Token: 0x020002A1 RID: 673
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[ComponentProtoName("PowerProvider")]
	public sealed class ApcPowerProviderComponent : BaseApcNetComponent
	{
		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x06000DA5 RID: 3493 RVA: 0x000474E2 File Offset: 0x000456E2
		[ViewVariables]
		public List<ApcPowerReceiverComponent> LinkedReceivers { get; } = new List<ApcPowerReceiverComponent>();

		// Token: 0x06000DA6 RID: 3494 RVA: 0x000474EA File Offset: 0x000456EA
		public void AddReceiver(ApcPowerReceiverComponent receiver)
		{
			this.LinkedReceivers.Add(receiver);
			receiver.NetworkLoad.LinkedNetwork = default(PowerState.NodeId);
			IApcNet net = base.Net;
			if (net == null)
			{
				return;
			}
			net.QueueNetworkReconnect();
		}

		// Token: 0x06000DA7 RID: 3495 RVA: 0x00047519 File Offset: 0x00045719
		public void RemoveReceiver(ApcPowerReceiverComponent receiver)
		{
			this.LinkedReceivers.Remove(receiver);
			receiver.NetworkLoad.LinkedNetwork = default(PowerState.NodeId);
			IApcNet net = base.Net;
			if (net == null)
			{
				return;
			}
			net.QueueNetworkReconnect();
		}

		// Token: 0x06000DA8 RID: 3496 RVA: 0x00047549 File Offset: 0x00045749
		protected override void AddSelfToNet(IApcNet apcNet)
		{
			apcNet.AddPowerProvider(this);
		}

		// Token: 0x06000DA9 RID: 3497 RVA: 0x00047552 File Offset: 0x00045752
		protected override void RemoveSelfFromNet(IApcNet apcNet)
		{
			apcNet.RemovePowerProvider(this);
		}
	}
}
