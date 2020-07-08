﻿using Fur.DatabaseVisitor.TenantSaaS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq;

namespace Fur.DatabaseVisitor.Contexts
{
    /// <summary>
    /// 框架内自定义 DbContext 类
    /// <code>abstract</code>
    /// <para>所有操作数据库的上下文都需继承 <see cref="FurDbContextOfT{TDbContext}"/></para>
    /// <para>该类默认继承 <see cref="DbContext"/></para>
    /// </summary>
    /// <typeparam name="TDbContext"><see cref="DbContext"/> 类型</typeparam>
    public abstract class FurDbContextOfT<TDbContext> : DbContext where TDbContext : DbContext
    {
        /// <summary>
        /// 租户实体表
        /// <para>框架默认启用了租户模式。参见：<see cref="Tenant"/></para>
        /// </summary>
        public virtual DbSet<Tenant> Tenants { get; set; }

        /// <summary>
        /// 租户提供器
        /// </summary>
        private ITenantProvider TenantProvider;

        /// <summary>
        /// 租户Id属性
        /// <para>支持子类重写</para>
        /// </summary>
        protected virtual int TenantId
        {
            get
            {
                TenantProvider ??= this.GetService<ITenantProvider>();
                return TenantProvider.GetTenantId();
            }
        }

        #region 默认构造函数 + public FurDbContextOfT(DbContextOptions<TDbContext> options) : base(options)
        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="options">DbContext上下文配置选项</param>
        public FurDbContextOfT(DbContextOptions<TDbContext> options) : base(options)
        {
        }
        #endregion

        #region DbContext上下文初始化配置时调用的方法 + protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        /// <summary>
        /// DbContext上下文初始化配置时调用的方法
        /// <para>可在这里配置数据库连接字符串，数据库提供器等</para>
        /// </summary>
        /// <param name="optionsBuilder">DbContext上下文配置选项构建器</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (FurDbContextOfTStatus.CallOnConfiguringed()) return;
        }
        #endregion

        #region DbContext 上下文初始化模型时调用的方法 + protected override void OnModelCreating(ModelBuilder modelBuilder)
        /// <summary>
        /// DbContext 上下文初始化模型时调用的方法
        /// </summary>
        /// <param name="modelBuilder">DbContext 上下文模型构建器</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            if (FurDbContextOfTStatus.CallOnModelCreatinged()) return;

            // 初始化租户实例表
            // 备注：后续 HasData迁移单独抽离出来，避免影响正常运行的代码
            //           只存在于数据库迁移命令中有效
            modelBuilder.Entity<Tenant>().HasData(
                new Tenant() { Id = 1, Name = "默认租户", Host = "localhost:44307" },
                new Tenant() { Id = 2, Name = "默认租户", Host = "localhost:41529" }
             );

            // 扫描数据库编译实体
            ScanDbCompileEntityToCreateModelEntity(modelBuilder);
        }
        #endregion

        #region DbContext 上下文扫描配置数据库编译实体调用方法 + protected virtual void ScanDbCompileEntityToCreateModelEntity(ModelBuilder modelBuilder)
        /// <summary>
        /// DbContext 上下文扫描配置数据库编译实体调用方法
        /// <para>该方法在 <see cref="OnModelCreating(ModelBuilder)"/> 中调用</para>
        /// </summary>
        /// <param name="modelBuilder">模型构建器</param>
        protected virtual void ScanDbCompileEntityToCreateModelEntity(ModelBuilder modelBuilder)
        {
            FurDbContextOfTStatus.ScanDbCompileEntityToCreateModelEntity(modelBuilder, nameof(TenantId), TenantId);
        }
        #endregion

        #region DbContext 上下文获取租户Id + public virtual int GetTenantId(string host)
        /// <summary>
        /// DbContext 上下文获取租户Id
        /// </summary>
        /// <param name="host">主机地址</param>
        /// <returns>int</returns>
        public virtual int GetTenantId(string host)
        {
            var tenant = Tenants.FirstOrDefault(t => t.Host == host);
            return tenant?.Id ?? 0;
        }
        #endregion
    }
}