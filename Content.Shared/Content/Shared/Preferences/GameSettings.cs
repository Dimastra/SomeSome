using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Preferences
{
	// Token: 0x02000245 RID: 581
	[NetSerializable]
	[Serializable]
	public sealed class GameSettings
	{
		// Token: 0x17000140 RID: 320
		// (get) Token: 0x0600068D RID: 1677 RVA: 0x00017358 File Offset: 0x00015558
		// (set) Token: 0x0600068E RID: 1678 RVA: 0x00017360 File Offset: 0x00015560
		public int MaxCharacterSlots
		{
			get
			{
				return this._maxCharacterSlots;
			}
			set
			{
				this._maxCharacterSlots = value;
			}
		}

		// Token: 0x04000684 RID: 1668
		private int _maxCharacterSlots;
	}
}
