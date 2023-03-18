using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.Components;
using Content.Shared.Damage;
using Content.Shared.GameTicking;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Damage.Systems
{
	// Token: 0x020005C8 RID: 1480
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GodmodeSystem : EntitySystem
	{
		// Token: 0x06001F90 RID: 8080 RVA: 0x000A59E7 File Offset: 0x000A3BE7
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.Reset), null, null);
		}

		// Token: 0x06001F91 RID: 8081 RVA: 0x000A5A03 File Offset: 0x000A3C03
		public void Reset(RoundRestartCleanupEvent ev)
		{
			this._entities.Clear();
		}

		// Token: 0x06001F92 RID: 8082 RVA: 0x000A5A10 File Offset: 0x000A3C10
		public bool EnableGodmode(EntityUid entity)
		{
			if (this._entities.ContainsKey(entity))
			{
				return false;
			}
			this._entities[entity] = new GodmodeSystem.OldEntityInformation(entity, this.EntityManager);
			MovedByPressureComponent moved;
			if (this.EntityManager.TryGetComponent<MovedByPressureComponent>(entity, ref moved))
			{
				moved.Enabled = false;
			}
			DamageableComponent damageable;
			if (this.EntityManager.TryGetComponent<DamageableComponent>(entity, ref damageable))
			{
				this._damageableSystem.SetDamage(damageable, new DamageSpecifier());
			}
			return true;
		}

		// Token: 0x06001F93 RID: 8083 RVA: 0x000A5A7E File Offset: 0x000A3C7E
		public bool HasGodmode(EntityUid entity)
		{
			return this._entities.ContainsKey(entity);
		}

		// Token: 0x06001F94 RID: 8084 RVA: 0x000A5A8C File Offset: 0x000A3C8C
		public bool DisableGodmode(EntityUid entity)
		{
			GodmodeSystem.OldEntityInformation old;
			if (!this._entities.Remove(entity, out old))
			{
				return false;
			}
			MovedByPressureComponent moved;
			if (this.EntityManager.TryGetComponent<MovedByPressureComponent>(entity, ref moved))
			{
				moved.Enabled = old.MovedByPressure;
			}
			DamageableComponent damageable;
			if (this.EntityManager.TryGetComponent<DamageableComponent>(entity, ref damageable) && old.Damage != null)
			{
				this._damageableSystem.SetDamage(damageable, old.Damage);
			}
			return true;
		}

		// Token: 0x06001F95 RID: 8085 RVA: 0x000A5AF2 File Offset: 0x000A3CF2
		public bool ToggleGodmode(EntityUid entity)
		{
			if (this.HasGodmode(entity))
			{
				this.DisableGodmode(entity);
				return false;
			}
			this.EnableGodmode(entity);
			return true;
		}

		// Token: 0x04001397 RID: 5015
		private readonly Dictionary<EntityUid, GodmodeSystem.OldEntityInformation> _entities = new Dictionary<EntityUid, GodmodeSystem.OldEntityInformation>();

		// Token: 0x04001398 RID: 5016
		[Dependency]
		private readonly DamageableSystem _damageableSystem;

		// Token: 0x02000AB0 RID: 2736
		[NullableContext(2)]
		[Nullable(0)]
		public sealed class OldEntityInformation
		{
			// Token: 0x06003591 RID: 13713 RVA: 0x0011CC0C File Offset: 0x0011AE0C
			[NullableContext(1)]
			public OldEntityInformation(EntityUid entity, IEntityManager entityManager)
			{
				this.Entity = entity;
				this.MovedByPressure = entityManager.HasComponent<MovedByPressureComponent>(entity);
				DamageableComponent damageable;
				if (entityManager.TryGetComponent<DamageableComponent>(entity, ref damageable))
				{
					this.Damage = damageable.Damage;
				}
			}

			// Token: 0x17000840 RID: 2112
			// (get) Token: 0x06003592 RID: 13714 RVA: 0x0011CC4A File Offset: 0x0011AE4A
			public EntityUid Entity { get; }

			// Token: 0x17000841 RID: 2113
			// (get) Token: 0x06003593 RID: 13715 RVA: 0x0011CC52 File Offset: 0x0011AE52
			public bool MovedByPressure { get; }

			// Token: 0x17000842 RID: 2114
			// (get) Token: 0x06003594 RID: 13716 RVA: 0x0011CC5A File Offset: 0x0011AE5A
			public DamageSpecifier Damage { get; }
		}
	}
}
