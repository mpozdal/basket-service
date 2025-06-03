using System.Text;
using System.Text.Json;
using BasketService.Application.DTOs;
using BasketService.Application.Interfaces;

namespace BasketService.Infrastructure.NotificationService;

public class NotificationServiceClient: INotificationServiceClient
{
    private readonly HttpClient _httpClient;

    public NotificationServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task SendEmailNotification(FinalizeBasketNotificationDto notification)
    {
        var data = new CreateNotificationDto()
        {
            Channel = "email",
            Recipient = notification.Recipient,
            Message = notification.Message,
            HighPriority = true,
            ForceSend = true,
            ScheduledAt = DateTime.UtcNow,
            TimeZone = "Warsaw/Europe"
        };
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("http://localhost:5001/api/notification", content);
        if(!response.IsSuccessStatusCode)
            throw new Exception("Notification Failed");
        Console.WriteLine("Notification Send Success");
    }
}