using System;
using System.Runtime.CompilerServices;
using Content.Client.Chat.Managers;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Chat.UI
{
	// Token: 0x020003E6 RID: 998
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TextSpeechBubble : SpeechBubble
	{
		// Token: 0x0600188E RID: 6286 RVA: 0x0008DAE9 File Offset: 0x0008BCE9
		public TextSpeechBubble(string text, EntityUid senderEntity, IEyeManager eyeManager, IChatManager chatManager, IEntityManager entityManager, string speechStyleClass) : base(text, senderEntity, eyeManager, chatManager, entityManager, speechStyleClass)
		{
		}

		// Token: 0x0600188F RID: 6287 RVA: 0x0008DAFC File Offset: 0x0008BCFC
		protected override Control BuildBubble(string text, string speechStyleClass)
		{
			RichTextLabel richTextLabel = new RichTextLabel
			{
				MaxWidth = 256f
			};
			richTextLabel.SetMessage(text);
			PanelContainer panelContainer = new PanelContainer();
			panelContainer.StyleClasses.Add("speechBox");
			panelContainer.StyleClasses.Add(speechStyleClass);
			panelContainer.Children.Add(richTextLabel);
			panelContainer.ModulateSelfOverride = new Color?(Color.White.WithAlpha(0.75f));
			return panelContainer;
		}
	}
}
