using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;

namespace Content.Shared.StatusEffect
{
	// Token: 0x02000158 RID: 344
	[NetSerializable]
	[Serializable]
	public sealed class StatusEffectState
	{
		// Token: 0x06000422 RID: 1058 RVA: 0x000106D3 File Offset: 0x0000E8D3
		public StatusEffectState(ValueTuple<TimeSpan, TimeSpan> cooldown, bool refresh, [Nullable(2)] string relevantComponent = null)
		{
			this.Cooldown = cooldown;
			this.CooldownRefresh = refresh;
			this.RelevantComponent = relevantComponent;
		}

		// Token: 0x06000423 RID: 1059 RVA: 0x000106F8 File Offset: 0x0000E8F8
		[NullableContext(1)]
		public StatusEffectState(StatusEffectState toCopy)
		{
			this.Cooldown = new ValueTuple<TimeSpan, TimeSpan>(toCopy.Cooldown.Item1, toCopy.Cooldown.Item2);
			this.CooldownRefresh = toCopy.CooldownRefresh;
			this.RelevantComponent = toCopy.RelevantComponent;
		}

		// Token: 0x040003F6 RID: 1014
		[ViewVariables]
		public ValueTuple<TimeSpan, TimeSpan> Cooldown;

		// Token: 0x040003F7 RID: 1015
		[ViewVariables]
		public bool CooldownRefresh = true;

		// Token: 0x040003F8 RID: 1016
		[Nullable(2)]
		[ViewVariables]
		public string RelevantComponent;
	}
}
