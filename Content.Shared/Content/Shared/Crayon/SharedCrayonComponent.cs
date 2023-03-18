using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Crayon
{
	// Token: 0x02000552 RID: 1362
	[NullableContext(1)]
	[Nullable(0)]
	[NetworkedComponent]
	[ComponentProtoName("Crayon")]
	[Access(new Type[]
	{
		typeof(SharedCrayonSystem)
	})]
	public abstract class SharedCrayonComponent : Component
	{
		// Token: 0x17000349 RID: 841
		// (get) Token: 0x06001093 RID: 4243 RVA: 0x00036227 File Offset: 0x00034427
		// (set) Token: 0x06001094 RID: 4244 RVA: 0x0003622F File Offset: 0x0003442F
		public string SelectedState { get; set; } = string.Empty;

		// Token: 0x04000F89 RID: 3977
		[DataField("color", false, 1, false, false, null)]
		public Color Color;

		// Token: 0x0200083A RID: 2106
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		public enum CrayonUiKey : byte
		{
			// Token: 0x0400193F RID: 6463
			Key
		}
	}
}
