﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortalClientes.Objetos
{
    [Serializable]
    public class Piloto
    {
        public string codigoPiloto { get; set; }
        public string nombre { get; set; }
        public string tipoDocumentoEsp { get; set; }
        public string tipoDocumentoEng { get; set; }
        public string tipo { get; set; }
        public string numero { get; set; }
        public string fechaExpiracion { get; set; }
        public string lugarTrabajo { get; set; }
        public string pais { get; set; }
        public string estatusEsp { get; set; }
        public string estatusEng { get; set; }
    }

    [Serializable]
    public class EventosPiloto
    {
        public string codigoPiloto { get; set; }
        public string piloto { get; set; }
        public string temaESP { get; set; }
        public string temaENG { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }
        public string descripcion { get; set; }
    }
}