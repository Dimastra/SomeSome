using System;
using System.Runtime.CompilerServices;
using Content.Client.Audio;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.IoC;

namespace Content.Client.UserInterface.Controls
{
	// Token: 0x020000D3 RID: 211
	[NullableContext(1)]
	[Nullable(0)]
	[Virtual]
	public class CommonButton : Button
	{
		// Token: 0x060005E9 RID: 1513 RVA: 0x000204A3 File Offset: 0x0001E6A3
		public CommonButton()
		{
			IoCManager.InjectDependencies<CommonButton>(this);
			base.OnMouseEntered += this.PlayHoverSound;
			base.OnPressed += this.PlayPressedSound;
		}

		// Token: 0x060005EA RID: 1514 RVA: 0x000204D6 File Offset: 0x0001E6D6
		private void PlayHoverSound(GUIMouseHoverEventArgs _)
		{
			this._audio.Play("/Audio/UI/hover.ogg", new AudioParams?(AudioParams.Default.WithVolume(-10f)));
		}

		// Token: 0x060005EB RID: 1515 RVA: 0x000204FD File Offset: 0x0001E6FD
		private void PlayPressedSound(BaseButton.ButtonEventArgs _)
		{
			this._audio.Play("/Audio/UI/pressed.ogg", new AudioParams?(AudioParams.Default.WithVolume(-5f)));
		}

		// Token: 0x040002A6 RID: 678
		[Dependency]
		private readonly UIAudioManager _audio;
	}
}
