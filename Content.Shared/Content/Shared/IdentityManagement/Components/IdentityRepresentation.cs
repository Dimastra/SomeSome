using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Enums;
using Robust.Shared.Localization;

namespace Content.Shared.IdentityManagement.Components
{
	// Token: 0x02000401 RID: 1025
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class IdentityRepresentation
	{
		// Token: 0x06000BF5 RID: 3061 RVA: 0x00027619 File Offset: 0x00025819
		[NullableContext(2)]
		public IdentityRepresentation([Nullable(1)] string trueName, int trueAge, Gender trueGender, string presumedName = null, string presumedJob = null)
		{
			this.TrueName = trueName;
			this.TrueAge = trueAge;
			this.TrueGender = trueGender;
			this.PresumedJob = presumedJob;
			this.PresumedName = presumedName;
		}

		// Token: 0x06000BF6 RID: 3062 RVA: 0x00027646 File Offset: 0x00025846
		public string ToStringKnown(bool trueName)
		{
			string result;
			if (!trueName)
			{
				if ((result = this.PresumedName) == null)
				{
					return this.ToStringUnknown();
				}
			}
			else
			{
				result = this.TrueName;
			}
			return result;
		}

		// Token: 0x06000BF7 RID: 3063 RVA: 0x00027664 File Offset: 0x00025864
		public string ToStringUnknown()
		{
			int trueAge = this.TrueAge;
			string @string;
			if (trueAge > 30)
			{
				if (trueAge > 60)
				{
					@string = Loc.GetString("identity-age-old");
				}
				else
				{
					@string = Loc.GetString("identity-age-middle-aged");
				}
			}
			else
			{
				@string = Loc.GetString("identity-age-young");
			}
			string ageString = @string;
			Gender trueGender = this.TrueGender;
			if (trueGender != 2)
			{
				if (trueGender != 3)
				{
					@string = Loc.GetString("identity-gender-person");
				}
				else
				{
					@string = Loc.GetString("identity-gender-masculine");
				}
			}
			else
			{
				@string = Loc.GetString("identity-gender-feminine");
			}
			string genderString = @string;
			if (this.PresumedJob != null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 3);
				defaultInterpolatedStringHandler.AppendFormatted(ageString);
				defaultInterpolatedStringHandler.AppendLiteral(" ");
				defaultInterpolatedStringHandler.AppendFormatted(this.PresumedJob);
				defaultInterpolatedStringHandler.AppendLiteral(" ");
				defaultInterpolatedStringHandler.AppendFormatted(genderString);
				return defaultInterpolatedStringHandler.ToStringAndClear();
			}
			return ageString + " " + genderString;
		}

		// Token: 0x04000BE6 RID: 3046
		public string TrueName;

		// Token: 0x04000BE7 RID: 3047
		public int TrueAge;

		// Token: 0x04000BE8 RID: 3048
		public Gender TrueGender;

		// Token: 0x04000BE9 RID: 3049
		[Nullable(2)]
		public string PresumedName;

		// Token: 0x04000BEA RID: 3050
		[Nullable(2)]
		public string PresumedJob;
	}
}
