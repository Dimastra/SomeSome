using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.AME.Components;
using Content.Server.Chat.Managers;
using Content.Server.Explosion.Components;
using Content.Server.Explosion.EntitySystems;
using Content.Server.NodeContainer.NodeGroups;
using Content.Server.NodeContainer.Nodes;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Random;
using Robust.Shared.ViewVariables;

namespace Content.Server.AME
{
	// Token: 0x020007D4 RID: 2004
	[NullableContext(1)]
	[Nullable(0)]
	[NodeGroup(new NodeGroupID[]
	{
		NodeGroupID.AMEngine
	})]
	public sealed class AMENodeGroup : BaseNodeGroup
	{
		// Token: 0x170006B6 RID: 1718
		// (get) Token: 0x06002B84 RID: 11140 RVA: 0x000E45A9 File Offset: 0x000E27A9
		[Nullable(2)]
		public AMEControllerComponent MasterController
		{
			[NullableContext(2)]
			get
			{
				return this._masterController;
			}
		}

		// Token: 0x170006B7 RID: 1719
		// (get) Token: 0x06002B85 RID: 11141 RVA: 0x000E45B1 File Offset: 0x000E27B1
		public int CoreCount
		{
			get
			{
				return this._cores.Count;
			}
		}

		// Token: 0x06002B86 RID: 11142 RVA: 0x000E45C0 File Offset: 0x000E27C0
		public override void LoadNodes(List<Node> groupNodes)
		{
			base.LoadNodes(groupNodes);
			IMapManager mapManager = IoCManager.Resolve<IMapManager>();
			MapGridComponent grid = null;
			foreach (Node node in groupNodes)
			{
				EntityUid nodeOwner = node.Owner;
				AMEShieldComponent shield;
				if (this._entMan.TryGetComponent<AMEShieldComponent>(nodeOwner, ref shield))
				{
					TransformComponent xform = this._entMan.GetComponent<TransformComponent>(nodeOwner);
					EntityUid? gridUid = xform.GridUid;
					if ((!(gridUid != ((grid != null) ? new EntityUid?(grid.Owner) : null)) || mapManager.TryGetGrid(xform.GridUid, ref grid)) && grid != null)
					{
						if ((from entity in grid.GetCellsInSquareArea(xform.Coordinates, 1)
						where entity != nodeOwner && this._entMan.HasComponent<AMEShieldComponent>(entity)
						select entity).Count<EntityUid>() >= 8)
						{
							this._cores.Add(shield);
							shield.SetCore();
						}
						else
						{
							shield.UnsetCore();
						}
					}
				}
			}
			foreach (Node node2 in groupNodes)
			{
				EntityUid nodeOwner2 = node2.Owner;
				AMEControllerComponent controller;
				if (this._entMan.TryGetComponent<AMEControllerComponent>(nodeOwner2, ref controller))
				{
					if (this._masterController == null)
					{
						this._masterController = controller;
					}
					controller.OnAMENodeGroupUpdate();
				}
			}
			this.UpdateCoreVisuals();
		}

		// Token: 0x06002B87 RID: 11143 RVA: 0x000E479C File Offset: 0x000E299C
		public void UpdateCoreVisuals()
		{
			int injectionAmount = 0;
			bool injecting = false;
			if (this._masterController != null)
			{
				injectionAmount = this._masterController.InjectionAmount;
				injecting = this._masterController.Injecting;
			}
			int injectionStrength = (this.CoreCount > 0) ? (injectionAmount / this.CoreCount) : 0;
			foreach (AMEShieldComponent ameshieldComponent in this._cores)
			{
				ameshieldComponent.UpdateCoreVisuals(injectionStrength, injecting);
			}
		}

		// Token: 0x06002B88 RID: 11144 RVA: 0x000E4828 File Offset: 0x000E2A28
		public float InjectFuel(int fuel, out bool overloading)
		{
			overloading = false;
			if (fuel > 0 && this.CoreCount > 0)
			{
				int safeFuelLimit = this.CoreCount * 2;
				if (fuel > safeFuelLimit)
				{
					int instability = 0;
					int num = fuel - this.CoreCount;
					if (RandomExtensions.Prob(this._random, 0.5f))
					{
						instability = 1;
					}
					if (num > 5)
					{
						instability = 5;
					}
					if (num > 10)
					{
						instability = 20;
					}
					if (instability != 0)
					{
						overloading = true;
						foreach (AMEShieldComponent ameshieldComponent in this._cores)
						{
							ameshieldComponent.CoreIntegrity -= instability;
						}
					}
				}
				return (float)fuel / (float)this.CoreCount * (float)fuel * 20000f;
			}
			return 0f;
		}

		// Token: 0x06002B89 RID: 11145 RVA: 0x000E48F0 File Offset: 0x000E2AF0
		public int GetTotalStability()
		{
			if (this.CoreCount < 1)
			{
				return 100;
			}
			int stability = 0;
			foreach (AMEShieldComponent core in this._cores)
			{
				stability += core.CoreIntegrity;
			}
			stability /= this.CoreCount;
			return stability;
		}

		// Token: 0x06002B8A RID: 11146 RVA: 0x000E4960 File Offset: 0x000E2B60
		public void ExplodeCores()
		{
			if (this._cores.Count < 1 || this.MasterController == null)
			{
				return;
			}
			float radius = 0f;
			foreach (AMEShieldComponent ameshieldComponent in this._cores)
			{
				radius += (float)this.MasterController.InjectionAmount;
			}
			radius *= 2f;
			radius = Math.Min(radius, 8f);
			AMEControllerComponent masterController = this._masterController;
			EntityUid? lastPlayer = (masterController != null) ? masterController._lastPlayerIncreasedFuel : null;
			this._chatManager.SendAdminAnnouncement(Loc.GetString("admin-chatalert-AME-exploded", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("lastplayer", this._entityManager.ToPrettyString(lastPlayer.GetValueOrDefault()))
			}));
			ExplosionSystem explosionSystem = EntitySystem.Get<ExplosionSystem>();
			EntityUid owner = this.MasterController.Owner;
			ExplosiveComponent explosive = null;
			bool delete = false;
			float? radius2 = new float?(radius);
			explosionSystem.TriggerExplosive(owner, explosive, delete, null, radius2, null);
		}

		// Token: 0x04001AFB RID: 6907
		[Nullable(2)]
		[ViewVariables]
		private AMEControllerComponent _masterController;

		// Token: 0x04001AFC RID: 6908
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04001AFD RID: 6909
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x04001AFE RID: 6910
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x04001AFF RID: 6911
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04001B00 RID: 6912
		private readonly List<AMEShieldComponent> _cores = new List<AMEShieldComponent>();
	}
}
