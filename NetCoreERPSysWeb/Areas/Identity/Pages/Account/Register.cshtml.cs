// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using NetCoreERPSys.DataAccess.Repository.IRepository;
using NetCoreERPSys.Models;
using NetCoreERPSys.Utility;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

namespace NetCoreERPSysWeb.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        private readonly IUnitOfWork _unitOfWork;

        public RegisterModel(
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IUnitOfWork unitOfWork)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// 当用户提交表单（HTTP POST 请求）时，请自动将表单中的数据绑定到这个 Input 属性上. 例如 Input.Email 属性
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public string? Role { get; set; }

            public int? CompanyId { get; set; }

            [ValidateNever]
            public IEnumerable<SelectListItem> RoleList { get; set; }

            [ValidateNever]
            public IEnumerable<SelectListItem> CompanyList { get; set; }

            public string? PhoneNumber { get; set; }

            public string? StreetAddress { get; set; }

            public string? City { get; set; }

            public string? State { get; set; }

            public string? PostalCode { get; set; }

            public string? Name { get; set; }
        }

        /*
         * ASP.NET Core Identity 注册页面 (Register.cshtml) 的标准后台代码.
         * 它的作用就是在用户通过 GET 请求访问注册页面时，进行准备工作.
         */
        public async Task OnGetAsync(string returnUrl = null)
        {
            // 在系统部署后，第一次有任何人（即第一个访问者）试图访问注册页面时，这段代码会被触发.
            if (!await _roleManager.RoleExistsAsync(SD.Role_User_Cust))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Cust));
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee));
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Comp));
            }

            Input = new InputModel();

            // 1. 从数据库中获取所有角色的名称，并将它们存储在一个临时的字符串集合中.
            var roleNames = _roleManager.Roles.Select(r => r.Name);

            // 2. 将刚才获取到的角色名称字符串集合，转换为一个 SelectListItem 对象的集合.
            var roleSelectList = roleNames.Select(r => new SelectListItem
            {
                Text = r, // 下拉框看到的文本.
                Value = r // 下拉框提交的数据.
            }).ToList(); // 使用 ToList() 立即执行查询并生成列表

            // 最后，将最终生成的 SelectListItem 列表，赋值给在第 1 步创建的 Input 实例的 RoleList 属性.
            Input.RoleList = roleSelectList;

            var companies = _unitOfWork.Company.GetAll();

            Input.CompanyList = companies.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.CompanyId.ToString() // 提交的是 Id.
            }).ToList();

            ReturnUrl = returnUrl;

            // 获取第三方登录的服务商信息.
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                // _userStore 和 _emailStore 是更底层的服务，它们的 Set... 方法包含了额外的、必要的处理逻辑.
                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                // 对于不属于 Identity 框架核心功能的, 使用普通绑定（直接赋值）即可.
                user.Name = Input.Name;
                user.StreetAddress = Input.StreetAddress;
                user.City = Input.City;
                user.State = Input.State;
                user.PostalCode = Input.PostalCode;
                user.PhoneNumber = Input.PhoneNumber;

                if (Input.Role == SD.Role_User_Comp)
                {
                    user.CompanyId = Input.CompanyId;
                }

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    if (!String.IsNullOrEmpty(Input.Role))
                    {
                        await _userManager.AddToRoleAsync(user, Input.Role);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, SD.Role_User_Cust);
                    }

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user); // 注册服务需要带上 AddDefaultTokenProviders();
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            // 重新渲染 Register.cshtml 页面 的时候, 会寻找 RoleList 和 CompanyList 属性来填充选项.
            // 但是 这两个属性在当前的 OnPostAsync 请求中是 null, 当 asp-items 尝试在一个 null 的列表上进行循环时，程序会抛出一个服务器端异常.
            return Page();
        }

        private ApplicationUser CreateUser() // 修改之后创建的 User 即 ApplicationUser
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
