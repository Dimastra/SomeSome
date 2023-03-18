using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.IoC;
using Robust.Shared.Reflection;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.UserInterface
{
	// Token: 0x020000FA RID: 250
	[NullableContext(2)]
	[Nullable(0)]
	[DataDefinition]
	public struct IntrinsicUIEntry
	{
		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x06000493 RID: 1171 RVA: 0x00015F63 File Offset: 0x00014163
		// (set) Token: 0x06000494 RID: 1172 RVA: 0x00015F6B File Offset: 0x0001416B
		[ViewVariables]
		public Enum Key { readonly get; set; }

		// Token: 0x06000495 RID: 1173 RVA: 0x00015F74 File Offset: 0x00014174
		public void AfterDeserialization()
		{
			Enum key;
			if (IoCManager.Resolve<IReflectionManager>().TryParseEnumReference(this._keyRaw, ref key, true))
			{
				this.Key = key;
			}
			ToggleIntrinsicUIEvent ev = this.ToggleAction.Event as ToggleIntrinsicUIEvent;
			if (ev != null)
			{
				ev.Key = this.Key;
			}
		}

		// Token: 0x06000496 RID: 1174 RVA: 0x00015FBD File Offset: 0x000141BD
		public IntrinsicUIEntry()
		{
			this.Key = null;
			this._keyRaw = null;
			this.ToggleAction = new InstantAction();
		}

		// Token: 0x040002B0 RID: 688
		[Nullable(1)]
		[DataField("key", true, 1, true, false, null)]
		private string _keyRaw;

		// Token: 0x040002B1 RID: 689
		[Nullable(1)]
		[DataField("toggleAction", false, 1, true, false, null)]
		public InstantAction ToggleAction;
	}
}
