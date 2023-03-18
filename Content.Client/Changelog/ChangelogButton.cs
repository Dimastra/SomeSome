using System;
using System.Runtime.CompilerServices;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Client.Changelog
{
	// Token: 0x020003ED RID: 1005
	public sealed class ChangelogButton : Button
	{
		// Token: 0x060018BB RID: 6331 RVA: 0x0008E46D File Offset: 0x0008C66D
		public ChangelogButton()
		{
			IoCManager.InjectDependencies<ChangelogButton>(this);
			base.Text = " ";
		}

		// Token: 0x060018BC RID: 6332 RVA: 0x0008E487 File Offset: 0x0008C687
		protected override void EnteredTree()
		{
			base.EnteredTree();
			this._changelogManager.NewChangelogEntriesChanged += this.UpdateStuff;
			this.UpdateStuff();
		}

		// Token: 0x060018BD RID: 6333 RVA: 0x0008E4AC File Offset: 0x0008C6AC
		protected override void ExitedTree()
		{
			base.ExitedTree();
			this._changelogManager.NewChangelogEntriesChanged -= this.UpdateStuff;
		}

		// Token: 0x060018BE RID: 6334 RVA: 0x0008E4CC File Offset: 0x0008C6CC
		private void UpdateStuff()
		{
			if (this._changelogManager.NewChangelogEntries)
			{
				base.Text = Loc.GetString("changelog-button-new-entries");
				base.StyleClasses.Add("Caution");
				return;
			}
			base.Text = Loc.GetString("changelog-button");
			base.StyleClasses.Remove("Caution");
		}

		// Token: 0x04000C9C RID: 3228
		[Nullable(1)]
		[Dependency]
		private readonly ChangelogManager _changelogManager;
	}
}
