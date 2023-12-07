using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using Aggregation_DataModels.Models;
using Aggregation_Services;

namespace Website.Pages.Jobs
{
    public class DeleteModel : PageModel
    {
        private readonly AggregationDbContext _context;

        public DeleteModel(AggregationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public TbJob TbJob { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null || _context.TbJobs == null)
            {
                return NotFound();
            }

            var tbjob = await _context.TbJobs.FirstOrDefaultAsync(m => m.Id == id);

            if (tbjob == null)
            {
                return NotFound();
            }
            else 
            {
                TbJob = tbjob;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long? id)
        {
            if (id == null || _context.TbJobs == null)
            {
                return NotFound();
            }
            var tbjob = await _context.TbJobs.FindAsync(id);

            if (tbjob != null)
            {
                TbJob = tbjob;
                _context.TbJobs.Remove(TbJob);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
