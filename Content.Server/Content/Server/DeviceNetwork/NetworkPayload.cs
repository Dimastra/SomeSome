using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Robust.Shared.Utility;

namespace Content.Server.DeviceNetwork
{
	// Token: 0x0200057F RID: 1407
	[NullableContext(2)]
	[Nullable(new byte[]
	{
		0,
		1,
		1
	})]
	public sealed class NetworkPayload : Dictionary<string, object>
	{
		// Token: 0x06001D7B RID: 7547 RVA: 0x0009D224 File Offset: 0x0009B424
		public bool TryGetValue<T>([Nullable(1)] string key, [NotNullWhen(true)] out T value)
		{
			T result;
			if (Extensions.TryCastValue<T, string, object>(this, key, ref result))
			{
				value = result;
				return true;
			}
			value = default(T);
			return false;
		}
	}
}
