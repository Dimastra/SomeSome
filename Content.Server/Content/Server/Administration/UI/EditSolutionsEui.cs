using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Systems;
using Content.Server.Chemistry.Components.SolutionManager;
using Content.Server.EUI;
using Content.Shared.Administration;
using Content.Shared.Chemistry.Components;
using Content.Shared.Eui;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Administration.UI
{
	// Token: 0x02000807 RID: 2055
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EditSolutionsEui : BaseEui
	{
		// Token: 0x06002C8C RID: 11404 RVA: 0x000E8383 File Offset: 0x000E6583
		public EditSolutionsEui(EntityUid entity)
		{
			IoCManager.InjectDependencies<EditSolutionsEui>(this);
			this.Target = entity;
		}

		// Token: 0x06002C8D RID: 11405 RVA: 0x000E8399 File Offset: 0x000E6599
		public override void Opened()
		{
			base.Opened();
			base.StateDirty();
		}

		// Token: 0x06002C8E RID: 11406 RVA: 0x000E83A7 File Offset: 0x000E65A7
		public override void Closed()
		{
			base.Closed();
			EntitySystem.Get<AdminVerbSystem>().OnEditSolutionsEuiClosed(base.Player);
		}

		// Token: 0x06002C8F RID: 11407 RVA: 0x000E83C0 File Offset: 0x000E65C0
		public override EuiStateBase GetNewState()
		{
			SolutionContainerManagerComponent componentOrNull = EntityManagerExt.GetComponentOrNull<SolutionContainerManagerComponent>(this._entityManager, this.Target);
			Dictionary<string, Solution> solutions = (componentOrNull != null) ? componentOrNull.Solutions : null;
			return new EditSolutionsEuiState(this.Target, solutions);
		}

		// Token: 0x06002C90 RID: 11408 RVA: 0x000E83F7 File Offset: 0x000E65F7
		public override void HandleMessage(EuiMessageBase msg)
		{
			if (msg is EditSolutionsEuiMsg.Close)
			{
				base.Close();
			}
		}

		// Token: 0x04001B87 RID: 7047
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04001B88 RID: 7048
		public readonly EntityUid Target;
	}
}
