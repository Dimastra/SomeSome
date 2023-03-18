using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.NPC.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;

namespace Content.Server.NPC.Systems
{
	// Token: 0x02000332 RID: 818
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class FactionSystem : EntitySystem
	{
		// Token: 0x060010E4 RID: 4324 RVA: 0x00056ED0 File Offset: 0x000550D0
		public override void Initialize()
		{
			base.Initialize();
			this._sawmill = Logger.GetSawmill("faction");
			base.SubscribeLocalEvent<FactionComponent, ComponentStartup>(new ComponentEventHandler<FactionComponent, ComponentStartup>(this.OnFactionStartup), null, null);
			this._protoManager.PrototypesReloaded += this.OnProtoReload;
			this.RefreshFactions();
		}

		// Token: 0x060010E5 RID: 4325 RVA: 0x00056F24 File Offset: 0x00055124
		public override void Shutdown()
		{
			base.Shutdown();
			this._protoManager.PrototypesReloaded -= this.OnProtoReload;
		}

		// Token: 0x060010E6 RID: 4326 RVA: 0x00056F43 File Offset: 0x00055143
		private void OnProtoReload(PrototypesReloadedEventArgs obj)
		{
			this.RefreshFactions();
		}

		// Token: 0x060010E7 RID: 4327 RVA: 0x00056F4B File Offset: 0x0005514B
		private void OnFactionStartup(EntityUid uid, FactionComponent component, ComponentStartup args)
		{
			this.RefreshFactions(component);
		}

		// Token: 0x060010E8 RID: 4328 RVA: 0x00056F54 File Offset: 0x00055154
		private void RefreshFactions(FactionComponent component)
		{
			foreach (string faction in component.Factions)
			{
				FactionData factionData;
				if (this._factions.TryGetValue(faction, out factionData))
				{
					component.FriendlyFactions.UnionWith(factionData.Friendly);
					component.HostileFactions.UnionWith(factionData.Hostile);
				}
			}
		}

		// Token: 0x060010E9 RID: 4329 RVA: 0x00056FD4 File Offset: 0x000551D4
		public void AddFaction(EntityUid uid, string faction, bool dirty = true)
		{
			if (!this._protoManager.HasIndex<FactionPrototype>(faction))
			{
				this._sawmill.Error("Unable to find faction " + faction);
				return;
			}
			FactionComponent comp = base.EnsureComp<FactionComponent>(uid);
			if (!comp.Factions.Add(faction))
			{
				return;
			}
			if (dirty)
			{
				this.RefreshFactions(comp);
			}
		}

		// Token: 0x060010EA RID: 4330 RVA: 0x00057028 File Offset: 0x00055228
		public void RemoveFaction(EntityUid uid, string faction, bool dirty = true)
		{
			if (!this._protoManager.HasIndex<FactionPrototype>(faction))
			{
				this._sawmill.Error("Unable to find faction " + faction);
				return;
			}
			FactionComponent component;
			if (!base.TryComp<FactionComponent>(uid, ref component))
			{
				return;
			}
			if (!component.Factions.Remove(faction))
			{
				return;
			}
			if (dirty)
			{
				this.RefreshFactions(component);
			}
		}

		// Token: 0x060010EB RID: 4331 RVA: 0x0005707F File Offset: 0x0005527F
		public IEnumerable<EntityUid> GetNearbyHostiles(EntityUid entity, float range, [Nullable(2)] FactionComponent component = null)
		{
			if (!base.Resolve<FactionComponent>(entity, ref component, false))
			{
				return Array.Empty<EntityUid>();
			}
			return this.GetNearbyFactions(entity, range, component.HostileFactions);
		}

		// Token: 0x060010EC RID: 4332 RVA: 0x000570A1 File Offset: 0x000552A1
		public IEnumerable<EntityUid> GetNearbyFriendlies(EntityUid entity, float range, [Nullable(2)] FactionComponent component = null)
		{
			if (!base.Resolve<FactionComponent>(entity, ref component, false))
			{
				return Array.Empty<EntityUid>();
			}
			return this.GetNearbyFactions(entity, range, component.FriendlyFactions);
		}

		// Token: 0x060010ED RID: 4333 RVA: 0x000570C3 File Offset: 0x000552C3
		private IEnumerable<EntityUid> GetNearbyFactions(EntityUid entity, float range, HashSet<string> factions)
		{
			TransformComponent entityXform;
			if (!base.GetEntityQuery<TransformComponent>().TryGetComponent(entity, ref entityXform))
			{
				yield break;
			}
			foreach (FactionComponent comp in this._lookup.GetComponentsInRange<FactionComponent>(entityXform.MapPosition, range))
			{
				if (!(comp.Owner == entity) && factions.Overlaps(comp.Factions))
				{
					yield return comp.Owner;
				}
			}
			HashSet<FactionComponent>.Enumerator enumerator = default(HashSet<FactionComponent>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x060010EE RID: 4334 RVA: 0x000570E8 File Offset: 0x000552E8
		[NullableContext(2)]
		public bool IsFriendly(EntityUid uidA, EntityUid uidB, FactionComponent factionA = null, FactionComponent factionB = null)
		{
			return base.Resolve<FactionComponent>(uidA, ref factionA, false) && base.Resolve<FactionComponent>(uidB, ref factionB, false) && (factionA.Factions.Overlaps(factionB.Factions) || factionA.FriendlyFactions.Overlaps(factionB.Factions));
		}

		// Token: 0x060010EF RID: 4335 RVA: 0x00057138 File Offset: 0x00055338
		public void MakeFriendly(string source, string target)
		{
			FactionData sourceFaction;
			if (!this._factions.TryGetValue(source, out sourceFaction))
			{
				this._sawmill.Error("Unable to find faction " + source);
				return;
			}
			if (!this._factions.ContainsKey(target))
			{
				this._sawmill.Error("Unable to find faction " + target);
				return;
			}
			sourceFaction.Friendly.Add(target);
			sourceFaction.Hostile.Remove(target);
			this.RefreshFactions();
		}

		// Token: 0x060010F0 RID: 4336 RVA: 0x000571B4 File Offset: 0x000553B4
		private void RefreshFactions()
		{
			this._factions.Clear();
			foreach (FactionPrototype faction in this._protoManager.EnumeratePrototypes<FactionPrototype>())
			{
				this._factions[faction.ID] = new FactionData
				{
					Friendly = faction.Friendly.ToHashSet<string>(),
					Hostile = faction.Hostile.ToHashSet<string>()
				};
			}
			foreach (FactionComponent comp in base.EntityQuery<FactionComponent>(true))
			{
				comp.FriendlyFactions.Clear();
				comp.HostileFactions.Clear();
				this.RefreshFactions(comp);
			}
		}

		// Token: 0x060010F1 RID: 4337 RVA: 0x00057298 File Offset: 0x00055498
		public void MakeHostile(string source, string target)
		{
			FactionData sourceFaction;
			if (!this._factions.TryGetValue(source, out sourceFaction))
			{
				this._sawmill.Error("Unable to find faction " + source);
				return;
			}
			if (!this._factions.ContainsKey(target))
			{
				this._sawmill.Error("Unable to find faction " + target);
				return;
			}
			sourceFaction.Friendly.Remove(target);
			sourceFaction.Hostile.Add(target);
			this.RefreshFactions();
		}

		// Token: 0x04000A09 RID: 2569
		[Dependency]
		private readonly IPrototypeManager _protoManager;

		// Token: 0x04000A0A RID: 2570
		[Dependency]
		private readonly EntityLookupSystem _lookup;

		// Token: 0x04000A0B RID: 2571
		private ISawmill _sawmill;

		// Token: 0x04000A0C RID: 2572
		private Dictionary<string, FactionData> _factions = new Dictionary<string, FactionData>();
	}
}
