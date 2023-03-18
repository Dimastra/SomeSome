using System;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Server.Speech.Components;
using Content.Shared.Speech.EntitySystems;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Random;

namespace Content.Server.Speech.EntitySystems
{
	// Token: 0x020001BD RID: 445
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SlurredSystem : SharedSlurredSystem
	{
		// Token: 0x060008B0 RID: 2224 RVA: 0x0002C883 File Offset: 0x0002AA83
		public override void Initialize()
		{
			base.SubscribeLocalEvent<SlurredAccentComponent, AccentGetEvent>(new ComponentEventHandler<SlurredAccentComponent, AccentGetEvent>(this.OnAccent), null, null);
		}

		// Token: 0x060008B1 RID: 2225 RVA: 0x0002C89C File Offset: 0x0002AA9C
		[NullableContext(2)]
		public override void DoSlur(EntityUid uid, TimeSpan time, StatusEffectsComponent status = null)
		{
			if (!base.Resolve<StatusEffectsComponent>(uid, ref status, false))
			{
				return;
			}
			if (!this._statusEffectsSystem.HasStatusEffect(uid, "SlurredSpeech", status))
			{
				this._statusEffectsSystem.TryAddStatusEffect<SlurredAccentComponent>(uid, "SlurredSpeech", time, true, status);
				return;
			}
			this._statusEffectsSystem.TryAddTime(uid, "SlurredSpeech", time, status);
		}

		// Token: 0x060008B2 RID: 2226 RVA: 0x0002C8F4 File Offset: 0x0002AAF4
		private float GetProbabilityScale(EntityUid uid)
		{
			ValueTuple<TimeSpan, TimeSpan>? time;
			if (!this._statusEffectsSystem.TryGetTime(uid, "Drunk", out time, null))
			{
				return 0f;
			}
			return Math.Clamp((float)(time.Value.Item2 - time.Value.Item1).TotalSeconds / 200f, 0f, 1f);
		}

		// Token: 0x060008B3 RID: 2227 RVA: 0x0002C958 File Offset: 0x0002AB58
		private void OnAccent(EntityUid uid, SlurredAccentComponent component, AccentGetEvent args)
		{
			float scale = this.GetProbabilityScale(uid);
			args.Message = this.Accentuate(args.Message, scale);
		}

		// Token: 0x060008B4 RID: 2228 RVA: 0x0002C980 File Offset: 0x0002AB80
		private string Accentuate(string message, float scale)
		{
			StringBuilder sb = new StringBuilder();
			foreach (char character in message)
			{
				if (RandomExtensions.Prob(this._random, scale / 3f))
				{
					char lower = char.ToLowerInvariant(character);
					string text;
					if (lower <= 'c')
					{
						if (lower != 'a')
						{
							if (lower != 'c')
							{
								goto IL_8B;
							}
							text = "k";
						}
						else
						{
							text = "ah";
						}
					}
					else if (lower != 'o')
					{
						if (lower != 's')
						{
							if (lower != 'u')
							{
								goto IL_8B;
							}
							text = "oo";
						}
						else
						{
							text = "ch";
						}
					}
					else
					{
						text = "u";
					}
					IL_A5:
					string newString = text;
					sb.Append(newString);
					goto IL_B2;
					IL_8B:
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
					defaultInterpolatedStringHandler.AppendFormatted<char>(character);
					text = defaultInterpolatedStringHandler.ToStringAndClear();
					goto IL_A5;
				}
				IL_B2:
				if (RandomExtensions.Prob(this._random, scale / 20f))
				{
					if (character == ' ')
					{
						sb.Append(Loc.GetString("slur-accent-confused"));
					}
					else if (character == '.')
					{
						sb.Append(' ');
						sb.Append(Loc.GetString("slur-accent-burp"));
					}
				}
				if (!RandomExtensions.Prob(this._random, scale * 3f / 20f))
				{
					sb.Append(character);
				}
				else
				{
					int num = this._random.Next(1, 3);
					string text;
					if (num != 1)
					{
						if (num != 2)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 3);
							defaultInterpolatedStringHandler.AppendFormatted<char>(character);
							defaultInterpolatedStringHandler.AppendFormatted<char>(character);
							defaultInterpolatedStringHandler.AppendFormatted<char>(character);
							text = defaultInterpolatedStringHandler.ToStringAndClear();
						}
						else
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 2);
							defaultInterpolatedStringHandler.AppendFormatted<char>(character);
							defaultInterpolatedStringHandler.AppendFormatted<char>(character);
							text = defaultInterpolatedStringHandler.ToStringAndClear();
						}
					}
					else
					{
						text = "'";
					}
					string next = text;
					sb.Append(next);
				}
			}
			return sb.ToString();
		}

		// Token: 0x04000543 RID: 1347
		[Dependency]
		private readonly StatusEffectsSystem _statusEffectsSystem;

		// Token: 0x04000544 RID: 1348
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000545 RID: 1349
		private const string SlurKey = "SlurredSpeech";
	}
}
