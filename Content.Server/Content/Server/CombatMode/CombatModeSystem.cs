using System;
using System.Runtime.CompilerServices;
using Content.Shared.CombatMode;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Server.CombatMode
{
	// Token: 0x02000633 RID: 1587
	public sealed class CombatModeSystem : SharedCombatModeSystem
	{
		// Token: 0x060021CE RID: 8654 RVA: 0x000B06BE File Offset: 0x000AE8BE
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SharedCombatModeComponent, ComponentGetState>(new ComponentEventRefHandler<SharedCombatModeComponent, ComponentGetState>(this.OnGetState), null, null);
		}

		// Token: 0x060021CF RID: 8655 RVA: 0x000B06DA File Offset: 0x000AE8DA
		[NullableContext(1)]
		private void OnGetState(EntityUid uid, SharedCombatModeComponent component, ref ComponentGetState args)
		{
			args.State = new SharedCombatModeSystem.CombatModeComponentState(component.IsInCombatMode, component.ActiveZone);
		}
	}
}
