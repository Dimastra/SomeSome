using System;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking.Rules.Configurations;
using Robust.Shared.Audio;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Server.StationEvents.Events
{
	// Token: 0x02000187 RID: 391
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class FalseAlarm : StationEventSystem
	{
		// Token: 0x17000154 RID: 340
		// (get) Token: 0x060007BA RID: 1978 RVA: 0x00026447 File Offset: 0x00024647
		public override string Prototype
		{
			get
			{
				return "FalseAlarm";
			}
		}

		// Token: 0x060007BB RID: 1979 RVA: 0x00026450 File Offset: 0x00024650
		public override void Started()
		{
			base.Started();
			StationEventRuleConfiguration cfg = StationEventSystem.GetRandomEventUnweighted(this.PrototypeManager, this.RobustRandom).Configuration as StationEventRuleConfiguration;
			if (cfg == null)
			{
				return;
			}
			if (cfg.StartAnnouncement != null)
			{
				this.ChatSystem.DispatchGlobalAnnouncement(Loc.GetString(cfg.StartAnnouncement), "Central Command", false, null, new Color?(Color.Gold));
			}
			if (cfg.StartAudio != null)
			{
				SoundSystem.Play(cfg.StartAudio.GetSound(null, null), Filter.Broadcast(), new AudioParams?(cfg.StartAudio.Params));
			}
		}
	}
}
