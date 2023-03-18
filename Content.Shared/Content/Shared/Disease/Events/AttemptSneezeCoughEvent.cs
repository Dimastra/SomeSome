using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;

namespace Content.Shared.Disease.Events
{
	// Token: 0x02000508 RID: 1288
	[NullableContext(1)]
	[Nullable(0)]
	[ByRefEvent]
	public struct AttemptSneezeCoughEvent : IEquatable<AttemptSneezeCoughEvent>
	{
		// Token: 0x06000F9D RID: 3997 RVA: 0x000328C1 File Offset: 0x00030AC1
		public AttemptSneezeCoughEvent(EntityUid uid, string SnoughMessage, [Nullable(2)] SoundSpecifier SnoughSound, bool Cancelled = false)
		{
			this.uid = uid;
			this.SnoughMessage = SnoughMessage;
			this.SnoughSound = SnoughSound;
			this.Cancelled = Cancelled;
		}

		// Token: 0x1700031C RID: 796
		// (get) Token: 0x06000F9E RID: 3998 RVA: 0x000328E0 File Offset: 0x00030AE0
		// (set) Token: 0x06000F9F RID: 3999 RVA: 0x000328E8 File Offset: 0x00030AE8
		public EntityUid uid { readonly get; set; }

		// Token: 0x1700031D RID: 797
		// (get) Token: 0x06000FA0 RID: 4000 RVA: 0x000328F1 File Offset: 0x00030AF1
		// (set) Token: 0x06000FA1 RID: 4001 RVA: 0x000328F9 File Offset: 0x00030AF9
		public string SnoughMessage { readonly get; set; }

		// Token: 0x1700031E RID: 798
		// (get) Token: 0x06000FA2 RID: 4002 RVA: 0x00032902 File Offset: 0x00030B02
		// (set) Token: 0x06000FA3 RID: 4003 RVA: 0x0003290A File Offset: 0x00030B0A
		[Nullable(2)]
		public SoundSpecifier SnoughSound { [NullableContext(2)] readonly get; [NullableContext(2)] set; }

		// Token: 0x1700031F RID: 799
		// (get) Token: 0x06000FA4 RID: 4004 RVA: 0x00032913 File Offset: 0x00030B13
		// (set) Token: 0x06000FA5 RID: 4005 RVA: 0x0003291B File Offset: 0x00030B1B
		public bool Cancelled { readonly get; set; }

		// Token: 0x06000FA6 RID: 4006 RVA: 0x00032924 File Offset: 0x00030B24
		[NullableContext(0)]
		[CompilerGenerated]
		public override readonly string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("AttemptSneezeCoughEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000FA7 RID: 4007 RVA: 0x00032970 File Offset: 0x00030B70
		[NullableContext(0)]
		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			builder.Append("uid = ");
			builder.Append(this.uid.ToString());
			builder.Append(", SnoughMessage = ");
			builder.Append(this.SnoughMessage);
			builder.Append(", SnoughSound = ");
			builder.Append(this.SnoughSound);
			builder.Append(", Cancelled = ");
			builder.Append(this.Cancelled.ToString());
			return true;
		}

		// Token: 0x06000FA8 RID: 4008 RVA: 0x000329FE File Offset: 0x00030BFE
		[CompilerGenerated]
		public static bool operator !=(AttemptSneezeCoughEvent left, AttemptSneezeCoughEvent right)
		{
			return !(left == right);
		}

		// Token: 0x06000FA9 RID: 4009 RVA: 0x00032A0A File Offset: 0x00030C0A
		[CompilerGenerated]
		public static bool operator ==(AttemptSneezeCoughEvent left, AttemptSneezeCoughEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x06000FAA RID: 4010 RVA: 0x00032A14 File Offset: 0x00030C14
		[CompilerGenerated]
		public override readonly int GetHashCode()
		{
			return ((EqualityComparer<EntityUid>.Default.GetHashCode(this.<uid>k__BackingField) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<SnoughMessage>k__BackingField)) * -1521134295 + EqualityComparer<SoundSpecifier>.Default.GetHashCode(this.<SnoughSound>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Cancelled>k__BackingField);
		}

		// Token: 0x06000FAB RID: 4011 RVA: 0x00032A76 File Offset: 0x00030C76
		[NullableContext(0)]
		[CompilerGenerated]
		public override readonly bool Equals(object obj)
		{
			return obj is AttemptSneezeCoughEvent && this.Equals((AttemptSneezeCoughEvent)obj);
		}

		// Token: 0x06000FAC RID: 4012 RVA: 0x00032A90 File Offset: 0x00030C90
		[CompilerGenerated]
		public readonly bool Equals(AttemptSneezeCoughEvent other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.<uid>k__BackingField, other.<uid>k__BackingField) && EqualityComparer<string>.Default.Equals(this.<SnoughMessage>k__BackingField, other.<SnoughMessage>k__BackingField) && EqualityComparer<SoundSpecifier>.Default.Equals(this.<SnoughSound>k__BackingField, other.<SnoughSound>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Cancelled>k__BackingField, other.<Cancelled>k__BackingField);
		}

		// Token: 0x06000FAD RID: 4013 RVA: 0x00032AFD File Offset: 0x00030CFD
		[CompilerGenerated]
		public readonly void Deconstruct(out EntityUid uid, out string SnoughMessage, [Nullable(2)] out SoundSpecifier SnoughSound, out bool Cancelled)
		{
			uid = this.uid;
			SnoughMessage = this.SnoughMessage;
			SnoughSound = this.SnoughSound;
			Cancelled = this.Cancelled;
		}
	}
}
