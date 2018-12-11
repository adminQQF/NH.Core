﻿using System;

//Nhibernate Code Generation Template 1.0
//author:Tibos
//blog:www.cnblogs.com/Tibos
//Entity Code Generation Template
namespace Tibos.Domain
{
	 	//Manager
		public class Manager
	{
      	/// <summary>
		/// 主键编号
        /// </summary>
        public virtual string Id
        {
            get; 
            set; 
        }        
		/// <summary>
		/// 用户名
        /// </summary>
        public virtual string UserName
        {
            get; 
            set; 
        }        
		/// <summary>
		/// 密码
        /// </summary>
        public virtual string Password
        {
            get; 
            set; 
        }        
		/// <summary>
		/// 手机号
        /// </summary>
        public virtual string Mobile
        {
            get; 
            set; 
        }        
		/// <summary>
		/// 邮箱
        /// </summary>
        public virtual string Email
        {
            get; 
            set; 
        }        
		/// <summary>
		/// 状态(0:正常,-1禁用)
        /// </summary>
        public virtual int Status
        {
            get; 
            set; 
        }        
		/// <summary>
		/// 最后登录时间
        /// </summary>
        public virtual DateTime LoginTime
        {
            get; 
            set; 
        }        
		/// <summary>
		/// 登录IP
        /// </summary>
        public virtual string LoginIp
        {
            get; 
            set; 
        }        
		   
	}
}