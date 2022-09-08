using API.Services.Interfaces;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace API.Services.Implements
{

    public class MailJetSettings
    {
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
    }
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public MailJetSettings _mailJetSettings { get; set; }

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(email, subject, htmlMessage);
        }

        public async Task Execute(string email, string subject, string body)
        {
            _mailJetSettings = _configuration.GetSection("MailJet").Get<MailJetSettings>();

            MailjetClient client = new MailjetClient(_mailJetSettings.ApiKey, _mailJetSettings.SecretKey)
            {
                Version = ApiVersion.V3_1,
            };
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
            .Property(Send.Messages, new JArray {
                new JObject {
                    {
                        "From",
                           new JObject {
                               {"Email", "buildingmanager.net5@gmail.com"},
                               {"Name", "Building Manager .NET 5"}
                           }
                    },
                    {
                        "To",
                            new JArray {
                                new JObject {
                                    {
                                        "Email",email
                                    }, {
                                        "Name",
                                        "User"
                                        }
                                    }
                                }
                    },
                    {
                        "Subject", subject
                    },
                    {
                        "HTMLPart", body
                    }
                    }
                });
            await client.PostAsync(request);
        }
    }
}

