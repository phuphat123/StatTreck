using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace App3.Helpers
{
    public interface IConnectionProvider
    {
        NpgsqlConnection GetConnection();
    }
}
