using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000839 RID: 2105
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Spawn)]
	internal sealed class DeleteEntitiesWithComponent : IConsoleCommand
	{
		// Token: 0x17000735 RID: 1845
		// (get) Token: 0x06002E13 RID: 11795 RVA: 0x000F0C64 File Offset: 0x000EEE64
		public string Command
		{
			get
			{
				return "deleteewc";
			}
		}

		// Token: 0x17000736 RID: 1846
		// (get) Token: 0x06002E14 RID: 11796 RVA: 0x000F0C6B File Offset: 0x000EEE6B
		public string Description
		{
			get
			{
				return Loc.GetString("delete-entities-with-component-command-description");
			}
		}

		// Token: 0x17000737 RID: 1847
		// (get) Token: 0x06002E15 RID: 11797 RVA: 0x000F0C77 File Offset: 0x000EEE77
		public string Help
		{
			get
			{
				return Loc.GetString("delete-entities-with-component-command-help-text");
			}
		}

		// Token: 0x06002E16 RID: 11798 RVA: 0x000F0C84 File Offset: 0x000EEE84
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length < 1)
			{
				shell.WriteLine(this.Help);
				return;
			}
			IComponentFactory factory = IoCManager.Resolve<IComponentFactory>();
			List<Type> components = new List<Type>();
			foreach (string arg in args)
			{
				components.Add(factory.GetRegistration(arg, false).Type);
			}
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			IEnumerable<IEnumerable<EntityUid>> entitiesWithComponents = from c in components
			select 
				from x in entityManager.GetAllComponents(c, false)
				select x.Owner;
			HashSet<EntityUid> hashSet = entitiesWithComponents.Skip(1).Aggregate(new HashSet<EntityUid>(entitiesWithComponents.First<IEnumerable<EntityUid>>()), delegate(HashSet<EntityUid> h, IEnumerable<EntityUid> e)
			{
				h.IntersectWith(e);
				return h;
			});
			int count = 0;
			foreach (EntityUid entity in hashSet)
			{
				entityManager.DeleteEntity(entity);
				count++;
			}
			shell.WriteLine(Loc.GetString("delete-entities-with-component-command-deleted-components", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("count", count)
			}));
		}
	}
}
