using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Crayon
{
	// Token: 0x02000557 RID: 1367
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class CrayonBoundUserInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x06001099 RID: 4249 RVA: 0x0003628E File Offset: 0x0003448E
		public CrayonBoundUserInterfaceState(string selected, bool selectableColor, Color color)
		{
			this.Selected = selected;
			this.SelectableColor = selectableColor;
			this.Color = color;
		}

		// Token: 0x04000F93 RID: 3987
		public string Selected;

		// Token: 0x04000F94 RID: 3988
		public bool SelectableColor;

		// Token: 0x04000F95 RID: 3989
		public Color Color;
	}
}
