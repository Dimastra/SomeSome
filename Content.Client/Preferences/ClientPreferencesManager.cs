using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Client.White.Sponsors;
using Content.Shared.Preferences;
using Content.Shared.White.Sponsors;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Utility;

namespace Content.Client.Preferences
{
	// Token: 0x02000180 RID: 384
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ClientPreferencesManager : IClientPreferencesManager
	{
		// Token: 0x14000045 RID: 69
		// (add) Token: 0x060009F7 RID: 2551 RVA: 0x0003A084 File Offset: 0x00038284
		// (remove) Token: 0x060009F8 RID: 2552 RVA: 0x0003A0BC File Offset: 0x000382BC
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event Action OnServerDataLoaded;

		// Token: 0x170001CC RID: 460
		// (get) Token: 0x060009F9 RID: 2553 RVA: 0x0003A0F1 File Offset: 0x000382F1
		// (set) Token: 0x060009FA RID: 2554 RVA: 0x0003A0F9 File Offset: 0x000382F9
		public GameSettings Settings { get; private set; }

		// Token: 0x170001CD RID: 461
		// (get) Token: 0x060009FB RID: 2555 RVA: 0x0003A102 File Offset: 0x00038302
		// (set) Token: 0x060009FC RID: 2556 RVA: 0x0003A10A File Offset: 0x0003830A
		public PlayerPreferences Preferences { get; private set; }

		// Token: 0x060009FD RID: 2557 RVA: 0x0003A114 File Offset: 0x00038314
		public void Initialize()
		{
			this._netManager.RegisterNetMessage<MsgPreferencesAndSettings>(new ProcessMessage<MsgPreferencesAndSettings>(this.HandlePreferencesAndSettings), 3);
			this._netManager.RegisterNetMessage<MsgUpdateCharacter>(null, 3);
			this._netManager.RegisterNetMessage<MsgSelectCharacter>(null, 3);
			this._netManager.RegisterNetMessage<MsgDeleteCharacter>(null, 3);
		}

		// Token: 0x060009FE RID: 2558 RVA: 0x0003A160 File Offset: 0x00038360
		public void SelectCharacter(ICharacterProfile profile)
		{
			this.SelectCharacter(this.Preferences.IndexOfCharacter(profile));
		}

		// Token: 0x060009FF RID: 2559 RVA: 0x0003A174 File Offset: 0x00038374
		public void SelectCharacter(int slot)
		{
			this.Preferences = new PlayerPreferences(this.Preferences.Characters, slot, this.Preferences.AdminOOCColor);
			MsgSelectCharacter msgSelectCharacter = new MsgSelectCharacter
			{
				SelectedCharacterIndex = slot
			};
			this._netManager.ClientSendMessage(msgSelectCharacter);
		}

		// Token: 0x06000A00 RID: 2560 RVA: 0x0003A1BC File Offset: 0x000383BC
		public void UpdateCharacter(ICharacterProfile profile, int slot)
		{
			SponsorInfo sponsorInfo;
			string[] sponsorMarkings = this._sponsorsManager.TryGetInfo(out sponsorInfo) ? sponsorInfo.AllowedMarkings : new string[0];
			profile.EnsureValid(sponsorMarkings);
			Dictionary<int, ICharacterProfile> dictionary = new Dictionary<int, ICharacterProfile>(this.Preferences.Characters);
			dictionary[slot] = profile;
			Dictionary<int, ICharacterProfile> characters = dictionary;
			this.Preferences = new PlayerPreferences(characters, this.Preferences.SelectedCharacterIndex, this.Preferences.AdminOOCColor);
			MsgUpdateCharacter msgUpdateCharacter = new MsgUpdateCharacter
			{
				Profile = profile,
				Slot = slot
			};
			this._netManager.ClientSendMessage(msgUpdateCharacter);
		}

		// Token: 0x06000A01 RID: 2561 RVA: 0x0003A250 File Offset: 0x00038450
		public void CreateCharacter(ICharacterProfile profile)
		{
			Dictionary<int, ICharacterProfile> dictionary = new Dictionary<int, ICharacterProfile>(this.Preferences.Characters);
			int? num = Extensions.FirstOrNull<int>(Enumerable.Range(0, this.Settings.MaxCharacterSlots).Except(dictionary.Keys));
			if (num == null)
			{
				throw new InvalidOperationException("Out of character slots!");
			}
			int value = num.Value;
			dictionary.Add(value, profile);
			this.Preferences = new PlayerPreferences(dictionary, this.Preferences.SelectedCharacterIndex, this.Preferences.AdminOOCColor);
			this.UpdateCharacter(profile, value);
		}

		// Token: 0x06000A02 RID: 2562 RVA: 0x0003A2DE File Offset: 0x000384DE
		public void DeleteCharacter(ICharacterProfile profile)
		{
			this.DeleteCharacter(this.Preferences.IndexOfCharacter(profile));
		}

		// Token: 0x06000A03 RID: 2563 RVA: 0x0003A2F4 File Offset: 0x000384F4
		public void DeleteCharacter(int slot)
		{
			IEnumerable<KeyValuePair<int, ICharacterProfile>> characters = from p in this.Preferences.Characters
			where p.Key != slot
			select p;
			this.Preferences = new PlayerPreferences(characters, this.Preferences.SelectedCharacterIndex, this.Preferences.AdminOOCColor);
			MsgDeleteCharacter msgDeleteCharacter = new MsgDeleteCharacter
			{
				Slot = slot
			};
			this._netManager.ClientSendMessage(msgDeleteCharacter);
		}

		// Token: 0x06000A04 RID: 2564 RVA: 0x0003A36B File Offset: 0x0003856B
		private void HandlePreferencesAndSettings(MsgPreferencesAndSettings message)
		{
			this.Preferences = message.Preferences;
			this.Settings = message.Settings;
			Action onServerDataLoaded = this.OnServerDataLoaded;
			if (onServerDataLoaded == null)
			{
				return;
			}
			onServerDataLoaded();
		}

		// Token: 0x040004F2 RID: 1266
		[Dependency]
		private readonly IClientNetManager _netManager;

		// Token: 0x040004F3 RID: 1267
		[Dependency]
		private readonly SponsorsManager _sponsorsManager;
	}
}
