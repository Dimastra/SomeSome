using System;
using System.Runtime.CompilerServices;
using Robust.Client.Graphics;
using Robust.Shared.Maths;

namespace Content.Client.Clickable
{
	// Token: 0x020003C4 RID: 964
	[NullableContext(1)]
	public interface IClickMapManager
	{
		// Token: 0x060017E8 RID: 6120
		bool IsOccluding(Texture texture, Vector2i pos);

		// Token: 0x060017E9 RID: 6121
		bool IsOccluding(RSI rsi, RSI.StateId state, RSI.State.Direction dir, int frame, Vector2i pos);
	}
}
