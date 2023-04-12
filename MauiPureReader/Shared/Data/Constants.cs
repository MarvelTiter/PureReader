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
            db.CreateTable<Content>();
        }
    }
    public static class Constants
    {
        public const string DatabaseFilename = "reader.db";

        public static string DbConnectString
        {
            get
            {
                var connStr = $"DataSource={Path.Combine(FileSystem.Current.AppDataDirectory, DatabaseFilename)}";
                return connStr;
            }
        }
    }
}
