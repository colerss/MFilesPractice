using GameVaultApp.Data;
using MFaaP.MFWSClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameVaultApp.ViewModels
{
    public class TestViewModel
    {
        public List<ExtendedObjectVersion> Objects { get; set; }
        public int ObjectSearchType { get; set; }
        public string ObjectSearchTitle { get; set; }

       
    }
}
