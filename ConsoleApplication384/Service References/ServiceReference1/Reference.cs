﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ConsoleApplication384.ServiceReference1 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceReference1.IServicioCalculadora")]
    public interface IServicioCalculadora {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioCalculadora/SayHello", ReplyAction="http://tempuri.org/IServicioCalculadora/SayHelloResponse")]
        string SayHello(string value);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServicioCalculadoraChannel : ConsoleApplication384.ServiceReference1.IServicioCalculadora, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServicioCalculadoraClient : System.ServiceModel.ClientBase<ConsoleApplication384.ServiceReference1.IServicioCalculadora>, ConsoleApplication384.ServiceReference1.IServicioCalculadora {
        
        public ServicioCalculadoraClient() {
        }
        
        public ServicioCalculadoraClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ServicioCalculadoraClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServicioCalculadoraClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServicioCalculadoraClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string SayHello(string value) {
            return base.Channel.SayHello(value);
        }
    }
}
