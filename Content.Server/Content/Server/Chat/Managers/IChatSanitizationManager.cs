using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Server.Chat.Managers
{
	// Token: 0x020006CC RID: 1740
	[NullableContext(1)]
	public interface IChatSanitizationManager
	{
		// Token: 0x06002456 RID: 9302
		void Initialize();

		// Token: 0x06002457 RID: 9303
		bool TrySanitizeOutSmilies(string input, EntityUid speaker, out string sanitized, [Nullable(2)] [NotNullWhen(true)] out string emote);

		// Token: 0x06002458 RID: 9304
		string SanitizeOutSlang(string input);
	}
}
