using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Hands.Components;
using Content.Server.Popups;
using Content.Shared.Administration.Logs;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Database;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;

namespace Content.Server.Chat
{
	// Token: 0x020006BC RID: 1724
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SuicideSystem : EntitySystem
	{
		// Token: 0x060023D8 RID: 9176 RVA: 0x000BA8D0 File Offset: 0x000B8AD0
		public bool Suicide(EntityUid victim)
		{
			if (this._tagSystem.HasTag(victim, "CannotSuicide"))
			{
				return false;
			}
			MobStateComponent mobState;
			if (!base.TryComp<MobStateComponent>(victim, ref mobState) || this._mobState.IsDead(victim, mobState))
			{
				return false;
			}
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Mind;
			LogStringHandler logStringHandler = new LogStringHandler(25, 1);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(this.EntityManager.ToPrettyString(victim), "player", "EntityManager.ToPrettyString(victim)");
			logStringHandler.AppendLiteral(" is attempting to suicide");
			adminLogger.Add(type, ref logStringHandler);
			SuicideEvent suicideEvent = new SuicideEvent(victim);
			if (this.SuicideAttemptBlocked(victim, suicideEvent))
			{
				return false;
			}
			bool environmentSuicide = false;
			if (!this._mobState.IsCritical(victim, mobState))
			{
				environmentSuicide = this.EnvironmentSuicideHandler(victim, suicideEvent);
			}
			if (suicideEvent.AttemptBlocked)
			{
				return false;
			}
			SuicideSystem.DefaultSuicideHandler(victim, suicideEvent);
			this.ApplyDeath(victim, suicideEvent.Kind.Value);
			ISharedAdminLogManager adminLogger2 = this._adminLogger;
			LogType type2 = LogType.Mind;
			logStringHandler = new LogStringHandler(9, 2);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(this.EntityManager.ToPrettyString(victim), "player", "EntityManager.ToPrettyString(victim)");
			logStringHandler.AppendLiteral(" suicided");
			logStringHandler.AppendFormatted(environmentSuicide ? " (environment)" : "");
			adminLogger2.Add(type2, ref logStringHandler);
			return true;
		}

		// Token: 0x060023D9 RID: 9177 RVA: 0x000BAA00 File Offset: 0x000B8C00
		private static void DefaultSuicideHandler(EntityUid victim, SuicideEvent suicideEvent)
		{
			if (suicideEvent.Handled)
			{
				return;
			}
			string othersMessage = Loc.GetString("suicide-command-default-text-others", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("name", victim)
			});
			victim.PopupMessageOtherClients(othersMessage);
			string selfMessage = Loc.GetString("suicide-command-default-text-self");
			victim.PopupMessage(selfMessage);
			suicideEvent.SetHandled(SuicideKind.Bloodloss);
		}

		// Token: 0x060023DA RID: 9178 RVA: 0x000BAA5F File Offset: 0x000B8C5F
		private bool SuicideAttemptBlocked(EntityUid victim, SuicideEvent suicideEvent)
		{
			base.RaiseLocalEvent<SuicideEvent>(victim, suicideEvent, false);
			return suicideEvent.AttemptBlocked;
		}

		// Token: 0x060023DB RID: 9179 RVA: 0x000BAA78 File Offset: 0x000B8C78
		private bool EnvironmentSuicideHandler(EntityUid victim, SuicideEvent suicideEvent)
		{
			EntityQuery<ItemComponent> itemQuery = base.GetEntityQuery<ItemComponent>();
			HandsComponent handsComponent;
			if (this.EntityManager.TryGetComponent<HandsComponent>(victim, ref handsComponent))
			{
				EntityUid? activeHandEntity = handsComponent.ActiveHandEntity;
				if (activeHandEntity != null)
				{
					EntityUid item = activeHandEntity.GetValueOrDefault();
					base.RaiseLocalEvent<SuicideEvent>(item, suicideEvent, false);
					if (suicideEvent.Handled)
					{
						return true;
					}
				}
			}
			foreach (EntityUid entity in this._entityLookupSystem.GetEntitiesInRange(victim, 1f, 5))
			{
				if (!itemQuery.HasComponent(entity))
				{
					base.RaiseLocalEvent<SuicideEvent>(entity, suicideEvent, false);
					if (suicideEvent.Handled)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060023DC RID: 9180 RVA: 0x000BAB3C File Offset: 0x000B8D3C
		private void ApplyDeath(EntityUid target, SuicideKind kind)
		{
			if (kind == SuicideKind.Special)
			{
				return;
			}
			DamageTypePrototype damagePrototype;
			if (!this._prototypeManager.TryIndex<DamageTypePrototype>(kind.ToString(), ref damagePrototype))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(76, 3);
				defaultInterpolatedStringHandler.AppendFormatted("SuicideSystem");
				defaultInterpolatedStringHandler.AppendLiteral(" could not find the damage type prototype associated with ");
				defaultInterpolatedStringHandler.AppendFormatted<SuicideKind>(kind);
				defaultInterpolatedStringHandler.AppendLiteral(". Falling back to ");
				defaultInterpolatedStringHandler.AppendFormatted<SuicideKind>(SuicideKind.Blunt);
				Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				damagePrototype = this._prototypeManager.Index<DamageTypePrototype>(SuicideKind.Blunt.ToString());
			}
			this._damageableSystem.TryChangeDamage(new EntityUid?(target), new DamageSpecifier(damagePrototype, 200), true, true, null, new EntityUid?(target));
		}

		// Token: 0x0400162D RID: 5677
		[Dependency]
		private readonly DamageableSystem _damageableSystem;

		// Token: 0x0400162E RID: 5678
		[Dependency]
		private readonly EntityLookupSystem _entityLookupSystem;

		// Token: 0x0400162F RID: 5679
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x04001630 RID: 5680
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04001631 RID: 5681
		[Dependency]
		private readonly TagSystem _tagSystem;

		// Token: 0x04001632 RID: 5682
		[Dependency]
		private readonly MobStateSystem _mobState;
	}
}
