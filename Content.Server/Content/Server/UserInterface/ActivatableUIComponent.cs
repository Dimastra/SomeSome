using System;
using System.Runtime.CompilerServices;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Reflection;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.UserInterface
{
	// Token: 0x020000F2 RID: 242
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class ActivatableUIComponent : Component, ISerializationHooks
	{
		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x0600046E RID: 1134 RVA: 0x00015932 File Offset: 0x00013B32
		// (set) Token: 0x0600046F RID: 1135 RVA: 0x0001593A File Offset: 0x00013B3A
		[ViewVariables]
		public Enum Key { get; set; }

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x06000470 RID: 1136 RVA: 0x00015943 File Offset: 0x00013B43
		[ViewVariables]
		public BoundUserInterface UserInterface
		{
			get
			{
				if (this.Key == null)
				{
					return null;
				}
				return base.Owner.GetUIOrNull(this.Key);
			}
		}

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x06000471 RID: 1137 RVA: 0x00015960 File Offset: 0x00013B60
		// (set) Token: 0x06000472 RID: 1138 RVA: 0x00015968 File Offset: 0x00013B68
		[ViewVariables]
		[DataField("inHandsOnly", false, 1, false, false, null)]
		public bool InHandsOnly { get; set; }

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x06000473 RID: 1139 RVA: 0x00015971 File Offset: 0x00013B71
		// (set) Token: 0x06000474 RID: 1140 RVA: 0x00015979 File Offset: 0x00013B79
		[DataField("singleUser", false, 1, false, false, null)]
		public bool SingleUser { get; set; }

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x06000475 RID: 1141 RVA: 0x00015982 File Offset: 0x00013B82
		// (set) Token: 0x06000476 RID: 1142 RVA: 0x0001598A File Offset: 0x00013B8A
		[ViewVariables]
		[DataField("adminOnly", false, 1, false, false, null)]
		public bool AdminOnly { get; set; }

		// Token: 0x06000477 RID: 1143 RVA: 0x00015994 File Offset: 0x00013B94
		void ISerializationHooks.AfterDeserialization()
		{
			Enum key;
			if (IoCManager.Resolve<IReflectionManager>().TryParseEnumReference(this._keyRaw, ref key, true))
			{
				this.Key = key;
			}
		}

		// Token: 0x0400029F RID: 671
		[Nullable(1)]
		[DataField("key", true, 1, true, false, null)]
		private string _keyRaw;

		// Token: 0x040002A0 RID: 672
		[Nullable(1)]
		[DataField("verbText", false, 1, false, false, null)]
		public string VerbText = "ui-verb-toggle-open";

		// Token: 0x040002A1 RID: 673
		[ViewVariables]
		[DataField("requireHands", false, 1, false, false, null)]
		public bool RequireHands = true;

		// Token: 0x040002A2 RID: 674
		[ViewVariables]
		[DataField("allowSpectator", false, 1, false, false, null)]
		public bool AllowSpectator = true;

		// Token: 0x040002A3 RID: 675
		[ViewVariables]
		[DataField("closeOnHandDeselect", false, 1, false, false, null)]
		public bool CloseOnHandDeselect = true;

		// Token: 0x040002A4 RID: 676
		[ViewVariables]
		public IPlayerSession CurrentSingleUser;
	}
}
