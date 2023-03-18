using System;
using System.Runtime.CompilerServices;
using Robust.Shared.IoC;

namespace Content.Server.Administration.Logs.Converters
{
	// Token: 0x0200081E RID: 2078
	[NullableContext(1)]
	public interface IAdminLogConverter
	{
		// Token: 0x06002DB5 RID: 11701
		void Init(IDependencyCollection dependencies);
	}
}
