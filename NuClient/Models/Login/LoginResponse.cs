using QRCoder;

namespace NuClient.Models.Login
{
	public class LoginResponse
    {
        public LoginResponse(bool mustAuthenticate)
        {
            MustAuthenticate = mustAuthenticate;
            if(MustAuthenticate)
            {
				Code = Guid.NewGuid().ToString();

				var qrGenerator = new QRCodeGenerator();
				_qrCodeData = qrGenerator.CreateQrCode(Code, QRCodeGenerator.ECCLevel.Q);
			}
        }

        private readonly QRCodeData _qrCodeData;

        public bool MustAuthenticate { get; }
        public string Code { get; }

        public string GetQrCodeAsAscii()
        {
            var qrCode = new AsciiQRCode(_qrCodeData);
            return qrCode.GetGraphic(1);
        }
    }
}
