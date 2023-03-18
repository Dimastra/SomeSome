using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Dice
{
	// Token: 0x0200050B RID: 1291
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(SharedDiceSystem)
	})]
	public sealed class DiceComponent : Component
	{
		// Token: 0x17000320 RID: 800
		// (get) Token: 0x06000FB0 RID: 4016 RVA: 0x00032B3B File Offset: 0x00030D3B
		[DataField("sound", false, 1, false, false, null)]
		public SoundSpecifier Sound { get; } = new SoundCollectionSpecifier("Dice", null);

		// Token: 0x17000321 RID: 801
		// (get) Token: 0x06000FB1 RID: 4017 RVA: 0x00032B43 File Offset: 0x00030D43
		[DataField("multiplier", false, 1, false, false, null)]
		public int Multiplier { get; } = 1;

		// Token: 0x17000322 RID: 802
		// (get) Token: 0x06000FB2 RID: 4018 RVA: 0x00032B4B File Offset: 0x00030D4B
		[DataField("offset", false, 1, false, false, null)]
		public int Offset { get; }

		// Token: 0x17000323 RID: 803
		// (get) Token: 0x06000FB3 RID: 4019 RVA: 0x00032B53 File Offset: 0x00030D53
		[DataField("sides", false, 1, false, false, null)]
		public int Sides { get; } = 20;

		// Token: 0x17000324 RID: 804
		// (get) Token: 0x06000FB4 RID: 4020 RVA: 0x00032B5B File Offset: 0x00030D5B
		// (set) Token: 0x06000FB5 RID: 4021 RVA: 0x00032B63 File Offset: 0x00030D63
		[DataField("currentValue", false, 1, false, false, null)]
		public int CurrentValue { get; set; } = 20;

		// Token: 0x02000833 RID: 2099
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		public sealed class DiceState : ComponentState
		{
			// Token: 0x06001908 RID: 6408 RVA: 0x0004F6D8 File Offset: 0x0004D8D8
			public DiceState(int value)
			{
				this.CurrentValue = value;
			}

			// Token: 0x0400192C RID: 6444
			public readonly int CurrentValue;
		}
	}
}
