using AutoMapper;
using Core.Abstractions;
using Core.Contracts.Models;
using Core.Models.Notifications;
using Integrations.Notifications.Contracts.Abstractions.Emails;
using Integrations.Notifications.Contracts.Models.Emails;

namespace Core.Services;

internal class EmailService : IEmailService
{
    private readonly IGmailProvider _gmailProvider;
    private readonly IMapper _mapper;

    public EmailService(IGmailProvider gmailProvider, IMapper mapper)
    {
        _gmailProvider = gmailProvider;
        _mapper = mapper;
    }

    public async Task<SendEmailNotificationsResponse> SendEmailsAsync(IEnumerable<EmailNotification> notificationDtos)
    {
        var notifications = notificationDtos.Select(_mapper.Map<EmailNotificationDto>);
        var sendEmailsResults = await _gmailProvider.SendEmailsAsync(notifications);
        return _mapper.Map<SendEmailNotificationsResponse>(sendEmailsResults);
    }
}