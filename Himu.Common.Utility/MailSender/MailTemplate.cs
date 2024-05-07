namespace Himu.Common.Service
{
    public static class MailActivationCodeTemplate
    {
        public static string GetTemplate(string userName, string token)
        {
            return "<b style=\"color:#707070;\"> Himu 客户酱：账号管理 </b>" +
                "<h2 style=\"color:#d9246e\"> 激活您的账号 </h2>" +
                $"<hr/> <p> 亲爱的 {userName}，感谢您在 Himu 注册账户！ 您的邮箱激活代码为： </p>" +
                $"<center> <h2> {token} </h2> </center> " +
                "<p> 了解更多详细信息，请联系本站管理员。 </p> <hr/>" +
                "<p> 谢谢！ <br/>爱你的 Himu 客服酱 ~ </p>";
        }
    }

    public static class MailResetPasswordCodeTemplate
    {
        public static string GetTemplate(string userName, string token)
        {
            return
                "<b style=\"color:#707070;\"> Himu 客户酱：账号管理 </b>" +
                "<h2 style=\"color:#d9246e\"> 重置您的账号密码 </h2>" +
                $"<hr/> <p> 亲爱的 {userName}，请注意！您正在使用邮箱重置密码， 您的密码重置代码为： </p>" +
                $"<center> <h2> {token} </h2> </center> " +
                "<b> 注意！如果本邮件不是由您主动操作，您的账号很可能有安全问题。 </b> <hr/>" +
                "<p> 了解更多详细信息，请联系本站管理员。 </p> <hr/>" +
                "<p> 谢谢！ <br/>爱你的 Himu 客服酱 ~ </p>";
        }
    }

    public static class MailActivationCompletedTemplate
    {
        public static string GetTemplate(string userName)
        {
            return "<b style=\"color:#707070;\"> Himu 客户酱：账号管理 </b>" +
                   $"<hr/> <p> 亲爱的 {userName}，你已成功完成账户的激活，现在你可以登录并使用相关服务了！^_^ </p>" +
                   "<p> 了解更多详细信息，请联系本站管理员。 </p> <hr/>" +
                   "<p> 谢谢！ <br/>爱你的 Himu 客服酱 ~ </p>";
        }
    }
}