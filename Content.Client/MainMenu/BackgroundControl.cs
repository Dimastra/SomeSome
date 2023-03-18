using System;
using System.Runtime.CompilerServices;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.MainMenu
{
	// Token: 0x0200024D RID: 589
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BackgroundControl : TextureRect
	{
		// Token: 0x06000ED9 RID: 3801 RVA: 0x00059896 File Offset: 0x00057A96
		public BackgroundControl()
		{
			IoCManager.InjectDependencies<BackgroundControl>(this);
		}

		// Token: 0x06000EDA RID: 3802 RVA: 0x000598A5 File Offset: 0x00057AA5
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			IRenderTexture buffer = this._buffer;
			if (buffer == null)
			{
				return;
			}
			buffer.Dispose();
		}

		// Token: 0x06000EDB RID: 3803 RVA: 0x000598C0 File Offset: 0x00057AC0
		protected override void Resized()
		{
			base.Resized();
			IRenderTexture buffer = this._buffer;
			if (buffer != null)
			{
				buffer.Dispose();
			}
			this._buffer = this._clyde.CreateRenderTarget(base.PixelSize, 1, null, null);
		}

		// Token: 0x06000EDC RID: 3804 RVA: 0x0005990C File Offset: 0x00057B0C
		protected override void Draw(DrawingHandleScreen handle)
		{
			if (this._buffer == null)
			{
				return;
			}
			handle.RenderInRenderTarget(this._buffer, delegate()
			{
				this.<>n__0(handle);
			}, new Color?(Color.Transparent));
			handle.DrawTextureRect(this._buffer.Texture, base.PixelSizeBox, null);
		}

		// Token: 0x04000765 RID: 1893
		[Dependency]
		private readonly IClyde _clyde;

		// Token: 0x04000766 RID: 1894
		[Dependency]
		private readonly IPrototypeManager _prototype;

		// Token: 0x04000767 RID: 1895
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04000768 RID: 1896
		[Nullable(2)]
		private IRenderTexture _buffer;
	}
}
