using System;
using System.Runtime.CompilerServices;
using Content.Shared.Disease;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Disease.Effects
{
	// Token: 0x02000568 RID: 1384
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DiseaseGenericStatusEffect : DiseaseEffect
	{
		// Token: 0x06001D4F RID: 7503 RVA: 0x0009C4F0 File Offset: 0x0009A6F0
		public override void Effect(DiseaseEffectArgs args)
		{
			StatusEffectsSystem statusSys = EntitySystem.Get<StatusEffectsSystem>();
			if (this.Type == StatusEffectDiseaseType.Add && this.Component != string.Empty)
			{
				statusSys.TryAddStatusEffect(args.DiseasedEntity, this.Key, TimeSpan.FromSeconds((double)this.Time), this.Refresh, this.Component, null);
				return;
			}
			if (this.Type == StatusEffectDiseaseType.Remove)
			{
				statusSys.TryRemoveTime(args.DiseasedEntity, this.Key, TimeSpan.FromSeconds((double)this.Time), null);
				return;
			}
			if (this.Type == StatusEffectDiseaseType.Set)
			{
				statusSys.TrySetTime(args.DiseasedEntity, this.Key, TimeSpan.FromSeconds((double)this.Time), null);
			}
		}

		// Token: 0x040012B8 RID: 4792
		[DataField("key", false, 1, true, false, null)]
		public string Key;

		// Token: 0x040012B9 RID: 4793
		[DataField("component", false, 1, false, false, null)]
		public string Component = string.Empty;

		// Token: 0x040012BA RID: 4794
		[DataField("time", false, 1, false, false, null)]
		public float Time = 1.01f;

		// Token: 0x040012BB RID: 4795
		[DataField("refresh", false, 1, false, false, null)]
		public bool Refresh;

		// Token: 0x040012BC RID: 4796
		[DataField("type", false, 1, false, false, null)]
		public StatusEffectDiseaseType Type;
	}
}
