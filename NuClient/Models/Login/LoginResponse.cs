﻿using QRCoder;
using System.Drawing;

namespace NuClient.Models.Login
{
    public class LoginResponse
    {
        public LoginResponse()
        {
            NeedsDeviceAuthorization = false;
        }

        public LoginResponse(string code)
        {
            NeedsDeviceAuthorization = true;
            Code = code;

            var qrGenerator = new QRCodeGenerator();
            _qrCodeData = qrGenerator.CreateQrCode(Code, QRCodeGenerator.ECCLevel.Q);
        }

        private readonly QRCodeData _qrCodeData;

        public bool NeedsDeviceAuthorization { get; }
        public string Code { get; }

        public string GetQrCodeAsAscii()
        {
            var qrCode = new AsciiQRCode(_qrCodeData);
            return qrCode.GetGraphic(1);
        }
    }
}