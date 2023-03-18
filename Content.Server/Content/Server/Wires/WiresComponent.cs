using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Wires
{
	// Token: 0x02000073 RID: 115
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class WiresComponent : Component
	{
		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000173 RID: 371 RVA: 0x00008530 File Offset: 0x00006730
		// (set) Token: 0x06000174 RID: 372 RVA: 0x00008538 File Offset: 0x00006738
		[ViewVariables]
		public bool IsPanelOpen { get; set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000175 RID: 373 RVA: 0x00008541 File Offset: 0x00006741
		// (set) Token: 0x06000176 RID: 374 RVA: 0x00008549 File Offset: 0x00006749
		[ViewVariables]
		public bool IsPanelVisible { get; set; } = true;

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000177 RID: 375 RVA: 0x00008552 File Offset: 0x00006752
		// (set) Token: 0x06000178 RID: 376 RVA: 0x0000855A File Offset: 0x0000675A
		[DataField("BoardName", false, 1, false, false, null)]
		public string BoardName { get; set; } = "Wires";

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000179 RID: 377 RVA: 0x00008563 File Offset: 0x00006763
		// (set) Token: 0x0600017A RID: 378 RVA: 0x0000856B File Offset: 0x0000676B
		[DataField("LayoutId", false, 1, true, false, null)]
		public string LayoutId { get; set; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x0600017B RID: 379 RVA: 0x00008574 File Offset: 0x00006774
		// (set) Token: 0x0600017C RID: 380 RVA: 0x0000857C File Offset: 0x0000677C
		[Nullable(2)]
		[ViewVariables]
		public string SerialNumber { [NullableContext(2)] get; [NullableContext(2)] set; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x0600017D RID: 381 RVA: 0x00008585 File Offset: 0x00006785
		// (set) Token: 0x0600017E RID: 382 RVA: 0x0000858D File Offset: 0x0000678D
		[ViewVariables]
		public int WireSeed { get; set; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600017F RID: 383 RVA: 0x00008596 File Offset: 0x00006796
		// (set) Token: 0x06000180 RID: 384 RVA: 0x0000859E File Offset: 0x0000679E
		[ViewVariables]
		public List<Wire> WiresList { get; set; } = new List<Wire>();

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000181 RID: 385 RVA: 0x000085A7 File Offset: 0x000067A7
		[ViewVariables]
		public List<int> WiresQueue { get; } = new List<int>();

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000182 RID: 386 RVA: 0x000085AF File Offset: 0x000067AF
		[DataField("alwaysRandomize", false, 1, false, false, null)]
		public bool AlwaysRandomize { get; }

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000183 RID: 387 RVA: 0x000085B7 File Offset: 0x000067B7
		[ViewVariables]
		public Dictionary<object, object> Statuses { get; } = new Dictionary<object, object>();

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000184 RID: 388 RVA: 0x000085BF File Offset: 0x000067BF
		[ViewVariables]
		public Dictionary<object, object> StateData { get; } = new Dictionary<object, object>();

		// Token: 0x04000126 RID: 294
		[ViewVariables]
		public bool IsScrewing;

		// Token: 0x04000129 RID: 297
		[DataField("pulseSound", false, 1, false, false, null)]
		public SoundSpecifier PulseSound = new SoundPathSpecifier("/Audio/Effects/multitool_pulse.ogg", null);

		// Token: 0x0400012A RID: 298
		[DataField("screwdriverOpenSound", false, 1, false, false, null)]
		public SoundSpecifier ScrewdriverOpenSound = new SoundPathSpecifier("/Audio/Machines/screwdriveropen.ogg", null);

		// Token: 0x0400012B RID: 299
		[DataField("screwdriverCloseSound", false, 1, false, false, null)]
		public SoundSpecifier ScrewdriverCloseSound = new SoundPathSpecifier("/Audio/Machines/screwdriverclose.ogg", null);
	}
}
