using Aggregation_DataModels.Models;
using Aggregation_Services;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using NToastNotify;

namespace Website.Pages.Users
{
    public class CreateModel : PageModel
    {
        private readonly IAggregationRepository aggregationRepository;
        private readonly IToastNotification toastNotification;
        private readonly IUserService userRepository;

        public CreateModel(IAggregationRepository aggregationRepository, IToastNotification toastNotification, IUserService userRepository)
        {
            this.aggregationRepository = aggregationRepository;
            this.toastNotification = toastNotification;
            this.userRepository = userRepository;
        }

        public bool IsAdminAccess()
        {
            return userRepository.HasAdminAccess();
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        #region Properties
        [BindProperty]
        public TbUser TbUser { get; set; } = default!;

        [BindProperty]
        [Display(Name = "User Id")]
        [Required(ErrorMessage = "User Id is Required")]
        [StringLength(maximumLength: 255, MinimumLength = 5)]
        public string UserId { get; set; } = default!;

        [BindProperty]
        [Display(Name = "User Name")]
        [Required(ErrorMessage = "User Name is Required")]
        [StringLength(maximumLength: 255, MinimumLength = 5)]
        public string Name { get; set; } = default!;
        #endregion

        public async Task<IActionResult> OnPostAsync()
        {
            if (!IsAdminAccess())
            {
                toastNotification.AddErrorToastMessage("Your request could not be processed becase of lack of permission!");
                return Redirect("../Utility/Unauthorized");
            }

            if (aggregationRepository.GetUser(UserId) != null)
            {
                ModelState.AddModelError("UserId", $"User Id is required and muse be uniqe. {UserId} already exist.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            TbUser.UserId = UserId;
            TbUser.Name = Name;

            await this.aggregationRepository.SaveUser(TbUser, true, true);

            return RedirectToPage("./Index");
        }
    }
}
