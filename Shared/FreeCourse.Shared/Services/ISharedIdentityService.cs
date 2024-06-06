using System;
using System.Collections.Generic;
using System.Text;

namespace FreeCourse.Shared.Services
{
    public interface ISharedIdentityService
    {
        string GetUserId {  get; }
    }
}
