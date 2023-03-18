using System;
using System.Runtime.CompilerServices;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;

namespace Content.Client.Administration.UI.CustomControls
{
	// Token: 0x020004CD RID: 1229
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class UICommandButton : CommandButton
	{
		// Token: 0x170006D1 RID: 1745
		// (get) Token: 0x06001F4B RID: 8011 RVA: 0x000B6E9A File Offset: 0x000B509A
		// (set) Token: 0x06001F4C RID: 8012 RVA: 0x000B6EA2 File Offset: 0x000B50A2
		public Type WindowType { get; set; }

		// Token: 0x06001F4D RID: 8013 RVA: 0x000B6EAB File Offset: 0x000B50AB
		[NullableContext(1)]
		protected override void Execute(BaseButton.ButtonEventArgs obj)
		{
			if (this.WindowType == null)
			{
				return;
			}
			this._window = (DefaultWindow)IoCManager.Resolve<IDynamicTypeFactory>().CreateInstance(this.WindowType);
			DefaultWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.OpenCentered();
		}

		// Token: 0x04000F0C RID: 3852
		private DefaultWindow _window;
	}
}
