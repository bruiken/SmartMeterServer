﻿using Microsoft.EntityFrameworkCore;

namespace Data.Models
{
    public class Permission
    {
        public int RoleId { get; set; }

        public bool CanModifyUsers { get; set; }
        public bool CanModifySettings { get; set; }
        public bool CanModifyInstallations { get; set; }
    }

    static class PermissionMeta
    {
        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permission>()
                .HasKey(c => c.RoleId);
        }
    }
}
