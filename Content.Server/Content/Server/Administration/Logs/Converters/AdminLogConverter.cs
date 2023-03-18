using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Robust.Shared.IoC;

namespace Content.Server.Administration.Logs.Converters
{
	// Token: 0x0200081F RID: 2079
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public abstract class AdminLogConverter<[Nullable(2)] T> : JsonConverter<T>, IAdminLogConverter
	{
		// Token: 0x06002DB6 RID: 11702 RVA: 0x000EFB0E File Offset: 0x000EDD0E
		public virtual void Init(IDependencyCollection dependencies)
		{
		}

		// Token: 0x06002DB7 RID: 11703 RVA: 0x000EFB10 File Offset: 0x000EDD10
		public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06002DB8 RID: 11704
		public abstract override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options);
	}
}
