using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Server.Construction
{
	// Token: 0x020005F0 RID: 1520
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class UpgradeExamineEvent : EntityEventArgs
	{
		// Token: 0x0600209B RID: 8347 RVA: 0x000AB67E File Offset: 0x000A987E
		public UpgradeExamineEvent(ref FormattedMessage message)
		{
			this.Message = message;
		}

		// Token: 0x0600209C RID: 8348 RVA: 0x000AB690 File Offset: 0x000A9890
		public void AddPercentageUpgrade(string upgradedLocId, float multiplier)
		{
			double percent = Math.Round((double)(100f * MathF.Abs(multiplier - 1f)), 2);
			string text;
			if (multiplier >= 1f)
			{
				if (multiplier != 1f && !float.IsNaN(multiplier))
				{
					text = "machine-upgrade-increased-by-percentage";
				}
				else
				{
					text = "machine-upgrade-not-upgraded";
				}
			}
			else
			{
				text = "machine-upgrade-decreased-by-percentage";
			}
			string locId = text;
			string upgraded = Loc.GetString(upgradedLocId);
			this.Message.AddMarkup(Loc.GetString(locId, new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("upgraded", upgraded),
				new ValueTuple<string, object>("percent", percent)
			}) + "\n");
		}

		// Token: 0x0600209D RID: 8349 RVA: 0x000AB73C File Offset: 0x000A993C
		public void AddNumberUpgrade(string upgradedLocId, int number)
		{
			int difference = Math.Abs(number);
			string text;
			if (number >= 0)
			{
				if (number != 0)
				{
					text = "machine-upgrade-increased-by-amount";
				}
				else
				{
					text = "machine-upgrade-not-upgraded";
				}
			}
			else
			{
				text = "machine-upgrade-decreased-by-amount";
			}
			string locId = text;
			string upgraded = Loc.GetString(upgradedLocId);
			this.Message.AddMarkup(Loc.GetString(locId, new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("upgraded", upgraded),
				new ValueTuple<string, object>("difference", difference)
			}) + "\n");
		}

		// Token: 0x04001422 RID: 5154
		private FormattedMessage Message;
	}
}
