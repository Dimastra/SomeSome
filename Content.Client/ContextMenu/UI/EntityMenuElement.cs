using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Client.Administration.Managers;
using Content.Client.Administration.Systems;
using Content.Shared.Administration;
using Content.Shared.IdentityManagement;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Players;

namespace Content.Client.ContextMenu.UI
{
	// Token: 0x02000384 RID: 900
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EntityMenuElement : ContextMenuElement
	{
		// Token: 0x0600161A RID: 5658 RVA: 0x00082434 File Offset: 0x00080634
		public EntityMenuElement(EntityUid? entity = null) : base(null)
		{
			IoCManager.InjectDependencies<EntityMenuElement>(this);
			this._adminSystem = this._entityManager.System<AdminSystem>();
			this.CountLabel = new Label
			{
				StyleClasses = 
				{
					"contextMenuCount"
				}
			};
			Control icon = base.Icon;
			LayoutContainer layoutContainer = new LayoutContainer();
			layoutContainer.Children.Add(this.EntityIcon);
			layoutContainer.Children.Add(this.CountLabel);
			icon.AddChild(layoutContainer);
			LayoutContainer.SetAnchorPreset(this.CountLabel, 3, false);
			LayoutContainer.SetGrowHorizontal(this.CountLabel, 1);
			LayoutContainer.SetGrowVertical(this.CountLabel, 1);
			this.Entity = entity;
			if (this.Entity == null)
			{
				return;
			}
			this.Count = 1;
			this.CountLabel.Visible = false;
			this.UpdateEntity(null);
		}

		// Token: 0x0600161B RID: 5659 RVA: 0x00082520 File Offset: 0x00080720
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			this.Entity = null;
			this.Count = 0;
		}

		// Token: 0x0600161C RID: 5660 RVA: 0x0008253C File Offset: 0x0008073C
		public void UpdateEntity(EntityUid? entity = null)
		{
			EntityUid? entity2 = entity;
			if (entity2 == null)
			{
				entity = this.Entity;
			}
			if (this._entityManager.Deleted(entity))
			{
				this.Text = string.Empty;
				this.EntityIcon.Sprite = null;
				return;
			}
			this.EntityIcon.Sprite = EntityManagerExt.GetComponentOrNull<SpriteComponent>(this._entityManager, entity);
			if (this._adminManager.HasFlag(AdminFlags.Admin | AdminFlags.Debug))
			{
				EntityStringRepresentation entityStringRepresentation = this._entityManager.ToPrettyString(entity.Value);
				string name = entityStringRepresentation.Name;
				EntityUid uid = entityStringRepresentation.Uid;
				string prototype = entityStringRepresentation.Prototype;
				ICommonSession session = entityStringRepresentation.Session;
				string text;
				if ((text = ((session != null) ? session.Name : null)) == null)
				{
					PlayerInfo playerInfo = this._adminSystem.PlayerList.FirstOrDefault((PlayerInfo player) => player.EntityUid == entity);
					text = ((playerInfo != null) ? playerInfo.Username : null);
				}
				string text2 = text;
				bool deleted = entityStringRepresentation.Deleted;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 5);
				defaultInterpolatedStringHandler.AppendFormatted(name);
				defaultInterpolatedStringHandler.AppendLiteral(" (");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(uid);
				defaultInterpolatedStringHandler.AppendFormatted((entityStringRepresentation.Prototype != null) ? (", " + prototype) : "");
				defaultInterpolatedStringHandler.AppendFormatted((text2 != null) ? (", " + text2) : "");
				defaultInterpolatedStringHandler.AppendLiteral(")");
				defaultInterpolatedStringHandler.AppendFormatted(deleted ? "D" : "");
				this.Text = defaultInterpolatedStringHandler.ToStringAndClear();
				return;
			}
			this.Text = Identity.Name(entity.Value, this._entityManager, this._playerManager.LocalPlayer.ControlledEntity);
		}

		// Token: 0x04000B92 RID: 2962
		public const string StyleClassEntityMenuCountText = "contextMenuCount";

		// Token: 0x04000B93 RID: 2963
		[Dependency]
		private readonly IClientAdminManager _adminManager;

		// Token: 0x04000B94 RID: 2964
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04000B95 RID: 2965
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000B96 RID: 2966
		private AdminSystem _adminSystem;

		// Token: 0x04000B97 RID: 2967
		public EntityUid? Entity;

		// Token: 0x04000B98 RID: 2968
		public int Count;

		// Token: 0x04000B99 RID: 2969
		public readonly Label CountLabel;

		// Token: 0x04000B9A RID: 2970
		public readonly SpriteView EntityIcon = new SpriteView
		{
			OverrideDirection = new Direction?(0)
		};
	}
}
