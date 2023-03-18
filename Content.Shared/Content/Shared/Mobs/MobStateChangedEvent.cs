using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Shared.Mobs.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Mobs
{
	// Token: 0x020002FC RID: 764
	[NullableContext(1)]
	[Nullable(0)]
	public struct MobStateChangedEvent : IEquatable<MobStateChangedEvent>
	{
		// Token: 0x06000885 RID: 2181 RVA: 0x0001CD0B File Offset: 0x0001AF0B
		public MobStateChangedEvent(EntityUid Target, MobStateComponent Component, MobState OldMobState, MobState NewMobState, EntityUid? Origin = null)
		{
			this.Target = Target;
			this.Component = Component;
			this.OldMobState = OldMobState;
			this.NewMobState = NewMobState;
			this.Origin = Origin;
		}

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x06000886 RID: 2182 RVA: 0x0001CD32 File Offset: 0x0001AF32
		// (set) Token: 0x06000887 RID: 2183 RVA: 0x0001CD3A File Offset: 0x0001AF3A
		public EntityUid Target { readonly get; set; }

		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x06000888 RID: 2184 RVA: 0x0001CD43 File Offset: 0x0001AF43
		// (set) Token: 0x06000889 RID: 2185 RVA: 0x0001CD4B File Offset: 0x0001AF4B
		public MobStateComponent Component { readonly get; set; }

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x0600088A RID: 2186 RVA: 0x0001CD54 File Offset: 0x0001AF54
		// (set) Token: 0x0600088B RID: 2187 RVA: 0x0001CD5C File Offset: 0x0001AF5C
		public MobState OldMobState { readonly get; set; }

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x0600088C RID: 2188 RVA: 0x0001CD65 File Offset: 0x0001AF65
		// (set) Token: 0x0600088D RID: 2189 RVA: 0x0001CD6D File Offset: 0x0001AF6D
		public MobState NewMobState { readonly get; set; }

		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x0600088E RID: 2190 RVA: 0x0001CD76 File Offset: 0x0001AF76
		// (set) Token: 0x0600088F RID: 2191 RVA: 0x0001CD7E File Offset: 0x0001AF7E
		public EntityUid? Origin { readonly get; set; }

		// Token: 0x06000890 RID: 2192 RVA: 0x0001CD88 File Offset: 0x0001AF88
		[NullableContext(0)]
		[CompilerGenerated]
		public override readonly string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("MobStateChangedEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000891 RID: 2193 RVA: 0x0001CDD4 File Offset: 0x0001AFD4
		[NullableContext(0)]
		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Target = ");
			builder.Append(this.Target.ToString());
			builder.Append(", Component = ");
			builder.Append(this.Component);
			builder.Append(", OldMobState = ");
			builder.Append(this.OldMobState.ToString());
			builder.Append(", NewMobState = ");
			builder.Append(this.NewMobState.ToString());
			builder.Append(", Origin = ");
			builder.Append(this.Origin.ToString());
			return true;
		}

		// Token: 0x06000892 RID: 2194 RVA: 0x0001CE97 File Offset: 0x0001B097
		[CompilerGenerated]
		public static bool operator !=(MobStateChangedEvent left, MobStateChangedEvent right)
		{
			return !(left == right);
		}

		// Token: 0x06000893 RID: 2195 RVA: 0x0001CEA3 File Offset: 0x0001B0A3
		[CompilerGenerated]
		public static bool operator ==(MobStateChangedEvent left, MobStateChangedEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x06000894 RID: 2196 RVA: 0x0001CEB0 File Offset: 0x0001B0B0
		[CompilerGenerated]
		public override readonly int GetHashCode()
		{
			return (((EqualityComparer<EntityUid>.Default.GetHashCode(this.<Target>k__BackingField) * -1521134295 + EqualityComparer<MobStateComponent>.Default.GetHashCode(this.<Component>k__BackingField)) * -1521134295 + EqualityComparer<MobState>.Default.GetHashCode(this.<OldMobState>k__BackingField)) * -1521134295 + EqualityComparer<MobState>.Default.GetHashCode(this.<NewMobState>k__BackingField)) * -1521134295 + EqualityComparer<EntityUid?>.Default.GetHashCode(this.<Origin>k__BackingField);
		}

		// Token: 0x06000895 RID: 2197 RVA: 0x0001CF29 File Offset: 0x0001B129
		[NullableContext(0)]
		[CompilerGenerated]
		public override readonly bool Equals(object obj)
		{
			return obj is MobStateChangedEvent && this.Equals((MobStateChangedEvent)obj);
		}

		// Token: 0x06000896 RID: 2198 RVA: 0x0001CF44 File Offset: 0x0001B144
		[CompilerGenerated]
		public readonly bool Equals(MobStateChangedEvent other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.<Target>k__BackingField, other.<Target>k__BackingField) && EqualityComparer<MobStateComponent>.Default.Equals(this.<Component>k__BackingField, other.<Component>k__BackingField) && EqualityComparer<MobState>.Default.Equals(this.<OldMobState>k__BackingField, other.<OldMobState>k__BackingField) && EqualityComparer<MobState>.Default.Equals(this.<NewMobState>k__BackingField, other.<NewMobState>k__BackingField) && EqualityComparer<EntityUid?>.Default.Equals(this.<Origin>k__BackingField, other.<Origin>k__BackingField);
		}

		// Token: 0x06000897 RID: 2199 RVA: 0x0001CFC9 File Offset: 0x0001B1C9
		[CompilerGenerated]
		public readonly void Deconstruct(out EntityUid Target, out MobStateComponent Component, out MobState OldMobState, out MobState NewMobState, out EntityUid? Origin)
		{
			Target = this.Target;
			Component = this.Component;
			OldMobState = this.OldMobState;
			NewMobState = this.NewMobState;
			Origin = this.Origin;
		}
	}
}
