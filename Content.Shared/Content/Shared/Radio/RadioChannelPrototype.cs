using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Radio
{
	// Token: 0x0200021E RID: 542
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("radioChannel", 1)]
	public sealed class RadioChannelPrototype : IPrototype
	{
		// Token: 0x17000127 RID: 295
		// (get) Token: 0x06000605 RID: 1541 RVA: 0x00015282 File Offset: 0x00013482
		// (set) Token: 0x06000606 RID: 1542 RVA: 0x0001528A File Offset: 0x0001348A
		[DataField("name", false, 1, false, false, null)]
		public string Name { get; private set; } = string.Empty;

		// Token: 0x17000128 RID: 296
		// (get) Token: 0x06000607 RID: 1543 RVA: 0x00015293 File Offset: 0x00013493
		[ViewVariables]
		public string LocalizedName
		{
			get
			{
				return Loc.GetString(this.Name);
			}
		}

		// Token: 0x17000129 RID: 297
		// (get) Token: 0x06000608 RID: 1544 RVA: 0x000152A0 File Offset: 0x000134A0
		// (set) Token: 0x06000609 RID: 1545 RVA: 0x000152A8 File Offset: 0x000134A8
		[DataField("keycodes", false, 1, false, false, null)]
		public List<char> KeyCodes { get; private set; } = new List<char>
		{
			'\0'
		};

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x0600060A RID: 1546 RVA: 0x000152B1 File Offset: 0x000134B1
		// (set) Token: 0x0600060B RID: 1547 RVA: 0x000152B9 File Offset: 0x000134B9
		[DataField("frequency", false, 1, false, false, null)]
		public int Frequency { get; private set; }

		// Token: 0x1700012B RID: 299
		// (get) Token: 0x0600060C RID: 1548 RVA: 0x000152C2 File Offset: 0x000134C2
		// (set) Token: 0x0600060D RID: 1549 RVA: 0x000152CA File Offset: 0x000134CA
		[DataField("color", false, 1, false, false, null)]
		public Color Color { get; private set; } = Color.Lime;

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x0600060E RID: 1550 RVA: 0x000152D3 File Offset: 0x000134D3
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }
	}
}
