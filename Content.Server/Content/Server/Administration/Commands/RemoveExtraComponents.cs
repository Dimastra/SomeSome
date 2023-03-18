using System;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000859 RID: 2137
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Mapping)]
	public sealed class RemoveExtraComponents : IConsoleCommand
	{
		// Token: 0x17000793 RID: 1939
		// (get) Token: 0x06002EBC RID: 11964 RVA: 0x000F2A6E File Offset: 0x000F0C6E
		public string Command
		{
			get
			{
				return "removeextracomponents";
			}
		}

		// Token: 0x17000794 RID: 1940
		// (get) Token: 0x06002EBD RID: 11965 RVA: 0x000F2A75 File Offset: 0x000F0C75
		public string Description
		{
			get
			{
				return "Removes all components from all entities of the specified id if that component is not in its prototype.\nIf no id is specified, it matches all entities.";
			}
		}

		// Token: 0x17000795 RID: 1941
		// (get) Token: 0x06002EBE RID: 11966 RVA: 0x000F2A7C File Offset: 0x000F0C7C
		public string Help
		{
			get
			{
				return this.Command + " <entityId> / " + this.Command;
			}
		}

		// Token: 0x06002EBF RID: 11967 RVA: 0x000F2A94 File Offset: 0x000F0C94
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			string id = (args.Length == 0) ? null : string.Join(" ", args);
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			IPrototypeManager prototypeManager = IoCManager.Resolve<IPrototypeManager>();
			IComponentFactory fac = IoCManager.Resolve<IComponentFactory>();
			EntityPrototype prototype = null;
			bool checkPrototype = !string.IsNullOrEmpty(id);
			if (checkPrototype && !prototypeManager.TryIndex<EntityPrototype>(id, ref prototype))
			{
				shell.WriteError("Can't find entity prototype with id \"" + id + "\"!");
				return;
			}
			int entities = 0;
			int components = 0;
			foreach (EntityUid entity in entityManager.GetEntities())
			{
				MetaDataComponent metaData = entityManager.GetComponent<MetaDataComponent>(entity);
				if ((!checkPrototype || metaData.EntityPrototype == prototype) && metaData.EntityPrototype != null)
				{
					bool modified = false;
					foreach (IComponent component in entityManager.GetComponents(entity))
					{
						if (!metaData.EntityPrototype.Components.ContainsKey(fac.GetComponentName(component.GetType())))
						{
							entityManager.RemoveComponent(entity, component);
							components++;
							modified = true;
						}
					}
					if (modified)
					{
						entities++;
					}
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 3);
			defaultInterpolatedStringHandler.AppendLiteral("Removed ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(components);
			defaultInterpolatedStringHandler.AppendLiteral(" components from ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(entities);
			defaultInterpolatedStringHandler.AppendLiteral(" entities");
			defaultInterpolatedStringHandler.AppendFormatted((id == null) ? "." : (" with id " + id));
			shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
		}
	}
}
