using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTHK_Link.Services.Interfaces;
using TTHK_Link.Models;

namespace TTHK_Link.Services.Fake;

public class FakeGroupService : IGroupService
{
    private readonly IAuthService _authService;

    public FakeGroupService(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<IReadOnlyList<Group>> GetGroupsForCurrentUserAsync()
    {
        var user = _authService.CurrentUser;

        // only one fake group for the user
        var groups = new List<Group>
        {
            new Group
            {
                Id = Guid.NewGuid().ToString(),
                GroupNameId = user.GroupNameId,  // "TiTge24"
                Title = "Programmeerimine",
                Description = "Chat selle aine jaoks"
            }
        };

        return Task.FromResult<IReadOnlyList<Group>>(groups);
    }
}
