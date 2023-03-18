using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Examine;
using Content.Shared.Popups;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.Popups
{
	// Token: 0x020001AA RID: 426
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PopupOverlay : Overlay
	{
		// Token: 0x17000223 RID: 547
		// (get) Token: 0x06000B1C RID: 2844 RVA: 0x00026117 File Offset: 0x00024317
		public override OverlaySpace Space
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x06000B1D RID: 2845 RVA: 0x00040EA0 File Offset: 0x0003F0A0
		public PopupOverlay(IConfigurationManager configManager, IEntityManager entManager, IPlayerManager playerMgr, IPrototypeManager protoManager, IResourceCache cache, IUserInterfaceManager uiManager, PopupSystem popup)
		{
			this._configManager = configManager;
			this._entManager = entManager;
			this._playerMgr = playerMgr;
			this._uiManager = uiManager;
			this._popup = popup;
			this._shader = protoManager.Index<ShaderPrototype>("unshaded").Instance();
			this._smallFont = new VectorFont(cache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Italic.ttf", true), 10);
			this._mediumFont = new VectorFont(cache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Italic.ttf", true), 12);
			this._largeFont = new VectorFont(cache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-BoldItalic.ttf", true), 14);
		}

		// Token: 0x06000B1E RID: 2846 RVA: 0x00040F40 File Offset: 0x0003F140
		protected override void Draw(in OverlayDrawArgs args)
		{
			if (args.ViewportControl == null)
			{
				return;
			}
			args.DrawingHandle.SetTransform(ref Matrix3.Identity);
			args.DrawingHandle.UseShader(this._shader);
			float num = this._configManager.GetCVar<float>(CVars.DisplayUIScale);
			if (num == 0f)
			{
				num = this._uiManager.DefaultUIScale;
			}
			this.DrawWorld(args.ScreenHandle, args, num);
			this.DrawScreen(args.ScreenHandle, args, num);
			args.DrawingHandle.UseShader(null);
		}

		// Token: 0x06000B1F RID: 2847 RVA: 0x00040FD0 File Offset: 0x0003F1D0
		private void DrawWorld(DrawingHandleScreen worldHandle, OverlayDrawArgs args, float scale)
		{
			PopupOverlay.<>c__DisplayClass13_0 CS$<>8__locals1 = new PopupOverlay.<>c__DisplayClass13_0();
			if (this._popup.WorldLabels.Count == 0)
			{
				return;
			}
			Matrix3 worldToScreenMatrix = args.ViewportControl.GetWorldToScreenMatrix();
			MapCoordinates origin;
			origin..ctor(args.WorldAABB.Center, args.MapId);
			PopupOverlay.<>c__DisplayClass13_0 CS$<>8__locals2 = CS$<>8__locals1;
			LocalPlayer localPlayer = this._playerMgr.LocalPlayer;
			CS$<>8__locals2.ourEntity = ((localPlayer != null) ? localPlayer.ControlledEntity : null);
			using (IEnumerator<PopupSystem.WorldPopupLabel> enumerator = this._popup.WorldLabels.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PopupSystem.WorldPopupLabel popup = enumerator.Current;
					MapCoordinates mapCoordinates = popup.InitialPos.ToMap(this._entManager);
					if (!(mapCoordinates.MapId != args.MapId))
					{
						float length = (mapCoordinates.Position - args.WorldBounds.Center).Length;
						if (args.WorldAABB.Contains(mapCoordinates.Position, true) && ExamineSystemShared.InRangeUnOccluded(origin, mapCoordinates, length, (EntityUid e) => e == popup.InitialPos.EntityId || e == CS$<>8__locals1.ourEntity, true, this._entManager))
						{
							Vector2 position = worldToScreenMatrix.Transform(mapCoordinates.Position);
							this.DrawPopup(popup, worldHandle, position, scale);
						}
					}
				}
			}
		}

		// Token: 0x06000B20 RID: 2848 RVA: 0x00041148 File Offset: 0x0003F348
		private void DrawScreen(DrawingHandleScreen screenHandle, OverlayDrawArgs args, float scale)
		{
			foreach (PopupSystem.CursorPopupLabel cursorPopupLabel in this._popup.CursorLabels)
			{
				WindowId window = cursorPopupLabel.InitialPos.Window;
				IViewportControl viewportControl = args.ViewportControl;
				WindowId? windowId;
				if (viewportControl == null)
				{
					windowId = null;
				}
				else
				{
					IClydeWindow window2 = viewportControl.Window;
					windowId = ((window2 != null) ? new WindowId?(window2.Id) : null);
				}
				if (!(window != windowId))
				{
					this.DrawPopup(cursorPopupLabel, screenHandle, cursorPopupLabel.InitialPos.Position, scale);
				}
			}
		}

		// Token: 0x06000B21 RID: 2849 RVA: 0x00041204 File Offset: 0x0003F404
		private void DrawPopup(PopupSystem.PopupLabel popup, DrawingHandleScreen handle, Vector2 position, float scale)
		{
			float num = MathF.Min(1f, 1f - (popup.TotalTime - 0.5f) / 2.5f);
			Vector2 vector = position - new Vector2(0f, 20f * (popup.TotalTime * popup.TotalTime + popup.TotalTime));
			Font font = this._smallFont;
			Color color = Color.White.WithAlpha(num);
			switch (popup.Type)
			{
			case PopupType.SmallCaution:
				color = Color.Red;
				break;
			case PopupType.Medium:
				font = this._mediumFont;
				color = Color.LightGray;
				break;
			case PopupType.MediumCaution:
				font = this._mediumFont;
				color = Color.Red;
				break;
			case PopupType.Large:
				font = this._largeFont;
				color = Color.LightGray;
				break;
			case PopupType.LargeCaution:
				font = this._largeFont;
				color = Color.Red;
				break;
			}
			Vector2 dimensions = handle.GetDimensions(font, popup.Text, scale);
			handle.DrawString(font, vector - dimensions / 2f, popup.Text, scale, color.WithAlpha(num));
		}

		// Token: 0x04000565 RID: 1381
		private readonly IConfigurationManager _configManager;

		// Token: 0x04000566 RID: 1382
		private readonly IEntityManager _entManager;

		// Token: 0x04000567 RID: 1383
		private readonly IPlayerManager _playerMgr;

		// Token: 0x04000568 RID: 1384
		private readonly IUserInterfaceManager _uiManager;

		// Token: 0x04000569 RID: 1385
		private readonly PopupSystem _popup;

		// Token: 0x0400056A RID: 1386
		private readonly ShaderInstance _shader;

		// Token: 0x0400056B RID: 1387
		private readonly Font _smallFont;

		// Token: 0x0400056C RID: 1388
		private readonly Font _mediumFont;

		// Token: 0x0400056D RID: 1389
		private readonly Font _largeFont;
	}
}
