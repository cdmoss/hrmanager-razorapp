using MHFoodBank.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MHFoodBank.Web.Services
{
    public interface IEmailForgotPassword
    {
        Task<bool> SendForgotPasswordEmailAsync(string email, HttpRequest request, IUrlHelper url);
    }
    public class EmailForgotPassword : IEmailForgotPassword
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;

        public EmailForgotPassword(UserManager<AppUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task<bool> SendForgotPasswordEmailAsync(string email, HttpRequest request, IUrlHelper url)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return false;
            }

            // For more information on how to enable account confirmation and password reset please 
            // visit https://go.microsoft.com/fwlink/?LinkID=532713
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = url.Page(
                "/Account/ForgotPassword",
                pageHandler: null,
                values: new { area = "Identity", code },
                protocol: request.Scheme);

            await _emailSender.SendEmailAsync(
                email,
                "Reset Password - MHFB",
                $"<img alt='Medicine Hat Food Bank Logo' src='https://static.wixstatic.com/media/9c0c8c_8e19160d960f483f9252fcfd8b45af4a~mv2_d_8359_7389_s_4_2.png/v1/fill/w_120,h_106,al_c,q_85,usm_0.66_1.00_0.01/9c0c8c_8e19160d960f483f9252fcfd8b45af4a~mv2_d_8359_7389_s_4_2.webp'><br/><br/>Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.<br/><br/>If you did not request to change your password, please contact your supervisor.");

            return true;
        }
    }
}
