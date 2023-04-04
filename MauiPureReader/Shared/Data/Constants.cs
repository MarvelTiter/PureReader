using MDbContext.ExpressionSql.Interface;
using MDbContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Data
{
    internal class InitContext : ExpressionContext
    {
        public override void Initialized(IDbInitial db)
        {
            db.CreateTable<Book>();
        }
    }
    public static class Constants
    {
        public const string DatabaseFilename = "reader.db";

        public static string DbConnectString => $"DataSource={Path.Combine(FileSystem.Current.AppDataDirectory, DatabaseFilename)}";
    }
}
