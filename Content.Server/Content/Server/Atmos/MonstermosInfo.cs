using System;
using Content.Shared.Atmos;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos
{
	// Token: 0x02000736 RID: 1846
	public struct MonstermosInfo
	{
		// Token: 0x170005C0 RID: 1472
		public float this[AtmosDirection direction]
		{
			get
			{
				switch (direction)
				{
				case AtmosDirection.North:
					return this.TransferDirectionNorth;
				case AtmosDirection.South:
					return this.TransferDirectionSouth;
				case AtmosDirection.North | AtmosDirection.South:
					break;
				case AtmosDirection.East:
					return this.TransferDirectionEast;
				default:
					if (direction == AtmosDirection.West)
					{
						return this.TransferDirectionWest;
					}
					break;
				}
				throw new ArgumentOutOfRangeException("direction");
			}
			set
			{
				switch (direction)
				{
				case AtmosDirection.North:
					this.TransferDirectionNorth = value;
					return;
				case AtmosDirection.South:
					this.TransferDirectionSouth = value;
					return;
				case AtmosDirection.North | AtmosDirection.South:
					break;
				case AtmosDirection.East:
					this.TransferDirectionEast = value;
					return;
				default:
					if (direction == AtmosDirection.West)
					{
						this.TransferDirectionWest = value;
						return;
					}
					break;
				}
				throw new ArgumentOutOfRangeException("direction");
			}
		}

		// Token: 0x170005C1 RID: 1473
		public float this[int index]
		{
			get
			{
				return this[(AtmosDirection)(1 << index)];
			}
			set
			{
				this[(AtmosDirection)(1 << index)] = value;
			}
		}

		// Token: 0x0400180E RID: 6158
		[ViewVariables]
		public int LastCycle;

		// Token: 0x0400180F RID: 6159
		[ViewVariables]
		public long LastQueueCycle;

		// Token: 0x04001810 RID: 6160
		[ViewVariables]
		public long LastSlowQueueCycle;

		// Token: 0x04001811 RID: 6161
		[ViewVariables]
		public float MoleDelta;

		// Token: 0x04001812 RID: 6162
		[ViewVariables]
		public float TransferDirectionEast;

		// Token: 0x04001813 RID: 6163
		[ViewVariables]
		public float TransferDirectionWest;

		// Token: 0x04001814 RID: 6164
		[ViewVariables]
		public float TransferDirectionNorth;

		// Token: 0x04001815 RID: 6165
		[ViewVariables]
		public float TransferDirectionSouth;

		// Token: 0x04001816 RID: 6166
		[ViewVariables]
		public float CurrentTransferAmount;

		// Token: 0x04001817 RID: 6167
		[ViewVariables]
		public AtmosDirection CurrentTransferDirection;

		// Token: 0x04001818 RID: 6168
		[ViewVariables]
		public bool FastDone;
	}
}
