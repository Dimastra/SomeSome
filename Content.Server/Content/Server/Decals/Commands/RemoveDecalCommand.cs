using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Content.Server.Decals.Commands
{
	// Token: 0x020005B3 RID: 1459
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Mapping)]
	public sealed class RemoveDecalCommand : IConsoleCommand
	{
		// Token: 0x1700047B RID: 1147
		// (get) Token: 0x06001E4F RID: 7759 RVA: 0x000A0F98 File Offset: 0x0009F198
		public string Command
		{
			get
			{
				return "rmdecal";
			}
		}

		// Token: 0x1700047C RID: 1148
		// (get) Token: 0x06001E50 RID: 7760 RVA: 0x000A0F9F File Offset: 0x0009F19F
		public string Description
		{
			get
			{
				return "removes a decal";
			}
		}

		// Token: 0x1700047D RID: 1149
		// (get) Token: 0x06001E51 RID: 7761 RVA: 0x000A0FA6 File Offset: 0x0009F1A6
		public string Help
		{
			get
			{
				return this.Command + " <uid> <gridId>";
			}
		}

		// Token: 0x06001E52 RID: 7762 RVA: 0x000A0FB8 File Offset: 0x0009F1B8
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 2)
			{
				shell.WriteError("Unexpected number of arguments.\nExpected two: " + this.Help);
				return;
			}
			uint uid;
			if (!uint.TryParse(args[0], NumberStyles.Any, CultureInfo.InvariantCulture, out uid))
			{
				shell.WriteError("Failed parsing uid.");
				return;
			}
			EntityUid rawGridId;
			if (!EntityUid.TryParse(args[1], ref rawGridId) || !IoCManager.Resolve<IMapManager>().GridExists(new EntityUid?(rawGridId)))
			{
				shell.WriteError("Failed parsing gridId.");
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
			if (EntitySystem.Get<DecalSystem>().RemoveDecal(rawGridId, uid, null))
			{
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Successfully removed decal ");
				defaultInterpolatedStringHandler.AppendFormatted<uint>(uid);
				defaultInterpolatedStringHandler.AppendLiteral(".");
				shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Failed trying to remove decal ");
			defaultInterpolatedStringHandler.AppendFormatted<uint>(uid);
			defaultInterpolatedStringHandler.AppendLiteral(".");
			shell.WriteError(defaultInterpolatedStringHandler.ToStringAndClear());
		}
	}
}
