
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System.ServiceModel.Channels;

namespace ConsoleApplication384.encoder
{
    public class CustomTextMessageEncoderFactory : MessageEncoderFactory
    {
        private MessageEncoder encoder;
        
        internal CustomTextMessageEncoderFactory()
        {            
            this.encoder = new CustomTextMessageEncoder(this);
        }

        public override MessageEncoder Encoder
        {
            get 
            { 
                return this.encoder;
            }
        }

        public override MessageVersion MessageVersion
        {
            get 
            {
                return MessageVersion.Soap11WSAddressing10;
            }
        }

        
    }
}
