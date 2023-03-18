using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Tabletop.Components
{
	// Token: 0x02000133 RID: 307
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(TabletopSystem)
	})]
	public sealed class TabletopGameComponent : Component
	{
		// Token: 0x170000FC RID: 252
		// (get) Token: 0x0600059A RID: 1434 RVA: 0x0001BCBA File Offset: 0x00019EBA
		[DataField("boardName", false, 1, false, false, null)]
		public string BoardName { get; } = "tabletop-default-board-name";

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x0600059B RID: 1435 RVA: 0x0001BCC2 File Offset: 0x00019EC2
		[DataField("setup", false, 1, true, false, null)]
		public TabletopSetup Setup { get; } = new TabletopChessSetup();

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x0600059C RID: 1436 RVA: 0x0001BCCA File Offset: 0x00019ECA
		[DataField("size", false, 1, false, false, null)]
		public Vector2i Size { get; } = new ValueTuple<int, int>(300, 300);

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x0600059D RID: 1437 RVA: 0x0001BCD2 File Offset: 0x00019ED2
		[DataField("cameraZoom", false, 1, false, false, null)]
		public Vector2 CameraZoom { get; } = Vector2.One;

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x0600059E RID: 1438 RVA: 0x0001BCDA File Offset: 0x00019EDA
		// (set) Token: 0x0600059F RID: 1439 RVA: 0x0001BCE2 File Offset: 0x00019EE2
		[Nullable(2)]
		[ViewVariables]
		public TabletopSession Session { [NullableContext(2)] get; [NullableContext(2)] set; }
	}
}
