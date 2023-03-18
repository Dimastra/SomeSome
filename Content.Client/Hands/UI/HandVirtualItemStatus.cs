using System;
using CompiledRobustXaml;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Hands.UI
{
	// Token: 0x020002DE RID: 734
	public sealed class HandVirtualItemStatus : Control
	{
		// Token: 0x0600127B RID: 4731 RVA: 0x0006E5DA File Offset: 0x0006C7DA
		public HandVirtualItemStatus()
		{
			HandVirtualItemStatus.!XamlIlPopulateTrampoline(this);
		}

		// Token: 0x0600127C RID: 4732 RVA: 0x0006E5E8 File Offset: 0x0006C7E8
		static void xaml(IServiceProvider A_0, Control A_1)
		{
			XamlIlContext.Context<Control> context = new XamlIlContext.Context<Control>(A_0, null, "resm:Content.Client.Hands.UI.HandVirtualItemStatus.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			Label label = new Label();
			string item = "ItemStatus";
			label.StyleClasses.Add(item);
			label.Text = "Blocked by";
			Control control = label;
			A_1.XamlChildren.Add(control);
			if ((control = (A_1 as Control)) != null)
			{
				context.RobustNameScope.Absorb(control.NameScope);
				control.NameScope = context.RobustNameScope;
			}
			context.RobustNameScope.Complete();
		}

		// Token: 0x0600127D RID: 4733 RVA: 0x0006E69B File Offset: 0x0006C89B
		private static void !XamlIlPopulateTrampoline(HandVirtualItemStatus A_0)
		{
			HandVirtualItemStatus.Populate:Content.Client.Hands.UI.HandVirtualItemStatus.xaml(null, A_0);
		}
	}
}
