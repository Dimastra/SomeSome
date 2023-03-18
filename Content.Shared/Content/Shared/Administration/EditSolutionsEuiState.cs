using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Components;
using Content.Shared.Eui;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration
{
	// Token: 0x02000735 RID: 1845
	[NetSerializable]
	[Serializable]
	public sealed class EditSolutionsEuiState : EuiStateBase
	{
		// Token: 0x06001657 RID: 5719 RVA: 0x00049249 File Offset: 0x00047449
		public EditSolutionsEuiState(EntityUid target, [Nullable(new byte[]
		{
			2,
			1,
			1
		})] Dictionary<string, Solution> solutions)
		{
			this.Target = target;
			this.Solutions = solutions;
		}

		// Token: 0x040016AB RID: 5803
		public readonly EntityUid Target;

		// Token: 0x040016AC RID: 5804
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		public readonly Dictionary<string, Solution> Solutions;
	}
}
