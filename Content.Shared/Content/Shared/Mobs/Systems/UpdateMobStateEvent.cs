using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Shared.Mobs.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Mobs.Systems
{
	// Token: 0x02000300 RID: 768
	[NullableContext(1)]
	[Nullable(0)]
	[ByRefEvent]
	public struct UpdateMobStateEvent : IEquatable<UpdateMobStateEvent>
	{
		// Token: 0x060008B1 RID: 2225 RVA: 0x0001D6A6 File Offset: 0x0001B8A6
		public UpdateMobStateEvent(EntityUid Target, MobStateComponent Component, MobState State, EntityUid? Origin = null)
		{
			this.Target = Target;
			this.Component = Component;
			this.State = State;
			this.Origin = Origin;
		}

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x060008B2 RID: 2226 RVA: 0x0001D6C5 File Offset: 0x0001B8C5
		// (set) Token: 0x060008B3 RID: 2227 RVA: 0x0001D6CD File Offset: 0x0001B8CD
		public EntityUid Target { readonly get; set; }

		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x060008B4 RID: 2228 RVA: 0x0001D6D6 File Offset: 0x0001B8D6
		// (set) Token: 0x060008B5 RID: 2229 RVA: 0x0001D6DE File Offset: 0x0001B8DE
		public MobStateComponent Component { readonly get; set; }

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x060008B6 RID: 2230 RVA: 0x0001D6E7 File Offset: 0x0001B8E7
		// (set) Token: 0x060008B7 RID: 2231 RVA: 0x0001D6EF File Offset: 0x0001B8EF
		public MobState State { readonly get; set; }

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x060008B8 RID: 2232 RVA: 0x0001D6F8 File Offset: 0x0001B8F8
		// (set) Token: 0x060008B9 RID: 2233 RVA: 0x0001D700 File Offset: 0x0001B900
		public EntityUid? Origin { readonly get; set; }

		// Token: 0x060008BA RID: 2234 RVA: 0x0001D70C File Offset: 0x0001B90C
		[NullableContext(0)]
		[CompilerGenerated]
		public override readonly string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("UpdateMobStateEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x060008BB RID: 2235 RVA: 0x0001D758 File Offset: 0x0001B958
		[NullableContext(0)]
		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Target = ");
			builder.Append(this.Target.ToString());
			builder.Append(", Component = ");
			builder.Append(this.Component);
			builder.Append(", State = ");
			builder.Append(this.State.ToString());
			builder.Append(", Origin = ");
			builder.Append(this.Origin.ToString());
			return true;
		}

		// Token: 0x060008BC RID: 2236 RVA: 0x0001D7F4 File Offset: 0x0001B9F4
		[CompilerGenerated]
		public static bool operator !=(UpdateMobStateEvent left, UpdateMobStateEvent right)
		{
			return !(left == right);
		}

		// Token: 0x060008BD RID: 2237 RVA: 0x0001D800 File Offset: 0x0001BA00
		[CompilerGenerated]
		public static bool operator ==(UpdateMobStateEvent left, UpdateMobStateEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x060008BE RID: 2238 RVA: 0x0001D80C File Offset: 0x0001BA0C
		[CompilerGenerated]
		public override readonly int GetHashCode()
		{
			return ((EqualityComparer<EntityUid>.Default.GetHashCode(this.<Target>k__BackingField) * -1521134295 + EqualityComparer<MobStateComponent>.Default.GetHashCode(this.<Component>k__BackingField)) * -1521134295 + EqualityComparer<MobState>.Default.GetHashCode(this.<State>k__BackingField)) * -1521134295 + EqualityComparer<EntityUid?>.Default.GetHashCode(this.<Origin>k__BackingField);
		}

		// Token: 0x060008BF RID: 2239 RVA: 0x0001D86E File Offset: 0x0001BA6E
		[NullableContext(0)]
		[CompilerGenerated]
		public override readonly bool Equals(object obj)
		{
			return obj is UpdateMobStateEvent && this.Equals((UpdateMobStateEvent)obj);
		}

		// Token: 0x060008C0 RID: 2240 RVA: 0x0001D888 File Offset: 0x0001BA88
		[CompilerGenerated]
		public readonly bool Equals(UpdateMobStateEvent other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.<Target>k__BackingField, other.<Target>k__BackingField) && EqualityComparer<MobStateComponent>.Default.Equals(this.<Component>k__BackingField, other.<Component>k__BackingField) && EqualityComparer<MobState>.Default.Equals(this.<State>k__BackingField, other.<State>k__BackingField) && EqualityComparer<EntityUid?>.Default.Equals(this.<Origin>k__BackingField, other.<Origin>k__BackingField);
		}

		// Token: 0x060008C1 RID: 2241 RVA: 0x0001D8F5 File Offset: 0x0001BAF5
		[CompilerGenerated]
		public readonly void Deconstruct(out EntityUid Target, out MobStateComponent Component, out MobState State, out EntityUid? Origin)
		{
			Target = this.Target;
			Component = this.Component;
			State = this.State;
			Origin = this.Origin;
		}
	}
}
