using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Services
{
    public interface IDialogService
    {
        Task Show(string title, string msg, string closeText);
    }
}
