using AutoMapper;
using Core.Contracts.Models;
using Core.Contracts.Notifications.Models.Emails;
using Core.Models.Notifications;

namespace Core.Mapping;

public class NotificationMappingProfile : Profile
{
    public NotificationMappingProfile()
    {
        CreateMap<EmailNotification, EmailNotificationDto>();

        CreateMap<List<SendEmailResult>, SendEmailNotificationsResponse>()
            .ForMember(x => x.TotalSubscribers, opt => opt.MapFrom(res => res.Select(x => x.Email).Distinct().Count()))
            .ForMember(x => x.SuccessfullyNotified, opt => opt.MapFrom(res => res.Count(x => x.IsSuccessful)))
            .ForMember(x => x.Failed, opt => opt.MapFrom(res => res.Where(x => !x.IsSuccessful)
                .Select(x => new FailedEmailNotificationSummary()
                {
                    EmailAddress = x.Email!, Error = string.Join(", ", x.Errors ?? Array.Empty<string>())
                }).ToList()));
    }
}