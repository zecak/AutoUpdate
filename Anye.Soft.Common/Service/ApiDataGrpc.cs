// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: ApiData.proto
// </auto-generated>
#pragma warning disable 0414, 1591
#region Designer generated code

using grpc = global::Grpc.Core;

namespace AnyeSoft.Common.Service {
  public static partial class gRPC
  {
    static readonly string __ServiceName = "AnyeSoft.Common.Service.gRPC";

    static void __Helper_SerializeMessage(global::Google.Protobuf.IMessage message, grpc::SerializationContext context)
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (message is global::Google.Protobuf.IBufferMessage)
      {
        context.SetPayloadLength(message.CalculateSize());
        global::Google.Protobuf.MessageExtensions.WriteTo(message, context.GetBufferWriter());
        context.Complete();
        return;
      }
      #endif
      context.Complete(global::Google.Protobuf.MessageExtensions.ToByteArray(message));
    }

    static class __Helper_MessageCache<T>
    {
      public static readonly bool IsBufferMessage = global::System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(global::Google.Protobuf.IBufferMessage)).IsAssignableFrom(typeof(T));
    }

    static T __Helper_DeserializeMessage<T>(grpc::DeserializationContext context, global::Google.Protobuf.MessageParser<T> parser) where T : global::Google.Protobuf.IMessage<T>
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (__Helper_MessageCache<T>.IsBufferMessage)
      {
        return parser.ParseFrom(context.PayloadAsReadOnlySequence());
      }
      #endif
      return parser.ParseFrom(context.PayloadAsNewBuffer());
    }

    static readonly grpc::Marshaller<global::AnyeSoft.Common.Service.APIRequest> __Marshaller_AnyeSoft_Common_Service_APIRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::AnyeSoft.Common.Service.APIRequest.Parser));
    static readonly grpc::Marshaller<global::AnyeSoft.Common.Service.APIReply> __Marshaller_AnyeSoft_Common_Service_APIReply = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::AnyeSoft.Common.Service.APIReply.Parser));

    static readonly grpc::Method<global::AnyeSoft.Common.Service.APIRequest, global::AnyeSoft.Common.Service.APIReply> __Method_Exec = new grpc::Method<global::AnyeSoft.Common.Service.APIRequest, global::AnyeSoft.Common.Service.APIReply>(
        grpc::MethodType.Unary,
        __ServiceName,
        "Exec",
        __Marshaller_AnyeSoft_Common_Service_APIRequest,
        __Marshaller_AnyeSoft_Common_Service_APIReply);

    static readonly grpc::Method<global::AnyeSoft.Common.Service.APIRequest, global::AnyeSoft.Common.Service.APIReply> __Method_Chat = new grpc::Method<global::AnyeSoft.Common.Service.APIRequest, global::AnyeSoft.Common.Service.APIReply>(
        grpc::MethodType.DuplexStreaming,
        __ServiceName,
        "Chat",
        __Marshaller_AnyeSoft_Common_Service_APIRequest,
        __Marshaller_AnyeSoft_Common_Service_APIReply);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::AnyeSoft.Common.Service.ApiDataReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of gRPC</summary>
    [grpc::BindServiceMethod(typeof(gRPC), "BindService")]
    public abstract partial class gRPCBase
    {
      /// <summary>
      ///简单rpc
      /// </summary>
      /// <param name="request">The request received from the client.</param>
      /// <param name="context">The context of the server-side call handler being invoked.</param>
      /// <returns>The response to send back to the client (wrapped by a task).</returns>
      public virtual global::System.Threading.Tasks.Task<global::AnyeSoft.Common.Service.APIReply> Exec(global::AnyeSoft.Common.Service.APIRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      /// <summary>
      ///双向流rpc
      /// </summary>
      /// <param name="requestStream">Used for reading requests from the client.</param>
      /// <param name="responseStream">Used for sending responses back to the client.</param>
      /// <param name="context">The context of the server-side call handler being invoked.</param>
      /// <returns>A task indicating completion of the handler.</returns>
      public virtual global::System.Threading.Tasks.Task Chat(grpc::IAsyncStreamReader<global::AnyeSoft.Common.Service.APIRequest> requestStream, grpc::IServerStreamWriter<global::AnyeSoft.Common.Service.APIReply> responseStream, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for gRPC</summary>
    public partial class gRPCClient : grpc::ClientBase<gRPCClient>
    {
      /// <summary>Creates a new client for gRPC</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      public gRPCClient(grpc::ChannelBase channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for gRPC that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      public gRPCClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      protected gRPCClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      protected gRPCClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      /// <summary>
      ///简单rpc
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The response received from the server.</returns>
      public virtual global::AnyeSoft.Common.Service.APIReply Exec(global::AnyeSoft.Common.Service.APIRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return Exec(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      ///简单rpc
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="options">The options for the call.</param>
      /// <returns>The response received from the server.</returns>
      public virtual global::AnyeSoft.Common.Service.APIReply Exec(global::AnyeSoft.Common.Service.APIRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_Exec, null, options, request);
      }
      /// <summary>
      ///简单rpc
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The call object.</returns>
      public virtual grpc::AsyncUnaryCall<global::AnyeSoft.Common.Service.APIReply> ExecAsync(global::AnyeSoft.Common.Service.APIRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return ExecAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      ///简单rpc
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="options">The options for the call.</param>
      /// <returns>The call object.</returns>
      public virtual grpc::AsyncUnaryCall<global::AnyeSoft.Common.Service.APIReply> ExecAsync(global::AnyeSoft.Common.Service.APIRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_Exec, null, options, request);
      }
      /// <summary>
      ///双向流rpc
      /// </summary>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The call object.</returns>
      public virtual grpc::AsyncDuplexStreamingCall<global::AnyeSoft.Common.Service.APIRequest, global::AnyeSoft.Common.Service.APIReply> Chat(grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return Chat(new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      ///双向流rpc
      /// </summary>
      /// <param name="options">The options for the call.</param>
      /// <returns>The call object.</returns>
      public virtual grpc::AsyncDuplexStreamingCall<global::AnyeSoft.Common.Service.APIRequest, global::AnyeSoft.Common.Service.APIReply> Chat(grpc::CallOptions options)
      {
        return CallInvoker.AsyncDuplexStreamingCall(__Method_Chat, null, options);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      protected override gRPCClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new gRPCClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static grpc::ServerServiceDefinition BindService(gRPCBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_Exec, serviceImpl.Exec)
          .AddMethod(__Method_Chat, serviceImpl.Chat).Build();
    }

    /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the  service binding logic.
    /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
    /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static void BindService(grpc::ServiceBinderBase serviceBinder, gRPCBase serviceImpl)
    {
      serviceBinder.AddMethod(__Method_Exec, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::AnyeSoft.Common.Service.APIRequest, global::AnyeSoft.Common.Service.APIReply>(serviceImpl.Exec));
      serviceBinder.AddMethod(__Method_Chat, serviceImpl == null ? null : new grpc::DuplexStreamingServerMethod<global::AnyeSoft.Common.Service.APIRequest, global::AnyeSoft.Common.Service.APIReply>(serviceImpl.Chat));
    }

  }
}
#endregion
