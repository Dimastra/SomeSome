using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Robust.Client.Input;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Client.UserInterface
{
	// Token: 0x0200006A RID: 106
	[NullableContext(2)]
	[Nullable(0)]
	public static class BoundKeyHelper
	{
		// Token: 0x060001F5 RID: 501 RVA: 0x0000DC9C File Offset: 0x0000BE9C
		[NullableContext(1)]
		public static string ShortKeyName(BoundKeyFunction keyFunction)
		{
			string text;
			if (!BoundKeyHelper.TryGetShortKeyName(keyFunction, out text))
			{
				return " ";
			}
			return Loc.GetString(text);
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x0000DCC0 File Offset: 0x0000BEC0
		private static string DefaultShortKeyName(BoundKeyFunction keyFunction)
		{
			string text = FormattedMessage.EscapeText(IoCManager.Resolve<IInputManager>().GetKeyFunctionButtonString(keyFunction));
			if (text.Length <= 3)
			{
				return text;
			}
			return null;
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x0000DCEC File Offset: 0x0000BEEC
		public static bool TryGetShortKeyName(BoundKeyFunction keyFunction, [NotNullWhen(true)] out string name)
		{
			IKeyBinding keyBinding;
			if (!IoCManager.Resolve<IInputManager>().TryGetKeyBinding(keyFunction, ref keyBinding))
			{
				name = null;
				return false;
			}
			Keyboard.Key baseKey = keyBinding.BaseKey;
			if (keyBinding.Mod1 != null || keyBinding.Mod2 != null || keyBinding.Mod3 != null)
			{
				name = null;
				return false;
			}
			name = null;
			string text;
			switch (baseKey)
			{
			case 1:
				text = "ML";
				goto IL_465;
			case 2:
				text = "MR";
				goto IL_465;
			case 3:
				text = "MM";
				goto IL_465;
			case 4:
				text = "M4";
				goto IL_465;
			case 5:
				text = "M5";
				goto IL_465;
			case 6:
				text = "M6";
				goto IL_465;
			case 7:
				text = "M7";
				goto IL_465;
			case 8:
				text = "M8";
				goto IL_465;
			case 9:
				text = "M9";
				goto IL_465;
			case 36:
				text = "0";
				goto IL_465;
			case 37:
				text = "1";
				goto IL_465;
			case 38:
				text = "2";
				goto IL_465;
			case 39:
				text = "3";
				goto IL_465;
			case 40:
				text = "4";
				goto IL_465;
			case 41:
				text = "5";
				goto IL_465;
			case 42:
				text = "6";
				goto IL_465;
			case 43:
				text = "7";
				goto IL_465;
			case 44:
				text = "8";
				goto IL_465;
			case 45:
				text = "9";
				goto IL_465;
			case 46:
				text = "0";
				goto IL_465;
			case 47:
				text = "1";
				goto IL_465;
			case 48:
				text = "2";
				goto IL_465;
			case 49:
				text = "3";
				goto IL_465;
			case 50:
				text = "4";
				goto IL_465;
			case 51:
				text = "5";
				goto IL_465;
			case 52:
				text = "6";
				goto IL_465;
			case 53:
				text = "7";
				goto IL_465;
			case 54:
				text = "8";
				goto IL_465;
			case 55:
				text = "9";
				goto IL_465;
			case 56:
				text = "Esc";
				goto IL_465;
			case 62:
				text = "Men";
				goto IL_465;
			case 63:
				text = "[";
				goto IL_465;
			case 64:
				text = "]";
				goto IL_465;
			case 65:
				text = ";";
				goto IL_465;
			case 66:
				text = ",";
				goto IL_465;
			case 67:
				text = ".";
				goto IL_465;
			case 68:
				text = "'";
				goto IL_465;
			case 69:
				text = "/";
				goto IL_465;
			case 70:
				text = "\\";
				goto IL_465;
			case 71:
				text = "~";
				goto IL_465;
			case 72:
				text = "=";
				goto IL_465;
			case 73:
				text = "Spc";
				goto IL_465;
			case 74:
				text = "Ret";
				goto IL_465;
			case 75:
				text = "Ent";
				goto IL_465;
			case 76:
				text = "Bks";
				goto IL_465;
			case 77:
				text = "Tab";
				goto IL_465;
			case 78:
				text = "PgU";
				goto IL_465;
			case 79:
				text = "PgD";
				goto IL_465;
			case 81:
				text = "Hom";
				goto IL_465;
			case 82:
				text = "Ins";
				goto IL_465;
			case 83:
				text = "Del";
				goto IL_465;
			case 84:
				text = "-";
				goto IL_465;
			case 86:
				text = "N-";
				goto IL_465;
			case 87:
				text = "N/";
				goto IL_465;
			case 88:
				text = "*";
				goto IL_465;
			case 89:
				text = "N.";
				goto IL_465;
			case 90:
				text = "Lft";
				goto IL_465;
			case 91:
				text = "Rgt";
				goto IL_465;
			case 93:
				text = "Dwn";
				goto IL_465;
			case 109:
				text = "||";
				goto IL_465;
			}
			text = BoundKeyHelper.DefaultShortKeyName(keyFunction);
			IL_465:
			name = text;
			return name != null;
		}
	}
}
