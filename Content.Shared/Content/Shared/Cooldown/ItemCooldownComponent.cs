using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Cooldown
{
	// Token: 0x0200055B RID: 1371
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class ItemCooldownComponent : Component
	{
		// Token: 0x1700034A RID: 842
		// (get) Token: 0x060010A3 RID: 4259 RVA: 0x00036337 File Offset: 0x00034537
		// (set) Token: 0x060010A4 RID: 4260 RVA: 0x0003633F File Offset: 0x0003453F
		[ViewVariables]
		public TimeSpan? CooldownEnd
		{
			get
			{
				return this._cooldownEnd;
			}
			set
			{
				this._cooldownEnd = value;
				base.Dirty(null);
			}
		}

		// Token: 0x1700034B RID: 843
		// (get) Token: 0x060010A5 RID: 4261 RVA: 0x0003634F File Offset: 0x0003454F
		// (set) Token: 0x060010A6 RID: 4262 RVA: 0x00036357 File Offset: 0x00034557
		[ViewVariables]
		public TimeSpan? CooldownStart
		{
			get
			{
				return this._cooldownStart;
			}
			set
			{
				this._cooldownStart = value;
				base.Dirty(null);
			}
		}

		// Token: 0x060010A7 RID: 4263 RVA: 0x00036367 File Offset: 0x00034567
		[NullableContext(1)]
		public override ComponentState GetComponentState()
		{
			return new ItemCooldownComponent.ItemCooldownComponentState
			{
				CooldownEnd = this.CooldownEnd,
				CooldownStart = this.CooldownStart
			};
		}

		// Token: 0x060010A8 RID: 4264 RVA: 0x00036388 File Offset: 0x00034588
		[NullableContext(2)]
		public override void HandleComponentState(ComponentState curState, ComponentState nextState)
		{
			base.HandleComponentState(curState, nextState);
			ItemCooldownComponent.ItemCooldownComponentState cast = curState as ItemCooldownComponent.ItemCooldownComponentState;
			if (cast == null)
			{
				return;
			}
			this.CooldownStart = cast.CooldownStart;
			this.CooldownEnd = cast.CooldownEnd;
		}

		// Token: 0x04000F96 RID: 3990
		private TimeSpan? _cooldownEnd;

		// Token: 0x04000F97 RID: 3991
		private TimeSpan? _cooldownStart;

		// Token: 0x0200083B RID: 2107
		[NetSerializable]
		[Serializable]
		private sealed class ItemCooldownComponentState : ComponentState
		{
			// Token: 0x1700051E RID: 1310
			// (get) Token: 0x06001925 RID: 6437 RVA: 0x0004F977 File Offset: 0x0004DB77
			// (set) Token: 0x06001926 RID: 6438 RVA: 0x0004F97F File Offset: 0x0004DB7F
			public TimeSpan? CooldownStart { get; set; }

			// Token: 0x1700051F RID: 1311
			// (get) Token: 0x06001927 RID: 6439 RVA: 0x0004F988 File Offset: 0x0004DB88
			// (set) Token: 0x06001928 RID: 6440 RVA: 0x0004F990 File Offset: 0x0004DB90
			public TimeSpan? CooldownEnd { get; set; }
		}
	}
}
