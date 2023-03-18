using System;
using System.Runtime.CompilerServices;
using Content.Client.Administration.Managers;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Content.Client.White.Sponsors;
using Content.Shared.Humanoid;
using Content.Shared.White.MeatyOre;
using Content.Shared.White.Sponsors;
using Robust.Client.Player;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Client.White.MeatyOre
{
	// Token: 0x02000022 RID: 34
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MeatyOreUIController : UIController
	{
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600007A RID: 122 RVA: 0x00004C35 File Offset: 0x00002E35
		[Nullable(2)]
		private MenuButton MeatyOreButton
		{
			[NullableContext(2)]
			get
			{
				GameTopMenuBar activeUIWidgetOrNull = this.UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>();
				if (activeUIWidgetOrNull == null)
				{
					return null;
				}
				return activeUIWidgetOrNull.MeatyOreButton;
			}
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00004C4D File Offset: 0x00002E4D
		public void LoadButton()
		{
			this.MeatyOreButton.OnPressed += this.MeatyOreButtonPressed;
			this._buttonLoaded = true;
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00004C6D File Offset: 0x00002E6D
		public void UnloadButton()
		{
			this.MeatyOreButton.OnPressed -= this.MeatyOreButtonPressed;
			this._buttonLoaded = false;
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00004C8D File Offset: 0x00002E8D
		private void MeatyOreButtonPressed(BaseButton.ButtonEventArgs obj)
		{
			this._entityNetworkManager.SendSystemNetworkMessage(new MeatyOreShopRequestEvent(), true);
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00004CA0 File Offset: 0x00002EA0
		public override void FrameUpdate(FrameEventArgs args)
		{
			base.FrameUpdate(args);
			if (!this._buttonLoaded)
			{
				return;
			}
			bool visible = this.CheckButtonVisibility();
			this.MeatyOreButton.Visible = visible;
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00004CD0 File Offset: 0x00002ED0
		private bool CheckButtonVisibility()
		{
			SponsorInfo sponsorInfo;
			if (!this._sponsorsManager.TryGetInfo(out sponsorInfo))
			{
				return false;
			}
			if (sponsorInfo == null || sponsorInfo.Tier == null || (sponsorInfo != null && sponsorInfo.MeatyOreCoin == 0))
			{
				return false;
			}
			EntityUid? controlledEntity = this._playerManager.LocalPlayer.ControlledEntity;
			return controlledEntity != null && this._entityManager.HasComponent<HumanoidAppearanceComponent>(controlledEntity);
		}

		// Token: 0x04000046 RID: 70
		[Dependency]
		private readonly IClientAdminManager _clientAdminManager;

		// Token: 0x04000047 RID: 71
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04000048 RID: 72
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000049 RID: 73
		[Dependency]
		private readonly IEntityNetworkManager _entityNetworkManager;

		// Token: 0x0400004A RID: 74
		[Dependency]
		private readonly SponsorsManager _sponsorsManager;

		// Token: 0x0400004B RID: 75
		private bool _buttonLoaded;
	}
}
