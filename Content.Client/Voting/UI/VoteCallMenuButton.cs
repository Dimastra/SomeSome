using System;
using System.Runtime.CompilerServices;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Client.Voting.UI
{
	// Token: 0x0200004B RID: 75
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class VoteCallMenuButton : Button
	{
		// Token: 0x06000158 RID: 344 RVA: 0x0000B2A0 File Offset: 0x000094A0
		public VoteCallMenuButton()
		{
			IoCManager.InjectDependencies<VoteCallMenuButton>(this);
			base.Text = Loc.GetString("ui-vote-menu-button");
			base.OnPressed += this.OnOnPressed;
		}

		// Token: 0x06000159 RID: 345 RVA: 0x0000B2D1 File Offset: 0x000094D1
		private void OnOnPressed(BaseButton.ButtonEventArgs obj)
		{
			new VoteCallMenu().OpenCentered();
		}

		// Token: 0x0600015A RID: 346 RVA: 0x0000B2DD File Offset: 0x000094DD
		protected override void EnteredTree()
		{
			base.EnteredTree();
			this.UpdateCanCall(this._voteManager.CanCallVote);
			this._voteManager.CanCallVoteChanged += this.UpdateCanCall;
		}

		// Token: 0x0600015B RID: 347 RVA: 0x0000B30D File Offset: 0x0000950D
		protected override void ExitedTree()
		{
			base.ExitedTree();
			this._voteManager.CanCallVoteChanged += this.UpdateCanCall;
		}

		// Token: 0x0600015C RID: 348 RVA: 0x0000B32C File Offset: 0x0000952C
		private void UpdateCanCall(bool canCall)
		{
			base.Disabled = !canCall;
		}

		// Token: 0x040000EC RID: 236
		[Dependency]
		private readonly IVoteManager _voteManager;
	}
}
