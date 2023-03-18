using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Storage.Components
{
	// Token: 0x0200013F RID: 319
	[NullableContext(1)]
	[Nullable(0)]
	[ByRefEvent]
	public readonly struct StorageBeforeCloseEvent : IEquatable<StorageBeforeCloseEvent>
	{
		// Token: 0x060003CF RID: 975 RVA: 0x0000F95B File Offset: 0x0000DB5B
		public StorageBeforeCloseEvent(HashSet<EntityUid> Contents, HashSet<EntityUid> BypassChecks)
		{
			this.Contents = Contents;
			this.BypassChecks = BypassChecks;
		}

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x060003D0 RID: 976 RVA: 0x0000F96B File Offset: 0x0000DB6B
		// (set) Token: 0x060003D1 RID: 977 RVA: 0x0000F973 File Offset: 0x0000DB73
		public HashSet<EntityUid> Contents { get; set; }

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x060003D2 RID: 978 RVA: 0x0000F97C File Offset: 0x0000DB7C
		// (set) Token: 0x060003D3 RID: 979 RVA: 0x0000F984 File Offset: 0x0000DB84
		public HashSet<EntityUid> BypassChecks { get; set; }

		// Token: 0x060003D4 RID: 980 RVA: 0x0000F990 File Offset: 0x0000DB90
		[NullableContext(0)]
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("StorageBeforeCloseEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x0000F9DC File Offset: 0x0000DBDC
		[NullableContext(0)]
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Contents = ");
			builder.Append(this.Contents);
			builder.Append(", BypassChecks = ");
			builder.Append(this.BypassChecks);
			return true;
		}

		// Token: 0x060003D6 RID: 982 RVA: 0x0000FA11 File Offset: 0x0000DC11
		[CompilerGenerated]
		public static bool operator !=(StorageBeforeCloseEvent left, StorageBeforeCloseEvent right)
		{
			return !(left == right);
		}

		// Token: 0x060003D7 RID: 983 RVA: 0x0000FA1D File Offset: 0x0000DC1D
		[CompilerGenerated]
		public static bool operator ==(StorageBeforeCloseEvent left, StorageBeforeCloseEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x060003D8 RID: 984 RVA: 0x0000FA27 File Offset: 0x0000DC27
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return EqualityComparer<HashSet<EntityUid>>.Default.GetHashCode(this.<Contents>k__BackingField) * -1521134295 + EqualityComparer<HashSet<EntityUid>>.Default.GetHashCode(this.<BypassChecks>k__BackingField);
		}

		// Token: 0x060003D9 RID: 985 RVA: 0x0000FA50 File Offset: 0x0000DC50
		[NullableContext(0)]
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is StorageBeforeCloseEvent && this.Equals((StorageBeforeCloseEvent)obj);
		}

		// Token: 0x060003DA RID: 986 RVA: 0x0000FA68 File Offset: 0x0000DC68
		[CompilerGenerated]
		public bool Equals(StorageBeforeCloseEvent other)
		{
			return EqualityComparer<HashSet<EntityUid>>.Default.Equals(this.<Contents>k__BackingField, other.<Contents>k__BackingField) && EqualityComparer<HashSet<EntityUid>>.Default.Equals(this.<BypassChecks>k__BackingField, other.<BypassChecks>k__BackingField);
		}

		// Token: 0x060003DB RID: 987 RVA: 0x0000FA9A File Offset: 0x0000DC9A
		[CompilerGenerated]
		public void Deconstruct(out HashSet<EntityUid> Contents, out HashSet<EntityUid> BypassChecks)
		{
			Contents = this.Contents;
			BypassChecks = this.BypassChecks;
		}
	}
}
