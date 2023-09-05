using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Security.Claims;
using TeamManageSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Sprint = TeamManageSystem.Models.Account.Sprint;
using Rating = TeamManageSystem.Models.Account.Rating;
using TMembers = TeamManageSystem.Models.Account.TMembers;
using Task = TeamManageSystem.Models.Account.Task;
using SprintRating = TeamManageSystem.Models.Account.SprintRating;
using MainSprint = TeamManageSystem.Models.Account.MainSprints;
using TeamRating = TeamManageSystem.Models.Account.TeamRating;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TeamManageSystem.Models.ViewModel;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using TeamManageSystem.Models.Account;
using TeamManageSystem.Migrations;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Newtonsoft.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Collections.Immutable;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Collections.Generic;

namespace TeamManageSystem.Controllers.Account
{
    [Authorize]
    public class DashboardController : Controller
    {
        //private readonly UserManager<IdentityUser> _userManager;
        private readonly TeamManageContext _context;
        public DashboardController(TeamManageContext context/* , UserManager<IdentityUser> userManager*/)
        {

            _context = context;
            // _userManager = userManager;

        }
        public IActionResult BestPerformer()
        {
            string userId = null;
            if (User.Identity.IsAuthenticated)
            {

                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            int userId1;
            int.TryParse(userId, out userId1);
            var members = _context.TMembers.Where(m => m.LeadId == userId1).ToList();
            var memberIds = members.Select(m => m.Id).ToList();
            var ratings = _context.Rating.Where(r => memberIds.Contains(r.MemberId)).OrderByDescending(r => r.Ratings).ToList();
            var highestRating = ratings.FirstOrDefault();
            // If you want to find all records with the highest Ratings (in case of ties)
            var highestRatings = ratings
                .Where(r => r.Ratings == highestRating?.Ratings)
                .ToList();
            return View(highestRatings);
        }
        public IActionResult TDashboard()
        {

            string userId = null;
            if (User.Identity.IsAuthenticated)
            {

                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            int userId1;
            int.TryParse(userId, out userId1);
            var profileuser = _context.User.Where(p => p.Id == userId1).FirstOrDefault();
            var members = _context.TMembers.Where(m => m.LeadId == userId1).ToList();
            var sprints = _context.Sprint.Where(s => s.CreatedBy == userId1).ToList();
            var memberIds = members.Select(m => m.Id).ToList();
            var ratings = _context.Rating.Where(r => memberIds.Contains(r.MemberId)).OrderByDescending(r => r.Ratings).ToList();

            var highestRating = ratings.FirstOrDefault();

            // If you want to find all records with the highest Ratings (in case of ties)
            var highestRatings = ratings
                .Where(r => r.Ratings == highestRating?.Ratings)
                .ToList();

            var dashboard = new Dashboard
            {
                MembersModel = members,
                SprintsModel = sprints,
                RatingsModel = ratings,
                usermodel = profileuser,
            };
            return View(dashboard);
        }
        [HttpGet]
        public IActionResult Profiles()
        {
            string userId = null;
            if (User.Identity.IsAuthenticated)
            {

                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            int userId1;
            int.TryParse(userId, out userId1);
            var member = _context.User.Where(m => m.Id == userId1).FirstOrDefault();
            return PartialView("_ProfilePartial", member);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(User model)
        {
            var user = _context.User.Where(u => u.Id == model.Id).FirstOrDefault();
                user.Username = model.Username;
                user.Email = model.Email;
                user.Mobile = model.Mobile;
                _context.Update(user);
                _context.SaveChanges();
            if (user.Role=="Admin")
            {
                return RedirectToAction(nameof(ADashboard));
            }
            else
            {
                return RedirectToAction(nameof(TDashboard));
            }
        }
        public IActionResult Home()
        {
            return View();
        }
        public IActionResult AddSprintRating(int? id)
        {
            string userId = null;
            // Get the logged-in user's UserId (replace this with your actual method of getting the UserId)
            userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            int userId1;
            int.TryParse(userId, out userId1);

            // Retrieve the list of members whose LeadId matches the logged-in UserId
            var assigneeList = _context.TMembers.Where(m => m.LeadId == userId1).Select(m => new { m.Id, m.TMname }).ToList();

            // You can use ViewData or a ViewModel to pass the list to the view
            ViewData["AssigneeList"] = new SelectList(assigneeList, "Id", "TMname"); // Assuming the member model has properties "Id" and "Name"

            return View();
        }
        [HttpPost]
        [Route("Dashboard/AddSprintRating")]
        [ValidateAntiForgeryToken]
        public IActionResult AddSprintRating(Models.Account.SprintRating Model)
        {
            try {
                int sprintid = int.Parse(Request.Form["SprintId"]);
                var assigned = _context.TMembers.FirstOrDefault(a => a.Id == Model.Mid);
                var srating = _context.SprintRating.Where(a => a.Mid == Model.Mid && a.SprintId == Model.SprintId).FirstOrDefault();
                string assigname;
                if (srating != null)
                {
                    srating.SRating = Model.SRating;


                }
                else
                {
                    assigname = assigned.TMname;
                    var sprintwiserating = new SprintRating()
                    {
                        SRating = Model.SRating,
                        SprintId = sprintid,
                        Mname = assigname,
                        Mid = Model.Mid,
                    };
                    _context.SprintRating.Add(sprintwiserating);
                }
                _context.SaveChanges();
                return RedirectToAction("ViewSprint", "Dashboard");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "An error occurred while adding/updating the rating.";
                return RedirectToAction("ViewSprint", "Dashboard");
            }
        }
        [HttpGet]
        public IActionResult AddScores(int id)
        {
            string userId = null;
            // Get the logged-in user's UserId (replace this with your actual method of getting the UserId)
            userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            int userId1;
            int.TryParse(userId, out userId1);
            var rating = _context.SprintRating.Where(r => r.SprintId == id).ToList();
            var teamMembers = _context.TMembers.Where(m => m.LeadId == userId1).ToList();
            var sprintrating = _context.SprintRating.Where(s => s.SprintId == id && s.IsDelete == 0).ToList();
            var sprintRating = _context.SprintRating.Where(s => s.SprintId == id && s.IsDelete == 1).ToList();

            if (sprintrating.Count == 0 && sprintRating.Count == 0)
            {
                var viewModelList = teamMembers.Select(member => new SRatingViewModel
                {
                    MemberID = member.Id,
                    MemberName = member.TMname,
                    SprintRating = 0 // Set SprintRating to 0 for all members
                }).ToList();
                var modellist = new ScoresModel
                {
                    RatingViewModel = viewModelList,
                    SprintRatings = rating,
                };

                return View(modellist);
            }
            else if (sprintrating.Count == 0 && sprintRating.Count != 0)
            {
                var sprintRatingMemberNames = sprintRating.Select(s => s.Mname).ToList();

                // Get the team members whose names are not in the sprintRatingMemberNames list
                var unmatchedTeamMembers = teamMembers
                    .Where(m => !sprintRatingMemberNames.Contains(m.TMname))
                    .ToList();
                var viewModelList = unmatchedTeamMembers.Select(member => new SRatingViewModel
                {
                    MemberID = member.Id,
                    MemberName = member.TMname,
                    SprintRating = 0 // Initialize SprintRating to 0 for the form
                }).ToList();
                var modellist = new ScoresModel
                {
                    RatingViewModel = viewModelList,
                    SprintRatings = rating,
                };
                return View(modellist);
            }
            else
            {
                var viewModelList = sprintrating.Select(member => new SRatingViewModel
                {
                    MemberID = member.Mid,
                    MemberName = member.Mname,
                    SprintRating = 0 // Initialize SprintRating to 0 for the form
                }).ToList();
                var modellist = new ScoresModel
                {
                    RatingViewModel = viewModelList,
                    SprintRatings = rating,
                };
                return View(modellist);
            }
            return View();
        }

        [HttpPost]
        [Route("Dashboard/AddScores")]
        [ValidateAntiForgeryToken]
        public IActionResult AddScores(List<SRatingViewModel> viewModelList, int SprintID)
        {



            foreach (var viewModel in viewModelList)
            {
                // Find the existing SprintRating record (if any) or create a new one
                var sprintRating = _context.SprintRating.FirstOrDefault(sr => sr.Mid == viewModel.MemberID && sr.SprintId == SprintID)
                                  ?? new SprintRating();
                var member = _context.TMembers.FirstOrDefault(m => m.Id == viewModel.MemberID);
                // Update the SprintRating properties
                sprintRating.SprintId = SprintID;
                sprintRating.SRating = viewModel.SprintRating;
                sprintRating.Mname = member.TMname;
                sprintRating.Mid = viewModel.MemberID;
                sprintRating.IsDelete = 0;

                // Add or update the record in the database
                if (sprintRating.id == 0)
                {
                    _context.SprintRating.Add(sprintRating);
                }
                else
                {
                    _context.SprintRating.Update(sprintRating);
                }


                _context.SaveChanges();
            }
            // Redirect to a success page or perform any other necessary action
            return RedirectToAction("ViewSprints", "Dashboard"); // Change to your desired action and controller


            // If model state is not valid, return back to the view
        }

        public IActionResult AddSprint()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddSprint(Sprint model)
        {


            // Your logic for authenticated users
            // For example, you can perform some action or return specific content.
            string userId = null;
            if (User.Identity.IsAuthenticated)
            {

                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            int userId1;
            int.TryParse(userId, out userId1);
            DateTime currentDate = DateTime.Now;
            // Get the month number from the current date
            //int monthNumber = currentDate.Month;
            //get the sprints of same month 
            //var sprintsInMonth = _context.Sprint
            // .Where(sprint => sprint.CreatedTime.Month == monthNumber && sprint.CreatedBy==userId1)
            // .ToList();
            // int Scount = sprintsInMonth.Count();
            var sprints = _context.Sprint.Where(s => s.CreatedBy == userId1).ToList();
            int count = sprints.Count();
            string title = "";
            if (!string.IsNullOrEmpty(model.Title))
            {
                title = model.Title;
            }
            else
            {
                title = "No Title";
            }
            var data = new Sprint()
            {
                Title = title,
                SprintNo = count + 1,
                SDate = model.SDate,
                EDate = model.EDate,
                CreatedTime = DateTime.Now,
                CreatedBy = userId1,
                Discription = model.Discription,
            };
            _context.Sprint.Add(data);
            _context.SaveChanges();
            TempData["SsuccessMessage"] = "Sprint Add Successfully";
            return RedirectToAction("TDashboard", "Dashboard");




        }

        /* public async Task<IActionResult> ViewSprint()
         {
             try
             {
                 string userId = null;
                 if (User.Identity.IsAuthenticated)
                 {

                     userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                 }
                 int userId1;
                 int.TryParse(userId, out userId1);
                 var sprints = await _context.Sprint
                 .Where(s => s.CreatedBy == userId1)
                 .ToListAsync();
                 return View(sprints);
             }
             catch (Exception ex)
             {
                 TempData["errorMessage"] = "An error occurred while retrieving Sprints";
                 return View(new List<Sprint>());
             }

         }*/

        public IActionResult ViewSprints()
        {
            try
            {
                string userId = null;
                if (User.Identity.IsAuthenticated)
                {

                    userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                }
                int userId1;
                int.TryParse(userId, out userId1);
                var sprints = _context.Sprint
                .Where(s => s.CreatedBy == userId1)
                .ToList();
                var members = _context.TMembers.Where(m => m.LeadId == userId1).ToList();
                var sprintRatings = _context.SprintRating.ToList()
                      .Join(
                          inner: members,
                          outerKeySelector: sr => sr.Mid,
                          innerKeySelector: member => member.Id,
                          resultSelector: (sr, member) => sr)
                      .ToList();
                var mainsprints = new MainSprints
                {
                    Sprints = sprints,
                    TeaMembers = members,
                    SprintRatings = sprintRatings,
                };

                return View(mainsprints);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "An error occurred while retrieving Sprints";
                return View(new List<Sprint>());
            }

        }

        public async Task<IActionResult> ViewTeam()
        {
            try
            {
                string userId = null;
                if (User.Identity.IsAuthenticated)
                {

                    userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                }
                int userId1;
                int.TryParse(userId, out userId1);
                var teamMembers = await _context.TMembers
                .Where(tm => tm.LeadId == userId1)
                .ToListAsync();
                return View(teamMembers);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "An error occurred while retrieving team members.";
                return View(new List<TMembers>());
            }
            //return _context.TMembers != null ?
            //View(await _context.TMembers.ToListAsync()) :
            //Problem("Entity set 'MVCStudentsContext.Students'  is null.");
        }


        public async Task<IActionResult> ViewRating(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var rating = await _context.Rating
                .FirstOrDefaultAsync(r => r.MemberId == id);
            if (rating == null)
            {
                var sprintrating = await _context.SprintRating.Where(s => s.Mid == id && s.IsDelete == 0).ToListAsync();
                var Member = await _context.TMembers.Where(m => m.Id == id).FirstOrDefaultAsync();
                string mname = Member.TMname;
                int sumSRating = sprintrating.Sum(s => s.SRating);
                int countSRating = sprintrating.Count;
                if (countSRating == 0)
                {
                    return BadRequest("There is no any Sprint Rating for this user! Firstly add Sprint Rating then View Whole Rating");
                }
                decimal avr = sumSRating / countSRating;
                var data = new Rating()
                {
                    MemberId = Member.Id,
                    MemberName = mname,
                    Ratings = (int)avr,
                    FeedBack = "",
                };

                _context.Rating.Add(data);
                _context.SaveChanges();
                return View(data);
            }

            var sprintrate = await _context.SprintRating.Where(s => s.Mid == id && s.IsDelete == 0).ToListAsync();
            int sumSRate = sprintrate.Sum(s => s.SRating);
            int countSRate = sprintrate.Count;
            decimal avrs = sumSRate / countSRate;
            rating.Ratings = (int)avrs;
            _context.SaveChanges();
            return View(rating);

            //var sprintrating = await _context.SprintRating.Where(s => s.Mid == id).ToListAsync();
            //int sumSRating = sprintrating.Sum(s => s.SRating);
            // int countSRating = sprintrating.Count;
            // decimal avr = sumSRating / countSRating;
            // rating.Ratings = (int)avr
        }

        public IActionResult BonusGenerate(int? id)
        {
            if (id == null || _context.Rating == null)
            {
                return NotFound();
            }
            var rating = _context.Rating.FirstOrDefault(r => r.Id == id);
            var member = _context.TMembers.FirstOrDefault(m => m.Id == rating.MemberId);
            if (rating == null)
            {
                return NotFound();
            }
            decimal basicSalary = member.Salary;
            int originalRating = rating.Ratings;
            decimal bonus = 0;
            string message = "";
            if (originalRating == 4)
            {
                bonus = (basicSalary / 100) * 5;
            }
            else if (originalRating == 5)
            {
                bonus = (basicSalary / 100) * 10;
            }
            else
            {
                message = "This Member is not eligible for bonus";
            }
            ViewBag.Message = message;
            ViewBag.BonusRating = bonus;
            return View(rating);

        }

        public IActionResult AddMember()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddMember(Models.Account.TMembers model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string userId = null;
                    if (User.Identity.IsAuthenticated)
                    {
                        // Your logic for authenticated users
                        // For example, you can perform some action or return specific content.
                        userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    }
                    int userId1;
                    int.TryParse(userId, out userId1);
                    var data = new TMembers()
                    {
                        TMname = model.TMname,
                        Role = model.Role,
                        LeadId = userId1,
                        Salary = model.Salary,
                    };
                    _context.TMembers.Add(data);
                    _context.SaveChanges();
                    TempData["TMsuccessMessage"] = "Team Member Add Successfully";
                    return RedirectToAction("TDashboard", "Dashboard");
                }
                else
                {
                    TempData["errorMessage"] = "Please Fill Out All Credentials";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "An error occurred while adding the team member.";
                return View(model);
            }

        }

        [HttpGet]
        public IActionResult FilteredSprints(DateTime startDate, DateTime endDate)
        {
            string userId = null;
            if (User.Identity.IsAuthenticated)
            {

                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            int userId1;
            int.TryParse(userId, out userId1);
            var sprints = _context.Sprint.Where(r => (r.SDate <= startDate && r.EDate <= endDate && r.EDate >= startDate) ||
                    (r.SDate >= startDate && r.EDate >= endDate && r.SDate <= endDate) ||
                    (r.SDate >= startDate && r.EDate <= endDate) ||
                    (r.SDate <= startDate && r.EDate >= endDate))
        .Where(r => r.CreatedBy == userId1)
        .ToList();
            var members = _context.TMembers.Where(m => m.LeadId == userId1).ToList();
            var sprintRatings = _context.SprintRating.ToList()
                  .Join(
                      inner: members,
                      outerKeySelector: sr => sr.Mid,
                      innerKeySelector: member => member.Id,
                      resultSelector: (sr, member) => sr)
                  .ToList();
            var mainsprints = new MainSprints
            {
                Sprints = sprints,
                TeaMembers = members,
                SprintRatings = sprintRatings,
            };

            return PartialView("_SprintPartial", mainsprints);
        }
        /*public PartialViewResult LoadSprintPartial(List<SprintViewModel> sprints)
        {
            return PartialView("_SprintPartial", sprints);
        }*/
        public IActionResult InputFilterSprint()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FilterSprints(SprintViewModel model)
        {
            string userId = null;
            if (User.Identity.IsAuthenticated)
            {

                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            int userId1;
            int.TryParse(userId, out userId1);
            if (ModelState.IsValid)
            {
                DateTime startDate = model.StartDate;
                DateTime endDate = model.EndDate;

                var sprints = await _context.Sprint
                .Where(r => r.SDate >= startDate && r.EDate <= endDate && r.CreatedBy == userId1)
                .ToListAsync();
                return View(sprints);


            }

            // If the model state is not valid, return the view with validation errors.
            return View(model);
        }

        public IActionResult InputFilterRating(int id)
        {
            var viewModel = new SprintViewModel
            {
                Id = id // Assuming you have a property named "Id" in the SprintViewModel class
            };
            return View();
        }
        [HttpPost]
        [Route("Dashboard/InputFilterRating")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SpecificRating(SprintViewModel model)
        {
            // int memberId = int.Parse(Request.Form["id"]);
            if (!int.TryParse(Request.Form["id"], out int memberId))
            {
                return NotFound(); // Or handle the error appropriately based on your requirements
            }
            if (memberId == null)
            {
                return NotFound();
            }

            // var rating = await _context.Rating
            // .FirstOrDefaultAsync(r => r.MemberId == memberId);
            string userId = null;
            if (User.Identity.IsAuthenticated)
            {

                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            int userId1;
            int.TryParse(userId, out userId1);
            if (ModelState.IsValid)
            {
                DateTime startDate = model.StartDate;
                DateTime endDate = model.EndDate;

                // get the record of sprints between the given dat range
                var sprints = await _context.Sprint
                .Where(r => r.SDate >= startDate && r.EDate <= endDate && r.CreatedBy == userId1)
                .ToListAsync();
                //select the sprintId's of retrieved sprints
                var sprintIds = sprints.Select(s => s.Id).ToList();
                //retrieve sprintratings they have memberId and previously retrieved sprintId's
                var sprintrating = await _context.SprintRating.Where(s => s.Mid == memberId && sprintIds.Contains(s.SprintId)).ToListAsync();
                //retrieve the record of member having same memberId   
                var Member = await _context.TMembers.Where(m => m.Id == memberId).FirstOrDefaultAsync();

                int sumSRating = sprintrating.Sum(s => s.SRating);
                int countSRating = sprintrating.Count;
                if (countSRating == 0)
                {
                    return BadRequest("There is no Sprint between this Time period");
                }
                decimal avr = sumSRating / countSRating;
                ViewBag.BonusRating = avr;
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;
                return View(Member);
            }
            return NotFound(model);
        }
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id == null || _context.Sprint == null)
            {
                return NotFound();
            }

            var sprints = await _context.Sprint
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sprints == null)
            {
                return NotFound();
            }

            return View(sprints);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSprint(int id)
        {
            string userId = null;
            if (User.Identity.IsAuthenticated)
            {

                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            int userId1;
            int.TryParse(userId, out userId1);
            if (_context.Sprint == null)
            {
                return Problem("Entity set 'TeamManageContext.Sprint'  is null.");
            }
            var sprints = await _context.Sprint.Where(s => s.CreatedBy == userId1).ToListAsync();
            var sprint = await _context.Sprint.FindAsync(id);
            if (sprint != null)
            {
                var filteredSprints = sprints.Where(s => s.CreatedTime > sprint.CreatedTime).ToList();
                foreach (var spr in filteredSprints)
                {
                    spr.SprintNo -= 1;
                }
                var sprintRatings = _context.SprintRating.Where(sr => sr.SprintId == id).ToList();
                // Remove the related SprintRating records from the context
                _context.SprintRating.RemoveRange(sprintRatings);
                _context.Sprint.Remove(sprint);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ViewSprints));
        }

        public IActionResult AddFeedback()
        {
            return View();
        }

        [HttpPost]
        [Route("Dashboard/AddFeedback")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFeedback(Rating model)
        {

            int member = model.MemberId;
            var rating = await _context.Rating
                     .FirstOrDefaultAsync(r => r.MemberId == member);
            if (rating == null)
            {
                return BadRequest("This member have no ratings! Firstly add sprint ratings then add Feedback");

            }
            rating.FeedBack = model.FeedBack;
            await _context.SaveChangesAsync();
            return RedirectToAction("ViewTeam", "Dashboard");
        }

        public IActionResult FinalReport(int id)
        {
            var sprintrating = _context.SprintRating.Where(s => s.Mid == id && s.IsDelete==0).ToList();
            if (sprintrating == null)
            {
                return BadRequest("This Member have no sprint ratings");
            }
            var rating = _context.Rating.FirstOrDefault(r => r.MemberId == id);
            if (rating == null)
            {
                return BadRequest("This Member have no average sprint rating");
            }
            var sprintIds = sprintrating.Select(s => s.SprintId).ToList();
            var sprints = _context.Sprint.Where(s => sprintIds.Contains(s.Id)).ToList();
            /*var sprintRatingViewModels = sprintrating
            .Join(sprints, sr => sr.SprintId, s => s.Id, (sr, s) => new SprintRatingViewModel
            {
                SprintId = sr.SprintId,
                SprintName = s.Title,
                Mname = sr.Mname,
                SRating = sr.SRating
            }).ToList();*/
            var report = new Report
            {
                SprintRatingModel = sprintrating,
                RatingModel = new Rating
                {
                    MemberName = rating.MemberName,
                    Ratings = rating.Ratings,
                    FeedBack = rating.FeedBack,
                }
            };
            return View(report);
        }

        public async Task<IActionResult> DeleteMember(int id)
        {
            if (id == null || _context.TMembers == null)
            {
                return NotFound();
            }

            var members = await _context.TMembers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (members == null)
            {
                return NotFound();
            }

            return View(members);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteM(int id)
        {
            if (_context.TMembers == null)
            {
                return Problem("Entity set 'TeamManageContext.Member'  is null.");
            }
            var member = await _context.TMembers.FindAsync(id);
            if (member != null)
            {
                var sprintRatings = _context.SprintRating.Where(sr => sr.Mid == id).ToList();

                // Remove the related SprintRating records from the context
                _context.SprintRating.RemoveRange(sprintRatings);
                _context.TMembers.Remove(member);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ViewTeam));
        }
        public async Task<IActionResult> EditMembers(int? id)
        {
            if (id == null || _context.TMembers == null)
            {
                return NotFound();
            }

            var members = await _context.TMembers.FindAsync(id);
            if (members == null)
            {
                return NotFound();
            }
            return View(members);
        }

        public async Task<IActionResult> EditSprint(int? id)
        {
            if (id == null || _context.Sprint == null)
            {
                return NotFound();
            }
            var sprint = await _context.Sprint.FindAsync(id);
            if (sprint == null)
            {
                return NotFound();
            }
            return View(sprint);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSprint(Sprint model)
        {
            var sprint = _context.Sprint.FirstOrDefault(s => s.Id == model.Id);
            if (model.Id != sprint.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                sprint.Title = model.Title;
                sprint.SDate = model.SDate;
                sprint.EDate = model.EDate;
                sprint.Discription = model.Discription;
                _context.Update(sprint);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(ViewSprints));
            }
            return View(sprint);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMembers(TMembers model)
        {
            var members = _context.TMembers.FirstOrDefault(m => m.Id == model.Id);
            if (model.Id != members.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                members.TMname = model.TMname;
                members.Salary = model.Salary;
                members.Role = model.Role;
                _context.Update(members);
                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(ViewTeam));
            }
            return View(members);
        }
        [HttpGet]
        public IActionResult RemoveMember(int sprintId)
        {
            var sprintrating = _context.SprintRating.Where(s => s.SprintId == sprintId && s.IsDelete == 0).ToList();
            string userId = null;
            if (User.Identity.IsAuthenticated)
            {

                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            int userId1;
            int.TryParse(userId, out userId1);
            if (sprintId == null)
            {
                return NotFound();
            }
            else if (sprintrating.Count == 0)
            {
                var member = _context.TMembers.Where(m => m.LeadId == userId1).ToList();
                var viewModel = new RemoveMemberViewModel
                {
                    MemberNames = member.Select(m => m.TMname).ToList()
                };
                return View(viewModel);
            }
            else
            {
                var viewModel = new RemoveMemberViewModel
                {
                    SprintRating = sprintrating
                };
                return View(viewModel);
            }
            return View(sprintrating);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveMember(int SprintID, string selectedMember)
        {
            if (SprintID == null || selectedMember == null)
            {
                return NotFound(); // Or handle the error appropriately based on your requirements
            }
            else
            {
                var member = _context.TMembers.Where(m => m.TMname == selectedMember).SingleOrDefault();
                var sprintrating = _context.SprintRating.Where(s => s.SprintId == SprintID && s.Mname == selectedMember).SingleOrDefault();
                if (sprintrating == null)
                {
                    var Srating = new SprintRating
                    {
                        SprintId = SprintID,
                        Mname = selectedMember,
                        Mid = member.Id,
                        IsDelete = 1,
                        SRating = 0,
                    };
                    _context.Add(Srating);
                }
                else
                {
                    sprintrating.IsDelete = 1;
                    _context.Update(sprintrating);

                }
                _context.SaveChanges();
                return RedirectToAction(nameof(ViewSprints));
            }
            // string sprintId = Request.Form["SprintID"];
            // int Sid;
            // int.TryParse(sprintId, out Sid);
            /* var sprintrating = _context.SprintRating.Where(s => s.SprintId == SprintID && s.Mid == memberId).FirstOrDefault();
             if (sprintrating != null)
             {
                 sprintrating.IsDelete = 1;
                 _context.Update(sprintrating);
                  _context.SaveChanges();

                 return RedirectToAction(nameof(ViewSprints));
             }*/
            return View();
        }
        [HttpGet]
        public IActionResult AddsprintMember(int sprintId)
        {
            string userId = null;
            if (User.Identity.IsAuthenticated)
            {

                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            int userId1;
            int.TryParse(userId, out userId1);
            var sprintrating = _context.SprintRating.Where(s => s.SprintId == sprintId && s.IsDelete == 1).ToList();
            var rating = _context.SprintRating.Where(s => s.SprintId == sprintId).ToList();
            var member = _context.TMembers.Where(m => m.LeadId == userId1).ToList();

            if (sprintId == null)
            {
                return NotFound();
            }
            if (rating.Count == 0)
            {
                string message = "All the members are already Added, You don't have any member to add! ";
                ViewBag.Message = message;
                var viewModel = new RemoveMemberViewModel
                {
                    SprintRating = new List<SprintRating>(), // Empty list
                };

                return View(viewModel);
            }
            if (rating.Count == member.Count)
            {
                if (sprintrating.Count == 0)
                {
                    string message = "All the members are already Added, You don't have any member to add! ";
                    ViewBag.Message = message;
                    var viewModel = new RemoveMemberViewModel
                    {
                        SprintRating = new List<SprintRating>(), // Empty list
                    };

                    return View(viewModel);
                }
                else
                {
                    var viewModel = new RemoveMemberViewModel
                    {
                        SprintRating = sprintrating
                    };
                    return View(viewModel);
                }
            }
            else
            {
                var sprintRatingMemberNames = rating.Select(s => s.Mname).ToList();

                // Get the team members whose names are not in the sprintRatingMemberNames list
                var unmatchedTeamMembers = member
                    .Where(m => !sprintRatingMemberNames.Contains(m.TMname))
                    .ToList();
                foreach (var item in unmatchedTeamMembers)
                {
                    var data = new SprintRating
                    {
                        Mid = item.Id,
                        Mname = item.TMname,
                        IsDelete = 1,
                        SRating = 0,
                        SprintId = sprintId,
                    };
                    _context.Add(data);
                }
                _context.SaveChanges();
                var Srating = _context.SprintRating.Where(s => s.IsDelete == 1 && s.SprintId == sprintId).ToList();
                var viewModel = new RemoveMemberViewModel
                {
                    SprintRating = Srating
                };
                return View(viewModel);
            }
            return View(sprintrating);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddsprintMember(int SprintID, string selectedMember)
        {
            if (SprintID == null || selectedMember == null)
            {
                return NotFound(); // Or handle the error appropriately based on your requirements
            }
            else
            {
                var member = _context.TMembers.Where(m => m.TMname == selectedMember).SingleOrDefault();
                var sprintrating = _context.SprintRating.Where(s => s.SprintId == SprintID && s.Mname == selectedMember).SingleOrDefault();
                if (sprintrating != null)
                {
                    sprintrating.IsDelete = 0;
                    _context.Update(sprintrating);
                }
                else
                {
                    return NotFound();
                }
                _context.SaveChanges();
                return RedirectToAction("AddScores", new { id = SprintID });
            }
            return View();
        }
        //Admin Pannel Controller Actions 
        public IActionResult ADashboard()
        {

            string userId = null;
            if (User.Identity.IsAuthenticated)
            {

                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            int userId1;
            int.TryParse(userId, out userId1);
            var users = _context.User.Where(u => u.Id == userId1).SingleOrDefault();
            if (users != null)
            {
                if (users.Role == "Admin")
                {
                    var members = _context.TMembers.ToList();
                    var sprints = _context.Sprint.ToList();
                    var teams = _context.User.Where(u => u.Role != users.Role).ToList();

                    var dashboard = new Dashboard
                    {
                        MembersModel = members,
                        SprintsModel = sprints,
                        UsersModel = teams,
                        usermodel = users,
                    };
                    return View(dashboard);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
            }
        }
        public IActionResult ViewAllMembers()
        {

            string userId = null;
            if (User.Identity.IsAuthenticated)
            {

                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            int userId1;
            int.TryParse(userId, out userId1);
            var users = _context.User.Where(u => u.Id == userId1).SingleOrDefault();
            if (users != null)
            {
                if (users.Role == "Admin")
                {
                    var teamMembers = _context.TMembers.ToList();
                    var ratings = _context.Rating.ToList();
                    var teamratings = new TeamRating
                    {
                        TeamMembers = teamMembers,
                        TeamRatings = ratings,
                    };

                    return View(teamratings);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
            }
        }
        public IActionResult ViewAllSprints()
        {
            try
            {
                string userId = null;
                if (User.Identity.IsAuthenticated)
                {

                    userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                }
                int userId1;
                int.TryParse(userId, out userId1);
                var users = _context.User.Where(u => u.Id == userId1).SingleOrDefault();
                if (users.Role == "Admin")
                {
                    var sprints = _context.Sprint.ToList();
                    var members = _context.TMembers.ToList();
                    var sprintRatings = _context.SprintRating.ToList()
                          .Join(
                              inner: members,
                              outerKeySelector: sr => sr.Mid,
                              innerKeySelector: member => member.Id,
                              resultSelector: (sr, member) => sr)
                          .ToList();
                    var mainsprints = new MainSprints
                    {
                        Sprints = sprints,
                        TeaMembers = members,
                        SprintRatings = sprintRatings,
                    };

                    return View(mainsprints);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "An error occurred while retrieving Sprints";
                return View(new List<Sprint>());
            }

        }
        [HttpGet]
        public IActionResult FilteredUserSprints(DateTime startDate, DateTime endDate, string userId)
        {
            string userIds = null;
            if (User.Identity.IsAuthenticated)
            {

                userIds = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            int userId1;
            int userId2;
            int.TryParse(userIds, out userId1);
            int.TryParse(userId, out userId2);
            var users = _context.User.Where(u => u.Id == userId1).SingleOrDefault();
            if (users.Role == "Admin")
            {
                var sprints = _context.Sprint.Where(r => (r.SDate <= startDate && r.EDate <= endDate && r.EDate >= startDate) ||
                    (r.SDate >= startDate && r.EDate >= endDate && r.SDate <= endDate) ||
                    (r.SDate >= startDate && r.EDate <= endDate) ||
                    (r.SDate <= startDate && r.EDate >= endDate))
        .Where(r => r.CreatedBy == userId2)
        .ToList();
                var members = _context.TMembers.Where(m => m.LeadId == userId2).ToList();
                var sprintRatings = _context.SprintRating.ToList()
                      .Join(
                          inner: members,
                          outerKeySelector: sr => sr.Mid,
                          innerKeySelector: member => member.Id,
                          resultSelector: (sr, member) => sr)
                      .ToList();
                var mainsprints = new MainSprints
                {
                    Sprints = sprints,
                    TeaMembers = members,
                    SprintRatings = sprintRatings,
                };

                return PartialView("_PartialForSpecificUser", mainsprints);
            }
            else
            {
                return NotFound();
            }
            return View();    
        }
        [HttpGet]
        public IActionResult FilteredAllSprints(DateTime startDate, DateTime endDate)
        {
            string userId = null;
            if (User.Identity.IsAuthenticated)
            {

                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            int userId1;
            int.TryParse(userId, out userId1);
            var users = _context.User.Where(u => u.Id == userId1).SingleOrDefault();
            if (users.Role == "Admin")
            {
                var sprints = _context.Sprint.Where(r => (r.SDate <= startDate && r.EDate <= endDate && r.EDate >= startDate) ||
                     (r.SDate >= startDate && r.EDate >= endDate && r.SDate <= endDate) ||
                     (r.SDate >= startDate && r.EDate <= endDate) ||
                     (r.SDate <= startDate && r.EDate >= endDate)).ToList();
                var members = _context.TMembers.ToList();
                var sprintRatings = _context.SprintRating.ToList()
                      .Join(
                          inner: members,
                          outerKeySelector: sr => sr.Mid,
                          innerKeySelector: member => member.Id,
                          resultSelector: (sr, member) => sr)
                      .ToList();
                var mainsprints = new MainSprints
                {
                    Sprints = sprints,
                    TeaMembers = members,
                    SprintRatings = sprintRatings,
                };

                return PartialView("_AllSprintPartial", mainsprints);
            }
            else
            {
                return NotFound();
            }
        }
        /* public IActionResult ViewAllUsers()
         {
             string userId = null;
             // Get the logged-in user's UserId (replace this with your actual method of getting the UserId)
             userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

             int userId1;
             int.TryParse(userId, out userId1);
             var user = _context.User.Where(u => u.Id == userId1).SingleOrDefault();
             if(user.Role=="Admin")
             {
                 var assigneeList = _context.User.Where(m => m.Role!="Admin").Select(m => new { m.Id, m.Username }).ToList();

                 // You can use ViewData or a ViewModel to pass the list to the view
                 ViewData["AssigneeList"] = new SelectList(assigneeList, "Id", "Username"); // Assuming the member model has properties "Id" and "Name"

                 return View();
             }
             else
             {
                 return NotFound();
             }
             // Retrieve the list of members whose LeadId matches the logged-in UserId
         }
        */
        public IActionResult NViewUserSprint()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ViewUserSprint(int id)
        {
            try
            {
                string userId = null;
                if (User.Identity.IsAuthenticated)
                {

                    userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                }
                int userId1;
                int.TryParse(userId, out userId1);
                var users = _context.User.Where(u => u.Id == userId1).SingleOrDefault();
                if (users.Role == "Admin")
                {
                    var sprints = _context.Sprint.Where(s => s.CreatedBy == id).ToList();
                    var members = _context.TMembers.ToList();
                    var sprintRatings = _context.SprintRating.ToList()
                          .Join(
                              inner: members,
                              outerKeySelector: sr => sr.Mid,
                              innerKeySelector: member => member.Id,
                              resultSelector: (sr, member) => sr)
                          .ToList();
                    var mainsprints = new MainSprints
                    {
                        Sprints = sprints,
                        TeaMembers = members,
                        SprintRatings = sprintRatings,
                    };
                    return View(mainsprints);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "An error occurred while adding/updating the rating.";
                return RedirectToAction("ViewSprint", "Dashboard");
            }
        }
        [HttpPost]
        public IActionResult UpdateRole(string role, string username)
        {
            string userId = null;
            if (User.Identity.IsAuthenticated)
            {

                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            int userId1;
            int.TryParse(userId, out userId1);
            var users = _context.User.Where(u => u.Id == userId1).SingleOrDefault();
            if(users.Role=="Admin")
            {
                var record = _context.User.Where(r => r.Username == username).SingleOrDefault();
                record.Role = role;
                _context.Update(record);
                _context.SaveChanges();
                return RedirectToAction("ViewAllUser");
            }
            return View();
        }
        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            string userId = null;
            if (User.Identity.IsAuthenticated)
            {

                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            int userId1;
            int.TryParse(userId, out userId1);
            var users = _context.User.Where(u => u.Id == userId1).SingleOrDefault();
            if (users.Role == "Admin")
            {
                if (_context.User == null)
                {
                    return Problem("Entity set 'TeamManageContext.Member'  is null.");
                }
                var leader = _context.User.FirstOrDefault(u => u.Id == id);
                if (leader != null)
                {
                    var sprints = _context.Sprint.Where(s => s.CreatedBy == id).ToList();
                    var sprintIds = sprints.Select(s => s.Id).ToList();
                    foreach (var sprintId in sprintIds)
                    {
                        var ratingsToDelete = _context.SprintRating.Where(r => r.SprintId == sprintId).ToList();
                        _context.SprintRating.RemoveRange(ratingsToDelete);
                    }
                    // Remove the related SprintRating records from the context
                    _context.Sprint.RemoveRange(sprints);
                    _context.User.Remove(leader);
                    _context.SaveChanges();
                }

                return RedirectToAction(nameof(ViewAllUser));
            }
            return NotFound();
        }
        public IActionResult ViewAllUser()
        {
            string userId = null;
            if (User.Identity.IsAuthenticated)
            {

                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            int userId1;
            int.TryParse(userId, out userId1);
            var users = _context.User.Where(u => u.Id == userId1).SingleOrDefault();
            if (users.Role == "Admin")
            {
                var user = _context.User.Where(u => u.Role != "Admin").ToList();
                return View(user);
            }
            else
            {
                return NotFound();
            }
        }
        public IActionResult DownloadReport([FromQuery(Name = "membersJson")] string membersJson)
        {
            var members = JsonConvert.DeserializeObject<TeamRating>(membersJson);

            // Create a new PDF document
            var pdfDocument = new PdfDocument();

            // Create a new page in the document
            var pdfPage = pdfDocument.AddPage();
            var gfx = XGraphics.FromPdfPage(pdfPage);

            // Define fonts and styles
            var font = new XFont("Arial", 12, XFontStyle.Regular);
            var boldfont = new XFont("Arial", 13, XFontStyle.Bold);
            // Set initial y-coordinate for content
            var yPosition = 20;

            // Draw table header
            gfx.DrawString("Member Name", boldfont, XBrushes.Black, new XPoint(40, yPosition));
            gfx.DrawString("Position", boldfont, XBrushes.Black, new XPoint(210, yPosition));
            gfx.DrawString("Salary", boldfont, XBrushes.Black, new XPoint(400, yPosition));
            gfx.DrawString("Rating", boldfont, XBrushes.Black, new XPoint(540, yPosition));

            // Draw table rows
            yPosition += 20;
            foreach (var item in members.TeamMembers)
            {
                gfx.DrawString(item.TMname, font, XBrushes.Black, new XPoint(40, yPosition));
                gfx.DrawString(item.Role, font, XBrushes.Black, new XPoint(210, yPosition));
                gfx.DrawString(item.Salary.ToString(), font, XBrushes.Black, new XPoint(400, yPosition));

                var matchingRating = members.TeamRatings.FirstOrDefault(r => r.MemberName == item.TMname);
                var ratingText = matchingRating != null ? matchingRating.Ratings.ToString() : "No Rating";
                gfx.DrawString(ratingText, font, XBrushes.Black, new XPoint(540, yPosition));

                yPosition += 20;
            }

            // Save the document to a memory stream
            var stream = new MemoryStream();
            pdfDocument.Save(stream);
            stream.Position = 0;

            // Return the PDF file
            return File(stream, "application/pdf", "MembersReport.pdf");
        }

        public IActionResult DownloadExcel([FromQuery(Name = "membersJson")] string membersJson)
        {
            var members = JsonConvert.DeserializeObject<TeamRating>(membersJson);
            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Members");

                //Set column widths
                worksheet.Column(1).Width = 20; // TMname
                worksheet.Column(2).Width = 25; // Role
                worksheet.Column(3).Width = 15; // Salary
                worksheet.Column(4).Width = 10; // Rating

                worksheet.Cells["A1"].Value = "TMname";
                worksheet.Cells["B1"].Value = "Role";
                worksheet.Cells["C1"].Value = "Salary";
                worksheet.Cells["D1"].Value = "Rating";

                var rowIndex = 2;
                foreach (var item in members.TeamMembers)
                {
                    worksheet.Cells[$"A{rowIndex}"].Value = item.TMname;
                    worksheet.Cells[$"B{rowIndex}"].Value = item.Role;
                    worksheet.Cells[$"C{rowIndex}"].Value = item.Salary;

                    var matchingRating = members.TeamRatings.FirstOrDefault(r => r.MemberName == item.TMname);
                    if (matchingRating != null)
                    {
                        worksheet.Cells[$"D{rowIndex}"].Value = matchingRating.Ratings;
                    }
                    else
                    {
                        worksheet.Cells[$"D{rowIndex}"].Value = "No Rating";
                    }

                    rowIndex++;
                }

                package.Save();
            }

            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "MembersReport.xlsx");
        }
        public IActionResult DeleteSprintMember(int SprintId, int MemberId)
        {
            if (SprintId == null || MemberId == null)
            {
                return NotFound(); // Or handle the error appropriately based on your requirements
            }
            else
            {
                var member = _context.TMembers.Where(m => m.Id == MemberId).SingleOrDefault();
                var sprintrating = _context.SprintRating.Where(s => s.SprintId == SprintId && s.Mid == MemberId).SingleOrDefault();
                if (sprintrating == null)
                {
                    var Srating = new SprintRating
                    {
                        SprintId = SprintId,
                        Mname = member.TMname,
                        Mid = MemberId,
                        IsDelete = 1,
                        SRating = 0,
                    };
                    _context.Add(Srating);
                }
                else
                {
                    sprintrating.IsDelete = 1;
                    _context.Update(sprintrating);

                }
                _context.SaveChanges();
                return RedirectToAction("AddScores", new { id = SprintId });
            }
        }
        /* public IActionResult GenerateSprintPdf([FromQuery(Name = "membersJson")] string membersJson)
         {
             var model = JsonConvert.DeserializeObject<MainSprints>(membersJson);
             var document = new PdfDocument();
             var page = document.AddPage();
             var graphics = XGraphics.FromPdfPage(page);
             var font = new XFont("Arial", 12, XFontStyle.Regular);
             var yPosition = 20;

             foreach (var item in model.Sprints)
             {
                 // Draw Sprint information and ratings
                 foreach (var member in model.TeaMembers)
                 {
                     var sprintRating = model.SprintRatings?.FirstOrDefault(sr => sr.Mid == member.Id && sr.SprintId == item.Id && sr.IsDelete == 0);
                     var ratingText = sprintRating != null ? sprintRating.SRating.ToString() : "No Rating";

                     graphics.DrawString(member.TMname, font, XBrushes.Black, new XPoint(20, yPosition)); // Member Name
                     graphics.DrawString(item.SprintNo.ToString(), font, XBrushes.Black, new XPoint(120, yPosition)); // Sprint No
                     graphics.DrawString(member.Salary.ToString(), font, XBrushes.Black, new XPoint(220, yPosition)); // Salary
                     graphics.DrawString(ratingText, font, XBrushes.Black, new XPoint(320, yPosition)); // Rating

                     yPosition += 20;
                 }

                 // Draw Sprint average rating and description
                 var filteredRatings = model.SprintRatings.Where(sr => sr.SprintId == item.Id && sr.SRating > 0);
                 if (filteredRatings.Any())
                 {
                     var averageSprintRating = filteredRatings.Average(sr => sr.SRating);
                     graphics.DrawString("Average Sprint Rating:", font, XBrushes.Black, new XPoint(20, yPosition));
                     graphics.DrawString($"{averageSprintRating:F2}", font, XBrushes.Black, new XPoint(320, yPosition));
                     yPosition += 20;
                 }

                 graphics.DrawString("Sprint Description:", font, XBrushes.Black, new XPoint(20, yPosition));
                 yPosition += 20;
                 graphics.DrawString(item.Discription, font, XBrushes.Black, new XPoint(20, yPosition));

                 yPosition += 20; // Move to the next line
             }

             // Save the document to a memory stream
             var stream = new MemoryStream();
             document.Save(stream);
             stream.Position = 0;

             return File(stream, "application/pdf", "SprintsReport.pdf");
         }*/
        // ... other actions and methods ...
    }



        /*  public IActionResult AddTasks(int? id)
       {
           string userId = null;
           // Get the logged-in user's UserId (replace this with your actual method of getting the UserId)
           userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

           int userId1;
           int.TryParse(userId, out userId1);

           // Retrieve the list of members whose LeadId matches the logged-in UserId
           var assigneeList = _context.TMembers.Where(m => m.LeadId == userId1).Select(m => new { m.Id, m.TMname }).ToList();

           // You can use ViewData or a ViewModel to pass the list to the view
           ViewData["AssigneeList"] = new SelectList(assigneeList, "Id","TMname"); // Assuming the member model has properties "Id" and "Name"

           return View();
       }

       [HttpPost]
       [Route("Dashboard/AddTasks")]
       [ValidateAntiForgeryToken]
       public IActionResult AddTasks(Models.Account.Task Model)
       {
           try
           {
               int sprintid = int.Parse(Request.Form["Id"]);
               //int assignid = Model.AssigneeId;
               var assigned = _context.TMembers.FirstOrDefault(a => a.Id == Model.AssigneeId);
               string assigname;
               if (assigned != null)
               {
                   assigname = assigned.TMname;
                   var tasks = new Task()
                   {
                       SprintId = sprintid,
                       Title = Model.Title,
                       AssigneeName = assigname,
                       AssigneeId = Model.AssigneeId,
                   };
                   _context.Task.Add(tasks);
                   _context.SaveChanges();
                   return RedirectToAction("ViewSprint", "Dashboard");
               }
               return RedirectToAction("ViewSprint", "Dashboard");
           }
           catch (Exception ex)
           {
               TempData["errorMessage"] = "An error occurred while adding/updating the rating.";
               return RedirectToAction("ViewSprint", "Dashboard");
           }


       }*/

        /*  public async Task<IActionResult> ViewTasks(int? id)
          {
              var tasks = await _context.Task.Where(t => t.SprintId == id).ToListAsync();
             // var member = awai _context .Task.Where(t=)
              return View(tasks);
          }*/
        /*public IActionResult ViewTeam()
    {
    var teamMembers = _context.TMembers.ToList();

    return View(teamMembers);
        }*/


        /* public IActionResult AddTaskRating()
         {
             return View();
         }

         [HttpPost]
         [Route("Dashboard/AddTaskRating")]
         [ValidateAntiForgeryToken]
         public IActionResult AddTaskRating(Task model)
         {

             int id = int.Parse(Request.Form["Id"]);
            Task existingRating = _context.Task.FirstOrDefault(r => r.Id == id);
             if (existingRating != null)
             {
                 // If a rating record already exists, update the existing one
                 existingRating.TaskRate = model.TaskRate;

             }
             _context.SaveChanges();
             return RedirectToAction("ViewSprint", "Dashboard");

         }*/
        /* public IActionResult AddRating()
       {
           return View(new Rating());
       }*/

        /*[HttpPost]
        [Route("Dashboard/AddRating")]
        [ValidateAntiForgeryToken]
        public IActionResult AddRating(Rating Model)
        {
            try
            {
                int memberId = int.Parse(Request.Form["Id"]);
                string memberName = Request.Form["TMname"];

                Rating existingRating = _context.Rating.FirstOrDefault(r => r.MemberId == memberId);
                if (existingRating != null)
                {
                    // If a rating record already exists, update the existing one
                    existingRating.Ratings = Model.Ratings;
                    existingRating.FeedBack = Model.FeedBack;
                }
                else
                {
                    var data = new Rating()
                    {
                        MemberId = memberId,
                        MemberName = memberName,
                        Ratings = Model.Ratings,
                        FeedBack = Model.FeedBack,
                    };
                    _context.Rating.Add(data);
                }
                _context.SaveChanges();
                return RedirectToAction("ViewTeam", "Dashboard");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "An error occurred while adding/updating the rating.";
                return RedirectToAction("ViewTeam", "Dashboard");
            }
        }*/
        /*
          // int memberId = int.Parse(Request.Form["id"]);
            if (!int.TryParse(Request.Form["id"], out int memberId))
            {
                return NotFound(); // Or handle the error appropriately based on your requirements
            }
            if (memberId == null)
            {
                return NotFound();
            }

            var rating = await _context.Rating
                .FirstOrDefaultAsync(r => r.MemberId == memberId);
            string userId = null;
            if (User.Identity.IsAuthenticated)
            {

                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            int userId1;
            int.TryParse(userId, out userId1);
            if (ModelState.IsValid)
            {
                DateTime startDate = model.StartDate;
                DateTime endDate = model.EndDate;

                var sprints = await _context.Sprint
                .Where(r => r.SDate >= startDate && r.EDate <= endDate && r.CreatedBy == userId1)
                .ToListAsync();
                var sprintIds = sprints.Select(s => s.Id).ToList();
                if (rating == null)
                {
                    var sprintrating = await _context.SprintRating.Where(s => s.Mid == memberId && sprintIds.Contains(s.SprintId)).ToListAsync();
                    var Member = await _context.TMembers.Where(m => m.Id == memberId).FirstOrDefaultAsync();
                    string mname = Member.TMname;
                    int sumSRating = sprintrating.Sum(s => s.SRating);
                    int countSRating = sprintrating.Count;
                    if(countSRating==0)
                    {
                        return BadRequest("There is no Sprint between this Time period");
                    }
                    decimal avr = sumSRating / countSRating;
                    var data = new Rating()
                    {
                        MemberId = Member.Id,
                        MemberName = mname,
                        Ratings = (int)avr,
                        FeedBack = "",
                    };

                    _context.Rating.Add(data);
                    _context.SaveChanges();
                    return View(data);
                }

                var sprintrate = await _context.SprintRating.Where(s => s.Mid == memberId && sprintIds.Contains(s.SprintId)).ToListAsync();
                int sumSRate = sprintrate.Sum(s => s.SRating);
                int countSRate = sprintrate.Count;
                if (countSRate == 0)
                {

                   return BadRequest("There is no Sprint between this Time period");
                }
                decimal avrs = sumSRate / countSRate;
                rating.Ratings = (int)avrs;
                _context.SaveChanges();
                return View(rating);
            }
            return NotFound(model);
         */
}

