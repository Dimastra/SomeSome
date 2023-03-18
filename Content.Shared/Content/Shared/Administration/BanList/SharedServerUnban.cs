using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.BanList
{
	// Token: 0x02000759 RID: 1881
	[NullableContext(2)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class SharedServerUnban : IEquatable<SharedServerUnban>
	{
		// Token: 0x06001714 RID: 5908 RVA: 0x0004AC53 File Offset: 0x00048E53
		public SharedServerUnban(string UnbanningAdmin, DateTime UnbanTime)
		{
			this.UnbanningAdmin = UnbanningAdmin;
			this.UnbanTime = UnbanTime;
			base..ctor();
		}

		// Token: 0x170004DB RID: 1243
		// (get) Token: 0x06001715 RID: 5909 RVA: 0x0004AC69 File Offset: 0x00048E69
		[Nullable(1)]
		[CompilerGenerated]
		private Type EqualityContract
		{
			[NullableContext(1)]
			[CompilerGenerated]
			get
			{
				return typeof(SharedServerUnban);
			}
		}

		// Token: 0x170004DC RID: 1244
		// (get) Token: 0x06001716 RID: 5910 RVA: 0x0004AC75 File Offset: 0x00048E75
		// (set) Token: 0x06001717 RID: 5911 RVA: 0x0004AC7D File Offset: 0x00048E7D
		public string UnbanningAdmin { get; set; }

		// Token: 0x170004DD RID: 1245
		// (get) Token: 0x06001718 RID: 5912 RVA: 0x0004AC86 File Offset: 0x00048E86
		// (set) Token: 0x06001719 RID: 5913 RVA: 0x0004AC8E File Offset: 0x00048E8E
		public DateTime UnbanTime { get; set; }

		// Token: 0x0600171A RID: 5914 RVA: 0x0004AC98 File Offset: 0x00048E98
		[NullableContext(1)]
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SharedServerUnban");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x0600171B RID: 5915 RVA: 0x0004ACE4 File Offset: 0x00048EE4
		[NullableContext(1)]
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			RuntimeHelpers.EnsureSufficientExecutionStack();
			builder.Append("UnbanningAdmin = ");
			builder.Append(this.UnbanningAdmin);
			builder.Append(", UnbanTime = ");
			builder.Append(this.UnbanTime.ToString());
			return true;
		}

		// Token: 0x0600171C RID: 5916 RVA: 0x0004AD37 File Offset: 0x00048F37
		[CompilerGenerated]
		public static bool operator !=(SharedServerUnban left, SharedServerUnban right)
		{
			return !(left == right);
		}

		// Token: 0x0600171D RID: 5917 RVA: 0x0004AD43 File Offset: 0x00048F43
		[CompilerGenerated]
		public static bool operator ==(SharedServerUnban left, SharedServerUnban right)
		{
			return left == right || (left != null && left.Equals(right));
		}

		// Token: 0x0600171E RID: 5918 RVA: 0x0004AD57 File Offset: 0x00048F57
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return (EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<UnbanningAdmin>k__BackingField)) * -1521134295 + EqualityComparer<DateTime>.Default.GetHashCode(this.<UnbanTime>k__BackingField);
		}

		// Token: 0x0600171F RID: 5919 RVA: 0x0004AD97 File Offset: 0x00048F97
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return this.Equals(obj as SharedServerUnban);
		}

		// Token: 0x06001720 RID: 5920 RVA: 0x0004ADA8 File Offset: 0x00048FA8
		[CompilerGenerated]
		public bool Equals(SharedServerUnban other)
		{
			return this == other || (other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<string>.Default.Equals(this.<UnbanningAdmin>k__BackingField, other.<UnbanningAdmin>k__BackingField) && EqualityComparer<DateTime>.Default.Equals(this.<UnbanTime>k__BackingField, other.<UnbanTime>k__BackingField));
		}

		// Token: 0x06001722 RID: 5922 RVA: 0x0004AE09 File Offset: 0x00049009
		[CompilerGenerated]
		private SharedServerUnban([Nullable(1)] SharedServerUnban original)
		{
			this.UnbanningAdmin = original.<UnbanningAdmin>k__BackingField;
			this.UnbanTime = original.<UnbanTime>k__BackingField;
		}

		// Token: 0x06001723 RID: 5923 RVA: 0x0004AE29 File Offset: 0x00049029
		[CompilerGenerated]
		public void Deconstruct(out string UnbanningAdmin, out DateTime UnbanTime)
		{
			UnbanningAdmin = this.UnbanningAdmin;
			UnbanTime = this.UnbanTime;
		}
	}
}
