using MHFoodBank.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MHFoodBank.Web.Services
{
    public interface IEmailConfirm
    {
        Task TrySendConfirmationEmail(AppUser user, HttpRequest request, IUrlHelper url);
    }

    public class EmailConfirmation : IEmailConfirm
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;

        public EmailConfirmation(UserManager<AppUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task TrySendConfirmationEmail(AppUser user, HttpRequest request, IUrlHelper url)
        {
            try
            {
                string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                string callbackUrl = url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = user.Id, code = code },
                    protocol: request.Scheme);

                await _emailSender.SendEmailAsync(user.Email, "MHFB - Confirm your email",
                    $"<img alt='Medicine Hat Food Bank Logo' src='https://static.wixstatic.com/media/9c0c8c_8e19160d960f483f9252fcfd8b45af4a~mv2_d_8359_7389_s_4_2.png/v1/fill/w_120,h_106,al_c,q_85,usm_0.66_1.00_0.01/9c0c8c_8e19160d960f483f9252fcfd8b45af4a~mv2_d_8359_7389_s_4_2.webp'><br/><br/>Thanks for applying as a volunteer to the Medicine Hat Food Bank!<br/><br/>Once you confirm your account, you will be able to login to manage your availability and personal information. Once you're accepted you will be able to view and sign up for open shifts.<br/><br/>Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
