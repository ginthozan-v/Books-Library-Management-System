using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace LMS.Models
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection();
    }
}
