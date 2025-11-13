using Cortex.Mediator.Notifications;
using OsitoPolar.EquipmentService.Shared.Domain.Model.Events;

namespace OsitoPolar.EquipmentService.Shared.Application.Internal.EventHandlers;

public interface IEventHandler<in TEvent> : INotificationHandler<TEvent> where TEvent : IEvent
{
    
}