using System;
using System.Runtime.CompilerServices;
using Content.Client.Viewport;
using Content.Shared.Input;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.State;
using Robust.Shared.ContentPack;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Players;
using Robust.Shared.Utility;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Content.Client.Screenshot
{
	// Token: 0x0200015D RID: 349
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class ScreenshotHook : IScreenshotHook
	{
		// Token: 0x06000928 RID: 2344 RVA: 0x00035CD4 File Offset: 0x00033ED4
		public void Initialize()
		{
			this._inputManager.SetInputCommand(ContentKeyFunctions.TakeScreenshot, InputCmdHandler.FromDelegate(delegate(ICommonSession _)
			{
				this._clyde.Screenshot(0, new CopyPixelsDelegate<Rgb24>(this.Take<Rgb24>), null);
			}, null, true, true));
			this._inputManager.SetInputCommand(ContentKeyFunctions.TakeScreenshotNoUI, InputCmdHandler.FromDelegate(delegate(ICommonSession _)
			{
				IMainViewportState mainViewportState = this._stateManager.CurrentState as IMainViewportState;
				if (mainViewportState != null)
				{
					mainViewportState.Viewport.Viewport.Screenshot(new CopyPixelsDelegate<Rgba32>(this.Take<Rgba32>));
					return;
				}
				Logger.InfoS("screenshot", "Can't take no-UI screenshot: current state is not GameScreen");
			}, null, true, true));
		}

		// Token: 0x06000929 RID: 2345 RVA: 0x00035D2C File Offset: 0x00033F2C
		[NullableContext(0)]
		private void Take<[IsUnmanaged] T>([Nullable(new byte[]
		{
			1,
			0
		})] Image<T> screenshot) where T : struct, ValueType, IPixel<T>
		{
			ScreenshotHook.<Take>d__6<T> <Take>d__;
			<Take>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Take>d__.<>4__this = this;
			<Take>d__.screenshot = screenshot;
			<Take>d__.<>1__state = -1;
			<Take>d__.<>t__builder.Start<ScreenshotHook.<Take>d__6<T>>(ref <Take>d__);
		}

		// Token: 0x04000492 RID: 1170
		private static readonly ResourcePath BaseScreenshotPath = new ResourcePath("/Screenshots", "/");

		// Token: 0x04000493 RID: 1171
		[Dependency]
		private readonly IInputManager _inputManager;

		// Token: 0x04000494 RID: 1172
		[Dependency]
		private readonly IClyde _clyde;

		// Token: 0x04000495 RID: 1173
		[Dependency]
		private readonly IResourceManager _resourceManager;

		// Token: 0x04000496 RID: 1174
		[Dependency]
		private readonly IStateManager _stateManager;
	}
}
