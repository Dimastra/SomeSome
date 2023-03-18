using System;
using System.Runtime.CompilerServices;
using Content.Shared.Speech.EntitySystems;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Drunk
{
	// Token: 0x020004D2 RID: 1234
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedDrunkSystem : EntitySystem
	{
		// Token: 0x06000EEC RID: 3820 RVA: 0x0002FEC8 File Offset: 0x0002E0C8
		[NullableContext(2)]
		public void TryApplyDrunkenness(EntityUid uid, float boozePower, bool applySlur = true, StatusEffectsComponent status = null)
		{
			if (!base.Resolve<StatusEffectsComponent>(uid, ref status, false))
			{
				return;
			}
			if (applySlur)
			{
				this._slurredSystem.DoSlur(uid, TimeSpan.FromSeconds((double)boozePower), status);
			}
			if (!this._statusEffectsSystem.HasStatusEffect(uid, "Drunk", status))
			{
				this._statusEffectsSystem.TryAddStatusEffect<DrunkComponent>(uid, "Drunk", TimeSpan.FromSeconds((double)boozePower), true, status);
				return;
			}
			this._statusEffectsSystem.TryAddTime(uid, "Drunk", TimeSpan.FromSeconds((double)boozePower), status);
		}

		// Token: 0x04000DFA RID: 3578
		public const string DrunkKey = "Drunk";

		// Token: 0x04000DFB RID: 3579
		[Dependency]
		private readonly StatusEffectsSystem _statusEffectsSystem;

		// Token: 0x04000DFC RID: 3580
		[Dependency]
		private readonly SharedSlurredSystem _slurredSystem;
	}
}
