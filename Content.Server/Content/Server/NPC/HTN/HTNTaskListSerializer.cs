using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.NPC.HTN.PrimitiveTasks;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Content.Server.NPC.HTN
{
	// Token: 0x0200034B RID: 843
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class HTNTaskListSerializer : ITypeSerializer<List<string>, SequenceDataNode>, ITypeReaderWriter<List<string>, SequenceDataNode>, ITypeReader<List<string>, SequenceDataNode>, ITypeValidator<List<string>, SequenceDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<List<string>, SequenceDataNode>, ITypeWriter<List<string>>, BaseSerializerInterfaces.ITypeInterface<List<string>>
	{
		// Token: 0x060011A3 RID: 4515 RVA: 0x0005D34C File Offset: 0x0005B54C
		public ValidationNode Validate(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, [Nullable(2)] ISerializationContext context = null)
		{
			List<ValidationNode> list = new List<ValidationNode>();
			IPrototypeManager protoManager = dependencies.Resolve<IPrototypeManager>();
			foreach (DataNode data in node.Sequence)
			{
				MappingDataNode mapping = data as MappingDataNode;
				if (mapping == null)
				{
					List<ValidationNode> list2 = list;
					DataNode dataNode = data;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Found invalid mapping node on ");
					defaultInterpolatedStringHandler.AppendFormatted<DataNode>(data);
					list2.Add(new ErrorNode(dataNode, defaultInterpolatedStringHandler.ToStringAndClear(), true));
				}
				else
				{
					string id = ((ValueDataNode)mapping["id"]).Value;
					bool isCompound = protoManager.HasIndex<HTNCompoundTask>(id);
					bool isPrimitive = protoManager.HasIndex<HTNPrimitiveTask>(id);
					list.Add((isCompound ^ isPrimitive) ? new ValidatedValueNode(node) : new ErrorNode(node, "Found duplicated HTN compound and primitive tasks for " + id, true));
				}
			}
			return new ValidatedSequenceNode(list);
		}

		// Token: 0x060011A4 RID: 4516 RVA: 0x0005D440 File Offset: 0x0005B640
		public List<string> Read(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, [Nullable(2)] ISerializationContext context = null, [Nullable(new byte[]
		{
			2,
			1,
			1
		})] ISerializationManager.InstantiationDelegate<List<string>> instanceProvider = null)
		{
			List<string> value = (instanceProvider != null) ? instanceProvider.Invoke() : new List<string>();
			foreach (DataNode dataNode in node.Sequence)
			{
				string id = ((ValueDataNode)((MappingDataNode)dataNode)["id"]).Value;
				value.Add(id);
			}
			return value;
		}

		// Token: 0x060011A5 RID: 4517 RVA: 0x0005D4BC File Offset: 0x0005B6BC
		public DataNode Write(ISerializationManager serializationManager, List<string> value, IDependencyCollection dependencies, bool alwaysWrite = false, [Nullable(2)] ISerializationContext context = null)
		{
			SequenceDataNode sequence = new SequenceDataNode();
			foreach (string task in value)
			{
				MappingDataNode mappingDataNode = new MappingDataNode();
				mappingDataNode["id"] = new ValueDataNode(task);
				MappingDataNode mapping = mappingDataNode;
				sequence.Add(mapping);
			}
			return sequence;
		}
	}
}
