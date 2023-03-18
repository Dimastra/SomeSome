using System;
using System.Runtime.CompilerServices;
using Content.Shared.White.JoinQueue;
using Robust.Client.Console;
using Robust.Client.GameObjects;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client.White.JoinQueue
{
	// Token: 0x02000025 RID: 37
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class QueueState : State
	{
		// Token: 0x06000090 RID: 144 RVA: 0x00005441 File Offset: 0x00003641
		protected override void Startup()
		{
			this._gui = new QueueGui();
			this._userInterfaceManager.StateRoot.AddChild(this._gui);
			this._gui.QuitPressed += this.OnQuitPressed;
		}

		// Token: 0x06000091 RID: 145 RVA: 0x0000547B File Offset: 0x0000367B
		protected override void Shutdown()
		{
			this._gui.QuitPressed -= this.OnQuitPressed;
			this._gui.Dispose();
			this.Ding();
		}

		// Token: 0x06000092 RID: 146 RVA: 0x000054A8 File Offset: 0x000036A8
		private void Ding()
		{
			AudioSystem audioSystem;
			if (IoCManager.Resolve<IEntityManager>().TrySystem<AudioSystem>(ref audioSystem))
			{
				audioSystem.PlayGlobal("/Audio/Effects/voteding.ogg", Filter.Local(), false, null);
			}
		}

		// Token: 0x06000093 RID: 147 RVA: 0x000054DE File Offset: 0x000036DE
		public void OnQueueUpdate(MsgQueueUpdate msg)
		{
			QueueGui gui = this._gui;
			if (gui == null)
			{
				return;
			}
			gui.UpdateInfo(msg.Total, msg.Position);
		}

		// Token: 0x06000094 RID: 148 RVA: 0x000054FC File Offset: 0x000036FC
		private void OnQuitPressed()
		{
			this._consoleHost.ExecuteCommand("quit");
		}

		// Token: 0x04000050 RID: 80
		[Dependency]
		private readonly IUserInterfaceManager _userInterfaceManager;

		// Token: 0x04000051 RID: 81
		[Dependency]
		private readonly IClientConsoleHost _consoleHost;

		// Token: 0x04000052 RID: 82
		private const string JoinSoundPath = "/Audio/Effects/voteding.ogg";

		// Token: 0x04000053 RID: 83
		[Nullable(2)]
		private QueueGui _gui;
	}
}
