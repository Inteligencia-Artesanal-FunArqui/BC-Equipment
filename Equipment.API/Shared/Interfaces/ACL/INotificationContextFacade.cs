namespace OsitoPolar.EquipmentService.Shared.Interfaces.ACL;

/// <summary>
/// Facade for communicating with Notifications Service
/// Provides methods to send email and in-app notifications
/// </summary>
public interface INotificationContextFacade
{
    /// <summary>
    /// Create an in-app notification for a user
    /// </summary>
    /// <param name="userId">User ID (Owner or Provider profile ID)</param>
    /// <param name="title">Notification title</param>
    /// <param name="message">Notification message</param>
    /// <returns>Task representing the async operation</returns>
    Task CreateInAppNotification(int userId, string title, string message);

    /// <summary>
    /// Send an email notification
    /// </summary>
    /// <param name="to">Recipient email address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="body">Email body</param>
    /// <param name="isHtml">Whether the body is HTML</param>
    /// <returns>Task representing the async operation</returns>
    Task SendEmailNotification(string to, string subject, string body, bool isHtml = false);
}
