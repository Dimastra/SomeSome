using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;

namespace Content.Server.Commands
{
	// Token: 0x02000631 RID: 1585
	[NullableContext(1)]
	[Nullable(0)]
	public static class CommandUtils
	{
		// Token: 0x060021CA RID: 8650 RVA: 0x000B0434 File Offset: 0x000AE634
		public static bool TryGetSessionByUsernameOrId(IConsoleShell shell, string usernameOrId, IPlayerSession performer, [Nullable(2)] [NotNullWhen(true)] out IPlayerSession session)
		{
			IPlayerManager plyMgr = IoCManager.Resolve<IPlayerManager>();
			if (plyMgr.TryGetSessionByUsername(usernameOrId, ref session))
			{
				return true;
			}
			Guid targetGuid;
			if (!Guid.TryParse(usernameOrId, out targetGuid))
			{
				shell.WriteLine("Unable to find user with that name/id.");
				return false;
			}
			if (plyMgr.TryGetSessionById(new NetUserId(targetGuid), ref session))
			{
				return true;
			}
			shell.WriteLine("Unable to find user with that name/id.");
			return false;
		}

		// Token: 0x060021CB RID: 8651 RVA: 0x000B0488 File Offset: 0x000AE688
		public static bool TryGetAttachedEntityByUsernameOrId(IConsoleShell shell, string usernameOrId, IPlayerSession performer, out EntityUid attachedEntity)
		{
			attachedEntity = default(EntityUid);
			IPlayerSession session;
			if (!CommandUtils.TryGetSessionByUsernameOrId(shell, usernameOrId, performer, out session))
			{
				return false;
			}
			if (session.AttachedEntity == null)
			{
				shell.WriteLine("User has no attached entity.");
				return false;
			}
			attachedEntity = session.AttachedEntity.Value;
			return true;
		}

		// Token: 0x060021CC RID: 8652 RVA: 0x000B04DC File Offset: 0x000AE6DC
		public static string SubstituteEntityDetails(IConsoleShell shell, EntityUid ent, string ruleString)
		{
			IEntityManager entMan = IoCManager.Resolve<IEntityManager>();
			TransformComponent transform = entMan.GetComponent<TransformComponent>(ent);
			ruleString = ruleString.Replace("$ID", ent.ToString());
			string text = ruleString;
			string oldValue = "$WX";
			Vector2 vector = transform.WorldPosition;
			ruleString = text.Replace(oldValue, vector.X.ToString(CultureInfo.InvariantCulture));
			string text2 = ruleString;
			string oldValue2 = "$WY";
			vector = transform.WorldPosition;
			ruleString = text2.Replace(oldValue2, vector.Y.ToString(CultureInfo.InvariantCulture));
			string text3 = ruleString;
			string oldValue3 = "$LX";
			vector = transform.LocalPosition;
			ruleString = text3.Replace(oldValue3, vector.X.ToString(CultureInfo.InvariantCulture));
			string text4 = ruleString;
			string oldValue4 = "$LY";
			vector = transform.LocalPosition;
			ruleString = text4.Replace(oldValue4, vector.Y.ToString(CultureInfo.InvariantCulture));
			ruleString = ruleString.Replace("$NAME", entMan.GetComponent<MetaDataComponent>(ent).EntityName);
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player != null)
			{
				EntityUid? attachedEntity = player.AttachedEntity;
				if (attachedEntity != null)
				{
					EntityUid p = attachedEntity.GetValueOrDefault();
					if (p.Valid)
					{
						TransformComponent pTransform = entMan.GetComponent<TransformComponent>(p);
						ruleString = ruleString.Replace("$PID", ent.ToString());
						string text5 = ruleString;
						string oldValue5 = "$PWX";
						vector = pTransform.WorldPosition;
						ruleString = text5.Replace(oldValue5, vector.X.ToString(CultureInfo.InvariantCulture));
						string text6 = ruleString;
						string oldValue6 = "$PWY";
						vector = pTransform.WorldPosition;
						ruleString = text6.Replace(oldValue6, vector.Y.ToString(CultureInfo.InvariantCulture));
						string text7 = ruleString;
						string oldValue7 = "$PLX";
						vector = pTransform.LocalPosition;
						ruleString = text7.Replace(oldValue7, vector.X.ToString(CultureInfo.InvariantCulture));
						string text8 = ruleString;
						string oldValue8 = "$PLY";
						vector = pTransform.LocalPosition;
						ruleString = text8.Replace(oldValue8, vector.Y.ToString(CultureInfo.InvariantCulture));
					}
				}
			}
			return ruleString;
		}
	}
}
