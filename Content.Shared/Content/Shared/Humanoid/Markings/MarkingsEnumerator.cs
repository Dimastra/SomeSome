using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Content.Shared.Humanoid.Markings
{
	// Token: 0x02000425 RID: 1061
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MarkingsEnumerator : IEnumerator<Marking>, IEnumerator, IDisposable
	{
		// Token: 0x06000CC0 RID: 3264 RVA: 0x0002A309 File Offset: 0x00028509
		public MarkingsEnumerator(List<Marking> markings, bool reverse)
		{
			this._markings = markings;
			this._reverse = reverse;
			if (this._reverse)
			{
				this.position = this._markings.Count;
				return;
			}
			this.position = -1;
		}

		// Token: 0x06000CC1 RID: 3265 RVA: 0x0002A340 File Offset: 0x00028540
		public bool MoveNext()
		{
			if (this._reverse)
			{
				this.position--;
				return this.position >= 0;
			}
			this.position++;
			return this.position < this._markings.Count;
		}

		// Token: 0x06000CC2 RID: 3266 RVA: 0x0002A391 File Offset: 0x00028591
		public void Reset()
		{
			if (this._reverse)
			{
				this.position = this._markings.Count;
				return;
			}
			this.position = -1;
		}

		// Token: 0x06000CC3 RID: 3267 RVA: 0x0002A3B4 File Offset: 0x000285B4
		public void Dispose()
		{
		}

		// Token: 0x170002A1 RID: 673
		// (get) Token: 0x06000CC4 RID: 3268 RVA: 0x0002A3B6 File Offset: 0x000285B6
		object IEnumerator.Current
		{
			get
			{
				return this._markings[this.position];
			}
		}

		// Token: 0x170002A2 RID: 674
		// (get) Token: 0x06000CC5 RID: 3269 RVA: 0x0002A3C9 File Offset: 0x000285C9
		public Marking Current
		{
			get
			{
				return this._markings[this.position];
			}
		}

		// Token: 0x04000C91 RID: 3217
		private List<Marking> _markings;

		// Token: 0x04000C92 RID: 3218
		private bool _reverse;

		// Token: 0x04000C93 RID: 3219
		private int position;
	}
}
