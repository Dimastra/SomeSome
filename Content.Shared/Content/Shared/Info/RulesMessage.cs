using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Info
{
	// Token: 0x020003EB RID: 1003
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class RulesMessage : EntityEventArgs
	{
		// Token: 0x06000BCB RID: 3019 RVA: 0x00026CA9 File Offset: 0x00024EA9
		public RulesMessage(string title, string rules)
		{
			this.Title = title;
			this.Text = rules;
		}

		// Token: 0x04000BBB RID: 3003
		public string Title;

		// Token: 0x04000BBC RID: 3004
		public string Text;
	}
}
