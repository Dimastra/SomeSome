using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000841 RID: 2113
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Mapping)]
	public sealed class FindEntitiesWithComponents : IConsoleCommand
	{
		// Token: 0x1700074D RID: 1869
		// (get) Token: 0x06002E3D RID: 11837 RVA: 0x000F15A3 File Offset: 0x000EF7A3
		public string Command
		{
			get
			{
				return "findentitieswithcomponents";
			}
		}

		// Token: 0x1700074E RID: 1870
		// (get) Token: 0x06002E3E RID: 11838 RVA: 0x000F15AA File Offset: 0x000EF7AA
		public string Description
		{
			get
			{
				return "Finds entities with all of the specified components.";
			}
		}

		// Token: 0x1700074F RID: 1871
		// (get) Token: 0x06002E3F RID: 11839 RVA: 0x000F15B1 File Offset: 0x000EF7B1
		public string Help
		{
			get
			{
				return this.Command + " <componentName1> <componentName2>...";
			}
		}

		// Token: 0x06002E40 RID: 11840 RVA: 0x000F15C4 File Offset: 0x000EF7C4
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
			if (args.Length == 0)
			{
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Invalid amount of arguments: ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(args.Length);
				defaultInterpolatedStringHandler.AppendLiteral(".\n");
				defaultInterpolatedStringHandler.AppendFormatted(this.Help);
				shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			List<Type> components = new List<Type>();
			IComponentFactory componentFactory = IoCManager.Resolve<IComponentFactory>();
			List<string> invalidArgs = new List<string>();
			foreach (string arg in args)
			{
				ComponentRegistration registration;
				if (!componentFactory.TryGetRegistration(arg, ref registration, false))
				{
					invalidArgs.Add(arg);
				}
				else
				{
					components.Add(registration.Type);
				}
			}
			if (invalidArgs.Count > 0)
			{
				shell.WriteLine("No component found for component names: " + string.Join(", ", invalidArgs));
				return;
			}
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			HashSet<string> entityIds = new HashSet<string>();
			IEnumerable<EntityUid>[] entitiesWithComponents = (from c in components
			select 
				from x in entityManager.GetAllComponents(c, false)
				select x.Owner).ToArray<IEnumerable<EntityUid>>();
			foreach (EntityUid entity in entitiesWithComponents.Skip(1).Aggregate(new HashSet<EntityUid>(entitiesWithComponents.First<IEnumerable<EntityUid>>()), delegate(HashSet<EntityUid> h, IEnumerable<EntityUid> e)
			{
				h.IntersectWith(e);
				return h;
			}))
			{
				EntityPrototype prototypeId = entityManager.GetComponent<MetaDataComponent>(entity).EntityPrototype;
				if (prototypeId != null)
				{
					entityIds.Add(prototypeId.ID);
				}
			}
			if (entityIds.Count == 0)
			{
				shell.WriteLine("No entities found with components " + string.Join(", ", args) + ".");
				return;
			}
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(17, 2);
			defaultInterpolatedStringHandler.AppendFormatted<int>(entityIds.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" entities found:\n");
			defaultInterpolatedStringHandler.AppendFormatted(string.Join("\n", entityIds));
			shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
		}
	}
}
