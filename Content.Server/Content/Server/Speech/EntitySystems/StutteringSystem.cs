using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Content.Server.Speech.Components;
using Content.Shared.Speech.EntitySystems;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Server.Speech.EntitySystems
{
	// Token: 0x020001BF RID: 447
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StutteringSystem : SharedStutteringSystem
	{
		// Token: 0x060008BC RID: 2236 RVA: 0x0002CC76 File Offset: 0x0002AE76
		public override void Initialize()
		{
			base.SubscribeLocalEvent<StutteringAccentComponent, AccentGetEvent>(new ComponentEventHandler<StutteringAccentComponent, AccentGetEvent>(this.OnAccent), null, null);
		}

		// Token: 0x060008BD RID: 2237 RVA: 0x0002CC8C File Offset: 0x0002AE8C
		[NullableContext(2)]
		public override void DoStutter(EntityUid uid, TimeSpan time, bool refresh, StatusEffectsComponent status = null)
		{
			if (!base.Resolve<StatusEffectsComponent>(uid, ref status, false))
			{
				return;
			}
			this._statusEffectsSystem.TryAddStatusEffect<StutteringAccentComponent>(uid, "Stutter", time, refresh, status);
		}

		// Token: 0x060008BE RID: 2238 RVA: 0x0002CCB1 File Offset: 0x0002AEB1
		private void OnAccent(EntityUid uid, StutteringAccentComponent component, AccentGetEvent args)
		{
			args.Message = this.Accentuate(args.Message);
		}

		// Token: 0x060008BF RID: 2239 RVA: 0x0002CCC8 File Offset: 0x0002AEC8
		public string Accentuate(string message)
		{
			int length = message.Length;
			StringBuilder finalMessage = new StringBuilder();
			for (int i = 0; i < length; i++)
			{
				string newLetter = message[i].ToString();
				if (StutteringSystem.Stutter.IsMatch(newLetter) && RandomExtensions.Prob(this._random, 0.8f))
				{
					if (RandomExtensions.Prob(this._random, 0.1f))
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 4);
						defaultInterpolatedStringHandler.AppendFormatted(newLetter);
						defaultInterpolatedStringHandler.AppendLiteral("-");
						defaultInterpolatedStringHandler.AppendFormatted(newLetter);
						defaultInterpolatedStringHandler.AppendLiteral("-");
						defaultInterpolatedStringHandler.AppendFormatted(newLetter);
						defaultInterpolatedStringHandler.AppendLiteral("-");
						defaultInterpolatedStringHandler.AppendFormatted(newLetter);
						newLetter = defaultInterpolatedStringHandler.ToStringAndClear();
					}
					else if (RandomExtensions.Prob(this._random, 0.2f))
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 3);
						defaultInterpolatedStringHandler.AppendFormatted(newLetter);
						defaultInterpolatedStringHandler.AppendLiteral("-");
						defaultInterpolatedStringHandler.AppendFormatted(newLetter);
						defaultInterpolatedStringHandler.AppendLiteral("-");
						defaultInterpolatedStringHandler.AppendFormatted(newLetter);
						newLetter = defaultInterpolatedStringHandler.ToStringAndClear();
					}
					else if (RandomExtensions.Prob(this._random, 0.05f))
					{
						newLetter = "";
					}
					else
					{
						newLetter = newLetter + "-" + newLetter;
					}
				}
				finalMessage.Append(newLetter);
			}
			return finalMessage.ToString();
		}

		// Token: 0x04000546 RID: 1350
		[Dependency]
		private readonly StatusEffectsSystem _statusEffectsSystem;

		// Token: 0x04000547 RID: 1351
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000548 RID: 1352
		private const string StutterKey = "Stutter";

		// Token: 0x04000549 RID: 1353
		private static readonly Regex Stutter = new Regex("[b-df-hj-np-tv-wxyz]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
	}
}
