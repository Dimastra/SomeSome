using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization;

namespace Content.Shared.Wires
{
	// Token: 0x02000028 RID: 40
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public struct StatusEntry
	{
		// Token: 0x06000033 RID: 51 RVA: 0x0000256A File Offset: 0x0000076A
		public StatusEntry(object key, object value)
		{
			this.Key = key;
			this.Value = value;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x0000257C File Offset: 0x0000077C
		public override string ToString()
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 2);
			defaultInterpolatedStringHandler.AppendFormatted<object>(this.Key);
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted<object>(this.Value);
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}

		// Token: 0x04000078 RID: 120
		public readonly object Key;

		// Token: 0x04000079 RID: 121
		public readonly object Value;
	}
}
