using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.EntitySystems
{
	// Token: 0x020006DE RID: 1758
	public abstract class SharedAtmosDebugOverlaySystem : EntitySystem
	{
		// Token: 0x04001574 RID: 5492
		public const int LocalViewRange = 16;

		// Token: 0x04001575 RID: 5493
		protected float AccumulatedFrameTime;

		// Token: 0x02000865 RID: 2149
		[NullableContext(1)]
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public readonly struct AtmosDebugOverlayData
		{
			// Token: 0x060019D0 RID: 6608 RVA: 0x0005175A File Offset: 0x0004F95A
			public AtmosDebugOverlayData(float temperature, float[] moles, AtmosDirection pressureDirection, AtmosDirection lastPressureDirection, int inExcited, AtmosDirection blockDirection, bool isSpace)
			{
				this.Temperature = temperature;
				this.Moles = moles;
				this.PressureDirection = pressureDirection;
				this.LastPressureDirection = lastPressureDirection;
				this.InExcitedGroup = inExcited;
				this.BlockDirection = blockDirection;
				this.IsSpace = isSpace;
			}

			// Token: 0x040019E8 RID: 6632
			public readonly float Temperature;

			// Token: 0x040019E9 RID: 6633
			public readonly float[] Moles;

			// Token: 0x040019EA RID: 6634
			public readonly AtmosDirection PressureDirection;

			// Token: 0x040019EB RID: 6635
			public readonly AtmosDirection LastPressureDirection;

			// Token: 0x040019EC RID: 6636
			public readonly int InExcitedGroup;

			// Token: 0x040019ED RID: 6637
			public readonly AtmosDirection BlockDirection;

			// Token: 0x040019EE RID: 6638
			public readonly bool IsSpace;
		}

		// Token: 0x02000866 RID: 2150
		[NullableContext(1)]
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class AtmosDebugOverlayMessage : EntityEventArgs
		{
			// Token: 0x17000547 RID: 1351
			// (get) Token: 0x060019D1 RID: 6609 RVA: 0x00051791 File Offset: 0x0004F991
			public EntityUid GridId { get; }

			// Token: 0x17000548 RID: 1352
			// (get) Token: 0x060019D2 RID: 6610 RVA: 0x00051799 File Offset: 0x0004F999
			public Vector2i BaseIdx { get; }

			// Token: 0x17000549 RID: 1353
			// (get) Token: 0x060019D3 RID: 6611 RVA: 0x000517A1 File Offset: 0x0004F9A1
			public SharedAtmosDebugOverlaySystem.AtmosDebugOverlayData[] OverlayData { get; }

			// Token: 0x060019D4 RID: 6612 RVA: 0x000517A9 File Offset: 0x0004F9A9
			public AtmosDebugOverlayMessage(EntityUid gridIndices, Vector2i baseIdx, SharedAtmosDebugOverlaySystem.AtmosDebugOverlayData[] overlayData)
			{
				this.GridId = gridIndices;
				this.BaseIdx = baseIdx;
				this.OverlayData = overlayData;
			}
		}

		// Token: 0x02000867 RID: 2151
		[NetSerializable]
		[Serializable]
		public sealed class AtmosDebugOverlayDisableMessage : EntityEventArgs
		{
		}
	}
}
