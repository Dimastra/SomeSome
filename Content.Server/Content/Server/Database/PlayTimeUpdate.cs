using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.Network;

namespace Content.Server.Database
{
	// Token: 0x020005B9 RID: 1465
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PlayTimeUpdate : IEquatable<PlayTimeUpdate>
	{
		// Token: 0x06001F15 RID: 7957 RVA: 0x000A3019 File Offset: 0x000A1219
		public PlayTimeUpdate(NetUserId User, string Tracker, TimeSpan Time)
		{
			this.User = User;
			this.Tracker = Tracker;
			this.Time = Time;
			base..ctor();
		}

		// Token: 0x1700048E RID: 1166
		// (get) Token: 0x06001F16 RID: 7958 RVA: 0x000A3036 File Offset: 0x000A1236
		[CompilerGenerated]
		private Type EqualityContract
		{
			[CompilerGenerated]
			get
			{
				return typeof(PlayTimeUpdate);
			}
		}

		// Token: 0x1700048F RID: 1167
		// (get) Token: 0x06001F17 RID: 7959 RVA: 0x000A3042 File Offset: 0x000A1242
		// (set) Token: 0x06001F18 RID: 7960 RVA: 0x000A304A File Offset: 0x000A124A
		public NetUserId User { get; set; }

		// Token: 0x17000490 RID: 1168
		// (get) Token: 0x06001F19 RID: 7961 RVA: 0x000A3053 File Offset: 0x000A1253
		// (set) Token: 0x06001F1A RID: 7962 RVA: 0x000A305B File Offset: 0x000A125B
		public string Tracker { get; set; }

		// Token: 0x17000491 RID: 1169
		// (get) Token: 0x06001F1B RID: 7963 RVA: 0x000A3064 File Offset: 0x000A1264
		// (set) Token: 0x06001F1C RID: 7964 RVA: 0x000A306C File Offset: 0x000A126C
		public TimeSpan Time { get; set; }

		// Token: 0x06001F1D RID: 7965 RVA: 0x000A3078 File Offset: 0x000A1278
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("PlayTimeUpdate");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06001F1E RID: 7966 RVA: 0x000A30C4 File Offset: 0x000A12C4
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			RuntimeHelpers.EnsureSufficientExecutionStack();
			builder.Append("User = ");
			builder.Append(this.User.ToString());
			builder.Append(", Tracker = ");
			builder.Append(this.Tracker);
			builder.Append(", Time = ");
			builder.Append(this.Time.ToString());
			return true;
		}

		// Token: 0x06001F1F RID: 7967 RVA: 0x000A313E File Offset: 0x000A133E
		[NullableContext(2)]
		[CompilerGenerated]
		public static bool operator !=(PlayTimeUpdate left, PlayTimeUpdate right)
		{
			return !(left == right);
		}

		// Token: 0x06001F20 RID: 7968 RVA: 0x000A314A File Offset: 0x000A134A
		[NullableContext(2)]
		[CompilerGenerated]
		public static bool operator ==(PlayTimeUpdate left, PlayTimeUpdate right)
		{
			return left == right || (left != null && left.Equals(right));
		}

		// Token: 0x06001F21 RID: 7969 RVA: 0x000A3160 File Offset: 0x000A1360
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return ((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<NetUserId>.Default.GetHashCode(this.<User>k__BackingField)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<Tracker>k__BackingField)) * -1521134295 + EqualityComparer<TimeSpan>.Default.GetHashCode(this.<Time>k__BackingField);
		}

		// Token: 0x06001F22 RID: 7970 RVA: 0x000A31C2 File Offset: 0x000A13C2
		[NullableContext(2)]
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return this.Equals(obj as PlayTimeUpdate);
		}

		// Token: 0x06001F23 RID: 7971 RVA: 0x000A31D0 File Offset: 0x000A13D0
		[NullableContext(2)]
		[CompilerGenerated]
		public bool Equals(PlayTimeUpdate other)
		{
			return this == other || (other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<NetUserId>.Default.Equals(this.<User>k__BackingField, other.<User>k__BackingField) && EqualityComparer<string>.Default.Equals(this.<Tracker>k__BackingField, other.<Tracker>k__BackingField) && EqualityComparer<TimeSpan>.Default.Equals(this.<Time>k__BackingField, other.<Time>k__BackingField));
		}

		// Token: 0x06001F25 RID: 7973 RVA: 0x000A3249 File Offset: 0x000A1449
		[CompilerGenerated]
		private PlayTimeUpdate(PlayTimeUpdate original)
		{
			this.User = original.<User>k__BackingField;
			this.Tracker = original.<Tracker>k__BackingField;
			this.Time = original.<Time>k__BackingField;
		}

		// Token: 0x06001F26 RID: 7974 RVA: 0x000A3275 File Offset: 0x000A1475
		[CompilerGenerated]
		public void Deconstruct(out NetUserId User, out string Tracker, out TimeSpan Time)
		{
			User = this.User;
			Tracker = this.Tracker;
			Time = this.Time;
		}
	}
}
