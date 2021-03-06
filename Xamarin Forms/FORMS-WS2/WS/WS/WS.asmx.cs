﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace WS
{
    /// <summary>
    /// Summary description for WS
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WS : System.Web.Services.WebService
    {

        public class EstruturaDados
        {
            public int ID { get; set; }
            public String Nome { get; set; }
            public DateTime Data { get; set; }
        }

        [WebMethod]
        public List<EstruturaDados> Listar()
        {
            return new List<EstruturaDados>()
            {
                new EstruturaDados() { ID=1, Nome="Nome 1", Data=DateTime.Now },
                new EstruturaDados() { ID=2, Nome="Nome 2", Data=DateTime.Now }
            };
        }
        [WebMethod]
        public EstruturaDados Gravar(EstruturaDados Dados)
        {
            return Dados;
        }
    }
}
