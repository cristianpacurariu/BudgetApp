﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Budget.DataAccess.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SpendingsEntities : DbContext
    {
        public SpendingsEntities()
            : base("name=SpendingsEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Operation> Operations { get; set; }
        public virtual DbSet<OperationType> OperationTypes { get; set; }
    }
}
