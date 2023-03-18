using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Shared.Administration.Logs;
using Content.Shared.Construction;
using Content.Shared.Database;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Construction.Completions
{
	// Token: 0x0200060E RID: 1550
	public sealed class AdminLog : IGraphAction
	{
		// Token: 0x06002134 RID: 8500 RVA: 0x000ADABC File Offset: 0x000ABCBC
		[NullableContext(1)]
		public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
		{
			IAdminLogManager logManager = IoCManager.Resolve<IAdminLogManager>();
			LogStringHandler logStringHandler;
			if (userUid != null)
			{
				ISharedAdminLogManager sharedAdminLogManager = logManager;
				LogType logType = this.LogType;
				LogImpact impact = this.Impact;
				logStringHandler = new LogStringHandler(19, 3);
				logStringHandler.AppendFormatted(this.Message);
				logStringHandler.AppendLiteral(" - Entity: ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(entityManager.ToPrettyString(uid), "entity", "entityManager.ToPrettyString(uid)");
				logStringHandler.AppendLiteral(", User: ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(entityManager.ToPrettyString(userUid.Value), "user", "entityManager.ToPrettyString(userUid.Value)");
				sharedAdminLogManager.Add(logType, impact, ref logStringHandler);
				return;
			}
			ISharedAdminLogManager sharedAdminLogManager2 = logManager;
			LogType logType2 = this.LogType;
			LogImpact impact2 = this.Impact;
			logStringHandler = new LogStringHandler(11, 2);
			logStringHandler.AppendFormatted(this.Message);
			logStringHandler.AppendLiteral(" - Entity: ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(entityManager.ToPrettyString(uid), "entity", "entityManager.ToPrettyString(uid)");
			sharedAdminLogManager2.Add(logType2, impact2, ref logStringHandler);
		}

		// Token: 0x04001472 RID: 5234
		[DataField("logType", false, 1, false, false, null)]
		public LogType LogType = LogType.Construction;

		// Token: 0x04001473 RID: 5235
		[DataField("impact", false, 1, false, false, null)]
		public LogImpact Impact;

		// Token: 0x04001474 RID: 5236
		[Nullable(1)]
		[DataField("message", false, 1, true, false, null)]
		public string Message = string.Empty;
	}
}
