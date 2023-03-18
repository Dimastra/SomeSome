using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Content.Shared.Decals
{
	// Token: 0x02000524 RID: 1316
	[NullableContext(1)]
	[Nullable(0)]
	[TypeSerializer]
	public sealed class DecalGridChunkCollectionTypeSerializer : ITypeSerializer<DecalGridComponent.DecalGridChunkCollection, MappingDataNode>, ITypeReaderWriter<DecalGridComponent.DecalGridChunkCollection, MappingDataNode>, ITypeReader<DecalGridComponent.DecalGridChunkCollection, MappingDataNode>, ITypeValidator<DecalGridComponent.DecalGridChunkCollection, MappingDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<DecalGridComponent.DecalGridChunkCollection, MappingDataNode>, ITypeWriter<DecalGridComponent.DecalGridChunkCollection>, BaseSerializerInterfaces.ITypeInterface<DecalGridComponent.DecalGridChunkCollection>
	{
		// Token: 0x06000FED RID: 4077 RVA: 0x00033307 File Offset: 0x00031507
		public ValidationNode Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, [Nullable(2)] ISerializationContext context = null)
		{
			return serializationManager.ValidateNode<Dictionary<Vector2i, Dictionary<uint, Decal>>>(node, context);
		}

		// Token: 0x06000FEE RID: 4078 RVA: 0x00033314 File Offset: 0x00031514
		public DecalGridComponent.DecalGridChunkCollection Read(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, [Nullable(2)] ISerializationContext context = null, [Nullable(new byte[]
		{
			2,
			1
		})] ISerializationManager.InstantiationDelegate<DecalGridComponent.DecalGridChunkCollection> _ = null)
		{
			Dictionary<Vector2i, DecalGridComponent.DecalChunk> dictionary = serializationManager.Read<Dictionary<Vector2i, DecalGridComponent.DecalChunk>>(node, hookCtx, context, null, true);
			SortedSet<uint> uids = new SortedSet<uint>();
			Dictionary<uint, Vector2i> uidChunkMap = new Dictionary<uint, Vector2i>();
			foreach (KeyValuePair<Vector2i, DecalGridComponent.DecalChunk> keyValuePair in dictionary)
			{
				Vector2i vector2i;
				DecalGridComponent.DecalChunk decalChunk;
				keyValuePair.Deconstruct(out vector2i, out decalChunk);
				Vector2i indices = vector2i;
				foreach (uint uid in decalChunk.Decals.Keys)
				{
					uids.Add(uid);
					uidChunkMap[uid] = indices;
				}
			}
			Dictionary<uint, uint> uidMap = new Dictionary<uint, uint>();
			uint nextIndex = 0U;
			foreach (uint uid2 in uids)
			{
				uidMap[uid2] = nextIndex++;
			}
			Dictionary<Vector2i, DecalGridComponent.DecalChunk> newDict = new Dictionary<Vector2i, DecalGridComponent.DecalChunk>();
			foreach (KeyValuePair<uint, uint> keyValuePair2 in uidMap)
			{
				uint num;
				uint num2;
				keyValuePair2.Deconstruct(out num, out num2);
				uint oldUid = num;
				uint newUid = num2;
				Vector2i indices2 = uidChunkMap[oldUid];
				if (!newDict.ContainsKey(indices2))
				{
					newDict[indices2] = new DecalGridComponent.DecalChunk();
				}
				newDict[indices2].Decals[newUid] = dictionary[indices2].Decals[oldUid];
			}
			return new DecalGridComponent.DecalGridChunkCollection(newDict)
			{
				NextDecalId = nextIndex
			};
		}

		// Token: 0x06000FEF RID: 4079 RVA: 0x000334DC File Offset: 0x000316DC
		public DataNode Write(ISerializationManager serializationManager, DecalGridComponent.DecalGridChunkCollection value, IDependencyCollection dependencies, bool alwaysWrite = false, [Nullable(2)] ISerializationContext context = null)
		{
			return serializationManager.WriteValue<Dictionary<Vector2i, DecalGridComponent.DecalChunk>>(value.ChunkCollection, alwaysWrite, context, true);
		}
	}
}
