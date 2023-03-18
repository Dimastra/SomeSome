using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Guidebook.Richtext
{
	// Token: 0x020002EE RID: 750
	public sealed class Box : BoxContainer, IDocumentTag
	{
		// Token: 0x060012E4 RID: 4836 RVA: 0x000708A0 File Offset: 0x0006EAA0
		[NullableContext(1)]
		public bool TryParseTag(Dictionary<string, string> args, [Nullable(2)] [NotNullWhen(true)] out Control control)
		{
			base.HorizontalExpand = true;
			control = this;
			string value;
			if (args.TryGetValue("Orientation", out value))
			{
				base.Orientation = Enum.Parse<BoxContainer.LayoutOrientation>(value);
			}
			else
			{
				base.Orientation = 0;
			}
			string value2;
			if (args.TryGetValue("HorizontalAlignment", out value2))
			{
				base.HorizontalAlignment = Enum.Parse<Control.HAlignment>(value2);
			}
			else
			{
				base.HorizontalAlignment = 2;
			}
			string value3;
			if (args.TryGetValue("VerticalAlignment", out value3))
			{
				base.VerticalAlignment = Enum.Parse<Control.VAlignment>(value3);
			}
			return true;
		}
	}
}
