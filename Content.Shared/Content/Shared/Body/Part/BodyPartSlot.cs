using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Shared.Body.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Body.Part
{
	// Token: 0x0200065C RID: 1628
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Access(new Type[]
	{
		typeof(SharedBodySystem)
	})]
	[DataRecord]
	[Serializable]
	public sealed class BodyPartSlot : IEquatable<BodyPartSlot>
	{
		// Token: 0x060013EB RID: 5099 RVA: 0x00042DF4 File Offset: 0x00040FF4
		public BodyPartSlot(string Id, EntityUid Parent, BodyPartType? Type)
		{
			this.Id = Id;
			this.Parent = Parent;
			this.Type = Type;
			base..ctor();
		}

		// Token: 0x170003FC RID: 1020
		// (get) Token: 0x060013EC RID: 5100 RVA: 0x00042E11 File Offset: 0x00041011
		[CompilerGenerated]
		private Type EqualityContract
		{
			[CompilerGenerated]
			get
			{
				return typeof(BodyPartSlot);
			}
		}

		// Token: 0x170003FD RID: 1021
		// (get) Token: 0x060013ED RID: 5101 RVA: 0x00042E1D File Offset: 0x0004101D
		// (set) Token: 0x060013EE RID: 5102 RVA: 0x00042E25 File Offset: 0x00041025
		public string Id { get; set; }

		// Token: 0x170003FE RID: 1022
		// (get) Token: 0x060013EF RID: 5103 RVA: 0x00042E2E File Offset: 0x0004102E
		// (set) Token: 0x060013F0 RID: 5104 RVA: 0x00042E36 File Offset: 0x00041036
		public EntityUid Parent { get; set; }

		// Token: 0x170003FF RID: 1023
		// (get) Token: 0x060013F1 RID: 5105 RVA: 0x00042E3F File Offset: 0x0004103F
		// (set) Token: 0x060013F2 RID: 5106 RVA: 0x00042E47 File Offset: 0x00041047
		public BodyPartType? Type { get; set; }

		// Token: 0x17000400 RID: 1024
		// (get) Token: 0x060013F3 RID: 5107 RVA: 0x00042E50 File Offset: 0x00041050
		// (set) Token: 0x060013F4 RID: 5108 RVA: 0x00042E58 File Offset: 0x00041058
		public EntityUid? Child { get; set; }

		// Token: 0x060013F5 RID: 5109 RVA: 0x00042E61 File Offset: 0x00041061
		public void Deconstruct(out EntityUid? child, out string id, out EntityUid parent, out BodyPartType? type)
		{
			child = this.Child;
			id = this.Id;
			parent = this.Parent;
			type = this.Type;
		}

		// Token: 0x060013F6 RID: 5110 RVA: 0x00042E90 File Offset: 0x00041090
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("BodyPartSlot");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x060013F7 RID: 5111 RVA: 0x00042EDC File Offset: 0x000410DC
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			RuntimeHelpers.EnsureSufficientExecutionStack();
			builder.Append("Id = ");
			builder.Append(this.Id);
			builder.Append(", Parent = ");
			builder.Append(this.Parent.ToString());
			builder.Append(", Type = ");
			builder.Append(this.Type.ToString());
			builder.Append(", Child = ");
			builder.Append(this.Child.ToString());
			return true;
		}

		// Token: 0x060013F8 RID: 5112 RVA: 0x00042F7D File Offset: 0x0004117D
		[NullableContext(2)]
		[CompilerGenerated]
		public static bool operator !=(BodyPartSlot left, BodyPartSlot right)
		{
			return !(left == right);
		}

		// Token: 0x060013F9 RID: 5113 RVA: 0x00042F89 File Offset: 0x00041189
		[NullableContext(2)]
		[CompilerGenerated]
		public static bool operator ==(BodyPartSlot left, BodyPartSlot right)
		{
			return left == right || (left != null && left.Equals(right));
		}

		// Token: 0x060013FA RID: 5114 RVA: 0x00042FA0 File Offset: 0x000411A0
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return (((EqualityComparer<System.Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<Id>k__BackingField)) * -1521134295 + EqualityComparer<EntityUid>.Default.GetHashCode(this.<Parent>k__BackingField)) * -1521134295 + EqualityComparer<BodyPartType?>.Default.GetHashCode(this.<Type>k__BackingField)) * -1521134295 + EqualityComparer<EntityUid?>.Default.GetHashCode(this.<Child>k__BackingField);
		}

		// Token: 0x060013FB RID: 5115 RVA: 0x00043019 File Offset: 0x00041219
		[NullableContext(2)]
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return this.Equals(obj as BodyPartSlot);
		}

		// Token: 0x060013FC RID: 5116 RVA: 0x00043028 File Offset: 0x00041228
		[NullableContext(2)]
		[CompilerGenerated]
		public bool Equals(BodyPartSlot other)
		{
			return this == other || (other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<string>.Default.Equals(this.<Id>k__BackingField, other.<Id>k__BackingField) && EqualityComparer<EntityUid>.Default.Equals(this.<Parent>k__BackingField, other.<Parent>k__BackingField) && EqualityComparer<BodyPartType?>.Default.Equals(this.<Type>k__BackingField, other.<Type>k__BackingField) && EqualityComparer<EntityUid?>.Default.Equals(this.<Child>k__BackingField, other.<Child>k__BackingField));
		}

		// Token: 0x060013FE RID: 5118 RVA: 0x000430B9 File Offset: 0x000412B9
		[CompilerGenerated]
		private BodyPartSlot(BodyPartSlot original)
		{
			this.Id = original.<Id>k__BackingField;
			this.Parent = original.<Parent>k__BackingField;
			this.Type = original.<Type>k__BackingField;
			this.Child = original.<Child>k__BackingField;
		}

		// Token: 0x060013FF RID: 5119 RVA: 0x000430F1 File Offset: 0x000412F1
		[CompilerGenerated]
		public void Deconstruct(out string Id, out EntityUid Parent, out BodyPartType? Type)
		{
			Id = this.Id;
			Parent = this.Parent;
			Type = this.Type;
		}
	}
}
