using System;
using System.Runtime.CompilerServices;
using Content.Shared.Dice;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Random;

namespace Content.Server.Dice
{
	// Token: 0x0200057C RID: 1404
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DiceSystem : SharedDiceSystem
	{
		// Token: 0x06001D6F RID: 7535 RVA: 0x0009CC0C File Offset: 0x0009AE0C
		[NullableContext(2)]
		public override void Roll(EntityUid uid, DiceComponent die = null)
		{
			if (!base.Resolve<DiceComponent>(uid, ref die, true))
			{
				return;
			}
			int roll = this._random.Next(1, die.Sides + 1);
			base.SetCurrentSide(uid, roll, die);
			this._popup.PopupEntity(Loc.GetString("dice-component-on-roll-land", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("die", uid),
				new ValueTuple<string, object>("currentSide", die.CurrentValue)
			}), uid, PopupType.Small);
			this._audio.PlayPvs(die.Sound, uid, null);
		}

		// Token: 0x040012E8 RID: 4840
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040012E9 RID: 4841
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x040012EA RID: 4842
		[Dependency]
		private readonly SharedPopupSystem _popup;
	}
}
