using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Destructible.Thresholds.Behaviors;
using Content.Server.Destructible.Thresholds.Triggers;
using Content.Shared.Damage;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Destructible.Thresholds
{
	// Token: 0x02000599 RID: 1433
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class DamageThreshold
	{
		// Token: 0x17000463 RID: 1123
		// (get) Token: 0x06001DDF RID: 7647 RVA: 0x0009ECD2 File Offset: 0x0009CED2
		// (set) Token: 0x06001DE0 RID: 7648 RVA: 0x0009ECDA File Offset: 0x0009CEDA
		[ViewVariables]
		public bool OldTriggered { get; private set; }

		// Token: 0x17000464 RID: 1124
		// (get) Token: 0x06001DE1 RID: 7649 RVA: 0x0009ECE3 File Offset: 0x0009CEE3
		// (set) Token: 0x06001DE2 RID: 7650 RVA: 0x0009ECEB File Offset: 0x0009CEEB
		[DataField("triggered", false, 1, false, false, null)]
		public bool Triggered { get; private set; }

		// Token: 0x17000465 RID: 1125
		// (get) Token: 0x06001DE3 RID: 7651 RVA: 0x0009ECF4 File Offset: 0x0009CEF4
		// (set) Token: 0x06001DE4 RID: 7652 RVA: 0x0009ECFC File Offset: 0x0009CEFC
		[DataField("triggersOnce", false, 1, false, false, null)]
		public bool TriggersOnce { get; set; }

		// Token: 0x17000466 RID: 1126
		// (get) Token: 0x06001DE5 RID: 7653 RVA: 0x0009ED05 File Offset: 0x0009CF05
		// (set) Token: 0x06001DE6 RID: 7654 RVA: 0x0009ED0D File Offset: 0x0009CF0D
		[Nullable(2)]
		[DataField("trigger", false, 1, false, false, null)]
		public IThresholdTrigger Trigger { [NullableContext(2)] get; [NullableContext(2)] set; }

		// Token: 0x17000467 RID: 1127
		// (get) Token: 0x06001DE7 RID: 7655 RVA: 0x0009ED16 File Offset: 0x0009CF16
		[ViewVariables]
		public IReadOnlyList<IThresholdBehavior> Behaviors
		{
			get
			{
				return this._behaviors;
			}
		}

		// Token: 0x06001DE8 RID: 7656 RVA: 0x0009ED20 File Offset: 0x0009CF20
		public bool Reached(DamageableComponent damageable, DestructibleSystem system)
		{
			if (this.Trigger == null)
			{
				return false;
			}
			if (this.Triggered && this.TriggersOnce)
			{
				return false;
			}
			if (this.OldTriggered)
			{
				this.OldTriggered = this.Trigger.Reached(damageable, system);
				return false;
			}
			if (!this.Trigger.Reached(damageable, system))
			{
				return false;
			}
			this.OldTriggered = true;
			return true;
		}

		// Token: 0x06001DE9 RID: 7657 RVA: 0x0009ED80 File Offset: 0x0009CF80
		public void Execute(EntityUid owner, DestructibleSystem system, IEntityManager entityManager, EntityUid? cause)
		{
			this.Triggered = true;
			foreach (IThresholdBehavior behavior in this.Behaviors)
			{
				if (!entityManager.EntityExists(owner))
				{
					break;
				}
				behavior.Execute(owner, system, cause);
			}
		}

		// Token: 0x0400132C RID: 4908
		[DataField("behaviors", false, 1, false, false, null)]
		private List<IThresholdBehavior> _behaviors = new List<IThresholdBehavior>();
	}
}
