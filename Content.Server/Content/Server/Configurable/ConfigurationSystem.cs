using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Configurable;
using Content.Shared.Interaction;
using Content.Shared.Tools.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Configurable
{
	// Token: 0x0200062E RID: 1582
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ConfigurationSystem : EntitySystem
	{
		// Token: 0x060021AF RID: 8623 RVA: 0x000AF79C File Offset: 0x000AD99C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ConfigurationComponent, ConfigurationComponent.ConfigurationUpdatedMessage>(new ComponentEventHandler<ConfigurationComponent, ConfigurationComponent.ConfigurationUpdatedMessage>(this.OnUpdate), null, null);
			base.SubscribeLocalEvent<ConfigurationComponent, ComponentStartup>(new ComponentEventHandler<ConfigurationComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<ConfigurationComponent, InteractUsingEvent>(new ComponentEventHandler<ConfigurationComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
			base.SubscribeLocalEvent<ConfigurationComponent, ContainerIsInsertingAttemptEvent>(new ComponentEventHandler<ConfigurationComponent, ContainerIsInsertingAttemptEvent>(this.OnInsert), null, null);
		}

		// Token: 0x060021B0 RID: 8624 RVA: 0x000AF800 File Offset: 0x000ADA00
		private void OnInteractUsing(EntityUid uid, ConfigurationComponent component, InteractUsingEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			ToolComponent tool;
			if (!base.TryComp<ToolComponent>(args.Used, ref tool) || !tool.Qualities.Contains(component.QualityNeeded))
			{
				return;
			}
			ActorComponent actor;
			if (!base.TryComp<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			args.Handled = this._uiSystem.TryOpen(uid, ConfigurationComponent.ConfigurationUiKey.Key, actor.PlayerSession, null);
		}

		// Token: 0x060021B1 RID: 8625 RVA: 0x000AF86A File Offset: 0x000ADA6A
		private void OnStartup(EntityUid uid, ConfigurationComponent component, ComponentStartup args)
		{
			this.UpdateUi(uid, component);
		}

		// Token: 0x060021B2 RID: 8626 RVA: 0x000AF874 File Offset: 0x000ADA74
		private void UpdateUi(EntityUid uid, ConfigurationComponent component)
		{
			BoundUserInterface ui;
			if (this._uiSystem.TryGetUi(uid, ConfigurationComponent.ConfigurationUiKey.Key, ref ui, null))
			{
				ui.SetState(new ConfigurationComponent.ConfigurationBoundUserInterfaceState(component.Config), null, true);
			}
		}

		// Token: 0x060021B3 RID: 8627 RVA: 0x000AF8AC File Offset: 0x000ADAAC
		private void OnUpdate(EntityUid uid, ConfigurationComponent component, ConfigurationComponent.ConfigurationUpdatedMessage args)
		{
			foreach (string key in component.Config.Keys)
			{
				string value = args.Config.GetValueOrDefault(key);
				if (!string.IsNullOrWhiteSpace(value) && (component.Validation == null || component.Validation.IsMatch(value)))
				{
					component.Config[key] = value;
				}
			}
			this.UpdateUi(uid, component);
			ConfigurationSystem.ConfigurationUpdatedEvent updatedEvent = new ConfigurationSystem.ConfigurationUpdatedEvent(component);
			base.RaiseLocalEvent<ConfigurationSystem.ConfigurationUpdatedEvent>(uid, updatedEvent, false);
		}

		// Token: 0x060021B4 RID: 8628 RVA: 0x000AF950 File Offset: 0x000ADB50
		private void OnInsert(EntityUid uid, ConfigurationComponent component, ContainerIsInsertingAttemptEvent args)
		{
			ToolComponent tool;
			if (!base.TryComp<ToolComponent>(args.EntityUid, ref tool) || !tool.Qualities.Contains(component.QualityNeeded))
			{
				return;
			}
			args.Cancel();
		}

		// Token: 0x040014A2 RID: 5282
		[Dependency]
		private readonly UserInterfaceSystem _uiSystem;

		// Token: 0x02000AE8 RID: 2792
		[Nullable(0)]
		public sealed class ConfigurationUpdatedEvent : EntityEventArgs
		{
			// Token: 0x0600366A RID: 13930 RVA: 0x001215EA File Offset: 0x0011F7EA
			public ConfigurationUpdatedEvent(ConfigurationComponent configuration)
			{
				this.Configuration = configuration;
			}

			// Token: 0x0400287A RID: 10362
			public ConfigurationComponent Configuration;
		}
	}
}
