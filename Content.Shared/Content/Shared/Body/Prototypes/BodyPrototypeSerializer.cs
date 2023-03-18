using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Body.Organ;
using Content.Shared.Prototypes;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Content.Shared.Body.Prototypes
{
	// Token: 0x02000655 RID: 1621
	[NullableContext(1)]
	[Nullable(0)]
	[TypeSerializer]
	public sealed class BodyPrototypeSerializer : ITypeReader<BodyPrototype, MappingDataNode>, ITypeValidator<BodyPrototype, MappingDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<BodyPrototype, MappingDataNode>
	{
		// Token: 0x060013C7 RID: 5063 RVA: 0x0004240C File Offset: 0x0004060C
		[return: TupleElementNames(new string[]
		{
			"Node",
			"Connections"
		})]
		[return: Nullable(new byte[]
		{
			0,
			1,
			1,
			1
		})]
		private ValueTuple<ValidationNode, List<string>> ValidateSlot(MappingDataNode slot, IDependencyCollection dependencies)
		{
			List<ValidationNode> nodes = new List<ValidationNode>();
			IPrototypeManager prototypes = dependencies.Resolve<IPrototypeManager>();
			List<string> connections = new List<string>();
			SequenceDataNode connectionsNode;
			if (slot.TryGet<SequenceDataNode>("connections", ref connectionsNode))
			{
				foreach (DataNode node in connectionsNode)
				{
					ValueDataNode connection = node as ValueDataNode;
					if (connection == null)
					{
						nodes.Add(new ErrorNode(node, "Connection is not a value data node", true));
					}
					else
					{
						connections.Add(connection.Value);
					}
				}
			}
			MappingDataNode organsNode;
			if (slot.TryGet<MappingDataNode>("organs", ref organsNode))
			{
				foreach (KeyValuePair<DataNode, DataNode> keyValuePair in organsNode)
				{
					DataNode dataNode;
					DataNode dataNode2;
					keyValuePair.Deconstruct(out dataNode, out dataNode2);
					DataNode key = dataNode;
					DataNode value = dataNode2;
					if (!(key is ValueDataNode))
					{
						nodes.Add(new ErrorNode(key, "Key is not a value data node", true));
					}
					else
					{
						ValueDataNode organ = value as ValueDataNode;
						EntityPrototype organPrototype;
						if (organ == null)
						{
							nodes.Add(new ErrorNode(value, "Value is not a value data node", true));
						}
						else if (!prototypes.TryIndex<EntityPrototype>(organ.Value, ref organPrototype))
						{
							nodes.Add(new ErrorNode(value, "No organ entity prototype found with id " + organ.Value, true));
						}
						else if (!organPrototype.HasComponent(null))
						{
							nodes.Add(new ErrorNode(value, "Organ " + organ.Value + " does not have a body component", true));
						}
					}
				}
			}
			return new ValueTuple<ValidationNode, List<string>>(new ValidatedSequenceNode(nodes), connections);
		}

		// Token: 0x060013C8 RID: 5064 RVA: 0x000425B8 File Offset: 0x000407B8
		public ValidationNode Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, [Nullable(2)] ISerializationContext context = null)
		{
			List<ValidationNode> nodes = new List<ValidationNode>();
			ValueDataNode root;
			if (!node.TryGet<ValueDataNode>("root", ref root))
			{
				nodes.Add(new ErrorNode(node, "No root value data node found", true));
			}
			MappingDataNode slots;
			if (!node.TryGet<MappingDataNode>("slots", ref slots))
			{
				nodes.Add(new ErrorNode(node, "No slots mapping data node found", true));
			}
			else if (root != null)
			{
				MappingDataNode mappingDataNode;
				if (!slots.TryGet<MappingDataNode>(root.Value, ref mappingDataNode))
				{
					nodes.Add(new ErrorNode(slots, "No slot found with id " + root.Value, true));
					return new ValidatedSequenceNode(nodes);
				}
				foreach (KeyValuePair<DataNode, DataNode> keyValuePair in slots)
				{
					DataNode dataNode;
					DataNode dataNode2;
					keyValuePair.Deconstruct(out dataNode, out dataNode2);
					DataNode key = dataNode;
					DataNode value = dataNode2;
					if (!(key is ValueDataNode))
					{
						nodes.Add(new ErrorNode(key, "Key is not a value data node", true));
					}
					else
					{
						MappingDataNode slot = value as MappingDataNode;
						if (slot == null)
						{
							nodes.Add(new ErrorNode(value, "Slot is not a mapping data node", true));
						}
						else
						{
							ValueTuple<ValidationNode, List<string>> result = this.ValidateSlot(slot, dependencies);
							nodes.Add(result.Item1);
							foreach (string connection in result.Item2)
							{
								if (!slots.TryGet<MappingDataNode>(connection, ref mappingDataNode))
								{
									nodes.Add(new ErrorNode(slots, "No slot found with id " + connection, true));
								}
							}
						}
					}
				}
			}
			return new ValidatedSequenceNode(nodes);
		}

		// Token: 0x060013C9 RID: 5065 RVA: 0x00042764 File Offset: 0x00040964
		public BodyPrototype Read(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, [Nullable(2)] ISerializationContext context = null, [Nullable(new byte[]
		{
			2,
			1
		})] ISerializationManager.InstantiationDelegate<BodyPrototype> instanceProvider = null)
		{
			string id = node.Get<ValueDataNode>("id").Value;
			string name = node.Get<ValueDataNode>("name").Value;
			string root = node.Get<ValueDataNode>("root").Value;
			MappingDataNode mappingDataNode = node.Get<MappingDataNode>("slots");
			Dictionary<string, ValueTuple<string, HashSet<string>, Dictionary<string, string>>> allConnections = new Dictionary<string, ValueTuple<string, HashSet<string>, Dictionary<string, string>>>();
			foreach (KeyValuePair<DataNode, DataNode> keyValuePair in mappingDataNode)
			{
				DataNode dataNode;
				DataNode dataNode2;
				keyValuePair.Deconstruct(out dataNode, out dataNode2);
				DataNode dataNode3 = dataNode;
				DataNode valueNode = dataNode2;
				string slotId = ((ValueDataNode)dataNode3).Value;
				MappingDataNode slot = (MappingDataNode)valueNode;
				string part = null;
				ValueDataNode value;
				if (slot.TryGet<ValueDataNode>("part", ref value))
				{
					part = value.Value;
				}
				HashSet<string> connections = null;
				SequenceDataNode slotConnectionsNode;
				if (slot.TryGet<SequenceDataNode>("connections", ref slotConnectionsNode))
				{
					connections = new HashSet<string>();
					foreach (ValueDataNode connection in slotConnectionsNode.Cast<ValueDataNode>())
					{
						connections.Add(connection.Value);
					}
				}
				Dictionary<string, string> organs = null;
				MappingDataNode slotOrgansNode;
				if (slot.TryGet<MappingDataNode>("organs", ref slotOrgansNode))
				{
					organs = new Dictionary<string, string>();
					using (IEnumerator<KeyValuePair<DataNode, DataNode>> enumerator3 = slotOrgansNode.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							keyValuePair = enumerator3.Current;
							keyValuePair.Deconstruct(out dataNode2, out dataNode);
							DataNode organKeyNode = dataNode2;
							DataNode organValueNode = dataNode;
							organs.Add(((ValueDataNode)organKeyNode).Value, ((ValueDataNode)organValueNode).Value);
						}
					}
				}
				allConnections.Add(slotId, new ValueTuple<string, HashSet<string>, Dictionary<string, string>>(part, connections, organs));
			}
			foreach (KeyValuePair<string, ValueTuple<string, HashSet<string>, Dictionary<string, string>>> keyValuePair2 in allConnections)
			{
				string text;
				ValueTuple<string, HashSet<string>, Dictionary<string, string>> valueTuple;
				keyValuePair2.Deconstruct(out text, out valueTuple);
				ref ValueTuple<string, HashSet<string>, Dictionary<string, string>> ptr = valueTuple;
				string slotId2 = text;
				HashSet<string> connections2 = ptr.Item2;
				if (connections2 != null)
				{
					foreach (string connection2 in connections2)
					{
						ValueTuple<string, HashSet<string>, Dictionary<string, string>> other = allConnections[connection2];
						ref HashSet<string> ptr2 = ref other.Item2;
						if (ptr2 == null)
						{
							ptr2 = new HashSet<string>();
						}
						other.Item2.Add(slotId2);
						allConnections[connection2] = other;
					}
				}
			}
			Dictionary<string, BodyPrototypeSlot> slots = new Dictionary<string, BodyPrototypeSlot>();
			foreach (KeyValuePair<string, ValueTuple<string, HashSet<string>, Dictionary<string, string>>> keyValuePair2 in allConnections)
			{
				string text;
				ValueTuple<string, HashSet<string>, Dictionary<string, string>> valueTuple;
				keyValuePair2.Deconstruct(out text, out valueTuple);
				ValueTuple<string, HashSet<string>, Dictionary<string, string>> valueTuple2 = valueTuple;
				string slotId3 = text;
				string part2 = valueTuple2.Item1;
				HashSet<string> connections3 = valueTuple2.Item2;
				Dictionary<string, string> organs2 = valueTuple2.Item3;
				BodyPrototypeSlot slot2 = new BodyPrototypeSlot(part2, connections3, organs2);
				slots.Add(slotId3, slot2);
			}
			return new BodyPrototype(id, name, root, slots);
		}
	}
}
