using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.RazorPages;

namespace Volo.Abp.Permissions.Web.Pages.AbpPermissions
{
    public class PermissionManagementModal : AbpPageModel
    {
        private readonly IPermissionAppService _permissionAppService;

        [Required]
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public string ProviderName { get; set; }

        [Required]
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public string ProviderKey { get; set; }

        [BindProperty]
        public List<PermissionGroupViewModel> Groups { get; set; }

        public PermissionManagementModal(IPermissionAppService permissionAppService)
        {
            _permissionAppService = permissionAppService;
        }

        public async Task OnGetAsync()
        {
            var result = await _permissionAppService.GetAsync(ProviderName, ProviderKey);
            Groups = ObjectMapper.Map<List<PermissionGroupDto>, List<PermissionGroupViewModel>>(result.Groups);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ValidateModel();

            var updatePermissionDtos = Groups
                .SelectMany(g => g.Permissions)
                .Select(p => new UpdatePermissionDto
                {
                    Name = p.Name,
                    IsGranted = p.IsGranted
                })
                .ToArray();

            await _permissionAppService.UpdateAsync(
                ProviderName,
                ProviderKey,
                new UpdatePermissionsDto
                {
                    Permissions = updatePermissionDtos
                }
            );

            return NoContent();
        }

        public class PermissionGroupViewModel
        {
            public string Name { get; set; }

            public string DisplayName { get; set; }

            public List<PermissionGrantInfoViewModel> Permissions { get; set; }
        }

        public class PermissionGrantInfoViewModel
        {
            [Required]
            [HiddenInput]
            public string Name { get; set; }

            public string DisplayName { get; set; }

            public string ParentName { get; set; }

            public bool IsGranted { get; set; }

            public List<ProviderInfoViewModel> Providers { get; set; }
        }

        public class ProviderInfoViewModel
        {
            public string ProviderName { get; set; }

            public string ProviderKey { get; set; }
        }
    }
}