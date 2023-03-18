using System;
using System.Runtime.CompilerServices;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Content.Shared.Construction.Steps
{
	// Token: 0x02000572 RID: 1394
	[NullableContext(1)]
	[Nullable(0)]
	[TypeSerializer]
	public sealed class ConstructionGraphStepTypeSerializer : ITypeReader<ConstructionGraphStep, MappingDataNode>, ITypeValidator<ConstructionGraphStep, MappingDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<ConstructionGraphStep, MappingDataNode>
	{
		// Token: 0x06001109 RID: 4361 RVA: 0x0003823C File Offset: 0x0003643C
		[return: Nullable(2)]
		private Type GetType(MappingDataNode node)
		{
			if (node.Has("material"))
			{
				return typeof(MaterialConstructionGraphStep);
			}
			if (node.Has("tool"))
			{
				return typeof(ToolConstructionGraphStep);
			}
			if (node.Has("component"))
			{
				return typeof(ComponentConstructionGraphStep);
			}
			if (node.Has("tag"))
			{
				return typeof(TagConstructionGraphStep);
			}
			if (node.Has("allTags") || node.Has("anyTags"))
			{
				return typeof(MultipleTagsConstructionGraphStep);
			}
			if (node.Has("minTemperature") || node.Has("maxTemperature"))
			{
				return typeof(TemperatureConstructionGraphStep);
			}
			return null;
		}

		// Token: 0x0600110A RID: 4362 RVA: 0x000382F4 File Offset: 0x000364F4
		public ConstructionGraphStep Read(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, [Nullable(2)] ISerializationContext context = null, [Nullable(new byte[]
		{
			2,
			1
		})] ISerializationManager.InstantiationDelegate<ConstructionGraphStep> instanceProvider = null)
		{
			Type type2 = this.GetType(node);
			if (type2 == null)
			{
				throw new ArgumentException("Tried to convert invalid YAML node mapping to ConstructionGraphStep!");
			}
			Type type = type2;
			return (ConstructionGraphStep)serializationManager.Read(type, node, hookCtx, context, false);
		}

		// Token: 0x0600110B RID: 4363 RVA: 0x0003832C File Offset: 0x0003652C
		public ValidationNode Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, [Nullable(2)] ISerializationContext context = null)
		{
			Type type = this.GetType(node);
			if (type == null)
			{
				return new ErrorNode(node, "No construction graph step type found.", true);
			}
			return serializationManager.ValidateNode(type, node, context);
		}
	}
}
