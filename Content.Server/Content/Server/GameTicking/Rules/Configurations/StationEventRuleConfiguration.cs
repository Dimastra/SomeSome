using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.GameTicking.Rules.Configurations
{
	// Token: 0x020004CC RID: 1228
	[NullableContext(2)]
	[Nullable(0)]
	public class StationEventRuleConfiguration : GameRuleConfiguration
	{
		// Token: 0x170003A6 RID: 934
		// (get) Token: 0x06001964 RID: 6500 RVA: 0x000860C5 File Offset: 0x000842C5
		[Nullable(1)]
		public override string Id
		{
			[NullableContext(1)]
			get
			{
				return this._id;
			}
		}

		// Token: 0x04001005 RID: 4101
		[Nullable(1)]
		[DataField("id", false, 1, true, false, null)]
		private string _id;

		// Token: 0x04001006 RID: 4102
		public const float WeightVeryLow = 0f;

		// Token: 0x04001007 RID: 4103
		public const float WeightLow = 5f;

		// Token: 0x04001008 RID: 4104
		public const float WeightNormal = 10f;

		// Token: 0x04001009 RID: 4105
		public const float WeightHigh = 15f;

		// Token: 0x0400100A RID: 4106
		public const float WeightVeryHigh = 20f;

		// Token: 0x0400100B RID: 4107
		[DataField("weight", false, 1, false, false, null)]
		public float Weight = 10f;

		// Token: 0x0400100C RID: 4108
		[DataField("startAnnouncement", false, 1, false, false, null)]
		public string StartAnnouncement;

		// Token: 0x0400100D RID: 4109
		[DataField("endAnnouncement", false, 1, false, false, null)]
		public string EndAnnouncement;

		// Token: 0x0400100E RID: 4110
		[DataField("startAudio", false, 1, false, false, null)]
		public SoundSpecifier StartAudio;

		// Token: 0x0400100F RID: 4111
		[DataField("endAudio", false, 1, false, false, null)]
		public SoundSpecifier EndAudio;

		// Token: 0x04001010 RID: 4112
		[DataField("earliestStart", false, 1, false, false, null)]
		public int EarliestStart = 5;

		// Token: 0x04001011 RID: 4113
		[DataField("reoccurrenceDelay", false, 1, false, false, null)]
		public int ReoccurrenceDelay = 30;

		// Token: 0x04001012 RID: 4114
		[DataField("startAfter", false, 1, false, false, null)]
		public float StartAfter;

		// Token: 0x04001013 RID: 4115
		[DataField("endAfter", false, 1, false, false, null)]
		public float EndAfter = float.MaxValue;

		// Token: 0x04001014 RID: 4116
		[DataField("minimumPlayers", false, 1, false, false, null)]
		public int MinimumPlayers;

		// Token: 0x04001015 RID: 4117
		[DataField("maxOccurrences", false, 1, false, false, null)]
		public int? MaxOccurrences;
	}
}
