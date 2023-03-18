using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Fax
{
	// Token: 0x02000500 RID: 1280
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class FaxMachineComponent : Component
	{
		// Token: 0x170003E9 RID: 1001
		// (get) Token: 0x06001A4F RID: 6735 RVA: 0x0008AB90 File Offset: 0x00088D90
		// (set) Token: 0x06001A50 RID: 6736 RVA: 0x0008AB98 File Offset: 0x00088D98
		[ViewVariables]
		[DataField("name", false, 1, false, false, null)]
		public string FaxName { get; set; } = "Unknown";

		// Token: 0x170003EA RID: 1002
		// (get) Token: 0x06001A51 RID: 6737 RVA: 0x0008ABA1 File Offset: 0x00088DA1
		// (set) Token: 0x06001A52 RID: 6738 RVA: 0x0008ABA9 File Offset: 0x00088DA9
		[Nullable(2)]
		[ViewVariables]
		[DataField("destinationAddress", false, 1, false, false, null)]
		public string DestinationFaxAddress { [NullableContext(2)] get; [NullableContext(2)] set; }

		// Token: 0x170003EB RID: 1003
		// (get) Token: 0x06001A53 RID: 6739 RVA: 0x0008ABB2 File Offset: 0x00088DB2
		// (set) Token: 0x06001A54 RID: 6740 RVA: 0x0008ABBA File Offset: 0x00088DBA
		[ViewVariables]
		[DataField("responsePings", false, 1, false, false, null)]
		public bool ResponsePings { get; set; } = true;

		// Token: 0x170003EC RID: 1004
		// (get) Token: 0x06001A55 RID: 6741 RVA: 0x0008ABC3 File Offset: 0x00088DC3
		// (set) Token: 0x06001A56 RID: 6742 RVA: 0x0008ABCB File Offset: 0x00088DCB
		[ViewVariables]
		[DataField("notifyAdmins", false, 1, false, false, null)]
		public bool NotifyAdmins { get; set; }

		// Token: 0x170003ED RID: 1005
		// (get) Token: 0x06001A57 RID: 6743 RVA: 0x0008ABD4 File Offset: 0x00088DD4
		// (set) Token: 0x06001A58 RID: 6744 RVA: 0x0008ABDC File Offset: 0x00088DDC
		[ViewVariables]
		[DataField("receiveNukeCodes", false, 1, false, false, null)]
		public bool ReceiveNukeCodes { get; set; }

		// Token: 0x170003EE RID: 1006
		// (get) Token: 0x06001A59 RID: 6745 RVA: 0x0008ABE5 File Offset: 0x00088DE5
		// (set) Token: 0x06001A5A RID: 6746 RVA: 0x0008ABED File Offset: 0x00088DED
		[ViewVariables]
		[DataField("receiveStationGoal", false, 1, false, false, null)]
		public bool ReceiveStationGoal { get; set; }

		// Token: 0x170003EF RID: 1007
		// (get) Token: 0x06001A5B RID: 6747 RVA: 0x0008ABF6 File Offset: 0x00088DF6
		[ViewVariables]
		public Dictionary<string, string> KnownFaxes { get; } = new Dictionary<string, string>();

		// Token: 0x170003F0 RID: 1008
		// (get) Token: 0x06001A5C RID: 6748 RVA: 0x0008ABFE File Offset: 0x00088DFE
		[ViewVariables]
		[DataField("printingQueue", false, 1, false, false, null)]
		public Queue<FaxPrintout> PrintingQueue { get; } = new Queue<FaxPrintout>();

		// Token: 0x040010BE RID: 4286
		[DataField("paperSlot", false, 1, true, false, null)]
		public ItemSlot PaperSlot = new ItemSlot();

		// Token: 0x040010C3 RID: 4291
		[DataField("emagSound", false, 1, false, false, null)]
		public SoundSpecifier EmagSound = new SoundCollectionSpecifier("sparks", null);

		// Token: 0x040010C4 RID: 4292
		[DataField("printSound", false, 1, false, false, null)]
		public SoundSpecifier PrintSound = new SoundPathSpecifier("/Audio/Machines/printer.ogg", null);

		// Token: 0x040010C5 RID: 4293
		[DataField("sendSound", false, 1, false, false, null)]
		public SoundSpecifier SendSound = new SoundPathSpecifier("/Audio/Machines/high_tech_confirm.ogg", null);

		// Token: 0x040010C8 RID: 4296
		[ViewVariables]
		[DataField("sendTimeoutRemaining", false, 1, false, false, null)]
		public float SendTimeoutRemaining;

		// Token: 0x040010C9 RID: 4297
		[ViewVariables]
		[DataField("sendTimeout", false, 1, false, false, null)]
		public float SendTimeout = 5f;

		// Token: 0x040010CA RID: 4298
		[DataField("insertingTimeRemaining", false, 1, false, false, null)]
		public float InsertingTimeRemaining;

		// Token: 0x040010CB RID: 4299
		[ViewVariables]
		public float InsertionTime = 1.88f;

		// Token: 0x040010CC RID: 4300
		[DataField("printingTimeRemaining", false, 1, false, false, null)]
		public float PrintingTimeRemaining;

		// Token: 0x040010CD RID: 4301
		[ViewVariables]
		public float PrintingTime = 2.3f;
	}
}
