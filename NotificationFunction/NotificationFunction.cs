using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Net;

namespace NotificationFunction
{
    public static class NotificationFunction
    {
        [FunctionName("NotificationFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("yourmail@gmail.com");
            mailMessage.To.Add("yourmail@gmail.com");
            mailMessage.Subject = "THis mail has been send from azure functions.";
            mailMessage.Body = "Your order has been created and will be processed shortly.";

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "yourmail@gmail.com";
            smtpClient.Port = 587;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("yourmail@gmail.com", "ABCDABCD");
            smtpClient.EnableSsl = true;

            try
            {
                smtpClient.Send(mailMessage);
                Console.WriteLine("Email Sent Successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            //string name = req.Query["name"];

            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            //name = name ?? data?.name;

            //string responseMessage = string.IsNullOrEmpty(name)
            //    ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //    : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult("Success");
        }
    }
}
