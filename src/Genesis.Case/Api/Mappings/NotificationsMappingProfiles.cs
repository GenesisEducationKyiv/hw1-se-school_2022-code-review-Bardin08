using Api.Models.Responses;
using AutoMapper;
using Core.Notifications.Emails.Models;

namespace Api.Mappings;

public class NotificationsMappingProfiles : Profile
{
    public NotificationsMappingProfiles()
    {
        CreateMap<SendEmailNotificationsResult, SendEmailsResponse>();
        CreateMap<FailedEmailNotificationSummary, FailedEmailNotificationSummaryResponse>();
    }
}