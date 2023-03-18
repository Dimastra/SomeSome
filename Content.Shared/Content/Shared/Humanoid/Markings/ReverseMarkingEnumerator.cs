using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Content.Shared.Humanoid.Markings
{
	// Token: 0x02000424 RID: 1060
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ReverseMarkingEnumerator : IEnumerable<Marking>, IEnumerable
	{
		// Token: 0x06000CBD RID: 3261 RVA: 0x0002A2E4 File Offset: 0x000284E4
		public ReverseMarkingEnumerator(List<Marking> markings)
		{
			this._markings = markings;
		}

		// Token: 0x06000CBE RID: 3262 RVA: 0x0002A2F3 File Offset: 0x000284F3
		public IEnumerator<Marking> GetEnumerator()
		{
			return new MarkingsEnumerator(this._markings, true);
		}

		// Token: 0x06000CBF RID: 3263 RVA: 0x0002A301 File Offset: 0x00028501
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x04000C90 RID: 3216
		private List<Marking> _markings;
	}
}
