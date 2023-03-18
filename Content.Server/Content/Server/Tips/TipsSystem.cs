using System;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Managers;
using Content.Server.GameTicking;
using Content.Shared.CCVar;
using Content.Shared.Chat;
using Content.Shared.Dataset;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Server.Tips
{
	// Token: 0x0200011F RID: 287
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TipsSystem : EntitySystem
	{
		// Token: 0x0600052D RID: 1325 RVA: 0x000192DC File Offset: 0x000174DC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GameRunLevelChangedEvent>(new EntityEventHandler<GameRunLevelChangedEvent>(this.OnGameRunLevelChanged), null, null);
			this._cfg.OnValueChanged<float>(CCVars.TipFrequencyOutOfRound, new Action<float>(this.SetOutOfRound), true);
			this._cfg.OnValueChanged<float>(CCVars.TipFrequencyInRound, new Action<float>(this.SetInRound), true);
			this._cfg.OnValueChanged<bool>(CCVars.TipsEnabled, new Action<bool>(this.SetEnabled), true);
			this._cfg.OnValueChanged<string>(CCVars.TipsDataset, new Action<string>(this.SetDataset), true);
			this.RecalculateNextTipTime();
		}

		// Token: 0x0600052E RID: 1326 RVA: 0x00019380 File Offset: 0x00017580
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			if (!this._tipsEnabled)
			{
				return;
			}
			if (this._nextTipTime != TimeSpan.Zero && this._timing.CurTime > this._nextTipTime)
			{
				this.AnnounceRandomTip();
				this.RecalculateNextTipTime();
			}
		}

		// Token: 0x0600052F RID: 1327 RVA: 0x000193D3 File Offset: 0x000175D3
		private void SetOutOfRound(float value)
		{
			this._tipTimeOutOfRound = value;
		}

		// Token: 0x06000530 RID: 1328 RVA: 0x000193DC File Offset: 0x000175DC
		private void SetInRound(float value)
		{
			this._tipTimeInRound = value;
		}

		// Token: 0x06000531 RID: 1329 RVA: 0x000193E5 File Offset: 0x000175E5
		private void SetEnabled(bool value)
		{
			this._tipsEnabled = value;
			if (this._nextTipTime != TimeSpan.Zero)
			{
				this.RecalculateNextTipTime();
			}
		}

		// Token: 0x06000532 RID: 1330 RVA: 0x00019406 File Offset: 0x00017606
		private void SetDataset(string value)
		{
			this._tipsDataset = value;
		}

		// Token: 0x06000533 RID: 1331 RVA: 0x00019410 File Offset: 0x00017610
		private void AnnounceRandomTip()
		{
			DatasetPrototype tips;
			if (!this._prototype.TryIndex<DatasetPrototype>(this._tipsDataset, ref tips))
			{
				return;
			}
			string tip = RandomExtensions.Pick<string>(this._random, tips.Values);
			string msg = Loc.GetString("tips-system-chat-message-wrap", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("tip", tip)
			});
			this._chat.ChatMessageToManyFiltered(Filter.Broadcast(), ChatChannel.OOC, tip, msg, EntityUid.Invalid, false, false, new Color?(Color.MediumPurple), null, 0f);
		}

		// Token: 0x06000534 RID: 1332 RVA: 0x00019494 File Offset: 0x00017694
		private void RecalculateNextTipTime()
		{
			if (this._ticker.RunLevel == GameRunLevel.InRound)
			{
				this._nextTipTime = this._timing.CurTime + TimeSpan.FromSeconds((double)this._tipTimeInRound);
				return;
			}
			this._nextTipTime = this._timing.CurTime + TimeSpan.FromSeconds((double)this._tipTimeOutOfRound);
		}

		// Token: 0x06000535 RID: 1333 RVA: 0x000194F4 File Offset: 0x000176F4
		private void OnGameRunLevelChanged(GameRunLevelChangedEvent ev)
		{
			if (ev.New == GameRunLevel.InRound || ev.Old == GameRunLevel.InRound)
			{
				this.RecalculateNextTipTime();
			}
		}

		// Token: 0x04000315 RID: 789
		[Dependency]
		private readonly IChatManager _chat;

		// Token: 0x04000316 RID: 790
		[Dependency]
		private readonly IPrototypeManager _prototype;

		// Token: 0x04000317 RID: 791
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04000318 RID: 792
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04000319 RID: 793
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x0400031A RID: 794
		[Dependency]
		private readonly GameTicker _ticker;

		// Token: 0x0400031B RID: 795
		private bool _tipsEnabled;

		// Token: 0x0400031C RID: 796
		private float _tipTimeOutOfRound;

		// Token: 0x0400031D RID: 797
		private float _tipTimeInRound;

		// Token: 0x0400031E RID: 798
		private string _tipsDataset = "";

		// Token: 0x0400031F RID: 799
		[ViewVariables]
		private TimeSpan _nextTipTime = TimeSpan.Zero;
	}
}
