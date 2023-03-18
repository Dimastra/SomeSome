using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Storage.Components
{
	// Token: 0x02000168 RID: 360
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public class BluespaceLockerBehaviorProperties : IEquatable<BluespaceLockerBehaviorProperties>
	{
		// Token: 0x1700012D RID: 301
		// (get) Token: 0x06000710 RID: 1808 RVA: 0x000238D4 File Offset: 0x00021AD4
		[CompilerGenerated]
		protected virtual Type EqualityContract
		{
			[CompilerGenerated]
			get
			{
				return typeof(BluespaceLockerBehaviorProperties);
			}
		}

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x06000711 RID: 1809 RVA: 0x000238E0 File Offset: 0x00021AE0
		// (set) Token: 0x06000712 RID: 1810 RVA: 0x000238E8 File Offset: 0x00021AE8
		[DataField("transportGas", false, 1, false, false, null)]
		[ViewVariables]
		public bool TransportGas { get; set; }

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x06000713 RID: 1811 RVA: 0x000238F1 File Offset: 0x00021AF1
		// (set) Token: 0x06000714 RID: 1812 RVA: 0x000238F9 File Offset: 0x00021AF9
		[DataField("transportEntities", false, 1, false, false, null)]
		[ViewVariables]
		public bool TransportEntities { get; set; }

		// Token: 0x17000130 RID: 304
		// (get) Token: 0x06000715 RID: 1813 RVA: 0x00023902 File Offset: 0x00021B02
		// (set) Token: 0x06000716 RID: 1814 RVA: 0x0002390A File Offset: 0x00021B0A
		[DataField("transportSentient", false, 1, false, false, null)]
		[ViewVariables]
		public bool TransportSentient { get; set; }

		// Token: 0x17000131 RID: 305
		// (get) Token: 0x06000717 RID: 1815 RVA: 0x00023913 File Offset: 0x00021B13
		// (set) Token: 0x06000718 RID: 1816 RVA: 0x0002391B File Offset: 0x00021B1B
		[DataField("actOnOpen", false, 1, false, false, null)]
		[ViewVariables]
		public bool ActOnOpen { get; set; }

		// Token: 0x17000132 RID: 306
		// (get) Token: 0x06000719 RID: 1817 RVA: 0x00023924 File Offset: 0x00021B24
		// (set) Token: 0x0600071A RID: 1818 RVA: 0x0002392C File Offset: 0x00021B2C
		[DataField("actOnClose", false, 1, false, false, null)]
		[ViewVariables]
		public bool ActOnClose { get; set; }

		// Token: 0x17000133 RID: 307
		// (get) Token: 0x0600071B RID: 1819 RVA: 0x00023935 File Offset: 0x00021B35
		// (set) Token: 0x0600071C RID: 1820 RVA: 0x0002393D File Offset: 0x00021B3D
		[DataField("delay", false, 1, false, false, null)]
		[ViewVariables]
		public float Delay { get; set; }

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x0600071D RID: 1821 RVA: 0x00023946 File Offset: 0x00021B46
		// (set) Token: 0x0600071E RID: 1822 RVA: 0x0002394E File Offset: 0x00021B4E
		[DataField("bluespaceEffectPrototype", false, 1, false, false, null)]
		[ViewVariables]
		public string BluespaceEffectPrototype { get; set; }

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x0600071F RID: 1823 RVA: 0x00023957 File Offset: 0x00021B57
		// (set) Token: 0x06000720 RID: 1824 RVA: 0x0002395F File Offset: 0x00021B5F
		[DataField("bluespaceEffectOnTeleportSource", false, 1, false, false, null)]
		[ViewVariables]
		public bool BluespaceEffectOnTeleportSource { get; set; }

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x06000721 RID: 1825 RVA: 0x00023968 File Offset: 0x00021B68
		// (set) Token: 0x06000722 RID: 1826 RVA: 0x00023970 File Offset: 0x00021B70
		[DataField("bluespaceEffectOnTeleportTarget", false, 1, false, false, null)]
		[ViewVariables]
		public bool BluespaceEffectOnTeleportTarget { get; set; }

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x06000723 RID: 1827 RVA: 0x00023979 File Offset: 0x00021B79
		// (set) Token: 0x06000724 RID: 1828 RVA: 0x00023981 File Offset: 0x00021B81
		[DataField("bluespaceEffectMinInterval", false, 1, false, false, null)]
		[ViewVariables]
		public double BluespaceEffectMinInterval { get; set; }

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x06000725 RID: 1829 RVA: 0x0002398A File Offset: 0x00021B8A
		// (set) Token: 0x06000726 RID: 1830 RVA: 0x00023992 File Offset: 0x00021B92
		[DataField("destroyAfterUses", false, 1, false, false, null)]
		[ViewVariables]
		public int DestroyAfterUses { get; set; }

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x06000727 RID: 1831 RVA: 0x0002399B File Offset: 0x00021B9B
		// (set) Token: 0x06000728 RID: 1832 RVA: 0x000239A3 File Offset: 0x00021BA3
		[DataField("destroyAfterUsesMinItemsToCountUse", false, 1, false, false, null)]
		[ViewVariables]
		public int DestroyAfterUsesMinItemsToCountUse { get; set; }

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x06000729 RID: 1833 RVA: 0x000239AC File Offset: 0x00021BAC
		// (set) Token: 0x0600072A RID: 1834 RVA: 0x000239B4 File Offset: 0x00021BB4
		[DataField("destroyType", false, 1, false, false, null)]
		[ViewVariables]
		public BluespaceLockerDestroyType DestroyType { get; set; }

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x0600072B RID: 1835 RVA: 0x000239BD File Offset: 0x00021BBD
		// (set) Token: 0x0600072C RID: 1836 RVA: 0x000239C5 File Offset: 0x00021BC5
		[DataField("clearLinksEvery", false, 1, false, false, null)]
		[ViewVariables]
		public int ClearLinksEvery { get; set; }

		// Token: 0x1700013C RID: 316
		// (get) Token: 0x0600072D RID: 1837 RVA: 0x000239CE File Offset: 0x00021BCE
		// (set) Token: 0x0600072E RID: 1838 RVA: 0x000239D6 File Offset: 0x00021BD6
		[DataField("clearLinksDebluespaces", false, 1, false, false, null)]
		[ViewVariables]
		public bool ClearLinksDebluespaces { get; set; }

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x0600072F RID: 1839 RVA: 0x000239DF File Offset: 0x00021BDF
		// (set) Token: 0x06000730 RID: 1840 RVA: 0x000239E7 File Offset: 0x00021BE7
		[DataField("invalidateOneWayLinks", false, 1, false, false, null)]
		[ViewVariables]
		public bool InvalidateOneWayLinks { get; set; }

		// Token: 0x06000731 RID: 1841 RVA: 0x000239F0 File Offset: 0x00021BF0
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("BluespaceLockerBehaviorProperties");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000732 RID: 1842 RVA: 0x00023A3C File Offset: 0x00021C3C
		[CompilerGenerated]
		protected virtual bool PrintMembers(StringBuilder builder)
		{
			RuntimeHelpers.EnsureSufficientExecutionStack();
			builder.Append("TransportGas = ");
			builder.Append(this.TransportGas.ToString());
			builder.Append(", TransportEntities = ");
			builder.Append(this.TransportEntities.ToString());
			builder.Append(", TransportSentient = ");
			builder.Append(this.TransportSentient.ToString());
			builder.Append(", ActOnOpen = ");
			builder.Append(this.ActOnOpen.ToString());
			builder.Append(", ActOnClose = ");
			builder.Append(this.ActOnClose.ToString());
			builder.Append(", Delay = ");
			builder.Append(this.Delay.ToString());
			builder.Append(", BluespaceEffectOnInit = ");
			builder.Append(this.BluespaceEffectOnInit.ToString());
			builder.Append(", BluespaceEffectPrototype = ");
			builder.Append(this.BluespaceEffectPrototype);
			builder.Append(", BluespaceEffectOnTeleportSource = ");
			builder.Append(this.BluespaceEffectOnTeleportSource.ToString());
			builder.Append(", BluespaceEffectOnTeleportTarget = ");
			builder.Append(this.BluespaceEffectOnTeleportTarget.ToString());
			builder.Append(", BluespaceEffectMinInterval = ");
			builder.Append(this.BluespaceEffectMinInterval.ToString());
			builder.Append(", DestroyAfterUses = ");
			builder.Append(this.DestroyAfterUses.ToString());
			builder.Append(", DestroyAfterUsesMinItemsToCountUse = ");
			builder.Append(this.DestroyAfterUsesMinItemsToCountUse.ToString());
			builder.Append(", DestroyType = ");
			builder.Append(this.DestroyType.ToString());
			builder.Append(", ClearLinksEvery = ");
			builder.Append(this.ClearLinksEvery.ToString());
			builder.Append(", ClearLinksDebluespaces = ");
			builder.Append(this.ClearLinksDebluespaces.ToString());
			builder.Append(", InvalidateOneWayLinks = ");
			builder.Append(this.InvalidateOneWayLinks.ToString());
			return true;
		}

		// Token: 0x06000733 RID: 1843 RVA: 0x00023CD6 File Offset: 0x00021ED6
		[NullableContext(2)]
		[CompilerGenerated]
		public static bool operator !=(BluespaceLockerBehaviorProperties left, BluespaceLockerBehaviorProperties right)
		{
			return !(left == right);
		}

		// Token: 0x06000734 RID: 1844 RVA: 0x00023CE2 File Offset: 0x00021EE2
		[NullableContext(2)]
		[CompilerGenerated]
		public static bool operator ==(BluespaceLockerBehaviorProperties left, BluespaceLockerBehaviorProperties right)
		{
			return left == right || (left != null && left.Equals(right));
		}

		// Token: 0x06000735 RID: 1845 RVA: 0x00023CF8 File Offset: 0x00021EF8
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return ((((((((((((((((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<TransportGas>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<TransportEntities>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<TransportSentient>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<ActOnOpen>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<ActOnClose>k__BackingField)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.<Delay>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.BluespaceEffectOnInit)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<BluespaceEffectPrototype>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<BluespaceEffectOnTeleportSource>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<BluespaceEffectOnTeleportTarget>k__BackingField)) * -1521134295 + EqualityComparer<double>.Default.GetHashCode(this.<BluespaceEffectMinInterval>k__BackingField)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.<DestroyAfterUses>k__BackingField)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.<DestroyAfterUsesMinItemsToCountUse>k__BackingField)) * -1521134295 + EqualityComparer<BluespaceLockerDestroyType>.Default.GetHashCode(this.<DestroyType>k__BackingField)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.<ClearLinksEvery>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<ClearLinksDebluespaces>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<InvalidateOneWayLinks>k__BackingField);
		}

		// Token: 0x06000736 RID: 1846 RVA: 0x00023E9C File Offset: 0x0002209C
		[NullableContext(2)]
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return this.Equals(obj as BluespaceLockerBehaviorProperties);
		}

		// Token: 0x06000737 RID: 1847 RVA: 0x00023EAC File Offset: 0x000220AC
		[NullableContext(2)]
		[CompilerGenerated]
		public virtual bool Equals(BluespaceLockerBehaviorProperties other)
		{
			return this == other || (other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<bool>.Default.Equals(this.<TransportGas>k__BackingField, other.<TransportGas>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<TransportEntities>k__BackingField, other.<TransportEntities>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<TransportSentient>k__BackingField, other.<TransportSentient>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<ActOnOpen>k__BackingField, other.<ActOnOpen>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<ActOnClose>k__BackingField, other.<ActOnClose>k__BackingField) && EqualityComparer<float>.Default.Equals(this.<Delay>k__BackingField, other.<Delay>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.BluespaceEffectOnInit, other.BluespaceEffectOnInit) && EqualityComparer<string>.Default.Equals(this.<BluespaceEffectPrototype>k__BackingField, other.<BluespaceEffectPrototype>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<BluespaceEffectOnTeleportSource>k__BackingField, other.<BluespaceEffectOnTeleportSource>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<BluespaceEffectOnTeleportTarget>k__BackingField, other.<BluespaceEffectOnTeleportTarget>k__BackingField) && EqualityComparer<double>.Default.Equals(this.<BluespaceEffectMinInterval>k__BackingField, other.<BluespaceEffectMinInterval>k__BackingField) && EqualityComparer<int>.Default.Equals(this.<DestroyAfterUses>k__BackingField, other.<DestroyAfterUses>k__BackingField) && EqualityComparer<int>.Default.Equals(this.<DestroyAfterUsesMinItemsToCountUse>k__BackingField, other.<DestroyAfterUsesMinItemsToCountUse>k__BackingField) && EqualityComparer<BluespaceLockerDestroyType>.Default.Equals(this.<DestroyType>k__BackingField, other.<DestroyType>k__BackingField) && EqualityComparer<int>.Default.Equals(this.<ClearLinksEvery>k__BackingField, other.<ClearLinksEvery>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<ClearLinksDebluespaces>k__BackingField, other.<ClearLinksDebluespaces>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<InvalidateOneWayLinks>k__BackingField, other.<InvalidateOneWayLinks>k__BackingField));
		}

		// Token: 0x06000739 RID: 1849 RVA: 0x000240A0 File Offset: 0x000222A0
		[CompilerGenerated]
		protected BluespaceLockerBehaviorProperties(BluespaceLockerBehaviorProperties original)
		{
			this.TransportGas = original.<TransportGas>k__BackingField;
			this.TransportEntities = original.<TransportEntities>k__BackingField;
			this.TransportSentient = original.<TransportSentient>k__BackingField;
			this.ActOnOpen = original.<ActOnOpen>k__BackingField;
			this.ActOnClose = original.<ActOnClose>k__BackingField;
			this.Delay = original.<Delay>k__BackingField;
			this.BluespaceEffectOnInit = original.BluespaceEffectOnInit;
			this.BluespaceEffectPrototype = original.<BluespaceEffectPrototype>k__BackingField;
			this.BluespaceEffectOnTeleportSource = original.<BluespaceEffectOnTeleportSource>k__BackingField;
			this.BluespaceEffectOnTeleportTarget = original.<BluespaceEffectOnTeleportTarget>k__BackingField;
			this.BluespaceEffectMinInterval = original.<BluespaceEffectMinInterval>k__BackingField;
			this.DestroyAfterUses = original.<DestroyAfterUses>k__BackingField;
			this.DestroyAfterUsesMinItemsToCountUse = original.<DestroyAfterUsesMinItemsToCountUse>k__BackingField;
			this.DestroyType = original.<DestroyType>k__BackingField;
			this.ClearLinksEvery = original.<ClearLinksEvery>k__BackingField;
			this.ClearLinksDebluespaces = original.<ClearLinksDebluespaces>k__BackingField;
			this.InvalidateOneWayLinks = original.<InvalidateOneWayLinks>k__BackingField;
		}

		// Token: 0x0600073A RID: 1850 RVA: 0x00024180 File Offset: 0x00022380
		public BluespaceLockerBehaviorProperties()
		{
			this.TransportGas = true;
			this.TransportEntities = true;
			this.TransportSentient = true;
			this.ActOnOpen = true;
			this.ActOnClose = true;
			this.BluespaceEffectPrototype = "EffectFlashBluespace";
			this.BluespaceEffectMinInterval = 2.0;
			this.DestroyAfterUses = -1;
			this.ClearLinksEvery = -1;
			base..ctor();
		}

		// Token: 0x0400041D RID: 1053
		[DataField("bluespaceEffectOnInit", false, 1, false, false, null)]
		[ViewVariables]
		public bool BluespaceEffectOnInit;
	}
}
