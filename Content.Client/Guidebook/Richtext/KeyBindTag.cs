using System;
using System.Runtime.CompilerServices;
using Robust.Client.Input;
using Robust.Client.UserInterface.RichText;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

namespace Content.Client.Guidebook.Richtext
{
	// Token: 0x020002F1 RID: 753
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class KeyBindTag : IMarkupTag
	{
		// Token: 0x170003F3 RID: 1011
		// (get) Token: 0x060012E9 RID: 4841 RVA: 0x00070938 File Offset: 0x0006EB38
		public string Name
		{
			get
			{
				return "keybind";
			}
		}

		// Token: 0x060012EA RID: 4842 RVA: 0x00070940 File Offset: 0x0006EB40
		public string TextBefore(MarkupNode node)
		{
			string text;
			if (!node.Value.TryGetString(ref text))
			{
				return "";
			}
			IKeyBinding keyBinding;
			if (!this._inputManager.TryGetKeyBinding(text, ref keyBinding))
			{
				return text;
			}
			return keyBinding.GetKeyString();
		}

		// Token: 0x0400097C RID: 2428
		[Dependency]
		private readonly IInputManager _inputManager;
	}
}
