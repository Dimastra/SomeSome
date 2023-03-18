using System;
using System.Runtime.CompilerServices;
using Content.Client.Resources;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Systems.Chat.Controls
{
	// Token: 0x020000A7 RID: 167
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ChannelFilterButton : ContainerButton
	{
		// Token: 0x0600044B RID: 1099 RVA: 0x00018CD4 File Offset: 0x00016ED4
		public ChannelFilterButton()
		{
			this._chatUIController = base.UserInterfaceManager.GetUIController<ChatUIController>();
			Texture texture = IoCManager.Resolve<IResourceCache>().GetTexture("/Textures/Interface/Nano/filter.svg.96dpi.png");
			base.Mode = 0;
			base.EnableAllKeybinds = true;
			TextureRect textureRect = new TextureRect();
			textureRect.Texture = texture;
			textureRect.HorizontalAlignment = 2;
			textureRect.VerticalAlignment = 2;
			TextureRect textureRect2 = textureRect;
			this._textureRect = textureRect;
			base.AddChild(textureRect2);
			base.ToggleMode = true;
			base.OnToggled += this.OnFilterButtonToggled;
			this.ChatFilterPopup = base.UserInterfaceManager.CreatePopup<ChannelFilterPopup>();
			this.ChatFilterPopup.OnVisibilityChanged += this.PopupVisibilityChanged;
			this._chatUIController.FilterableChannelsChanged += this.ChatFilterPopup.SetChannels;
			this._chatUIController.UnreadMessageCountsUpdated += this.ChatFilterPopup.UpdateUnread;
			this.ChatFilterPopup.SetChannels(this._chatUIController.FilterableChannels);
		}

		// Token: 0x0600044C RID: 1100 RVA: 0x00018DCE File Offset: 0x00016FCE
		private void PopupVisibilityChanged(Control control)
		{
			base.Pressed = control.Visible;
		}

		// Token: 0x0600044D RID: 1101 RVA: 0x00018DDC File Offset: 0x00016FDC
		private void OnFilterButtonToggled(BaseButton.ButtonToggledEventArgs args)
		{
			if (args.Pressed)
			{
				Vector2 globalPosition = base.GlobalPosition;
				float num;
				float num2;
				this.ChatFilterPopup.MinSize.Deconstruct(ref num, ref num2);
				float val = num;
				float item = num2;
				UIBox2 value = UIBox2.FromDimensions(globalPosition - new ValueTuple<float, float>(120f, 0f), new ValueTuple<float, float>(Math.Max(val, this.ChatFilterPopup.MinWidth), item));
				this.ChatFilterPopup.Open(new UIBox2?(value), null);
				return;
			}
			this.ChatFilterPopup.Close();
		}

		// Token: 0x0600044E RID: 1102 RVA: 0x00018E76 File Offset: 0x00017076
		protected override void KeyBindDown(GUIBoundKeyEventArgs args)
		{
			if (args.Function == EngineKeyFunctions.Use)
			{
				return;
			}
			base.KeyBindDown(args);
		}

		// Token: 0x0600044F RID: 1103 RVA: 0x00018E94 File Offset: 0x00017094
		private void UpdateChildColors()
		{
			if (this._textureRect == null)
			{
				return;
			}
			switch (base.DrawMode)
			{
			case 0:
				this._textureRect.ModulateSelfOverride = new Color?(ChannelFilterButton.ColorNormal);
				return;
			case 1:
				this._textureRect.ModulateSelfOverride = new Color?(ChannelFilterButton.ColorPressed);
				return;
			case 2:
				this._textureRect.ModulateSelfOverride = new Color?(ChannelFilterButton.ColorHovered);
				break;
			case 3:
				break;
			default:
				return;
			}
		}

		// Token: 0x06000450 RID: 1104 RVA: 0x00018F09 File Offset: 0x00017109
		protected override void DrawModeChanged()
		{
			base.DrawModeChanged();
			this.UpdateChildColors();
		}

		// Token: 0x06000451 RID: 1105 RVA: 0x00018F17 File Offset: 0x00017117
		protected override void StylePropertiesChanged()
		{
			base.StylePropertiesChanged();
			this.UpdateChildColors();
		}

		// Token: 0x06000452 RID: 1106 RVA: 0x00018F28 File Offset: 0x00017128
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			this._chatUIController.FilterableChannelsChanged -= this.ChatFilterPopup.SetChannels;
			this._chatUIController.UnreadMessageCountsUpdated -= this.ChatFilterPopup.UpdateUnread;
		}

		// Token: 0x0400020F RID: 527
		private static readonly Color ColorNormal = Color.FromHex("#7b7e7e", null);

		// Token: 0x04000210 RID: 528
		private static readonly Color ColorHovered = Color.FromHex("#969999", null);

		// Token: 0x04000211 RID: 529
		private static readonly Color ColorPressed = Color.FromHex("#789B8C", null);

		// Token: 0x04000212 RID: 530
		[Nullable(2)]
		private readonly TextureRect _textureRect;

		// Token: 0x04000213 RID: 531
		public readonly ChannelFilterPopup ChatFilterPopup;

		// Token: 0x04000214 RID: 532
		private readonly ChatUIController _chatUIController;

		// Token: 0x04000215 RID: 533
		private const int FilterDropdownOffset = 120;
	}
}
