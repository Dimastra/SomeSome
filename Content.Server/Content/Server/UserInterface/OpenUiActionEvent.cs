using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Reflection;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.UserInterface
{
	// Token: 0x020000FE RID: 254
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class OpenUiActionEvent : InstantActionEvent, ISerializationHooks
	{
		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x060004A3 RID: 1187 RVA: 0x000161D9 File Offset: 0x000143D9
		// (set) Token: 0x060004A4 RID: 1188 RVA: 0x000161E1 File Offset: 0x000143E1
		[ViewVariables]
		public Enum Key { get; set; }

		// Token: 0x060004A5 RID: 1189 RVA: 0x000161EC File Offset: 0x000143EC
		void ISerializationHooks.AfterDeserialization()
		{
			Enum key;
			if (IoCManager.Resolve<IReflectionManager>().TryParseEnumReference(this._keyRaw, ref key, true))
			{
				this.Key = key;
				return;
			}
			Logger.Error("Invalid UI key (" + this._keyRaw + ") in open-UI action");
		}

		// Token: 0x040002B7 RID: 695
		[Nullable(1)]
		[DataField("key", true, 1, true, false, null)]
		private string _keyRaw;
	}
}
