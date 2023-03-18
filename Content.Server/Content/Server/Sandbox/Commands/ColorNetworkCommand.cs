using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Managers;
using Content.Server.Atmos.Piping.Components;
using Content.Server.Atmos.Piping.EntitySystems;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.NodeGroups;
using Content.Server.NodeContainer.Nodes;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Server.Sandbox.Commands
{
	// Token: 0x02000217 RID: 535
	[NullableContext(1)]
	[Nullable(0)]
	[AnyCommand]
	public sealed class ColorNetworkCommand : IConsoleCommand
	{
		// Token: 0x17000199 RID: 409
		// (get) Token: 0x06000AA2 RID: 2722 RVA: 0x00037D38 File Offset: 0x00035F38
		public string Command
		{
			get
			{
				return "colornetwork";
			}
		}

		// Token: 0x1700019A RID: 410
		// (get) Token: 0x06000AA3 RID: 2723 RVA: 0x00037D3F File Offset: 0x00035F3F
		public string Description
		{
			get
			{
				return Loc.GetString("color-network-command-description");
			}
		}

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x06000AA4 RID: 2724 RVA: 0x00037D4B File Offset: 0x00035F4B
		public string Help
		{
			get
			{
				return Loc.GetString("color-network-command-help-text", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("command", this.Command)
				});
			}
		}

		// Token: 0x06000AA5 RID: 2725 RVA: 0x00037D74 File Offset: 0x00035F74
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			SandboxSystem sandboxManager = EntitySystem.Get<SandboxSystem>();
			IAdminManager adminManager = IoCManager.Resolve<IAdminManager>();
			if (shell.IsClient && !sandboxManager.IsSandboxEnabled && !adminManager.HasAdminFlag((IPlayerSession)shell.Player, AdminFlags.Mapping))
			{
				shell.WriteError("You are not currently able to use mapping commands.");
			}
			if (args.Length != 3)
			{
				shell.WriteLine(Loc.GetString("shell-wrong-arguments-number"));
				return;
			}
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			int targetId;
			if (!int.TryParse(args[0], out targetId))
			{
				shell.WriteLine(Loc.GetString("shell-argument-must-be-number"));
				return;
			}
			EntityUid eUid;
			eUid..ctor(targetId);
			if (!eUid.IsValid() || !entityManager.EntityExists(eUid))
			{
				shell.WriteLine(Loc.GetString("shell-invalid-entity-id"));
				return;
			}
			NodeContainerComponent nodeContainerComponent;
			if (!entityManager.TryGetComponent<NodeContainerComponent>(eUid, ref nodeContainerComponent))
			{
				shell.WriteLine(Loc.GetString("shell-entity-is-not-node-container"));
				return;
			}
			NodeGroupID nodeGroupId;
			if (!Enum.TryParse<NodeGroupID>(args[1], out nodeGroupId))
			{
				shell.WriteLine(Loc.GetString("shell-node-group-is-invalid"));
				return;
			}
			Color? color = Color.TryFromHex(args[2]);
			if (color == null)
			{
				shell.WriteError(Loc.GetString("shell-invalid-color-hex"));
				return;
			}
			this.PaintNodes(nodeContainerComponent, nodeGroupId, color.Value);
		}

		// Token: 0x06000AA6 RID: 2726 RVA: 0x00037E9C File Offset: 0x0003609C
		private void PaintNodes(NodeContainerComponent nodeContainerComponent, NodeGroupID nodeGroupId, Color color)
		{
			INodeGroup group = nodeContainerComponent.Nodes[nodeGroupId.ToString().ToLower()].NodeGroup;
			if (group == null)
			{
				return;
			}
			foreach (Node x in group.Nodes)
			{
				AtmosPipeColorComponent atmosPipeColorComponent;
				if (IoCManager.Resolve<IEntityManager>().TryGetComponent<AtmosPipeColorComponent>(x.Owner, ref atmosPipeColorComponent))
				{
					EntitySystem.Get<AtmosPipeColorSystem>().SetColor(x.Owner, atmosPipeColorComponent, color);
				}
			}
		}
	}
}
