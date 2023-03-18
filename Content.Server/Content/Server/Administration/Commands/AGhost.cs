using System;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking;
using Content.Server.Ghost.Components;
using Content.Server.Mind;
using Content.Server.Players;
using Content.Shared.Administration;
using Content.Shared.Ghost;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000831 RID: 2097
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class AGhost : IConsoleCommand
	{
		// Token: 0x1700071F RID: 1823
		// (get) Token: 0x06002DEA RID: 11754 RVA: 0x000F03F8 File Offset: 0x000EE5F8
		public string Command
		{
			get
			{
				return "aghost";
			}
		}

		// Token: 0x17000720 RID: 1824
		// (get) Token: 0x06002DEB RID: 11755 RVA: 0x000F03FF File Offset: 0x000EE5FF
		public string Description
		{
			get
			{
				return "Makes you an admin ghost.";
			}
		}

		// Token: 0x17000721 RID: 1825
		// (get) Token: 0x06002DEC RID: 11756 RVA: 0x000F0406 File Offset: 0x000EE606
		public string Help
		{
			get
			{
				return "aghost";
			}
		}

		// Token: 0x06002DED RID: 11757 RVA: 0x000F0410 File Offset: 0x000EE610
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player == null)
			{
				shell.WriteLine("Nah");
				return;
			}
			PlayerData playerData = player.ContentData();
			Mind mind = (playerData != null) ? playerData.Mind : null;
			if (mind == null)
			{
				shell.WriteLine("You can't ghost here!");
				return;
			}
			if (mind.VisitingEntity == null || !this._entities.HasComponent<GhostComponent>(mind.VisitingEntity))
			{
				bool canReturn = mind.CurrentEntity != null && !this._entities.HasComponent<GhostComponent>(mind.CurrentEntity);
				EntityCoordinates coordinates = (player.AttachedEntity != null) ? this._entities.GetComponent<TransformComponent>(player.AttachedEntity.Value).Coordinates : EntitySystem.Get<GameTicker>().GetObserverSpawnPoint();
				EntityUid ghost = this._entities.SpawnEntity("AdminObserver", coordinates);
				this._entities.GetComponent<TransformComponent>(ghost).AttachToGridOrMap();
				if (canReturn)
				{
					if (!string.IsNullOrWhiteSpace(mind.CharacterName))
					{
						this._entities.GetComponent<MetaDataComponent>(ghost).EntityName = mind.CharacterName;
					}
					else
					{
						IPlayerSession session = mind.Session;
						if (!string.IsNullOrWhiteSpace((session != null) ? session.Name : null))
						{
							this._entities.GetComponent<MetaDataComponent>(ghost).EntityName = mind.Session.Name;
						}
					}
					mind.Visit(ghost);
				}
				else
				{
					this._entities.GetComponent<MetaDataComponent>(ghost).EntityName = player.Name;
					mind.TransferTo(new EntityUid?(ghost), false, false);
				}
				GhostComponent comp = this._entities.GetComponent<GhostComponent>(ghost);
				EntitySystem.Get<SharedGhostSystem>().SetCanReturnToBody(comp, canReturn);
				return;
			}
			Mind mind2 = player.ContentData().Mind;
			if (mind2 == null)
			{
				return;
			}
			mind2.UnVisit();
		}

		// Token: 0x04001C4B RID: 7243
		[Dependency]
		private readonly IEntityManager _entities;
	}
}
