using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Wires
{
	// Token: 0x02000026 RID: 38
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public struct StatusLightData
	{
		// Token: 0x06000028 RID: 40 RVA: 0x0000247A File Offset: 0x0000067A
		public StatusLightData(Color color, StatusLightState state, string text)
		{
			this.Color = color;
			this.State = state;
			this.Text = text;
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000029 RID: 41 RVA: 0x00002491 File Offset: 0x00000691
		public readonly Color Color { get; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600002A RID: 42 RVA: 0x00002499 File Offset: 0x00000699
		public readonly StatusLightState State { get; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600002B RID: 43 RVA: 0x000024A1 File Offset: 0x000006A1
		public readonly string Text { get; }

		// Token: 0x0600002C RID: 44 RVA: 0x000024AC File Offset: 0x000006AC
		public override string ToString()
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 3);
			defaultInterpolatedStringHandler.AppendLiteral("Color: ");
			defaultInterpolatedStringHandler.AppendFormatted<Color>(this.Color);
			defaultInterpolatedStringHandler.AppendLiteral(", State: ");
			defaultInterpolatedStringHandler.AppendFormatted<StatusLightState>(this.State);
			defaultInterpolatedStringHandler.AppendLiteral(", Text: ");
			defaultInterpolatedStringHandler.AppendFormatted(this.Text);
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}
	}
}
