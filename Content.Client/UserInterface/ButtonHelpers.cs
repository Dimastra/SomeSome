using System;
using System.Runtime.CompilerServices;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.UserInterface
{
	// Token: 0x0200006B RID: 107
	public static class ButtonHelpers
	{
		// Token: 0x060001F8 RID: 504 RVA: 0x0000E16C File Offset: 0x0000C36C
		[NullableContext(1)]
		public static void SetButtonDisabledRecursive(Control parent, bool val)
		{
			foreach (Control control in parent.Children)
			{
				Button button = control as Button;
				if (button != null)
				{
					button.Disabled = val;
				}
				else if (control.ChildCount > 0)
				{
					ButtonHelpers.SetButtonDisabledRecursive(control, val);
				}
			}
		}
	}
}
