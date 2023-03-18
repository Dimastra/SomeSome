using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Robust.Shared.GameObjects;

namespace Content.Server.Administration.Logs.Converters
{
	// Token: 0x02000821 RID: 2081
	[AdminLogConverter]
	public sealed class EntityStringRepresentationConverter : AdminLogConverter<EntityStringRepresentation>
	{
		// Token: 0x06002DBB RID: 11707 RVA: 0x000EFB28 File Offset: 0x000EDD28
		[NullableContext(1)]
		public override void Write(Utf8JsonWriter writer, EntityStringRepresentation value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteNumber("id", (int)value.Uid);
			if (value.Name != null)
			{
				writer.WriteString("name", value.Name);
			}
			if (value.Session != null)
			{
				writer.WriteString("player", value.Session.UserId.UserId);
			}
			writer.WriteEndObject();
		}
	}
}
