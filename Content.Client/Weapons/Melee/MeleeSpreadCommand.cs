using System;
using System.Runtime.CompilerServices;
using Content.Shared.CombatMode;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Weapons.Melee
{
	// Token: 0x0200003C RID: 60
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MeleeSpreadCommand : IConsoleCommand
	{
		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000101 RID: 257 RVA: 0x000090AB File Offset: 0x000072AB
		public string Command
		{
			get
			{
				return "showmeleespread";
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000102 RID: 258 RVA: 0x000090B2 File Offset: 0x000072B2
		public string Description
		{
			get
			{
				return "Shows the current weapon's range and arc for debugging";
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000103 RID: 259 RVA: 0x000090B9 File Offset: 0x000072B9
		public string Help
		{
			get
			{
				return this.Command ?? "";
			}
		}

		// Token: 0x06000104 RID: 260 RVA: 0x000090CC File Offset: 0x000072CC
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IDependencyCollection instance = IoCManager.Instance;
			if (instance == null)
			{
				return;
			}
			IOverlayManager overlayManager = instance.Resolve<IOverlayManager>();
			if (overlayManager.RemoveOverlay<MeleeArcOverlay>())
			{
				return;
			}
			IEntitySystemManager entitySystemManager = instance.Resolve<IEntitySystemManager>();
			overlayManager.AddOverlay(new MeleeArcOverlay(instance.Resolve<IEntityManager>(), instance.Resolve<IEyeManager>(), instance.Resolve<IInputManager>(), instance.Resolve<IPlayerManager>(), entitySystemManager.GetEntitySystem<MeleeWeaponSystem>(), entitySystemManager.GetEntitySystem<SharedCombatModeSystem>()));
		}
	}
}
