using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Atmos;
using Content.Shared.Disposal.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Disposal.Unit.Components
{
	// Token: 0x02000555 RID: 1365
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[ComponentReference(typeof(SharedDisposalUnitComponent))]
	public sealed class DisposalUnitComponent : SharedDisposalUnitComponent, IGasMixtureHolder
	{
		// Token: 0x1700044E RID: 1102
		// (get) Token: 0x06001CCE RID: 7374 RVA: 0x00099920 File Offset: 0x00097B20
		// (set) Token: 0x06001CCF RID: 7375 RVA: 0x00099928 File Offset: 0x00097B28
		[ViewVariables]
		public bool Engaged { get; set; }

		// Token: 0x1700044F RID: 1103
		// (get) Token: 0x06001CD0 RID: 7376 RVA: 0x00099931 File Offset: 0x00097B31
		// (set) Token: 0x06001CD1 RID: 7377 RVA: 0x00099939 File Offset: 0x00097B39
		[DataField("air", false, 1, false, false, null)]
		public GasMixture Air { get; set; } = new GasMixture(2500f);

		// Token: 0x04001268 RID: 4712
		[ViewVariables]
		public TimeSpan LastExitAttempt;

		// Token: 0x04001269 RID: 4713
		[DataField("pressure", false, 1, false, false, null)]
		public float Pressure = 1f;

		// Token: 0x0400126A RID: 4714
		[DataField("autoEngageEnabled", false, 1, false, false, null)]
		public bool AutomaticEngage = true;

		// Token: 0x0400126B RID: 4715
		[ViewVariables]
		[DataField("autoEngageTime", false, 1, false, false, null)]
		public readonly TimeSpan AutomaticEngageTime = TimeSpan.FromSeconds(30.0);

		// Token: 0x0400126C RID: 4716
		[ViewVariables]
		[DataField("flushDelay", false, 1, false, false, null)]
		public readonly TimeSpan FlushDelay = TimeSpan.FromSeconds(3.0);

		// Token: 0x0400126D RID: 4717
		[ViewVariables]
		[DataField("entryDelay", false, 1, false, false, null)]
		public float EntryDelay = 0.5f;

		// Token: 0x0400126E RID: 4718
		[ViewVariables]
		public float DraggedEntryDelay = 0.5f;

		// Token: 0x0400126F RID: 4719
		[Nullable(2)]
		public CancellationTokenSource AutomaticEngageToken;

		// Token: 0x04001270 RID: 4720
		[ViewVariables]
		public Container Container;

		// Token: 0x04001271 RID: 4721
		[ViewVariables]
		public bool Powered;

		// Token: 0x04001272 RID: 4722
		[ViewVariables]
		public SharedDisposalUnitComponent.PressureState State;
	}
}
