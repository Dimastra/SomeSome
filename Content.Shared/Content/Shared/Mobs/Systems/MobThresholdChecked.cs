using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Shared.Damage;
using Content.Shared.Mobs.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Mobs.Systems
{
	// Token: 0x02000302 RID: 770
	[NullableContext(1)]
	[Nullable(0)]
	[ByRefEvent]
	public readonly struct MobThresholdChecked : IEquatable<MobThresholdChecked>
	{
		// Token: 0x060008D7 RID: 2263 RVA: 0x0001E02F File Offset: 0x0001C22F
		public MobThresholdChecked(EntityUid Target, MobStateComponent MobState, MobThresholdsComponent Threshold, DamageableComponent Damageable)
		{
			this.Target = Target;
			this.MobState = MobState;
			this.Threshold = Threshold;
			this.Damageable = Damageable;
		}

		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x060008D8 RID: 2264 RVA: 0x0001E04E File Offset: 0x0001C24E
		// (set) Token: 0x060008D9 RID: 2265 RVA: 0x0001E056 File Offset: 0x0001C256
		public EntityUid Target { get; set; }

		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x060008DA RID: 2266 RVA: 0x0001E05F File Offset: 0x0001C25F
		// (set) Token: 0x060008DB RID: 2267 RVA: 0x0001E067 File Offset: 0x0001C267
		public MobStateComponent MobState { get; set; }

		// Token: 0x170001AA RID: 426
		// (get) Token: 0x060008DC RID: 2268 RVA: 0x0001E070 File Offset: 0x0001C270
		// (set) Token: 0x060008DD RID: 2269 RVA: 0x0001E078 File Offset: 0x0001C278
		public MobThresholdsComponent Threshold { get; set; }

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x060008DE RID: 2270 RVA: 0x0001E081 File Offset: 0x0001C281
		// (set) Token: 0x060008DF RID: 2271 RVA: 0x0001E089 File Offset: 0x0001C289
		public DamageableComponent Damageable { get; set; }

		// Token: 0x060008E0 RID: 2272 RVA: 0x0001E094 File Offset: 0x0001C294
		[NullableContext(0)]
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("MobThresholdChecked");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x060008E1 RID: 2273 RVA: 0x0001E0E0 File Offset: 0x0001C2E0
		[NullableContext(0)]
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Target = ");
			builder.Append(this.Target.ToString());
			builder.Append(", MobState = ");
			builder.Append(this.MobState);
			builder.Append(", Threshold = ");
			builder.Append(this.Threshold);
			builder.Append(", Damageable = ");
			builder.Append(this.Damageable);
			return true;
		}

		// Token: 0x060008E2 RID: 2274 RVA: 0x0001E160 File Offset: 0x0001C360
		[CompilerGenerated]
		public static bool operator !=(MobThresholdChecked left, MobThresholdChecked right)
		{
			return !(left == right);
		}

		// Token: 0x060008E3 RID: 2275 RVA: 0x0001E16C File Offset: 0x0001C36C
		[CompilerGenerated]
		public static bool operator ==(MobThresholdChecked left, MobThresholdChecked right)
		{
			return left.Equals(right);
		}

		// Token: 0x060008E4 RID: 2276 RVA: 0x0001E178 File Offset: 0x0001C378
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return ((EqualityComparer<EntityUid>.Default.GetHashCode(this.<Target>k__BackingField) * -1521134295 + EqualityComparer<MobStateComponent>.Default.GetHashCode(this.<MobState>k__BackingField)) * -1521134295 + EqualityComparer<MobThresholdsComponent>.Default.GetHashCode(this.<Threshold>k__BackingField)) * -1521134295 + EqualityComparer<DamageableComponent>.Default.GetHashCode(this.<Damageable>k__BackingField);
		}

		// Token: 0x060008E5 RID: 2277 RVA: 0x0001E1DA File Offset: 0x0001C3DA
		[NullableContext(0)]
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is MobThresholdChecked && this.Equals((MobThresholdChecked)obj);
		}

		// Token: 0x060008E6 RID: 2278 RVA: 0x0001E1F4 File Offset: 0x0001C3F4
		[CompilerGenerated]
		public bool Equals(MobThresholdChecked other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.<Target>k__BackingField, other.<Target>k__BackingField) && EqualityComparer<MobStateComponent>.Default.Equals(this.<MobState>k__BackingField, other.<MobState>k__BackingField) && EqualityComparer<MobThresholdsComponent>.Default.Equals(this.<Threshold>k__BackingField, other.<Threshold>k__BackingField) && EqualityComparer<DamageableComponent>.Default.Equals(this.<Damageable>k__BackingField, other.<Damageable>k__BackingField);
		}

		// Token: 0x060008E7 RID: 2279 RVA: 0x0001E261 File Offset: 0x0001C461
		[CompilerGenerated]
		public void Deconstruct(out EntityUid Target, out MobStateComponent MobState, out MobThresholdsComponent Threshold, out DamageableComponent Damageable)
		{
			Target = this.Target;
			MobState = this.MobState;
			Threshold = this.Threshold;
			Damageable = this.Damageable;
		}
	}
}
