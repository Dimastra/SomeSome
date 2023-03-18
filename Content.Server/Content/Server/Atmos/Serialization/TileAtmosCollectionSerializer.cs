using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Content.Server.Atmos.Serialization
{
	// Token: 0x0200073C RID: 1852
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TileAtmosCollectionSerializer : ITypeSerializer<Dictionary<Vector2i, TileAtmosphere>, MappingDataNode>, ITypeReaderWriter<Dictionary<Vector2i, TileAtmosphere>, MappingDataNode>, ITypeReader<Dictionary<Vector2i, TileAtmosphere>, MappingDataNode>, ITypeValidator<Dictionary<Vector2i, TileAtmosphere>, MappingDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<Dictionary<Vector2i, TileAtmosphere>, MappingDataNode>, ITypeWriter<Dictionary<Vector2i, TileAtmosphere>>, BaseSerializerInterfaces.ITypeInterface<Dictionary<Vector2i, TileAtmosphere>>, ITypeCopier<Dictionary<Vector2i, TileAtmosphere>>
	{
		// Token: 0x060026ED RID: 9965 RVA: 0x000CCAE7 File Offset: 0x000CACE7
		public ValidationNode Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, [Nullable(2)] ISerializationContext context = null)
		{
			return serializationManager.ValidateNode<TileAtmosCollectionSerializer.TileAtmosData>(node, context);
		}

		// Token: 0x060026EE RID: 9966 RVA: 0x000CCAF4 File Offset: 0x000CACF4
		public Dictionary<Vector2i, TileAtmosphere> Read(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, [Nullable(2)] ISerializationContext context = null, [Nullable(new byte[]
		{
			2,
			1,
			1
		})] ISerializationManager.InstantiationDelegate<Dictionary<Vector2i, TileAtmosphere>> instanceProvider = null)
		{
			TileAtmosCollectionSerializer.TileAtmosData data = serializationManager.Read<TileAtmosCollectionSerializer.TileAtmosData>(node, hookCtx, context, null, false);
			Dictionary<Vector2i, TileAtmosphere> tiles = new Dictionary<Vector2i, TileAtmosphere>();
			if (data.TilesUniqueMixes != null)
			{
				foreach (KeyValuePair<Vector2i, int> keyValuePair in data.TilesUniqueMixes)
				{
					Vector2i vector2i;
					int num;
					keyValuePair.Deconstruct(out vector2i, out num);
					Vector2i indices = vector2i;
					int mix = num;
					try
					{
						tiles.Add(indices, new TileAtmosphere(EntityUid.Invalid, indices, data.UniqueMixes[mix].Clone(), false, false));
					}
					catch (ArgumentOutOfRangeException)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(83, 2);
						defaultInterpolatedStringHandler.AppendLiteral("Error during atmos serialization! Tile at ");
						defaultInterpolatedStringHandler.AppendFormatted<Vector2i>(indices);
						defaultInterpolatedStringHandler.AppendLiteral(" points to an unique mix (");
						defaultInterpolatedStringHandler.AppendFormatted<int>(mix);
						defaultInterpolatedStringHandler.AppendLiteral(") out of range!");
						Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
					}
				}
			}
			return tiles;
		}

		// Token: 0x060026EF RID: 9967 RVA: 0x000CCBFC File Offset: 0x000CADFC
		public DataNode Write(ISerializationManager serializationManager, Dictionary<Vector2i, TileAtmosphere> value, IDependencyCollection dependencies, bool alwaysWrite = false, [Nullable(2)] ISerializationContext context = null)
		{
			List<GasMixture> uniqueMixes = new List<GasMixture>();
			Dictionary<GasMixture, int> uniqueMixHash = new Dictionary<GasMixture, int>();
			Dictionary<Vector2i, int> tiles = new Dictionary<Vector2i, int>();
			foreach (KeyValuePair<Vector2i, TileAtmosphere> keyValuePair in value)
			{
				Vector2i vector2i;
				TileAtmosphere tileAtmosphere;
				keyValuePair.Deconstruct(out vector2i, out tileAtmosphere);
				Vector2i indices = vector2i;
				TileAtmosphere tile = tileAtmosphere;
				if (tile.Air != null)
				{
					int index;
					if (uniqueMixHash.TryGetValue(tile.Air, out index))
					{
						tiles[indices] = index;
					}
					else
					{
						uniqueMixes.Add(tile.Air);
						int newIndex = uniqueMixes.Count - 1;
						uniqueMixHash[tile.Air] = newIndex;
						tiles[indices] = newIndex;
					}
				}
			}
			if (uniqueMixes.Count == 0)
			{
				uniqueMixes = null;
			}
			if (tiles.Count == 0)
			{
				tiles = null;
			}
			return serializationManager.WriteValue<TileAtmosCollectionSerializer.TileAtmosData>(new TileAtmosCollectionSerializer.TileAtmosData
			{
				UniqueMixes = uniqueMixes,
				TilesUniqueMixes = tiles
			}, alwaysWrite, context, false);
		}

		// Token: 0x060026F0 RID: 9968 RVA: 0x000CCCF8 File Offset: 0x000CAEF8
		public void CopyTo(ISerializationManager serializationManager, Dictionary<Vector2i, TileAtmosphere> source, ref Dictionary<Vector2i, TileAtmosphere> target, SerializationHookContext hookCtx, [Nullable(2)] ISerializationContext context = null)
		{
			target.Clear();
			foreach (KeyValuePair<Vector2i, TileAtmosphere> keyValuePair in source)
			{
				Vector2i vector2i;
				TileAtmosphere tileAtmosphere;
				keyValuePair.Deconstruct(out vector2i, out tileAtmosphere);
				Vector2i key = vector2i;
				TileAtmosphere val = tileAtmosphere;
				Dictionary<Vector2i, TileAtmosphere> dictionary = target;
				Vector2i key2 = key;
				EntityUid gridIndex = val.GridIndex;
				Vector2i gridIndices = val.GridIndices;
				GasMixture air = val.Air;
				GasMixture mixture = (air != null) ? air.Clone() : null;
				GasMixture air2 = val.Air;
				dictionary.Add(key2, new TileAtmosphere(gridIndex, gridIndices, mixture, air2 != null && air2.Immutable, val.Space));
			}
		}

		// Token: 0x02000B11 RID: 2833
		[NullableContext(0)]
		[DataDefinition]
		private struct TileAtmosData
		{
			// Token: 0x040028DC RID: 10460
			[Nullable(new byte[]
			{
				2,
				1
			})]
			[DataField("uniqueMixes", false, 1, false, false, null)]
			public List<GasMixture> UniqueMixes;

			// Token: 0x040028DD RID: 10461
			[Nullable(2)]
			[DataField("tiles", false, 1, false, false, null)]
			public Dictionary<Vector2i, int> TilesUniqueMixes;
		}
	}
}
