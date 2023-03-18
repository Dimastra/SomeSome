using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Content.Server.GuideGenerator
{
	// Token: 0x02000480 RID: 1152
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class UniversalJsonConverter<[Nullable(2)] T> : JsonConverter<T>
	{
		// Token: 0x06001702 RID: 5890 RVA: 0x0007923B File Offset: 0x0007743B
		public override bool CanConvert(Type typeToConvert)
		{
			return typeof(T).IsAssignableFrom(typeToConvert);
		}

		// Token: 0x06001703 RID: 5891 RVA: 0x0007924D File Offset: 0x0007744D
		public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001704 RID: 5892 RVA: 0x00079254 File Offset: 0x00077454
		public override void Write(Utf8JsonWriter writer, T obj, JsonSerializerOptions options)
		{
			if (obj == null)
			{
				writer.WriteNullValue();
				return;
			}
			FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			PropertyInfo[] properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			writer.WriteStartObject();
			foreach (FieldInfo field in fields)
			{
				if (Attribute.GetCustomAttribute(field, typeof(JsonIgnoreAttribute), true) == null && Attribute.GetCustomAttribute(field, typeof(CompilerGeneratedAttribute), true) == null)
				{
					JsonPropertyNameAttribute attr = (JsonPropertyNameAttribute)Attribute.GetCustomAttribute(field, typeof(JsonPropertyNameAttribute), true);
					string name = (attr == null) ? field.Name : attr.Name;
					this.WriteKV(writer, name, field.GetValue(obj), options);
				}
			}
			foreach (PropertyInfo prop in properties)
			{
				if (Attribute.GetCustomAttribute(prop, typeof(JsonIgnoreAttribute), true) == null)
				{
					JsonPropertyNameAttribute attr2 = (JsonPropertyNameAttribute)Attribute.GetCustomAttribute(prop, typeof(JsonPropertyNameAttribute), true);
					string name2 = (attr2 == null) ? prop.Name : attr2.Name;
					this.WriteKV(writer, name2, prop.GetValue(obj), options);
				}
			}
			writer.WriteEndObject();
		}

		// Token: 0x06001705 RID: 5893 RVA: 0x00079393 File Offset: 0x00077593
		public void WriteKV(Utf8JsonWriter writer, string key, [Nullable(2)] object obj, JsonSerializerOptions options)
		{
			writer.WritePropertyName(key);
			JsonSerializer.Serialize<object>(writer, obj, options);
		}
	}
}
