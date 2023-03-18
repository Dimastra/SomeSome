using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Content.Server.Containers
{
	// Token: 0x020005E9 RID: 1513
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ContainerFillSerializer : ITypeValidator<Dictionary<string, List<string>>, MappingDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<Dictionary<string, List<string>>, MappingDataNode>
	{
		// Token: 0x170004D4 RID: 1236
		// (get) Token: 0x06002045 RID: 8261 RVA: 0x000A8300 File Offset: 0x000A6500
		private static PrototypeIdListSerializer<EntityPrototype> ListSerializer
		{
			get
			{
				return new PrototypeIdListSerializer<EntityPrototype>();
			}
		}

		// Token: 0x06002046 RID: 8262 RVA: 0x000A8308 File Offset: 0x000A6508
		public ValidationNode Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, [Nullable(2)] ISerializationContext context = null)
		{
			Dictionary<ValidationNode, ValidationNode> mapping = new Dictionary<ValidationNode, ValidationNode>();
			foreach (KeyValuePair<DataNode, DataNode> keyValuePair in node.Children)
			{
				DataNode dataNode;
				DataNode dataNode2;
				keyValuePair.Deconstruct(out dataNode, out dataNode2);
				DataNode key = dataNode;
				DataNode val = dataNode2;
				ValidationNode keyVal = serializationManager.ValidateNode<string>(key, context);
				SequenceDataNode seq = val as SequenceDataNode;
				ValidationNode listVal = (seq != null) ? ContainerFillSerializer.ListSerializer.Validate(serializationManager, seq, dependencies, context) : new ErrorNode(val, "ContainerFillComponent prototypes must be a sequence/list", true);
				mapping.Add(keyVal, listVal);
			}
			return new ValidatedMappingNode(mapping);
		}
	}
}
