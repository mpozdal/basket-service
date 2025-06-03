using BasketService.Application.DTOs;

namespace BasketService.Application.Interfaces;

public interface INotificationServiceClient
{
    Task SendEmailNotification(FinalizeBasketNotificationDto  notification);
}