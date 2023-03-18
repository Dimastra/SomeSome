using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Disposal.Unit.Components;
using Content.Server.Disposal.Unit.EntitySystems;
using Content.Server.DoAfter;
using Content.Server.Storage.Components;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Placeable;
using Content.Shared.Storage;
using Content.Shared.Storage.Components;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Server.Storage.EntitySystems
{
	// Token: 0x0200015F RID: 351
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DumpableSystem : EntitySystem
	{
		// Token: 0x060006B6 RID: 1718 RVA: 0x00020D6C File Offset: 0x0001EF6C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<DumpableComponent, AfterInteractEvent>(new ComponentEventHandler<DumpableComponent, AfterInteractEvent>(this.OnAfterInteract), null, new Type[]
			{
				typeof(StorageSystem)
			});
			base.SubscribeLocalEvent<DumpableComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<DumpableComponent, GetVerbsEvent<AlternativeVerb>>(this.AddDumpVerb), null, null);
			base.SubscribeLocalEvent<DumpableComponent, GetVerbsEvent<UtilityVerb>>(new ComponentEventHandler<DumpableComponent, GetVerbsEvent<UtilityVerb>>(this.AddUtilityVerbs), null, null);
			base.SubscribeLocalEvent<DumpableComponent, DoAfterEvent>(new ComponentEventHandler<DumpableComponent, DoAfterEvent>(this.OnDoAfter), null, null);
		}

		// Token: 0x060006B7 RID: 1719 RVA: 0x00020DE4 File Offset: 0x0001EFE4
		private void OnAfterInteract(EntityUid uid, DumpableComponent component, AfterInteractEvent args)
		{
			if (!args.CanReach)
			{
				return;
			}
			if (base.HasComp<DisposalUnitComponent>(args.Target) || base.HasComp<PlaceableSurfaceComponent>(args.Target))
			{
				this.StartDoAfter(uid, new EntityUid?(args.Target.Value), args.User, component);
			}
		}

		// Token: 0x060006B8 RID: 1720 RVA: 0x00020E38 File Offset: 0x0001F038
		private void AddDumpVerb(EntityUid uid, DumpableComponent dumpable, GetVerbsEvent<AlternativeVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract)
			{
				return;
			}
			ServerStorageComponent storage;
			if (!base.TryComp<ServerStorageComponent>(uid, ref storage) || storage.StoredEntities == null || storage.StoredEntities.Count == 0)
			{
				return;
			}
			AlternativeVerb verb = new AlternativeVerb
			{
				Act = delegate()
				{
					this.StartDoAfter(uid, null, args.User, dumpable);
				},
				Text = Loc.GetString("dump-verb-name"),
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/drop.svg.192dpi.png", "/"))
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x060006B9 RID: 1721 RVA: 0x00020EFC File Offset: 0x0001F0FC
		private void AddUtilityVerbs(EntityUid uid, DumpableComponent dumpable, GetVerbsEvent<UtilityVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract)
			{
				return;
			}
			ServerStorageComponent storage;
			if (!base.TryComp<ServerStorageComponent>(uid, ref storage) || storage.StoredEntities == null || storage.StoredEntities.Count == 0)
			{
				return;
			}
			if (base.HasComp<DisposalUnitComponent>(args.Target))
			{
				UtilityVerb verb = new UtilityVerb
				{
					Act = delegate()
					{
						this.StartDoAfter(uid, new EntityUid?(args.Target), args.User, dumpable);
					},
					Text = Loc.GetString("dump-disposal-verb-name", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("unit", args.Target)
					}),
					IconEntity = new EntityUid?(uid)
				};
				args.Verbs.Add(verb);
			}
			if (base.HasComp<PlaceableSurfaceComponent>(args.Target))
			{
				UtilityVerb verb2 = new UtilityVerb
				{
					Act = delegate()
					{
						this.StartDoAfter(uid, new EntityUid?(args.Target), args.User, dumpable);
					},
					Text = Loc.GetString("dump-placeable-verb-name", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("surface", args.Target)
					}),
					IconEntity = new EntityUid?(uid)
				};
				args.Verbs.Add(verb2);
			}
		}

		// Token: 0x060006BA RID: 1722 RVA: 0x0002107C File Offset: 0x0001F27C
		public void StartDoAfter(EntityUid storageUid, EntityUid? targetUid, EntityUid userUid, DumpableComponent dumpable)
		{
			SharedStorageComponent storage;
			if (!base.TryComp<SharedStorageComponent>(storageUid, ref storage) || storage.StoredEntities == null)
			{
				return;
			}
			float delay = (float)storage.StoredEntities.Count * (float)dumpable.DelayPerItem.TotalSeconds * dumpable.Multiplier;
			SharedDoAfterSystem doAfterSystem = this._doAfterSystem;
			float delay2 = delay;
			EntityUid? used = new EntityUid?(storageUid);
			doAfterSystem.DoAfter(new DoAfterEventArgs(userUid, delay2, default(CancellationToken), targetUid, used)
			{
				BreakOnTargetMove = true,
				BreakOnUserMove = true,
				BreakOnStun = true,
				NeedHand = true
			});
		}

		// Token: 0x060006BB RID: 1723 RVA: 0x00021108 File Offset: 0x0001F308
		private void OnDoAfter(EntityUid uid, DumpableComponent component, DoAfterEvent args)
		{
			SharedStorageComponent storage;
			if (args.Handled || args.Cancelled || !base.TryComp<SharedStorageComponent>(uid, ref storage) || storage.StoredEntities == null)
			{
				return;
			}
			Queue<EntityUid> dumpQueue = new Queue<EntityUid>();
			foreach (EntityUid entity in storage.StoredEntities)
			{
				dumpQueue.Enqueue(entity);
			}
			foreach (EntityUid entity2 in dumpQueue)
			{
				TransformComponent transform = base.Transform(entity2);
				this._container.AttachParentToContainerOrGrid(transform);
				transform.LocalPosition += this._random.NextVector2Box(1f, 1f) / 2f;
				transform.LocalRotation = this._random.NextAngle();
			}
			if (args.Args.Target == null)
			{
				return;
			}
			if (base.HasComp<DisposalUnitComponent>(args.Args.Target.Value))
			{
				foreach (EntityUid entity3 in dumpQueue)
				{
					this._disposalUnitSystem.DoInsertDisposalUnit(args.Args.Target.Value, entity3, args.Args.User, null);
				}
				return;
			}
			if (base.HasComp<PlaceableSurfaceComponent>(args.Args.Target.Value))
			{
				foreach (EntityUid entity4 in dumpQueue)
				{
					base.Transform(entity4).LocalPosition = base.Transform(args.Args.Target.Value).LocalPosition + this._random.NextVector2Box(1f, 1f) / 4f;
				}
			}
		}

		// Token: 0x040003E2 RID: 994
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x040003E3 RID: 995
		[Dependency]
		private readonly DisposalUnitSystem _disposalUnitSystem;

		// Token: 0x040003E4 RID: 996
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040003E5 RID: 997
		[Dependency]
		private readonly SharedContainerSystem _container;
	}
}
