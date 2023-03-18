using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Shared.Wires
{
	// Token: 0x0200002A RID: 42
	[NullableContext(1)]
	[Nullable(0)]
	public static class HackingWiresExt
	{
		// Token: 0x06000036 RID: 54 RVA: 0x000025E4 File Offset: 0x000007E4
		public static string Name(this WireColor color)
		{
			string text;
			switch (color)
			{
			case WireColor.Red:
				text = "Red";
				break;
			case WireColor.Blue:
				text = "Blue";
				break;
			case WireColor.Green:
				text = "Green";
				break;
			case WireColor.Orange:
				text = "Orange";
				break;
			case WireColor.Brown:
				text = "Brown";
				break;
			case WireColor.Gold:
				text = "Gold";
				break;
			case WireColor.Gray:
				text = "Gray";
				break;
			case WireColor.Cyan:
				text = "Cyan";
				break;
			case WireColor.Navy:
				text = "Navy";
				break;
			case WireColor.Purple:
				text = "Purple";
				break;
			case WireColor.Pink:
				text = "Pink";
				break;
			case WireColor.Fuchsia:
				text = "Fuchsia";
				break;
			default:
				throw new InvalidOperationException();
			}
			return Loc.GetString(text);
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00002698 File Offset: 0x00000898
		public static Color ColorValue(this WireColor color)
		{
			Color result;
			switch (color)
			{
			case WireColor.Red:
				result = Color.Red;
				break;
			case WireColor.Blue:
				result = Color.Blue;
				break;
			case WireColor.Green:
				result = Color.LimeGreen;
				break;
			case WireColor.Orange:
				result = Color.Orange;
				break;
			case WireColor.Brown:
				result = Color.Brown;
				break;
			case WireColor.Gold:
				result = Color.Gold;
				break;
			case WireColor.Gray:
				result = Color.Gray;
				break;
			case WireColor.Cyan:
				result = Color.Cyan;
				break;
			case WireColor.Navy:
				result = Color.Navy;
				break;
			case WireColor.Purple:
				result = Color.Purple;
				break;
			case WireColor.Pink:
				result = Color.Pink;
				break;
			case WireColor.Fuchsia:
				result = Color.Fuchsia;
				break;
			default:
				throw new InvalidOperationException();
			}
			return result;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00002744 File Offset: 0x00000944
		public static string Name(this WireLetter letter)
		{
			string text;
			switch (letter)
			{
			case WireLetter.α:
				text = "Alpha";
				break;
			case WireLetter.β:
				text = "Beta";
				break;
			case WireLetter.γ:
				text = "Gamma";
				break;
			case WireLetter.δ:
				text = "Delta";
				break;
			case WireLetter.ε:
				text = "Epsilon";
				break;
			case WireLetter.ζ:
				text = "Zeta";
				break;
			case WireLetter.η:
				text = "Eta";
				break;
			case WireLetter.θ:
				text = "Theta";
				break;
			case WireLetter.ι:
				text = "Iota";
				break;
			case WireLetter.κ:
				text = "Kappa";
				break;
			case WireLetter.λ:
				text = "Lambda";
				break;
			case WireLetter.μ:
				text = "Mu";
				break;
			case WireLetter.ν:
				text = "Nu";
				break;
			case WireLetter.ξ:
				text = "Xi";
				break;
			case WireLetter.ο:
				text = "Omicron";
				break;
			case WireLetter.π:
				text = "Pi";
				break;
			case WireLetter.ρ:
				text = "Rho";
				break;
			case WireLetter.σ:
				text = "Sigma";
				break;
			case WireLetter.τ:
				text = "Tau";
				break;
			case WireLetter.υ:
				text = "Upsilon";
				break;
			case WireLetter.φ:
				text = "Phi";
				break;
			case WireLetter.χ:
				text = "Chi";
				break;
			case WireLetter.ψ:
				text = "Psi";
				break;
			case WireLetter.ω:
				text = "Omega";
				break;
			default:
				throw new InvalidOperationException();
			}
			return Loc.GetString(text);
		}

		// Token: 0x06000039 RID: 57 RVA: 0x000028A0 File Offset: 0x00000AA0
		public static char Letter(this WireLetter letter)
		{
			char result;
			switch (letter)
			{
			case WireLetter.α:
				result = 'α';
				break;
			case WireLetter.β:
				result = 'β';
				break;
			case WireLetter.γ:
				result = 'γ';
				break;
			case WireLetter.δ:
				result = 'δ';
				break;
			case WireLetter.ε:
				result = 'ε';
				break;
			case WireLetter.ζ:
				result = 'ζ';
				break;
			case WireLetter.η:
				result = 'η';
				break;
			case WireLetter.θ:
				result = 'θ';
				break;
			case WireLetter.ι:
				result = 'ι';
				break;
			case WireLetter.κ:
				result = 'κ';
				break;
			case WireLetter.λ:
				result = 'λ';
				break;
			case WireLetter.μ:
				result = 'μ';
				break;
			case WireLetter.ν:
				result = 'ν';
				break;
			case WireLetter.ξ:
				result = 'ξ';
				break;
			case WireLetter.ο:
				result = 'ο';
				break;
			case WireLetter.π:
				result = 'π';
				break;
			case WireLetter.ρ:
				result = 'ρ';
				break;
			case WireLetter.σ:
				result = 'σ';
				break;
			case WireLetter.τ:
				result = 'τ';
				break;
			case WireLetter.υ:
				result = 'υ';
				break;
			case WireLetter.φ:
				result = 'φ';
				break;
			case WireLetter.χ:
				result = 'χ';
				break;
			case WireLetter.ψ:
				result = 'ψ';
				break;
			case WireLetter.ω:
				result = 'ω';
				break;
			default:
				throw new InvalidOperationException();
			}
			return result;
		}
	}
}
