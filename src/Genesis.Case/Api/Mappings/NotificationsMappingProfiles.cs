using Api.Models.Responses;
using AutoMapper;
using Core.Contracts.Models;

namespace Api.Mappings;

public class NotificationsMappingProfiles : Profile
{
    public NotificationsMappingProfiles()
    {
        CreateMap<SendEmailNotificationsResponse, SendEmailsResponse>();
        CreateMap<FailedEmailNotificationSummary, FailedEmailNotificationSummaryResponse>();
    }
}