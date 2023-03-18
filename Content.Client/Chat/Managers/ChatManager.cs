using System;
using System.Runtime.CompilerServices;
using Content.Client.Administration.Managers;
using Content.Client.Ghost;
using Content.Shared.Administration;
using Content.Shared.Chat;
using Robust.Client.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Utility;

namespace Content.Client.Chat.Managers
{
	// Token: 0x020003E9 RID: 1001
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class ChatManager : IChatManager
	{
		// Token: 0x0600189A RID: 6298 RVA: 0x0008DDFA File Offset: 0x0008BFFA
		public void Initialize()
		{
			this._sawmill = Logger.GetSawmill("chat");
			this._sawmill.Level = new LogLevel?(2);
		}

		// Token: 0x0600189B RID: 6299 RVA: 0x0008DE20 File Offset: 0x0008C020
		public void SendMessage(string text, ChatSelectChannel channel)
		{
			string text2 = text.ToString();
			if (channel <= ChatSelectChannel.LOOC)
			{
				if (channel <= ChatSelectChannel.Whisper)
				{
					if (channel != ChatSelectChannel.Local)
					{
						if (channel != ChatSelectChannel.Whisper)
						{
							goto IL_19A;
						}
						this._consoleHost.ExecuteCommand("whisper \"" + CommandParsing.Escape(text2) + "\"");
						return;
					}
				}
				else if (channel != ChatSelectChannel.Radio)
				{
					if (channel != ChatSelectChannel.LOOC)
					{
						goto IL_19A;
					}
					this._consoleHost.ExecuteCommand("looc \"" + CommandParsing.Escape(text2) + "\"");
					return;
				}
			}
			else if (channel <= ChatSelectChannel.Emotes)
			{
				if (channel == ChatSelectChannel.OOC)
				{
					this._consoleHost.ExecuteCommand("ooc \"" + CommandParsing.Escape(text2) + "\"");
					return;
				}
				if (channel != ChatSelectChannel.Emotes)
				{
					goto IL_19A;
				}
				this._consoleHost.ExecuteCommand("me \"" + CommandParsing.Escape(text2) + "\"");
				return;
			}
			else if (channel != ChatSelectChannel.Dead)
			{
				if (channel == ChatSelectChannel.Admin)
				{
					this._consoleHost.ExecuteCommand("asay \"" + CommandParsing.Escape(text2) + "\"");
					return;
				}
				if (channel == ChatSelectChannel.Console)
				{
					this._consoleHost.ExecuteCommand(text);
					return;
				}
				goto IL_19A;
			}
			else
			{
				GhostSystem entitySystemOrNull = this._systems.GetEntitySystemOrNull<GhostSystem>();
				if (entitySystemOrNull == null || !entitySystemOrNull.IsGhost)
				{
					if (this._adminMgr.HasFlag(AdminFlags.Admin))
					{
						this._consoleHost.ExecuteCommand("dsay \"" + CommandParsing.Escape(text2) + "\"");
						return;
					}
					this._sawmill.Warning("Tried to speak on deadchat without being ghost or admin.");
					return;
				}
			}
			this._consoleHost.ExecuteCommand("say \"" + CommandParsing.Escape(text2) + "\"");
			return;
			IL_19A:
			throw new ArgumentOutOfRangeException("channel", channel, null);
		}

		// Token: 0x04000C90 RID: 3216
		[Dependency]
		private readonly IClientConsoleHost _consoleHost;

		// Token: 0x04000C91 RID: 3217
		[Dependency]
		private readonly IClientAdminManager _adminMgr;

		// Token: 0x04000C92 RID: 3218
		[Dependency]
		private readonly IEntitySystemManager _systems;

		// Token: 0x04000C93 RID: 3219
		private ISawmill _sawmill;
	}
}
