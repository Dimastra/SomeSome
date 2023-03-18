using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Content.Shared.Administration
{
	// Token: 0x02000731 RID: 1841
	[NullableContext(1)]
	[Nullable(0)]
	public static class AdminFlagsHelper
	{
		// Token: 0x06001649 RID: 5705 RVA: 0x00048EC0 File Offset: 0x000470C0
		static AdminFlagsHelper()
		{
			AdminFlags[] array = (AdminFlags[])Enum.GetValues(typeof(AdminFlags));
			List<AdminFlags> allFlags = new List<AdminFlags>();
			foreach (AdminFlags value in array)
			{
				string name = value.ToString().ToUpper();
				if (BitOperations.PopCount((uint)value) == 1)
				{
					allFlags.Add(value);
					AdminFlagsHelper.Everything |= value;
					AdminFlagsHelper.NameFlagsMap.Add(name, value);
					AdminFlagsHelper.FlagsNameMap[BitOperations.Log2((uint)value)] = name;
				}
			}
			AdminFlagsHelper.AllFlags = allFlags.ToArray();
		}

		// Token: 0x0600164A RID: 5706 RVA: 0x00048F68 File Offset: 0x00047168
		public static AdminFlags NamesToFlags(IEnumerable<string> names)
		{
			AdminFlags flags = AdminFlags.None;
			foreach (string name in names)
			{
				AdminFlags value;
				if (!AdminFlagsHelper.NameFlagsMap.TryGetValue(name, out value))
				{
					throw new ArgumentException("Invalid admin flag name: " + name);
				}
				flags |= value;
			}
			return flags;
		}

		// Token: 0x0600164B RID: 5707 RVA: 0x00048FD0 File Offset: 0x000471D0
		public static AdminFlags NameToFlag(string name)
		{
			return AdminFlagsHelper.NameFlagsMap[name];
		}

		// Token: 0x0600164C RID: 5708 RVA: 0x00048FE0 File Offset: 0x000471E0
		public static string[] FlagsToNames(AdminFlags flags)
		{
			string[] array = new string[BitOperations.PopCount((uint)flags)];
			int highest = BitOperations.LeadingZeroCount((uint)flags);
			int ai = 0;
			for (int i = 0; i < 32 - highest; i++)
			{
				AdminFlags flagValue = (AdminFlags)(1 << i);
				if ((flags & flagValue) != AdminFlags.None)
				{
					array[ai++] = AdminFlagsHelper.FlagsNameMap[i];
				}
			}
			return array;
		}

		// Token: 0x0600164D RID: 5709 RVA: 0x00049030 File Offset: 0x00047230
		public static string PosNegFlagsText(AdminFlags posFlags, AdminFlags negFlags)
		{
			IEnumerable<ValueTuple<string, string>> posFlagNames = from f in AdminFlagsHelper.FlagsToNames(posFlags)
			select new ValueTuple<string, string>(f, "+" + f);
			IEnumerable<ValueTuple<string, string>> negFlagNames = from f in AdminFlagsHelper.FlagsToNames(negFlags)
			select new ValueTuple<string, string>(f, "-" + f);
			return string.Join<string>(' ', from f in posFlagNames.Concat(negFlagNames)
			orderby f.Item1
			select f into p
			select p.Item2);
		}

		// Token: 0x040016A5 RID: 5797
		private static readonly Dictionary<string, AdminFlags> NameFlagsMap = new Dictionary<string, AdminFlags>();

		// Token: 0x040016A6 RID: 5798
		private static readonly string[] FlagsNameMap = new string[32];

		// Token: 0x040016A7 RID: 5799
		public static readonly AdminFlags Everything;

		// Token: 0x040016A8 RID: 5800
		public static readonly IReadOnlyList<AdminFlags> AllFlags;
	}
}
