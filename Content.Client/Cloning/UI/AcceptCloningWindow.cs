using System;
using System.Runtime.CompilerServices;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;

namespace Content.Client.Cloning.UI
{
	// Token: 0x020003BE RID: 958
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AcceptCloningWindow : DefaultWindow
	{
		// Token: 0x060017D2 RID: 6098 RVA: 0x00088FA0 File Offset: 0x000871A0
		public AcceptCloningWindow()
		{
			base.Title = Loc.GetString("accept-cloning-window-title");
			Control contents = base.Contents;
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			Control.OrderedChildCollection children = boxContainer.Children;
			BoxContainer boxContainer2 = new BoxContainer();
			boxContainer2.Orientation = 1;
			boxContainer2.Children.Add(new Label
			{
				Text = Loc.GetString("accept-cloning-window-prompt-text-part")
			});
			Control.OrderedChildCollection children2 = boxContainer2.Children;
			BoxContainer boxContainer3 = new BoxContainer();
			boxContainer3.Orientation = 0;
			boxContainer3.Align = 1;
			Control.OrderedChildCollection children3 = boxContainer3.Children;
			Button button = new Button();
			button.Text = Loc.GetString("accept-cloning-window-accept-button");
			Button button2 = button;
			this.AcceptButton = button;
			children3.Add(button2);
			boxContainer3.Children.Add(new Control
			{
				MinSize = new ValueTuple<float, float>(20f, 0f)
			});
			Control.OrderedChildCollection children4 = boxContainer3.Children;
			Button button3 = new Button();
			button3.Text = Loc.GetString("accept-cloning-window-deny-button");
			button2 = button3;
			this.DenyButton = button3;
			children4.Add(button2);
			children2.Add(boxContainer3);
			children.Add(boxContainer2);
			contents.AddChild(boxContainer);
		}

		// Token: 0x04000C1E RID: 3102
		public readonly Button DenyButton;

		// Token: 0x04000C1F RID: 3103
		public readonly Button AcceptButton;
	}
}
