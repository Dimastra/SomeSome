using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Preferences
{
	// Token: 0x0200024D RID: 589
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class PlayerPreferences
	{
		// Token: 0x060006DD RID: 1757 RVA: 0x00018170 File Offset: 0x00016370
		public PlayerPreferences([Nullable(new byte[]
		{
			1,
			0,
			1
		})] IEnumerable<KeyValuePair<int, ICharacterProfile>> characters, int selectedCharacterIndex, Color adminOOCColor)
		{
			this._characters = new Dictionary<int, ICharacterProfile>(characters);
			this.SelectedCharacterIndex = selectedCharacterIndex;
			this.AdminOOCColor = adminOOCColor;
		}

		// Token: 0x17000157 RID: 343
		// (get) Token: 0x060006DE RID: 1758 RVA: 0x00018192 File Offset: 0x00016392
		public IReadOnlyDictionary<int, ICharacterProfile> Characters
		{
			get
			{
				return this._characters;
			}
		}

		// Token: 0x060006DF RID: 1759 RVA: 0x0001819A File Offset: 0x0001639A
		public ICharacterProfile GetProfile(int index)
		{
			return this._characters[index];
		}

		// Token: 0x17000158 RID: 344
		// (get) Token: 0x060006E0 RID: 1760 RVA: 0x000181A8 File Offset: 0x000163A8
		public int SelectedCharacterIndex { get; }

		// Token: 0x17000159 RID: 345
		// (get) Token: 0x060006E1 RID: 1761 RVA: 0x000181B0 File Offset: 0x000163B0
		public ICharacterProfile SelectedCharacter
		{
			get
			{
				return this.Characters[this.SelectedCharacterIndex];
			}
		}

		// Token: 0x1700015A RID: 346
		// (get) Token: 0x060006E2 RID: 1762 RVA: 0x000181C3 File Offset: 0x000163C3
		// (set) Token: 0x060006E3 RID: 1763 RVA: 0x000181CB File Offset: 0x000163CB
		public Color AdminOOCColor { get; set; }

		// Token: 0x060006E4 RID: 1764 RVA: 0x000181D4 File Offset: 0x000163D4
		public int IndexOfCharacter(ICharacterProfile profile)
		{
			if (Extensions.FirstOrNull<KeyValuePair<int, ICharacterProfile>>(this._characters, (KeyValuePair<int, ICharacterProfile> p) => p.Value == profile) == null)
			{
				return -1;
			}
			KeyValuePair<int, ICharacterProfile>? keyValuePair;
			return keyValuePair.GetValueOrDefault().Key;
		}

		// Token: 0x060006E5 RID: 1765 RVA: 0x00018220 File Offset: 0x00016420
		public bool TryIndexOfCharacter(ICharacterProfile profile, out int index)
		{
			return (index = this.IndexOfCharacter(profile)) != -1;
		}

		// Token: 0x040006A0 RID: 1696
		private Dictionary<int, ICharacterProfile> _characters;
	}
}
