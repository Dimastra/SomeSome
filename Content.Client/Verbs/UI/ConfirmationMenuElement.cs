using System;
using System.Runtime.CompilerServices;
using Content.Client.ContextMenu.UI;
using Content.Shared.Verbs;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.Verbs.UI
{
	// Token: 0x02000061 RID: 97
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ConfirmationMenuElement : ContextMenuElement
	{
		// Token: 0x17000041 RID: 65
		// (set) Token: 0x060001C4 RID: 452 RVA: 0x0000CA78 File Offset: 0x0000AC78
		public override string Text
		{
			set
			{
				FormattedMessage formattedMessage = new FormattedMessage();
				formattedMessage.PushColor(Color.White);
				formattedMessage.AddMarkupPermissive(value.Trim());
				base.Label.SetMessage(formattedMessage);
			}
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x0000CAAE File Offset: 0x0000ACAE
		public ConfirmationMenuElement(Verb verb, [Nullable(2)] string text) : base(text)
		{
			this.Verb = verb;
			base.Icon.Visible = false;
			base.SetOnlyStyleClass("confirmationContextMenuButton");
		}

		// Token: 0x0400012E RID: 302
		public const string StyleClassConfirmationContextMenuButton = "confirmationContextMenuButton";

		// Token: 0x0400012F RID: 303
		public readonly Verb Verb;
	}
}
