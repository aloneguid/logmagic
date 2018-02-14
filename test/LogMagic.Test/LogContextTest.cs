﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LogMagic.Test
{
   public class LogContextTest
   {
      private static readonly ILog log = L.G(typeof(LogContextTest));

      [Fact]
      public void Context_pushes_properties_and_removes_on_exit()
      {
         Assert.Null(L.GetContextValue("p1"));

         using (L.Context("p1", "v1"))
         {
            Assert.Equal("v1", L.GetContextValue("p1"));
         }

         Assert.Null(L.GetContextValue("p1"));
      }
   }
}
