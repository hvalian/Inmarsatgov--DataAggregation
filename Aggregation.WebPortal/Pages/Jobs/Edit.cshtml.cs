using Aggregation_DataModels.Models;
using Aggregation_Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace Website.Pages.Jobs
{
    public class EditModel : PageModel
    {
        private readonly AggregationDbContext _context;

        public EditModel(AggregationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TbJob TbJob { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null || _context.TbJobs == null)
            {
                return NotFound();
            }

            var tbjob =  await _context.TbJobs.FirstOrDefaultAsync(m => m.Id == id);
            if (tbjob == null)
            {
                return NotFound();
            }
            TbJob = tbjob;
           ViewData["Interval"] = new SelectList(_context.TbIntervals, "Id", "Id");
           ViewData["JobType"] = new SelectList(_context.TbJobTypes, "Id", "Id");
           ViewData["Priority"] = new SelectList(_context.TbPriorities, "Id", "Id");
           ViewData["Status"] = new SelectList(_context.TbStatuses, "Id", "Id");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(TbJob).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TbJobExists(TbJob.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Redirect("../Jobs/Index");
        }

        private bool TbJobExists(long id)
        {
          return _context.TbJobs.Any(e => e.Id == id);
        }
    }
}
