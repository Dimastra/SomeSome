using System;
using System.Runtime.CompilerServices;
using Content.Server.UserInterface;
using Content.Shared.Crayon;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Crayon
{
	// Token: 0x020005DB RID: 1499
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class CrayonComponent : SharedCrayonComponent
	{
		// Token: 0x170004C5 RID: 1221
		// (get) Token: 0x06002004 RID: 8196 RVA: 0x000A75B3 File Offset: 0x000A57B3
		// (set) Token: 0x06002005 RID: 8197 RVA: 0x000A75BB File Offset: 0x000A57BB
		[ViewVariables]
		[DataField("selectableColor", false, 1, false, false, null)]
		public bool SelectableColor { get; set; }

		// Token: 0x170004C6 RID: 1222
		// (get) Token: 0x06002006 RID: 8198 RVA: 0x000A75C4 File Offset: 0x000A57C4
		// (set) Token: 0x06002007 RID: 8199 RVA: 0x000A75CC File Offset: 0x000A57CC
		[ViewVariables]
		public int Charges { get; set; }

		// Token: 0x170004C7 RID: 1223
		// (get) Token: 0x06002008 RID: 8200 RVA: 0x000A75D5 File Offset: 0x000A57D5
		// (set) Token: 0x06002009 RID: 8201 RVA: 0x000A75DD File Offset: 0x000A57DD
		[ViewVariables]
		[DataField("capacity", false, 1, false, false, null)]
		public int Capacity { get; set; } = 30;

		// Token: 0x170004C8 RID: 1224
		// (get) Token: 0x0600200A RID: 8202 RVA: 0x000A75E6 File Offset: 0x000A57E6
		[ViewVariables]
		public BoundUserInterface UserInterface
		{
			get
			{
				return base.Owner.GetUIOrNull(SharedCrayonComponent.CrayonUiKey.Key);
			}
		}

		// Token: 0x040013DC RID: 5084
		[DataField("useSound", false, 1, false, false, null)]
		public SoundSpecifier UseSound;

		// Token: 0x040013E0 RID: 5088
		[ViewVariables]
		[DataField("deleteEmpty", false, 1, false, false, null)]
		public bool DeleteEmpty = true;
	}
}
