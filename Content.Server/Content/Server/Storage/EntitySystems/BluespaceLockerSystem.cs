using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.DoAfter;
using Content.Server.Explosion.EntitySystems;
using Content.Server.Mind.Components;
using Content.Server.Resist;
using Content.Server.Station.Components;
using Content.Server.Storage.Components;
using Content.Server.Tools.Systems;
using Content.Shared.Access.Components;
using Content.Shared.Coordinates;
using Content.Shared.DoAfter;
using Content.Shared.Lock;
using Content.Shared.Storage.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server.Storage.EntitySystems
{
	// Token: 0x0200015D RID: 349
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BluespaceLockerSystem : EntitySystem
	{
		// Token: 0x060006A6 RID: 1702 RVA: 0x0002011C File Offset: 0x0001E31C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<BluespaceLockerComponent, ComponentStartup>(new ComponentEventHandler<BluespaceLockerComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<BluespaceLockerComponent, StorageBeforeOpenEvent>(new ComponentEventRefHandler<BluespaceLockerComponent, StorageBeforeOpenEvent>(this.PreOpen), null, null);
			base.SubscribeLocalEvent<BluespaceLockerComponent, StorageAfterCloseEvent>(new ComponentEventRefHandler<BluespaceLockerComponent, StorageAfterCloseEvent>(this.PostClose), null, null);
			base.SubscribeLocalEvent<BluespaceLockerComponent, DoAfterEvent>(new ComponentEventHandler<BluespaceLockerComponent, DoAfterEvent>(this.OnDoAfter), null, null);
		}

		// Token: 0x060006A7 RID: 1703 RVA: 0x0002017F File Offset: 0x0001E37F
		private void OnStartup(EntityUid uid, BluespaceLockerComponent component, ComponentStartup args)
		{
			this.GetTarget(uid, component, true);
			if (component.BehaviorProperties.BluespaceEffectOnInit)
			{
				this.BluespaceEffect(uid, component, component, true);
			}
		}

		// Token: 0x060006A8 RID: 1704 RVA: 0x000201A4 File Offset: 0x0001E3A4
		public void BluespaceEffect(EntityUid effectTargetUid, BluespaceLockerComponent effectSourceComponent, [Nullable(2)] BluespaceLockerComponent effectTargetComponent, bool bypassLimit = false)
		{
			if (!bypassLimit && base.Resolve<BluespaceLockerComponent>(effectTargetUid, ref effectTargetComponent, false) && effectTargetComponent.BehaviorProperties.BluespaceEffectMinInterval > 0.0)
			{
				uint curTimeTicks = this._timing.CurTick.Value;
				if (curTimeTicks < effectTargetComponent.BluespaceEffectNextTime)
				{
					return;
				}
				effectTargetComponent.BluespaceEffectNextTime = curTimeTicks + (uint)((double)this._timing.TickRate * effectTargetComponent.BehaviorProperties.BluespaceEffectMinInterval);
			}
			base.Spawn(effectSourceComponent.BehaviorProperties.BluespaceEffectPrototype, effectTargetUid.ToCoordinates());
		}

		// Token: 0x060006A9 RID: 1705 RVA: 0x0002022C File Offset: 0x0001E42C
		private void PreOpen(EntityUid uid, BluespaceLockerComponent component, ref StorageBeforeOpenEvent args)
		{
			EntityStorageComponent entityStorageComponent = null;
			int transportedEntities = 0;
			if (!base.Resolve<EntityStorageComponent>(uid, ref entityStorageComponent, true))
			{
				return;
			}
			if (!component.BehaviorProperties.ActOnOpen)
			{
				return;
			}
			ValueTuple<EntityUid, EntityStorageComponent, BluespaceLockerComponent>? target = this.GetTarget(uid, component, false);
			if (target == null)
			{
				return;
			}
			if (target.Value.Item2.Open)
			{
				this._entityStorage.CloseStorage(target.Value.Item1, target.Value.Item2);
			}
			if (target.Value.Item3 == null)
			{
				if (component.BehaviorProperties.TransportEntities || component.BehaviorProperties.TransportSentient)
				{
					foreach (EntityUid entity in target.Value.Item2.Contents.ContainedEntities.ToArray<EntityUid>())
					{
						if (this.EntityManager.HasComponent<MindComponent>(entity))
						{
							if (component.BehaviorProperties.TransportSentient)
							{
								entityStorageComponent.Contents.Insert(entity, this.EntityManager, null, null, null, null);
								transportedEntities++;
							}
						}
						else if (component.BehaviorProperties.TransportEntities)
						{
							entityStorageComponent.Contents.Insert(entity, this.EntityManager, null, null, null, null);
							transportedEntities++;
						}
					}
				}
				if (component.BehaviorProperties.TransportGas)
				{
					entityStorageComponent.Air.CopyFromMutable(target.Value.Item2.Air);
					target.Value.Item2.Air.Clear();
				}
				if (component.BehaviorProperties.BluespaceEffectOnTeleportSource)
				{
					this.BluespaceEffect(target.Value.Item1, component, target.Value.Item3, false);
				}
				if (component.BehaviorProperties.BluespaceEffectOnTeleportTarget)
				{
					this.BluespaceEffect(uid, component, component, false);
				}
			}
			this.DestroyAfterLimit(uid, component, transportedEntities);
		}

		// Token: 0x060006AA RID: 1706 RVA: 0x000203F8 File Offset: 0x0001E5F8
		private bool ValidLink(EntityUid locker, EntityUid link, BluespaceLockerComponent lockerComponent, bool intendToLink = false)
		{
			EntityStorageComponent linkStorage;
			return link.Valid && base.TryComp<EntityStorageComponent>(link, ref linkStorage) && linkStorage.LifeStage != 10 && !(link == locker) && (!lockerComponent.BehaviorProperties.InvalidateOneWayLinks || (intendToLink && lockerComponent.AutoLinksBidirectional) || (base.HasComp<BluespaceLockerComponent>(link) && base.Comp<BluespaceLockerComponent>(link).BluespaceLinks.Contains(locker)));
		}

		// Token: 0x060006AB RID: 1707 RVA: 0x00020468 File Offset: 0x0001E668
		private bool AccessMatch([Nullable(new byte[]
		{
			2,
			1,
			1
		})] IReadOnlyCollection<HashSet<string>> a, [Nullable(new byte[]
		{
			2,
			1,
			1
		})] IReadOnlyCollection<HashSet<string>> b)
		{
			if ((a == null || a.Count == 0) && (b == null || b.Count == 0))
			{
				return true;
			}
			if (a != null)
			{
				if (a.Any((HashSet<string> aSet) => aSet.Count == 0))
				{
					return true;
				}
			}
			if (b != null)
			{
				if (b.Any((HashSet<string> bSet) => bSet.Count == 0))
				{
					return true;
				}
			}
			return a != null && b != null && a.Any((HashSet<string> aSet) => b.Any(new Func<HashSet<string>, bool>(aSet.SetEquals)));
		}

		// Token: 0x060006AC RID: 1708 RVA: 0x00020528 File Offset: 0x0001E728
		private bool ValidAutolink(EntityUid locker, EntityUid link, BluespaceLockerComponent lockerComponent)
		{
			if (!this.ValidLink(locker, link, lockerComponent, true))
			{
				return false;
			}
			if (lockerComponent.PickLinksFromSameMap && link.ToCoordinates().GetMapId(this.EntityManager) != locker.ToCoordinates().GetMapId(this.EntityManager))
			{
				return false;
			}
			if (lockerComponent.PickLinksFromStationGrids && !base.HasComp<StationMemberComponent>(link.ToCoordinates().GetGridUid(this.EntityManager)))
			{
				return false;
			}
			if (lockerComponent.PickLinksFromResistLockers && !base.HasComp<ResistLockerComponent>(link))
			{
				return false;
			}
			if (lockerComponent.PickLinksFromSameAccess)
			{
				AccessReaderComponent sourceAccess;
				base.TryComp<AccessReaderComponent>(locker, ref sourceAccess);
				AccessReaderComponent targetAccess;
				base.TryComp<AccessReaderComponent>(link, ref targetAccess);
				if (!this.AccessMatch((sourceAccess != null) ? sourceAccess.AccessLists : null, (targetAccess != null) ? targetAccess.AccessLists : null))
				{
					return false;
				}
			}
			if (base.HasComp<BluespaceLockerComponent>(link))
			{
				if (lockerComponent.PickLinksFromNonBluespaceLockers)
				{
					return false;
				}
			}
			else if (lockerComponent.PickLinksFromBluespaceLockers)
			{
				return false;
			}
			return true;
		}

		// Token: 0x060006AD RID: 1709 RVA: 0x00020614 File Offset: 0x0001E814
		[return: TupleElementNames(new string[]
		{
			"uid",
			"storageComponent",
			"bluespaceLockerComponent"
		})]
		[return: Nullable(new byte[]
		{
			0,
			1,
			2
		})]
		public ValueTuple<EntityUid, EntityStorageComponent, BluespaceLockerComponent>? GetTarget(EntityUid lockerUid, BluespaceLockerComponent component, bool init = false)
		{
			EntityUid link;
			for (;;)
			{
				if ((long)component.BluespaceLinks.Count < (long)((ulong)component.MinBluespaceLinks))
				{
					EntityStorageComponent[] storages = base.EntityQuery<EntityStorageComponent>(false).ToArray<EntityStorageComponent>();
					this._robustRandom.Shuffle<EntityStorageComponent>(storages);
					EntityStorageComponent[] array = storages;
					for (int i = 0; i < array.Length; i++)
					{
						EntityUid potentialLink = array[i].Owner;
						if (this.ValidAutolink(lockerUid, potentialLink, component))
						{
							component.BluespaceLinks.Add(potentialLink);
							if (component.AutoLinksBidirectional || component.AutoLinksUseProperties)
							{
								BluespaceLockerComponent targetBluespaceComponent = base.CompOrNull<BluespaceLockerComponent>(potentialLink);
								if (targetBluespaceComponent == null)
								{
									targetBluespaceComponent = base.AddComp<BluespaceLockerComponent>(potentialLink);
									if (component.AutoLinksBidirectional)
									{
										targetBluespaceComponent.BluespaceLinks.Add(lockerUid);
									}
									if (component.AutoLinksUseProperties)
									{
										targetBluespaceComponent.BehaviorProperties = component.AutoLinkProperties.<Clone>$();
									}
									this.GetTarget(potentialLink, targetBluespaceComponent, true);
									this.BluespaceEffect(potentialLink, targetBluespaceComponent, targetBluespaceComponent, true);
								}
								else if (component.AutoLinksBidirectional)
								{
									targetBluespaceComponent.BluespaceLinks.Add(lockerUid);
								}
							}
							if ((long)component.BluespaceLinks.Count >= (long)((ulong)component.MinBluespaceLinks))
							{
								break;
							}
						}
					}
				}
				if (component.BluespaceLinks.Count == 0)
				{
					break;
				}
				link = component.BluespaceLinks.ToArray<EntityUid>()[this._robustRandom.Next(0, component.BluespaceLinks.Count)];
				if (this.ValidLink(lockerUid, link, component, false))
				{
					goto Block_10;
				}
				component.BluespaceLinks.Remove(link);
			}
			if (component.MinBluespaceLinks == 0U && !init)
			{
				base.RemComp<BluespaceLockerComponent>(lockerUid);
			}
			return null;
			Block_10:
			return new ValueTuple<EntityUid, EntityStorageComponent, BluespaceLockerComponent>?(new ValueTuple<EntityUid, EntityStorageComponent, BluespaceLockerComponent>(link, base.Comp<EntityStorageComponent>(link), base.CompOrNull<BluespaceLockerComponent>(link)));
		}

		// Token: 0x060006AE RID: 1710 RVA: 0x000207B7 File Offset: 0x0001E9B7
		private void PostClose(EntityUid uid, BluespaceLockerComponent component, ref StorageAfterCloseEvent args)
		{
			this.PostClose(uid, component, true);
		}

		// Token: 0x060006AF RID: 1711 RVA: 0x000207C2 File Offset: 0x0001E9C2
		private void OnDoAfter(EntityUid uid, BluespaceLockerComponent component, DoAfterEvent args)
		{
			if (args.Handled || args.Cancelled)
			{
				return;
			}
			this.PostClose(uid, component, false);
			args.Handled = true;
		}

		// Token: 0x060006B0 RID: 1712 RVA: 0x000207E8 File Offset: 0x0001E9E8
		private void PostClose(EntityUid uid, BluespaceLockerComponent component, bool doDelay = true)
		{
			EntityStorageComponent entityStorageComponent = null;
			int transportedEntities = 0;
			if (!base.Resolve<EntityStorageComponent>(uid, ref entityStorageComponent, true))
			{
				return;
			}
			if (!component.BehaviorProperties.ActOnClose)
			{
				return;
			}
			if (doDelay && component.BehaviorProperties.Delay > 0f)
			{
				base.EnsureComp<DoAfterComponent>(uid);
				this._doAfterSystem.DoAfter(new DoAfterEventArgs(uid, component.BehaviorProperties.Delay, default(CancellationToken), null, null));
				return;
			}
			ValueTuple<EntityUid, EntityStorageComponent, BluespaceLockerComponent>? target = this.GetTarget(uid, component, false);
			if (target == null)
			{
				return;
			}
			if (component.BehaviorProperties.TransportEntities || component.BehaviorProperties.TransportSentient)
			{
				foreach (EntityUid entity in entityStorageComponent.Contents.ContainedEntities.ToArray<EntityUid>())
				{
					if (this.EntityManager.HasComponent<MindComponent>(entity))
					{
						if (component.BehaviorProperties.TransportSentient)
						{
							target.Value.Item2.Contents.Insert(entity, this.EntityManager, null, null, null, null);
							transportedEntities++;
						}
					}
					else if (component.BehaviorProperties.TransportEntities)
					{
						target.Value.Item2.Contents.Insert(entity, this.EntityManager, null, null, null, null);
						transportedEntities++;
					}
				}
			}
			if (component.BehaviorProperties.TransportGas)
			{
				target.Value.Item2.Air.CopyFromMutable(entityStorageComponent.Air);
				entityStorageComponent.Air.Clear();
			}
			if (target.Value.Item2.Open)
			{
				this._entityStorage.EmptyContents(target.Value.Item1, target.Value.Item2);
				this._entityStorage.ReleaseGas(target.Value.Item1, target.Value.Item2);
			}
			else
			{
				if (target.Value.Item2.IsWeldedShut)
				{
					this._weldableSystem.ForceWeldedState(target.Value.Item1, false, null);
					if (target.Value.Item2.IsWeldedShut)
					{
						target.Value.Item2.IsWeldedShut = false;
					}
				}
				LockComponent lockComponent = null;
				if (base.Resolve<LockComponent>(target.Value.Item1, ref lockComponent, false) && lockComponent.Locked)
				{
					this._lockSystem.Unlock(target.Value.Item1, new EntityUid?(target.Value.Item1), lockComponent);
				}
				this._entityStorage.OpenStorage(target.Value.Item1, target.Value.Item2);
			}
			if (component.BehaviorProperties.BluespaceEffectOnTeleportSource)
			{
				this.BluespaceEffect(uid, component, component, false);
			}
			if (component.BehaviorProperties.BluespaceEffectOnTeleportTarget)
			{
				this.BluespaceEffect(target.Value.Item1, component, target.Value.Item3, false);
			}
			this.DestroyAfterLimit(uid, component, transportedEntities);
		}

		// Token: 0x060006B1 RID: 1713 RVA: 0x00020AEC File Offset: 0x0001ECEC
		private void DestroyAfterLimit(EntityUid uid, BluespaceLockerComponent component, int transportedEntities)
		{
			if (component.BehaviorProperties.DestroyAfterUsesMinItemsToCountUse > transportedEntities)
			{
				return;
			}
			if (component.BehaviorProperties.ClearLinksEvery != -1)
			{
				component.UsesSinceLinkClear++;
				if (component.BehaviorProperties.ClearLinksEvery <= component.UsesSinceLinkClear)
				{
					if (component.BehaviorProperties.ClearLinksDebluespaces)
					{
						foreach (EntityUid link in component.BluespaceLinks)
						{
							base.RemComp<BluespaceLockerComponent>(link);
						}
					}
					component.BluespaceLinks.Clear();
					component.UsesSinceLinkClear = 0;
				}
			}
			if (component.BehaviorProperties.DestroyAfterUses == -1)
			{
				return;
			}
			BluespaceLockerBehaviorProperties behaviorProperties = component.BehaviorProperties;
			int destroyAfterUses = behaviorProperties.DestroyAfterUses;
			behaviorProperties.DestroyAfterUses = destroyAfterUses - 1;
			if (component.BehaviorProperties.DestroyAfterUses > 0)
			{
				return;
			}
			switch (component.BehaviorProperties.DestroyType)
			{
			case BluespaceLockerDestroyType.Delete:
				break;
			case BluespaceLockerDestroyType.DeleteComponent:
				goto IL_12C;
			case BluespaceLockerDestroyType.Explode:
				this._explosionSystem.QueueExplosion(uid.ToCoordinates().ToMap(this.EntityManager), "Default", 4f, 1f, 2f, 1f, 0, true, false, false);
				break;
			default:
				goto IL_12C;
			}
			base.QueueDel(uid);
			return;
			IL_12C:
			base.RemComp<BluespaceLockerComponent>(uid);
		}

		// Token: 0x040003D9 RID: 985
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x040003DA RID: 986
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x040003DB RID: 987
		[Dependency]
		private readonly EntityStorageSystem _entityStorage;

		// Token: 0x040003DC RID: 988
		[Dependency]
		private readonly WeldableSystem _weldableSystem;

		// Token: 0x040003DD RID: 989
		[Dependency]
		private readonly LockSystem _lockSystem;

		// Token: 0x040003DE RID: 990
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x040003DF RID: 991
		[Dependency]
		private readonly ExplosionSystem _explosionSystem;
	}
}
