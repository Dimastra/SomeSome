using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Server.Decals
{
	// Token: 0x020005B0 RID: 1456
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Mapping)]
	public sealed class EditDecalCommand : IConsoleCommand
	{
		// Token: 0x17000475 RID: 1141
		// (get) Token: 0x06001E2B RID: 7723 RVA: 0x0009F788 File Offset: 0x0009D988
		public string Command
		{
			get
			{
				return "editdecal";
			}
		}

		// Token: 0x17000476 RID: 1142
		// (get) Token: 0x06001E2C RID: 7724 RVA: 0x0009F78F File Offset: 0x0009D98F
		public string Description
		{
			get
			{
				return "Edits a decal.";
			}
		}

		// Token: 0x17000477 RID: 1143
		// (get) Token: 0x06001E2D RID: 7725 RVA: 0x0009F796 File Offset: 0x0009D996
		public string Help
		{
			get
			{
				return this.Command + " <gridId> <uid> <mode>\\n\nPossible modes are:\\n\n- position <x position> <y position>\\n\n- color <color>\\n\n- id <id>\\n\n- rotation <degrees>\\n\n- zindex <zIndex>\\n\n- clean <cleanable>\n";
			}
		}

		// Token: 0x06001E2E RID: 7726 RVA: 0x0009F7A8 File Offset: 0x0009D9A8
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length < 4)
			{
				shell.WriteError("Expected at least 5 arguments.");
				return;
			}
			EntityUid gridId;
			if (!EntityUid.TryParse(args[0], ref gridId))
			{
				shell.WriteError("Failed parsing gridId '" + args[3] + "'.");
				return;
			}
			uint uid;
			if (!uint.TryParse(args[1], NumberStyles.Any, CultureInfo.InvariantCulture, out uid))
			{
				shell.WriteError("Failed parsing uid '" + args[1] + "'.");
				return;
			}
			if (!IoCManager.Resolve<IMapManager>().GridExists(new EntityUid?(gridId)))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 1);
				defaultInterpolatedStringHandler.AppendLiteral("No grid with gridId ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(gridId);
				defaultInterpolatedStringHandler.AppendLiteral(" exists.");
				shell.WriteError(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			DecalSystem decalSystem = EntitySystem.Get<DecalSystem>();
			string a = args[2].ToLower();
			if (!(a == "position"))
			{
				if (!(a == "color"))
				{
					if (!(a == "id"))
					{
						if (!(a == "rotation"))
						{
							if (!(a == "zindex"))
							{
								if (!(a == "clean"))
								{
									shell.WriteError("Invalid mode.");
									return;
								}
								if (args.Length != 4)
								{
									shell.WriteError("Expected 5 arguments.");
									return;
								}
								bool cleanable;
								if (!bool.TryParse(args[3], out cleanable))
								{
									shell.WriteError("Failed parsing cleanable.");
									return;
								}
								if (!decalSystem.SetDecalCleanable(gridId, uid, cleanable, null))
								{
									shell.WriteError("Failed changing decal cleanable flag.");
									return;
								}
							}
							else
							{
								if (args.Length != 4)
								{
									shell.WriteError("Expected 5 arguments.");
									return;
								}
								int zIndex;
								if (!int.TryParse(args[3], NumberStyles.Any, CultureInfo.InvariantCulture, out zIndex))
								{
									shell.WriteError("Failed parsing zIndex.");
									return;
								}
								if (!decalSystem.SetDecalZIndex(gridId, uid, zIndex, null))
								{
									shell.WriteError("Failed changing decal zIndex.");
									return;
								}
							}
						}
						else
						{
							if (args.Length != 4)
							{
								shell.WriteError("Expected 5 arguments.");
								return;
							}
							double degrees;
							if (!double.TryParse(args[3], NumberStyles.Any, CultureInfo.InvariantCulture, out degrees))
							{
								shell.WriteError("Failed parsing degrees.");
								return;
							}
							if (!decalSystem.SetDecalRotation(gridId, uid, Angle.FromDegrees(degrees), null))
							{
								shell.WriteError("Failed changing decal rotation.");
								return;
							}
						}
					}
					else
					{
						if (args.Length != 4)
						{
							shell.WriteError("Expected 5 arguments.");
							return;
						}
						if (!decalSystem.SetDecalId(gridId, uid, args[3], null))
						{
							shell.WriteError("Failed changing decal id.");
							return;
						}
					}
				}
				else
				{
					if (args.Length != 4)
					{
						shell.WriteError("Expected 5 arguments.");
						return;
					}
					Color color;
					if (!Color.TryFromName(args[3], ref color))
					{
						shell.WriteError("Failed parsing color.");
						return;
					}
					if (!decalSystem.SetDecalColor(gridId, uid, new Color?(color), null))
					{
						shell.WriteError("Failed changing decal color.");
						return;
					}
				}
			}
			else
			{
				if (args.Length != 5)
				{
					shell.WriteError("Expected 6 arguments.");
					return;
				}
				float x;
				float y;
				if (!float.TryParse(args[3], NumberStyles.Any, CultureInfo.InvariantCulture, out x) || !float.TryParse(args[4], NumberStyles.Any, CultureInfo.InvariantCulture, out y))
				{
					shell.WriteError("Failed parsing position.");
					return;
				}
				if (!decalSystem.SetDecalPosition(gridId, uid, new EntityCoordinates(gridId, new ValueTuple<float, float>(x, y)), null))
				{
					shell.WriteError("Failed changing decalposition.");
					return;
				}
			}
		}
	}
}
