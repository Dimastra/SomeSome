using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Body.Prototypes
{
	// Token: 0x02000654 RID: 1620
	[NullableContext(1)]
	[Nullable(0)]
	[DataRecord]
	public sealed class BodyPrototypeSlot : IEquatable<BodyPrototypeSlot>
	{
		// Token: 0x170003F5 RID: 1013
		// (get) Token: 0x060013BB RID: 5051 RVA: 0x000421AD File Offset: 0x000403AD
		[CompilerGenerated]
		private Type EqualityContract
		{
			[CompilerGenerated]
			get
			{
				return typeof(BodyPrototypeSlot);
			}
		}

		// Token: 0x060013BC RID: 5052 RVA: 0x000421BC File Offset: 0x000403BC
		[NullableContext(2)]
		public BodyPrototypeSlot(string part, [Nullable(new byte[]
		{
			2,
			1
		})] HashSet<string> connections, [Nullable(new byte[]
		{
			2,
			1,
			1
		})] Dictionary<string, string> organs)
		{
			this.Connections = new HashSet<string>();
			this.Organs = new Dictionary<string, string>();
			base..ctor();
			this.Part = part;
			this.Connections = (connections ?? new HashSet<string>());
			this.Organs = (organs ?? new Dictionary<string, string>());
		}

		// Token: 0x060013BD RID: 5053 RVA: 0x0004220C File Offset: 0x0004040C
		public void Deconstruct([Nullable(2)] out string part, out HashSet<string> connections, out Dictionary<string, string> organs)
		{
			part = this.Part;
			connections = this.Connections;
			organs = this.Organs;
		}

		// Token: 0x060013BE RID: 5054 RVA: 0x00042228 File Offset: 0x00040428
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("BodyPrototypeSlot");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x060013BF RID: 5055 RVA: 0x00042274 File Offset: 0x00040474
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			RuntimeHelpers.EnsureSufficientExecutionStack();
			builder.Append("Part = ");
			builder.Append(this.Part);
			builder.Append(", Connections = ");
			builder.Append(this.Connections);
			builder.Append(", Organs = ");
			builder.Append(this.Organs);
			return true;
		}

		// Token: 0x060013C0 RID: 5056 RVA: 0x000422D2 File Offset: 0x000404D2
		[NullableContext(2)]
		[CompilerGenerated]
		public static bool operator !=(BodyPrototypeSlot left, BodyPrototypeSlot right)
		{
			return !(left == right);
		}

		// Token: 0x060013C1 RID: 5057 RVA: 0x000422DE File Offset: 0x000404DE
		[NullableContext(2)]
		[CompilerGenerated]
		public static bool operator ==(BodyPrototypeSlot left, BodyPrototypeSlot right)
		{
			return left == right || (left != null && left.Equals(right));
		}

		// Token: 0x060013C2 RID: 5058 RVA: 0x000422F4 File Offset: 0x000404F4
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return ((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Part)) * -1521134295 + EqualityComparer<HashSet<string>>.Default.GetHashCode(this.Connections)) * -1521134295 + EqualityComparer<Dictionary<string, string>>.Default.GetHashCode(this.Organs);
		}

		// Token: 0x060013C3 RID: 5059 RVA: 0x00042356 File Offset: 0x00040556
		[NullableContext(2)]
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return this.Equals(obj as BodyPrototypeSlot);
		}

		// Token: 0x060013C4 RID: 5060 RVA: 0x00042364 File Offset: 0x00040564
		[NullableContext(2)]
		[CompilerGenerated]
		public bool Equals(BodyPrototypeSlot other)
		{
			return this == other || (other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<string>.Default.Equals(this.Part, other.Part) && EqualityComparer<HashSet<string>>.Default.Equals(this.Connections, other.Connections) && EqualityComparer<Dictionary<string, string>>.Default.Equals(this.Organs, other.Organs));
		}

		// Token: 0x060013C6 RID: 5062 RVA: 0x000423DD File Offset: 0x000405DD
		[CompilerGenerated]
		private BodyPrototypeSlot(BodyPrototypeSlot original)
		{
			this.Part = original.Part;
			this.Connections = original.Connections;
			this.Organs = original.Organs;
		}

		// Token: 0x04001389 RID: 5001
		[Nullable(2)]
		[DataField("part", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public readonly string Part;

		// Token: 0x0400138A RID: 5002
		public readonly HashSet<string> Connections;

		// Token: 0x0400138B RID: 5003
		public readonly Dictionary<string, string> Organs;
	}
}
