using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ProjectSystemAPI.DTO;
using ChatServerDTO.DB;

namespace ChatServerDTO.DB
{
    public class PostPassword
    {
        public static bool PostPass(UserDTO user, string password, UserDTO from)
        {
            try
            {
                MailAddress fromAdress = new MailAddress("nikitina@suz-ppk.ru", from.FIO);
                MailAddress toAdress = new MailAddress(user.Email);
                MailMessage message = new MailMessage(fromAdress, toAdress);
                message.Body = "Добрый день, " + user.FIO + "! " + Environment.NewLine + "Ваш новый логин: " + user.Email + " " + Environment.NewLine + "Ваш новый пароль: " + password + " ";
                message.Subject = "Регистрация нового пользователя";

                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Host = "smtp.beget.com";
                smtpClient.Port = 25;
                //smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(fromAdress.Address, "zzPwr%j0");

                smtpClient.Send(message);
                return true;
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }
    }
}
