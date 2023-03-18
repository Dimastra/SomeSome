using System;
using System.Runtime.CompilerServices;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Controls
{
	// Token: 0x020000D7 RID: 215
	public sealed class HLine : Container
	{
		// Token: 0x170000FD RID: 253
		// (get) Token: 0x060005FF RID: 1535 RVA: 0x00020AD4 File Offset: 0x0001ECD4
		// (set) Token: 0x06000600 RID: 1536 RVA: 0x00020B0C File Offset: 0x0001ED0C
		public Color? Color
		{
			get
			{
				StyleBoxFlat styleBoxFlat = this._line.PanelOverride as StyleBoxFlat;
				if (styleBoxFlat != null)
				{
					return new Color?(styleBoxFlat.BackgroundColor);
				}
				return null;
			}
			set
			{
				StyleBoxFlat styleBoxFlat = this._line.PanelOverride as StyleBoxFlat;
				if (styleBoxFlat != null)
				{
					styleBoxFlat.BackgroundColor = value.Value;
				}
			}
		}

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x06000601 RID: 1537 RVA: 0x00020B3C File Offset: 0x0001ED3C
		// (set) Token: 0x06000602 RID: 1538 RVA: 0x00020B70 File Offset: 0x0001ED70
		public float? Thickness
		{
			get
			{
				StyleBoxFlat styleBoxFlat = this._line.PanelOverride as StyleBoxFlat;
				if (styleBoxFlat != null)
				{
					return styleBoxFlat.ContentMarginTopOverride;
				}
				return null;
			}
			set
			{
				StyleBoxFlat styleBoxFlat = this._line.PanelOverride as StyleBoxFlat;
				if (styleBoxFlat != null)
				{
					styleBoxFlat.ContentMarginTopOverride = new float?(value.Value);
				}
			}
		}

		// Token: 0x06000603 RID: 1539 RVA: 0x00020BA4 File Offset: 0x0001EDA4
		public HLine()
		{
			this._line = new PanelContainer();
			this._line.PanelOverride = new StyleBoxFlat();
			this._line.PanelOverride.ContentMarginTopOverride = this.Thickness;
			base.AddChild(this._line);
		}

		// Token: 0x040002AE RID: 686
		[Nullable(1)]
		private readonly PanelContainer _line;
	}
}
