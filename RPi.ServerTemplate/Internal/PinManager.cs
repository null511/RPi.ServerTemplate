using Raspberry.IO.GeneralPurpose;
using System;

namespace RPiServerTemplate.Internal
{
    internal class PinManager : IDisposable
    {
        private GpioConnection connection;
        private OutputPinConfiguration pinConfig;


        public PinManager() {}

        public void Connect()
        {
            pinConfig = ConnectorPin.P1Pin18.Output();
            connection = new GpioConnection(pinConfig);
        }

        public void Blink(TimeSpan duration)
        {
            if (connection == null)
                throw new ApplicationException("Pin Manager is not connected!");

            connection.Blink(pinConfig, duration);
        }

        public void Dispose()
        {
            connection?.Close();
        }
    }
}
