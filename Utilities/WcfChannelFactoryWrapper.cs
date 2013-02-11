using System;
using System.ServiceModel;

namespace org.theGecko.Utilities
{
	public class WcfChannelFactoryWrapper<T> : IDisposable where T : class
	{
		private readonly ChannelFactory<T> _factory;
		public T Channel
		{
			get; internal set;
		}

        public WcfChannelFactoryWrapper(string endpointConfigurationName, EndpointAddress remoteAddress)
        {
            _factory = new ChannelFactory<T>(endpointConfigurationName, remoteAddress);
            Initialize();
        }

		public WcfChannelFactoryWrapper(string endpointConfigurationName)
		{
			_factory = new ChannelFactory<T>(endpointConfigurationName);
		    Initialize();
		}

        private void Initialize()
        {
			Channel = _factory.CreateChannel();            
        }

        public T CreateChannel(string endPoint)
        {
            return _factory.CreateChannel(new EndpointAddress(endPoint));
        }

        public T CreateChannel()
        {
            return _factory.CreateChannel();
        }

        public void DestroyChannel(T channel)
        {
            if (channel != null && channel is IClientChannel)
            {
                IClientChannel clientChannel = (channel as IClientChannel);
                try
                {
                    if (clientChannel.State == CommunicationState.Faulted)
                    {
                        clientChannel.Abort();
                    }
                    else if (clientChannel.State != CommunicationState.Closed)
                    {
                        clientChannel.Close();
                    }
                }
                catch (CommunicationException)
                {
                    clientChannel.Abort();
                }
                catch (TimeoutException)
                {
                    clientChannel.Abort();
                }
                finally
                {
                    clientChannel.Dispose();
                    channel = null;
                }
            }
        }

		#region IDisposable Members

		public void Dispose()
		{
		    DestroyChannel(Channel);
		
			try
			{
				if (_factory.State == CommunicationState.Faulted)
				{
					_factory.Abort();
				}
				else if (_factory.State != CommunicationState.Closed)
				{
					_factory.Close();
				}
			}
			catch (CommunicationException)
			{
				_factory.Abort();
			}
			catch (TimeoutException)
			{
				_factory.Abort();
			}
            finally
            {
                ((IDisposable)_factory).Dispose();
            }
		}

		#endregion
	}
}
