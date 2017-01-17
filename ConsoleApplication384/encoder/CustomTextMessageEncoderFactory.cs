
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System.ServiceModel.Channels;

namespace ConsoleApplication384.encoder
{
    public class CustomTextMessageEncoderFactory : MessageEncoderFactory
    {
        internal CustomTextMessageEncoderFactory()
        {            
            Encoder = new CustomTextMessageEncoder(this);
        }

        public override MessageEncoder Encoder { get; }

        public override MessageVersion MessageVersion { get; } = MessageVersion.Soap11WSAddressing10;
    }
}
