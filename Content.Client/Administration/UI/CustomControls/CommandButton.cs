using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Client.Guidebook.Richtext;
using Robust.Client.Console;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;

namespace Content.Client.Administration.UI.CustomControls
{
	// Token: 0x020004C8 RID: 1224
	[NullableContext(2)]
	[Nullable(0)]
	[Virtual]
	public class CommandButton : Button, IDocumentTag
	{
		// Token: 0x170006CA RID: 1738
		// (get) Token: 0x06001F1E RID: 7966 RVA: 0x000B6541 File Offset: 0x000B4741
		// (set) Token: 0x06001F1F RID: 7967 RVA: 0x000B6549 File Offset: 0x000B4749
		public string Command { get; set; }

		// Token: 0x06001F20 RID: 7968 RVA: 0x000B6552 File Offset: 0x000B4752
		public CommandButton()
		{
			base.OnPressed += this.Execute;
		}

		// Token: 0x06001F21 RID: 7969 RVA: 0x000B656D File Offset: 0x000B476D
		protected virtual bool CanPress()
		{
			return string.IsNullOrEmpty(this.Command) || IoCManager.Resolve<IClientConGroupController>().CanCommand(this.Command.Split(' ', StringSplitOptions.None)[0]);
		}

		// Token: 0x06001F22 RID: 7970 RVA: 0x000B6598 File Offset: 0x000B4798
		protected override void EnteredTree()
		{
			if (!this.CanPress())
			{
				base.Visible = false;
			}
		}

		// Token: 0x06001F23 RID: 7971 RVA: 0x000B65A9 File Offset: 0x000B47A9
		[NullableContext(1)]
		protected virtual void Execute(BaseButton.ButtonEventArgs obj)
		{
			if (!string.IsNullOrEmpty(this.Command))
			{
				IoCManager.Resolve<IClientConsoleHost>().ExecuteCommand(this.Command);
			}
		}

		// Token: 0x06001F24 RID: 7972 RVA: 0x000B65C8 File Offset: 0x000B47C8
		[NullableContext(1)]
		public bool TryParseTag(Dictionary<string, string> args, [Nullable(2)] [NotNullWhen(true)] out Control control)
		{
			string text;
			string command;
			if (args.Count != 2 || !args.TryGetValue("Text", out text) || !args.TryGetValue("Command", out command))
			{
				Logger.Error("Invalid arguments passed to CommandButton");
				control = null;
				return false;
			}
			this.Command = command;
			base.Text = Loc.GetString(text);
			control = this;
			return true;
		}
	}
}
