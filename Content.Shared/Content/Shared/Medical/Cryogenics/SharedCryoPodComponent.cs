using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Medical.Cryogenics
{
	// Token: 0x0200030F RID: 783
	[NullableContext(1)]
	[Nullable(0)]
	[NetworkedComponent]
	public abstract class SharedCryoPodComponent : Component
	{
		// Token: 0x170001AF RID: 431
		// (get) Token: 0x060008F6 RID: 2294 RVA: 0x0001E383 File Offset: 0x0001C583
		// (set) Token: 0x060008F7 RID: 2295 RVA: 0x0001E38B File Offset: 0x0001C58B
		[ViewVariables]
		[DataField("port", false, 1, false, false, null)]
		public string PortName { get; set; } = "port";

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x060008F8 RID: 2296 RVA: 0x0001E394 File Offset: 0x0001C594
		// (set) Token: 0x060008F9 RID: 2297 RVA: 0x0001E39C File Offset: 0x0001C59C
		[ViewVariables]
		[DataField("solutionContainerName", false, 1, false, false, null)]
		public string SolutionContainerName { get; set; } = "beakerSlot";

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x060008FA RID: 2298 RVA: 0x0001E3A5 File Offset: 0x0001C5A5
		// (set) Token: 0x060008FB RID: 2299 RVA: 0x0001E3AD File Offset: 0x0001C5AD
		[ViewVariables]
		[DataField("locked", false, 1, false, false, null)]
		public bool Locked { get; set; }

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x060008FC RID: 2300 RVA: 0x0001E3B6 File Offset: 0x0001C5B6
		// (set) Token: 0x060008FD RID: 2301 RVA: 0x0001E3BE File Offset: 0x0001C5BE
		[ViewVariables]
		[DataField("permaLocked", false, 1, false, false, null)]
		public bool PermaLocked { get; set; }

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x060008FE RID: 2302 RVA: 0x0001E3C7 File Offset: 0x0001C5C7
		// (set) Token: 0x060008FF RID: 2303 RVA: 0x0001E3CF File Offset: 0x0001C5CF
		public bool IsPrying { get; set; }

		// Token: 0x040008F0 RID: 2288
		[ViewVariables]
		[DataField("beakerTransferTime", false, 1, false, false, null)]
		public float BeakerTransferTime = 1f;

		// Token: 0x040008F1 RID: 2289
		[ViewVariables]
		[DataField("nextInjectionTime", false, 1, false, false, typeof(TimeOffsetSerializer))]
		public TimeSpan? NextInjectionTime;

		// Token: 0x040008F2 RID: 2290
		[ViewVariables]
		[DataField("beakerTransferAmount", false, 1, false, false, null)]
		public float BeakerTransferAmount = 1f;

		// Token: 0x040008F3 RID: 2291
		[ViewVariables]
		[DataField("entryDelay", false, 1, false, false, null)]
		public float EntryDelay = 2f;

		// Token: 0x040008F4 RID: 2292
		[ViewVariables]
		[DataField("pryDelay", false, 1, false, false, null)]
		public float PryDelay = 5f;

		// Token: 0x040008F5 RID: 2293
		[ViewVariables]
		public ContainerSlot BodyContainer;

		// Token: 0x020007DA RID: 2010
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		public enum CryoPodVisuals : byte
		{
			// Token: 0x04001838 RID: 6200
			ContainsEntity,
			// Token: 0x04001839 RID: 6201
			IsOn
		}
	}
}
