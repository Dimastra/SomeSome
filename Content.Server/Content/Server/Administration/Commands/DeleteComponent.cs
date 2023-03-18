using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000838 RID: 2104
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Spawn)]
	public sealed class DeleteComponent : IConsoleCommand
	{
		// Token: 0x17000732 RID: 1842
		// (get) Token: 0x06002E0E RID: 11790 RVA: 0x000F0B23 File Offset: 0x000EED23
		public string Command
		{
			get
			{
				return "deletecomponent";
			}
		}

		// Token: 0x17000733 RID: 1843
		// (get) Token: 0x06002E0F RID: 11791 RVA: 0x000F0B2A File Offset: 0x000EED2A
		public string Description
		{
			get
			{
				return "Deletes all instances of the specified component.";
			}
		}

		// Token: 0x17000734 RID: 1844
		// (get) Token: 0x06002E10 RID: 11792 RVA: 0x000F0B31 File Offset: 0x000EED31
		public string Help
		{
			get
			{
				return "Usage: " + this.Command + " <name>";
			}
		}

		// Token: 0x06002E11 RID: 11793 RVA: 0x000F0B48 File Offset: 0x000EED48
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length == 0)
			{
				shell.WriteLine("Not enough arguments.\n" + this.Help);
				return;
			}
			string name = string.Join(" ", args);
			IComponentFactory componentFactory = IoCManager.Resolve<IComponentFactory>();
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			ComponentRegistration registration;
			if (!componentFactory.TryGetRegistration(name, ref registration, false))
			{
				shell.WriteLine("No component exists with name " + name + ".");
				return;
			}
			Type componentType = registration.Type;
			IEnumerable<IComponent> allComponents = entityManager.GetAllComponents(componentType, true);
			int i = 0;
			foreach (IComponent component in allComponents)
			{
				EntityUid uid = component.Owner;
				entityManager.RemoveComponent(uid, component);
				i++;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 2);
			defaultInterpolatedStringHandler.AppendLiteral("Removed ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(i);
			defaultInterpolatedStringHandler.AppendLiteral(" components with name ");
			defaultInterpolatedStringHandler.AppendFormatted(name);
			defaultInterpolatedStringHandler.AppendLiteral(".");
			shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
		}
	}
}
