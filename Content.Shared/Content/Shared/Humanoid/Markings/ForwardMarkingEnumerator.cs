using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Content.Shared.Humanoid.Markings
{
	// Token: 0x02000423 RID: 1059
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ForwardMarkingEnumerator : IEnumerable<Marking>, IEnumerable
	{
		// Token: 0x06000CBA RID: 3258 RVA: 0x0002A2BF File Offset: 0x000284BF
		public ForwardMarkingEnumerator(List<Marking> markings)
		{
			this._markings = markings;
		}

		// Token: 0x06000CBB RID: 3259 RVA: 0x0002A2CE File Offset: 0x000284CE
		public IEnumerator<Marking> GetEnumerator()
		{
			return new MarkingsEnumerator(this._markings, false);
		}

		// Token: 0x06000CBC RID: 3260 RVA: 0x0002A2DC File Offset: 0x000284DC
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x04000C8F RID: 3215
		private List<Marking> _markings;
	}
}
