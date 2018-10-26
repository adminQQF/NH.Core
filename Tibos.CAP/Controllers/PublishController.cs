﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Tibos.CAP.Models;
using Tibos.Domain;

namespace Tibos.CAP.Controllers
{
    [Produces("application/json")]
    [Route("api/Publish")]
    public class PublishController : Controller
    {

        private readonly ICapPublisher _capBus;

        public PublishController(ICapPublisher capPublisher)
        {
            _capBus = capPublisher;
        }

        [Route("~/adonet/transaction")]
        public IActionResult AdonetWithTransaction()
        {
            string ConnectionString = "Data Source=132.232.4.73;Initial Catalog=666;port=3306; User ID=root;Password=123456;SslMode = none;";
            using (var connection = new MySqlConnection(ConnectionString))
            {
                using (var transaction = connection.BeginTransaction(_capBus,autoCommit:false))
                {
                    connection.Execute("insert into test(id) values(1)", transaction: (IDbTransaction)transaction.DbTransaction);
                    //your business code
                    Users user = new Users();
                    user.address = "XX";
                    _capBus.Publish("kkk.services.bar", user, "callback-show-execute-time");
                    transaction.Commit();
                    //transaction.Rollback();
                }
            }

            return Ok();
        }

        [CapSubscribe("callback-show-execute-time")]
        public void BarMessageProcessor(Users user)
        {

        }
    }
}