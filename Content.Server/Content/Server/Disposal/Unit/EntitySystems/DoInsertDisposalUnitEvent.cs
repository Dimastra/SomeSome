using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Server.Disposal.Unit.EntitySystems
{
	// Token: 0x02000551 RID: 1361
	[NullableContext(1)]
	[Nullable(0)]
	public class DoInsertDisposalUnitEvent : IEquatable<DoInsertDisposalUnitEvent>
	{
		// Token: 0x06001CA3 RID: 7331 RVA: 0x00099500 File Offset: 0x00097700
		public DoInsertDisposalUnitEvent(EntityUid? User, EntityUid ToInsert, EntityUid Unit)
		{
			this.User = User;
			this.ToInsert = ToInsert;
			this.Unit = Unit;
			base..ctor();
		}

		// Token: 0x17000440 RID: 1088
		// (get) Token: 0x06001CA4 RID: 7332 RVA: 0x0009951D File Offset: 0x0009771D
		[CompilerGenerated]
		protected virtual Type EqualityContract
		{
			[CompilerGenerated]
			get
			{
				return typeof(DoInsertDisposalUnitEvent);
			}
		}

		// Token: 0x17000441 RID: 1089
		// (get) Token: 0x06001CA5 RID: 7333 RVA: 0x00099529 File Offset: 0x00097729
		// (set) Token: 0x06001CA6 RID: 7334 RVA: 0x00099531 File Offset: 0x00097731
		public EntityUid? User { get; set; }

		// Token: 0x17000442 RID: 1090
		// (get) Token: 0x06001CA7 RID: 7335 RVA: 0x0009953A File Offset: 0x0009773A
		// (set) Token: 0x06001CA8 RID: 7336 RVA: 0x00099542 File Offset: 0x00097742
		public EntityUid ToInsert { get; set; }

		// Token: 0x17000443 RID: 1091
		// (get) Token: 0x06001CA9 RID: 7337 RVA: 0x0009954B File Offset: 0x0009774B
		// (set) Token: 0x06001CAA RID: 7338 RVA: 0x00099553 File Offset: 0x00097753
		public EntityUid Unit { get; set; }

		// Token: 0x06001CAB RID: 7339 RVA: 0x0009955C File Offset: 0x0009775C
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("DoInsertDisposalUnitEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06001CAC RID: 7340 RVA: 0x000995A8 File Offset: 0x000977A8
		[CompilerGenerated]
		protected virtual bool PrintMembers(StringBuilder builder)
		{
			RuntimeHelpers.EnsureSufficientExecutionStack();
			builder.Append("User = ");
			builder.Append(this.User.ToString());
			builder.Append(", ToInsert = ");
			builder.Append(this.ToInsert.ToString());
			builder.Append(", Unit = ");
			builder.Append(this.Unit.ToString());
			return true;
		}

		// Token: 0x06001CAD RID: 7341 RVA: 0x00099630 File Offset: 0x00097830
		[NullableContext(2)]
		[CompilerGenerated]
		public static bool operator !=(DoInsertDisposalUnitEvent left, DoInsertDisposalUnitEvent right)
		{
			return !(left == right);
		}

		// Token: 0x06001CAE RID: 7342 RVA: 0x0009963C File Offset: 0x0009783C
		[NullableContext(2)]
		[CompilerGenerated]
		public static bool operator ==(DoInsertDisposalUnitEvent left, DoInsertDisposalUnitEvent right)
		{
			return left == right || (left != null && left.Equals(right));
		}

		// Token: 0x06001CAF RID: 7343 RVA: 0x00099650 File Offset: 0x00097850
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return ((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<EntityUid?>.Default.GetHashCode(this.<User>k__BackingField)) * -1521134295 + EqualityComparer<EntityUid>.Default.GetHashCode(this.<ToInsert>k__BackingField)) * -1521134295 + EqualityComparer<EntityUid>.Default.GetHashCode(this.<Unit>k__BackingField);
		}

		// Token: 0x06001CB0 RID: 7344 RVA: 0x000996B2 File Offset: 0x000978B2
		[NullableContext(2)]
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return this.Equals(obj as DoInsertDisposalUnitEvent);
		}

		// Token: 0x06001CB1 RID: 7345 RVA: 0x000996C0 File Offset: 0x000978C0
		[NullableContext(2)]
		[CompilerGenerated]
		public virtual bool Equals(DoInsertDisposalUnitEvent other)
		{
			return this == other || (other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<EntityUid?>.Default.Equals(this.<User>k__BackingField, other.<User>k__BackingField) && EqualityComparer<EntityUid>.Default.Equals(this.<ToInsert>k__BackingField, other.<ToInsert>k__BackingField) && EqualityComparer<EntityUid>.Default.Equals(this.<Unit>k__BackingField, other.<Unit>k__BackingField));
		}

		// Token: 0x06001CB3 RID: 7347 RVA: 0x00099739 File Offset: 0x00097939
		[CompilerGenerated]
		protected DoInsertDisposalUnitEvent(DoInsertDisposalUnitEvent original)
		{
			this.User = original.<User>k__BackingField;
			this.ToInsert = original.<ToInsert>k__BackingField;
			this.Unit = original.<Unit>k__BackingField;
		}

		// Token: 0x06001CB4 RID: 7348 RVA: 0x00099765 File Offset: 0x00097965
		[CompilerGenerated]
		public void Deconstruct(out EntityUid? User, out EntityUid ToInsert, out EntityUid Unit)
		{
			User = this.User;
			ToInsert = this.ToInsert;
			Unit = this.Unit;
		}
	}
}
