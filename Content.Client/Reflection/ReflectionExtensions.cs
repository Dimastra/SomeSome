using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Robust.Shared.Reflection;

namespace Content.Client.Reflection
{
	// Token: 0x02000173 RID: 371
	public static class ReflectionExtensions
	{
		// Token: 0x060009BF RID: 2495 RVA: 0x00038C65 File Offset: 0x00036E65
		[NullableContext(1)]
		public static bool IsInIntegrationTest(this IReflectionManager refl)
		{
			return refl.Assemblies.Any((Assembly a) => a.FullName != null && a.FullName.Contains("IntegrationTests"));
		}
	}
}
