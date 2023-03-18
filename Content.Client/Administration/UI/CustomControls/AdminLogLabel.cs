using System;
using System.Runtime.CompilerServices;
using Content.Shared.Administration.Logs;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Administration.UI.CustomControls
{
	// Token: 0x020004C5 RID: 1221
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AdminLogLabel : RichTextLabel
	{
		// Token: 0x06001F15 RID: 7957 RVA: 0x000B6440 File Offset: 0x000B4640
		public AdminLogLabel(ref SharedAdminLog log, HSeparator separator)
		{
			this.Log = log;
			this.Separator = separator;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 2);
			defaultInterpolatedStringHandler.AppendFormatted<DateTime>(log.Date, "HH:mm:ss");
			defaultInterpolatedStringHandler.AppendLiteral(": ");
			defaultInterpolatedStringHandler.AppendFormatted(log.Message);
			base.SetMessage(defaultInterpolatedStringHandler.ToStringAndClear());
			base.OnVisibilityChanged += this.VisibilityChanged;
		}

		// Token: 0x170006C6 RID: 1734
		// (get) Token: 0x06001F16 RID: 7958 RVA: 0x000B64B9 File Offset: 0x000B46B9
		public SharedAdminLog Log { get; }

		// Token: 0x170006C7 RID: 1735
		// (get) Token: 0x06001F17 RID: 7959 RVA: 0x000B64C1 File Offset: 0x000B46C1
		public HSeparator Separator { get; }

		// Token: 0x06001F18 RID: 7960 RVA: 0x000B64C9 File Offset: 0x000B46C9
		private void VisibilityChanged(Control control)
		{
			this.Separator.Visible = base.Visible;
		}

		// Token: 0x06001F19 RID: 7961 RVA: 0x000B64DC File Offset: 0x000B46DC
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			base.OnVisibilityChanged -= this.VisibilityChanged;
		}
	}
}
