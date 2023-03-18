using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Shared.Maps;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Noise;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Parallax.Biomes
{
	// Token: 0x020002A3 RID: 675
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedBiomeSystem : EntitySystem
	{
		// Token: 0x06000787 RID: 1927 RVA: 0x000194D5 File Offset: 0x000176D5
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<BiomeComponent, ComponentGetState>(new ComponentEventRefHandler<BiomeComponent, ComponentGetState>(this.OnBiomeGetState), null, null);
			base.SubscribeLocalEvent<BiomeComponent, ComponentHandleState>(new ComponentEventRefHandler<BiomeComponent, ComponentHandleState>(this.OnBiomeHandleState), null, null);
		}

		// Token: 0x06000788 RID: 1928 RVA: 0x00019508 File Offset: 0x00017708
		private void OnBiomeHandleState(EntityUid uid, BiomeComponent component, ref ComponentHandleState args)
		{
			SharedBiomeSystem.BiomeComponentState state = args.Current as SharedBiomeSystem.BiomeComponentState;
			if (state == null)
			{
				return;
			}
			component.Seed = state.Seed;
		}

		// Token: 0x06000789 RID: 1929 RVA: 0x00019531 File Offset: 0x00017731
		private void OnBiomeGetState(EntityUid uid, BiomeComponent component, ref ComponentGetState args)
		{
			args.State = new SharedBiomeSystem.BiomeComponentState(component.Seed, component.BiomePrototype);
		}

		// Token: 0x0600078A RID: 1930 RVA: 0x0001954C File Offset: 0x0001774C
		protected T Pick<[Nullable(2)] T>(List<T> collection, float value)
		{
			if (collection.Count == 1)
			{
				return collection[0];
			}
			value *= (float)collection.Count;
			foreach (T item in collection)
			{
				value -= 1f;
				if (value <= 0f)
				{
					return item;
				}
			}
			throw new ArgumentOutOfRangeException();
		}

		// Token: 0x0600078B RID: 1931 RVA: 0x000195CC File Offset: 0x000177CC
		protected int Pick(int count, float value)
		{
			if (count == 1)
			{
				return 0;
			}
			value *= (float)count;
			for (int i = 0; i < count; i++)
			{
				value -= 1f;
				if (value <= 0f)
				{
					return i;
				}
			}
			throw new ArgumentOutOfRangeException();
		}

		// Token: 0x0600078C RID: 1932 RVA: 0x0001960C File Offset: 0x0001780C
		public bool TryGetBiomeTile(EntityUid uid, MapGridComponent grid, Vector2i indices, [NotNullWhen(true)] out Tile? tile)
		{
			TileRef tileRef;
			if (grid.TryGetTileRef(indices, ref tileRef))
			{
				tile = new Tile?(tileRef.Tile);
				return true;
			}
			BiomeComponent biome;
			if (!base.TryComp<BiomeComponent>(uid, ref biome))
			{
				tile = null;
				return false;
			}
			return this.TryGetBiomeTile(indices, this.ProtoManager.Index<BiomePrototype>(biome.BiomePrototype), new FastNoise(biome.Seed), grid, out tile);
		}

		// Token: 0x0600078D RID: 1933 RVA: 0x00019674 File Offset: 0x00017874
		public bool TryGetBiomeTile(Vector2i indices, BiomePrototype prototype, FastNoise seed, [Nullable(2)] MapGridComponent grid, [NotNullWhen(true)] out Tile? tile)
		{
			TileRef tileRef;
			if (grid != null && grid.TryGetTileRef(indices, ref tileRef) && !tileRef.Tile.IsEmpty)
			{
				tile = new Tile?(tileRef.Tile);
				return true;
			}
			float oldFrequency = seed.GetFrequency();
			for (int i = prototype.Layers.Count - 1; i >= 0; i--)
			{
				BiomeTileLayer tileLayer = prototype.Layers[i] as BiomeTileLayer;
				if (tileLayer != null)
				{
					seed.SetFrequency(tileLayer.Frequency);
					if (this.TryGetTile(indices, seed, tileLayer.Threshold, this.ProtoManager.Index<ContentTileDefinition>(tileLayer.Tile), tileLayer.Variants, out tile))
					{
						seed.SetFrequency(oldFrequency);
						return true;
					}
				}
			}
			seed.SetFrequency(oldFrequency);
			tile = null;
			return false;
		}

		// Token: 0x0600078E RID: 1934 RVA: 0x00019738 File Offset: 0x00017938
		protected bool TryGetEntity(Vector2i indices, BiomePrototype prototype, FastNoise noise, MapGridComponent grid, [Nullable(2)] [NotNullWhen(true)] out string entity)
		{
			Tile? tileRef;
			if (!this.TryGetBiomeTile(indices, prototype, noise, grid, out tileRef))
			{
				entity = null;
				return false;
			}
			string tileId = this.TileDefManager[(int)tileRef.Value.TypeId].ID;
			float oldFrequency = noise.GetFrequency();
			int seed = noise.GetSeed();
			for (int i = prototype.Layers.Count - 1; i >= 0; i--)
			{
				IBiomeLayer layer = prototype.Layers[i];
				IBiomeWorldLayer worldLayer = layer as IBiomeWorldLayer;
				if (worldLayer != null && worldLayer.AllowedTiles.Contains(tileId))
				{
					int offset = worldLayer.SeedOffset;
					noise.SetSeed(seed + offset);
					noise.SetFrequency(worldLayer.Frequency);
					if ((noise.GetCellular((float)indices.X, (float)indices.Y) + 1f) / 2f >= layer.Threshold)
					{
						BiomeEntityLayer biomeLayer = layer as BiomeEntityLayer;
						if (biomeLayer == null)
						{
							entity = null;
							noise.SetFrequency(oldFrequency);
							noise.SetSeed(seed);
							return false;
						}
						entity = this.Pick<string>(biomeLayer.Entities, (noise.GetSimplex((float)indices.X, (float)indices.Y) + 1f) / 2f);
						noise.SetFrequency(oldFrequency);
						noise.SetSeed(seed);
						return true;
					}
				}
			}
			noise.SetFrequency(oldFrequency);
			noise.SetSeed(seed);
			entity = null;
			return false;
		}

		// Token: 0x0600078F RID: 1935 RVA: 0x00019898 File Offset: 0x00017A98
		public bool TryGetDecals(Vector2i indices, BiomePrototype prototype, FastNoise noise, MapGridComponent grid, [TupleElementNames(new string[]
		{
			"ID",
			"Position"
		})] [Nullable(new byte[]
		{
			2,
			0,
			1
		})] [NotNullWhen(true)] out List<ValueTuple<string, Vector2>> decals)
		{
			Tile? tileRef;
			if (!this.TryGetBiomeTile(indices, prototype, noise, grid, out tileRef))
			{
				decals = null;
				return false;
			}
			string tileId = this.TileDefManager[(int)tileRef.Value.TypeId].ID;
			float oldFrequency = noise.GetFrequency();
			int seed = noise.GetSeed();
			for (int i = prototype.Layers.Count - 1; i >= 0; i--)
			{
				IBiomeLayer layer = prototype.Layers[i];
				IBiomeWorldLayer worldLayer = layer as IBiomeWorldLayer;
				if (worldLayer != null && worldLayer.AllowedTiles.Contains(tileId))
				{
					int offset = worldLayer.SeedOffset;
					noise.SetSeed(seed + offset);
					noise.SetFrequency(worldLayer.Frequency);
					BiomeDecalLayer decalLayer = layer as BiomeDecalLayer;
					if (decalLayer == null)
					{
						if ((noise.GetCellular((float)indices.X, (float)indices.Y) + 1f) / 2f >= layer.Threshold)
						{
							decals = null;
							noise.SetFrequency(oldFrequency);
							noise.SetSeed(seed);
							return false;
						}
					}
					else
					{
						decals = new List<ValueTuple<string, Vector2>>();
						int x = 0;
						while ((float)x < decalLayer.Divisions)
						{
							int y = 0;
							while ((float)y < decalLayer.Divisions)
							{
								Vector2 index;
								index..ctor((float)indices.X + (float)x * 1f / decalLayer.Divisions, (float)indices.Y + (float)y * 1f / decalLayer.Divisions);
								if ((noise.GetCellular(index.X, index.Y) + 1f) / 2f >= decalLayer.Threshold)
								{
									decals.Add(new ValueTuple<string, Vector2>(this.Pick<string>(decalLayer.Decals, (noise.GetSimplex(index.X, index.Y) + 1f) / 2f), index));
								}
								y++;
							}
							x++;
						}
						noise.SetFrequency(oldFrequency);
						noise.SetSeed(seed);
						if (decals.Count != 0)
						{
							return true;
						}
					}
				}
			}
			noise.SetFrequency(oldFrequency);
			noise.SetSeed(seed);
			decals = null;
			return false;
		}

		// Token: 0x06000790 RID: 1936 RVA: 0x00019AB8 File Offset: 0x00017CB8
		public bool TryGetTile(Vector2i indices, FastNoise seed, float threshold, ContentTileDefinition tileDef, [Nullable(2)] List<byte> variants, [NotNullWhen(true)] out Tile? tile)
		{
			if (threshold > 0f && (seed.GetSimplexFractal((float)indices.X, (float)indices.Y) + 1f) / 2f < threshold)
			{
				tile = null;
				return false;
			}
			byte variant = 0;
			int variantCount = (variants != null) ? variants.Count : ((int)tileDef.Variants);
			if (variantCount > 1)
			{
				float variantValue = (seed.GetSimplex((float)indices.X * 2f, (float)indices.Y * 2f) + 1f) / 2f;
				variant = (byte)this.Pick(variantCount, variantValue);
				if (variants != null)
				{
					variant = variants[(int)variant];
				}
			}
			tile = new Tile?(new Tile(tileDef.TileId, 0, variant));
			return true;
		}

		// Token: 0x040007AA RID: 1962
		[Dependency]
		protected readonly IPrototypeManager ProtoManager;

		// Token: 0x040007AB RID: 1963
		[Dependency]
		protected readonly ITileDefinitionManager TileDefManager;

		// Token: 0x040007AC RID: 1964
		protected const byte ChunkSize = 8;

		// Token: 0x020007BE RID: 1982
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		private sealed class BiomeComponentState : ComponentState
		{
			// Token: 0x06001821 RID: 6177 RVA: 0x0004D87E File Offset: 0x0004BA7E
			public BiomeComponentState(int seed, string prototype)
			{
				this.Seed = seed;
				this.Prototype = prototype;
			}

			// Token: 0x040017F3 RID: 6131
			public int Seed;

			// Token: 0x040017F4 RID: 6132
			public string Prototype;
		}
	}
}
