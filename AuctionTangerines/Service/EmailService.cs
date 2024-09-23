using AuctionTangerines.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionTangerines
{
    public class EmailService
    {
  
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Аукцион", "your_email@example.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;

            emailMessage.Body = new TextPart("html") { Text = message };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.example.com", 587, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync("mandarinTest@example.com", "132456");
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }

        public async Task SendWinNotification(string winnerEmail, Mandarin tangerine)
        {
            var subject = "Поздравляем! Вы выиграли аукцион!";
            var message = $"Поздравляем! Вы выиграли аукцион на мандаринку! " +
                          $"Текущая ставка: {tangerine.CurrentPrice}. " +
                          $"Пожалуйста, проверьте ваш чек.";

            await SendEmailAsync(winnerEmail, subject, message);
        }

        public void GenerateReceipt(Mandarin tangerine)
        {
            var receiptContent = $"Чек\n" +
                                 $"--------------------\n" +
                                 $"Мандаринка ID: {tangerine.Id}\n" +
                                 $"Текущая ставка: {tangerine.CurrentPrice}\n" +
                                 $"Победитель: {tangerine.WinnerEmail}\n" +
                                 $"Дата аукциона: {tangerine.AuctionEndTime}\n" +
                                 $"--------------------\n" +
                                 $"Спасибо за участие в аукционе!";


            System.IO.File.WriteAllText($"Receipt_{tangerine.Id}.txt", receiptContent);

            SendEmailAsync(tangerine.WinnerEmail, "Ваш чек", receiptContent);
        }
    }
}
