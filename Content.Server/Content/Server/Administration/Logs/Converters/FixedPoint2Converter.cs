using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Content.Shared.FixedPoint;

namespace Content.Server.Administration.Logs.Converters
{
	// Token: 0x02000823 RID: 2083
	[AdminLogConverter]
	public sealed class FixedPoint2Converter : AdminLogConverter<FixedPoint2>
	{
		// Token: 0x06002DC1 RID: 11713 RVA: 0x000EFC58 File Offset: 0x000EDE58
		[NullableContext(1)]
		public override void Write(Utf8JsonWriter writer, FixedPoint2 value, JsonSerializerOptions options)
		{
			writer.WriteNumberValue(value.Int());
		}
	}
}
