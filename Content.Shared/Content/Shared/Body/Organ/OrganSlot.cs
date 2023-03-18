using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Shared.Body.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Body.Organ
{
	// Token: 0x02000661 RID: 1633
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Access(new Type[]
	{
		typeof(SharedBodySystem)
	})]
	[DataRecord]
	[Serializable]
	public sealed class OrganSlot : IEquatable<OrganSlot>
	{
		// Token: 0x06001402 RID: 5122 RVA: 0x00043131 File Offset: 0x00041331
		public OrganSlot(string Id, EntityUid Parent)
		{
			this.Id = Id;
			this.Parent = Parent;
			base..ctor();
		}

		// Token: 0x17000401 RID: 1025
		// (get) Token: 0x06001403 RID: 5123 RVA: 0x00043147 File Offset: 0x00041347
		[CompilerGenerated]
		private Type EqualityContract
		{
			[CompilerGenerated]
			get
			{
				return typeof(OrganSlot);
			}
		}

		// Token: 0x17000402 RID: 1026
		// (get) Token: 0x06001404 RID: 5124 RVA: 0x00043153 File Offset: 0x00041353
		// (set) Token: 0x06001405 RID: 5125 RVA: 0x0004315B File Offset: 0x0004135B
		public string Id { get; set; }

		// Token: 0x17000403 RID: 1027
		// (get) Token: 0x06001406 RID: 5126 RVA: 0x00043164 File Offset: 0x00041364
		// (set) Token: 0x06001407 RID: 5127 RVA: 0x0004316C File Offset: 0x0004136C
		public EntityUid Parent { get; set; }

		// Token: 0x17000404 RID: 1028
		// (get) Token: 0x06001408 RID: 5128 RVA: 0x00043175 File Offset: 0x00041375
		// (set) Token: 0x06001409 RID: 5129 RVA: 0x0004317D File Offset: 0x0004137D
		public EntityUid? Child { get; set; }

		// Token: 0x0600140A RID: 5130 RVA: 0x00043186 File Offset: 0x00041386
		public void Deconstruct(out EntityUid? child, out string id, out EntityUid parent)
		{
			child = this.Child;
			id = this.Id;
			parent = this.Parent;
		}

		// Token: 0x0600140B RID: 5131 RVA: 0x000431A8 File Offset: 0x000413A8
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("OrganSlot");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x0600140C RID: 5132 RVA: 0x000431F4 File Offset: 0x000413F4
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			RuntimeHelpers.EnsureSufficientExecutionStack();
			builder.Append("Id = ");
			builder.Append(this.Id);
			builder.Append(", Parent = ");
			builder.Append(this.Parent.ToString());
			builder.Append(", Child = ");
			builder.Append(this.Child.ToString());
			return true;
		}

		// Token: 0x0600140D RID: 5133 RVA: 0x0004326E File Offset: 0x0004146E
		[NullableContext(2)]
		[CompilerGenerated]
		public static bool operator !=(OrganSlot left, OrganSlot right)
		{
			return !(left == right);
		}

		// Token: 0x0600140E RID: 5134 RVA: 0x0004327A File Offset: 0x0004147A
		[NullableContext(2)]
		[CompilerGenerated]
		public static bool operator ==(OrganSlot left, OrganSlot right)
		{
			return left == right || (left != null && left.Equals(right));
		}

		// Token: 0x0600140F RID: 5135 RVA: 0x00043290 File Offset: 0x00041490
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return ((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<Id>k__BackingField)) * -1521134295 + EqualityComparer<EntityUid>.Default.GetHashCode(this.<Parent>k__BackingField)) * -1521134295 + EqualityComparer<EntityUid?>.Default.GetHashCode(this.<Child>k__BackingField);
		}

		// Token: 0x06001410 RID: 5136 RVA: 0x000432F2 File Offset: 0x000414F2
		[NullableContext(2)]
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return this.Equals(obj as OrganSlot);
		}

		// Token: 0x06001411 RID: 5137 RVA: 0x00043300 File Offset: 0x00041500
		[NullableContext(2)]
		[CompilerGenerated]
		public bool Equals(OrganSlot other)
		{
			return this == other || (other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<string>.Default.Equals(this.<Id>k__BackingField, other.<Id>k__BackingField) && EqualityComparer<EntityUid>.Default.Equals(this.<Parent>k__BackingField, other.<Parent>k__BackingField) && EqualityComparer<EntityUid?>.Default.Equals(this.<Child>k__BackingField, other.<Child>k__BackingField));
		}

		// Token: 0x06001413 RID: 5139 RVA: 0x00043379 File Offset: 0x00041579
		[CompilerGenerated]
		private OrganSlot(OrganSlot original)
		{
			this.Id = original.<Id>k__BackingField;
			this.Parent = original.<Parent>k__BackingField;
			this.Child = original.<Child>k__BackingField;
		}

		// Token: 0x06001414 RID: 5140 RVA: 0x000433A5 File Offset: 0x000415A5
		[CompilerGenerated]
		public void Deconstruct(out string Id, out EntityUid Parent)
		{
			Id = this.Id;
			Parent = this.Parent;
		}
	}
}
