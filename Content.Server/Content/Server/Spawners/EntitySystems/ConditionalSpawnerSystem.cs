using System;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking;
using Content.Server.Spawners.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Server.Spawners.EntitySystems
{
	// Token: 0x020001D2 RID: 466
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ConditionalSpawnerSystem : EntitySystem
	{
		// Token: 0x060008DD RID: 2269 RVA: 0x0002D1B8 File Offset: 0x0002B3B8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GameRuleStartedEvent>(new EntityEventHandler<GameRuleStartedEvent>(this.OnRuleStarted), null, null);
			base.SubscribeLocalEvent<ConditionalSpawnerComponent, MapInitEvent>(new ComponentEventHandler<ConditionalSpawnerComponent, MapInitEvent>(this.OnCondSpawnMapInit), null, null);
			base.SubscribeLocalEvent<RandomSpawnerComponent, MapInitEvent>(new ComponentEventHandler<RandomSpawnerComponent, MapInitEvent>(this.OnRandSpawnMapInit), null, null);
		}

		// Token: 0x060008DE RID: 2270 RVA: 0x0002D207 File Offset: 0x0002B407
		private void OnCondSpawnMapInit(EntityUid uid, ConditionalSpawnerComponent component, MapInitEvent args)
		{
			this.TrySpawn(component);
		}

		// Token: 0x060008DF RID: 2271 RVA: 0x0002D210 File Offset: 0x0002B410
		private void OnRandSpawnMapInit(EntityUid uid, RandomSpawnerComponent component, MapInitEvent args)
		{
			this.Spawn(component);
			this.EntityManager.QueueDeleteEntity(uid);
		}

		// Token: 0x060008E0 RID: 2272 RVA: 0x0002D228 File Offset: 0x0002B428
		private void OnRuleStarted(GameRuleStartedEvent args)
		{
			foreach (ConditionalSpawnerComponent spawner in this.EntityManager.EntityQuery<ConditionalSpawnerComponent>(false))
			{
				this.RuleStarted(spawner, args);
			}
		}

		// Token: 0x060008E1 RID: 2273 RVA: 0x0002D27C File Offset: 0x0002B47C
		public void RuleStarted(ConditionalSpawnerComponent component, GameRuleStartedEvent obj)
		{
			if (component.GameRules.Contains(obj.Rule.ID))
			{
				this.Spawn(component);
			}
		}

		// Token: 0x060008E2 RID: 2274 RVA: 0x0002D2A0 File Offset: 0x0002B4A0
		private void TrySpawn(ConditionalSpawnerComponent component)
		{
			if (component.GameRules.Count == 0)
			{
				this.Spawn(component);
				return;
			}
			foreach (string rule in component.GameRules)
			{
				if (this._ticker.IsGameRuleStarted(rule))
				{
					this.Spawn(component);
					break;
				}
			}
		}

		// Token: 0x060008E3 RID: 2275 RVA: 0x0002D318 File Offset: 0x0002B518
		private void Spawn(ConditionalSpawnerComponent component)
		{
			if (component.Chance != 1f && !RandomExtensions.Prob(this._robustRandom, component.Chance))
			{
				return;
			}
			if (component.Prototypes.Count == 0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(62, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Prototype list in ConditionalSpawnComponent is empty! Entity: ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(component.Owner);
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			if (!base.Deleted(component.Owner, null))
			{
				this.EntityManager.SpawnEntity(RandomExtensions.Pick<string>(this._robustRandom, component.Prototypes), base.Transform(component.Owner).Coordinates);
			}
		}

		// Token: 0x060008E4 RID: 2276 RVA: 0x0002D3C0 File Offset: 0x0002B5C0
		private void Spawn(RandomSpawnerComponent component)
		{
			if (component.RarePrototypes.Count > 0 && (component.RareChance == 1f || RandomExtensions.Prob(this._robustRandom, component.RareChance)))
			{
				this.EntityManager.SpawnEntity(RandomExtensions.Pick<string>(this._robustRandom, component.RarePrototypes), base.Transform(component.Owner).Coordinates);
				return;
			}
			if (component.Chance != 1f && !RandomExtensions.Prob(this._robustRandom, component.Chance))
			{
				return;
			}
			if (component.Prototypes.Count == 0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(59, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Prototype list in RandomSpawnerComponent is empty! Entity: ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(component.Owner);
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			if (base.Deleted(component.Owner, null))
			{
				return;
			}
			float offset = component.Offset;
			float xOffset = this._robustRandom.NextFloat(-offset, offset);
			float yOffset = this._robustRandom.NextFloat(-offset, offset);
			EntityCoordinates coordinates = base.Transform(component.Owner).Coordinates.Offset(new Vector2(xOffset, yOffset));
			this.EntityManager.SpawnEntity(RandomExtensions.Pick<string>(this._robustRandom, component.Prototypes), coordinates);
		}

		// Token: 0x0400055F RID: 1375
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x04000560 RID: 1376
		[Dependency]
		private readonly GameTicker _ticker;
	}
}
