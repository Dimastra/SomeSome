using System;
using System.Runtime.CompilerServices;
using Content.Shared.Verbs;
using Robust.Client.UserInterface.Controls;
using Robust.Client.Utility;
using Robust.Shared.Maths;

namespace Content.Client.Examine
{
	// Token: 0x02000329 RID: 809
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ExamineButton : ContainerButton
	{
		// Token: 0x0600144C RID: 5196 RVA: 0x0007711C File Offset: 0x0007531C
		public ExamineButton(ExamineVerb verb)
		{
			base.Margin = new Thickness(4f, 4f, 4f, 4f);
			base.SetOnlyStyleClass("examine-button");
			this.Verb = verb;
			if (verb.Disabled)
			{
				base.Disabled = true;
			}
			base.ToolTip = (verb.Message ?? verb.Text);
			base.TooltipDelay = new float?(0.3f);
			this.Icon = new TextureRect
			{
				SetWidth = 32f,
				SetHeight = 32f
			};
			if (verb.Icon != null)
			{
				this.Icon.Texture = SpriteSpecifierExt.Frame0(verb.Icon);
				this.Icon.Stretch = 7;
				base.AddChild(this.Icon);
			}
		}

		// Token: 0x04000A3A RID: 2618
		public const string StyleClassExamineButton = "examine-button";

		// Token: 0x04000A3B RID: 2619
		public const int ElementHeight = 32;

		// Token: 0x04000A3C RID: 2620
		public const int ElementWidth = 32;

		// Token: 0x04000A3D RID: 2621
		private const int Thickness = 4;

		// Token: 0x04000A3E RID: 2622
		public TextureRect Icon;

		// Token: 0x04000A3F RID: 2623
		public ExamineVerb Verb;
	}
}
