using System;
using System.Runtime.CompilerServices;
using Robust.Server;

namespace Content.Server
{
	// Token: 0x02000017 RID: 23
	internal static class Program
	{
		// Token: 0x06000044 RID: 68 RVA: 0x00003001 File Offset: 0x00001201
		[NullableContext(1)]
		public static void Main(string[] args)
		{
			ContentStart.Start(args);
		}
	}
}
