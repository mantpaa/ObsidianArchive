using ObsidianArchive.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObsidianArchive.Business.Services.IServices
{
    public interface IApplicationUserService
    {
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
    }
}
