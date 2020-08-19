using System;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Domain.Members;
using Atlas.Domain.Members.Commands;
using Atlas.Models;
using Atlas.Models.Admin.Members;
using Atlas.Server.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.Server.Controllers.Admin
{
    [Route("api/admin/members")]
    public class MembersController : AdminControllerBase
    {
        private readonly IContextService _contextService;
        private readonly IMemberService _memberService;
        private readonly IMemberRules _memberRules;
        private readonly IMemberModelBuilder _modelBuilder;
        private readonly UserManager<IdentityUser> _userManager;

        public MembersController(IContextService contextService,
            IMemberService memberService,
            IMemberRules memberRules,
            IMemberModelBuilder modelBuilder, 
            UserManager<IdentityUser> userManager)
        {
            _contextService = contextService;
            _memberService = memberService;
            _memberRules = memberRules;
            _modelBuilder = modelBuilder;
            _userManager = userManager;
        }

        [HttpGet("index-model")]
        public async Task<IndexPageModel> List([FromQuery] int? page = 1, [FromQuery] string search = null, [FromQuery] string status = null)
        {
            return await _modelBuilder.BuildIndexPageModelAsync(new QueryOptions(search, page), status);
        }

        [HttpGet("create")]
        public async Task<CreatePageModel> Create()
        {
            return await _modelBuilder.BuildCreatePageModelAsync();
        }

        [HttpPost("save")]
        public async Task<ActionResult> Save(CreatePageModel.UserModel model)
        {
            if (!ModelState.IsValid) return BadRequest();

            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var createResult = await _userManager.CreateAsync(user, model.Password);

            if (!createResult.Succeeded) return BadRequest();

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmResult = await _userManager.ConfirmEmailAsync(user, code);

            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var command = new CreateMember
            {
                UserId = user.Id,
                Email = user.Email,
                SiteId = site.Id,
                MemberId = member.Id
            };

            await _memberService.CreateAsync(command);

            return Ok(command.Id);
        }

        [HttpGet("edit/{id}")]
        public async Task<ActionResult<EditPageModel>> Edit(Guid id)
        {
            var result = await _modelBuilder.BuildEditPageModelAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpGet("edit-by-user-id/{id}")]
        public async Task<ActionResult<EditPageModel>> EditByUserId(string id)
        {
            var result = await _modelBuilder.BuildEditPageModelAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpPost("update")]
        public async Task<ActionResult> Update(EditPageModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var user = await _userManager.FindByIdAsync(model.Info.UserId);

            if (user != null && model.Roles.Count > 0)
            {
                foreach (var role in model.Roles)
                {
                    if (role.Selected)
                    {
                        if (!await _userManager.IsInRoleAsync(user, role.Name))
                        {
                            await _userManager.AddToRoleAsync(user, role.Name);
                        }
                    }
                    else
                    {
                        if (await _userManager.IsInRoleAsync(user, role.Name))
                        {
                            await _userManager.RemoveFromRoleAsync(user, role.Name);
                        }
                    }
                }
            }

            var command = new UpdateMember
            {
                Id = model.Member.Id,
                DisplayName = model.Member.DisplayName,
                SiteId = site.Id,
                MemberId = member.Id,
                Roles = model.Roles.Where(x => x.Selected).Select(x => x.Name).ToList()
            };

            await _memberService.UpdateAsync(command);

            return Ok();
        }

        [HttpGet("activity/{id}")]
        public async Task<ActionResult<ActivityPageModel>> Activity(Guid id, [FromQuery] int? page = 1, [FromQuery] string search = null)
        {
            var site = await _contextService.CurrentSiteAsync();

            var result = await _modelBuilder.BuildActivityPageModelAsync(site.Id, id, new QueryOptions(search, page));

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpPost("suspend")]
        public async Task<ActionResult> Suspend([FromBody] Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var command = new SuspendMember
            {
                Id = id,
                SiteId = site.Id,
                MemberId = member.Id
            };

            await _memberService.SuspendAsync(command);

            return Ok();
        }

        [HttpPost("reinstate")]
        public async Task<ActionResult> Reinstate([FromBody] Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var command = new ReinstateMember
            {
                Id = id,
                SiteId = site.Id,
                MemberId = member.Id
            };

            await _memberService.ReinstateAsync(command);

            return Ok();
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var command = new DeleteMember
            {
                Id = id,
                SiteId = site.Id,
                MemberId = member.Id
            };

            var userId = await _memberService.DeleteAsync(command);

            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            return Ok();
        }

        [HttpGet("is-display-name-unique/{name}")]
        public async Task<IActionResult> IsDisplayNameUnique(string name)
        {
            var isNameUnique = await _memberRules.IsDisplayNameUniqueAsync(name);
            return Ok(isNameUnique);
        }

        [HttpGet("is-display-name-unique/{name}/{id}")]
        public async Task<IActionResult> IsNameUnique(string name, Guid id)
        {
            var isNameUnique = await _memberRules.IsDisplayNameUniqueAsync(name, id);
            return Ok(isNameUnique);
        }
    }
}
