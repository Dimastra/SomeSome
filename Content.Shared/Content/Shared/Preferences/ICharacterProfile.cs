using System;
using System.Runtime.CompilerServices;
using Content.Shared.Humanoid;

namespace Content.Shared.Preferences
{
	// Token: 0x02000247 RID: 583
	[NullableContext(1)]
	public interface ICharacterProfile
	{
		// Token: 0x17000151 RID: 337
		// (get) Token: 0x060006C9 RID: 1737
		string Name { get; }

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x060006CA RID: 1738
		ICharacterAppearance CharacterAppearance { get; }

		// Token: 0x060006CB RID: 1739
		bool MemberwiseEquals(ICharacterProfile other);

		// Token: 0x060006CC RID: 1740
		void EnsureValid(string[] sponsorMarkings);
	}
}
