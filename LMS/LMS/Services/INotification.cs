using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Services
{
    public interface INotification
    {
        void CreateNotification(String title, String message);
    }
}
