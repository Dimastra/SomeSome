using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Shared.Chemistry.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Chemistry.Reaction
{
	// Token: 0x020005EE RID: 1518
	[NullableContext(2)]
	[Nullable(0)]
	[ByRefEvent]
	public struct GetMixableSolutionAttemptEvent : IEquatable<GetMixableSolutionAttemptEvent>
	{
		// Token: 0x06001264 RID: 4708 RVA: 0x0003C023 File Offset: 0x0003A223
		public GetMixableSolutionAttemptEvent(EntityUid Mixed, Solution MixedSolution = null)
		{
			this.Mixed = Mixed;
			this.MixedSolution = MixedSolution;
		}

		// Token: 0x170003AF RID: 943
		// (get) Token: 0x06001265 RID: 4709 RVA: 0x0003C033 File Offset: 0x0003A233
		// (set) Token: 0x06001266 RID: 4710 RVA: 0x0003C03B File Offset: 0x0003A23B
		public EntityUid Mixed { readonly get; set; }

		// Token: 0x170003B0 RID: 944
		// (get) Token: 0x06001267 RID: 4711 RVA: 0x0003C044 File Offset: 0x0003A244
		// (set) Token: 0x06001268 RID: 4712 RVA: 0x0003C04C File Offset: 0x0003A24C
		public Solution MixedSolution { readonly get; set; }

		// Token: 0x06001269 RID: 4713 RVA: 0x0003C058 File Offset: 0x0003A258
		[NullableContext(0)]
		[CompilerGenerated]
		public override readonly string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("GetMixableSolutionAttemptEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x0600126A RID: 4714 RVA: 0x0003C0A4 File Offset: 0x0003A2A4
		[NullableContext(0)]
		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Mixed = ");
			builder.Append(this.Mixed.ToString());
			builder.Append(", MixedSolution = ");
			builder.Append(this.MixedSolution);
			return true;
		}

		// Token: 0x0600126B RID: 4715 RVA: 0x0003C0F2 File Offset: 0x0003A2F2
		[CompilerGenerated]
		public static bool operator !=(GetMixableSolutionAttemptEvent left, GetMixableSolutionAttemptEvent right)
		{
			return !(left == right);
		}

		// Token: 0x0600126C RID: 4716 RVA: 0x0003C0FE File Offset: 0x0003A2FE
		[CompilerGenerated]
		public static bool operator ==(GetMixableSolutionAttemptEvent left, GetMixableSolutionAttemptEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x0600126D RID: 4717 RVA: 0x0003C108 File Offset: 0x0003A308
		[CompilerGenerated]
		public override readonly int GetHashCode()
		{
			return EqualityComparer<EntityUid>.Default.GetHashCode(this.<Mixed>k__BackingField) * -1521134295 + EqualityComparer<Solution>.Default.GetHashCode(this.<MixedSolution>k__BackingField);
		}

		// Token: 0x0600126E RID: 4718 RVA: 0x0003C131 File Offset: 0x0003A331
		[NullableContext(0)]
		[CompilerGenerated]
		public override readonly bool Equals(object obj)
		{
			return obj is GetMixableSolutionAttemptEvent && this.Equals((GetMixableSolutionAttemptEvent)obj);
		}

		// Token: 0x0600126F RID: 4719 RVA: 0x0003C149 File Offset: 0x0003A349
		[CompilerGenerated]
		public readonly bool Equals(GetMixableSolutionAttemptEvent other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.<Mixed>k__BackingField, other.<Mixed>k__BackingField) && EqualityComparer<Solution>.Default.Equals(this.<MixedSolution>k__BackingField, other.<MixedSolution>k__BackingField);
		}

		// Token: 0x06001270 RID: 4720 RVA: 0x0003C17B File Offset: 0x0003A37B
		[CompilerGenerated]
		public readonly void Deconstruct(out EntityUid Mixed, out Solution MixedSolution)
		{
			Mixed = this.Mixed;
			MixedSolution = this.MixedSolution;
		}
	}
}
