using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared.Decals
{
	// Token: 0x02000525 RID: 1317
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(SharedDecalSystem)
	})]
	[NetworkedComponent]
	public sealed class DecalGridComponent : Component
	{
		// Token: 0x1700032B RID: 811
		// (get) Token: 0x06000FF1 RID: 4081 RVA: 0x000334F7 File Offset: 0x000316F7
		// (set) Token: 0x06000FF2 RID: 4082 RVA: 0x000334FF File Offset: 0x000316FF
		public GameTick ForceTick { get; set; }

		// Token: 0x04000F18 RID: 3864
		[DataField("chunkCollection", false, 1, false, true, null)]
		public DecalGridComponent.DecalGridChunkCollection ChunkCollection = new DecalGridComponent.DecalGridChunkCollection(new Dictionary<Vector2i, DecalGridComponent.DecalChunk>());

		// Token: 0x04000F19 RID: 3865
		public readonly Dictionary<uint, Vector2i> DecalIndex = new Dictionary<uint, Vector2i>();

		// Token: 0x04000F1B RID: 3867
		public readonly Dictionary<uint, int> DecalZIndexIndex = new Dictionary<uint, int>();

		// Token: 0x04000F1C RID: 3868
		public readonly SortedDictionary<int, SortedDictionary<uint, Decal>> DecalRenderIndex = new SortedDictionary<int, SortedDictionary<uint, Decal>>();

		// Token: 0x02000834 RID: 2100
		[Nullable(0)]
		[DataDefinition]
		[NetSerializable]
		[Serializable]
		public sealed class DecalChunk
		{
			// Token: 0x06001909 RID: 6409 RVA: 0x0004F6E7 File Offset: 0x0004D8E7
			public DecalChunk()
			{
				this.Decals = new Dictionary<uint, Decal>();
			}

			// Token: 0x0600190A RID: 6410 RVA: 0x0004F6FA File Offset: 0x0004D8FA
			public DecalChunk(Dictionary<uint, Decal> decals)
			{
				this.Decals = decals;
			}

			// Token: 0x0600190B RID: 6411 RVA: 0x0004F709 File Offset: 0x0004D909
			public DecalChunk(DecalGridComponent.DecalChunk chunk)
			{
				this.Decals = Extensions.ShallowClone<uint, Decal>(chunk.Decals);
				this.LastModified = chunk.LastModified;
			}

			// Token: 0x0400192D RID: 6445
			[IncludeDataField(false, 1, false, typeof(DictionarySerializer<uint, Decal>))]
			public Dictionary<uint, Decal> Decals;

			// Token: 0x0400192E RID: 6446
			[NonSerialized]
			public GameTick LastModified;
		}

		// Token: 0x02000835 RID: 2101
		[Nullable(0)]
		[DataRecord]
		[NetSerializable]
		[Serializable]
		public class DecalGridChunkCollection : IEquatable<DecalGridComponent.DecalGridChunkCollection>
		{
			// Token: 0x0600190C RID: 6412 RVA: 0x0004F72E File Offset: 0x0004D92E
			public DecalGridChunkCollection(Dictionary<Vector2i, DecalGridComponent.DecalChunk> ChunkCollection)
			{
				this.ChunkCollection = ChunkCollection;
				base..ctor();
			}

			// Token: 0x17000516 RID: 1302
			// (get) Token: 0x0600190D RID: 6413 RVA: 0x0004F73D File Offset: 0x0004D93D
			[CompilerGenerated]
			protected virtual Type EqualityContract
			{
				[CompilerGenerated]
				get
				{
					return typeof(DecalGridComponent.DecalGridChunkCollection);
				}
			}

			// Token: 0x17000517 RID: 1303
			// (get) Token: 0x0600190E RID: 6414 RVA: 0x0004F749 File Offset: 0x0004D949
			// (set) Token: 0x0600190F RID: 6415 RVA: 0x0004F751 File Offset: 0x0004D951
			public Dictionary<Vector2i, DecalGridComponent.DecalChunk> ChunkCollection { get; set; }

			// Token: 0x06001910 RID: 6416 RVA: 0x0004F75C File Offset: 0x0004D95C
			[CompilerGenerated]
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("DecalGridChunkCollection");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x06001911 RID: 6417 RVA: 0x0004F7A8 File Offset: 0x0004D9A8
			[CompilerGenerated]
			protected virtual bool PrintMembers(StringBuilder builder)
			{
				RuntimeHelpers.EnsureSufficientExecutionStack();
				builder.Append("ChunkCollection = ");
				builder.Append(this.ChunkCollection);
				builder.Append(", NextDecalId = ");
				builder.Append(this.NextDecalId.ToString());
				return true;
			}

			// Token: 0x06001912 RID: 6418 RVA: 0x0004F7F8 File Offset: 0x0004D9F8
			[NullableContext(2)]
			[CompilerGenerated]
			public static bool operator !=(DecalGridComponent.DecalGridChunkCollection left, DecalGridComponent.DecalGridChunkCollection right)
			{
				return !(left == right);
			}

			// Token: 0x06001913 RID: 6419 RVA: 0x0004F804 File Offset: 0x0004DA04
			[NullableContext(2)]
			[CompilerGenerated]
			public static bool operator ==(DecalGridComponent.DecalGridChunkCollection left, DecalGridComponent.DecalGridChunkCollection right)
			{
				return left == right || (left != null && left.Equals(right));
			}

			// Token: 0x06001914 RID: 6420 RVA: 0x0004F818 File Offset: 0x0004DA18
			[CompilerGenerated]
			public override int GetHashCode()
			{
				return (EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<Dictionary<Vector2i, DecalGridComponent.DecalChunk>>.Default.GetHashCode(this.<ChunkCollection>k__BackingField)) * -1521134295 + EqualityComparer<uint>.Default.GetHashCode(this.NextDecalId);
			}

			// Token: 0x06001915 RID: 6421 RVA: 0x0004F858 File Offset: 0x0004DA58
			[NullableContext(2)]
			[CompilerGenerated]
			public override bool Equals(object obj)
			{
				return this.Equals(obj as DecalGridComponent.DecalGridChunkCollection);
			}

			// Token: 0x06001916 RID: 6422 RVA: 0x0004F868 File Offset: 0x0004DA68
			[NullableContext(2)]
			[CompilerGenerated]
			public virtual bool Equals(DecalGridComponent.DecalGridChunkCollection other)
			{
				return this == other || (other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<Dictionary<Vector2i, DecalGridComponent.DecalChunk>>.Default.Equals(this.<ChunkCollection>k__BackingField, other.<ChunkCollection>k__BackingField) && EqualityComparer<uint>.Default.Equals(this.NextDecalId, other.NextDecalId));
			}

			// Token: 0x06001918 RID: 6424 RVA: 0x0004F8C9 File Offset: 0x0004DAC9
			[CompilerGenerated]
			protected DecalGridChunkCollection(DecalGridComponent.DecalGridChunkCollection original)
			{
				this.ChunkCollection = original.<ChunkCollection>k__BackingField;
				this.NextDecalId = original.NextDecalId;
			}

			// Token: 0x06001919 RID: 6425 RVA: 0x0004F8E9 File Offset: 0x0004DAE9
			[CompilerGenerated]
			public void Deconstruct(out Dictionary<Vector2i, DecalGridComponent.DecalChunk> ChunkCollection)
			{
				ChunkCollection = this.ChunkCollection;
			}

			// Token: 0x04001930 RID: 6448
			public uint NextDecalId;
		}
	}
}
