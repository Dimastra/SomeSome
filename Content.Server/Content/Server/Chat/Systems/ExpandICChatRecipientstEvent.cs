using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;
using Robust.Shared.Players;

namespace Content.Server.Chat.Systems
{
	// Token: 0x020006BF RID: 1727
	[NullableContext(1)]
	[Nullable(0)]
	public class ExpandICChatRecipientstEvent : IEquatable<ExpandICChatRecipientstEvent>
	{
		// Token: 0x0600240B RID: 9227 RVA: 0x000BC6F0 File Offset: 0x000BA8F0
		public ExpandICChatRecipientstEvent(EntityUid Source, float VoiceRange, Dictionary<ICommonSession, ChatSystem.ICChatRecipientData> Recipients)
		{
			this.Source = Source;
			this.VoiceRange = VoiceRange;
			this.Recipients = Recipients;
			base..ctor();
		}

		// Token: 0x17000556 RID: 1366
		// (get) Token: 0x0600240C RID: 9228 RVA: 0x000BC70D File Offset: 0x000BA90D
		[CompilerGenerated]
		protected virtual Type EqualityContract
		{
			[CompilerGenerated]
			get
			{
				return typeof(ExpandICChatRecipientstEvent);
			}
		}

		// Token: 0x17000557 RID: 1367
		// (get) Token: 0x0600240D RID: 9229 RVA: 0x000BC719 File Offset: 0x000BA919
		// (set) Token: 0x0600240E RID: 9230 RVA: 0x000BC721 File Offset: 0x000BA921
		public EntityUid Source { get; set; }

		// Token: 0x17000558 RID: 1368
		// (get) Token: 0x0600240F RID: 9231 RVA: 0x000BC72A File Offset: 0x000BA92A
		// (set) Token: 0x06002410 RID: 9232 RVA: 0x000BC732 File Offset: 0x000BA932
		public float VoiceRange { get; set; }

		// Token: 0x17000559 RID: 1369
		// (get) Token: 0x06002411 RID: 9233 RVA: 0x000BC73B File Offset: 0x000BA93B
		// (set) Token: 0x06002412 RID: 9234 RVA: 0x000BC743 File Offset: 0x000BA943
		public Dictionary<ICommonSession, ChatSystem.ICChatRecipientData> Recipients { get; set; }

		// Token: 0x06002413 RID: 9235 RVA: 0x000BC74C File Offset: 0x000BA94C
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("ExpandICChatRecipientstEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06002414 RID: 9236 RVA: 0x000BC798 File Offset: 0x000BA998
		[CompilerGenerated]
		protected virtual bool PrintMembers(StringBuilder builder)
		{
			RuntimeHelpers.EnsureSufficientExecutionStack();
			builder.Append("Source = ");
			builder.Append(this.Source.ToString());
			builder.Append(", VoiceRange = ");
			builder.Append(this.VoiceRange.ToString());
			builder.Append(", Recipients = ");
			builder.Append(this.Recipients);
			return true;
		}

		// Token: 0x06002415 RID: 9237 RVA: 0x000BC812 File Offset: 0x000BAA12
		[NullableContext(2)]
		[CompilerGenerated]
		public static bool operator !=(ExpandICChatRecipientstEvent left, ExpandICChatRecipientstEvent right)
		{
			return !(left == right);
		}

		// Token: 0x06002416 RID: 9238 RVA: 0x000BC81E File Offset: 0x000BAA1E
		[NullableContext(2)]
		[CompilerGenerated]
		public static bool operator ==(ExpandICChatRecipientstEvent left, ExpandICChatRecipientstEvent right)
		{
			return left == right || (left != null && left.Equals(right));
		}

		// Token: 0x06002417 RID: 9239 RVA: 0x000BC834 File Offset: 0x000BAA34
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return ((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<EntityUid>.Default.GetHashCode(this.<Source>k__BackingField)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.<VoiceRange>k__BackingField)) * -1521134295 + EqualityComparer<Dictionary<ICommonSession, ChatSystem.ICChatRecipientData>>.Default.GetHashCode(this.<Recipients>k__BackingField);
		}

		// Token: 0x06002418 RID: 9240 RVA: 0x000BC896 File Offset: 0x000BAA96
		[NullableContext(2)]
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return this.Equals(obj as ExpandICChatRecipientstEvent);
		}

		// Token: 0x06002419 RID: 9241 RVA: 0x000BC8A4 File Offset: 0x000BAAA4
		[NullableContext(2)]
		[CompilerGenerated]
		public virtual bool Equals(ExpandICChatRecipientstEvent other)
		{
			return this == other || (other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<EntityUid>.Default.Equals(this.<Source>k__BackingField, other.<Source>k__BackingField) && EqualityComparer<float>.Default.Equals(this.<VoiceRange>k__BackingField, other.<VoiceRange>k__BackingField) && EqualityComparer<Dictionary<ICommonSession, ChatSystem.ICChatRecipientData>>.Default.Equals(this.<Recipients>k__BackingField, other.<Recipients>k__BackingField));
		}

		// Token: 0x0600241B RID: 9243 RVA: 0x000BC91D File Offset: 0x000BAB1D
		[CompilerGenerated]
		protected ExpandICChatRecipientstEvent(ExpandICChatRecipientstEvent original)
		{
			this.Source = original.<Source>k__BackingField;
			this.VoiceRange = original.<VoiceRange>k__BackingField;
			this.Recipients = original.<Recipients>k__BackingField;
		}

		// Token: 0x0600241C RID: 9244 RVA: 0x000BC949 File Offset: 0x000BAB49
		[CompilerGenerated]
		public void Deconstruct(out EntityUid Source, out float VoiceRange, out Dictionary<ICommonSession, ChatSystem.ICChatRecipientData> Recipients)
		{
			Source = this.Source;
			VoiceRange = this.VoiceRange;
			Recipients = this.Recipients;
		}
	}
}
