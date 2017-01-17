
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;

namespace ConsoleApplication384.encoder
{
    public class CustomTextMessageBindingElement : MessageEncodingBindingElement, IWsdlExportExtension
    {
        
        private string mediaType;
        private string encoding;
        private readonly XmlDictionaryReaderQuotas _readerQuotas;
        

        public CustomTextMessageBindingElement()
        {
            _readerQuotas = new XmlDictionaryReaderQuotas();
        }


        #region IMessageEncodingBindingElement Members

        public override MessageEncoderFactory CreateMessageEncoderFactory()
        {
            return new CustomTextMessageEncoderFactory();
        }

        #endregion


        public override MessageVersion MessageVersion
        {
            get { return MessageVersion.Soap11WSAddressing10; }
            set
            {
                
            }
        }

        public override BindingElement Clone()
        {
            return new CustomTextMessageBindingElement();
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.BindingParameters.Add(this);
            return context.BuildInnerChannelFactory<TChannel>();
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return context.CanBuildInnerChannelFactory<TChannel>();
        }

        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.BindingParameters.Add(this);
            return context.BuildInnerChannelListener<TChannel>();
        }

        public override bool CanBuildChannelListener<TChannel>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.BindingParameters.Add(this);
            return context.CanBuildInnerChannelListener<TChannel>();
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (typeof(T) == typeof(XmlDictionaryReaderQuotas))
            {
                return (T)(object)_readerQuotas;
            }
            else
            {
                return base.GetProperty<T>(context);
            }
        }

        #region IWsdlExportExtension Members

        void IWsdlExportExtension.ExportContract(WsdlExporter exporter, WsdlContractConversionContext context)
        {
        }

        void IWsdlExportExtension.ExportEndpoint(WsdlExporter exporter, WsdlEndpointConversionContext context)
        {
            // The MessageEncodingBindingElement is responsible for ensuring that the WSDL has the correct
            // SOAP version. We can delegate to the WCF implementation of TextMessageEncodingBindingElement for this.
            var mebe = new TextMessageEncodingBindingElement
            {
                MessageVersion = MessageVersion
            };
            ((IWsdlExportExtension)mebe).ExportEndpoint(exporter, context);
        }

        #endregion
    }
}
