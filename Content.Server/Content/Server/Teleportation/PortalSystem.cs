using System;
using System.Runtime.CompilerServices;
using Content.Server.Mind.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Teleportation.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Content.Server.Teleportation
{
	// Token: 0x0200012A RID: 298
	public sealed class PortalSystem : SharedPortalSystem
	{
		// Token: 0x06000562 RID: 1378 RVA: 0x0001A92C File Offset: 0x00018B2C
		protected override void LogTeleport(EntityUid portal, EntityUid subject, EntityCoordinates source, EntityCoordinates target)
		{
			if (base.HasComp<MindComponent>(subject))
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Teleport;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(26, 4);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(subject), "player", "ToPrettyString(subject)");
				logStringHandler.AppendLiteral(" teleported via ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(portal), "ToPrettyString(portal)");
				logStringHandler.AppendLiteral(" from ");
				logStringHandler.AppendFormatted<EntityCoordinates>(source, "source");
				logStringHandler.AppendLiteral(" to ");
				logStringHandler.AppendFormatted<EntityCoordinates>(target, "target");
				adminLogger.Add(type, impact, ref logStringHandler);
			}
		}

		// Token: 0x04000347 RID: 839
		[Nullable(1)]
		[Dependency]
		private readonly ISharedAdminLogManager _adminLogger;
	}
}
