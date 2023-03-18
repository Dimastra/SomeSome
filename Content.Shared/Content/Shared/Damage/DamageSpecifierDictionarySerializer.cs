using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Content.Shared.Damage
{
	// Token: 0x02000531 RID: 1329
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DamageSpecifierDictionarySerializer : ITypeReader<Dictionary<string, FixedPoint2>, MappingDataNode>, ITypeValidator<Dictionary<string, FixedPoint2>, MappingDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<Dictionary<string, FixedPoint2>, MappingDataNode>
	{
		// Token: 0x0600102A RID: 4138 RVA: 0x00034410 File Offset: 0x00032610
		public ValidationNode Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, [Nullable(2)] ISerializationContext context = null)
		{
			Dictionary<ValidationNode, ValidationNode> vals = new Dictionary<ValidationNode, ValidationNode>();
			MappingDataNode typesNode;
			if (node.TryGet<MappingDataNode>("types", ref typesNode))
			{
				vals.Add(new ValidatedValueNode(new ValueDataNode("types")), this._damageTypeSerializer.Validate(serializationManager, typesNode, dependencies, context));
			}
			MappingDataNode groupsNode;
			if (node.TryGet<MappingDataNode>("groups", ref groupsNode))
			{
				vals.Add(new ValidatedValueNode(new ValueDataNode("groups")), this._damageGroupSerializer.Validate(serializationManager, groupsNode, dependencies, context));
			}
			return new ValidatedMappingNode(vals);
		}

		// Token: 0x0600102B RID: 4139 RVA: 0x00034494 File Offset: 0x00032694
		public Dictionary<string, FixedPoint2> Read(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, [Nullable(2)] ISerializationContext context = null, [Nullable(new byte[]
		{
			2,
			1,
			1
		})] ISerializationManager.InstantiationDelegate<Dictionary<string, FixedPoint2>> instanceProvider = null)
		{
			Dictionary<string, FixedPoint2> dict = (instanceProvider != null) ? instanceProvider.Invoke() : new Dictionary<string, FixedPoint2>();
			DataNode typesNode;
			if (node.TryGet("types", ref typesNode))
			{
				serializationManager.Read<Dictionary<string, FixedPoint2>>(typesNode, null, false, () => dict, true);
			}
			DataNode groupsNode;
			if (!node.TryGet("groups", ref groupsNode))
			{
				return dict;
			}
			IPrototypeManager prototypeManager = dependencies.Resolve<IPrototypeManager>();
			foreach (KeyValuePair<string, FixedPoint2> entry in serializationManager.Read<Dictionary<string, FixedPoint2>>(groupsNode, null, false, null, true))
			{
				DamageGroupPrototype group;
				if (!prototypeManager.TryIndex<DamageGroupPrototype>(entry.Key, ref group))
				{
					dependencies.Resolve<ILogManager>().RootSawmill.Error("Unknown damage group given to DamageSpecifier: " + entry.Key);
				}
				else
				{
					int remainingTypes = group.DamageTypes.Count;
					FixedPoint2 remainingDamage = entry.Value;
					foreach (string damageType in group.DamageTypes)
					{
						FixedPoint2 damage = remainingDamage / FixedPoint2.New(remainingTypes);
						if (!dict.TryAdd(damageType, damage))
						{
							Dictionary<string, FixedPoint2> dict2 = dict;
							string key = damageType;
							dict2[key] += damage;
						}
						remainingDamage -= damage;
						remainingTypes--;
					}
				}
			}
			return dict;
		}

		// Token: 0x04000F3D RID: 3901
		private ITypeValidator<Dictionary<string, FixedPoint2>, MappingDataNode> _damageTypeSerializer = new PrototypeIdDictionarySerializer<FixedPoint2, DamageTypePrototype>();

		// Token: 0x04000F3E RID: 3902
		private ITypeValidator<Dictionary<string, FixedPoint2>, MappingDataNode> _damageGroupSerializer = new PrototypeIdDictionarySerializer<FixedPoint2, DamageGroupPrototype>();
	}
}
