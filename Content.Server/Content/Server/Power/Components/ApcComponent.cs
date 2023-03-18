using System;
using System.Runtime.CompilerServices;
using Content.Server.Power.NodeGroups;
using Content.Shared.APC;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Power.Components
{
	// Token: 0x0200029F RID: 671
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class ApcComponent : BaseApcNetComponent
	{
		// Token: 0x170001D6 RID: 470
		// (get) Token: 0x06000D9E RID: 3486 RVA: 0x0004743C File Offset: 0x0004563C
		// (set) Token: 0x06000D9F RID: 3487 RVA: 0x00047444 File Offset: 0x00045644
		[DataField("open", false, 1, false, false, null)]
		public bool IsApcOpen { get; set; }

		// Token: 0x06000DA0 RID: 3488 RVA: 0x0004744D File Offset: 0x0004564D
		protected override void AddSelfToNet(IApcNet apcNet)
		{
			apcNet.AddApc(this);
		}

		// Token: 0x06000DA1 RID: 3489 RVA: 0x00047456 File Offset: 0x00045656
		protected override void RemoveSelfFromNet(IApcNet apcNet)
		{
			apcNet.RemoveApc(this);
		}

		// Token: 0x04000811 RID: 2065
		[DataField("onReceiveMessageSound", false, 1, false, false, null)]
		public SoundSpecifier OnReceiveMessageSound = new SoundPathSpecifier("/Audio/Machines/machine_switch.ogg", null);

		// Token: 0x04000812 RID: 2066
		[ViewVariables]
		public ApcChargeState LastChargeState;

		// Token: 0x04000813 RID: 2067
		public TimeSpan LastChargeStateTime;

		// Token: 0x04000815 RID: 2069
		[ViewVariables]
		public ApcExternalPowerState LastExternalState;

		// Token: 0x04000816 RID: 2070
		public TimeSpan LastUiUpdate;

		// Token: 0x04000817 RID: 2071
		[ViewVariables]
		public bool MainBreakerEnabled = true;

		// Token: 0x04000818 RID: 2072
		public const float HighPowerThreshold = 0.9f;

		// Token: 0x04000819 RID: 2073
		public static TimeSpan VisualsChangeDelay = TimeSpan.FromSeconds(1.0);

		// Token: 0x0400081A RID: 2074
		[DataField("screwdriverOpenSound", false, 1, false, false, null)]
		public SoundSpecifier ScrewdriverOpenSound = new SoundPathSpecifier("/Audio/Machines/screwdriveropen.ogg", null);

		// Token: 0x0400081B RID: 2075
		[DataField("screwdriverCloseSound", false, 1, false, false, null)]
		public SoundSpecifier ScrewdriverCloseSound = new SoundPathSpecifier("/Audio/Machines/screwdriverclose.ogg", null);
	}
}
