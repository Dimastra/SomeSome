using System;
using System.Runtime.CompilerServices;
using Content.Server.Power.NodeGroups;
using Content.Server.Power.Pow3r;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Power.Components
{
	// Token: 0x020002B7 RID: 695
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	[RegisterComponent]
	public sealed class PowerConsumerComponent : BaseNetConnectorComponent<IBasePowerNet>
	{
		// Token: 0x170001EF RID: 495
		// (get) Token: 0x06000DFD RID: 3581 RVA: 0x00047C0A File Offset: 0x00045E0A
		// (set) Token: 0x06000DFE RID: 3582 RVA: 0x00047C17 File Offset: 0x00045E17
		[DataField("drawRate", false, 1, false, false, null)]
		[ViewVariables]
		public float DrawRate
		{
			get
			{
				return this.NetworkLoad.DesiredPower;
			}
			set
			{
				this.NetworkLoad.DesiredPower = value;
			}
		}

		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x06000DFF RID: 3583 RVA: 0x00047C25 File Offset: 0x00045E25
		[ViewVariables]
		public float ReceivedPower
		{
			get
			{
				return this.NetworkLoad.ReceivingPower;
			}
		}

		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x06000E00 RID: 3584 RVA: 0x00047C32 File Offset: 0x00045E32
		public PowerState.Load NetworkLoad { get; } = new PowerState.Load();

		// Token: 0x06000E01 RID: 3585 RVA: 0x00047C3A File Offset: 0x00045E3A
		protected override void AddSelfToNet(IBasePowerNet powerNet)
		{
			powerNet.AddConsumer(this);
		}

		// Token: 0x06000E02 RID: 3586 RVA: 0x00047C43 File Offset: 0x00045E43
		protected override void RemoveSelfFromNet(IBasePowerNet powerNet)
		{
			powerNet.RemoveConsumer(this);
		}

		// Token: 0x04000845 RID: 2117
		public float LastReceived = float.NaN;
	}
}
