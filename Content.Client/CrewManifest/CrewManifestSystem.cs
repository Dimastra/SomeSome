using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.CrewManifest;
using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.CrewManifest
{
	// Token: 0x0200036D RID: 877
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CrewManifestSystem : EntitySystem
	{
		// Token: 0x17000458 RID: 1112
		// (get) Token: 0x06001597 RID: 5527 RVA: 0x0007FDA7 File Offset: 0x0007DFA7
		public IReadOnlySet<string> Departments
		{
			get
			{
				return this._departments;
			}
		}

		// Token: 0x06001598 RID: 5528 RVA: 0x0007FDAF File Offset: 0x0007DFAF
		public override void Initialize()
		{
			base.Initialize();
			this.BuildDepartmentLookup();
			this._prototypeManager.PrototypesReloaded += this.OnPrototypesReload;
		}

		// Token: 0x06001599 RID: 5529 RVA: 0x0007FDD4 File Offset: 0x0007DFD4
		public override void Shutdown()
		{
			this._prototypeManager.PrototypesReloaded -= this.OnPrototypesReload;
		}

		// Token: 0x0600159A RID: 5530 RVA: 0x0007FDED File Offset: 0x0007DFED
		public void RequestCrewManifest(EntityUid uid)
		{
			base.RaiseNetworkEvent(new RequestCrewManifestMessage(uid));
		}

		// Token: 0x0600159B RID: 5531 RVA: 0x0007FDFB File Offset: 0x0007DFFB
		private void OnPrototypesReload(PrototypesReloadedEventArgs _)
		{
			this._jobDepartmentLookup.Clear();
			this._departments.Clear();
			this.BuildDepartmentLookup();
		}

		// Token: 0x0600159C RID: 5532 RVA: 0x0007FE1C File Offset: 0x0007E01C
		private void BuildDepartmentLookup()
		{
			foreach (DepartmentPrototype departmentPrototype in this._prototypeManager.EnumeratePrototypes<DepartmentPrototype>())
			{
				this._departments.Add(departmentPrototype.ID);
				for (int i = 1; i <= departmentPrototype.Roles.Count; i++)
				{
					Dictionary<string, int> dictionary;
					if (!this._jobDepartmentLookup.TryGetValue(departmentPrototype.Roles[i - 1], out dictionary))
					{
						dictionary = new Dictionary<string, int>();
						this._jobDepartmentLookup.Add(departmentPrototype.Roles[i - 1], dictionary);
					}
					dictionary.Add(departmentPrototype.ID, i);
				}
			}
		}

		// Token: 0x0600159D RID: 5533 RVA: 0x0007FEDC File Offset: 0x0007E0DC
		public int GetDepartmentOrder(string department, string jobPrototype)
		{
			if (!this.Departments.Contains(department))
			{
				return -1;
			}
			Dictionary<string, int> dictionary;
			if (!this._jobDepartmentLookup.TryGetValue(jobPrototype, out dictionary))
			{
				return -1;
			}
			int result;
			if (!dictionary.TryGetValue(department, out result))
			{
				return -1;
			}
			return result;
		}

		// Token: 0x04000B4A RID: 2890
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000B4B RID: 2891
		private Dictionary<string, Dictionary<string, int>> _jobDepartmentLookup = new Dictionary<string, Dictionary<string, int>>();

		// Token: 0x04000B4C RID: 2892
		private HashSet<string> _departments = new HashSet<string>();
	}
}
