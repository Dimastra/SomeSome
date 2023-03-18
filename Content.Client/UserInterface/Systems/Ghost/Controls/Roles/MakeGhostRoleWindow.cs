﻿using System;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.UserInterface.Systems.Ghost.Controls.Roles
{
	// Token: 0x02000093 RID: 147
	[GenerateTypedNameReferences]
	public sealed class MakeGhostRoleWindow : DefaultWindow
	{
		// Token: 0x06000367 RID: 871 RVA: 0x00014BE4 File Offset: 0x00012DE4
		public MakeGhostRoleWindow()
		{
			MakeGhostRoleWindow.!XamlIlPopulateTrampoline(this);
			this.MakeSentientLabel.MinSize = new ValueTuple<float, float>(150f, 0f);
			this.RoleEntityLabel.MinSize = new ValueTuple<float, float>(150f, 0f);
			this.RoleNameLabel.MinSize = new ValueTuple<float, float>(150f, 0f);
			this.RoleName.MinSize = new ValueTuple<float, float>(300f, 0f);
			this.RoleDescriptionLabel.MinSize = new ValueTuple<float, float>(150f, 0f);
			this.RoleDescription.MinSize = new ValueTuple<float, float>(300f, 0f);
			this.RoleRulesLabel.MinSize = new ValueTuple<float, float>(150f, 0f);
			this.RoleRules.MinSize = new ValueTuple<float, float>(300f, 0f);
			this.MakeButton.OnPressed += this.OnPressed;
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x06000368 RID: 872 RVA: 0x00014D0C File Offset: 0x00012F0C
		// (set) Token: 0x06000369 RID: 873 RVA: 0x00014D14 File Offset: 0x00012F14
		private EntityUid? EntityUid { get; set; }

		// Token: 0x14000017 RID: 23
		// (add) Token: 0x0600036A RID: 874 RVA: 0x00014D20 File Offset: 0x00012F20
		// (remove) Token: 0x0600036B RID: 875 RVA: 0x00014D58 File Offset: 0x00012F58
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event MakeGhostRoleWindow.MakeRole OnMake;

		// Token: 0x0600036C RID: 876 RVA: 0x00014D90 File Offset: 0x00012F90
		public void SetEntity(EntityUid uid)
		{
			this.EntityUid = new EntityUid?(uid);
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			this.RoleName.Text = entityManager.GetComponent<MetaDataComponent>(uid).EntityName;
			Label roleEntity = this.RoleEntity;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(uid);
			roleEntity.Text = defaultInterpolatedStringHandler.ToStringAndClear();
		}

		// Token: 0x0600036D RID: 877 RVA: 0x00014DEC File Offset: 0x00012FEC
		[NullableContext(1)]
		private void OnPressed(BaseButton.ButtonEventArgs args)
		{
			if (this.EntityUid == null)
			{
				return;
			}
			MakeGhostRoleWindow.MakeRole onMake = this.OnMake;
			if (onMake == null)
			{
				return;
			}
			onMake(this.EntityUid.Value, this.RoleName.Text, this.RoleDescription.Text, this.RoleRules.Text, this.MakeSentientCheckbox.Pressed);
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x0600036E RID: 878 RVA: 0x00014E54 File Offset: 0x00013054
		private Label RoleEntityLabel
		{
			get
			{
				return base.FindControl<Label>("RoleEntityLabel");
			}
		}

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x0600036F RID: 879 RVA: 0x00014E61 File Offset: 0x00013061
		private Label RoleEntity
		{
			get
			{
				return base.FindControl<Label>("RoleEntity");
			}
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x06000370 RID: 880 RVA: 0x00014E6E File Offset: 0x0001306E
		private Label RoleNameLabel
		{
			get
			{
				return base.FindControl<Label>("RoleNameLabel");
			}
		}

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x06000371 RID: 881 RVA: 0x00014E7B File Offset: 0x0001307B
		private LineEdit RoleName
		{
			get
			{
				return base.FindControl<LineEdit>("RoleName");
			}
		}

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x06000372 RID: 882 RVA: 0x00014E88 File Offset: 0x00013088
		private Label RoleDescriptionLabel
		{
			get
			{
				return base.FindControl<Label>("RoleDescriptionLabel");
			}
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x06000373 RID: 883 RVA: 0x00014E95 File Offset: 0x00013095
		private LineEdit RoleDescription
		{
			get
			{
				return base.FindControl<LineEdit>("RoleDescription");
			}
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x06000374 RID: 884 RVA: 0x00014EA2 File Offset: 0x000130A2
		private Label RoleRulesLabel
		{
			get
			{
				return base.FindControl<Label>("RoleRulesLabel");
			}
		}

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x06000375 RID: 885 RVA: 0x00014EAF File Offset: 0x000130AF
		private LineEdit RoleRules
		{
			get
			{
				return base.FindControl<LineEdit>("RoleRules");
			}
		}

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x06000376 RID: 886 RVA: 0x00014EBC File Offset: 0x000130BC
		private Label MakeSentientLabel
		{
			get
			{
				return base.FindControl<Label>("MakeSentientLabel");
			}
		}

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x06000377 RID: 887 RVA: 0x00014EC9 File Offset: 0x000130C9
		private CheckBox MakeSentientCheckbox
		{
			get
			{
				return base.FindControl<CheckBox>("MakeSentientCheckbox");
			}
		}

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x06000378 RID: 888 RVA: 0x00014ED6 File Offset: 0x000130D6
		private Button MakeButton
		{
			get
			{
				return base.FindControl<Button>("MakeButton");
			}
		}

		// Token: 0x06000379 RID: 889 RVA: 0x00014EE4 File Offset: 0x000130E4
		static void xaml(IServiceProvider A_0, DefaultWindow A_1)
		{
			XamlIlContext.Context<DefaultWindow> context = new XamlIlContext.Context<DefaultWindow>(A_0, null, "resm:Content.Client.UserInterface.Systems.Ghost.Controls.Roles.MakeGhostRoleWindow.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Title = (string)new LocExtension("Make Ghost Role").ProvideValue();
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			BoxContainer boxContainer2 = new BoxContainer();
			boxContainer2.Orientation = 0;
			Label label = new Label();
			label.Name = "RoleEntityLabel";
			Control control = label;
			context.RobustNameScope.Register("RoleEntityLabel", control);
			label.Text = "Entity";
			control = label;
			boxContainer2.XamlChildren.Add(control);
			Label label2 = new Label();
			label2.Name = "RoleEntity";
			control = label2;
			context.RobustNameScope.Register("RoleEntity", control);
			label2.Text = "";
			control = label2;
			boxContainer2.XamlChildren.Add(control);
			control = boxContainer2;
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer3 = new BoxContainer();
			boxContainer3.Orientation = 0;
			Label label3 = new Label();
			label3.Name = "RoleNameLabel";
			control = label3;
			context.RobustNameScope.Register("RoleNameLabel", control);
			label3.Text = "Role Name";
			control = label3;
			boxContainer3.XamlChildren.Add(control);
			LineEdit lineEdit = new LineEdit();
			lineEdit.Name = "RoleName";
			control = lineEdit;
			context.RobustNameScope.Register("RoleName", control);
			lineEdit.HorizontalExpand = true;
			control = lineEdit;
			boxContainer3.XamlChildren.Add(control);
			control = boxContainer3;
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer4 = new BoxContainer();
			boxContainer4.Orientation = 0;
			Label label4 = new Label();
			label4.Name = "RoleDescriptionLabel";
			control = label4;
			context.RobustNameScope.Register("RoleDescriptionLabel", control);
			label4.Text = "Role Description";
			control = label4;
			boxContainer4.XamlChildren.Add(control);
			LineEdit lineEdit2 = new LineEdit();
			lineEdit2.Name = "RoleDescription";
			control = lineEdit2;
			context.RobustNameScope.Register("RoleDescription", control);
			lineEdit2.HorizontalExpand = true;
			control = lineEdit2;
			boxContainer4.XamlChildren.Add(control);
			control = boxContainer4;
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer5 = new BoxContainer();
			boxContainer5.Orientation = 0;
			Label label5 = new Label();
			label5.Name = "RoleRulesLabel";
			control = label5;
			context.RobustNameScope.Register("RoleRulesLabel", control);
			label5.Text = "Role Rules";
			control = label5;
			boxContainer5.XamlChildren.Add(control);
			LineEdit lineEdit3 = new LineEdit();
			lineEdit3.Name = "RoleRules";
			control = lineEdit3;
			context.RobustNameScope.Register("RoleRules", control);
			lineEdit3.HorizontalExpand = true;
			lineEdit3.Text = (string)new LocExtension("ghost-role-component-default-rules").ProvideValue();
			control = lineEdit3;
			boxContainer5.XamlChildren.Add(control);
			control = boxContainer5;
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer6 = new BoxContainer();
			boxContainer6.Orientation = 0;
			Label label6 = new Label();
			label6.Name = "MakeSentientLabel";
			control = label6;
			context.RobustNameScope.Register("MakeSentientLabel", control);
			label6.Text = "Make Sentient";
			control = label6;
			boxContainer6.XamlChildren.Add(control);
			CheckBox checkBox = new CheckBox();
			checkBox.Name = "MakeSentientCheckbox";
			control = checkBox;
			context.RobustNameScope.Register("MakeSentientCheckbox", control);
			control = checkBox;
			boxContainer6.XamlChildren.Add(control);
			control = boxContainer6;
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer7 = new BoxContainer();
			boxContainer7.Orientation = 0;
			Button button = new Button();
			button.Name = "MakeButton";
			control = button;
			context.RobustNameScope.Register("MakeButton", control);
			button.Text = "Make";
			control = button;
			boxContainer7.XamlChildren.Add(control);
			control = boxContainer7;
			boxContainer.XamlChildren.Add(control);
			control = boxContainer;
			A_1.XamlChildren.Add(control);
			if ((control = (A_1 as Control)) != null)
			{
				context.RobustNameScope.Absorb(control.NameScope);
				control.NameScope = context.RobustNameScope;
			}
			context.RobustNameScope.Complete();
		}

		// Token: 0x0600037A RID: 890 RVA: 0x000153AC File Offset: 0x000135AC
		private static void !XamlIlPopulateTrampoline(MakeGhostRoleWindow A_0)
		{
			MakeGhostRoleWindow.Populate:Content.Client.UserInterface.Systems.Ghost.Controls.Roles.MakeGhostRoleWindow.xaml(null, A_0);
		}

		// Token: 0x02000094 RID: 148
		// (Invoke) Token: 0x0600037C RID: 892
		public delegate void MakeRole(EntityUid uid, string name, string description, string rules, bool makeSentient);
	}
}
