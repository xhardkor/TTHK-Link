using System.Collections.Generic;
using System.Threading.Tasks;
using TTHK_Link.Models;

namespace TTHK_Link.Services.Interfaces;

public interface IGroupService
{
    Task<IReadOnlyList<Group>> GetGroupsForCurrentUserAsync();
}
