﻿using System;

namespace RPC.Contract
{
    public class Hello : IHello
    {
        public string SayHello(string msg)
        {
            return msg;
        }
    }
}
