using System;
using System.Runtime.CompilerServices;
using Content.Shared.Preferences;

namespace Content.Client.Preferences
{
	// Token: 0x02000182 RID: 386
	[NullableContext(1)]
	public interface IClientPreferencesManager
	{
		// Token: 0x14000046 RID: 70
		// (add) Token: 0x06000A08 RID: 2568
		// (remove) Token: 0x06000A09 RID: 2569
		event Action OnServerDataLoaded;

		// Token: 0x170001CE RID: 462
		// (get) Token: 0x06000A0A RID: 2570 RVA: 0x0003A3A9 File Offset: 0x000385A9
		bool ServerDataLoaded
		{
			get
			{
				return this.Settings != null;
			}
		}

		// Token: 0x170001CF RID: 463
		// (get) Token: 0x06000A0B RID: 2571
		[Nullable(2)]
		GameSettings Settings { [NullableContext(2)] get; }

		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x06000A0C RID: 2572
		[Nullable(2)]
		PlayerPreferences Preferences { [NullableContext(2)] get; }

		// Token: 0x06000A0D RID: 2573
		void Initialize();

		// Token: 0x06000A0E RID: 2574
		void SelectCharacter(ICharacterProfile profile);

		// Token: 0x06000A0F RID: 2575
		void SelectCharacter(int slot);

		// Token: 0x06000A10 RID: 2576
		void UpdateCharacter(ICharacterProfile profile, int slot);

		// Token: 0x06000A11 RID: 2577
		void CreateCharacter(ICharacterProfile profile);

		// Token: 0x06000A12 RID: 2578
		void DeleteCharacter(ICharacterProfile profile);

		// Token: 0x06000A13 RID: 2579
		void DeleteCharacter(int slot);
	}
}
