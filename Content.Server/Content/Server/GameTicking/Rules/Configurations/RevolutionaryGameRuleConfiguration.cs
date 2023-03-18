using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.GameTicking.Rules.Configurations
{
	// Token: 0x020004CA RID: 1226
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RevolutionaryGameRuleConfiguration : GameRuleConfiguration
	{
		// Token: 0x170003A5 RID: 933
		// (get) Token: 0x06001961 RID: 6497 RVA: 0x00086063 File Offset: 0x00084263
		public override string Id
		{
			get
			{
				return "Revolution";
			}
		}

		// Token: 0x04000FFC RID: 4092
		[DataField("playersPerHeadRev", false, 1, false, false, null)]
		public int PlayersPerHeadRev = 10;

		// Token: 0x04000FFD RID: 4093
		[DataField("maxHeadRevs", false, 1, false, false, null)]
		public int MaxHeadRev = 5;

		// Token: 0x04000FFE RID: 4094
		[DataField("HeadRev", false, 1, false, false, null)]
		public string HeadRevRolePrototype = "HeadRev";

		// Token: 0x04000FFF RID: 4095
		[Nullable(2)]
		[DataField("greetingSound", false, 1, false, false, typeof(SoundSpecifierTypeSerializer))]
		public SoundSpecifier GreetSound = new SoundPathSpecifier("/Audio/Misc/nukeops.ogg", null);
	}
}
