using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.IoC;
using Robust.Shared.Reflection;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Utility;

namespace Content.Server.NPC
{
	// Token: 0x02000330 RID: 816
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NPCBlackboardSerializer : ITypeReader<NPCBlackboard, MappingDataNode>, ITypeValidator<NPCBlackboard, MappingDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<NPCBlackboard, MappingDataNode>, ITypeCopier<NPCBlackboard>, BaseSerializerInterfaces.ITypeInterface<NPCBlackboard>
	{
		// Token: 0x060010DF RID: 4319 RVA: 0x00056C00 File Offset: 0x00054E00
		public ValidationNode Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, [Nullable(2)] ISerializationContext context = null)
		{
			List<ValidationNode> validated = new List<ValidationNode>();
			if (node.Count > 0)
			{
				IReflectionManager reflection = dependencies.Resolve<IReflectionManager>();
				foreach (KeyValuePair<DataNode, DataNode> data in node)
				{
					string key = YamlHelpers.AsString(YamlNodeHelpers.ToYamlNode(data.Key));
					if (data.Value.Tag == null)
					{
						validated.Add(new ErrorNode(data.Key, "Unable to validate " + key + "'s type", true));
					}
					else
					{
						string tag = data.Value.Tag;
						string typeString = tag.Substring(6, tag.Length - 6);
						Type type;
						if (!reflection.TryLooseGetType(typeString, ref type))
						{
							validated.Add(new ErrorNode(data.Key, "Unable to find type for " + typeString, true));
						}
						else
						{
							ValidationNode validatedNode = serializationManager.ValidateNode(type, data.Value, context);
							validated.Add(validatedNode);
						}
					}
				}
			}
			return new ValidatedSequenceNode(validated);
		}

		// Token: 0x060010E0 RID: 4320 RVA: 0x00056D18 File Offset: 0x00054F18
		public NPCBlackboard Read(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, [Nullable(2)] ISerializationContext context = null, [Nullable(new byte[]
		{
			2,
			1
		})] ISerializationManager.InstantiationDelegate<NPCBlackboard> instanceProvider = null)
		{
			NPCBlackboard value = (instanceProvider != null) ? instanceProvider.Invoke() : new NPCBlackboard();
			if (node.Count > 0)
			{
				IReflectionManager reflection = dependencies.Resolve<IReflectionManager>();
				foreach (KeyValuePair<DataNode, DataNode> data in node)
				{
					string key = YamlHelpers.AsString(YamlNodeHelpers.ToYamlNode(data.Key));
					if (data.Value.Tag == null)
					{
						throw new NullReferenceException("Found null tag for " + key);
					}
					string tag = data.Value.Tag;
					string typeString = tag.Substring(6, tag.Length - 6);
					Type type;
					if (!reflection.TryLooseGetType(typeString, ref type))
					{
						throw new NullReferenceException("Found null type for " + key);
					}
					object bbData = serializationManager.Read(type, data.Value, hookCtx, context, false);
					if (bbData == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 2);
						defaultInterpolatedStringHandler.AppendLiteral("Found null data for ");
						defaultInterpolatedStringHandler.AppendFormatted(key);
						defaultInterpolatedStringHandler.AppendLiteral(", expected ");
						defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
						throw new NullReferenceException(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					value.SetValue(key, bbData);
				}
			}
			return value;
		}

		// Token: 0x060010E1 RID: 4321 RVA: 0x00056E60 File Offset: 0x00055060
		public void CopyTo(ISerializationManager serializationManager, NPCBlackboard source, ref NPCBlackboard target, SerializationHookContext hookCtx, [Nullable(2)] ISerializationContext context = null)
		{
			target.Clear();
			foreach (KeyValuePair<string, object> current in source)
			{
				target.SetValue(current.Key, current.Value);
			}
		}
	}
}
