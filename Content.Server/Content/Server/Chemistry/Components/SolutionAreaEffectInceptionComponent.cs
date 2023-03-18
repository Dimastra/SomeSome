using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.Chemistry.Components
{
	// Token: 0x020006AF RID: 1711
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class SolutionAreaEffectInceptionComponent : Component
	{
		// Token: 0x060023A9 RID: 9129 RVA: 0x000BA248 File Offset: 0x000B8448
		public void Setup(int amount, float duration, float spreadDelay, float removeDelay)
		{
			this._amountCounterSpreading = amount;
			this._duration = duration;
			this._spreadDelay = spreadDelay;
			this._removeDelay = removeDelay;
			this._reactionTimer = 1.5f;
			this._averageExposures = (duration + (float)amount * (spreadDelay + removeDelay) / 2f) / 1.5f;
		}

		// Token: 0x060023AA RID: 9130 RVA: 0x000BA298 File Offset: 0x000B8498
		public void InceptionUpdate(float frameTime)
		{
			this._group.RemoveWhere((SolutionAreaEffectComponent effect) => effect.Deleted);
			if (this._group.Count == 0)
			{
				return;
			}
			if (this._amountCounterSpreading > 0)
			{
				this._spreadTimer += frameTime;
				if (this._spreadTimer > this._spreadDelay)
				{
					this._spreadTimer -= this._spreadDelay;
					foreach (SolutionAreaEffectComponent solutionAreaEffectComponent in new HashSet<SolutionAreaEffectComponent>(from effect in this._group
					where effect.Amount == this._amountCounterSpreading
					select effect))
					{
						solutionAreaEffectComponent.Spread();
					}
					this._amountCounterSpreading--;
				}
			}
			else
			{
				this._lifeTimer += frameTime;
			}
			if (this._lifeTimer > this._duration)
			{
				this._spreadTimer += frameTime;
				if (this._spreadTimer > this._removeDelay)
				{
					this._spreadTimer -= this._removeDelay;
					foreach (SolutionAreaEffectComponent solutionAreaEffectComponent2 in new HashSet<SolutionAreaEffectComponent>(from effect in this._group
					where effect.Amount == this._amountCounterRemoving
					select effect))
					{
						solutionAreaEffectComponent2.Kill();
					}
					this._amountCounterRemoving++;
				}
			}
			this._reactionTimer += frameTime;
			if (this._reactionTimer > 1.5f)
			{
				this._reactionTimer -= 1.5f;
				foreach (SolutionAreaEffectComponent solutionAreaEffectComponent3 in this._group)
				{
					solutionAreaEffectComponent3.React(this._averageExposures);
				}
			}
		}

		// Token: 0x060023AB RID: 9131 RVA: 0x000BA4A8 File Offset: 0x000B86A8
		public void Add(SolutionAreaEffectComponent effect)
		{
			this._group.Add(effect);
			effect.Inception = this;
		}

		// Token: 0x060023AC RID: 9132 RVA: 0x000BA4BE File Offset: 0x000B86BE
		public void Remove(SolutionAreaEffectComponent effect)
		{
			this._group.Remove(effect);
			effect.Inception = null;
		}

		// Token: 0x04001605 RID: 5637
		private const float ReactionDelay = 1.5f;

		// Token: 0x04001606 RID: 5638
		private readonly HashSet<SolutionAreaEffectComponent> _group = new HashSet<SolutionAreaEffectComponent>();

		// Token: 0x04001607 RID: 5639
		[ViewVariables]
		private float _lifeTimer;

		// Token: 0x04001608 RID: 5640
		[ViewVariables]
		private float _spreadTimer;

		// Token: 0x04001609 RID: 5641
		[ViewVariables]
		private float _reactionTimer;

		// Token: 0x0400160A RID: 5642
		[ViewVariables]
		private int _amountCounterSpreading;

		// Token: 0x0400160B RID: 5643
		[ViewVariables]
		private int _amountCounterRemoving;

		// Token: 0x0400160C RID: 5644
		[ViewVariables]
		private float _duration;

		// Token: 0x0400160D RID: 5645
		[ViewVariables]
		private float _spreadDelay;

		// Token: 0x0400160E RID: 5646
		[ViewVariables]
		private float _removeDelay;

		// Token: 0x0400160F RID: 5647
		[ViewVariables]
		private float _averageExposures;
	}
}
