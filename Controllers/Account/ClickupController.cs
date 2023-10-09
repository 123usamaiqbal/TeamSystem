using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http;
using System.Security.Claims;
using TeamManageSystem.Data;
using TeamManageSystem.Models.Account;
using TeamManageSystem.Models.ClickupModels;
using System.Text;
using TeamManageSystem.Models.ViewModel;

namespace TeamManageSystem.Controllers.Account
{
    [Authorize]
    public class ClickupController : Controller
    {
        private readonly HttpClient httpClient;
        //private readonly UserManager<IdentityUser> _userManager;
        private readonly TeamManageContext _context;
        public ClickupController(TeamManageContext context)
        {
            
            this.httpClient = new HttpClient();
            string apiKey = "pk_61720881_AW7PTFGHC9P6NTXDZD1YKVHDBL09V4SL";
            httpClient.DefaultRequestHeaders.Add("Authorization", apiKey);
            _context = context;

        }
        
        [HttpGet]
        public IActionResult GetSpaces()
        {
            var spacerecord = _context.ClickupSpace.ToList();
            _context.ClickupSpace.RemoveRange(spacerecord);
            var list = _context.ClickupLists.ToList();
            _context.ClickupLists.RemoveRange(list);
            var folder = _context.ClickupFolders.ToList();
            _context.ClickupFolders.RemoveRange(folder);
            _context.SaveChanges();

            // Make the GET request
            var response = httpClient.GetAsync($"https://api.clickup.com/api/v2/team/37405723/space").Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Request failed with status code {response.StatusCode}");
            }
                string data = response.Content.ReadAsStringAsync().Result;
                var spaceResponse = JsonConvert.DeserializeObject<SpaceResponse>(data);

                List<ClickupSpace> spaces = spaceResponse.Spaces;
                foreach (var items in spaceResponse.Spaces)
                {
                var record = new ClickupSpace
                        {
                            id = items.id,
                            name = items.name,
                            team_id = "37405723",
                        };
                        _context.ClickupSpace.Add(record);
                }
                _context.SaveChanges();
                var space = _context.ClickupSpace.ToList();
                foreach (var item in space)
                { GetLists(item.id);
                  GetFolders(item.id);
                }
                var folders = _context.ClickupFolders.ToList();
                foreach(var item in folders)
                {
                GetFolderLists(item.id);
                }
                return RedirectToAction("ADashboard", "Dashboard");
            
        }
        public IActionResult GetFolders(string Space_id)
        {
            var response = httpClient.GetAsync($"https://api.clickup.com/api/v2/space/{Space_id}/folder").Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Request failed with status code {response.StatusCode}");
            }
            string data = response.Content.ReadAsStringAsync().Result;
            FolderResponse folderResponse = JsonConvert.DeserializeObject<FolderResponse>(data);
            List<ClickupFolders> folders = folderResponse.Folders;
            foreach (var items in folderResponse.Folders)
            {
                     var record = new ClickupFolders
                     {
                         id = items.id,
                         name = items.name,
                         Spaceid = Space_id,

                     };
                     _context.ClickupFolders.Add(record);
            }
            _context.SaveChanges();
            return Ok();
        }
        public IActionResult GetFolderLists(string folder_id)
        {
            // Make the GET request
            var response = httpClient.GetAsync($"https://api.clickup.com/api/v2/folder/{folder_id}/list").Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Request failed with status code {response.StatusCode}");
            }
            string data = response.Content.ReadAsStringAsync().Result;
            ListResponse listResponse = JsonConvert.DeserializeObject<ListResponse>(data);
            List<ClickupLists> lists = listResponse.Lists;
            var folder = _context.ClickupFolders.Where(f => f.id == folder_id).FirstOrDefault();
            foreach (var items in listResponse.Lists)
            {
                    var record = new ClickupLists
                    {
                        id = items.id,
                        name = items.name,
                        task_count = items.task_count,
                        space_id = folder.Spaceid,

                    };
                    _context.ClickupLists.Add(record);
            }
            _context.SaveChanges();
            return Ok();
        }
        public IActionResult GetLists(string Space_id)
        {
            // Make the GET request
            var response = httpClient.GetAsync($"https://api.clickup.com/api/v2/space/{Space_id}/list").Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Request failed with status code {response.StatusCode}");
            }
            string data = response.Content.ReadAsStringAsync().Result;
            ListResponse listResponse = JsonConvert.DeserializeObject<ListResponse>(data);
            List<ClickupLists> lists = listResponse.Lists;
            foreach (var items in listResponse.Lists)
            {
                    var record = new ClickupLists
                    {
                        id = items.id,
                        name = items.name,
                        task_count = items.task_count,
                        space_id = Space_id,

                    };
                    _context.ClickupLists.Add(record);
            }
            _context.SaveChanges();
            return Ok();
        }
        public IActionResult GetTasks()
        {
            //note time
            var tasksrecord = _context.ClikupTask.ToList();
            if(tasksrecord.Count > 0) { _context.ClikupTask.RemoveRange(tasksrecord); }
            var assigneesRecord = _context.ClickupTaskAssignee.ToList();
            if (assigneesRecord.Count > 0) { _context.ClickupTaskAssignee.RemoveRange(assigneesRecord); }
            _context.SaveChanges();
            var lists = _context.ClickupLists.ToList();
            foreach (var item in lists)
            {
                var response = httpClient.GetAsync($"https://api.clickup.com/api/v2/list/{item.id}/task").Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Request failed with status code {response.StatusCode}");
                }
                string data = response.Content.ReadAsStringAsync().Result;
                TaskResponse taskResponse = JsonConvert.DeserializeObject<TaskResponse>(data);
                List<ClickupTasks> tasks = taskResponse.Tasks;
                foreach (var items in taskResponse.Tasks)
                {
                    string priority;
                    if (items.priority == null)
                    {
                        priority = "0";
                    }
                    else { priority = items.priority.id; }
                    var records = new ClikupTask
                        {
                            id = items.id,
                            name = items.name,
                            description = items.description,
                            statusvalue = items.status.status,
                            color = items.status.color,
                            type = items.status.type,
                            orderindex = items.status.orderindex,
                            date_created = items.date_created,
                            date_updated = items.date_updated,
                            date_closed = items.date_closed,
                            date_done = items.date_done,
                            list_id = item.id,
                            priority = priority,

                        };
                        _context.ClikupTask.Add(records);
                        foreach (var assigne in items.assignees)
                        {
                            var assignee = new ClickupTaskAssignee
                            {
                                assigneeid = assigne.id,
                                username = assigne.username,
                                taskid = items.id,
                            };
                            _context.ClickupTaskAssignee.Add(assignee);
                        }
                }

            }
            _context.SaveChanges();
            return RedirectToAction("ADashboard", "Dashboard");
        }
        [HttpPost]
        public IActionResult DeleteTask(string id)
        {
            var url = $"https://api.clickup.com/api/v2/task/{id}";
            // Make the GET request
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            var response = httpClient.SendAsync(request).Result;    
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Request failed with status code {response.StatusCode}");
            }
            var task = _context.ClikupTask.Where(t => t.id == id).FirstOrDefault();
            _context.ClikupTask.Remove(task);
            _context.SaveChanges();
            return RedirectToAction("ViewSprints", "Dashboard");

        }
        public IActionResult UpdateTask(ClikupTaskViewModel model)
        {
            var url = $"https://api.clickup.com/api/v2/task/{model.id}";

            // Create an object to represent the task update data
            var taskUpdateData = new
            {
                name = model.name,
                description = model.description,
                status = model.statusvalue,
                priority = model.priority 
            };

            // Serialize the task update data to JSON
            var taskUpdateJson = JsonConvert.SerializeObject(taskUpdateData);

            // Create a request to update the task
            var request = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = new StringContent(taskUpdateJson, Encoding.UTF8, "application/json")
            };

            // Send the PUT request to ClickUp
            var response = httpClient.SendAsync(request).Result;

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = response.Content.ReadAsStringAsync();
                throw new Exception($"Request failed with status code {response.StatusCode}");
            }
            var statuscolor = _context.StatusColors.Where( s => s.statusvalue == model.statusvalue ).FirstOrDefault();
            // Update the task information in your database
            var task = _context.ClikupTask.Where(t => t.id == model.id).FirstOrDefault();
            task.name = model.name;
            task.description = model.description;
            task.statusvalue = model.statusvalue;
            task.priority = model.priority.ToString();
            task.color = statuscolor.color;
            _context.SaveChanges();
            return RedirectToAction("ViewSprints", "Dashboard");

        }

    }
}
