using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Map.Enumerators;

namespace Content.Server.Atmos.EntitySystems
{
	// Token: 0x02000793 RID: 1939
	public struct AtmosObstructionEnumerator
	{
		// Token: 0x06002942 RID: 10562 RVA: 0x000D6D47 File Offset: 0x000D4F47
		public AtmosObstructionEnumerator(AnchoredEntitiesEnumerator enumerator, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<AirtightComponent> query)
		{
			this._enumerator = enumerator;
			this._query = query;
		}

		// Token: 0x06002943 RID: 10563 RVA: 0x000D6D58 File Offset: 0x000D4F58
		[NullableContext(2)]
		public bool MoveNext([NotNullWhen(true)] out AirtightComponent airtight)
		{
			EntityUid? uid;
			if (!this._enumerator.MoveNext(ref uid))
			{
				airtight = null;
				return false;
			}
			return this._query.TryGetComponent(uid.Value, ref airtight) || this.MoveNext(out airtight);
		}

		// Token: 0x040019A7 RID: 6567
		private AnchoredEntitiesEnumerator _enumerator;

		// Token: 0x040019A8 RID: 6568
		[Nullable(new byte[]
		{
			0,
			1
		})]
		private EntityQuery<AirtightComponent> _query;
	}
}
