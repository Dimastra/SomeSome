using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Destructible;
using Content.Server.DoAfter;
using Content.Server.Gatherable.Components;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.EntityList;
using Content.Shared.Interaction;
using Content.Shared.Tag;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Gatherable
{
	// Token: 0x020004A1 RID: 1185
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GatherableSystem : EntitySystem
	{
		// Token: 0x060017D2 RID: 6098 RVA: 0x0007C3E6 File Offset: 0x0007A5E6
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GatherableComponent, InteractUsingEvent>(new ComponentEventHandler<GatherableComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
			base.SubscribeLocalEvent<GatherableComponent, DoAfterEvent>(new ComponentEventHandler<GatherableComponent, DoAfterEvent>(this.OnDoAfter), null, null);
		}

		// Token: 0x060017D3 RID: 6099 RVA: 0x0007C418 File Offset: 0x0007A618
		private void OnInteractUsing(EntityUid uid, GatherableComponent component, InteractUsingEvent args)
		{
			GatheringToolComponent tool;
			if (base.TryComp<GatheringToolComponent>(args.Used, ref tool))
			{
				EntityWhitelist toolWhitelist = component.ToolWhitelist;
				if (toolWhitelist == null || toolWhitelist.IsValid(args.Used, null))
				{
					if (tool.MaxGatheringEntities < tool.GatheringEntities.Count + 1)
					{
						return;
					}
					float damageTime = (this._destructible.DestroyedAt(uid, null) / tool.Damage.Total).Float();
					damageTime = Math.Max(1f, damageTime);
					EntityUid user = args.User;
					float delay = damageTime;
					EntityUid? target = new EntityUid?(uid);
					EntityUid? used = new EntityUid?(args.Used);
					DoAfterEventArgs doAfter = new DoAfterEventArgs(user, delay, default(CancellationToken), target, used)
					{
						BreakOnDamage = true,
						BreakOnStun = true,
						BreakOnTargetMove = true,
						BreakOnUserMove = true,
						MovementThreshold = 0.25f
					};
					this._doAfterSystem.DoAfter(doAfter);
					return;
				}
			}
		}

		// Token: 0x060017D4 RID: 6100 RVA: 0x0007C500 File Offset: 0x0007A700
		private void OnDoAfter(EntityUid uid, GatherableComponent component, DoAfterEvent args)
		{
			GatheringToolComponent tool;
			if (!base.TryComp<GatheringToolComponent>(args.Args.Used, ref tool) || args.Args.Target == null)
			{
				return;
			}
			if (args.Handled || args.Cancelled)
			{
				tool.GatheringEntities.Remove(args.Args.Target.Value);
				return;
			}
			this._destructible.DestroyEntity(args.Args.Target.Value);
			this._audio.PlayPvs(tool.GatheringSound, args.Args.Target.Value, null);
			tool.GatheringEntities.Remove(args.Args.Target.Value);
			if (component.MappedLoot == null)
			{
				return;
			}
			MapCoordinates playerPos = base.Transform(args.Args.User).MapPosition;
			foreach (KeyValuePair<string, string> keyValuePair in component.MappedLoot)
			{
				string text;
				string text2;
				keyValuePair.Deconstruct(out text, out text2);
				string tag = text;
				string table = text2;
				if (!(tag != "All") || this._tagSystem.HasTag(tool.Owner, tag))
				{
					List<string> spawnLoot = this._prototypeManager.Index<EntityLootTablePrototype>(table).GetSpawns(null);
					MapCoordinates spawnPos = playerPos.Offset(this._random.NextVector2(0.3f));
					base.Spawn(spawnLoot[0], spawnPos);
				}
			}
			args.Handled = true;
		}

		// Token: 0x04000EC4 RID: 3780
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000EC5 RID: 3781
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000EC6 RID: 3782
		[Dependency]
		private readonly DamageableSystem _damageableSystem;

		// Token: 0x04000EC7 RID: 3783
		[Dependency]
		private readonly DestructibleSystem _destructible;

		// Token: 0x04000EC8 RID: 3784
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x04000EC9 RID: 3785
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x04000ECA RID: 3786
		[Dependency]
		private readonly TagSystem _tagSystem;
	}
}
