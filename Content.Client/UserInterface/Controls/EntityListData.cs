using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Client.UserInterface.Controls
{
	// Token: 0x020000DC RID: 220
	[NullableContext(1)]
	[Nullable(0)]
	public class EntityListData : ListData, IEquatable<EntityListData>
	{
		// Token: 0x06000628 RID: 1576 RVA: 0x0002152E File Offset: 0x0001F72E
		public EntityListData(EntityUid Uid)
		{
			this.Uid = Uid;
			base..ctor();
		}

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x06000629 RID: 1577 RVA: 0x0002153D File Offset: 0x0001F73D
		[CompilerGenerated]
		protected override Type EqualityContract
		{
			[CompilerGenerated]
			get
			{
				return typeof(EntityListData);
			}
		}

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x0600062A RID: 1578 RVA: 0x00021549 File Offset: 0x0001F749
		// (set) Token: 0x0600062B RID: 1579 RVA: 0x00021551 File Offset: 0x0001F751
		public EntityUid Uid { get; set; }

		// Token: 0x0600062C RID: 1580 RVA: 0x0002155C File Offset: 0x0001F75C
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("EntityListData");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x0600062D RID: 1581 RVA: 0x000215A8 File Offset: 0x0001F7A8
		[CompilerGenerated]
		protected override bool PrintMembers(StringBuilder builder)
		{
			if (base.PrintMembers(builder))
			{
				builder.Append(", ");
			}
			builder.Append("Uid = ");
			builder.Append(this.Uid.ToString());
			return true;
		}

		// Token: 0x0600062E RID: 1582 RVA: 0x000215F2 File Offset: 0x0001F7F2
		[NullableContext(2)]
		[CompilerGenerated]
		public static bool operator !=(EntityListData left, EntityListData right)
		{
			return !(left == right);
		}

		// Token: 0x0600062F RID: 1583 RVA: 0x000215FE File Offset: 0x0001F7FE
		[NullableContext(2)]
		[CompilerGenerated]
		public static bool operator ==(EntityListData left, EntityListData right)
		{
			return left == right || (left != null && left.Equals(right));
		}

		// Token: 0x06000630 RID: 1584 RVA: 0x00021612 File Offset: 0x0001F812
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return base.GetHashCode() * -1521134295 + EqualityComparer<EntityUid>.Default.GetHashCode(this.<Uid>k__BackingField);
		}

		// Token: 0x06000631 RID: 1585 RVA: 0x00021631 File Offset: 0x0001F831
		[NullableContext(2)]
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return this.Equals(obj as EntityListData);
		}

		// Token: 0x06000632 RID: 1586 RVA: 0x0002163F File Offset: 0x0001F83F
		[NullableContext(2)]
		[CompilerGenerated]
		public sealed override bool Equals(ListData other)
		{
			return this.Equals(other);
		}

		// Token: 0x06000633 RID: 1587 RVA: 0x00021648 File Offset: 0x0001F848
		[NullableContext(2)]
		[CompilerGenerated]
		public virtual bool Equals(EntityListData other)
		{
			return this == other || (base.Equals(other) && EqualityComparer<EntityUid>.Default.Equals(this.<Uid>k__BackingField, other.<Uid>k__BackingField));
		}

		// Token: 0x06000635 RID: 1589 RVA: 0x00021679 File Offset: 0x0001F879
		[CompilerGenerated]
		protected EntityListData(EntityListData original) : base(original)
		{
			this.Uid = original.<Uid>k__BackingField;
		}

		// Token: 0x06000636 RID: 1590 RVA: 0x0002168E File Offset: 0x0001F88E
		[CompilerGenerated]
		public void Deconstruct(out EntityUid Uid)
		{
			Uid = this.Uid;
		}
	}
}
