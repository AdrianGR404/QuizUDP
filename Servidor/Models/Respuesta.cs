﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servidor.Models
{
    public class Respuesta
    {
        public string nombre { get; set; }
        public string respuesta { get; set; }
        public string EndpointId { get; set; } = ""; 

    }
}
