using System;
using Content.Server.Atmos.Piping.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Piping.Components
{
	// Token: 0x02000766 RID: 1894
	[RegisterComponent]
	public sealed class AtmosPipeColorComponent : Component
	{
		// Token: 0x17000619 RID: 1561
		// (get) Token: 0x06002818 RID: 10264 RVA: 0x000D1F06 File Offset: 0x000D0106
		// (set) Token: 0x06002819 RID: 10265 RVA: 0x000D1F0E File Offset: 0x000D010E
		[DataField("color", false, 1, false, false, null)]
		public Color Color { get; set; } = Color.White;

		// Token: 0x1700061A RID: 1562
		// (get) Token: 0x0600281A RID: 10266 RVA: 0x000D1F17 File Offset: 0x000D0117
		// (set) Token: 0x0600281B RID: 10267 RVA: 0x000D1F1F File Offset: 0x000D011F
		[ViewVariables]
		public Color ColorVV
		{
			get
			{
				return this.Color;
			}
			set
			{
				EntitySystem.Get<AtmosPipeColorSystem>().SetColor(base.Owner, this, value);
			}
		}
	}
}
