using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Content.Server.Maps
{
	// Token: 0x020003D8 RID: 984
	[NullableContext(1)]
	public interface IGameMapManager
	{
		// Token: 0x06001437 RID: 5175
		void Initialize();

		// Token: 0x06001438 RID: 5176
		IEnumerable<GameMapPrototype> CurrentlyEligibleMaps();

		// Token: 0x06001439 RID: 5177
		IEnumerable<GameMapPrototype> AllVotableMaps();

		// Token: 0x0600143A RID: 5178
		IEnumerable<GameMapPrototype> AllMaps();

		// Token: 0x0600143B RID: 5179
		[NullableContext(2)]
		GameMapPrototype GetSelectedMap();

		// Token: 0x0600143C RID: 5180
		void ClearSelectedMap();

		// Token: 0x0600143D RID: 5181
		bool TrySelectMapIfEligible(string gameMap);

		// Token: 0x0600143E RID: 5182
		void SelectMap(string gameMap);

		// Token: 0x0600143F RID: 5183
		void SelectMapRandom();

		// Token: 0x06001440 RID: 5184
		void SelectMapFromRotationQueue(bool markAsPlayed = false);

		// Token: 0x06001441 RID: 5185
		void SelectMapByConfigRules();

		// Token: 0x06001442 RID: 5186
		bool CheckMapExists(string gameMap);
	}
}
