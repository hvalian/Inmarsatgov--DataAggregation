using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

using Aggregation_DataModels.Models;
using Aggregation_Services;


namespace Website.Pages.Jobs
{
    public class CreateModel : PageModel
    {
        private readonly AggregationDbContext _context;

        public CreateModel(AggregationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["Interval"] = new SelectList(_context.TbIntervals, "Id", "Id");
        ViewData["JobType"] = new SelectList(_context.TbJobTypes, "Id", "Id");
        ViewData["Priority"] = new SelectList(_context.TbPriorities, "Id", "Id");
        ViewData["Status"] = new SelectList(_context.TbStatuses, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public TbJob TbJob { get; set; }
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.TbJobs.Add(TbJob);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
