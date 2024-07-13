using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Chen.Infrastructure.EmailCore
{
    public record EmailBus(string StmpServer, string MailAccount, string Password)
    {
        // 发送邮件
        public async Task<bool> SendEmailAsync(string mailTo, string mailTitle, string mailContent)
        {
            //设置发送方邮件信息，例如：qq邮箱
            //string stmpServer = @"smtp.qq.com";//smtp服务器地址
            //string mailAccount = @"2578036363@qq.com";//邮箱账号
            //string pwd = @"hvuagjcurnytebja";//邮箱密码（qq邮箱此处使用授权码，其他邮箱见邮箱规定使用的是邮箱密码还是授权码）

            string stmpServer = StmpServer;//smtp服务器地址
            string mailAccount = MailAccount;//邮箱账号
            string pwd = Password;//邮箱密码（qq邮箱此处使用授权码，其他邮箱见邮箱规定使用的是邮箱密码还是授权码）

            //邮件服务设置
            using SmtpClient smtpClient = new SmtpClient();
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//指定电子邮件发送方式
            smtpClient.Host = stmpServer;//指定发送方SMTP服务器
            smtpClient.EnableSsl = true;//使用安全加密连接
            smtpClient.UseDefaultCredentials = false;//不和请求一起发送
            smtpClient.Credentials = new NetworkCredential(mailAccount, pwd);//设置发送账号密码

            using MailMessage mailMessage = new MailMessage(mailAccount, mailTo);//实例化邮件信息实体并设置发送方和接收方
            mailMessage.Subject = mailTitle;//设置发送邮件得标题
            mailMessage.Body = mailContent;//设置发送邮件内容
            mailMessage.BodyEncoding = Encoding.UTF8;//设置发送邮件得编码
            mailMessage.IsBodyHtml = false;//设置标题是否为HTML格式
            mailMessage.Priority = MailPriority.Normal;//设置邮件发送优先级

            try
            {
                await smtpClient.SendMailAsync(mailMessage);//发送邮件
                return true;
            }
            catch (SmtpException ex)
            {
                throw ex;
            }
        }
    }
}
