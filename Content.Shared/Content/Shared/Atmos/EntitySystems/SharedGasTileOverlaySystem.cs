using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.EntitySystems
{
	// Token: 0x020006E0 RID: 1760
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedGasTileOverlaySystem : EntitySystem
	{
		// Token: 0x0600155D RID: 5469 RVA: 0x00045DE0 File Offset: 0x00043FE0
		public override void Initialize()
		{
			base.Initialize();
			List<int> visibleGases = new List<int>();
			for (int i = 0; i < 9; i++)
			{
				GasPrototype gasPrototype = this.ProtoMan.Index<GasPrototype>(i.ToString());
				if (!string.IsNullOrEmpty(gasPrototype.GasOverlayTexture) || (!string.IsNullOrEmpty(gasPrototype.GasOverlaySprite) && !string.IsNullOrEmpty(gasPrototype.GasOverlayState)))
				{
					visibleGases.Add(i);
				}
			}
			this.VisibleGasId = visibleGases.ToArray();
		}

		// Token: 0x0600155E RID: 5470 RVA: 0x00045E53 File Offset: 0x00044053
		public static Vector2i GetGasChunkIndices(Vector2i indices)
		{
			return new Vector2i((int)MathF.Floor((float)indices.X / 8f), (int)MathF.Floor((float)indices.Y / 8f));
		}

		// Token: 0x04001578 RID: 5496
		public const byte ChunkSize = 8;

		// Token: 0x04001579 RID: 5497
		protected float AccumulatedFrameTime;

		// Token: 0x0400157A RID: 5498
		[Dependency]
		protected readonly IPrototypeManager ProtoMan;

		// Token: 0x0400157B RID: 5499
		public int[] VisibleGasId;

		// Token: 0x02000868 RID: 2152
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		public readonly struct GasOverlayData : IEquatable<SharedGasTileOverlaySystem.GasOverlayData>
		{
			// Token: 0x060019D6 RID: 6614 RVA: 0x000517CE File Offset: 0x0004F9CE
			[NullableContext(1)]
			public GasOverlayData(byte fireState, byte[] opacity)
			{
				this.FireState = fireState;
				this.Opacity = opacity;
			}

			// Token: 0x060019D7 RID: 6615 RVA: 0x000517E0 File Offset: 0x0004F9E0
			public bool Equals(SharedGasTileOverlaySystem.GasOverlayData other)
			{
				if (this.FireState != other.FireState)
				{
					return false;
				}
				byte[] opacity = this.Opacity;
				int? num = (opacity != null) ? new int?(opacity.Length) : null;
				byte[] opacity2 = other.Opacity;
				int? num2 = (opacity2 != null) ? new int?(opacity2.Length) : null;
				if (!(num.GetValueOrDefault() == num2.GetValueOrDefault() & num != null == (num2 != null)))
				{
					return false;
				}
				if (this.Opacity != null && other.Opacity != null)
				{
					for (int i = 0; i < this.Opacity.Length; i++)
					{
						if (this.Opacity[i] != other.Opacity[i])
						{
							return false;
						}
					}
				}
				return true;
			}

			// Token: 0x040019F2 RID: 6642
			public readonly byte FireState;

			// Token: 0x040019F3 RID: 6643
			[Nullable(1)]
			public readonly byte[] Opacity;
		}

		// Token: 0x02000869 RID: 2153
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class GasOverlayUpdateEvent : EntityEventArgs
		{
			// Token: 0x040019F4 RID: 6644
			public Dictionary<EntityUid, List<GasOverlayChunk>> UpdatedChunks = new Dictionary<EntityUid, List<GasOverlayChunk>>();

			// Token: 0x040019F5 RID: 6645
			public Dictionary<EntityUid, HashSet<Vector2i>> RemovedChunks = new Dictionary<EntityUid, HashSet<Vector2i>>();
		}
	}
}
