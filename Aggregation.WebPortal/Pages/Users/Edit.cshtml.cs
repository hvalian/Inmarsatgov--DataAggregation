using Aggregation_DataModels.Models;
using Aggregation_Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using System.ComponentModel.DataAnnotations;

namespace Website.Pages.Users
{
    public class EditModel : PageModel
    {
        private readonly IAggregationRepository aggregationRepository;
        private readonly IToastNotification toastNotification;
        private readonly IUserService userRepository;

        public EditModel(IAggregationRepository aggregationRepository, IToastNotification toastNotification, IUserService userRepository)
        {
            this.aggregationRepository = aggregationRepository;
            this.toastNotification = toastNotification;
            this.userRepository = userRepository;
        }

        public bool IsAdminAccess()
        {
            return userRepository.HasAdminAccess();
        }

        public bool IsAdminEditable()
        {
            return (userRepository.GetUserId() != TbUser.UserId);
        }

        #region Properties
        [BindProperty]
        public TbUser TbUser { get; set; } = default!;

        [BindProperty]
        [Display(Name = "User Name")]
        [Required(ErrorMessage = "User Name is Required")]
        [StringLength(maximumLength: 255, MinimumLength = 5)]
        public string Name { get; set; } = default!;
        #endregion

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!IsAdminAccess())
            {
                toastNotification.AddErrorToastMessage("Your request could not be processed becase of lack of permission!");
                return Redirect("../Utility/Unauthorized");
            }

            if (id == null)
            {
                return NotFound();
            }

            var tbuser = await this.aggregationRepository.GetUserAsync((int)id);

            if (tbuser == null)
            {
                return NotFound();
            }

            TbUser = tbuser;
            Name = tbuser.Name;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!IsAdminAccess())
            {
                toastNotification.AddErrorToastMessage("Your request could not be processed becase of lack of permission!");
                return Redirect("../Utility/Unauthorized");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            TbUser.Name = Name;

            await this.aggregationRepository.SaveUser(TbUser, false, userRepository.GetUserId() != TbUser.UserId);


            return RedirectToPage("./Index");
        }
    }
}
