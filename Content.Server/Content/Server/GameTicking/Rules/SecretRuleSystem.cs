using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking.Presets;
using Content.Shared.GameTicking;
using Content.Shared.Random;
using Content.Shared.Random.Helpers;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.GameTicking.Rules
{
	// Token: 0x020004C0 RID: 1216
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SecretRuleSystem : GameRuleSystem
	{
		// Token: 0x1700038D RID: 909
		// (get) Token: 0x06001904 RID: 6404 RVA: 0x0008322A File Offset: 0x0008142A
		public override string Prototype
		{
			get
			{
				return "Secret";
			}
		}

		// Token: 0x06001905 RID: 6405 RVA: 0x00083231 File Offset: 0x00081431
		public override void Started()
		{
			this.PickRule();
		}

		// Token: 0x06001906 RID: 6406 RVA: 0x00083239 File Offset: 0x00081439
		public override void Ended()
		{
		}

		// Token: 0x06001907 RID: 6407 RVA: 0x0008323C File Offset: 0x0008143C
		private void PickRule()
		{
			WeightedRandomPrototype preset = this._prototypeManager.Index<WeightedRandomPrototype>("Secret");
			int readyPlayers = this._ticker.PlayerGameStatuses.Values.ToArray<PlayerGameStatus>().Count((PlayerGameStatus status) => status == PlayerGameStatus.ReadyToPlay);
			foreach (KeyValuePair<string, float> pair in preset.Weights)
			{
				GamePresetPrototype prototype;
				if (!this._prototypeManager.TryIndex<GamePresetPrototype>(pair.Key, ref prototype))
				{
					Logger.Warning("Couldn't find " + pair.Key + " game preset for secret game preset");
					preset.Weights.Remove(pair.Key);
				}
				else
				{
					int? minPlayers = prototype.MinPlayers;
					int num = readyPlayers;
					if (minPlayers.GetValueOrDefault() > num & minPlayers != null)
					{
						preset.Weights.Remove(pair.Key);
					}
				}
			}
			if (preset.Weights.Count == 0)
			{
				preset.Weights.Add("Traitor", 1f);
			}
			string presetToPlay = preset.Pick(this._random);
			Logger.InfoS("gamepreset", "Selected " + presetToPlay + " for secret.");
			foreach (string rule in this._prototypeManager.Index<GamePresetPrototype>(presetToPlay).Rules)
			{
				this._ticker.StartGameRule(this._prototypeManager.Index<GameRulePrototype>(rule));
			}
		}

		// Token: 0x04000F9D RID: 3997
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000F9E RID: 3998
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000F9F RID: 3999
		[Dependency]
		private readonly GameTicker _ticker;
	}
}
