using System.Net.Http.Json;
using OsitoPolar.EquipmentService.Shared.Interfaces.ACL;

namespace OsitoPolar.EquipmentService.Application.ACL.Services;

/// <summary>
/// HTTP Facade for communicating with Notifications Service
/// Replaces direct database access with HTTP calls to the Notifications microservice
/// </summary>
public class NotificationsHttpFacade : INotificationContextFacade
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NotificationsHttpFacade> _logger;

    public NotificationsHttpFacade(HttpClient httpClient, ILogger<NotificationsHttpFacade> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task CreateInAppNotification(int userId, string title, string message)
    {
        try
        {
            _logger.LogInformation("Creating in-app notification for user {UserId} via Notifications Service", userId);

            var request = new CreateInAppNotificationRequest(
                UserId: userId,
                Title: title,
                Message: message,
                Type: "Info"
            );

            var response = await _httpClient.PostAsJsonAsync("/api/v1/notifications/in-app", request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to create in-app notification: {Error}", error);
                // Don't throw - notifications are not critical, continue execution
                return;
            }

            _logger.LogInformation("In-app notification created successfully for user {UserId}", userId);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Failed to create in-app notification for user {UserId}", userId);
            // Don't throw - notifications are not critical, continue execution
        }
    }

    public async Task SendEmailNotification(string to, string subject, string body, bool isHtml = false)
    {
        try
        {
            _logger.LogInformation("Sending email to {To} via Notifications Service", to);

            var request = new SendEmailRequest(
                To: to,
                Subject: subject,
                Body: body,
                IsHtml: isHtml
            );

            var response = await _httpClient.PostAsJsonAsync("/api/v1/notifications/emails", request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to send email: {Error}", error);
                // Don't throw - notifications are not critical, continue execution
                return;
            }

            _logger.LogInformation("Email sent successfully to {To}", to);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Failed to send email to {To}", to);
            // Don't throw - notifications are not critical, continue execution
        }
    }
}

// Request DTOs for HTTP communication with Notifications Service
internal record CreateInAppNotificationRequest(
    int UserId,
    string Title,
    string Message,
    string Type
);

internal record SendEmailRequest(
    string To,
    string Subject,
    string Body,
    bool IsHtml
);
