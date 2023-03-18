using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Client.UserInterface.Controls;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;

namespace Content.Client.Viewport
{
	// Token: 0x0200005C RID: 92
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ViewportManager
	{
		// Token: 0x060001AE RID: 430 RVA: 0x0000C524 File Offset: 0x0000A724
		public void Initialize()
		{
			this._cfg.OnValueChanged<bool>(CCVars.ViewportStretch, delegate(bool _)
			{
				this.UpdateCfg();
			}, false);
			this._cfg.OnValueChanged<int>(CCVars.ViewportSnapToleranceClip, delegate(int _)
			{
				this.UpdateCfg();
			}, false);
			this._cfg.OnValueChanged<int>(CCVars.ViewportSnapToleranceMargin, delegate(int _)
			{
				this.UpdateCfg();
			}, false);
			this._cfg.OnValueChanged<bool>(CCVars.ViewportScaleRender, delegate(bool _)
			{
				this.UpdateCfg();
			}, false);
			this._cfg.OnValueChanged<int>(CCVars.ViewportFixedScaleFactor, delegate(int _)
			{
				this.UpdateCfg();
			}, false);
		}

		// Token: 0x060001AF RID: 431 RVA: 0x0000C5C2 File Offset: 0x0000A7C2
		private void UpdateCfg()
		{
			this._viewports.ForEach(delegate(MainViewport v)
			{
				v.UpdateCfg();
			});
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x0000C5EE File Offset: 0x0000A7EE
		public void AddViewport(MainViewport vp)
		{
			this._viewports.Add(vp);
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x0000C5FC File Offset: 0x0000A7FC
		public void RemoveViewport(MainViewport vp)
		{
			this._viewports.Remove(vp);
		}

		// Token: 0x04000117 RID: 279
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04000118 RID: 280
		private readonly List<MainViewport> _viewports = new List<MainViewport>();
	}
}
