using MDbContext;
using MDbContext.ExpressionSql.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiPureReader.Data
{
    internal class InitContext : ExpressionContext
    {
        public override void Initialized(IDbInitial db)
        {
            db.CreateTable<Book>();
        }
    }
}
