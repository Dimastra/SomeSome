using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Client.CombatMode;
using Content.Client.Examine;
using Content.Client.Gameplay;
using Content.Client.Popups;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Content.Shared.Verbs;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Utility;

namespace Content.Client.Verbs
{
	// Token: 0x0200005E RID: 94
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class VerbSystem : SharedVerbSystem
	{
		// Token: 0x060001BB RID: 443 RVA: 0x0000C63A File Offset: 0x0000A83A
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeNetworkEvent<VerbsResponseEvent>(new EntityEventHandler<VerbsResponseEvent>(this.HandleVerbResponse), null, null);
		}

		// Token: 0x060001BC RID: 444 RVA: 0x0000C658 File Offset: 0x0000A858
		[NullableContext(2)]
		public bool TryGetEntityMenuEntities(MapCoordinates targetPos, [NotNullWhen(true)] out List<EntityUid> result)
		{
			VerbSystem.<>c__DisplayClass12_0 CS$<>8__locals1 = new VerbSystem.<>c__DisplayClass12_0();
			result = null;
			GameplayStateBase gameplayStateBase = this._stateManager.CurrentState as GameplayStateBase;
			if (gameplayStateBase == null)
			{
				return false;
			}
			VerbSystem.<>c__DisplayClass12_0 CS$<>8__locals2 = CS$<>8__locals1;
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			CS$<>8__locals2.player = ((localPlayer != null) ? localPlayer.ControlledEntity : null);
			if (CS$<>8__locals1.player == null)
			{
				return false;
			}
			MenuVisibility menuVisibility = this._eyeManager.CurrentEye.DrawFov ? this.Visibility : (this.Visibility | MenuVisibility.NoFov);
			List<EntityUid> list;
			if ((menuVisibility & MenuVisibility.NoFov) == MenuVisibility.Default)
			{
				CS$<>8__locals1.entitiesUnderMouse = gameplayStateBase.GetClickableEntities(targetPos).ToHashSet<EntityUid>();
				if (!this._examineSystem.CanExamine(CS$<>8__locals1.player.Value, targetPos, new SharedInteractionSystem.Ignored(CS$<>8__locals1.<TryGetEntityMenuEntities>g__Predicate|0), null, null))
				{
					return false;
				}
				ExaminerComponent examinerComp;
				base.TryComp<ExaminerComponent>(CS$<>8__locals1.player.Value, ref examinerComp);
				list = new List<EntityUid>();
				using (HashSet<EntityUid>.Enumerator enumerator = this._entityLookup.GetEntitiesInRange(targetPos, 0.25f, 46).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						EntityUid entityUid = enumerator.Current;
						if (this._examineSystem.CanExamine(CS$<>8__locals1.player.Value, targetPos, new SharedInteractionSystem.Ignored(CS$<>8__locals1.<TryGetEntityMenuEntities>g__Predicate|0), new EntityUid?(entityUid), examinerComp))
						{
							list.Add(entityUid);
						}
					}
					goto IL_16A;
				}
			}
			list = this._entityLookup.GetEntitiesInRange(targetPos, 0.25f, 46).ToList<EntityUid>();
			IL_16A:
			if (list.Count == 0)
			{
				return false;
			}
			if (menuVisibility == MenuVisibility.All)
			{
				result = list;
				return true;
			}
			if ((menuVisibility & MenuVisibility.InContainer) == MenuVisibility.Default)
			{
				for (int i = list.Count - 1; i >= 0; i--)
				{
					EntityUid entityUid2 = list[i];
					if (!this.ContainerSystem.IsInSameOrTransparentContainer(CS$<>8__locals1.player.Value, entityUid2, null, null, false))
					{
						Extensions.RemoveSwap<EntityUid>(list, i);
					}
				}
			}
			if ((menuVisibility & MenuVisibility.Invisible) == MenuVisibility.Default)
			{
				EntityQuery<SpriteComponent> entityQuery = base.GetEntityQuery<SpriteComponent>();
				EntityQuery<TagComponent> entityQuery2 = base.GetEntityQuery<TagComponent>();
				for (int j = list.Count - 1; j >= 0; j--)
				{
					EntityUid entityUid3 = list[j];
					SpriteComponent spriteComponent;
					if (!entityQuery.TryGetComponent(entityUid3, ref spriteComponent) || !spriteComponent.Visible || this._tagSystem.HasTag(entityUid3, "HideContextMenu", entityQuery2))
					{
						Extensions.RemoveSwap<EntityUid>(list, j);
					}
				}
			}
			if ((menuVisibility & MenuVisibility.NoFov) == MenuVisibility.Default)
			{
				EntityQuery<TransformComponent> entityQuery3 = base.GetEntityQuery<TransformComponent>();
				MapCoordinates mapPosition = entityQuery3.GetComponent(CS$<>8__locals1.player.Value).MapPosition;
				for (int k = list.Count - 1; k >= 0; k--)
				{
					EntityUid entityUid4 = list[k];
					if (!ExamineSystemShared.InRangeUnOccluded(mapPosition, entityQuery3.GetComponent(entityUid4).MapPosition, 16f, null, true, null))
					{
						Extensions.RemoveSwap<EntityUid>(list, k);
					}
				}
			}
			if (list.Count == 0)
			{
				return false;
			}
			result = list;
			return true;
		}

		// Token: 0x060001BD RID: 445 RVA: 0x0000C928 File Offset: 0x0000AB28
		public SortedSet<Verb> GetVerbs(EntityUid target, EntityUid user, Type type, bool force = false)
		{
			return this.GetVerbs(target, user, new List<Type>
			{
				type
			}, force);
		}

		// Token: 0x060001BE RID: 446 RVA: 0x0000C940 File Offset: 0x0000AB40
		public SortedSet<Verb> GetVerbs(EntityUid target, EntityUid user, List<Type> verbTypes, bool force = false)
		{
			if (!target.IsClientSide())
			{
				base.RaiseNetworkEvent(new RequestServerVerbsEvent(target, verbTypes, null, force));
			}
			if (!base.Exists(target))
			{
				return new SortedSet<Verb>();
			}
			return base.GetLocalVerbs(target, user, verbTypes, force);
		}

		// Token: 0x060001BF RID: 447 RVA: 0x0000C98C File Offset: 0x0000AB8C
		public void ExecuteVerb(EntityUid target, Verb verb)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			if (entityUid == null)
			{
				return;
			}
			if (verb.Disabled)
			{
				if (!string.IsNullOrWhiteSpace(verb.Message))
				{
					this._popupSystem.PopupEntity(verb.Message, entityUid.Value, PopupType.Small);
				}
				return;
			}
			if (verb.ClientExclusive)
			{
				this.ExecuteVerb(verb, entityUid.Value, target, false);
				return;
			}
			this.EntityManager.RaisePredictiveEvent<ExecuteVerbEvent>(new ExecuteVerbEvent(target, verb));
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x0000CA1D File Offset: 0x0000AC1D
		private void HandleVerbResponse(VerbsResponseEvent msg)
		{
			Action<VerbsResponseEvent> onVerbsResponse = this.OnVerbsResponse;
			if (onVerbsResponse == null)
			{
				return;
			}
			onVerbsResponse(msg);
		}

		// Token: 0x0400011B RID: 283
		[Dependency]
		private readonly CombatModeSystem _combatMode;

		// Token: 0x0400011C RID: 284
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x0400011D RID: 285
		[Dependency]
		private readonly ExamineSystem _examineSystem;

		// Token: 0x0400011E RID: 286
		[Dependency]
		private readonly TagSystem _tagSystem;

		// Token: 0x0400011F RID: 287
		[Dependency]
		private readonly IStateManager _stateManager;

		// Token: 0x04000120 RID: 288
		[Dependency]
		private readonly EntityLookupSystem _entityLookup;

		// Token: 0x04000121 RID: 289
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000122 RID: 290
		public const float EntityMenuLookupSize = 0.25f;

		// Token: 0x04000123 RID: 291
		[Dependency]
		private readonly IEyeManager _eyeManager;

		// Token: 0x04000124 RID: 292
		public MenuVisibility Visibility;

		// Token: 0x04000125 RID: 293
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public Action<VerbsResponseEvent> OnVerbsResponse;
	}
}
