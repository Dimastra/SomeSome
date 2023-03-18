using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Server.Atmos.Components;
using Robust.Shared.GameObjects;

namespace Content.Server.Atmos.EntitySystems
{
	// Token: 0x0200078F RID: 1935
	[NullableContext(1)]
	[Nullable(0)]
	[ByRefEvent]
	public readonly struct AirtightChanged : IEquatable<AirtightChanged>
	{
		// Token: 0x06002929 RID: 10537 RVA: 0x000D67B3 File Offset: 0x000D49B3
		public AirtightChanged(EntityUid Entity, AirtightComponent Airtight)
		{
			this.Entity = Entity;
			this.Airtight = Airtight;
		}

		// Token: 0x17000647 RID: 1607
		// (get) Token: 0x0600292A RID: 10538 RVA: 0x000D67C3 File Offset: 0x000D49C3
		// (set) Token: 0x0600292B RID: 10539 RVA: 0x000D67CB File Offset: 0x000D49CB
		public EntityUid Entity { get; set; }

		// Token: 0x17000648 RID: 1608
		// (get) Token: 0x0600292C RID: 10540 RVA: 0x000D67D4 File Offset: 0x000D49D4
		// (set) Token: 0x0600292D RID: 10541 RVA: 0x000D67DC File Offset: 0x000D49DC
		public AirtightComponent Airtight { get; set; }

		// Token: 0x0600292E RID: 10542 RVA: 0x000D67E8 File Offset: 0x000D49E8
		[NullableContext(0)]
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("AirtightChanged");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x0600292F RID: 10543 RVA: 0x000D6834 File Offset: 0x000D4A34
		[NullableContext(0)]
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Entity = ");
			builder.Append(this.Entity.ToString());
			builder.Append(", Airtight = ");
			builder.Append(this.Airtight);
			return true;
		}

		// Token: 0x06002930 RID: 10544 RVA: 0x000D6882 File Offset: 0x000D4A82
		[CompilerGenerated]
		public static bool operator !=(AirtightChanged left, AirtightChanged right)
		{
			return !(left == right);
		}

		// Token: 0x06002931 RID: 10545 RVA: 0x000D688E File Offset: 0x000D4A8E
		[CompilerGenerated]
		public static bool operator ==(AirtightChanged left, AirtightChanged right)
		{
			return left.Equals(right);
		}

		// Token: 0x06002932 RID: 10546 RVA: 0x000D6898 File Offset: 0x000D4A98
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return EqualityComparer<EntityUid>.Default.GetHashCode(this.<Entity>k__BackingField) * -1521134295 + EqualityComparer<AirtightComponent>.Default.GetHashCode(this.<Airtight>k__BackingField);
		}

		// Token: 0x06002933 RID: 10547 RVA: 0x000D68C1 File Offset: 0x000D4AC1
		[NullableContext(0)]
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is AirtightChanged && this.Equals((AirtightChanged)obj);
		}

		// Token: 0x06002934 RID: 10548 RVA: 0x000D68D9 File Offset: 0x000D4AD9
		[CompilerGenerated]
		public bool Equals(AirtightChanged other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.<Entity>k__BackingField, other.<Entity>k__BackingField) && EqualityComparer<AirtightComponent>.Default.Equals(this.<Airtight>k__BackingField, other.<Airtight>k__BackingField);
		}

		// Token: 0x06002935 RID: 10549 RVA: 0x000D690B File Offset: 0x000D4B0B
		[CompilerGenerated]
		public void Deconstruct(out EntityUid Entity, out AirtightComponent Airtight)
		{
			Entity = this.Entity;
			Airtight = this.Airtight;
		}
	}
}
