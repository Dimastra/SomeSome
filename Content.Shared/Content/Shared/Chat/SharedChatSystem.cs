using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Popups;
using Content.Shared.Radio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Shared.Chat
{
	// Token: 0x02000603 RID: 1539
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedChatSystem : EntitySystem
	{
		// Token: 0x060012DC RID: 4828 RVA: 0x0003DD2F File Offset: 0x0003BF2F
		public override void Initialize()
		{
			base.Initialize();
			this._prototypeManager.PrototypesReloaded += this.OnPrototypeReload;
			this.CacheRadios();
		}

		// Token: 0x060012DD RID: 4829 RVA: 0x0003DD54 File Offset: 0x0003BF54
		private void OnPrototypeReload(PrototypesReloadedEventArgs obj)
		{
			if (obj.ByType.ContainsKey(typeof(RadioChannelPrototype)))
			{
				this.CacheRadios();
			}
		}

		// Token: 0x060012DE RID: 4830 RVA: 0x0003DD74 File Offset: 0x0003BF74
		private void CacheRadios()
		{
			this._keyCodes.Clear();
			foreach (RadioChannelPrototype proto in this._prototypeManager.EnumeratePrototypes<RadioChannelPrototype>())
			{
				foreach (char keycode in proto.KeyCodes)
				{
					if (!this._keyCodes.ContainsKey(keycode))
					{
						this._keyCodes.Add(keycode, proto);
					}
				}
			}
		}

		// Token: 0x060012DF RID: 4831 RVA: 0x0003DE20 File Offset: 0x0003C020
		public override void Shutdown()
		{
			this._prototypeManager.PrototypesReloaded -= this.OnPrototypeReload;
		}

		// Token: 0x060012E0 RID: 4832 RVA: 0x0003DE3C File Offset: 0x0003C03C
		public bool TryProccessRadioMessage(EntityUid source, string input, out string output, [Nullable(2)] out RadioChannelPrototype channel, bool quiet = false)
		{
			output = input.Trim();
			channel = null;
			if (input.Length == 0)
			{
				return false;
			}
			if (input.StartsWith(';'))
			{
				output = this.SanitizeMessageCapital(input.Substring(1, input.Length - 1).TrimStart());
				channel = this._prototypeManager.Index<RadioChannelPrototype>("Common");
				return true;
			}
			if (!input.StartsWith(':'))
			{
				return false;
			}
			if (input.Length < 2 || char.IsWhiteSpace(input[1]))
			{
				output = this.SanitizeMessageCapital(input.Substring(1, input.Length - 1).TrimStart());
				if (!quiet)
				{
					this._popup.PopupEntity(Loc.GetString("chat-manager-no-radio-key"), source, source, PopupType.Small);
				}
				return true;
			}
			char channelKey = input[1];
			output = this.SanitizeMessageCapital(input.Substring(2, input.Length - 2).TrimStart());
			if (channelKey == 'h')
			{
				GetDefaultRadioChannelEvent ev = new GetDefaultRadioChannelEvent();
				base.RaiseLocalEvent<GetDefaultRadioChannelEvent>(source, ev, false);
				if (ev.Channel != null)
				{
					this._prototypeManager.TryIndex<RadioChannelPrototype>(ev.Channel, ref channel);
				}
				return true;
			}
			if (!this._keyCodes.TryGetValue(channelKey, out channel) && !quiet)
			{
				string msg = Loc.GetString("chat-manager-no-such-channel", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("key", channelKey)
				});
				this._popup.PopupEntity(msg, source, source, PopupType.Small);
			}
			return true;
		}

		// Token: 0x060012E1 RID: 4833 RVA: 0x0003DFA0 File Offset: 0x0003C1A0
		public string SanitizeMessageCapital(string message)
		{
			if (string.IsNullOrEmpty(message))
			{
				return message;
			}
			message = char.ToUpper(message[0]).ToString() + message.Remove(0, 1);
			return message;
		}

		// Token: 0x060012E3 RID: 4835 RVA: 0x0003DFF0 File Offset: 0x0003C1F0
		// Note: this type is marked as 'beforefieldinit'.
		static SharedChatSystem()
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 2);
			defaultInterpolatedStringHandler.AppendFormatted<char>(':');
			defaultInterpolatedStringHandler.AppendFormatted<char>('h');
			SharedChatSystem.DefaultChannelPrefix = defaultInterpolatedStringHandler.ToStringAndClear();
		}

		// Token: 0x0400118C RID: 4492
		public const char RadioCommonPrefix = ';';

		// Token: 0x0400118D RID: 4493
		public const char RadioChannelPrefix = ':';

		// Token: 0x0400118E RID: 4494
		public const char LocalPrefix = '.';

		// Token: 0x0400118F RID: 4495
		public const char ConsolePrefix = '/';

		// Token: 0x04001190 RID: 4496
		public const char DeadPrefix = '\\';

		// Token: 0x04001191 RID: 4497
		public const char LOOCPrefix = '(';

		// Token: 0x04001192 RID: 4498
		public const char OOCPrefix = '[';

		// Token: 0x04001193 RID: 4499
		public const char EmotesPrefix = '@';

		// Token: 0x04001194 RID: 4500
		public const char AdminPrefix = ']';

		// Token: 0x04001195 RID: 4501
		public const char WhisperPrefix = ',';

		// Token: 0x04001196 RID: 4502
		public const char DefaultChannelKey = 'h';

		// Token: 0x04001197 RID: 4503
		public const string CommonChannel = "Common";

		// Token: 0x04001198 RID: 4504
		public static string DefaultChannelPrefix;

		// Token: 0x04001199 RID: 4505
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x0400119A RID: 4506
		[Dependency]
		private readonly SharedPopupSystem _popup;

		// Token: 0x0400119B RID: 4507
		private Dictionary<char, RadioChannelPrototype> _keyCodes = new Dictionary<char, RadioChannelPrototype>();
	}
}
