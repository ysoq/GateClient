using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GateClient.Dto
{
    public class GateInDto
    {
        public string? ticketKind { get; set; }
        public string? code { get; set; }
        public string? password { get; set; }
        public string? flightId { get; set; }
        public string? ticketNo { get; set; }
        public string? idcard { get; set; }
        public string? qrCode { get; set; }
        public string? shipId { get; set; }
        public string? flightShipCode { get; set; }
        public bool? faceVerify { get; set; }
    }
}
